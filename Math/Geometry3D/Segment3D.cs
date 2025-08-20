using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

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
        /// <param name="closestPoint">Closest point on the line segment to the target.</param>
        [BurstCompile]
        public void GetClosestPoint(in Point3D point, out Point3D closestPoint)
        {
            Direction3D lineDirection = math.normalize((float3) (end - start));
            Offset3D offset = point - start;
            float distance = math.dot((float3) offset, (float3) lineDirection);
            closestPoint = start + lineDirection * distance;
        }

        /// <summary>
        ///     Computes the symmetric point to the given point relative to this line.
        /// </summary>
        /// <param name="point">Point to compute the symmetric point of.</param>
        /// <param name="symmetricPoint">Symmetric point to the given point relative to this line.</param>
        /// <remarks>
        ///     We simply mirror it using line as it will give us the same result.
        /// </remarks>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetSymmetricPoint(in Point3D point, out Point3D symmetricPoint)
            => new Line3D(this).GetSymmetricPoint(point, out symmetricPoint);

        /// <summary>
        ///     Computes whether this line crosses the given <paramref name="plane"/>.
        /// </summary>
        /// <param name="plane">Plane to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Intersects(in Plane3D plane)
            => !plane.ArePointsOnSameSide(start, end);

        /// <summary>
        ///     Computes whether this line segment intersects with the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">Line to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] public bool Intersects(in Line3D line)
        {
            Offset3D u = end - start;
            Offset3D w = start - line.point;

            float a = math.dot((float3) u, (float3) u);
            float b = math.dot((float3) u, (float3) line.direction);
            float c = math.dot((float3) line.direction, (float3) line.direction);
            float d = math.dot((float3) u, (float3) w);
            float e = math.dot((float3) line.direction, (float3) w);
            float denominator = a * c - b * b;

            if (Mathf.Approximately(math.abs(denominator), 0))
            {
                // segment and line are parallel
                if (Mathf.Approximately(math.lengthsq(math.cross((float3) u, (float3) w)), 0)) return true;

                return false;
            }

            float t = (b * e - c * d) / denominator;
            // float s = (a * e - b * d) / denominator;

            // Check if the intersection point is on the line segment
            return t is >= 0f and <= 1f;
        }

        /// <summary>
        ///     Computes whether this line segment intersects with the given <paramref name="segment"/>.
        /// </summary>
        /// <param name="segment">Line segment to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] public bool Intersects(in Segment3D segment)
        {
            Offset3D u = end - start;
            Offset3D v = segment.end - segment.start;
            Offset3D w = start - segment.start;
            float a = math.dot((float3) u, (float3) u);
            float b = math.dot((float3) u, (float3) v);
            float c = math.dot((float3) v, (float3) v);
            float d = math.dot((float3) u, (float3) w);
            float e = math.dot((float3) v, (float3) w);
            float denominator = a * c - b * b;

            if (Hint.Unlikely(Mathf.Approximately(math.abs(denominator), 0))) // lines are parallel
            {
                // We're collinear
                if (Mathf.Approximately(math.lengthsq(math.cross((float3) w, (float3) u)), 0)) return true;
                return false;
            }

            float t = (b * e - c * d) / denominator;
            float s = (a * e - b * d) / denominator;

            // Check if the intersection point is on both segments
            return t is >= 0f and <= 1f && s is >= 0f and <= 1f;
        }

        /// <summary>
        ///     Computes whether this line is coincident with the given <paramref name="plane"/>.
        /// </summary>
        /// <param name="plane">Plane to check for coincidence with.</param>
        /// <returns><see langword="true"/> if the two shapes are coincident, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        ///     When whole line is coincident to plane then segment of this line is also coincident.
        ///     And when segment of this line is coincident to plane then whole line is also coincident.
        /// </remarks>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCoincident(in Plane3D plane) => new Line3D(this).IsCoincident(plane);

        /// <summary>
        ///     Computes whether this line is coincident with the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">Line to check for coincidence with.</param>
        /// <returns><see langword="true"/> if the two shapes are coincident, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        ///     When whole line is coincident to line then segment of this line is also coincident.
        ///     And when segment of this line is coincident to line then whole line is also coincident.
        /// </remarks>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsCoincident(in Line3D line)
            => new Line3D(this).IsCoincident(line);

        /// <summary>
        ///     Computes whether this line is coincident with the given <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point to check for coincidence with.</param>
        /// <returns><see langword="true"/> if the two shapes are coincident, <see langword="false"/> otherwise.</returns>
        [BurstCompile]
        public bool IsCoincident(in Point3D point)
        {
            GetClosestPoint(point, out Point3D closestPoint);
            return Hint.Unlikely(Mathf.Approximately(closestPoint.X, point.X) &&
                                 Mathf.Approximately(closestPoint.Y, point.Y) &&
                                 Mathf.Approximately(closestPoint.Z, point.Z));
        }

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
            => math.distancesq((float3) start, (float3) end);

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