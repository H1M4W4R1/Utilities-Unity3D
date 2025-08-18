using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;

namespace Systems.Utilities.Collections
{
    [BurstCompile]
    public static class ArrayUtil
    {
        /// <summary>
        ///     Allocates array if length has changed, otherwise leaves old array to be cleaned up.
        /// </summary>
        [BurstCompile]
        public static void PerformEfficientAllocation<TDataType>(
            ref NativeArray<TDataType> source,
            int nLength,
            Allocator allocator)
            where TDataType : struct
        {
            if (Hint.Unlikely(!source.IsCreated))
            {
                source = new NativeArray<TDataType>(nLength, allocator);
                return;
            }

            if (Hint.Likely(source.Length == nLength)) return;

            source.Dispose();
            source = new NativeArray<TDataType>(nLength, allocator);
        }
        
    }
}