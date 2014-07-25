// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Algiers had DST until May 1st 1981, after which time it didn't have any - so
    /// we use that to test a time zone whose transitions run out. (When Algiers
    /// decided to stop using DST, it changed its standard offset to be what had previously
    /// been its DST offset, i.e. +1.)
    /// </summary>
    [TestFixture]
    public class AlgiersTest
    {
        private static readonly DateTimeZone Algiers = DateTimeZoneProviders.Tzdb["Africa/Algiers"];

        [Test]
        public void GetPeriod_BeforeLast()
        {
            Instant april1981 = Instant.FromUtc(1981, 4, 1, 0, 0);
            var actual = Algiers.GetZoneInterval(april1981);
            var expected = new ZoneInterval("WET", Instant.FromTicksSinceUnixEpoch(3418020000000000L), Instant.FromTicksSinceUnixEpoch(3575232000000000L), Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetPeriod_AfterLastTransition()
        {
            var may1981 = DateTimeZone.Utc.AtStrictly(new LocalDateTime(1981, 5, 1, 0, 0, 1)).ToInstant();
            var actual = Algiers.GetZoneInterval(may1981);
            var expected = new ZoneInterval("CET", Instant.FromTicksSinceUnixEpoch(3575232000000000L), Instant.MaxValue, Offset.FromSeconds(NodaConstants.SecondsPerHour), Offset.Zero);
            Assert.AreEqual(expected, actual);
        }
    }
}
