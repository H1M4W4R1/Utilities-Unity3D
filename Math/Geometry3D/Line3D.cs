using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Line in 3D space
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)] public readonly struct Line3D : IUnmanaged<Line3D>, IEquatable<Line3D>
    {
        [FieldOffset(0)] private readonly int3x2 vectorized;

        [FieldOffset(0)] public readonly float3 start;
        [FieldOffset(12)] public readonly float3 end;

        public Line3D(in float3 start, in float3 end)
        {
            vectorized = int3x2.zero;
            this.start = start;
            this.end = end;
        }
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float3 GetClosestPointToPoint(in float3 target)
        {
            float3 lineDirection = math.normalize(end - start);
            float3 offset = target - start;
            float distance = math.dot(offset, lineDirection);
            return start + lineDirection * distance;
        }
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float3 GetSymmetricPoint(in float3 point)
        {
            float3 pointOnLine = GetClosestPointToPoint(point);
            float3 vectorToPoint = point - pointOnLine;
            return pointOnLine - vectorToPoint;
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool CrossesPlane(in Plane3D plane) => !plane.ArePointsOnSameSide(start, end);
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float Length()
            => math.distance(start, end);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly float LengthSq()
            => math.distancesq(start, end);

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