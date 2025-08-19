using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry2D
{
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly unsafe struct Frustum2D : IUnmanaged<Frustum2D>
    {
        // Four boundary lines of the 2D frustum (convex trapezoid): left, right, far, near
        // Note: Line2D uses explicit layout internally; here we conservatively space fields.
        [FieldOffset(0)] private readonly Line2D _left;
        [FieldOffset(24)] private readonly Line2D _right;
        [FieldOffset(48)] private readonly Line2D _far;
        [FieldOffset(72)] private readonly Line2D _near;

        /// <summary>
        ///     Constructs a 2D frustum (trapezoid) from camera-like parameters.
        ///     verticalSize corresponds to the far-plane vertical size in 3D; here it maps to height used with aspect to derive lateral extent.
        /// </summary>
        public Frustum2D(
            in float2 camPos,
            in quaternion camRot,
            float verticalSize,
            float aspect,
            float near,
            float far)
        {
            // Build 2D basis from rotation (use XY components of rotated 3D basis)
            float2 forward = math.mul(camRot, new float3(0, 1, 0)).xy;
            float2 rightV = math.mul(camRot, new float3(1, 0, 0)).xy;

            // Centers for near/far segments along forward
            float2 nearCenter = camPos + forward * near;
            float2 farCenter = camPos + forward * far;

            // Lateral extents (widths) derived analogously to 3D where width = height * aspect
            float halfFarHeight = verticalSize * 0.5f;
            float halfFarWidth = halfFarHeight * aspect;
            float halfNearHeight = halfFarHeight * (near / far);
            float halfNearWidth = halfNearHeight * aspect;

            // Near/Far segment endpoints (left/right)
            float2 nl = nearCenter - rightV * halfNearWidth;
            float2 nr = nearCenter + rightV * halfNearWidth;
            float2 fl = farCenter - rightV * halfFarWidth;
            float2 fr = farCenter + rightV * halfFarWidth;

            // Lines (orientation does not affect ContainsPoint since we use explicit inward normals)
            _left = new Line2D(nl, fl);
            _right = new Line2D(fr, nr);
            _near = new Line2D(nl, nr);
            _far = new Line2D(fr, fl);
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(in float2 point)
        {
            // Compute inward normals from stored line directions
            float2 dLeft = _left.end - _left.start;   // approx forward
            float2 dRight = _right.end - _right.start; // approx -forward
            float2 dNear = _near.end - _near.start;   // approx +right
            float2 dFar = _far.end - _far.start;      // approx -right

            float2 nLeft = RotateCW(math.normalizesafe(dLeft));   // inward toward +right
            float2 nRight = RotateCCW(math.normalizesafe(dRight)); // inward toward -right
            float2 nNear = RotateCCW(math.normalizesafe(dNear));   // inward toward +forward
            float2 nFar = RotateCCW(math.normalizesafe(dFar));     // inward toward -forward

            if (math.dot(nLeft, point - _left.start) < 0f) return false;
            if (math.dot(nRight, point - _right.start) < 0f) return false;
            if (math.dot(nNear, point - _near.start) < 0f) return false;
            if (math.dot(nFar, point - _far.start) < 0f) return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static float2 RotateCW(in float2 v)
            => new float2(v.y, -v.x);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] private static float2 RotateCCW(in float2 v)
            => new float2(-v.y, v.x);

        /// <summary>
        ///     Computes gizmo lines for drawing the trapezoid outline.
        /// </summary>
        [BurstCompile]
        public void ComputeGizmoLines(out NativeArray<Line2D> lines, Allocator linesAllocator = Allocator.TempJob)
        {
            lines = new NativeArray<Line2D>(4, linesAllocator);

            // Intersections (corners) from existing boundary endpoints
            // Left boundary (_left): nl -> fl, Right boundary (_right): fr -> nr
            float2 nl = _left.start;   // near-left
            float2 fl = _left.end;     // far-left
            float2 fr = _right.start;  // far-right
            float2 nr = _right.end;    // near-right

            // Edges ordered: near, right, far, left
            lines[0] = new Line2D(nl, nr); // near edge
            lines[1] = new Line2D(nr, fr); // right edge
            lines[2] = new Line2D(fr, fl); // far edge
            lines[3] = new Line2D(fl, nl); // left edge
        }
    }
}