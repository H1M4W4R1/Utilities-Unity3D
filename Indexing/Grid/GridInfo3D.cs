using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Core information about 3-dimensional grid
    /// </summary>
    public readonly struct GridInfo3D
    {
        public readonly int3 originPoint; // 12B
        public readonly int3 size; // 12B
        public readonly float3 worldOriginPoint; // 12B
        public readonly float3 tileSize; // 12B

        public GridInfo3D(int3 originPoint, int3 size, float3 worldOriginPoint, float3 tileSize)
        {
            this.originPoint = originPoint;
            this.size = size;
            this.worldOriginPoint = worldOriginPoint;
            this.tileSize = tileSize;
        }
        
        /// <summary>
        ///     Computes world position of a node (using relative input)
        /// </summary>
        [BurstCompile] public float3 GetWorldPositionRelative(int3 relativeCellPosition)
            => worldOriginPoint + tileSize * relativeCellPosition;

        /// <summary>
        ///     Computes world position of a node (using absolute input)
        /// </summary>
        [BurstCompile] public float3 GetWorldPositionAbsolute(int3 absoluteCellPosition) =>
            GetWorldPositionRelative(absoluteCellPosition - originPoint);

        /// <summary>
        ///     Computes relative position from world one
        /// </summary>
        [BurstCompile] public int3 GetRelativePositionFromWorld(float3 worldPosition)
        {
            // Transform world position into grid-local space
            float3 local = (worldPosition - worldOriginPoint) / tileSize;

            // Round/truncate to nearest integer cell index
            return (int3) math.round(local);
        }
        
        /// <summary>
        ///     Computes absolute position from world one
        /// </summary>
        [BurstCompile]
        public int3 GetAbsolutePositionFromWorld(float3 worldPosition)
        {
            // Get relative first
            int3 relative = GetRelativePositionFromWorld(worldPosition);

            // Convert to absolute by adding origin offset
            return relative + originPoint;
        }
    }
}