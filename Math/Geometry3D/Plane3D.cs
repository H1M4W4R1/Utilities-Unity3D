using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Plane in 3D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public struct Plane3D : IUnmanaged<Plane3D>, IEquatable<Plane3D>
    {
        /// <summary>
        ///     Vectorized data
        /// </summary>
        [FieldOffset(0)] private int4 vectorized;

        /// <summary>
        ///     Normal vector of the plane
        /// </summary>
        [FieldOffset(0)] public float3 normal;
        
        /// <summary>
        ///     Distance from origin
        /// </summary>
        [FieldOffset(12)] public float distance;

        /// <summary>
        ///     Create a new plane from a float4 containing the normal vector and distance.
        /// </summary>
        /// <param name="planeData">float4 containing (nx, ny, nz, distance)</param>
        public Plane3D(in float4 planeData)
        {
            vectorized = int4.zero;
            normal = new float3(planeData.x, planeData.y, planeData.z);
            distance = planeData.w;
        }

        /// <summary>
        ///     Creates a new plane with the given normal vector and distance.
        /// </summary>
        /// <param name="nx">x-component of the normal vector</param>
        /// <param name="ny">y-component of the normal vector</param>
        /// <param name="nz">z-component of the normal vector</param>
        /// <param name="distance">distance from the origin</param>
        public Plane3D(float nx, float ny, float nz, float distance)
        {
            vectorized = int4.zero;
            normal = new float3(nx, ny, nz);
            this.distance = distance;
        }
        
        /// <summary>
        ///     Creates a new plane with the given points.
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <param name="c">Third point</param>
        public Plane3D(in float3 a, in float3 b, in float3 c)
        {
            vectorized = int4.zero;
            normal = math.normalize(math.cross(b - a, c - a));
            distance = -math.dot(normal, a);
        }

        /// <summary>
        ///     A 3D plane, represented by a normal vector and a distance from the origin.
        /// </summary>
        /// <param name="normal">The normal vector of the plane, a float3.</param>
        /// <param name="distance">The distance from the origin, a float.</param>
        public Plane3D(in float3 normal, float distance)
        {
            vectorized = int4.zero;
            this.normal = normal;
            this.distance = distance;
        }

        /// <summary>
        ///     Constructs a new plane from a normal vector and a point that it passes through.
        /// </summary>
        /// <param name="normal">The normal vector of the plane, a float3.</param>
        /// <param name="point">A point that the plane passes through, a float3.</param>
        public Plane3D(in float3 normal, in float3 point)
        {
            vectorized = int4.zero;
            this.normal = normal;
            distance = -math.dot(normal, point);
        }

        /// <summary>
        ///     Determines if the given point is on the positive side of the plane.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>
        ///     <see langword="true"/> if the point is on the positive side of the plane, <see langword="false"/> otherwise.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsOnPositiveSide(in float3 point) => 
            math.dot(normal, point) + distance >= 0f;
        
        /// <summary>
        ///     Creates a new plane with the same normal vector but negative distance.
        /// </summary>
        /// <returns>
        ///     A new plane with the same normal vector but negative distance.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly Plane3D Flip()
            => new(-normal, -distance);

        /// <summary>
        ///     Creates a new plane that is translated by the given amount.
        /// </summary>
        /// <param name="translation">The amount to translate the plane by.</param>
        /// <returns>
        ///     A new plane that is translated by the given amount.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Plane3D Translate(in float3 translation)
            => new(normal, distance + math.dot(normal, translation));

        
        /// <summary>
        ///     Finds the closest point to the given target point on the plane.
        /// </summary>
        /// <param name="target">Target point to find the closest point from.</param>
        /// <returns>Closest point on the plane to the target.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float3 GetClosestPoint(in float3 target) =>
            target - normal * math.dot(normal, target) + distance;

        /// <summary>
        ///     Finds the distance from the given target point to the plane.
        /// </summary>
        /// <param name="target">Target point to find the distance from.</param>
        /// <returns>Distance from the target point to the plane.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceToPoint(in float3 target) => math.dot(normal, target) + distance;

        /// <summary>
        ///     Determines whether two points are on the same side of the plane.
        /// </summary>
        /// <param name="a">First point to check.</param>
        /// <param name="b">Second point to check.</param>
        /// <returns>
        ///     <see langword="true"/> if both points are on the same side of the plane, <see langword="false"/> otherwise.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ArePointsOnSameSide(in float3 a, in float3 b)
        {
            float distA = DistanceToPoint(a);
            float distB = DistanceToPoint(b);

            return (distA <= 0 && distB <= 0) || (distA > 0 && distB > 0);
        }

        /// <summary>
        ///     Determines whether a ray starting at the given origin and moving in the given direction intersects with the plane.
        /// </summary>
        /// <param name="origin">Origin of the ray to check.</param>
        /// <param name="direction">Direction of the ray to check.</param>
        /// <param name="enter">Time at which the ray intersects with the plane, or 0 if no intersection occurs.</param>
        /// <returns><see langword="true"/> if the ray intersects with the plane, <see langword="false"/> otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Raycast(in float3 origin, in float3 direction, out float enter)
        {
            float a = Vector3.Dot(direction, normal);
            float num = -Vector3.Dot(origin, normal) - distance;
            if (Mathf.Approximately(a, 0.0f))
            {
                enter = 0.0f;
                return false;
            }

            enter = num / a;
            return enter > 0.0;
        }
        
        /// <summary>
        ///     Finds the point at which three planes intersect.
        /// </summary>
        /// <param name="p1">First plane.</param>
        /// <param name="p2">Second plane.</param>
        /// <param name="p3">Third plane.</param>
        /// <param name="point">The point at which the three planes intersect, or zero if no intersection occurs.</param>
        /// <returns>
        ///     <see langword="true"/> if the three planes intersect at a single point, <see langword="false"/> otherwise.
        /// </returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntersectPlanes(in Plane3D p1, in Plane3D p2, in Plane3D p3, out float3 point)
        {
            float3 n1 = p1.normal;
            float3 n2 = p2.normal;
            float3 n3 = p3.normal;

            float det = math.dot(n1, math.cross(n2, n3));
            if (math.abs(det) < 1e-6f)
            {
                point = float3.zero;
                return false; // planes nearly parallel
            }

            float3 c1 = math.cross(n2, n3) * -p1.distance;
            float3 c2 = math.cross(n3, n1) * -p2.distance;
            float3 c3 = math.cross(n1, n2) * -p3.distance;

            point = (c1 + c2 + c3) / det;
            return true;
        }

#region Operators 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4(in Plane3D plane)
            => new(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Plane3D(in float4 plane) => new(plane);
        
#endregion

#region IEquatable<Plane3D> - implemented

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly bool Equals(Plane3D other)
            => vectorized.Equals(other.vectorized);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly override bool Equals(object obj)
            => obj is Plane3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly override int GetHashCode()
            => vectorized.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Plane3D left, in Plane3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Plane3D left, in Plane3D right) => !left.Equals(right);

#endregion
    }
}