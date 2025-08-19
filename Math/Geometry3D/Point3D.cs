using System;
using System.Runtime.CompilerServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Represents a point in 3D space
    /// </summary>
    public readonly struct Point3D : IUnmanaged<Point3D>, IEquatable<Point3D>
    {
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Point3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Point3D(in float3 value) : this(value.x, value.y, value.z)
        {
        }

        public Point3D(in Vector3 value) : this(value.x, value.y, value.z)
        {
        }

#region Calculation operations

        public static Offset3D operator -(in Point3D target, in Point3D source) => new(source, target);

        public static Point3D operator +(in Point3D target, in Offset3D offset)
            => new(target.x + offset.x, target.y + offset.y, target.z + offset.z);
        
        public static Point3D operator -(in Point3D target, in Offset3D offset)
            => new(target.x - offset.x, target.y - offset.y, target.z - offset.z);

#endregion

#region Conversion Operators

        public static implicit operator float3(in Point3D point) => new(point.x, point.y, point.z);
        public static implicit operator Vector3(in Point3D point) => new(point.x, point.y, point.z);

        public static implicit operator Point3D(in float3 point) => new(point.x, point.y, point.z);
        public static implicit operator Point3D(in Vector3 point) => new(point.x, point.y, point.z);

#endregion

#region IEquatable<Point3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Point3D other)
        {
            return Mathf.Approximately(x, other.x) &&
                   Mathf.Approximately(y, other.y) &&
                   Mathf.Approximately(z, other.z);
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Point3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => ((float3) this).GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Point3D left, in Point3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Point3D left, in Point3D right) => !left.Equals(right);

#endregion
    }
}