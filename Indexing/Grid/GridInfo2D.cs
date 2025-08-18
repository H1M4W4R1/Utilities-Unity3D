using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Core information about 2-dimensional grid
    /// </summary>
    public struct GridInfo2D
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
    }
}