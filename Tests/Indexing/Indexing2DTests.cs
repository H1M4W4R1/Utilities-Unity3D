using NUnit.Framework;
using Systems.Utilities.Indexing.Grid;
using Unity.Mathematics;

namespace Systems.Utilities.Tests.Indexing
{
    [TestFixture]
    public class Indexing2DTests
    {
        private GridInfo2D _gridInfo;

        [SetUp]
        public void Setup()
        {
            _gridInfo = new GridInfo2D(
                originPoint: new int2(0, 0),
                size: new int2(10, 10),
                worldOriginPoint: new float2(0f, 0f),
                tileSize: new float2(1f, 1f)
            );
        }

        #region GridInfo2D Tests

        [Test]
        public void Constructor_ShouldInitializeFields()
        {
            Assert.AreEqual(new int2(0, 0), _gridInfo.originPoint);
            Assert.AreEqual(new int2(10, 10), _gridInfo.size);
            Assert.AreEqual(new float2(0f, 0f), _gridInfo.worldOriginPoint);
            Assert.AreEqual(new float2(1f, 1f), _gridInfo.tileSize);
        }

        [Test]
        public void GetWorldPositionRelative_ShouldReturnCorrectWorldPosition()
        {
            int2 relativePosition = new int2(5, 5);
            float2 expectedWorldPosition = new float2(5f, 5f);
            
            float2 actualWorldPosition = _gridInfo.GetWorldPositionRelative(relativePosition);
            
            Assert.AreEqual(expectedWorldPosition, actualWorldPosition);
        }

        [Test]
        public void GetWorldPositionAbsolute_ShouldReturnCorrectWorldPosition()
        {
            int2 absolutePosition = new int2(5, 5);
            float2 expectedWorldPosition = new float2(5f, 5f);

            float2 actualWorldPosition = _gridInfo.GetWorldPositionAbsolute(absolutePosition);

            Assert.AreEqual(expectedWorldPosition, actualWorldPosition);
        }

        [Test]
        public void GetRelativePositionFromWorld_ShouldReturnCorrectRelativePosition()
        {
            float2 worldPosition = new float2(5.5f, 5.5f);
            int2 expectedRelativePosition = new int2(6, 6);

            int2 actualRelativePosition = _gridInfo.GetRelativePositionFromWorld(worldPosition);

            Assert.AreEqual(expectedRelativePosition, actualRelativePosition);
        }

        [Test]
        public void GetAbsolutePositionFromWorld_ShouldReturnCorrectAbsolutePosition()
        {
            float2 worldPosition = new float2(5.5f, 5.5f);
            int2 expectedAbsolutePosition = new int2(6, 6);

            int2 actualAbsolutePosition = _gridInfo.GetAbsolutePositionFromWorld(worldPosition);

            Assert.AreEqual(expectedAbsolutePosition, actualAbsolutePosition);
        }

        #endregion

        #region Index2D Constructors and Basic Properties

        [Test]
        public void Constructor_FromValue_ShouldStoreValue()
        {
            Index2D index = new Index2D(50);
            Assert.AreEqual(50, index.value);
        }

        [Test]
        public void Constructor_FromAbsoluteCoordinates_ShouldConvertCorrectly()
        {
            Index2D index = new Index2D(2, 3, _gridInfo);
            Assert.AreEqual(23, index.value);
        }

        [Test]
        public void Constructor_FromInt2AbsoluteCoordinates_ShouldConvertCorrectly()
        {
            Index2D index = new Index2D(new int2(2, 3), _gridInfo);
            Assert.AreEqual(23, index.value);
        }

        [Test]
        public void ImplicitConversion_ToInt_ShouldReturnStoredValue()
        {
            Index2D index = new Index2D(42);
            int value = index;
            Assert.AreEqual(42, value);
        }

        #endregion

        #region Index2D Position and World Mapping

        [Test]
        public void GetAbsolutePosition_ShouldReturnCorrectAbsoluteTilePosition()
        {
            Index2D index = new Index2D(23);
            int2 expectedPosition = new int2(2, 3);
            Assert.AreEqual(expectedPosition, index.GetAbsolutePosition(_gridInfo));
        }

        [Test]
        public void GetWorldPosition_ShouldReturnCorrectWorldPosition()
        {
            Index2D index = new Index2D(23);
            float2 expectedPosition = new float2(2f, 3f);
            Assert.AreEqual(expectedPosition, index.GetWorldPosition(_gridInfo));
        }

        [Test]
        public void ToIndexAbsolute_FromInt2_ShouldReturnCorrectIndex()
        {
            int2 pos = new int2(2, 3);
            Assert.AreEqual(23, Index2D.ToIndexAbsolute(pos, _gridInfo));
        }

