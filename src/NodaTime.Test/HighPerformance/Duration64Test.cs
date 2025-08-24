// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.HighPerformance;
using NUnit.Framework;
using System.Globalization;

namespace NodaTime.Test.HighPerformance;

public partial class Duration64Test
{
    /// <summary>
    /// Using the default constructor is equivalent to Duration64.Zero.
    /// </summary>
    [Test]
    public void DefaultConstructor()
    {
        var actual = new Duration64();
        Assert.AreEqual(Duration64.Zero, actual);
    }

    // Tests copied from Nanoseconds in its brief existence... there may well be some overlap between
    // this and older Duration64 tests.

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
        var duration = Duration64.FromNanoseconds(int64Nanos);
        Assert.AreEqual(int64Nanos, duration.Nanoseconds);
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
        var left = FromDaysAndNanoseconds(leftDays, leftNanos);
        var right = FromDaysAndNanoseconds(rightDays, rightNanos);
        var result = FromDaysAndNanoseconds(resultDays, resultNanos);

        Assert.AreEqual(result, left + right);
        Assert.AreEqual(result, left.Plus(right));
        Assert.AreEqual(result, Duration64.Add(left, right));

        Assert.AreEqual(left, result - right);
        Assert.AreEqual(left, result.Minus(right));
        Assert.AreEqual(left, Duration64.Subtract(result, right));
    }

    [Test]
    public void Equality()
    {
        var equal1 = FromDaysAndNanoseconds(1, NodaConstants.NanosecondsPerHour);
        var equal2 = Duration64.FromTicks(NodaConstants.TicksPerHour * 25);
        var different1 = FromDaysAndNanoseconds(1, 200L);
        var different2 = FromDaysAndNanoseconds(2, NodaConstants.TicksPerHour);

        TestHelper.TestEqualsStruct(equal1, equal2, different1);
        TestHelper.TestOperatorEquality(equal1, equal2, different1);

        TestHelper.TestEqualsStruct(equal1, equal2, different2);
        TestHelper.TestOperatorEquality(equal1, equal2, different2);
    }

    [Test]
    public void Comparison()
    {
        var equal1 = FromDaysAndNanoseconds(1, NodaConstants.NanosecondsPerHour);
        var equal2 = Duration64.FromTicks(NodaConstants.TicksPerHour * 25);
        var greater1 = FromDaysAndNanoseconds(1, NodaConstants.NanosecondsPerHour + 1);
        var greater2 = FromDaysAndNanoseconds(2, 0L);

        TestHelper.TestCompareToStruct(equal1, equal2, greater1);
        TestHelper.TestNonGenericCompareTo(equal1, equal2, greater1);
        TestHelper.TestOperatorComparisonEquality(equal1, equal2, greater1, greater2);
    }

    [Test]
    [TestCase(0, 0L, 0, 0L)]
    [TestCase(1, 0L, -1, 0L)]
    [TestCase(0, 500L, -1, NodaConstants.NanosecondsPerDay - 500L)]
    // 100 years, instead of the 1000 year test in Duration
    [TestCase(36500, 500L, -36501, NodaConstants.NanosecondsPerDay - 500L)]
    public void UnaryNegation(int startDays, long startNanoOfDay, int expectedDays, long expectedNanoOfDay)
    {
        var start = FromDaysAndNanoseconds(startDays, startNanoOfDay);
        var expected = FromDaysAndNanoseconds(expectedDays, expectedNanoOfDay);
        Assert.AreEqual(expected, -start);
        // Test it the other way round as well...
        Assert.AreEqual(start, -expected);
    }

    [Test]
    public void MaxMinRelationship()
    {
        // Max and Min work like they do for other signed types - basically the max value is one less than the absolute
        // of the min value.
        Assert.AreEqual(Duration64.MinValue, -Duration64.MaxValue - Duration64.Epsilon);
    }

    [Test]
    public void Max()
    {
        Duration64 x = Duration64.FromNanoseconds(100);
        Duration64 y = Duration64.FromNanoseconds(200);
        Assert.AreEqual(y, Duration64.Max(x, y));
        Assert.AreEqual(y, Duration64.Max(y, x));
        Assert.AreEqual(x, Duration64.Max(x, Duration64.MinValue));
        Assert.AreEqual(x, Duration64.Max(Duration64.MinValue, x));
        Assert.AreEqual(Duration64.MaxValue, Duration64.Max(Duration64.MaxValue, x));
        Assert.AreEqual(Duration64.MaxValue, Duration64.Max(x, Duration64.MaxValue));
    }

    [Test]
    public void Min()
    {
        Duration64 x = Duration64.FromNanoseconds(100);
        Duration64 y = Duration64.FromNanoseconds(200);
        Assert.AreEqual(x, Duration64.Min(x, y));
        Assert.AreEqual(x, Duration64.Min(y, x));
        Assert.AreEqual(Duration64.MinValue, Duration64.Min(x, Duration64.MinValue));
        Assert.AreEqual(Duration64.MinValue, Duration64.Min(Duration64.MinValue, x));
        Assert.AreEqual(x, Duration64.Min(Duration64.MaxValue, x));
        Assert.AreEqual(x, Duration64.Min(x, Duration64.MaxValue));
    }

    [Test]
    public void AdditiveIdentityIsZero() => Assert.AreEqual(Duration64.AdditiveIdentity, Duration64.Zero);

    [Test]
    public void MinValue()
    {
        Assert.AreEqual("-106751:23:47:16.854775808", Duration64.MinValue.ToString());
        Assert.AreEqual("-106751:23:47:16.854775808", Duration64.MinValue.ToString("o", CultureInfo.InvariantCulture));

        Assert.AreEqual(Duration64.MinValue, Duration64.Zero
            - Duration64.FromDays(106751)
            - Duration64.FromHours(23)
            - Duration64.FromMinutes(47)
            - Duration64.FromSeconds(16)
            - Duration64.FromNanoseconds(854775808));

        var minValueAsDuration = Duration.Zero
            - Duration.FromDays(106751)
            - Duration.FromHours(23)
            - Duration.FromMinutes(47)
            - Duration.FromSeconds(16)
            - Duration.FromNanoseconds(854775808);
        Assert.AreEqual(Duration64.MinValue, Duration64.FromDuration(minValueAsDuration));
        Assert.AreEqual(minValueAsDuration, Duration64.MinValue.ToDuration());
    }

    [Test]
    public void MaxValue()
    {
        Assert.AreEqual("106751:23:47:16.854775807", Duration64.MaxValue.ToString());
        Assert.AreEqual("106751:23:47:16.854775807", Duration64.MaxValue.ToString("o", CultureInfo.InvariantCulture));

        Assert.AreEqual(Duration64.MaxValue, Duration64.Zero
            + Duration64.FromDays(106751)
            + Duration64.FromHours(23)
            + Duration64.FromMinutes(47)
            + Duration64.FromSeconds(16)
            + Duration64.FromNanoseconds(854775807));

        var maxValueAsDuration = Duration.Zero
            + Duration.FromDays(106751)
            + Duration.FromHours(23)
            + Duration.FromMinutes(47)
            + Duration.FromSeconds(16)
            + Duration.FromNanoseconds(854775807);
        Assert.AreEqual(Duration64.MaxValue, Duration64.FromDuration(maxValueAsDuration));
        Assert.AreEqual(maxValueAsDuration, Duration64.MaxValue.ToDuration());
    }

    [Test]
    public void TotalNanoseconds() => Assert.AreEqual(123L, Duration64.FromNanoseconds(123L).TotalNanoseconds);

    /// <summary>
    /// Factory method just to make porting tests easier.
    /// </summary>
    private static Duration64 FromDaysAndNanoseconds(long days, long nanoseconds) =>
        new(days * NodaConstants.NanosecondsPerDay + nanoseconds);
}
