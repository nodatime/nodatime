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
    }
}
