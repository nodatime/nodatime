// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Demo
{
    [TestFixture]
    public class TimeZoneDemo
    {
        [Test]
        public void EarlyParis()
        {
            DateTimeZone paris = DateTimeZoneProviders.Tzdb["Europe/Paris"];
            Offset offset = paris.GetUtcOffset(Instant.FromUtc(1900, 1, 1, 0, 0));
            Assert.AreEqual("+00:09:21", offset.ToString());
        }

        [Test]
        public void BritishDoubleSummerTime()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            Offset offset = london.GetUtcOffset(Instant.FromUtc(1942, 7, 1, 0, 0));
            Assert.AreEqual("+02", offset.ToString());
        }

        [Test]
        public void ZoneInterval()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZoneInterval interval = london.GetZoneInterval(Instant.FromUtc(2010, 6, 19, 0, 0));
            Assert.AreEqual("BST", interval.Name);
            Assert.AreEqual(Instant.FromUtc(2010, 3, 28, 1, 0), interval.Start);
            Assert.AreEqual(Instant.FromUtc(2010, 10, 31, 1, 0), interval.End);
            Assert.AreEqual(Offset.FromHours(1), interval.WallOffset);
            Assert.AreEqual(Offset.FromHours(1), interval.Savings);
        }
    }
}