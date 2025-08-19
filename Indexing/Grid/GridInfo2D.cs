using Unity.Burst;
using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Core information about 2-dimensional grid
    /// </summary>
    public readonly struct GridInfo2D
    {
        public readonly int2 originPoint; // 8B
        public readonly int2 size; // 8B
        public readonly float2 worldOriginPoint; // 8B
        public readonly float2 tileSize; // 8B

        public GridInfo2D(int2 originPoint, int2 size, float2 worldOriginPoint, float2 tileSize)
        {
            this.originPoint = originPoint;
            this.size = size;
            this.worldOriginPoint = worldOriginPoint;
            this.tileSize = tileSize;
        }

        /// <summary>
        ///     Computes world position of a node (using relative input)
        /// </summary>
        [BurstCompile] public float2 GetWorldPositionRelative(int2 relativeCellPosition)
            => worldOriginPoint + tileSize * relativeCellPosition;

        /// <summary>
        ///     Computes world position of a node (using absolute input)
        /// </summary>
        [BurstCompile] public float2 GetWorldPositionAbsolute(int2 absoluteCellPosition) =>
            GetWorldPositionRelative(absoluteCellPosition - originPoint);
        
        /// <summary>
        ///     Computes relative position from world one
        /// </summary>
        [BurstCompile] public int2 GetRelativePositionFromWorld(float2 worldPosition)
        {
            // Transform world position into grid-local space
            float2 local = (worldPosition - worldOriginPoint) / tileSize;

            // Round/truncate to nearest integer cell index
            return (int2) math.round(local);
        }
        
        /// <summary>
        ///     Computes absolute position from world one
        /// </summary>
        [BurstCompile]
        public int2 GetAbsolutePositionFromWorld(float2 worldPosition)
        {
            // Get relative first
            int2 relative = GetRelativePositionFromWorld(worldPosition);

            // Convert to absolute by adding origin offset
            return relative + originPoint;
        }
    }
}