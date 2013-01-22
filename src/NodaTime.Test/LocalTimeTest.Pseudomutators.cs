// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        public void PlusHours_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(14, 15, 8);
            LocalTime expectedBackward = new LocalTime(10, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusHours(2));
            Assert.AreEqual(expectedBackward, start.PlusHours(-2));
        }

        [Test]
        public void PlusHours_CrossingDayBoundary()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(8, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(20));
            Assert.AreEqual(start, start.PlusHours(20).PlusHours(-20));
        }

        [Test]
        public void PlusHours_CrossingSeveralDaysBoundary()
        {
            // Christmas day + 10 days and 1 hour
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(13, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(241));
            Assert.AreEqual(start, start.PlusHours(241).PlusHours(-241));
        }

        // Having tested that hours cross boundaries correctly, the other time unit
        // tests are straightforward
        [Test]
        public void PlusMinutes_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 17, 8);
            LocalTime expectedBackward = new LocalTime(12, 13, 8);
            Assert.AreEqual(expectedForward, start.PlusMinutes(2));
            Assert.AreEqual(expectedBackward, start.PlusMinutes(-2));
        }

        [Test]
        public void PlusSeconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 18);
            LocalTime expectedBackward = new LocalTime(12, 14, 58);
            Assert.AreEqual(expectedForward, start.PlusSeconds(10));
            Assert.AreEqual(expectedBackward, start.PlusSeconds(-10));
        }

        [Test]
        public void PlusMilliseconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 700);
            LocalTime expectedBackward = new LocalTime(12, 15, 7, 900);
            Assert.AreEqual(expectedForward, start.PlusMilliseconds(400));
            Assert.AreEqual(expectedBackward, start.PlusMilliseconds(-400));
        }

        [Test]
        public void PlusTicks_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300, 7500);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 301, 1500);
            LocalTime expectedBackward = new LocalTime(12, 15, 8, 300, 3500);
            Assert.AreEqual(expectedForward, start.PlusTicks(4000));
            Assert.AreEqual(expectedBackward, start.PlusTicks(-4000));
        }

        [Test]
        public void PlusTicks_Long()
        {
            Assert.IsTrue(NodaConstants.TicksPerStandardDay > int.MaxValue);
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 9);
            LocalTime expectedBackward = new LocalTime(12, 15, 7);
            Assert.AreEqual(expectedForward, start.PlusTicks(NodaConstants.TicksPerStandardDay + NodaConstants.TicksPerSecond));
            Assert.AreEqual(expectedBackward, start.PlusTicks(-NodaConstants.TicksPerStandardDay - NodaConstants.TicksPerSecond));
        }

    }
}
