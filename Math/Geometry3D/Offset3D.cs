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
        public readonly float x;
        public readonly float y;
        public readonly float z;

        public Offset3D(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Offset3D(in Point3D from, in Point3D to) : this(to.x - from.x, to.y - from.y, to.z - from.z)
        {
        }

        public Offset3D(in float3 value) : this(value.x, value.y, value.z)
        {
        }

        public Offset3D(in Vector3 value) : this(value.x, value.y, value.z)
        {
        }

#region Calculation operations

        public static Offset3D operator +(in Offset3D left, in Offset3D right)
            => new(left.x + right.x, left.y + right.y, left.z + right.z);
        
        public static Offset3D operator -(in Offset3D left, in Offset3D right)
            => new(left.x - right.x, left.y - right.y, left.z - right.z);
        
        public static Offset3D operator *(in Offset3D left, float right)
            => new(left.x * right, left.y * right, left.z * right);
        
        public static Offset3D operator *(float left, in Offset3D right)
            => new(left * right.x, left * right.y, left * right.z);
        
        public static Offset3D operator /(in Offset3D left, float right)
            => new(left.x / right, left.y / right, left.z / right);
        
        public static Offset3D operator /(float left, in Offset3D right)
            => new(left / right.x, left / right.y, left / right.z);

#endregion

#region Conversion Operators

        public static implicit operator float3(in Offset3D point) => new(point.x, point.y, point.z);
        public static implicit operator Vector3(in Offset3D point) => new(point.x, point.y, point.z);

        public static implicit operator Offset3D(in float3 point) => new(point.x, point.y, point.z);
        public static implicit operator Offset3D(in Vector3 point) => new(point.x, point.y, point.z);

#endregion

#region IEquatable<Offset3D> - implemented

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Equals(Offset3D other)
        {
            return Mathf.Approximately(x, other.x) && Mathf.Approximately(y, other.y) &&
                   Mathf.Approximately(z, other.z);
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override bool Equals(object obj)
            => obj is Offset3D other && Equals(other);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public override int GetHashCode()
            => ((float3) this).GetHashCode();

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Offset3D left, Offset3D right) => left.Equals(right);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Offset3D left, Offset3D right) => !left.Equals(right);

#endregion
    }
}