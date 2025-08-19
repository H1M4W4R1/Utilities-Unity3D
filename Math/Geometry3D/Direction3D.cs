using System;
using System.Runtime.CompilerServices;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Direction in 3D space
    /// </summary>
    public readonly struct Direction3D : IUnmanaged<Direction3D>, IEquatable<Direction3D>
    {
        private readonly float3 _value;

        public Direction3D(in float3 value) => _value = value;

        public Direction3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }

        public Direction3D(in Vector3 value) : this((float3) value)
        {
        }

#region Math operators

        public static Offset3D operator *(in Direction3D left, float right) => new(left._value * right);
        public static Offset3D operator *(float left, in Direction3D right) => new(left * right._value);
        public static Offset3D operator /(in Direction3D left, float right) => new(left._value / right);
        public static Offset3D operator /(float left, in Direction3D right) => new(left / right._value);

#endregion

#region Conversion operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3(in Direction3D direction) => direction._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Direction3D(in float3 direction) => new(direction);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(in Direction3D direction) => direction._value;

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Direction3D(in Vector3 direction) => new(direction);

#endregion

#region IEquatable<Direction3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Direction3D other)
            => _value.Equals(other._value);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Direction3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => _value.GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Direction3D left, in Direction3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Direction3D left, in Direction3D right) => !left.Equals(right);

#endregion
    }
}