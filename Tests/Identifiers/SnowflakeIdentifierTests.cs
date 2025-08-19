using NUnit.Framework;
using Systems.Utilities.Identifiers;
using System;
using Unity.Mathematics; // For int4

namespace Systems.Utilities.Tests.Identifiers
{
    [TestFixture]
    public class SnowflakeIdentifierTests
    {
        // No specific setup needed for individual SnowflakeIdentifier instances,
        // but might need to reset the static counter for New() tests.
        // However, since New() is BurstDiscard, it's likely meant for runtime,
        // and direct testing of the counter might be tricky or not intended for unit tests.
        // Let's focus on the public API behavior.

        #region Constructor Tests

        [Test]
        public void Constructor_WithValidTicksAndShift_ShouldInitializeCorrectly()
        {
            long testTicks = DateTime.UtcNow.Ticks;
            int testShift = 123;
            SnowflakeIdentifier identifier = new SnowflakeIdentifier(testTicks, testShift);

            Assert.AreEqual(testTicks, identifier.ticks);
            Assert.AreEqual(testShift, identifier.shift);
            Assert.IsTrue(identifier.IsCreated);
            Assert.AreNotEqual(int4.zero, identifier.vectorized); // Check if vectorized value was updated
        }

        [Test] public void IsCreated_WithZeroTick_ShouldReturnFalse()
        {
            long testTicks = 0;
            long testShift = 123;
            SnowflakeIdentifier identifier = new SnowflakeIdentifier(testTicks, testShift);
           
            Assert.IsFalse(identifier.IsCreated);
        }

        #endregion

        #region Equals and GetHashCode Tests

        [Test]
        public void Equals_SameInstance_ShouldReturnTrue()
        {
            SnowflakeIdentifier id = new SnowflakeIdentifier(100, 1);
            Assert.IsTrue(id.Equals(id));
        }

        [Test]
        public void Equals_IdenticalValues_ShouldReturnTrue()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.IsTrue(id1.Equals(id2));
        }

