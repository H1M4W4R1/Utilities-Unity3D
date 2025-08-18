using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

namespace Systems.Utilities.Frustum
{
    [BurstCompile]
    public static class FrustumUtil
    {
        /// <summary>
        ///     Build a plane from 3 points (counter-clockwise order).
        ///     Returned as float4(normal.xyz, distance).
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreatePlane(in float3 a, in float3 b, in float3 c, out float4 plane)
        {
            float3 normal = math.normalize(math.cross(b - a, c - a));
            float distance = -math.dot(normal, a);
            plane = new float4(normal, distance);
        }

        /// <summary>
        ///     Check if a point is inside plane.
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsInside(in float3 point, in float4 plane)
        {
            return math.dot(plane.xyz, point) + plane.w >= 0f;
        }

        /// <summary>
        ///     Computes the 6 frustum planes as float4(x,y,z,d).
        /// </summary>
        [BurstCompile]
        public static void ExtractFrustumPlanes(
            in float3 camPos,
            in quaternion camRot,
            float verticalSize,
            float aspect,
            float near,
            float far,
            out float4 left,
            out float4 right,
            out float4 top,
            out float4 bottom,
            out float4 nearP,
            out float4 farP)
        {
            // Basis vectors
            float3 forward = math.mul(camRot, new float3(0, 0, 1));
            float3 up      = math.mul(camRot, new float3(0, 1, 0));
            float3 rightV  = math.mul(camRot, new float3(1, 0, 0));

            // Near/Far centers
            float3 nearCenter = camPos + forward * near;
            float3 farCenter  = camPos + forward * far;

            // Half sizes
            float halfFarHeight = verticalSize * 0.5f;
            float halfFarWidth   = halfFarHeight * aspect;
            float halfNearHeight = halfFarHeight * (near / far);
            float halfNearWidth  = halfNearHeight * aspect;

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
            CreatePlane(camPos, nbl, fbl, out left);
            CreatePlane(camPos, fbr, nbr, out right);
            CreatePlane(camPos, ntr, ftr, out top);
            CreatePlane(camPos, fbl, nbl, out bottom);
            CreatePlane(nbl, ntr, ntl, out nearP);
            CreatePlane(ftr, fbr, ftl, out farP);
        }

        /// <summary>
        ///     Extract frustrum planes from camera
        /// </summary>
        /// <remarks>
        ///     NativeArray must be preallocated with size of 6 to make this work properly
        /// </remarks>
        [BurstDiscard] public static void ExtractFrustumPlanes(
            [NotNull] this Camera camera,
            ref NativeArray<float4> planes)
        {
            Matrix4x4 projectionMatrix = camera.projectionMatrix;
            Matrix4x4 viewMatrix = camera.worldToCameraMatrix;
            ExtractFrustumPlanes(projectionMatrix, viewMatrix, ref planes);
        }
        
        /// <summary>
        ///     Extract frustrum planes from camera planes
        /// </summary>
        /// <remarks>
        ///     NativeArray must be preallocated with size of 6 to make this work properly
        /// </remarks>
        [BurstCompile] public static void ExtractFrustumPlanes(
            in Matrix4x4 projectionMatrix, in Matrix4x4 worldToCameraMatrix, ref NativeArray<float4> planes)
        {
            // Assume that planes length must be six
            Assert.IsTrue(planes.IsCreated, "Planes array must be created");
            Assert.IsTrue(planes.Length == 6, "Planes array length must be 6");

            Matrix4x4 mat = projectionMatrix * worldToCameraMatrix;

            // Left
            planes[0] = new float4(mat.m30 + mat.m00, mat.m31 + mat.m01, mat.m32 + mat.m02, mat.m33 + mat.m03);
            // Right
            planes[1] = new float4(mat.m30 - mat.m00, mat.m31 - mat.m01, mat.m32 - mat.m02, mat.m33 - mat.m03);
            // Bottom
            planes[2] = new float4(mat.m30 + mat.m10, mat.m31 + mat.m11, mat.m32 + mat.m12, mat.m33 + mat.m13);
            // Top
            planes[3] = new float4(mat.m30 - mat.m10, mat.m31 - mat.m11, mat.m32 - mat.m12, mat.m33 - mat.m13);
            // Near
            planes[4] = new float4(mat.m30 + mat.m20, mat.m31 + mat.m21, mat.m32 + mat.m22, mat.m33 + mat.m23);
            // Far
            planes[5] = new float4(mat.m30 - mat.m20, mat.m31 - mat.m21, mat.m32 - mat.m22, mat.m33 - mat.m23);

            // Normalize planes (optional, for consistent distances)
            for (int i = 0; i < 6; i++)
            {
                float3 normal = new(planes[i].x, planes[i].y, planes[i].z);
                float length = math.length(normal);
                planes[i] /= length;
            }
        }
        
        /// <summary>
        ///     Check if a point is inside frustum. 
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PointInFrustum(
            in float3 point,
            in float4 left,
            in float4 right,
            in float4 top,
            in float4 bottom,
            in float4 nearP,
            in float4 farP)
        {
            return IsInside(point, left) &&
                   IsInside(point, right) &&
                   IsInside(point, top) &&
                   IsInside(point, bottom) &&
                   IsInside(point, nearP) &&
                   IsInside(point, farP);
        }
        
        /// <summary>
        ///     Quickly check if point is inside view frustrum
        /// </summary>
        [BurstCompile]
        public static bool PointInFrustum(in float3 point, in NativeArray<float4> planes)
        {
            Assert.IsTrue(planes.IsCreated, "Planes array must be created");
            Assert.AreEqual(6, planes.Length, "Planes array length must be 6");
            for (int i = 0; i < 6; i++)
            {
                if(!IsInside(point, planes[i])) return false;
            }
            return true;
        }
    }
}