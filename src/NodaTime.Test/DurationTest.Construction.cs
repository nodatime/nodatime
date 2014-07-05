// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.Ticks);
        }

        [Test]
        public void Factory_StandardDays()
        {
            Duration test = Duration.FromStandardDays(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerStandardDay, test.Ticks);

            test = Duration.FromStandardDays(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerStandardDay, test.Ticks);

            test = Duration.FromStandardDays(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardHours()
        {
            Duration test = Duration.FromHours(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerHour, test.Ticks);

            test = Duration.FromHours(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerHour, test.Ticks);

            test = Duration.FromHours(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void FromMinutes()
        {
            Duration test = Duration.FromMinutes(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerMinute, test.Ticks);

            test = Duration.FromMinutes(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerMinute, test.Ticks);

            test = Duration.FromMinutes(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void FromSeconds()
        {
            Duration test = Duration.FromSeconds(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, test.Ticks);

            test = Duration.FromSeconds(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, test.Ticks);

            test = Duration.FromSeconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void FromMilliseconds()
        {
            Duration test = Duration.FromMilliseconds(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.FromMilliseconds(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.FromMilliseconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void FromTicks()
        {
            Duration test = Duration.FromTicks(1);
            Assert.AreEqual(1, test.Ticks);

            test = Duration.FromTicks(2);
            Assert.AreEqual(2, test.Ticks);

            test = Duration.FromTicks(0);
            Assert.AreEqual(Duration.Zero, test);
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
    }
}
