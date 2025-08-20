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
    [BurstCompile]
    public readonly struct Point3D : IUnmanaged<Point3D>, IEquatable<Point3D>
    {
        private readonly float3 _value;

        public float X => _value.x;
        public float Y => _value.y;
        public float Z => _value.z;

        /// <summary>
        ///     Creates a new <see cref="Point3D"/> from the given <see cref="float3"/>.
        /// </summary>
        /// <param name="value">The value to use for the <see cref="Point3D"/>.</param>
        public Point3D(in float3 value)
        {
            this._value = value;
        }

        
        /// <summary>
        ///     Creates a new <see cref="Point3D"/> from the given values.
        /// </summary>
        /// <param name="x">The x-coordinate of the new <see cref="Point3D"/>.</param>
        /// <param name="y">The y-coordinate of the new <see cref="Point3D"/>.</param>
        /// <param name="z">The z-coordinate of the new <see cref="Point3D"/>.</param>
        public Point3D(float x, float y, float z) : this(new float3(x, y, z))
        {
        }


        /// <summary>
        ///     Creates a new <see cref="Point3D"/> from the given <see cref="Vector3"/>.
        /// </summary>
        /// <param name="value">The value to use for the <see cref="Point3D"/>.</param>
        public Point3D(in Vector3 value) : this((float3) value)
        {
        }

#region Calculation operations

        /// <summary>
        ///     Computes the offset from the given <paramref name="source"/> point to the given <paramref name="target"/> point.
        /// </summary>
        /// <param name="target">The target point to compute the offset to.</param>
        /// <param name="source">The source point to compute the offset from.</param>
        /// <returns>The offset from the source point to the target point.</returns>
        public static Offset3D operator -(in Point3D target, in Point3D source) => new(source, target);

        /// <summary>
        ///     Computes the point offset from the given <paramref name="target"/> point by the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="target">The target point to offset from.</param>
        /// <param name="offset">The offset to apply to the target point.</param>
        /// <returns>A new <see cref="Point3D"/> instance with the offset applied.</returns>
        public static Point3D operator +(in Point3D target, in Offset3D offset)
            => new(target.X + offset.X, target.Y + offset.Y, target.Z + offset.Z);

        /// <summary>
        ///     Computes the point offset from the given <paramref name="target"/> point by negating the given <paramref name="offset"/>.
        /// </summary>
        /// <param name="target">The target point to offset from.</param>
        /// <param name="offset">The offset to negate and apply to the target point.</param>
        /// <returns>A new <see cref="Point3D"/> instance with the negated offset applied.</returns>
        public static Point3D operator -(in Point3D target, in Offset3D offset)
            => new(target.X - offset.X, target.Y - offset.Y, target.Z - offset.Z);

#endregion

#region Conversion Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3(in Point3D point) => point._value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Vector3(in Point3D point) => point._value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Point3D(in float3 point) => new(point);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Point3D(in Vector3 point) => new(point);

#endregion

#region IEquatable<Point3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Point3D other)
            => _value.Equals(other._value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Point3D other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => _value.GetHashCode();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Point3D left, in Point3D right) => left.Equals(right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Point3D left, in Point3D right) => !left.Equals(right);

#endregion
    }
}