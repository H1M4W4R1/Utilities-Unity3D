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
        private readonly float3 _value;

        public float X => _value.x;
        public float Y => _value.y;
        public float Z => _value.z;

        public Point3D(in float3 value)
        {
            this._value = value;
        }


        public Point3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }


        public Point3D(in Vector3 value) : this((float3) value)
        {
        }

#region Calculation operations

        public static Offset3D operator -(in Point3D target, in Point3D source) => new(source, target);

        public static Point3D operator +(in Point3D target, in Offset3D offset)
            => new(target.X + offset.X, target.Y + offset.Y, target.Z + offset.Z);

        public static Point3D operator -(in Point3D target, in Offset3D offset)
            => new(target.X - offset.X, target.Y - offset.Y, target.Z - offset.Z);

#endregion

#region Conversion Operators

        public static explicit operator float3(in Point3D point) => new(point.X, point.Y, point.Z);
        public static explicit operator Vector3(in Point3D point) => new(point.X, point.Y, point.Z);

        public static implicit operator Point3D(in float3 point) => new(point.x, point.y, point.z);
        public static implicit operator Point3D(in Vector3 point) => new(point.x, point.y, point.z);

#endregion

#region IEquatable<Point3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Point3D other)
            => _value.Equals(other._value);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Point3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => _value.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Point3D left, in Point3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Point3D left, in Point3D right) => !left.Equals(right);

#endregion
    }
}