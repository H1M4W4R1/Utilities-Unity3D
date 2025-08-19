using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Systems.Utilities.Annotations;
using Systems.Utilities.Math.Geometry;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Utilities.Math
{
    [BurstCompile] [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Frustum3D : IUnmanaged<Frustum3D>
    {
        [FieldOffset(0)] private readonly Plane3D _left;
        [FieldOffset(16)] private readonly Plane3D _right;
        [FieldOffset(32)] private readonly Plane3D _up;
        [FieldOffset(48)] private readonly Plane3D _down;
        [FieldOffset(64)] private readonly Plane3D _far;
        [FieldOffset(80)] private readonly Plane3D _near;

       
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
            
            // Normalize planes (optional, for consistent distances)
            fixed (Plane3D* planes = &_left)
            {
                for (int i = 0; i < 6; i++)
                {
                    float3 normal = planes[i].normal;
                    float length = math.length(normal);
                    planes[i].normal /= length;
                    planes[i].distance /= length;
                }    
            }
        }

        public Frustum3D(
            in float3 camPos,
            in quaternion camRot,
            float verticalSize,
            float aspect,
            float near,
            float far)
        {
            // Basis vectors
            float3 forward = math.mul(camRot, new float3(0, 0, 1));
            float3 up = math.mul(camRot, new float3(0, 1, 0));
            float3 rightV = math.mul(camRot, new float3(1, 0, 0));

            // Near/Far centers
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
            
            // Normalize planes (optional, for consistent distances)
            fixed (Plane3D* planes = &_left)
            {
                for (int i = 0; i < 6; i++)
                {
                    float3 normal = planes[i].normal;
                    float length = math.length(normal);
                    planes[i].normal /= length;
                    planes[i].distance /= length;
                }    
            }
        }

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsPoint(in float3 point)
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

            return false;
        }
        
        /// <summary>
        ///     Computes gizmo lines for drawing
        /// </summary>
        /// <param name="lines">Lines array, you have to dispose it manually</param>
        /// <param name="linesAllocator">Allocator used to create output array</param>
        [BurstCompile]
        public void ComputeGizmoLines(out NativeArray<Line3D> lines, Allocator linesAllocator = Allocator.TempJob)
        {
            lines = new NativeArray<Line3D>(12, linesAllocator);
            
            // Compute corners
            Plane3D.IntersectPlanes(_near, _up, _left, out float3 ntl);
            Plane3D.IntersectPlanes(_near, _up, _right, out float3 ntr);
            Plane3D.IntersectPlanes(_near, _down, _left, out float3 nbl);
            Plane3D.IntersectPlanes(_near, _down, _right, out float3 nbr);

            Plane3D.IntersectPlanes(_far, _up, _left, out float3 ftl);
            Plane3D.IntersectPlanes(_far, _up, _right, out float3 ftr);
            Plane3D.IntersectPlanes(_far, _down, _left, out float3 fbl);
            Plane3D.IntersectPlanes(_far, _down, _right, out float3 fbr);
            
            // Near plane
            lines[0] = new Line3D(ntl, ntr);
            lines[1] = new Line3D(ntr, nbr);
            lines[2] = new Line3D(nbr, nbl);
            lines[3] = new Line3D(nbl, ntl);

            // Far plane
            lines[4] = new Line3D(ftl, ftr);
            lines[5] = new Line3D(ftr, fbr);
            lines[6] = new Line3D(fbr, fbl);
            lines[7] = new Line3D(fbl, ftl);

            // Edges
            lines[8] = new Line3D(ntl, ftl);
            lines[9] = new Line3D(ntr, ftr);
            lines[10] = new Line3D(nbl, fbl);
            lines[11] = new Line3D(nbr, fbr);
        }
    }
}