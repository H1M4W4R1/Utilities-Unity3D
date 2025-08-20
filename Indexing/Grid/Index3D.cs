using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;

namespace Systems.Utilities.Indexing.Grid
{
    /// <summary>
    ///     Index of cell in 3D grid space
    /// </summary>
    public readonly struct Index3D
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
        public Index3D(int value)
        {
            this.value = value;
        }

        /// <summary>
        ///     Create new tile index from tile position (absolute)
        /// </summary>
        public Index3D(int x, int y, int z, in GridInfo3D gridInfo) :
            this(ToIndexAbsolute(x, y, z, gridInfo))
        {
        }

        /// <summary>
        ///     Create new tile index from tile position (absolute)
        /// </summary>
        public Index3D(int3 tilePosition, in GridInfo3D gridInfo) :
            this(ToIndexAbsolute(tilePosition.x, tilePosition.y, tilePosition.z, gridInfo))
        {
        }

        /// <summary>
        ///     Get tilemap position from this index
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int3 GetAbsolutePosition(in GridInfo3D gridInfo)
        {
            FromIndexAbsolute(value, gridInfo, out int3 result);
            return result;
        }

        /// <summary>
        ///     Get world location of this tile
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float3
            GetWorldPosition(in GridInfo3D gridInfo)
        {
            // Convert back into tilemap position
            FromIndexAbsolute(value, gridInfo, out int3 result);

            // Convert into world position
            result -= gridInfo.originPoint; // We move by origin point to get offset of the tile from origin
            return gridInfo.worldOriginPoint + gridInfo.tileSize * result;
        }

        /// <summary>
        /// Converts 3D coordinates (x, y, z) into a 1D tile index.
        /// Uses absolute coordinates of a tile
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexAbsolute(in int3 tilePosition, in GridInfo3D gridInfo) =>
            ToIndexAbsolute(tilePosition.x, tilePosition.y, tilePosition.z, gridInfo);

        /// <summary>
        /// Converts 3D coordinates (x, y, z) into a 1D tile index.
        /// Uses absolute coordinates of a tile
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexAbsolute(int x, int y, int z, in GridInfo3D gridInfo)
        {
            // Compute real tilemap offset, we subtract origin point
            // to orient index values around left bottom corner of Tilemap
            int3 tilemapOffset = new int3(x, y, z) - new int3(gridInfo.originPoint.x, gridInfo.originPoint.y,
                gridInfo.originPoint.z);

            return ToIndexRelative(tilemapOffset, gridInfo);
        }

        /// <summary>
        /// Converts 3D coordinates (x, y, z) into a 1D tile index.
        /// Uses relative coordinates of a tile
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexRelative(int x, int y, int z, in GridInfo3D gridInfo)
            => ToIndexRelative(new int3(x, y, z), gridInfo);

        /// <summary>
        /// Converts 3D coordinates (x, y, z) into a 1D tile index.
        /// Uses relative coordinates of a tile
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToIndexRelative(in int3 tileOffset, in GridInfo3D gridInfo)
            => (tileOffset.x * gridInfo.size.y * gridInfo.size.z) +
               (tileOffset.y * gridInfo.size.z) + tileOffset.z;


        /// <summary>
        /// Converts 1D tile index back into absolute 3D coordinates (x, y, z).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromIndexAbsolute(int index, in GridInfo3D gridInfo, out int3 tilePosition)
        {
            FromIndexAbsolute(index, gridInfo, out int x, out int y, out int z);

            // Re-apply origin
            tilePosition = new int3(x, y, z);
        }

        /// <summary>
        /// Converts 1D tile index back into absolute 3D coordinates (x, y, z).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexAbsolute(
            int index,
            in GridInfo3D gridInfo,
            out int x,
            out int y,
            out int z)
        {
            FromIndexRelative(index, gridInfo, out x, out y, out z);

            // Re-apply origin
            x += gridInfo.originPoint.x;
            y += gridInfo.originPoint.y;
            z += gridInfo.originPoint.z;
        }

        /// <summary>
        /// Converts 1D tile index back into relative 3D coordinates (x, y, z).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexRelative(
            int index,
            in GridInfo3D gridInfo,
            out int3 tileOffset)
        {
            FromIndexRelative(index, gridInfo, out int x, out int y, out int z);
            tileOffset = new int3(x, y, z);
        }

        /// <summary>
        /// Converts 1D tile index back into relative 3D coordinates (x, y, z).
        /// </summary>
        /// <remarks>
        ///     The only faster way I know is to make this use sizes that are powers of 2
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void FromIndexRelative(
            int index,
            in GridInfo3D gridInfo,
            out int x,
            out int y,
            out int z)
        {
            int sizeZ = gridInfo.size.z;
            int sizeYZ = gridInfo.size.y * sizeZ;
            
            x = index / sizeYZ;
            
            int remainder = index - x * sizeYZ;
            y = remainder / sizeZ;
            z = remainder - y * sizeZ;
        }

        public static implicit operator int(Index3D tileIndex) => tileIndex.value;

#region NEIGHBORS

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, +1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, -1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetEastIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, 0, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetWestIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, 0, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthEastIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, +1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthWestIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, +1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthEastIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, -1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthWestIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, -1, 0, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetUpIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, 0, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, 0, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetEastUpIndex3D(in GridInfo3D gridInfo) =>
            GetNeighborIndex3D(+1, 0, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetEastDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, 0, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetWestUpIndex3D(in GridInfo3D gridInfo) =>
            GetNeighborIndex3D(-1, 0, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetWestDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, 0, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthUpIndex3D(in GridInfo3D gridInfo) =>
            GetNeighborIndex3D(0, +1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, +1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthUpIndex3D(in GridInfo3D gridInfo) =>
            GetNeighborIndex3D(0, -1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(0, -1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthEastUpIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, +1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNorthEastDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, +1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetNorthWestUpIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, +1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNorthWestDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, +1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthEastUpIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, -1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSouthEastDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(+1, -1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int GetSouthWestUpIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, -1, +1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetSouthWestDownIndex3D(in GridInfo3D gridInfo)
            => GetNeighborIndex3D(-1, -1, -1, gridInfo);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetNeighborIndex3D(int3 direction, in GridInfo3D gridInfo)
            => GetNeighborIndex3D(direction.x, direction.y, direction.z, gridInfo);

        [BurstCompile]
        public int GetNeighborIndex3D(int dx, int dy, int dz, in GridInfo3D gridInfo)
        {
            FromIndexRelative(value, gridInfo, out int x, out int y, out int z);

            int nx = x + dx;
            int ny = y + dy;
            int nz = z + dz;

            return Hint.Likely(nx >= 0 && nx < gridInfo.size.x && ny >= 0 && ny < gridInfo.size.y &&
                               nz >= 0 && nz < gridInfo.size.z)
                ? ToIndexRelative(nx, ny, nz, gridInfo)
                : NONE;
        }

#endregion
    }
}