// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalInstantTest
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
        public void MinusOffset_Zero_IsNeutralElement()
        {
            Instant sampleInstant = new Instant(1, 23456L);
            LocalInstant sampleLocalInstant = new LocalInstant(1, 23456L);
            Assert.AreEqual(sampleInstant, sampleLocalInstant.Minus(Offset.Zero));
            Assert.AreEqual(sampleInstant, sampleLocalInstant.MinusZeroOffset());
        }

        [Test]
        [TestCase(0, 0, "1970-01-01T00:00:00 LOC")]
        [TestCase(0, 1, "1970-01-01T00:00:00.000000001 LOC")]
        [TestCase(0, 1000, "1970-01-01T00:00:00.000001 LOC")]
        [TestCase(0, 1000000, "1970-01-01T00:00:00.001 LOC")]
        [TestCase(-1, NodaConstants.NanosecondsPerDay - 1, "1969-12-31T23:59:59.999999999 LOC")]
        public void ToString_Valid(int day, long nanoOfDay, string expectedText)
        {
            Assert.AreEqual(expectedText, new LocalInstant(day, nanoOfDay).ToString());
        }

        [Test]
        public void ToString_Extremes()
        {
            Assert.AreEqual(InstantPatternParser.BeforeMinValueText, LocalInstant.BeforeMinValue.ToString());
            Assert.AreEqual(InstantPatternParser.AfterMaxValueText, LocalInstant.AfterMaxValue.ToString());
        }

        [Test]
        public void SafeMinus_NormalTime()
        {
            var start = new LocalInstant(0, 0);
            var end = start.SafeMinus(Offset.FromHours(1));
            Assert.AreEqual(Duration.FromHours(-1), end.TimeSinceEpoch);
        }

        // A null offset indicates "BeforeMinValue". Otherwise, MinValue.Plus(offset)
        [Test]
        [TestCase(null, 0, null)]
        [TestCase(null, 1, null)]
        [TestCase(null, -1, null)]
        [TestCase(1, 1, 0)]
        [TestCase(1, 2, null)]
        [TestCase(2, 1, 1)]
        public void SafeMinus_NearStartOfTime(int? initialOffset, int offsetToSubtract, int? finalOffset)
        {
            var start = initialOffset == null
                ? LocalInstant.BeforeMinValue
                : Instant.MinValue.Plus(Offset.FromHours(initialOffset.Value));
            var expected = finalOffset == null
                ? Instant.BeforeMinValue
                : Instant.MinValue + Duration.FromHours(finalOffset.Value);
            var actual = start.SafeMinus(Offset.FromHours(offsetToSubtract));
            Assert.AreEqual(expected, actual);
        }

        // A null offset indicates "AfterMaxValue". Otherwise, MaxValue.Plus(offset)
        [Test]
        [TestCase(null, 0, null)]
        [TestCase(null, 1, null)]
        [TestCase(null, -1, null)]
        [TestCase(-1, -1, 0)]
        [TestCase(-1, -2, null)]
        [TestCase(-2, -1, -1)]
        public void SafeMinus_NearEndOfTime(int? initialOffset, int offsetToSubtract, int? finalOffset)
        {
            var start = initialOffset == null
                ? LocalInstant.AfterMaxValue
                : Instant.MaxValue.Plus(Offset.FromHours(initialOffset.Value));
            var expected = finalOffset == null
                ? Instant.AfterMaxValue
                : Instant.MaxValue + Duration.FromHours(finalOffset.Value);
            var actual = start.SafeMinus(Offset.FromHours(offsetToSubtract));
            Assert.AreEqual(expected, actual);
        }
    }
}
