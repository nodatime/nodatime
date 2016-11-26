// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using System.Numerics;
using static NodaTime.NodaConstants;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.BclCompatibleTicks);
        }

        private static void TestFactoryMethod(Func<long, Duration> method, long value, long nanosecondsPerUnit)
        {
            Duration duration = method(value);
            BigInteger expectedNanoseconds = (BigInteger)value * nanosecondsPerUnit;
            Assert.AreEqual(duration.ToBigIntegerNanoseconds(), expectedNanoseconds);
        }

        private static void TestFactoryMethod(Func<int, Duration> method, int value, long nanosecondsPerUnit)
        {
            Duration duration = method(value);
            BigInteger expectedNanoseconds = (BigInteger) value * nanosecondsPerUnit;
            Assert.AreEqual(duration.ToBigIntegerNanoseconds(), expectedNanoseconds);
        }

        [Test]
        [TestCase(-100), TestCase(-1), TestCase(0), TestCase(1), TestCase(100)]
        public void FromDays_Int32(int days)
        {
            TestFactoryMethod(Duration.FromDays, days, NanosecondsPerDay);
        }

        [Test]
        [TestCase(-100), TestCase(-1), TestCase(0), TestCase(1), TestCase(100)]
        public void FromHours_Int32(int hours)
        {
            TestFactoryMethod(Duration.FromHours, hours, NanosecondsPerHour);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromMinutes_Int64(long minutes)
        {
            TestFactoryMethod(Duration.FromMinutes, minutes, NanosecondsPerMinute);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromSeconds_Int64(long seconds)
        {
            TestFactoryMethod(Duration.FromSeconds, seconds, NanosecondsPerSecond);
        }

        [Test]
        [TestCase(int.MinValue - 100L), TestCase(-100), TestCase(-1), TestCase(0)]
        [TestCase(1), TestCase(100), TestCase(int.MaxValue + 100L)]
        public void FromMilliseconds_Int64(long milliseconds)
        {
            TestFactoryMethod(Duration.FromMilliseconds, milliseconds, NanosecondsPerMillisecond);
        }

        [Test]
        [TestCase(1.5, 1, NanosecondsPerDay / 2)]
        [TestCase(-0.25, -1, 3 * NanosecondsPerDay / 4)]
        [TestCase(100000.5, 100000, NanosecondsPerDay / 2)]
        [TestCase(-5000, -5000, 0)]
        public void FromDays_Double(double days, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromDays(days);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(36.5, 1, NanosecondsPerDay / 2 + NanosecondsPerHour / 2)]
        [TestCase(-0.25, -1, NanosecondsPerDay - NanosecondsPerHour / 4)]
        [TestCase(24000.5, 1000, NanosecondsPerHour / 2)]
        public void FromHours_Double(double hours, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromHours(hours);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(MinutesPerDay + MinutesPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, NanosecondsPerSecond * 90)]
        [TestCase(-MinutesPerDay + 1.5, -1, NanosecondsPerSecond * 90)]
        public void FromMinutes_Double(double minutes, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromMinutes(minutes);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(SecondsPerDay + SecondsPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, NanosecondsPerMillisecond * 1500)]
        [TestCase(-SecondsPerDay + 1.5, -1, NanosecondsPerMillisecond * 1500)]
        public void FromSeconds_Double(double seconds, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromSeconds(seconds);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(MillisecondsPerDay + MillisecondsPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, 1500000)]
        [TestCase(-MillisecondsPerDay + 1.5, -1, 1500000)]
        public void FromMilliseconds_Double(double milliseconds, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromMilliseconds(milliseconds);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromAndToTimeSpan()
        {
            TimeSpan timeSpan = TimeSpan.FromHours(3) + TimeSpan.FromSeconds(2) + TimeSpan.FromTicks(1);
            Duration duration = Duration.FromHours(3) + Duration.FromSeconds(2) + Duration.FromTicks(1);
            Assert.AreEqual(duration, Duration.FromTimeSpan(timeSpan));
            Assert.AreEqual(timeSpan, duration.ToTimeSpan());

            Duration maxDuration = Duration.FromTicks(Int64.MaxValue);
            Assert.AreEqual(maxDuration, Duration.FromTimeSpan(TimeSpan.MaxValue));
            Assert.AreEqual(TimeSpan.MaxValue, maxDuration.ToTimeSpan());
            Duration minDuration = Duration.FromTicks(Int64.MinValue);

            Assert.AreEqual(minDuration, Duration.FromTimeSpan(TimeSpan.MinValue));
            Assert.AreEqual(TimeSpan.MinValue, minDuration.ToTimeSpan());
        }

        [Test]
        public void FromNanoseconds_Int64()
        {
            Assert.AreEqual(Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay - 1L));
            Assert.AreEqual(Duration.OneDay, Duration.FromNanoseconds(NanosecondsPerDay));
            Assert.AreEqual(Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay + 1L));

            Assert.AreEqual(-Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay - 1L));
            Assert.AreEqual(-Duration.OneDay, Duration.FromNanoseconds(-NanosecondsPerDay));
            Assert.AreEqual(-Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay + 1L));
        }

        [Test]
        public void FromNanoseconds_BigInteger()
        {
            Assert.AreEqual(Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay - BigInteger.One));
            Assert.AreEqual(Duration.OneDay, Duration.FromNanoseconds(NanosecondsPerDay + BigInteger.Zero));
            Assert.AreEqual(Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay + BigInteger.One));

            Assert.AreEqual(-Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay - BigInteger.One));
            Assert.AreEqual(-Duration.OneDay, Duration.FromNanoseconds(-NanosecondsPerDay + BigInteger.Zero));
            Assert.AreEqual(-Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay + BigInteger.One));
        }

        [Test]
        public void FromNanoseconds_Double()
        {
            Assert.AreEqual(Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay - 1d));
            Assert.AreEqual(Duration.OneDay, Duration.FromNanoseconds(NanosecondsPerDay + 0d));
            Assert.AreEqual(Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(NanosecondsPerDay + 1d));

            Assert.AreEqual(-Duration.OneDay - Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay - 1d));
            Assert.AreEqual(-Duration.OneDay, Duration.FromNanoseconds(-NanosecondsPerDay + 0d));
            Assert.AreEqual(-Duration.OneDay + Duration.Epsilon, Duration.FromNanoseconds(-NanosecondsPerDay + 1d));

            // Checks for values outside the range of long...
            // Find a value which is pretty big, but will definitely still convert back to a positive long.
            double largeDoubleValue = (long.MaxValue / 16) * 15;
            long largeInt64Value = (long) largeDoubleValue; // This won't be exactly long.MaxValue
            Assert.AreEqual(Duration.FromNanoseconds(largeInt64Value) * 8, Duration.FromNanoseconds(largeDoubleValue * 8d));
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
            Assert.AreEqual(ticks * (BigInteger) NodaConstants.NanosecondsPerTick, nanoseconds.ToBigIntegerNanoseconds());

            // Just another sanity check, although Ticks is covered in more detail later.
            Assert.AreEqual(ticks, nanoseconds.BclCompatibleTicks);
        }

        private static void AssertOutOfRange<T>(Func<T, Duration> factoryMethod, params T[] values)
        {
            foreach (var value in values)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => factoryMethod(value));
            }
        }

        [Test]
        public void FactoryMethods_OutOfRange()
        {
            // Not checking the exact values here so much as that the exception is appropriate.
            AssertOutOfRange(Duration.FromDays, int.MinValue, int.MaxValue);
            AssertOutOfRange(Duration.FromHours, int.MinValue, int.MaxValue);
            AssertOutOfRange(Duration.FromMinutes, long.MinValue, long.MaxValue);
            AssertOutOfRange(Duration.FromSeconds, long.MinValue, long.MaxValue);
            AssertOutOfRange(Duration.FromMilliseconds, long.MinValue, long.MaxValue);
            // FromTicks(long) never throws.

            double[] bigBadDoubles = { double.NegativeInfinity, double.MinValue, double.MaxValue, double.PositiveInfinity, double.NaN };
            AssertOutOfRange(Duration.FromDays, bigBadDoubles);
            AssertOutOfRange(Duration.FromHours, bigBadDoubles);
            AssertOutOfRange(Duration.FromMinutes, bigBadDoubles);
            AssertOutOfRange(Duration.FromSeconds, bigBadDoubles);
            AssertOutOfRange(Duration.FromMilliseconds,  bigBadDoubles);
            AssertOutOfRange(Duration.FromTicks, bigBadDoubles);
            AssertOutOfRange(Duration.FromNanoseconds, bigBadDoubles);
            
            // No such concept as BigInteger.Min/MaxValue, so use the values we know to be just outside valid bounds.
            AssertOutOfRange(Duration.FromNanoseconds, Duration.MinNanoseconds - 1, Duration.MaxNanoseconds + 1);
        }
    }
}
