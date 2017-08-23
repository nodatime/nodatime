// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        [TestCase(-1, 0)]
        [TestCase(24, 0)]
        [TestCase(0, -1)]
        [TestCase(0, 60)]
        public void InvalidConstructionToMinute(int hour, int minute)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalTime(hour, minute));
        }

        [Test]
        [TestCase(-1, 0, 0)]
        [TestCase(24, 0, 0)]
        [TestCase(0, -1, 0)]
        [TestCase(0, 60, 0)]
        [TestCase(0, 0, 60)]
        [TestCase(0, 0, -1)]
        public void InvalidConstructionToSecond(int hour, int minute, int second)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalTime(hour, minute, second));
        }

        [Test]
        [TestCase(-1, 0, 0, 0)]
        [TestCase(24, 0, 0, 0)]
        [TestCase(0, -1, 0, 0)]
        [TestCase(0, 60, 0, 0)]
        [TestCase(0, 0, 60, 0)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, 0, -1)]
        [TestCase(0, 0, 0, 1000)]
        public void InvalidConstructionToMillisecond(int hour, int minute, int second, int millisecond)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalTime(hour, minute, second, millisecond));
        }

        [Test]
        [TestCase(-1, 0, 0, 0, 0)]
        [TestCase(24, 0, 0, 0, 0)]
        [TestCase(0, -1, 0, 0, 0)]
        [TestCase(0, 60, 0, 0, 0)]
        [TestCase(0, 0, 60, 0, 0)]
        [TestCase(0, 0, -1, 0, 0)]
        [TestCase(0, 0, 0, -1, 0)]
        [TestCase(0, 0, 0, 1000, 0)]
        [TestCase(0, 0, 0, 0, -1)]
        [TestCase(0, 0, 0, 0, (int) NodaConstants.TicksPerMillisecond)]
        public void FromHourMinuteSecondMillisecondTick_Invalid(int hour, int minute, int second, int millisecond, int tick)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => LocalTime.FromHourMinuteSecondMillisecondTick(hour, minute, second, millisecond, tick));
        }

        [Test]
        [TestCase(-1, 0, 0, 0)]
        [TestCase(24, 0, 0, 0)]
        [TestCase(0, -1, 0, 0)]
        [TestCase(0, 60, 0, 0)]
        [TestCase(0, 0, 60, 0)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, 0, -1)]
        [TestCase(0, 0, 0, (int) NodaConstants.TicksPerSecond)]
        public void FromHourMinuteSecondTick_Invalid(int hour, int minute, int second, int tick)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => LocalTime.FromHourMinuteSecondTick(hour, minute, second, tick));
        }

        [Test]
        public void FromHourMinuteSecondTick_Valid()
        {
            var result = LocalTime.FromHourMinuteSecondTick(1, 2, 3, (int) (NodaConstants.TicksPerSecond - 1));
            Assert.AreEqual(1, result.Hour);
            Assert.AreEqual(2, result.Minute);
            Assert.AreEqual(3, result.Second);
            Assert.AreEqual((int)(NodaConstants.TicksPerSecond - 1), result.TickOfSecond);
        }

        [Test]
        [TestCase(-1, 0, 0, 0)]
        [TestCase(24, 0, 0, 0)]
        [TestCase(0, -1, 0, 0)]
        [TestCase(0, 60, 0, 0)]
        [TestCase(0, 0, 60, 0)]
        [TestCase(0, 0, -1, 0)]
        [TestCase(0, 0, 0, -1)]
        [TestCase(0, 0, 0, NodaConstants.NanosecondsPerSecond)]
        public void FromHourMinuteSecondNanosecond_Invalid(int hour, int minute, int second, long nanosecond)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => LocalTime.FromHourMinuteSecondNanosecond(hour, minute, second, nanosecond));
        }

        [Test]
        public void FromNanosecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromNanosecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight.PlusNanoseconds(-1), LocalTime.FromNanosecondsSinceMidnight(NodaConstants.NanosecondsPerDay - 1));
        }

        [Test]
        public void FromNanosecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromNanosecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromNanosecondsSinceMidnight(NodaConstants.NanosecondsPerDay));
        }

        [Test]
        public void FromTicksSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromTicksSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromTicks(1), LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerDay - 1));
        }

        [Test]
        public void FromTicksSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromTicksSinceMidnight(NodaConstants.TicksPerDay));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromMillisecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromMilliseconds(1), LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerDay - 1));
        }

        [Test]
        public void FromMillisecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromMillisecondsSinceMidnight(NodaConstants.MillisecondsPerDay));
        }

        [Test]
        public void FromSecondsSinceMidnight_Valid()
        {
            Assert.AreEqual(LocalTime.Midnight, LocalTime.FromSecondsSinceMidnight(0));
            Assert.AreEqual(LocalTime.Midnight - Period.FromSeconds(1), LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerDay - 1));
        }

        [Test]
        public void FromSecondsSinceMidnight_RangeChecks()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalTime.FromSecondsSinceMidnight(NodaConstants.SecondsPerDay));
        }
    }
}
