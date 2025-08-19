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

        public float X => _value.x;
        public float Y => _value.y;
        public float Z => _value.z;
        
        
        /// <summary>
        ///     Constructs a new <see cref="Direction3D"/> instance from the given <see cref="float3"/>.
        /// </summary>
        /// <param name="value">The value to use for the direction.</param>
        public Direction3D(in float3 value) => _value = value;

        /// <summary>
        ///     Constructs a new <see cref="Direction3D"/> instance from the given components.
        /// </summary>
        /// <param name="x">The x component of the direction.</param>
        /// <param name="y">The y component of the direction.</param>
        /// <param name="z">The z component of the direction.</param>
        public Direction3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }

        
        /// <summary>
        ///     Constructs a new <see cref="Direction3D"/> instance from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The value to use for the direction.</param>
        public Direction3D(in Vector3 value) : this((float3) value)
        {
        }

#region Math operators

        /// <summary>
        ///     Multiplies the direction by the given scalar.
        /// </summary>
        /// <param name="left">The direction to multiply.</param>
        /// <param name="right">The scalar to multiply by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the multiplication.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(in Direction3D left, float right) => new(left._value * right);
        
        /// <summary>
        ///     Multiplies the direction by the given scalar.
        /// </summary>
        /// <param name="left">The scalar to multiply by.</param>
        /// <param name="right">The direction to multiply.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the multiplication.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(float left, in Direction3D right) => new(left * right._value);
        
        /// <summary>
        ///     Divides the direction by the given scalar.
        /// </summary>
        /// <param name="left">The direction to divide.</param>
        /// <param name="right">The scalar to divide by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the division.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(in Direction3D left, float right) => new(left._value / right);
        
        /// <summary>
        ///     Divides the given scalar by the direction.
        /// </summary>
        /// <param name="left">The scalar to divide.</param>
        /// <param name="right">The direction to divide by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the division.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(float left, in Direction3D right) => new(left / right._value);

        /// <summary>
        ///     Computes the negation of the given direction.
        /// </summary>
        /// <param name="direction">The direction to negate.</param>
        /// <returns>A new <see cref="Direction3D"/> instance with the negated direction.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Direction3D operator -(in Direction3D direction) => new(-direction._value);
#endregion

#region Conversion operators

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3(in Direction3D direction) => direction._value;

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
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