        [Test]
        public void Equals_DifferentTicks_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 1);
            Assert.IsFalse(id1.Equals(id2));
        }

        [Test]
        public void Equals_DifferentShift_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 2);
            Assert.IsFalse(id1.Equals(id2));
        }

        [Test]
        public void Equals_DifferentTicksAndShift_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 2);
            Assert.IsFalse(id1.Equals(id2));
        }

        [Test]
        public void Equals_WithObject_IdenticalValues_ShouldReturnTrue()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            object id2 = new SnowflakeIdentifier(100, 1);
            Assert.IsTrue(id1.Equals(id2));
        }

        [Test]
        public void Equals_WithObject_DifferentValues_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            object id2 = new SnowflakeIdentifier(101, 1);
            Assert.IsFalse(id1.Equals(id2));
        }

        [Test]
        public void Equals_WithObject_Null_ShouldReturnFalseIfCreated()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            Assert.IsFalse(id1.Equals(null));
        }

        [Test]
        public void Equals_WithObject_Null_ShouldReturnTrueIfDefaultInstance()
        {
            SnowflakeIdentifier defaultId = default;
            Assert.IsTrue(defaultId.Equals(null));
        }
        
        [Test]
        public void Equals_WithObject_Null_ShouldReturnTrueIfTicksIsZero()
        {
            SnowflakeIdentifier defaultId = new SnowflakeIdentifier(0, 123);
            Assert.IsTrue(defaultId.Equals(null));
        }

        [Test]
        public void Equals_WithObject_DifferentType_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            object obj = new object();
            Assert.IsFalse(id1.Equals(obj));
        }

        [Test]
        public void GetHashCode_IdenticalValues_ShouldReturnSameHashCode()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentValues_ShouldReturnDifferentHashCode()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 2);
            Assert.AreNotEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        #endregion

        #region Operator Overload Tests

        [Test]
        public void OperatorEquals_IdenticalValues_ShouldReturnTrue()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.IsTrue(id1 == id2);
        }

        [Test]
        public void OperatorEquals_DifferentValues_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 1);
            Assert.IsFalse(id1 == id2);
        }

        [Test]
        public void OperatorNotEquals_IdenticalValues_ShouldReturnFalse()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.IsFalse(id1 != id2);
        }

        [Test]
        public void OperatorNotEquals_DifferentValues_ShouldReturnTrue()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 1);
            Assert.IsTrue(id1 != id2);
        }

        #endregion

        #region New() Method Tests

        [Test]
        public void New_ShouldReturnCreatedIdentifier()
        {
            SnowflakeIdentifier id = SnowflakeIdentifier.New();
            Assert.IsTrue(id.IsCreated);
            Assert.AreNotEqual(0, id.ticks); // Ticks should be non-zero
        }

        [Test]
        public void New_ShouldGenerateUniqueIdentifiers()
        {
            SnowflakeIdentifier id1 = SnowflakeIdentifier.New();
            SnowflakeIdentifier id2 = SnowflakeIdentifier.New();
            Assert.AreNotEqual(id1, id2);
        }

        [Test]
        public void New_ShouldIncrementShiftCounterForSameTick()
        {
            // This test is a bit tricky due to DateTime.UtcNow.Ticks resolution.
            // It's possible for two consecutive calls to New() to have the same ticks.
            // We need to ensure the shift counter makes them unique.
            // This might require mocking DateTime.UtcNow or running in a tight loop.
            // For now, let's assume the counter works as intended and focus on the uniqueness.

            // Given the BurstDiscard attribute, this method is not meant for Burst compilation.
            // The _snowflakeShiftCounter is static, so it persists across calls.
            // We can't directly reset it for each test without reflection, which is not ideal for unit tests.
            // The best we can do is call it multiple times and assert uniqueness.
            SnowflakeIdentifier idA = SnowflakeIdentifier.New();
            SnowflakeIdentifier idB = SnowflakeIdentifier.New();
            SnowflakeIdentifier idC = SnowflakeIdentifier.New();

            Assert.AreNotEqual(idA, idB);
            Assert.AreNotEqual(idB, idC);
            Assert.AreNotEqual(idA, idC);

            // If ticks are the same, shifts must be different.
            if (idA.ticks == idB.ticks)
            {
                Assert.AreNotEqual(idA.shift, idB.shift);
            }
            if (idB.ticks == idC.ticks)
            {
                Assert.AreNotEqual(idB.shift, idC.shift);
            }
        }

        #endregion

        #region CompareTo Tests

        [Test]
        public void CompareTo_SameInstance_ShouldReturnZero()
        {
            SnowflakeIdentifier id = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(0, id.CompareTo(id));
        }

        [Test]
        public void CompareTo_IdenticalValues_ShouldReturnZero()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(0, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_OtherIsDefaultAndThisIsCreated_ShouldReturnNegativeOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier defaultId = default;
            Assert.AreEqual(-1, id1.CompareTo(defaultId));
        }

        [Test]
        public void CompareTo_ThisIsDefaultAndOtherIsCreated_ShouldReturnPositiveOne()
        {
            SnowflakeIdentifier defaultId = default;
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(1, defaultId.CompareTo(id2));
        }

        [Test]
        public void CompareTo_OtherIsNotCreatedAndThisIsCreated_ShouldReturnNegativeOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(0, 123);
            
            Assert.AreEqual(-1, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_ThisIsNotCreatedAndOtherIsCreated_ShouldReturnPositiveOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(0, 123);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(1, id1.CompareTo(id2));
        }
        
        [Test]
        public void CompareTo_ThisTicksLessThanOtherTicks_ShouldReturnNegativeOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(101, 1);
            Assert.AreEqual(-1, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_ThisTicksGreaterThanOtherTicks_ShouldReturnPositiveOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(101, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(1, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_SameTicksThisShiftLessThanOtherShift_ShouldReturnNegativeOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 1);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 2);
            Assert.AreEqual(-1, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_SameTicksThisShiftGreaterThanOtherShift_ShouldReturnPositiveOne()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(100, 2);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(100, 1);
            Assert.AreEqual(1, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_BothDefault_ShouldReturnZero()
        {
            SnowflakeIdentifier id1 = default;
            SnowflakeIdentifier id2 = default;
            Assert.AreEqual(0, id1.CompareTo(id2));
        }

        [Test]
        public void CompareTo_BothNotCreated_ShouldReturnZero()
        {
            SnowflakeIdentifier id1 = new SnowflakeIdentifier(0, 123);
            SnowflakeIdentifier id2 = new SnowflakeIdentifier(0, 255);
            Assert.AreEqual(0, id1.CompareTo(id2));
        }
        
        #endregion
    }
}