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
        public void FromTicks_Int64(long ticks)
        {
            var nanoseconds = Duration.FromTicks(ticks);
            Assert.AreEqual(ticks * (BigInteger)NodaConstants.NanosecondsPerTick, nanoseconds.ToBigIntegerNanoseconds());

            // Just another sanity check, although Ticks is covered in more detail later.
            Assert.AreEqual(ticks, nanoseconds.BclCompatibleTicks);
        }

        [Test]
        [TestCase(Duration.MinDays, Duration.MinDays, 0)]
        [TestCase(1.5, 1, NanosecondsPerDay / 2)]
        [TestCase(-0.25, -1, 3 * NanosecondsPerDay / 4)]
        [TestCase(100000.5, 100000, NanosecondsPerDay / 2)]
        [TestCase(-5000, -5000, 0)]
        [TestCase(-0.1, -1, 9 * NanosecondsPerDay / 10)]
        [TestCase(0.1, 0, NanosecondsPerDay / 10)]
        [TestCase(0.01, 0, NanosecondsPerDay / 100)]
        [TestCase(4.1, 4, NanosecondsPerDay / 10)]
        [TestCase(4.01, 4, NanosecondsPerDay / 100)]
        [TestCase(Duration.MaxDays + 0.5, Duration.MaxDays, NanosecondsPerDay / 2)]
        // The additional 19312 is due to the value being precisely 16777215.99000000022351741790771484375
        // This sort of difference is inevitable with very large numbers.
        [TestCase(Duration.MaxDays + 0.99, Duration.MaxDays, NanosecondsPerDay * 99 / 100 + 19312)]
        public void FromDays_Double(double days, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromDays(days);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(Duration.MinDays * (double) HoursPerDay, Duration.MinDays, 0)]
        [TestCase(36.5, 1, NanosecondsPerDay / 2 + NanosecondsPerHour / 2)]
        [TestCase(-0.25, -1, NanosecondsPerDay - NanosecondsPerHour / 4)]
        [TestCase(24000.5, 1000, NanosecondsPerHour / 2)]
        [TestCase(0.01, 0, NanosecondsPerHour / 100)]
        [TestCase(0.1, 0, NanosecondsPerHour / 10)]
        [TestCase(4.01, 0, 4 * NanosecondsPerHour + NanosecondsPerHour / 100)]
        [TestCase(4.1, 0, 4 * NanosecondsPerHour  + NanosecondsPerHour / 10)]
        [TestCase(4 * HoursPerDay + 0.1, 4, NanosecondsPerHour / 10)]
        [TestCase(4 * HoursPerDay + 0.01, 4, NanosecondsPerHour / 100)]
        // The additional 34332ns is due to the imprecision of dealing with very large numbers.
        [TestCase((Duration.MaxDays + 1L) * 24 - 0.01, Duration.MaxDays, NanosecondsPerDay - NanosecondsPerHour / 100 + 34332)]
        public void FromHours_Double(double hours, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromHours(hours);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(Duration.MinDays * (double) MinutesPerDay, Duration.MinDays, 0)]
        [TestCase(MinutesPerDay + MinutesPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, NanosecondsPerSecond * 90)]
        [TestCase(-MinutesPerDay + 1.5, -1, NanosecondsPerSecond * 90)]
        [TestCase(MinutesPerDay * 1000 + 1.5, 1000, NanosecondsPerMinute * 3 / 2)]
        [TestCase(0.01, 0, NanosecondsPerMinute / 100)]
        [TestCase(0.1, 0, NanosecondsPerMinute / 10)]
        [TestCase(4.01, 0, 4 * NanosecondsPerMinute + NanosecondsPerMinute / 100)]
        [TestCase(4.1, 0, 4 * NanosecondsPerMinute + NanosecondsPerMinute / 10)]
        [TestCase(4 * MinutesPerDay + 0.1, 4, NanosecondsPerMinute / 10)]
        [TestCase(4 * MinutesPerDay + 0.01, 4, NanosecondsPerMinute / 100)]
        // The additional 100708ns is due to the imprecision of dealing with very large numbers.
        [TestCase((Duration.MaxDays + 1L) * MinutesPerDay - 0.01, Duration.MaxDays, NanosecondsPerDay - NanosecondsPerMinute / 100 + 100708)]
        public void FromMinutes_Double(double minutes, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromMinutes(minutes);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(Duration.MinDays * (double) SecondsPerDay, Duration.MinDays, 0)]
        [TestCase(SecondsPerDay + SecondsPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, NanosecondsPerMillisecond * 1500)]
        [TestCase(-SecondsPerDay + 1.5, -1, NanosecondsPerMillisecond * 1500)]
        [TestCase(0.01, 0, 10_000_000)]
        [TestCase(0.1, 0, 100_000_000)]
        [TestCase(4.01, 0, 4_010_000_000)]
        [TestCase(4.1, 0, 4_100_000_000)]
        [TestCase(4 * SecondsPerDay + 0.1, 4, NanosecondsPerSecond / 10)]
        [TestCase(4 * SecondsPerDay + 0.01, 4, NanosecondsPerSecond / 100)]
        [TestCase(Duration.MaxDays * (double) SecondsPerDay + 0.5, Duration.MaxDays, NanosecondsPerSecond / 2)]
        public void FromSeconds_Double(double seconds, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromSeconds(seconds);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(Duration.MinDays * (double) MillisecondsPerDay, Duration.MinDays, 0)]
        [TestCase(MillisecondsPerDay + MillisecondsPerDay / 2, 1, NanosecondsPerDay / 2)]
        [TestCase(1.5, 0, 1500000)]
        [TestCase(-MillisecondsPerDay + 1.5, -1, 1500000)]
        [TestCase(MillisecondsPerDay * 8123L + MillisecondsPerSecond + 0.5, 8123, NanosecondsPerSecond + NanosecondsPerMillisecond / 2)]
        [TestCase(0.01, 0, 10_000)]
        [TestCase(0.1, 0, 100_000)]
        [TestCase(4.01, 0, 4_010_000)]
        [TestCase(4.1, 0, 4_100_000)]
        [TestCase(4 * MillisecondsPerDay + 0.01, 4, NanosecondsPerMillisecond / 100)]
        [TestCase(4 * MillisecondsPerDay + 0.1, 4, NanosecondsPerMillisecond / 10)]
        [TestCase(Duration.MaxDays * (double) MillisecondsPerDay + 0.5, Duration.MaxDays, NanosecondsPerMillisecond / 2)]
        public void FromMilliseconds_Double(double milliseconds, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromMilliseconds(milliseconds);
            var expected = new Duration(expectedDays, expectedNanoOfDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(Duration.MinDays * (double) TicksPerDay, Duration.MinDays, 0)]
        [TestCase(0.01, 0, 1)]
        [TestCase(0.1, 0, 10)]
        [TestCase(4.01, 0, 401)]
        [TestCase(4.1, 0, 410)]
        [TestCase(4 * TicksPerDay + 0.1, 4, NanosecondsPerTick / 10)]
        [TestCase(4 * TicksPerDay + 0.01, 4, NanosecondsPerTick / 100)]
        // Rounding down
        [TestCase(86_399_999_999_9.994d, 0, NanosecondsPerDay - 1)]
        // Rounding up across a day boundary
        [TestCase(86_399_999_999_9.995d, 1, 0)]
        // We need to add a lot of ticks to the start of "max day" just to get within significant digits.
        // The subtracted 51,200 is just due to the imprecision at these values.
        [TestCase(Duration.MaxDays * (double) TicksPerDay + 5_000_000_000, Duration.MaxDays, NanosecondsPerTick * 5_000_000_000 - 51_200)]
        public void FromTicks_Double(double ticks, int expectedDays, long expectedNanoOfDay)
        {
            var actual = Duration.FromTicks(ticks);
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

            AssertOutOfRange(Duration.FromDays, new double[] { Duration.MinDays - 0.01, Duration.MaxDays + 1 });
            AssertOutOfRange(Duration.FromHours, new double[] { Duration.MinDays * HoursPerDay - 0.01, (Duration.MaxDays + 1) * HoursPerDay });
            AssertOutOfRange(Duration.FromMinutes, new double[] { (double) Duration.MinDays * MinutesPerDay - 0.01, (double) (Duration.MaxDays + 1) * MinutesPerDay });
            AssertOutOfRange(Duration.FromSeconds, new double[] { (double) Duration.MinDays * SecondsPerDay - 0.01, (double) (Duration.MaxDays + 1) * SecondsPerDay });
            // We can't actually get to 100th of a millisecond precision with a 64-bit floating point number at this scale.
            AssertOutOfRange(Duration.FromMilliseconds, new double[] { (double) Duration.MinDays * MillisecondsPerDay - 1, (double) (Duration.MaxDays + 1) * MillisecondsPerDay });
            // We subtract a million ticks rather than just 1 to make sure it's actually significant.
            AssertOutOfRange(Duration.FromTicks, new double[] { (double) Duration.MinDays * TicksPerDay - 1_000_000, (double) (Duration.MaxDays + 1) * TicksPerDay });

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
