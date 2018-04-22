// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class InstantDemo
    {
        [Test]
        public void Construction()
        {
            // 10 million ticks = 1 second...
            Instant instant = Instant.FromUnixTimeTicks(10000000);
            // Epoch is 1970 UTC
            // An instant isn't really "in" a time zone or calendar, but
            // it's convenient to consider UTC in the ISO-8601 calendar.
            Assert.AreEqual("1970-01-01T00:00:01Z", instant.ToString());
        }

        [Test]
        public void AdditionWithDuration()
        {
            // Some arbitrary instant. I've no idea when.
            Instant instant = Instant.FromUnixTimeTicks(150000000);
            // A very short duration: a duration is simply a number of ticks.
            Duration duration = Duration.FromTicks(1000);
            Instant later = instant + duration;
            Assert.AreEqual(Instant.FromUnixTimeTicks(150001000), later);
        }

        [Test]
        public void Comparison()
        {
            Instant early = Instant.FromUnixTimeTicks(1000);
            Instant late = Instant.FromUnixTimeTicks(2000);
            Assert.That(early < late);
        }

        [Test]
        public void ConvenienceConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 6, 9, 14, 15, 0);
            // Here's a number I prepared earlier...
            Assert.AreEqual(12760929000000000, instant.ToUnixTimeTicks());
            // But it really is correct
            Assert.AreEqual("2010-06-09T14:15:00Z", instant.ToString());
        }
    }
}