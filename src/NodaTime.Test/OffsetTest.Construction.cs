// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        [Test]
        public void Zero()
        {
            Offset test = Offset.Zero;
            Assert.AreEqual(0, test.Milliseconds);
        }

        [Test]
        public void FromSeconds_Valid()
        {
            var test = Offset.FromSeconds(12345);
            Assert.AreEqual(12345, test.Seconds);
        }

        [Test]
        public void FromSeconds_Invalid()
        {
            int seconds = 24 * NodaConstants.SecondsPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromSeconds(seconds));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromSeconds(-seconds));
        }

        [Test]
        public void FromMilliseconds_Valid()
        {
            Offset value = Offset.FromMilliseconds(-15 * NodaConstants.MillisecondsPerMinute);
            Assert.AreEqual(-15 * NodaConstants.SecondsPerMinute, value.Seconds);
            Assert.AreEqual(-15 * NodaConstants.MillisecondsPerMinute, value.Milliseconds);
        }

        [Test]
        public void FromMilliseconds_Invalid()
        {
            int millis = 24 * NodaConstants.MillisecondsPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromMilliseconds(millis));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromMilliseconds(-millis));
        }

        [Test]
        public void FromTicks_Valid()
        {
            Offset value = Offset.FromTicks(-15 * NodaConstants.TicksPerMinute);
            Assert.AreEqual(-15 * NodaConstants.SecondsPerMinute, value.Seconds);
            Assert.AreEqual(-15 * NodaConstants.TicksPerMinute, value.Ticks);
        }
        
        [Test]
        public void FromTicks_Invalid()
        {
            long ticks = 24 * NodaConstants.TicksPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromTicks(ticks));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromTicks(-ticks));
        }

        [Test]
        public void FromNanoseconds_Valid()
        {
            Offset value = Offset.FromNanoseconds(-15 * NodaConstants.NanosecondsPerMinute);
            Assert.AreEqual(-15 * NodaConstants.SecondsPerMinute, value.Seconds);
            Assert.AreEqual(-15 * NodaConstants.NanosecondsPerMinute, value.Nanoseconds);
        }

        [Test]
        public void FromNanoseconds_Invalid()
        {
            long nanos = 24 * NodaConstants.NanosecondsPerHour;
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromNanoseconds(nanos));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromNanoseconds(-nanos));
        }

        [Test]
        public void FromHours_Valid()
        {
            Offset value = Offset.FromHours(-15);
            Assert.AreEqual(-15 * NodaConstants.SecondsPerHour, value.Seconds);
        }

        [Test]
        public void FromHours_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromHours(24));
            Assert.Throws<ArgumentOutOfRangeException>(() => Offset.FromHours(-24));
        }

        [Test]
        public void FromHoursAndMinutes_Valid()
        {
            Offset value = Offset.FromHoursAndMinutes(5, 30);
            Assert.AreEqual(5 * NodaConstants.SecondsPerHour + 30 * NodaConstants.SecondsPerMinute, value.Seconds);
        }
    }
}
