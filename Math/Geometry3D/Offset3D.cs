using System;
using System.Runtime.CompilerServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Represents offset in 3D space
    /// </summary>
    public readonly struct Offset3D : IUnmanaged<Offset3D>, IEquatable<Offset3D>
    {
        private readonly float3 _value;

        public float X => _value.x;
        public float Y => _value.y;
        public float Z => _value.z;

        public Offset3D(in float3 value)
        {
            this._value = value;
        }

        public Offset3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }

        public Offset3D(in Point3D from, in Point3D to) : this(to.X - from.X, to.Y - from.Y, to.Z - from.Z)
        {
        }

        public Offset3D(in Vector3 value) : this((float3) value)
        {
        }

#region Calculation operations

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator +(in Offset3D left, in Offset3D right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator -(in Offset3D left, in Offset3D right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(in Offset3D left, float right)
            => new(left.X * right, left.Y * right, left.Z * right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(float left, in Offset3D right)
            => new(left * right.X, left * right.Y, left * right.Z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(in Offset3D left, float right)
            => new(left.X / right, left.Y / right, left.Z / right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(float left, in Offset3D right)
            => new(left / right.X, left / right.Y, left / right.Z);

#endregion

#region Conversion Operators

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3(in Offset3D point) => new(point.X, point.Y, point.Z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(in Offset3D point) => new(point.X, point.Y, point.Z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Offset3D(in float3 point) => new(point.x, point.y, point.z);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Offset3D(in Vector3 point) => new(point.x, point.y, point.z);

#endregion

#region IEquatable<Offset3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Offset3D other) => _value.Equals(other._value);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is Offset3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _value.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Offset3D left, in Offset3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Offset3D left, in Offset3D right) => !left.Equals(right);

#endregion
    }
}