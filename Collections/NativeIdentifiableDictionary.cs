using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Systems.Utilities.Collections
{
    /// <summary>
    ///     Identifier-sorted dictionary of identifiers and objects with a quick lookup query
    ///     using binary tree search algorithm
    /// </summary>
    [BurstCompile]
    public struct NativeIdentifiableDictionary<TIdentifier, TObject> : IDictionary<TIdentifier, TObject>,
        INativeDisposable
        where TIdentifier : unmanaged, IComparable<TIdentifier>
        where TObject : unmanaged
    {
        private UnsafeList<TIdentifier> _keys;
        private UnsafeList<TObject> _values;

        public bool IsReadOnly => false;

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get
                => math.min(_keys.Length, _values.Length);
        }


        public ICollection<TIdentifier> Keys => throw new NotSupportedException();
        public ICollection<TObject> Values => throw new NotSupportedException();

        public NativeIdentifiableDictionary(Allocator allocator)
        {
            _keys = new UnsafeList<TIdentifier>(64, allocator);
            _values = new UnsafeList<TObject>(64, allocator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Dispose()
        {
            _keys.Dispose();
            _values.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            Dispose();
            return inputDeps;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TIdentifier, TObject>> GetEnumerator()
        {
            Assert.AreEqual(_keys.Length, _values.Length);
            for (int nIndex = 0; nIndex < Count; nIndex++)
            {
                yield return new KeyValuePair<TIdentifier, TObject>(_keys[nIndex], _values[nIndex]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();


#region IDictionary<K,V> — implemented

        [BurstDiscard] public void Add(KeyValuePair<TIdentifier, TObject> item) => Add(item.Key, item.Value);

        [BurstDiscard] public bool Contains(KeyValuePair<TIdentifier, TObject> item)
        {
            int idx = BinarySearch(item.Key);
            if (idx < 0) return false;
            return EqualityComparer<TObject>.Default.Equals(_values[idx], item.Value);
        }

        [BurstDiscard] public void CopyTo(KeyValuePair<TIdentifier, TObject>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if ((uint) arrayIndex > (uint) array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Destination array is too small.");

            for (int i = 0; i < Count; i++)
                array[arrayIndex + i] = new KeyValuePair<TIdentifier, TObject>(_keys[i], _values[i]);
        }

        [BurstDiscard] public bool Remove(KeyValuePair<TIdentifier, TObject> item)
        {
            int idx = BinarySearch(item.Key);
            if (idx < 0) return false;
            if (!EqualityComparer<TObject>.Default.Equals(_values[idx], item.Value)) return false;

            RemoveAtOrdered(idx);
            return true;
        }

        [BurstCompile] public void Add(TIdentifier key, TObject value)
        {
            int idx = BinarySearch(key);
            if (idx >= 0) return;

            int insertIndex = ~idx;
            InsertAtOrdered(insertIndex, key, value);
        }

        public bool ContainsKey(TIdentifier key) => BinarySearch(key) >= 0;

        [BurstCompile] public bool Remove(TIdentifier key)
        {
            int idx = BinarySearch(key);
            if (idx < 0) return false;
            RemoveAtOrdered(idx);
            return true;
        }

        [BurstCompile] public bool TryGetValue(TIdentifier key, out TObject value)
        {
            int idx = BinarySearch(key);
            if (idx >= 0)
            {
                value = _values[idx];
                return true;
            }

            value = default;
            return false;
        }

        public TObject this[TIdentifier key]
        {
            [BurstCompile] get
            {
                int idx = BinarySearch(key);
                if (idx < 0) return default;
                return _values[idx];
            }

            [BurstCompile] set
            {
                int idx = BinarySearch(key);
                if (idx >= 0)
                {
                    _values[idx] = value; // update existing
                }
                else
                {
                    InsertAtOrdered(~idx, key, value); // insert new, preserving sort
                }
            }
        }

        [BurstCompile] public void Clear()
        {
            _keys.Clear();
            _values.Clear();
        }

#endregion

#region Internals — binary search & ordered insert/remove

        // Standard binary search over sorted _keys.
        // Returns index if found; otherwise bitwise complement of insertion index (~insertionIndex).
        [BurstCompile] private int BinarySearch(TIdentifier key)
        {
            int lo = 0;
            int hi = Count - 1;

            while (lo <= hi)
            {
                int mid = lo + ((hi - lo) >> 1);
                TIdentifier midKey = _keys[mid];

                // Rely on user-provided operators for TIdentifier
                int cmp = midKey.CompareTo(key);
                if (cmp == 0) return mid;

                if (cmp < 0)
                    lo = mid + 1;
                else
                    hi = mid - 1;
            }

            return ~lo;
        }

        [BurstCompile] private void InsertAtOrdered(int index, TIdentifier key, TObject value)
        {
            // Grow by one at the end, then memmove the tail up by 1 to create a gap.
            _keys.Add(default);
            _values.Add(default);

            int countBefore = Count - 1; // previous count before Add(default)
            int moveCount = countBefore - index; // elements to move up

            unsafe
            {
                if (moveCount > 0)
                {
                    // Keys
                    TIdentifier* keyPtr = _keys.Ptr;
                    int keySize = UnsafeUtility.SizeOf<TIdentifier>();
                    UnsafeUtility.MemMove(
                        destination: keyPtr + index + 1,
                        source: keyPtr + index,
                        size: moveCount * keySize);

                    // Values
                    TObject* valPtr = _values.Ptr;
                    int valSize = UnsafeUtility.SizeOf<TObject>();
                    UnsafeUtility.MemMove(
                        destination: valPtr + index + 1,
                        source: valPtr + index,
                        size: moveCount * valSize);
                }

                // Write new pair into the gap
                _keys[index] = key;
                _values[index] = value;
            }
        }

        [BurstCompile] private void RemoveAtOrdered(int index)
        {
            int count = Count;
            int moveCount = (count - 1) - index;
            if (moveCount > 0)
            {
                unsafe
                {
                    // Keys
                    TIdentifier* keyPtr = _keys.Ptr;
                    int keySize = UnsafeUtility.SizeOf<TIdentifier>();
                    UnsafeUtility.MemMove(
                        destination: keyPtr + index,
                        source: keyPtr + index + 1,
                        size: moveCount * keySize);

                    // Values
                    TObject* valPtr = _values.Ptr;
                    int valSize = UnsafeUtility.SizeOf<TObject>();
                    UnsafeUtility.MemMove(
                        destination: valPtr + index,
                        source: valPtr + index + 1,
                        size: moveCount * valSize);
                }
            }

            // Shrink logical length by 1 (keep capacity)
            _keys.Length = count - 1;
            _values.Length = count - 1;
        }

#endregion
    }
}