using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Index of cell in 2D grid space
    /// </summary>
    public readonly struct Index2D
    {
        // No tile...
        public const int NONE = -1;

        /// <summary>
        ///     Index value storage
        /// </summary>
        public readonly int value; // 4B

        /// <summary>
        ///     Create new tile index from value
        /// </summary>
        public Index2D(int value)
        {
            this.value = value;
        }

        /// <summary>
        ///     Create new tile index from tile position (absolute)
        /// </summary>
        public Index2D(int x, int y, in GridInfo2D gridInfo) :
            this(ToIndexAbsolute(x, y, gridInfo))
        {
        }

        /// <summary>
        ///     Create new tile index from tile position (absolute)
        /// </summary>
        public Index2D(int2 tilePosition, in GridInfo2D gridInfo) :
            this(ToIndexAbsolute(tilePosition.x, tilePosition.y, gridInfo))
        {
        }

        /// <summary>
        ///     Get tilemap position from this index
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int2 GetAbsolutePosition(in GridInfo2D gridInfo)
        {
            FromIndexAbsolute(value, gridInfo, out int2 result);
            return result;
        }

        /// <summary>
        ///     Get world location of this tile
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public float2
            GetWorldPosition(in GridInfo2D gridInfo)
        {
            // Convert back into tilemap position
            FromIndexAbsolute(value, gridInfo, out int2 result);

            // Convert into world position
            result -= gridInfo.originPoint; // We move by origin point to get offset of the tile from origin
            return gridInfo.worldOriginPoint + gridInfo.tileSize * result;
        }

        /// <summary>
        /// Converts 2D coordinates (x, y, z) into a 1D tile index.
        /// Uses absolute coordinates of a tile
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexAbsolute(in int2 tilePosition, in GridInfo2D gridInfo) =>
            ToIndexAbsolute(tilePosition.x, tilePosition.y, gridInfo);

        /// <summary>
        /// Converts 2D coordinates (x, y, z) into a 1D tile index.
        /// Uses absolute coordinates of a tile
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexAbsolute(int x, int y, in GridInfo2D gridInfo)
        {
            // Compute real tilemap offset, we subtract origin point
            // to orient index values around left bottom corner of Tilemap
            int2 tilemapOffset = new int2(x, y) - new int2(gridInfo.originPoint.x, gridInfo.originPoint.y);

            return ToIndexRelative(tilemapOffset, gridInfo);
        }

        /// <summary>
        /// Converts 2D coordinates (x, y, z) into a 1D tile index.
        /// Uses relative coordinates of a tile
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexRelative(int x, int y, in GridInfo2D gridInfo)
            => ToIndexRelative(new int2(x, y), gridInfo);

        /// <summary>
        /// Converts 2D coordinates (x, y, z) into a 1D tile index.
        /// Uses relative coordinates of a tile
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexRelative(in int2 tileOffset, in GridInfo2D gridInfo)
            => (tileOffset.x * gridInfo.size.y) + tileOffset.y;

        /// <summary>
        /// Converts 1D tile index back into absolute 2D coordinates (x, y, z).
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromIndexAbsolute(int index, in GridInfo2D gridInfo, out int2 tilePosition)
        {
            FromIndexAbsolute(index, gridInfo, out int x, out int y);

            // Re-apply origin
            tilePosition = new int2(x, y);
        }

        /// <summary>
        /// Converts 1D tile index back into absolute 2D coordinates (x, y, z).
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexAbsolute(
            int index,
            in GridInfo2D gridInfo,
            out int x,
            out int y)
        {
            FromIndexRelative(index, gridInfo, out x, out y);

            // Re-apply origin
            x += gridInfo.originPoint.x;
            y += gridInfo.originPoint.y;
        }

        /// <summary>
        /// Converts 1D tile index back into relative 2D coordinates (x, y, z).
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexRelative(
            int index,
            in GridInfo2D gridInfo,
            out int2 tileOffset)
        {
            FromIndexRelative(index, gridInfo, out int x, out int y);
            tileOffset = new int2(x, y);
        }

        /// <summary>
        /// Converts 1D tile index back into relative 2D coordinates (x, y, z).
        /// </summary>
        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexRelative(
            int index,
            in GridInfo2D gridInfo,
            out int x,
            out int y)
        {
            int sizeY = gridInfo.size.y;

            // Decode offsets
            x = index / sizeY;
            y = index % sizeY;
        }

        public static implicit operator int(Index2D tileIndex) => tileIndex.value;

#region NEIGHBORS

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNorthIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(0, +1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSouthIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(0, -1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetEastIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(+1, 0, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetWestIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(-1, 0, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNorthEastIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(+1, +1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNorthWestIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(-1, +1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSouthEastIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(+1, -1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSouthWestIndex2D(in GridInfo2D gridInfo)
            => GetNeighborIndex2D(-1, -1, gridInfo);

        [BurstCompile] [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNeighborIndex2D(int dx, int dy, in GridInfo2D gridInfo)
        {
            FromIndexRelative(value, gridInfo, out int x, out int y);

            int nx = x + dx;
            int ny = y + dy;

            return Hint.Likely(nx >= 0 && nx < gridInfo.size.x && ny >= 0 && ny < gridInfo.size.y)
                ? ToIndexRelative(nx, ny, gridInfo)
                : NONE;
        }

#endregion
    }
}