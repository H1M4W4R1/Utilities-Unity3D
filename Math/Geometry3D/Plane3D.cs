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
        [FieldOffset(0)] private int4 vectorized;

        [FieldOffset(0)] public float3 normal;
        [FieldOffset(12)] public float distance;

        public Plane3D(in float4 planeData)
        {
            vectorized = int4.zero;
            normal = new float3(planeData.x, planeData.y, planeData.z);
            distance = planeData.w;
        }

        public Plane3D(float nx, float ny, float nz, float distance)
        {
            vectorized = int4.zero;
            normal = new float3(nx, ny, nz);
            this.distance = distance;
        }

        public Plane3D(in float3 a, in float3 b, in float3 c)
        {
            vectorized = int4.zero;
            normal = math.normalize(math.cross(b - a, c - a));
            distance = -math.dot(normal, a);
        }

        public Plane3D(in float3 normal, float distance)
        {
            vectorized = int4.zero;
            this.normal = normal;
            this.distance = distance;
        }

        public Plane3D(in float3 normal, in float3 point)
        {
            vectorized = int4.zero;
            this.normal = normal;
            distance = -math.dot(normal, point);
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool IsOnPositiveSide(in float3 point) => 
            math.dot(normal, point) + distance >= 0f;
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly Plane3D Flip()
            => new(-normal, -distance);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Plane3D Translate(in float3 translation)
            => new(normal, distance + math.dot(normal, translation));

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float3 ClosestPointToPoint(in float3 target) =>
            target - normal * math.dot(normal, target) + distance;

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float DistanceToPoint(in float3 target) => math.dot(normal, target) + distance;

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool ArePointsOnSameSide(in float3 a, in float3 b)
        {
            float distA = DistanceToPoint(a);
            float distB = DistanceToPoint(b);

            return (distA <= 0 && distB <= 0) || (distA > 0 && distB > 0);
        }

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
            => new float4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Plane3D(in float4 plane) => new Plane3D(plane);
        
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