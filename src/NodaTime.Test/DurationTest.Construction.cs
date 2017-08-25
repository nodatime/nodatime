// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using System.Numerics;
using static NodaTime.NodaConstants;
using System.Linq;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        // Test cases for factory methods. In general, we want to check the limits, very small, very large
        // and medium values, with a mixture of positive and negative, "on and off" day boundaries.

        private static readonly int[] DayCases = {
            Duration.MinDays, -3000 * 365, -100, -1, 0, 1, 100, 3000 * 365, Duration.MaxDays
        };

        private static readonly int[] HourCases = {
            Duration.MinDays * HoursPerDay, -3000 * 365 * HoursPerDay, -100, -48, -1, 0,
            1, 48, 100, 3000 * 365 * HoursPerDay, ((Duration.MaxDays + 1) * HoursPerDay) - 1
        };

        private static readonly long[] MinuteCases = GenerateCases(MinutesPerDay);

        private static readonly long[] SecondCases = GenerateCases(SecondsPerDay);

        private static readonly long[] MillisecondCases = GenerateCases(MillisecondsPerDay);

        // No boundary versions as Int64 doesn't have enough ticks to exceed our limits.
        private static readonly long[] TickCases = GenerateCases(TicksPerDay).Skip(1).Reverse().Skip(1).Reverse().ToArray();

        private static long[] GenerateCases(long unitsPerDay) =>
            unchecked (new[] {
                Duration.MinDays * unitsPerDay,
                -3000L * 365 * unitsPerDay - 1,
                -3000L * 365 * unitsPerDay,
                -5 * unitsPerDay / 2,
                -2 * unitsPerDay
                -1,
                0,
                1,
                2 * unitsPerDay,
                5 * unitsPerDay / 2,
                3000L * 365 * unitsPerDay,
                3000L * 365 * unitsPerDay + 1,
                ((Duration.MaxDays + 1) * unitsPerDay) - 1,
            });

        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.BclCompatibleTicks);
            Assert.AreEqual(0, test.NanosecondOfFloorDay);
            Assert.AreEqual(0, test.FloorDays);
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
        [TestCaseSource(nameof(DayCases))]
        public void FromDays_Int32(int days)
        {
            TestFactoryMethod(Duration.FromDays, days, NanosecondsPerDay);
        }

        [Test]
        [TestCaseSource(nameof(HourCases))]
        public void FromHours_Int32(int hours)
        {
            TestFactoryMethod(Duration.FromHours, hours, NanosecondsPerHour);
        }

        [Test]
        [TestCaseSource(nameof(MinuteCases))]
        public void FromMinutes_Int64(long minutes)
        {
            TestFactoryMethod(Duration.FromMinutes, minutes, NanosecondsPerMinute);
        }

        [Test]
        [TestCaseSource(nameof(SecondCases))]
        public void FromSeconds_Int64(long seconds)
        {
            TestFactoryMethod(Duration.FromSeconds, seconds, NanosecondsPerSecond);
        }

        [Test]
        [TestCaseSource(nameof(MillisecondCases))]
        public void FromMilliseconds_Int64(long milliseconds)
        {
            TestFactoryMethod(Duration.FromMilliseconds, milliseconds, NanosecondsPerMillisecond);
        }

        [Test]
        [TestCaseSource(nameof(TickCases))]
        public void FromTicks(long ticks)
        {
            var nanoseconds = Duration.FromTicks(ticks);
            Assert.AreEqual(ticks * (BigInteger)NodaConstants.NanosecondsPerTick, nanoseconds.ToBigIntegerNanoseconds());

            // Just another sanity check, although Ticks is covered in more detail later.
            Assert.AreEqual(ticks, nanoseconds.BclCompatibleTicks);
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
        public void FromNanosecondsDecimal_Limits()
        {
            Assert.AreEqual(Duration.MinValue, Duration.FromNanoseconds(Duration.MinDecimalNanoseconds));
            Assert.AreEqual(Duration.MaxValue, Duration.FromNanoseconds(Duration.MaxDecimalNanoseconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => Duration.FromNanoseconds(Duration.MinDecimalNanoseconds - 1m));
            Assert.Throws<ArgumentOutOfRangeException>(() => Duration.FromNanoseconds(Duration.MaxDecimalNanoseconds + 1m));
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

#if !NETCORE

        [Test]
        [TestCase(typeof(ArgumentException), Duration.MinDays - 1, 0)]
        [TestCase(typeof(ArgumentException), Duration.MaxDays + 1, 0)]
        [TestCase(typeof(ArgumentException), 0, -1)]
        [TestCase(typeof(ArgumentException), 0, NodaConstants.NanosecondsPerDay)]
        public void InvalidBinaryData(Type expectedExceptionType, int days, long nanoOfDay) =>
            TestHelper.AssertBinaryDeserializationFailure<Duration>(expectedExceptionType, info =>
            {
                info.AddValue(BinaryFormattingConstants.DurationDefaultDaysSerializationName, days);
                info.AddValue(BinaryFormattingConstants.DurationDefaultNanosecondOfDaySerializationName, nanoOfDay);
            });
#endif

        [Test]
        public void FactoryMethods_OutOfRange()
        {
            // Each set of cases starts with the minimum and ends with the maximum, so we can test just beyond the limits easily.
            AssertLimitsInt32(Duration.FromDays, DayCases);
            AssertLimitsInt32(Duration.FromHours, HourCases);
            AssertLimitsInt64(Duration.FromMinutes, MinuteCases);
            AssertLimitsInt64(Duration.FromSeconds, SecondCases);
            AssertLimitsInt64(Duration.FromMilliseconds, MillisecondCases);
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

            void AssertLimitsInt32(Func<int, Duration> factoryMethod, int[] allCases) =>
                AssertOutOfRange(factoryMethod, allCases.First() - 1, allCases.Last() + 1);

            void AssertLimitsInt64(Func<long, Duration> factoryMethod, long[] allCases) =>
                AssertOutOfRange(factoryMethod, allCases.First() - 1, allCases.Last() + 1);

            void AssertOutOfRange<T>(Func<T, Duration> factoryMethod, params T[] values)
            {
                foreach (var value in values)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => factoryMethod(value));
                }
            }
        }
    }
}
