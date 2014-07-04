// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class LocalInstantTest
    {
        [Test]
        public void Equality()
        {
            LocalInstant equal = new LocalInstant(1, 100L);
            LocalInstant different1 = new LocalInstant(1, 200L);
            LocalInstant different2 = new LocalInstant(2, 100L);

            TestHelper.TestEqualsStruct(equal, equal, different1);
            TestHelper.TestOperatorEquality(equal, equal, different1);

            TestHelper.TestEqualsStruct(equal, equal, different2);
            TestHelper.TestOperatorEquality(equal, equal, different2);
        }

        [Test]
        public void Comparison()
        {
            LocalInstant equal = new LocalInstant(1, 100L);
            LocalInstant greater1 = new LocalInstant(1, 101L);
            LocalInstant greater2 = new LocalInstant(2, 0L);

            TestHelper.TestCompareToStruct(equal, equal, greater1);
            TestHelper.TestNonGenericCompareTo(equal, equal, greater1);
            TestHelper.TestOperatorComparisonEquality(equal, equal, greater1);

            TestHelper.TestCompareToStruct(equal, equal, greater2);
            TestHelper.TestNonGenericCompareTo(equal, equal, greater2);
            TestHelper.TestOperatorComparisonEquality(equal, equal, greater2);
        }

        #region operator +
        [Test]
        public void OperatorPlus_DurationZero_IsNeutralElement()
        {
            LocalInstant sample = new LocalInstant(1, 2345L);
            Assert.AreEqual(sample, sample + Duration.Zero);
        }

        [Test]
        public void OperatorPlus_DurationNonZero()
        {
            var simple = new LocalInstant(0, 3000000L) + Duration.Epsilon;
            Assert.AreEqual(0, simple.DaysSinceEpoch);
            Assert.AreEqual(3000001L, simple.NanosecondOfDay);

            var crossDay = new LocalInstant(1, NodaConstants.NanosecondsPerStandardDay - 1) + Duration.Epsilon;
            Assert.AreEqual(2, crossDay.DaysSinceEpoch);
            Assert.AreEqual(0, crossDay.NanosecondOfDay);
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinusDuration_Zero_IsNeutralElement()
        {
            LocalInstant sample = new LocalInstant(1, 2345L);
            Assert.AreEqual(sample, sample - Duration.Zero);
        }

        [Test]
        public void OperatorMinusDuration_NonZero()
        {
            var simple = new LocalInstant(0, 3000000L) - Duration.Epsilon;
            Assert.AreEqual(0, simple.DaysSinceEpoch);
            Assert.AreEqual(2999999L, simple.NanosecondOfDay);

            var crossDay = new LocalInstant(2, 0L) - Duration.Epsilon;
            Assert.AreEqual(1, crossDay.DaysSinceEpoch);
            Assert.AreEqual(NodaConstants.TicksPerStandardDay - 1, crossDay.NanosecondOfDay);
        }

        [Test]
        public void MinusOffset_Zero_IsNeutralElement()
        {
            Instant sampleInstant = new Instant(1, 23456L);
            LocalInstant sampleLocalInstant = new LocalInstant(1, 23456L);
            Assert.AreEqual(sampleInstant, sampleLocalInstant.Minus(Offset.Zero));
            Assert.AreEqual(sampleInstant, sampleLocalInstant.MinusZeroOffset());
        }
        #endregion
    }
}