        [Test]
        public void ToIndexAbsolute_FromInts_ShouldReturnCorrectIndex()
        {
            Assert.AreEqual(23, Index2D.ToIndexAbsolute(2, 3, _gridInfo));
        }

        [Test]
        public void ToIndexRelative_FromInt2_ShouldReturnCorrectIndex()
        {
            int2 pos = new int2(2, 3);
            Assert.AreEqual(23, Index2D.ToIndexRelative(pos, _gridInfo));
        }

        [Test]
        public void ToIndexRelative_FromInts_ShouldReturnCorrectIndex()
        {
            Assert.AreEqual(23, Index2D.ToIndexRelative(2, 3, _gridInfo));
        }

        [Test]
        public void FromIndexAbsolute_ShouldReturnCorrectAbsoluteCoordinates_Int2()
        {
            Index2D.FromIndexAbsolute(23, _gridInfo, out int2 pos);
            Assert.AreEqual(new int2(2, 3), pos);
        }

        [Test]
        public void FromIndexAbsolute_ShouldReturnCorrectAbsoluteCoordinates_Ints()
        {
            Index2D.FromIndexAbsolute(23, _gridInfo, out int x, out int y);
            Assert.AreEqual(2, x);
            Assert.AreEqual(3, y);
        }

        [Test]
        public void FromIndexRelative_ShouldReturnCorrectRelativeCoordinates_Int2()
        {
            Index2D.FromIndexRelative(23, _gridInfo, out int2 pos);
            Assert.AreEqual(new int2(2, 3), pos);
        }

        [Test]
        public void FromIndexRelative_ShouldReturnCorrectRelativeCoordinates_Ints()
        {
            Index2D.FromIndexRelative(23, _gridInfo, out int x, out int y);
            Assert.AreEqual(2, x);
            Assert.AreEqual(3, y);
        }

        #endregion

        #region Index2D Neighbor Methods

        [Test]
        public void GetNorthIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(56, centerIndex.GetNorthIndex2D(_gridInfo));

            Index2D topEdgeIndex = new Index2D(9); // (0,9)
            Assert.AreEqual(Index2D.NONE, topEdgeIndex.GetNorthIndex2D(_gridInfo));
        }

        [Test]
        public void GetSouthIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(54, centerIndex.GetSouthIndex2D(_gridInfo));

            Index2D bottomEdgeIndex = new Index2D(0); // (0,0)
            Assert.AreEqual(Index2D.NONE, bottomEdgeIndex.GetSouthIndex2D(_gridInfo));
        }

        [Test]
        public void GetEastIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(65, centerIndex.GetEastIndex2D(_gridInfo));

            Index2D rightEdgeIndex = new Index2D(95); // (9,5)
            Assert.AreEqual(Index2D.NONE, rightEdgeIndex.GetEastIndex2D(_gridInfo));
        }

        [Test]
        public void GetWestIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(45, centerIndex.GetWestIndex2D(_gridInfo));

            Index2D leftEdgeIndex = new Index2D(5); // (0,5)
            Assert.AreEqual(Index2D.NONE, leftEdgeIndex.GetWestIndex2D(_gridInfo));
        }

        [Test]
        public void GetNorthEastIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(66, centerIndex.GetNorthEastIndex2D(_gridInfo));

            Index2D topRightCorner = new Index2D(99); // (9,9)
            Assert.AreEqual(Index2D.NONE, topRightCorner.GetNorthEastIndex2D(_gridInfo));
        }

        [Test]
        public void GetNorthWestIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(46, centerIndex.GetNorthWestIndex2D(_gridInfo));

            Index2D topLeftCorner = new Index2D(9); // (0,9)
            Assert.AreEqual(Index2D.NONE, topLeftCorner.GetNorthWestIndex2D(_gridInfo));
        }

        [Test]
        public void GetSouthEastIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(64, centerIndex.GetSouthEastIndex2D(_gridInfo));

            Index2D bottomRightCorner = new Index2D(90); // (9,0)
            Assert.AreEqual(Index2D.NONE, bottomRightCorner.GetSouthEastIndex2D(_gridInfo));
        }

        [Test]
        public void GetSouthWestIndex2D_ShouldReturnCorrectNeighborOrNone()
        {
            Index2D centerIndex = new Index2D(55); // (5,5)
            Assert.AreEqual(44, centerIndex.GetSouthWestIndex2D(_gridInfo));

            Index2D bottomLeftCorner = new Index2D(0); // (0,0)
            Assert.AreEqual(Index2D.NONE, bottomLeftCorner.GetSouthWestIndex2D(_gridInfo));
        }

        #endregion
    }
}
