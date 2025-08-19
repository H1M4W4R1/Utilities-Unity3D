using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Segment of a line in 3D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly struct Segment3D : IUnmanaged<Segment3D>, IEquatable<Segment3D>
    {
        /// <summary>
        ///     Vectorized data
        /// </summary>
        [FieldOffset(0)] private readonly int3x2 vectorized;

        /// <summary>
        ///     Start point of the line segment
        /// </summary>
        [FieldOffset(0)] public readonly Point3D start;

        /// <summary>
        ///     End point of the line segment
        /// </summary>
        [FieldOffset(12)] public readonly Point3D end;

        /// <summary>
        ///     Constructs a new line segment from the given start and end points.
        /// </summary>
        /// <param name="start">Start point of the line segment.</param>
        /// <param name="end">End point of the line segment.</param>
        public Segment3D(in Point3D start, in Point3D end)
        {
            vectorized = int3x2.zero;
            this.start = start;
            this.end = end;
        }

        /// <summary>
        ///     Finds the closest point to the given target point on the line segment.
        /// </summary>
        /// <param name="point">Target point to find the closest point from.</param>
        /// <returns>Closest point on the line to the target.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point3D GetClosestPoint(in Point3D point)
        {
            Direction3D lineDirection = math.normalize((float3) (end - start));
            Offset3D offset = point - start;
            float distance = math.dot((float3) offset, (float3) lineDirection);
            return start + lineDirection * distance;
        }

        /// <summary>
        ///     Computes the symmetric point to the given point relative to this line.
        /// </summary>
        /// <param name="point">Point to compute the symmetric point of.</param>
        /// <returns>Symmetric point to the given point relative to this line.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point3D GetSymmetricPoint(in Point3D point)
        {
            Point3D pointOnLine = GetClosestPoint(point);
            Offset3D vectorToPoint = point - pointOnLine;
            return pointOnLine - vectorToPoint;
        }

        /// <summary>
        ///     Computes whether this line crosses the given <paramref name="plane"/>.
        /// </summary>
        /// <param name="plane">Plane to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CrossesPlane(in Plane3D plane) => !plane.ArePointsOnSameSide(start, end);

        /// <summary>
        ///     Computes the length of the line segment.
        /// </summary>
        /// <returns>Length of the line segment.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public float Length()
            => math.distance((float3) start, (float3) end);

        /// <summary>
        ///     Computes the length of the line segment squared.
        /// </summary>
        /// <returns>Length of the line segment squared.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public float LengthSq()
            => math.distancesq((float3)start, (float3) end);

#region IEquatable<Line3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Segment3D other)
            => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Segment3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Segment3D left, in Segment3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Segment3D left, in Segment3D right) => !left.Equals(right);

#endregion
    }
}