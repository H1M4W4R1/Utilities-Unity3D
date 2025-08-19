using NUnit.Framework;
using Systems.Utilities.Indexing.Grid;
using Unity.Mathematics;

namespace Systems.Utilities.Tests.Indexing
{
    [TestFixture] public class Indexing3DTests
    {
        private GridInfo3D _gridInfo;

        [SetUp] public void Setup()
        {
            _gridInfo = new GridInfo3D(
                originPoint: new int3(0, 0, 0),
                size: new int3(10, 10, 10),
                worldOriginPoint: new float3(0f, 0f, 0f),
                tileSize: new float3(1f, 1f, 1f)
            );
        }

#region GridInfo3D Tests

        [Test] public void Constructor_ShouldInitializeFields()
        {
            Assert.AreEqual(new int3(0, 0, 0), _gridInfo.originPoint);
            Assert.AreEqual(new int3(10, 10, 10), _gridInfo.size);
            Assert.AreEqual(new float3(0f, 0f, 0f), _gridInfo.worldOriginPoint);
            Assert.AreEqual(new float3(1f, 1f, 1f), _gridInfo.tileSize);
        }

        [Test] public void GetWorldPositionRelative_ShouldReturnCorrectWorldPosition()
        {
            int3 relativePosition = new int3(5, 5, 5);
            float3 expectedWorldPosition = new float3(5f, 5f, 5f);

            float3 actualWorldPosition = _gridInfo.GetWorldPositionRelative(relativePosition);

            Assert.AreEqual(expectedWorldPosition, actualWorldPosition);
        }

        [Test] public void GetWorldPositionAbsolute_ShouldReturnCorrectWorldPosition()
        {
            int3 absolutePosition = new int3(5, 5, 5);
            float3 expectedWorldPosition = new float3(5f, 5f, 5f);

            float3 actualWorldPosition = _gridInfo.GetWorldPositionAbsolute(absolutePosition);

            Assert.AreEqual(expectedWorldPosition, actualWorldPosition);
        }

        [Test] public void GetRelativePositionFromWorld_ShouldReturnCorrectRelativePosition()
        {
            float3 worldPosition = new float3(5.5f, 5.5f, 5.5f);
            int3 expectedRelativePosition = new int3(6, 6, 6);

            int3 actualRelativePosition = _gridInfo.GetRelativePositionFromWorld(worldPosition);

            Assert.AreEqual(expectedRelativePosition, actualRelativePosition);
        }

        [Test] public void GetAbsolutePositionFromWorld_ShouldReturnCorrectAbsolutePosition()
        {
            float3 worldPosition = new float3(5.5f, 5.5f, 5.5f);
            int3 expectedAbsolutePosition = new int3(6, 6, 6);

            int3 actualAbsolutePosition = _gridInfo.GetAbsolutePositionFromWorld(worldPosition);

            Assert.AreEqual(expectedAbsolutePosition, actualAbsolutePosition);
        }

#endregion

#region Index3D Constructors and Basic Properties

        [Test] public void Constructor_FromValue_ShouldStoreValue()
        {
            Index3D index = new Index3D(555);
            Assert.AreEqual(555, index.value);
        }

        [Test] public void Constructor_FromAbsoluteCoordinates_ShouldConvertCorrectly()
        {
            // Assuming x-major order: value = x * size.y * size.z + y * size.z + z
            Index3D index = new Index3D(2, 3, 4, _gridInfo);
            Assert.AreEqual(2 * 10 * 10 + 3 * 10 + 4, index.value);
        }

        [Test] public void Constructor_FromInt3AbsoluteCoordinates_ShouldConvertCorrectly()
        {
            Index3D index = new Index3D(new int3(2, 3, 4), _gridInfo);
            Assert.AreEqual(2 * 10 * 10 + 3 * 10 + 4, index.value);
        }

        [Test] public void ImplicitConversion_ToInt_ShouldReturnStoredValue()
        {
            Index3D index = new Index3D(42);
            int value = index;
            Assert.AreEqual(42, value);
        }

#endregion

#region Index3D Position and World Mapping

        [Test] public void GetAbsolutePosition_ShouldReturnCorrectAbsoluteTilePosition()
        {
            Index3D index = new Index3D(234);
            int3 expectedPosition = new int3(2, 3, 4);
            Assert.AreEqual(expectedPosition, index.GetAbsolutePosition(_gridInfo));
        }

        [Test] public void GetWorldPosition_ShouldReturnCorrectWorldPosition()
        {
            Index3D index = new Index3D(234);
            float3 expectedPosition = new float3(2f, 3f, 4f);
            Assert.AreEqual(expectedPosition, index.GetWorldPosition(_gridInfo));
        }

        [Test] public void ToIndexAbsolute_FromInt3_ShouldReturnCorrectIndex()
        {
            int3 pos = new int3(2, 3, 4);
            Assert.AreEqual(234, Index3D.ToIndexAbsolute(pos, _gridInfo));
        }

        [Test] public void ToIndexAbsolute_FromInts_ShouldReturnCorrectIndex()
        {
            Assert.AreEqual(234, Index3D.ToIndexAbsolute(2, 3, 4, _gridInfo));
        }

        [Test] public void ToIndexRelative_FromInt3_ShouldReturnCorrectIndex()
        {
            int3 pos = new int3(2, 3, 4);
            Assert.AreEqual(234, Index3D.ToIndexRelative(pos, _gridInfo));
        }

        [Test] public void ToIndexRelative_FromInts_ShouldReturnCorrectIndex()
        {
            Assert.AreEqual(234, Index3D.ToIndexRelative(2, 3, 4, _gridInfo));
        }

        [Test] public void FromIndexAbsolute_ShouldReturnCorrectAbsoluteCoordinates_Int3()
        {
            Index3D.FromIndexAbsolute(234, _gridInfo, out int3 pos);
            Assert.AreEqual(new int3(2, 3, 4), pos);
        }

        [Test] public void FromIndexAbsolute_ShouldReturnCorrectAbsoluteCoordinates_Ints()
        {
            Index3D.FromIndexAbsolute(234, _gridInfo, out int x, out int y, out int z);
            Assert.AreEqual(2, x);
            Assert.AreEqual(3, y);
            Assert.AreEqual(4, z);
        }

        [Test] public void FromIndexRelative_ShouldReturnCorrectRelativeCoordinates_Int3()
        {
            Index3D.FromIndexRelative(234, _gridInfo, out int3 pos);
            Assert.AreEqual(new int3(2, 3, 4), pos);
        }

        [Test] public void FromIndexRelative_ShouldReturnCorrectRelativeCoordinates_Ints()
        {
            Index3D.FromIndexRelative(234, _gridInfo, out int x, out int y, out int z);
            Assert.AreEqual(2, x);
            Assert.AreEqual(3, y);
            Assert.AreEqual(4, z);
        }

#endregion

#region Index3D Neighbor Methods

        private Index3D _centerIndex;

        [SetUp] public void NeighborTestSetup()
        {
            _centerIndex = new Index3D(5, 5, 5, _gridInfo);
        }

        // Axis
        [Test] public void GetNorthIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 6, 5, _gridInfo), _centerIndex.GetNorthIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 9, 5, _gridInfo).GetNorthIndex3D(_gridInfo));
        }

        [Test] public void GetSouthIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 4, 5, _gridInfo), _centerIndex.GetSouthIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 0, 5, _gridInfo).GetSouthIndex3D(_gridInfo));
        }

        [Test] public void GetEastIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 5, 5, _gridInfo), _centerIndex.GetEastIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 5, 5, _gridInfo).GetEastIndex3D(_gridInfo));
        }

        [Test] public void GetWestIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 5, 5, _gridInfo), _centerIndex.GetWestIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 5, 5, _gridInfo).GetWestIndex3D(_gridInfo));
        }

        [Test] public void GetUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 5, 6, _gridInfo), _centerIndex.GetUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 5, 9, _gridInfo).GetUpIndex3D(_gridInfo));
        }

        [Test] public void GetDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 5, 4, _gridInfo), _centerIndex.GetDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 5, 0, _gridInfo).GetDownIndex3D(_gridInfo));
        }

        // XY Plane Diagonals
        [Test] public void GetNorthEastIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 6, 5, _gridInfo), _centerIndex.GetNorthEastIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 9, 5, _gridInfo).GetNorthEastIndex3D(_gridInfo));
        }

        [Test] public void GetNorthWestIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 6, 5, _gridInfo), _centerIndex.GetNorthWestIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 9, 5, _gridInfo).GetNorthWestIndex3D(_gridInfo));
        }

        [Test] public void GetSouthEastIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 4, 5, _gridInfo), _centerIndex.GetSouthEastIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 0, 5, _gridInfo).GetSouthEastIndex3D(_gridInfo));
        }

        [Test] public void GetSouthWestIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 4, 5, _gridInfo), _centerIndex.GetSouthWestIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 0, 5, _gridInfo).GetSouthWestIndex3D(_gridInfo));
        }

        // XZ Plane Diagonals
        [Test] public void GetEastUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 5, 6, _gridInfo), _centerIndex.GetEastUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 5, 9, _gridInfo).GetEastUpIndex3D(_gridInfo));
        }

        [Test] public void GetWestUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 5, 6, _gridInfo), _centerIndex.GetWestUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 5, 9, _gridInfo).GetWestUpIndex3D(_gridInfo));
        }

        [Test] public void GetEastDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 5, 4, _gridInfo), _centerIndex.GetEastDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 5, 0, _gridInfo).GetEastDownIndex3D(_gridInfo));
        }

        [Test] public void GetWestDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 5, 4, _gridInfo), _centerIndex.GetWestDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 5, 0, _gridInfo).GetWestDownIndex3D(_gridInfo));
        }

        // YZ Plane Diagonals
        [Test] public void GetNorthUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 6, 6, _gridInfo), _centerIndex.GetNorthUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 9, 9, _gridInfo).GetNorthUpIndex3D(_gridInfo));
        }

        [Test] public void GetSouthUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 4, 6, _gridInfo), _centerIndex.GetSouthUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 0, 9, _gridInfo).GetSouthUpIndex3D(_gridInfo));
        }

        [Test] public void GetNorthDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 6, 4, _gridInfo), _centerIndex.GetNorthDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 9, 0, _gridInfo).GetNorthDownIndex3D(_gridInfo));
        }

        [Test] public void GetSouthDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(5, 4, 4, _gridInfo), _centerIndex.GetSouthDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(5, 0, 0, _gridInfo).GetSouthDownIndex3D(_gridInfo));
        }

        // Corners
        [Test] public void GetNorthEastUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 6, 6, _gridInfo), _centerIndex.GetNorthEastUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 9, 9, _gridInfo).GetNorthEastUpIndex3D(_gridInfo));
        }

        [Test] public void GetNorthWestUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 6, 6, _gridInfo), _centerIndex.GetNorthWestUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 9, 9, _gridInfo).GetNorthWestUpIndex3D(_gridInfo));
        }

        [Test] public void GetSouthEastUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 4, 6, _gridInfo), _centerIndex.GetSouthEastUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 0, 9, _gridInfo).GetSouthEastUpIndex3D(_gridInfo));
        }

        [Test] public void GetSouthWestUpIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 4, 6, _gridInfo), _centerIndex.GetSouthWestUpIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 0, 9, _gridInfo).GetSouthWestUpIndex3D(_gridInfo));
        }

        [Test] public void GetNorthEastDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 6, 4, _gridInfo),
                _centerIndex.GetNorthEastDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 9, 0, _gridInfo).GetNorthEastDownIndex3D(_gridInfo));
        }

        [Test] public void GetNorthWestDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 6, 4, _gridInfo),
                _centerIndex.GetNorthWestDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 9, 0, _gridInfo).GetNorthWestDownIndex3D(_gridInfo));
        }

        [Test] public void GetSouthEastDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(6, 4, 4, _gridInfo),
                _centerIndex.GetSouthEastDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(9, 0, 0, _gridInfo).GetSouthEastDownIndex3D(_gridInfo));
        }

        [Test] public void GetSouthWestDownIndex3D_ShouldReturnCorrectNeighborOrNone()
        {
            Assert.AreEqual((int) new Index3D(4, 4, 4, _gridInfo),
                _centerIndex.GetSouthWestDownIndex3D(_gridInfo));
            Assert.AreEqual(Index3D.NONE, new Index3D(0, 0, 0, _gridInfo).GetSouthWestDownIndex3D(_gridInfo));
        }

#endregion
    }
}