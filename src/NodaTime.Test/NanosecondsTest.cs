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
        public void FromTicks(long ticks)
        {
            var nanoseconds = Nanoseconds.FromTicks(ticks);
            Assert.AreEqual(ticks * (decimal) NodaConstants.NanosecondsPerTick, (decimal) nanoseconds);

            // Just another sanity check, although Ticks is covered in more detail later.
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

        [Test]
        [TestCase(1, 5L, 2, 2, 10L, TestName = "Small, positive")]
        [TestCase(-1, NodaConstants.NanosecondsPerStandardDay - 10, 2, -1, NodaConstants.NanosecondsPerStandardDay - 20, TestName = "Small, negative")]
        [TestCase(365000, 1L, 2, 365000 * 2, 2L, TestName = "More than 2^63 nanos before multiplication")]
        [TestCase(1000, 1L, 365, 365000, 365L, TestName = "More than 2^63 nanos after multiplication")]
        [TestCase(1000, 1L, -365, -365001, NodaConstants.NanosecondsPerStandardDay - 365L, TestName = "Less than -2^63 nanos after multiplication")]
        [TestCase(0, 1L, NodaConstants.NanosecondsPerStandardDay, 1, 0L, TestName = "Large scalar")]
        public void Multiplication(int startDays, long startNanoOfDay, long scalar, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Nanoseconds(startDays, startNanoOfDay);
            var expected = new Nanoseconds(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, start * scalar);
        }
        
        [Test]
        [TestCase(0, 0L, 0, 0L)]
        [TestCase(1, 0L, -1, 0L)]
        [TestCase(0, 500L, -1, NodaConstants.NanosecondsPerStandardDay - 500L)]
        [TestCase(365000, 500L, -365001, NodaConstants.NanosecondsPerStandardDay - 500L)]
        public void UnaryNegation(int startDays, long startNanoOfDay, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Nanoseconds(startDays, startNanoOfDay);
            var expected = new Nanoseconds(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, -start);
            // Test it the other way round as well...
            Assert.AreEqual(start, -expected);
        }

        [Test]
        // Test cases around 0
        [TestCase(-1, NodaConstants.NanosecondsPerStandardDay - 1, NodaConstants.NanosecondsPerStandardDay, 0, 0L)]
        [TestCase(0, 0L, NodaConstants.NanosecondsPerStandardDay, 0, 0L)]
        [TestCase(0, 1L, NodaConstants.NanosecondsPerStandardDay, 0, 0L)]

        // Test cases around dividing -1 day by "nanos per day"
        [TestCase(-2, NodaConstants.NanosecondsPerStandardDay - 1, NodaConstants.NanosecondsPerStandardDay, -1, NodaConstants.NanosecondsPerStandardDay - 1)] // -1ns
        [TestCase(-1, 0, NodaConstants.NanosecondsPerStandardDay, -1, NodaConstants.NanosecondsPerStandardDay - 1)] // -1ns
        [TestCase(-1, 1L, NodaConstants.NanosecondsPerStandardDay, 0, 0L)]

        // Test cases around dividing 1 day by "nanos per day"
        [TestCase(0, NodaConstants.NanosecondsPerStandardDay - 1, NodaConstants.NanosecondsPerStandardDay, 0, 0L)]
        [TestCase(1, 0, NodaConstants.NanosecondsPerStandardDay, 0, 1L)]
        [TestCase(1, NodaConstants.NanosecondsPerStandardDay - 1, NodaConstants.NanosecondsPerStandardDay, 0, 1L)]

        [TestCase(10, 20L, 5, 2, 4L)]

        // Large value, which will use decimal arithmetic
        [TestCase(365000, 3000L, 1000, 365, 3L)]
        public void Division(int startDays, long startNanoOfDay, long divisor, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Nanoseconds(startDays, startNanoOfDay);
            var expected = new Nanoseconds(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, start / divisor);
        }

        [Test]
        public void Ticks_Zero()
        {
            Assert.AreEqual(0, Nanoseconds.FromTicks(0).Ticks);
            Assert.AreEqual(0, ((Nanoseconds) 99L).Ticks);
            Assert.AreEqual(0, ((Nanoseconds) (-99L)).Ticks);
        }

        [Test]
        [TestCase(5L)]
        [TestCase(NodaConstants.TicksPerStandardDay * 2)]
        [TestCase(NodaConstants.TicksPerStandardDay * 365000)]
        public void Ticks_Positive(long ticks)
        {
            Assert.IsTrue(ticks > 0);
            Nanoseconds start = Nanoseconds.FromTicks(ticks);
            Assert.AreEqual(ticks, start.Ticks);

            // We truncate towards zero... so subtracting 1 nanosecond should
            // reduce the number of ticks, and adding 99 nanoseconds should not change it
            Assert.AreEqual(ticks - 1, start.Minus(1L).Ticks);
            Assert.AreEqual(ticks, start.Plus(99L).Ticks);
        }

        [Test]
        [TestCase(-5L)]
        [TestCase(-NodaConstants.TicksPerStandardDay * 2)]
        [TestCase(-NodaConstants.TicksPerStandardDay * 365000)]
        public void Ticks_Negative(long ticks)
        {
            Assert.IsTrue(ticks < 0);
            Nanoseconds start = Nanoseconds.FromTicks(ticks);
            Assert.AreEqual(ticks, start.Ticks);

            // We truncate towards zero... to subtracting 99 nanoseconds should
            // have no effect, and adding 1 should increase the number of ticks
            Assert.AreEqual(ticks, start.Minus(99L).Ticks);
            Assert.AreEqual(ticks + 1, start.Plus(1L).Ticks);
        }
    }
}
