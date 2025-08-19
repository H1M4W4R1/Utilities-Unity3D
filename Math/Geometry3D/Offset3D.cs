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

        /// <summary>
        ///     Initializes a new instance of the <see cref="Offset3D"/> struct from the given float3 value.
        /// </summary>
        /// <param name="value">The float3 value to initialize the structure from.</param>
        public Offset3D(in float3 value)
        {
            this._value = value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Offset3D"/> struct with the given offset values.
        /// </summary>
        /// <param name="x">The x component of the offset.</param>
        /// <param name="y">The y component of the offset.</param>
        /// <param name="z">The z component of the offset.</param>
        public Offset3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Offset3D"/> struct as the offset from the given point to the other given point.
        /// </summary>
        /// <param name="from">The starting point of the offset.</param>
        /// <param name="to">The ending point of the offset.</param>
        public Offset3D(in Point3D from, in Point3D to) : this(to.X - from.X, to.Y - from.Y, to.Z - from.Z)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Offset3D"/> struct from a Unity-compatible <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The Unity-compatible <see cref="Vector3"/> to initialize the structure from.</param>
        public Offset3D(in Vector3 value) : this((float3) value)
        {
        }

#region Calculation operations

        /// <summary>
        ///     Adds the given offset to the given offset.
        /// </summary>
        /// <param name="left">The offset to add to.</param>
        /// <param name="right">The offset to add.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the addition.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator +(in Offset3D left, in Offset3D right)
            => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

        /// <summary>
        ///     Subtracts the given offset from the given offset.
        /// </summary>
        /// <param name="left">The offset to subtract from.</param>
        /// <param name="right">The offset to subtract.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the subtraction.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator -(in Offset3D left, in Offset3D right)
            => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

        /// <summary>
        ///     Multiplies the given offset by the given scalar.
        /// </summary>
        /// <param name="left">The offset to multiply.</param>
        /// <param name="right">The scalar to multiply by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the multiplication.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(in Offset3D left, float right)
            => new(left.X * right, left.Y * right, left.Z * right);

        /// <summary>
        ///     Multiplies the given scalar by the given offset.
        /// </summary>
        /// <param name="left">The scalar to multiply by.</param>
        /// <param name="right">The offset to multiply.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the multiplication.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator *(float left, in Offset3D right)
            => new(left * right.X, left * right.Y, left * right.Z);

        /// <summary>
        ///     Divides the given offset by the given scalar.
        /// </summary>
        /// <param name="left">The offset to divide.</param>
        /// <param name="right">The scalar to divide by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the division.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(in Offset3D left, float right)
            => new(left.X / right, left.Y / right, left.Z / right);

        /// <summary>
        ///     Divides the given scalar by the given offset.
        /// </summary>
        /// <param name="left">The scalar to divide.</param>
        /// <param name="right">The offset to divide by.</param>
        /// <returns>A new <see cref="Offset3D"/> instance with the result of the division.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator /(float left, in Offset3D right)
            => new(left / right.X, left / right.Y, left / right.Z);
        
        /// <summary>
        ///     Returns a new <see cref="Offset3D"/> which is the negation of the given <see cref="Offset3D"/>.
        /// </summary>
        /// <param name="point">The point to negate.</param>
        /// <returns>A new <see cref="Offset3D"/> which is the negation of the given <see cref="Offset3D"/>.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Offset3D operator -(in Offset3D point) => new(-point.X, -point.Y, -point.Z);
        
#endregion

#region Conversion Operators
        
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Offset3D(in Segment3D segment) => new(segment.start, segment.end);
        
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