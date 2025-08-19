using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Math.Geometry2D
{
    /// <summary>
    ///     Segment of a line in 2D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly struct Segment2D : IUnmanaged<Segment2D>, IEquatable<Segment2D>
    {
        /// <summary>
        ///     Vectorized data
        /// </summary>
        [FieldOffset(0)] private readonly int4 vectorized;

        /// <summary>
        ///     Start point of the line
        /// </summary>
        [FieldOffset(0)] public readonly float2 start;
        
        /// <summary>
        ///     End point of the line
        /// </summary>
        [FieldOffset(12)] public readonly float2 end;

        /// <summary>
        ///     Constructs a new line segment from the given start and end points.
        /// </summary>
        /// <param name="start">Start point of the line segment.</param>
        /// <param name="end">End point of the line segment.</param>
        public Segment2D(in float2 start, in float2 end)
        {
            vectorized = int4.zero;
            this.start = start;
            this.end = end;
        }
        
        /// <summary>
        ///     Finds the closest point to the given target point on the line segment.
        /// </summary>
        /// <param name="target">Target point to find the closest point from.</param>
        /// <returns>Closest point on the line to the target.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float2 GetClosestPointToPoint(in float2 target)
        {
            float2 lineDirection = math.normalize(end - start);
            float2 v = target - start;
            float distance = math.dot(v, lineDirection);
            return start + lineDirection * distance;
        }

        /// <summary>
        ///     Computes the symmetric point to the given point relative to this line.
        /// </summary>
        /// <param name="point">Point to compute the symmetric point of.</param>
        /// <returns>Symmetric point to the given point relative to this line.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float2 GetSymmetricPoint(in float2 point)
        {
            float2 pointOnLine = GetClosestPointToPoint(point);
            float2 vectorToPoint = point - pointOnLine;
            return pointOnLine - vectorToPoint;
        }

        /// <summary>
        ///     Computes whether this line crosses the given <paramref name="otherSegment"/>.
        /// </summary>
        /// <param name="otherSegment">Line to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two lines intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CrossesLine(in Segment2D otherSegment) => !otherSegment.ArePointsOnSameSide(start, end);

        
        /// <summary>
        ///     Checks whether two points are on the same side of this line.
        /// </summary>
        /// <param name="a">First point to check.</param>
        /// <param name="b">Second point to check.</param>
        /// <returns>
        ///     <see langword="true"/> if both points are on the same side of this line, <see langword="false"/> otherwise.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ArePointsOnSameSide(in float2 a, in float2 b)
        {
            float2 lineDirection = end - start;
            float2 normal = new(-lineDirection.y, lineDirection.x); // Perpendicular vector
            return math.dot(normal, a - start) * math.dot(normal, b - start) > 0;
        }

        /// <summary>
        ///     Computes the length of the line segment.
        /// </summary>
        /// <returns>Length of the line segment.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float Length()
            => math.distance(start, end);

        /// <summary>
        ///     Computes the length of the line segment squared.
        /// </summary>
        /// <returns>Length of the line segment squared.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float LengthSq()
            => math.distancesq(start, end);
      

#region IEquatable<Line3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Segment2D other)
            => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Segment2D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Segment2D left, in Segment2D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Segment2D left, in Segment2D right) => !left.Equals(right);

#endregion
    }
}