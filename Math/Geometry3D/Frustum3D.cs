using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Systems.Utilities.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math.Geometry3D
{
    /// <summary>
    ///     Struct that represents a 3D frustum.
    ///     A frustum is defined by six planes: left, right, up, down, far, and near.
    ///     Each plane is defined as a normal vector and a distance from the origin.
    /// </summary>
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public readonly unsafe struct Frustum3D : IUnmanaged<Frustum3D>
    {
        [FieldOffset(0)] private readonly Plane3D _left;
        [FieldOffset(16)] private readonly Plane3D _right;
        [FieldOffset(32)] private readonly Plane3D _up;
        [FieldOffset(48)] private readonly Plane3D _down;
        [FieldOffset(64)] private readonly Plane3D _far;
        [FieldOffset(80)] private readonly Plane3D _near;


        /// <summary>
        ///     Creates a frustum from a camera object.
        /// 
        ///     The frustum is defined by six planes: left, right, up, down, far, and near.
        ///     Each plane is defined as a normal vector and a distance from the origin.
        ///     The normal vector points away from the frustum, and the distance is the
        ///     distance from the origin to the plane.
        /// 
        ///     The planes are normalized to ensure consistent distances.     
        /// </summary>
        /// <param name="cameraObject">The camera object to create the frustum from.</param>
        public Frustum3D([NotNull] Camera cameraObject)
        {
            Matrix4x4 projectionMatrix = cameraObject.projectionMatrix;
            Matrix4x4 viewMatrix = cameraObject.worldToCameraMatrix;

            Matrix4x4 mat = projectionMatrix * viewMatrix;

            _left = new Plane3D(mat.m30 + mat.m00, mat.m31 + mat.m01, mat.m32 + mat.m02, mat.m33 + mat.m03);
            _right = new Plane3D(mat.m30 - mat.m00, mat.m31 - mat.m01, mat.m32 - mat.m02, mat.m33 - mat.m03);
            _down = new Plane3D(mat.m30 + mat.m10, mat.m31 + mat.m11, mat.m32 + mat.m12, mat.m33 + mat.m13);
            _up = new Plane3D(mat.m30 - mat.m10, mat.m31 - mat.m11, mat.m32 - mat.m12, mat.m33 - mat.m13);
            _near = new Plane3D(mat.m30 + mat.m20, mat.m31 + mat.m21, mat.m32 + mat.m22, mat.m33 + mat.m23);
            _far = new Plane3D(mat.m30 - mat.m20, mat.m31 - mat.m21, mat.m32 - mat.m22, mat.m33 - mat.m23);
        }


        /// <summary>
        ///     Creates a frustum from camera-like parameters.
        /// </summary>
        /// <param name="camPos">The camera position.</param>
        /// <param name="camRot">The camera rotation.</param>
        /// <param name="verticalSize">The vertical size of the frustum (far plane).</param>
        /// <param name="aspect">The aspect ratio of the frustum.</param>
        /// <param name="near">The near plane distance.</param>
        /// <param name="far">The far plane distance.</param>
        public Frustum3D(
            in float3 camPos,
            in quaternion camRot,
            float verticalSize,
            float aspect,
            float near,
            float far)
        {
            // Build 3D basis from rotation
            float3 forward = math.mul(camRot, new float3(0, 0, 1));
            float3 up = math.mul(camRot, new float3(0, 1, 0));
            float3 rightV = math.mul(camRot, new float3(1, 0, 0));

            // Centers
            float3 nearCenter = camPos + forward * near;
            float3 farCenter = camPos + forward * far;

            // Half sizes
            float halfFarHeight = verticalSize * 0.5f;
            float halfFarWidth = halfFarHeight * aspect;
            float halfNearHeight = halfFarHeight * (near / far);
            float halfNearWidth = halfNearHeight * aspect;

            // Near plane corners
            float3 ntl = nearCenter + up * halfNearHeight - rightV * halfNearWidth;
            float3 ntr = nearCenter + up * halfNearHeight + rightV * halfNearWidth;
            float3 nbl = nearCenter - up * halfNearHeight - rightV * halfNearWidth;
            float3 nbr = nearCenter - up * halfNearHeight + rightV * halfNearWidth;

            // Far plane corners
            float3 ftl = farCenter + up * halfFarHeight - rightV * halfFarWidth;
            float3 ftr = farCenter + up * halfFarHeight + rightV * halfFarWidth;
            float3 fbl = farCenter - up * halfFarHeight - rightV * halfFarWidth;
            float3 fbr = farCenter - up * halfFarHeight + rightV * halfFarWidth;

            // Planes
            _left = new Plane3D(camPos, nbl, fbl);
            _right = new Plane3D(camPos, fbr, nbr);
            _up = new Plane3D(camPos, ntr, ftr);
            _down = new Plane3D(camPos, fbl, nbl);
            _near = new Plane3D(nbl, ntr, ntl);
            _far = new Plane3D(ftr, fbr, ftl);
        }


        /// <summary>
        ///     Checks if a point is inside the frustum.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is inside, false otherwise.</returns>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(in Point3D point)
        {
            // Check for all planes
            fixed (Plane3D* planes = &_left)
            {
                for (int i = 0; i < 6; i++)
                {
                    Plane3D plane = planes[i];
                    if (!plane.IsOnPositiveSide(point)) return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Computes the frustum's bounding lines.
        /// </summary>
        /// <param name="lines">Lines array, you have to dispose it manually</param>
        /// <param name="linesAllocator">Allocator used to create output array</param>
        [BurstCompile] public void ComputeGizmoLines(
            out NativeArray<Segment3D> lines,
            Allocator linesAllocator = Allocator.TempJob)
        {
            lines = new NativeArray<Segment3D>(12, linesAllocator);

            // Compute corners
            Plane3D.IntersectPlanes(_near, _up, _left, out Point3D ntl);
            Plane3D.IntersectPlanes(_near, _up, _right, out Point3D ntr);
            Plane3D.IntersectPlanes(_near, _down, _left, out Point3D nbl);
            Plane3D.IntersectPlanes(_near, _down, _right, out Point3D nbr);

            Plane3D.IntersectPlanes(_far, _up, _left, out Point3D ftl);
            Plane3D.IntersectPlanes(_far, _up, _right, out Point3D ftr);
            Plane3D.IntersectPlanes(_far, _down, _left, out Point3D fbl);
            Plane3D.IntersectPlanes(_far, _down, _right, out Point3D fbr);

            // Near plane
            lines[0] = new Segment3D(ntl, ntr);
            lines[1] = new Segment3D(ntr, nbr);
            lines[2] = new Segment3D(nbr, nbl);
            lines[3] = new Segment3D(nbl, ntl);

            // Far plane
            lines[4] = new Segment3D(ftl, ftr);
            lines[5] = new Segment3D(ftr, fbr);
            lines[6] = new Segment3D(fbr, fbl);
            lines[7] = new Segment3D(fbl, ftl);

            // Edges
            lines[8] = new Segment3D(ntl, ftl);
            lines[9] = new Segment3D(ntr, ftr);
            lines[10] = new Segment3D(nbl, fbl);
            lines[11] = new Segment3D(nbr, fbr);
        }
    }
}