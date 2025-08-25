// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.HighPerformance;
using NUnit.Framework;
using System;
using System.Linq;
using static NodaTime.NodaConstants;

namespace NodaTime.Test.HighPerformance;

partial class Duration64Test
{
    private static readonly Duration64 OneDay = Duration64.FromNanoseconds(NanosecondsPerDay);

    // Test cases for factory methods. In general, we want to check the limits, very small, very large
    // and medium values, with a mixture of positive and negative, "on and off" day boundaries.
    private static readonly int[] DayCases = GenerateCases(NanosecondsPerDay).Select(c => (int) c).ToArray();

    private static readonly int[] HourCases = GenerateCases(NanosecondsPerHour).Select(c => (int) c).ToArray();

    private static readonly long[] MinuteCases = GenerateCases(NanosecondsPerMinute);

    private static readonly long[] SecondCases = GenerateCases(NanosecondsPerSecond);

    private static readonly long[] MillisecondCases = GenerateCases(NanosecondsPerMillisecond);

    // No boundary versions as Int64 doesn't have enough ticks to exceed our limits.
    private static readonly long[] TickCases = GenerateCases(TicksPerDay).Skip(1).Reverse().Skip(1).Reverse().ToArray();

    private static long[] GenerateCases(long nanosecondsPerUnit)
    {
        var unitsPerDay = NanosecondsPerDay / nanosecondsPerUnit;
        return unchecked(new[] {
            long.MinValue / nanosecondsPerUnit,
            -100L * 365 * unitsPerDay - 1,
            -100L * 365 * unitsPerDay,
            -5 * unitsPerDay / 2,
            -2 * unitsPerDay
            -1,
            0,
            1,
            2 * unitsPerDay,
            5 * unitsPerDay / 2,
            100L * 365 * unitsPerDay,
            100L * 365 * unitsPerDay + 1,
            long.MaxValue / nanosecondsPerUnit,
        });
    }

    [Test]
    public void Zero()
    {
        Duration64 test = Duration64.Zero;
        Assert.AreEqual(0, test.Nanoseconds);
    }

    private static void TestFactoryMethod(Func<long, Duration64> method, long value, long nanosecondsPerUnit)
    {
        Duration64 duration = method(value);
        long expectedNanoseconds = value * nanosecondsPerUnit;
        Assert.AreEqual(duration.Nanoseconds, expectedNanoseconds);
    }

    private static void TestFactoryMethod(Func<int, Duration64> method, int value, long nanosecondsPerUnit)
    {
        Duration64 duration = method(value);
        var expectedNanoseconds = value * nanosecondsPerUnit;
        Assert.AreEqual(duration.Nanoseconds, expectedNanoseconds);
    }

    [Test]
    [TestCaseSource(nameof(DayCases))]
    public void FromDays_Int32(int days)
    {
        TestFactoryMethod(Duration64.FromDays, days, NanosecondsPerDay);
    }

    [Test]
    [TestCaseSource(nameof(HourCases))]
    public void FromHours_Int32(int hours)
    {
        TestFactoryMethod(Duration64.FromHours, hours, NanosecondsPerHour);
    }

    [Test]
    [TestCaseSource(nameof(MinuteCases))]
    public void FromMinutes_Int64(long minutes)
    {
        TestFactoryMethod(Duration64.FromMinutes, minutes, NanosecondsPerMinute);
    }

    [Test]
    [TestCaseSource(nameof(SecondCases))]
    public void FromSeconds_Int64(long seconds)
    {
        TestFactoryMethod(Duration64.FromSeconds, seconds, NanosecondsPerSecond);
    }

    [Test]
    [TestCaseSource(nameof(MillisecondCases))]
    public void FromMilliseconds_Int64(long milliseconds)
    {
        TestFactoryMethod(Duration64.FromMilliseconds, milliseconds, NanosecondsPerMillisecond);
    }

    [Test]
    [TestCaseSource(nameof(TickCases))]
    public void FromTicks(long ticks)
    {
        var nanoseconds = Duration64.FromTicks(ticks);
        Assert.AreEqual(ticks * NodaConstants.NanosecondsPerTick, nanoseconds.Nanoseconds);
    }

    [Test]
    public void FromNanoseconds_Int64()
    {
        Assert.AreEqual(OneDay - Duration64.Epsilon, Duration64.FromNanoseconds(NanosecondsPerDay - 1L));
        Assert.AreEqual(OneDay, Duration64.FromNanoseconds(NanosecondsPerDay));
        Assert.AreEqual(OneDay + Duration64.Epsilon, Duration64.FromNanoseconds(NanosecondsPerDay + 1L));

        Assert.AreEqual(-OneDay - Duration64.Epsilon, Duration64.FromNanoseconds(-NanosecondsPerDay - 1L));
        Assert.AreEqual(-OneDay, Duration64.FromNanoseconds(-NanosecondsPerDay));
        Assert.AreEqual(-OneDay + Duration64.Epsilon, Duration64.FromNanoseconds(-NanosecondsPerDay + 1L));
    }

    [Test]
    public void FactoryMethods_OutOfRange()
    {
        // Each set of cases starts with the minimum and ends with the maximum, so we can test just beyond the limits easily.
        AssertLimitsInt32(Duration64.FromDays, DayCases);
        AssertLimitsInt32(Duration64.FromHours, HourCases);
        AssertLimitsInt64(Duration64.FromMinutes, MinuteCases);
        AssertLimitsInt64(Duration64.FromSeconds, SecondCases);
        AssertLimitsInt64(Duration64.FromMilliseconds, MillisecondCases);
        // FromTicks(long) never throws.

        void AssertLimitsInt32(Func<int, Duration64> factoryMethod, int[] allCases) =>
            AssertOverflow(factoryMethod, allCases.First() - 1, allCases.Last() + 1);

        void AssertLimitsInt64(Func<long, Duration64> factoryMethod, long[] allCases) =>
            AssertOverflow(factoryMethod, allCases.First() - 1, allCases.Last() + 1);

        void AssertOverflow<T>(Func<T, Duration64> factoryMethod, params T[] values)
        {
            foreach (var value in values)
            {
                Assert.Throws<OverflowException>(() => factoryMethod(value));
            }
        }
    }
}
