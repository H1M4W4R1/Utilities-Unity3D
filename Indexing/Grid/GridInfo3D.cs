using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Core information about 3-dimensional grid
    /// </summary>
    public struct GridInfo3D
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
    }
}