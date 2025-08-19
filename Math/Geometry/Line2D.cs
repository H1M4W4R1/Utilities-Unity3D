using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Math.Geometry
{
    /// <summary>
    ///     Line in 3D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly struct Line2D : IUnmanaged<Line2D>, IEquatable<Line2D>
    {
        [FieldOffset(0)] private readonly int4 vectorized;

        [FieldOffset(0)] public readonly float2 start;
        [FieldOffset(12)] public readonly float2 end;

        public Line2D(float2 start, float2 end)
        {
            vectorized = int4.zero;
            this.start = start;
            this.end = end;
        }
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float2 GetClosestPointToPoint(in float2 target)
        {
            float2 lineDirection = math.normalize(end - start);
            float2 v = target - start;
            float distance = math.dot(v, lineDirection);
            return start + lineDirection * distance;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float2 GetSymmetricPoint(in float2 point)
        {
            float2 pointOnLine = GetClosestPointToPoint(point);
            float2 vectorToPoint = point - pointOnLine;
            return pointOnLine - vectorToPoint;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CrossesLine(in Line2D otherLine) => !otherLine.ArePointsOnSameSide(start, end);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ArePointsOnSameSide(in float2 a, in float2 b)
        {
            float2 lineDirection = end - start;
            float2 normal = new(-lineDirection.y, lineDirection.x); // Perpendicular vector
            return math.dot(normal, a - start) * math.dot(normal, b - start) > 0;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float Length()
            => math.distance(start, end);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float LengthSq()
            => math.distancesq(start, end);


#region IEquatable<Line3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Line2D other)
            => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Line2D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Line2D left, in Line2D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Line2D left, in Line2D right) => !left.Equals(right);

#endregion
    }
}