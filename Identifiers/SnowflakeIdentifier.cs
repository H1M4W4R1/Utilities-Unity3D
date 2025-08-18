using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Identifiers.Abstract;
using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Identifiers
{
    /// <summary>
    ///     Time-based identifier for unique identification of objects.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct SnowflakeIdentifier : IIdentifier<SnowflakeIdentifier>
    {
        /// <summary>
        ///     Internal shift counter to prevent duplication of IDs created at same time.
        /// </summary>
        private static int _snowflakeShiftCounter;

        [FieldOffset(0)] public readonly int4 vectorized;

        [FieldOffset(0)] public readonly long ticks;
        [FieldOffset(8)] public readonly int shift;
        [FieldOffset(12)] public readonly short reserved0;
        [FieldOffset(14)] public readonly byte reserved1;
        
        [MarshalAs(UnmanagedType.U1)]
        [FieldOffset(15)] public readonly bool isCreated;

        public SnowflakeIdentifier(long ticks, int shift)
        {
            vectorized = int4.zero;
            this.ticks = ticks;
            this.shift = shift;
            reserved0 = 0;
            reserved1 = 0;
            isCreated = true;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SnowflakeIdentifier other) => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
        {
            if (obj is SnowflakeIdentifier other) return Equals(other);
            if (obj is null) return !isCreated;
            return false;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SnowflakeIdentifier left, SnowflakeIdentifier right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SnowflakeIdentifier left, SnowflakeIdentifier right) => !left.Equals(right);

        /// <summary>
        ///     Create new instance of SnowflakeIdentifier
        /// </summary>
        /// <remarks>
        ///     Available only in Mono, not burstable due to usage of <see cref="DateTime.UtcNow"/>
        /// </remarks>
        [BurstDiscard] public static SnowflakeIdentifier New()
            => new(DateTime.UtcNow.Ticks, _snowflakeShiftCounter++);
        
        // IComparable implementation (using operators you defined)
        [BurstCompile] 
        public int CompareTo(SnowflakeIdentifier other)
        {
            if (Equals(other)) return 0;

            if (ticks < other.ticks) return -1;
            if (ticks > other.ticks) return 1;
            if (shift < other.shift) return -1;
            if (shift > other.shift) return 1;
            
            return 0; // Weird shit, consider equal
        }
    }
}