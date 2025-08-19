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
    ///     Line in 3D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly struct Line3D : IUnmanaged<Line3D>, IEquatable<Line3D>
    {
        /// <summary>
        ///     Vectorized data
        /// </summary>
        [FieldOffset(0)] private readonly int3x2 vectorized;

        /// <summary>
        ///     Point defining the line
        /// </summary>
        [FieldOffset(0)] public readonly Point3D point;

        /// <summary>
        ///     Line direction
        /// </summary>
        [FieldOffset(12)] public readonly Direction3D direction;

        /// <summary>
        ///     Creates a new line with the given point and direction.
        /// </summary>
        /// <param name="point">The point defining the line.</param>
        /// <param name="direction">The line direction.</param>
        public Line3D(in Point3D point, in Direction3D direction)
        {
            vectorized = int3x2.zero;
            this.point = point;
            this.direction = direction;
        }

        /// <summary>
        ///     Creates a new line from the given segment.
        ///     The given segment is converted into a line by using its start point as the line point
        ///     and the vector from the start point to the end point as the line direction.
        /// </summary>
        /// <param name="lineSegment">Segment to create a line from.</param>
        public Line3D(in Segment3D lineSegment)
        {
            vectorized = int3x2.zero;
            point = lineSegment.start;
            direction = math.normalize((float3) (lineSegment.end - lineSegment.start));
        }

        /// <summary>
        ///     Finds the closest point to the given target point on the line.
        /// </summary>
        /// <param name="tPoint">Target point to find the closest point from.</param>
        /// <returns>Closest point on the line to the target.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point3D GetClosestPoint(in Point3D tPoint)
        {
            Offset3D v = tPoint - point;
            float t = math.dot((float3) v, (float3) direction) /
                      math.dot((float3) direction, (float3) direction);
            return point + t * direction;
        }

        /// <summary>
        ///     Computes the distance from the given point to the line.
        /// </summary>
        /// <param name="tPoint">Point to compute the distance from.</param>
        /// <returns>Distance from the given point to the line.</returns>
        public float GetDistance(in Point3D tPoint)
        {
            Offset3D offset = tPoint - GetClosestPoint(tPoint);
            return math.length((float3) offset);
        }

        /// <summary>
        ///     Computes the symmetric point to the given point relative to this line.
        /// </summary>
        /// <param name="tPoint">Point to compute the symmetric point of.</param>
        /// <returns>Symmetric point to the given point relative to this line.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point3D GetSymmetricPoint(in Point3D tPoint)
        {
            Point3D pointOnLine = GetClosestPoint(tPoint);
            Offset3D vectorToPoint = tPoint - pointOnLine;
            return pointOnLine - vectorToPoint;
        }

        /// <summary>
        ///     Computes whether this line intersects with the given <paramref name="plane"/>.
        /// </summary>
        /// <param name="plane">Plane to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Intersects(in Plane3D plane)
        {
            // Check if the line is parallel to the plane
            float lowDet = math.dot((float3) plane.normal, (float3) direction);
            if (Hint.Unlikely(Mathf.Approximately(lowDet, 0))) return false;

            // Intersects or is coincident
            return true;
        }

        /// <summary>
        ///     Computes whether this line intersects with the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">Line to check for intersection with.</param>
        /// <returns><see langword="true"/> if the two shapes intersect, <see langword="false"/> otherwise.</returns>
        public bool Intersects(in Line3D line)
        {
            float3 r = (float3) (point - line.point);
            float a = math.dot((float3) direction, (float3) direction);
            float b = math.dot((float3) direction, (float3) line.direction);
            float c = math.dot((float3) line.direction, (float3) line.direction);
            float d = math.dot((float3) direction, r);
            float e = math.dot((float3) line.direction, r);

            float det = a * c - b * b;
            if (Mathf.Approximately(math.abs(det), 0)) return false;

            float t = (b * e - c * d) / det;
            float u = (a * e - b * d) / det;

            float3 q1 = (float3) (point + t * direction);
            float3 q2 = (float3) (line.point + u * line.direction);

            return Mathf.Approximately(math.lengthsq(q1 - q2), 0); // detect intersection
        }

        /// <summary>
        ///     Computes whether this line is coincident with the given <paramref name="plane"/>.
        /// </summary>
        /// <param name="plane">Plane to check for coincidence with.</param>
        /// <returns><see langword="true"/> if the two shapes are coincident, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCoincident(in Plane3D plane)
        {
            // If plane does not intersect, it is not coincident
            if (!Hint.Likely(Intersects(plane))) return false;

            // Check if the line is parallel to the plane
            return Hint.Unlikely(math.dot((float3) plane.normal, (float3) point) + plane.distance == 0);
        }

        /// <summary>
        ///     Computes whether this line is collinear with the given <paramref name="line"/>.
        /// </summary>
        /// <param name="line">Line to check for collinearity with.</param>
        /// <returns><see langword="true"/> if the two lines are collinear, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool IsCoincident(in Line3D line)
        {
            float3 r = (float3) (point - line.point);
            float a = math.dot((float3) direction, (float3) direction);
            float b = math.dot((float3) direction, (float3) line.direction);
            float c = math.dot((float3) line.direction, (float3) line.direction);

            float det = a * c - b * b;
            if (Mathf.Approximately(math.abs(det), 0))
            {
                // Check if lines are collinear
                float3 cross = math.cross(r, (float3) direction);
                if (!Mathf.Approximately(math.lengthsq(cross), 0)) return false; // parallel, no intersection

                return true; // coincident
            }

            // Probably intersects
            return false;
        }

        /// <summary>
        ///     Computes whether this line is coincident with the given <paramref name="tPoint"/>.
        /// </summary>
        /// <param name="tPoint">Point to check for coincidence with.</param>
        /// <returns><see langword="true"/> if the two shapes are coincident, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCoincident(in Point3D tPoint)
        {
            Point3D closestPoint = GetClosestPoint(tPoint);
            return Hint.Unlikely(Mathf.Approximately(closestPoint.X, tPoint.X) &&
                                 Mathf.Approximately(closestPoint.Y, tPoint.Y) &&
                                 Mathf.Approximately(closestPoint.Z, tPoint.Z));
        }


#region IEquatable<Line3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Line3D other)
            => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Line3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Line3D left, in Line3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Line3D left, in Line3D right) => !left.Equals(right);

#endregion
    }
}