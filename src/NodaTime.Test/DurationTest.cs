// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class DurationTest
    {
        /// <summary>
        /// Using the default constructor is equivalent to Duration.Zero.
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Duration();
            Assert.AreEqual(Duration.Zero, actual);
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(Duration.FromTicks(12345L));
        }

        [Test]
        public void XmlSerialization()
        {
            Duration value = new PeriodBuilder { Days = 5, Hours = 3, Minutes = 20, Seconds = 35, Ticks = 1234500 }.Build().ToDuration();
            TestHelper.AssertXmlRoundtrip(value, "<value>5:03:20:35.12345</value>");
            TestHelper.AssertXmlRoundtrip(-value, "<value>-5:03:20:35.12345</value>");
        }

        [Test]
        [TestCase("<value>XYZ</value>", typeof(UnparsableValueException), Description = "Completely unparsable")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Duration>(xml, expectedExceptionType);
        }

        // Tests copied from Nanoseconds in its brief existence... there may well be some overlap between
        // this and older Duration tests.

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerDay)]
        [TestCase(-NodaConstants.NanosecondsPerDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerDay)]
        [TestCase(NodaConstants.NanosecondsPerDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void Int64Conversions(long int64Nanos)
        {
            var nanoseconds = Duration.FromNanoseconds(int64Nanos);
            Assert.AreEqual(int64Nanos, nanoseconds.ToInt64Nanoseconds());
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.NanosecondsPerDay - 1)]
        [TestCase(-NodaConstants.NanosecondsPerDay)]
        [TestCase(-NodaConstants.NanosecondsPerDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.NanosecondsPerDay - 1)]
        [TestCase(NodaConstants.NanosecondsPerDay)]
        [TestCase(NodaConstants.NanosecondsPerDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void DecimalConversions(long int64Nanos)
        {
            decimal decimalNanos = int64Nanos;
            var nanoseconds = Duration.FromNanoseconds(decimalNanos);
            Assert.AreEqual(decimalNanos, nanoseconds.ToDecimalNanoseconds());

            // And multiply it by 100, which proves we still work for values out of the range of Int64
            decimalNanos *= 100;
            nanoseconds = Duration.FromNanoseconds(decimalNanos);
            Assert.AreEqual(decimalNanos, nanoseconds.ToDecimalNanoseconds());
        }

        [Test]
        [TestCase(long.MinValue)]
        [TestCase(long.MinValue + 1)]
        [TestCase(-NodaConstants.TicksPerDay - 1)]
        [TestCase(-NodaConstants.TicksPerDay)]
        [TestCase(-NodaConstants.TicksPerDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(NodaConstants.TicksPerDay - 1)]
        [TestCase(NodaConstants.TicksPerDay)]
        [TestCase(NodaConstants.TicksPerDay + 1)]
        [TestCase(long.MaxValue - 1)]
        [TestCase(long.MaxValue)]
        public void FromTicks(long ticks)
        {
            var nanoseconds = Duration.FromTicks(ticks);
            Assert.AreEqual(ticks * (decimal) NodaConstants.NanosecondsPerTick, nanoseconds.ToDecimalNanoseconds());

            // Just another sanity check, although Ticks is covered in more detail later.
            Assert.AreEqual(ticks, nanoseconds.Ticks);
        }

        [Test]
        public void ConstituentParts_Positive()
        {
            var nanos = Duration.FromNanoseconds(NodaConstants.NanosecondsPerDay * 5 + 100);
            Assert.AreEqual(5, nanos.FloorDays);
            Assert.AreEqual(100, nanos.NanosecondOfFloorDay);
        }

        [Test]
        public void ConstituentParts_Negative()
        {
            var nanos = Duration.FromNanoseconds(NodaConstants.NanosecondsPerDay * -5 + 100);
            Assert.AreEqual(-5, nanos.FloorDays);
            Assert.AreEqual(100, nanos.NanosecondOfFloorDay);
        }

        [Test]
        public void ConstituentParts_Large()
        {
            // And outside the normal range of long...
            var nanos = Duration.FromNanoseconds(NodaConstants.NanosecondsPerDay * 365000m + 500m);
            Assert.AreEqual(365000, nanos.FloorDays);
            Assert.AreEqual(500, nanos.NanosecondOfFloorDay);
        }

        [Test]
        [TestCase(1, 100L, 2, 200L, 3, 300L)]
        [TestCase(1, NodaConstants.NanosecondsPerDay - 5,
                  3, 100L,
                  5, 95L, TestName = "Overflow")]
        [TestCase(1, 10L,
                  -1, NodaConstants.NanosecondsPerDay - 100L,
                  0, NodaConstants.NanosecondsPerDay - 90L,
                  TestName = "Underflow")]
        public void Addition_Subtraction(int leftDays, long leftNanos,
                                         int rightDays, long rightNanos,
                                         int resultDays, long resultNanos)
        {
            var left = new Duration(leftDays, leftNanos);
            var right = new Duration(rightDays, rightNanos);
            var result = new Duration(resultDays, resultNanos);

            Assert.AreEqual(result, left + right);
            Assert.AreEqual(result, left.Plus(right));
            Assert.AreEqual(result, Duration.Add(left, right));

            Assert.AreEqual(left, result - right);
            Assert.AreEqual(left, result.Minus(right));
            Assert.AreEqual(left, Duration.Subtract(result, right));
        }

        [Test]
        public void Equality()
        {
            var equal1 = new Duration(1, NodaConstants.NanosecondsPerHour);
            var equal2 = Duration.FromTicks(NodaConstants.TicksPerHour * 25);
            var different1 = new Duration(1, 200L);
            var different2 = new Duration(2, NodaConstants.TicksPerHour);

            TestHelper.TestEqualsStruct(equal1, equal2, different1);
            TestHelper.TestOperatorEquality(equal1, equal2, different1);

            TestHelper.TestEqualsStruct(equal1, equal2, different2);
            TestHelper.TestOperatorEquality(equal1, equal2, different2);
        }

        [Test]
        public void Comparison()
        {
            var equal1 = new Duration(1, NodaConstants.NanosecondsPerHour);
            var equal2 = Duration.FromTicks(NodaConstants.TicksPerHour * 25);
            var greater1 = new Duration(1, NodaConstants.NanosecondsPerHour + 1);
            var greater2 = new Duration(2, 0L);

            TestHelper.TestCompareToStruct(equal1, equal2, greater1);
            TestHelper.TestNonGenericCompareTo(equal1, equal2, greater1);
            TestHelper.TestOperatorComparisonEquality(equal1, equal2, greater1);

            TestHelper.TestCompareToStruct(equal1, equal2, greater2);
            TestHelper.TestNonGenericCompareTo(equal1, equal2, greater2);
            TestHelper.TestOperatorComparisonEquality(equal1, equal2, greater2);
        }

        [Test]
        [TestCase(1, 5L, 2, 2, 10L, TestName = "Small, positive")]
        [TestCase(-1, NodaConstants.NanosecondsPerDay - 10, 2, -1, NodaConstants.NanosecondsPerDay - 20, TestName = "Small, negative")]
        [TestCase(365000, 1L, 2, 365000 * 2, 2L, TestName = "More than 2^63 nanos before multiplication")]
        [TestCase(1000, 1L, 365, 365000, 365L, TestName = "More than 2^63 nanos after multiplication")]
        [TestCase(1000, 1L, -365, -365001, NodaConstants.NanosecondsPerDay - 365L, TestName = "Less than -2^63 nanos after multiplication")]
        [TestCase(0, 1L, NodaConstants.NanosecondsPerDay, 1, 0L, TestName = "Large scalar")]
        public void Multiplication(int startDays, long startNanoOfDay, long scalar, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Duration(startDays, startNanoOfDay);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, start * scalar);
        }

        [Test]
        [TestCase(0, 0L, 0, 0L)]
        [TestCase(1, 0L, -1, 0L)]
        [TestCase(0, 500L, -1, NodaConstants.NanosecondsPerDay - 500L)]
        [TestCase(365000, 500L, -365001, NodaConstants.NanosecondsPerDay - 500L)]
        public void UnaryNegation(int startDays, long startNanoOfDay, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Duration(startDays, startNanoOfDay);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, -start);
            // Test it the other way round as well...
            Assert.AreEqual(start, -expected);
        }

        [Test]
        // Test cases around 0
        [TestCase(-1, NodaConstants.NanosecondsPerDay - 1, NodaConstants.NanosecondsPerDay, 0, 0L)]
        [TestCase(0, 0L, NodaConstants.NanosecondsPerDay, 0, 0L)]
        [TestCase(0, 1L, NodaConstants.NanosecondsPerDay, 0, 0L)]

        // Test cases around dividing -1 day by "nanos per day"
        [TestCase(-2, NodaConstants.NanosecondsPerDay - 1, NodaConstants.NanosecondsPerDay, -1, NodaConstants.NanosecondsPerDay - 1)] // -1ns
        [TestCase(-1, 0, NodaConstants.NanosecondsPerDay, -1, NodaConstants.NanosecondsPerDay - 1)] // -1ns
        [TestCase(-1, 1L, NodaConstants.NanosecondsPerDay, 0, 0L)]

        // Test cases around dividing 1 day by "nanos per day"
        [TestCase(0, NodaConstants.NanosecondsPerDay - 1, NodaConstants.NanosecondsPerDay, 0, 0L)]
        [TestCase(1, 0, NodaConstants.NanosecondsPerDay, 0, 1L)]
        [TestCase(1, NodaConstants.NanosecondsPerDay - 1, NodaConstants.NanosecondsPerDay, 0, 1L)]

        [TestCase(10, 20L, 5, 2, 4L)]

        // Large value, which will use decimal arithmetic
        [TestCase(365000, 3000L, 1000, 365, 3L)]
        public void Division(int startDays, long startNanoOfDay, long divisor, int expectedDays, long expectedNanoOfDay)
        {
            var start = new Duration(startDays, startNanoOfDay);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, start / divisor);
        }

        [Test]
        public void Ticks_Zero()
        {
            Assert.AreEqual(0, Duration.FromTicks(0).Ticks);
            Assert.AreEqual(0, Duration.FromNanoseconds(99L).Ticks);
            Assert.AreEqual(0, Duration.FromNanoseconds(-99L).Ticks);
        }

        [Test]
        [TestCase(5L)]
        [TestCase(NodaConstants.TicksPerDay * 2)]
        [TestCase(NodaConstants.TicksPerDay * 365000)]
        public void Ticks_Positive(long ticks)
        {
            Assert.IsTrue(ticks > 0);
            Duration start = Duration.FromTicks(ticks);
            Assert.AreEqual(ticks, start.Ticks);

            // We truncate towards zero... so subtracting 1 nanosecond should
            // reduce the number of ticks, and adding 99 nanoseconds should not change it
            Assert.AreEqual(ticks - 1, start.MinusSmallNanoseconds(1L).Ticks);
            Assert.AreEqual(ticks, start.PlusSmallNanoseconds(99L).Ticks);
        }

        [Test]
        [TestCase(-5L)]
        [TestCase(-NodaConstants.TicksPerDay * 2)]
        [TestCase(-NodaConstants.TicksPerDay * 365000)]
        public void Ticks_Negative(long ticks)
        {
            Assert.IsTrue(ticks < 0);
            Duration start = Duration.FromTicks(ticks);
            Assert.AreEqual(ticks, start.Ticks);

            // We truncate towards zero... so subtracting 99 nanoseconds should
            // have no effect, and adding 1 should increase the number of ticks
            Assert.AreEqual(ticks, start.MinusSmallNanoseconds(99L).Ticks);
            Assert.AreEqual(ticks + 1, start.PlusSmallNanoseconds(1L).Ticks);
        }

        [Test]
        public void Validation()
        {
            TestHelper.AssertValid(Duration.FromDays, (1 << 24) - 1);
            TestHelper.AssertOutOfRange(Duration.FromDays, 1 << 24);
            TestHelper.AssertValid(Duration.FromDays, -(1 << 24));
            TestHelper.AssertOutOfRange(Duration.FromDays, -(1 << 24) - 1);
        }

        [Test]
        public void TicksWithOverflow()
        {
            Duration maxTicks = Duration.FromTicks(long.MaxValue) + Duration.FromTicks(1);
            Assert.Throws<OverflowException>(() => maxTicks.Ticks.ToString());
        }

        [Test]
        public void PositiveComponents()
        {
            // Worked out with a calculator :)
            Duration duration = Duration.FromNanoseconds(1234567890123456L);
            Assert.AreEqual(14, duration.Days);
            Assert.AreEqual(24967890123456L, duration.NanosecondOfDay);
            Assert.AreEqual(6, duration.Hours);
            Assert.AreEqual(56, duration.Minutes);
            Assert.AreEqual(7, duration.Seconds);
            Assert.AreEqual(890, duration.Milliseconds);
            Assert.AreEqual(8901234, duration.SubsecondTicks);
            Assert.AreEqual(890123456, duration.SubsecondNanoseconds);
        }

        [Test]
        public void NegativeComponents()
        {
            // Worked out with a calculator :)
            Duration duration = Duration.FromNanoseconds(-1234567890123456L);
            Assert.AreEqual(-14, duration.Days);
            Assert.AreEqual(-24967890123456L, duration.NanosecondOfDay);
            Assert.AreEqual(-6, duration.Hours);
            Assert.AreEqual(-56, duration.Minutes);
            Assert.AreEqual(-7, duration.Seconds);
            Assert.AreEqual(-890, duration.Milliseconds);
            Assert.AreEqual(-8901234, duration.SubsecondTicks);
            Assert.AreEqual(-890123456, duration.SubsecondNanoseconds);
        }
    }
}
