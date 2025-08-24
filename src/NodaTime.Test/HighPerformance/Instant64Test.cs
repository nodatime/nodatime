// Copyright 2025 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.HighPerformance;
using NUnit.Framework;
using System;
using System.Globalization;

namespace NodaTime.Test.HighPerformance;

public partial class Instant64Test
{
    private static readonly Instant64 one = Instant64.FromUnixTimeNanoseconds(1L);
    private static readonly Instant64 threeMillion = Instant64.FromUnixTimeNanoseconds(3000000L);
    private static readonly Instant64 negativeFiftyMillion = Instant64.FromUnixTimeNanoseconds(-50000000L);

    [Test]
    public void FromUtcNoSeconds()
    {
        Instant64 viaUtc = Instant64.FromInstant(DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 0)).ToInstant());
        Assert.AreEqual(viaUtc, Instant64.FromUtc(2008, 4, 3, 10, 35));
    }

    [Test]
    public void FromUtcWithSeconds()
    {
        Instant64 viaUtc = Instant64.FromInstant(DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 23)).ToInstant());
        Assert.AreEqual(viaUtc, Instant64.FromUtc(2008, 4, 3, 10, 35, 23));
    }

    [Test]
    public void FromTicksSinceUnixEpoch()
    {
        Instant64 instant = Instant64.FromUnixTimeTicks(12345L);
        Assert.AreEqual(12345L, instant.ToUnixTimeTicks());
    }

    [Test]
    public void FromUnixTimeMilliseconds_Valid()
    {
        Instant64 actual = Instant64.FromUnixTimeMilliseconds(12345L);
        Instant64 expected = Instant64.FromUnixTimeTicks(12345L * NodaConstants.TicksPerMillisecond);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FromUnixTimeMilliseconds_TooLarge()
    {
        Assert.Throws<OverflowException>(() => Instant64.FromUnixTimeMilliseconds(long.MaxValue / 100));
    }

    [Test]
    public void FromUnixTimeMilliseconds_TooSmall()
    {
        Assert.Throws<OverflowException>(() => Instant64.FromUnixTimeMilliseconds(long.MinValue / 100));
    }

    [Test]
    public void FromUnixTimeSeconds_Valid()
    {
        Instant64 actual = Instant64.FromUnixTimeSeconds(12345L);
        Instant64 expected = Instant64.FromUnixTimeTicks(12345L * NodaConstants.TicksPerSecond);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void FromUnixTimeSeconds_TooLarge()
    {
        Assert.Throws<OverflowException>(() => Instant64.FromUnixTimeSeconds(long.MaxValue / 1000000));
    }

    [Test]
    public void FromUnixTimeSeconds_TooSmall()
    {
        Assert.Throws<OverflowException>(() => Instant64.FromUnixTimeSeconds(long.MinValue / 1000000));
    }

    [Test]
    [TestCase(-1500, -2)]
    [TestCase(-1001, -2)]
    [TestCase(-1000, -1)]
    [TestCase(-999, -1)]
    [TestCase(-500, -1)]
    [TestCase(0, 0)]
    [TestCase(500, 0)]
    [TestCase(999, 0)]
    [TestCase(1000, 1)]
    [TestCase(1001, 1)]
    [TestCase(1500, 1)]
    public void ToUnixTimeSeconds(long milliseconds, int expectedSeconds)
    {
        var instant = Instant64.FromUnixTimeMilliseconds(milliseconds);
        Assert.AreEqual(expectedSeconds, instant.ToUnixTimeSeconds());
    }

    // The fact that all the expected nanosecond values are multiples of
    // a million is slightly unfortunate, but the logic is so simple that
    // it's not worth going to extreme lengths to test more fine-grained values.
    [Test]
    [TestCase(-1500, -2, 500_000_000)]
    [TestCase(-1001, -2, 999_000_000)]
    [TestCase(-1000, -1, 0)]
    [TestCase(-999, -1, 1_000_000)]
    [TestCase(-500, -1, 500_000_000)]
    [TestCase(0, 0, 0)]
    [TestCase(500, 0, 500_000_000)]
    [TestCase(999, 0, 999_000_000)]
    [TestCase(1000, 1, 0)]
    [TestCase(1001, 1, 1_000_000)]
    [TestCase(1500, 1, 500_000_000)]
    public void ToUnixTimeSecondsAndNanoseconds(long milliseconds, int expectedSeconds, int expectedNanoseconds)
    {
        var instant = Instant64.FromUnixTimeMilliseconds(milliseconds);
        var (actualSeconds, actualNanoseconds) = instant.ToUnixTimeSecondsAndNanoseconds();
        Assert.AreEqual(expectedSeconds, actualSeconds);
        Assert.AreEqual(expectedNanoseconds, actualNanoseconds);
    }

    [Test]
    [TestCase(-15000, -2)]
    [TestCase(-10001, -2)]
    [TestCase(-10000, -1)]
    [TestCase(-9999, -1)]
    [TestCase(-5000, -1)]
    [TestCase(0, 0)]
    [TestCase(5000, 0)]
    [TestCase(9999, 0)]
    [TestCase(10000, 1)]
    [TestCase(10001, 1)]
    [TestCase(15000, 1)]
    public void ToUnixTimeMilliseconds(long ticks, int expectedMilliseconds)
    {
        var instant = Instant64.FromUnixTimeTicks(ticks);
        Assert.AreEqual(expectedMilliseconds, instant.ToUnixTimeMilliseconds());
    }

    [Test]
    public void UnixConversions_ExtremeValues()
    {
        // Round down to a whole second to make round-tripping work.
        var max = Instant64.FromUnixTimeNanoseconds(Instant64.MaxValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerSecond * NodaConstants.NanosecondsPerSecond);
        Assert.AreEqual(max, Instant64.FromUnixTimeSeconds(max.ToUnixTimeSeconds()));
        Assert.AreEqual(max, Instant64.FromUnixTimeMilliseconds(max.ToUnixTimeMilliseconds()));
        Assert.AreEqual(max, Instant64.FromUnixTimeTicks(max.ToUnixTimeTicks()));

        var min = Instant64.FromUnixTimeNanoseconds(Instant64.MinValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerSecond * NodaConstants.NanosecondsPerSecond);
        Assert.AreEqual(min, Instant64.FromUnixTimeSeconds(min.ToUnixTimeSeconds()));
        Assert.AreEqual(min, Instant64.FromUnixTimeMilliseconds(min.ToUnixTimeMilliseconds()));
        Assert.AreEqual(min, Instant64.FromUnixTimeTicks(min.ToUnixTimeTicks()));
    }

    [Test]
    public void Max()
    {
        Instant64 x = Instant64.FromUnixTimeTicks(100);
        Instant64 y = Instant64.FromUnixTimeTicks(200);
        Assert.AreEqual(y, Instant64.Max(x, y));
        Assert.AreEqual(y, Instant64.Max(y, x));
        Assert.AreEqual(x, Instant64.Max(x, Instant64.MinValue));
        Assert.AreEqual(x, Instant64.Max(Instant64.MinValue, x));
        Assert.AreEqual(Instant64.MaxValue, Instant64.Max(Instant64.MaxValue, x));
        Assert.AreEqual(Instant64.MaxValue, Instant64.Max(x, Instant64.MaxValue));
    }

    [Test]
    public void Min()
    {
        Instant64 x = Instant64.FromUnixTimeTicks(100);
        Instant64 y = Instant64.FromUnixTimeTicks(200);
        Assert.AreEqual(x, Instant64.Min(x, y));
        Assert.AreEqual(x, Instant64.Min(y, x));
        Assert.AreEqual(Instant64.MinValue, Instant64.Min(x, Instant64.MinValue));
        Assert.AreEqual(Instant64.MinValue, Instant64.Min(Instant64.MinValue, x));
        Assert.AreEqual(x, Instant64.Min(Instant64.MaxValue, x));
        Assert.AreEqual(x, Instant64.Min(x, Instant64.MaxValue));
    }

    /// <summary>
    /// Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO Calendar
    /// </summary>
    [Test]
    public void DefaultConstructor()
    {
        var actual = new Instant64();
        Assert.AreEqual(Instant64.UnixEpoch, actual);
    }

    [Test]
    [TestCase(-101L, -2L)]
    [TestCase(-100L, -1L)]
    [TestCase(-99L, -1L)]
    [TestCase(-1L, -1L)]
    [TestCase(0L, 0L)]
    [TestCase(99L, 0L)]
    [TestCase(100L, 1L)]
    [TestCase(101L, 1L)]
    public void ToUnixTimeTicksTruncatesDown(long nanoseconds, long expectedTicks)
    {
        Instant64 instant = Instant64.FromUnixTimeNanoseconds(nanoseconds);
        Assert.AreEqual(expectedTicks, instant.ToUnixTimeTicks());
    }

    [Test]
    public void PlusDuration_Overflow()
    {
        TestHelper.AssertOverflow(Instant64.MinValue.Plus, -Duration64.Epsilon);
        TestHelper.AssertOverflow(Instant64.MaxValue.Plus, Duration64.Epsilon);
    }

    [Test]
    public void FromUnixTimeMilliseconds_Range()
    {
        long smallestValid = Instant64.MinValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerMillisecond;
        long largestValid = Instant64.MaxValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerMillisecond;
        TestHelper.AssertValid(Instant64.FromUnixTimeMilliseconds, smallestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeMilliseconds, smallestValid - 1);
        TestHelper.AssertValid(Instant64.FromUnixTimeMilliseconds, largestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeMilliseconds, largestValid + 1);
    }

    [Test]
    public void FromUnixTimeSeconds_Range()
    {
        long smallestValid = Instant64.MinValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerSecond;
        long largestValid = Instant64.MaxValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerSecond;
        TestHelper.AssertValid(Instant64.FromUnixTimeSeconds, smallestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeSeconds, smallestValid - 1);
        TestHelper.AssertValid(Instant64.FromUnixTimeSeconds, largestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeSeconds, largestValid + 1);
    }

    [Test]
    public void FromTicksSinceUnixEpoch_Range()
    {
        long smallestValid = Instant64.MinValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerTick;
        long largestValid = Instant64.MaxValue.ToUnixTimeNanoseconds() / NodaConstants.NanosecondsPerTick;
        TestHelper.AssertValid(Instant64.FromUnixTimeTicks, smallestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeTicks, smallestValid - 1);
        TestHelper.AssertValid(Instant64.FromUnixTimeTicks, largestValid);
        TestHelper.AssertOverflow(Instant64.FromUnixTimeTicks, largestValid + 1);
    }

    [Test]
    public void MinValue()
    {
        Assert.AreEqual("1677-09-21T00:12:43Z", Instant64.MinValue.ToString());
        Assert.AreEqual("1677-09-21T00:12:43.145224192Z", Instant64.MinValue.ToString("uuuu-MM-dd'T'HH:mm:ss.fffffffff'Z'", CultureInfo.InvariantCulture));

        var minFromUtc = Instant64.FromUtc(1677, 9, 21, 0, 12, 44).PlusNanoseconds(145224192).PlusNanoseconds(-NodaConstants.NanosecondsPerSecond);
        Assert.AreEqual(minFromUtc, Instant64.MinValue);

        var minAsInstant = Instant.FromUtc(1677, 9, 21, 0, 12, 43).PlusNanoseconds(145224192);
        Assert.AreEqual(Instant64.MinValue.ToInstant(), minAsInstant);
    }

    [Test]
    public void MaxValue()
    {
        Assert.AreEqual("2262-04-11T23:47:16Z", Instant64.MaxValue.ToString());
        Assert.AreEqual("2262-04-11T23:47:16.854775807Z", Instant64.MaxValue.ToString("uuuu-MM-dd'T'HH:mm:ss.fffffffff'Z'", CultureInfo.InvariantCulture));

        var maxFromUtc = Instant64.FromUtc(2262, 4, 11, 23, 47, 16).PlusNanoseconds(854775807);
        Assert.AreEqual(maxFromUtc, Instant64.MaxValue);

        var maxAsInstant = Instant.FromUtc(2262, 4, 11, 23, 47, 16).PlusNanoseconds(854775807);
        Assert.AreEqual(Instant64.MaxValue.ToInstant(), maxAsInstant);
    }

    [Test]
    public void ToInstant()
    {
        var instant64 = Instant64.FromUnixTimeNanoseconds(1);
        var instant = NodaConstants.UnixEpoch.PlusNanoseconds(1);
        Assert.AreEqual(instant, instant64.ToInstant());
    }
}
