// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class NanosecondsTest
    {
        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void Int64Conversions(long int64Nanos)
        {
            var nanoseconds = (Nanoseconds) int64Nanos;
            Assert.AreEqual(int64Nanos, (long) nanoseconds);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(-NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay)]
        [TestCase(NodaConstants.NanosecondsPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void DecimalConversions(long int64Nanos)
        {
            decimal decimalNanos = int64Nanos;
            var nanoseconds = (Nanoseconds) decimalNanos;
            Assert.AreEqual(decimalNanos, (decimal) nanoseconds);

            // And multiply it by 100, which proves we still work for values out of the range of Int64
            decimalNanos *= 100;
            nanoseconds = (Nanoseconds) decimalNanos;
            Assert.AreEqual(decimalNanos, (decimal) nanoseconds);
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(-NodaConstants.TicksPerStandardDay)]
        [TestCase(-NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(NodaConstants.TicksPerStandardDay)]
        [TestCase(NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void TickConversions(long ticks)
        {
            var nanoseconds = Nanoseconds.FromTicks(ticks);
            Assert.AreEqual(ticks, nanoseconds.Ticks);
        }

        [Test]
        public void ConstituentParts_Positive()
        {
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * 5 + 100);
            Assert.AreEqual(5, nanos.Days);
            Assert.AreEqual(100, nanos.NanosecondOfDay);
        }


        [Test]
        public void ConstituentParts_Negative()
        {
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * -5 + 100);
            Assert.AreEqual(-5, nanos.Days);
            Assert.AreEqual(100, nanos.NanosecondOfDay);
        }

        [Test]
        public void ConstituentParts_Large()
        {
            // And outside the normal range of long...
            var nanos = (Nanoseconds) (NodaConstants.NanosecondsPerStandardDay * 365000m + 500m);
            Assert.AreEqual(365000, nanos.Days);
            Assert.AreEqual(500, nanos.NanosecondOfDay);
        }

        [Test]
        [TestCase(1, 100L, 2, 200L, 3, 300L)]
        [TestCase(1, NodaConstants.NanosecondsPerStandardDay - 5,
                  3, 100L,
                  5, 95L, TestName = "Overflow")]
        [TestCase(1, 10L,
                  -1, NodaConstants.NanosecondsPerStandardDay - 100L,
                  0, NodaConstants.NanosecondsPerStandardDay - 90L,
                  TestName = "Underflow")]
        public void Addition_Subtraction(int leftDays, long leftNanos,
                                         int rightDays, long rightNanos,
                                         int resultDays, long resultNanos)
        {
            var left = new Nanoseconds(leftDays, leftNanos);
            var right = new Nanoseconds(rightDays, rightNanos);
            var result = new Nanoseconds(resultDays, resultNanos);

            Assert.AreEqual(result, left + right);
            Assert.AreEqual(result, left.Plus(right));
            Assert.AreEqual(result, Nanoseconds.Add(left, right));

            Assert.AreEqual(left, result - right);
            Assert.AreEqual(left, result.Minus(right));
            Assert.AreEqual(left, Nanoseconds.Subtract(result, right));
        }

        [Test]
        public void Equality()
        {
            var equal1 = new Nanoseconds(1, NodaConstants.NanosecondsPerHour);
            var equal2 = Nanoseconds.FromTicks(NodaConstants.TicksPerHour * 25);
            var different1 = new Nanoseconds(1, 200L);
            var different2 = new Nanoseconds(2, NodaConstants.TicksPerHour);

            TestHelper.TestEqualsStruct(equal1, equal2, different1);
            TestHelper.TestOperatorEquality(equal1, equal2, different1);

            TestHelper.TestEqualsStruct(equal1, equal2, different2);
            TestHelper.TestOperatorEquality(equal1, equal2, different2);
        }

        [Test]
        public void Comparison()
        {
            var equal1 = new Nanoseconds(1, NodaConstants.NanosecondsPerHour);
            var equal2 = Nanoseconds.FromTicks(NodaConstants.TicksPerHour * 25);
            var greater1 = new Nanoseconds(1, NodaConstants.NanosecondsPerHour + 1);
            var greater2 = new Nanoseconds(2, 0L);

            TestHelper.TestCompareToStruct(equal1, equal2, greater1);
            TestHelper.TestNonGenericCompareTo(equal1, equal2, greater1);
            TestHelper.TestOperatorComparisonEquality(equal1, equal2, greater1);

            TestHelper.TestCompareToStruct(equal1, equal2, greater2);
            TestHelper.TestNonGenericCompareTo(equal1, equal2, greater2);
            TestHelper.TestOperatorComparisonEquality(equal1, equal2, greater2);
        }
    }
}
