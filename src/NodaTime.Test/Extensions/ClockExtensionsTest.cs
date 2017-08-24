// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NodaTime.Testing;
using NUnit.Framework;

namespace NodaTime.Test.Extensions
{
    public class ClockExtensionsTest
    {
        private static DateTimeZone NewYork = DateTimeZoneProviders.Tzdb["America/New_York"];

        [Test]
        public void InZone_NoCalendar()
        {
            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var zoned = clock.InZone(NewYork);
            var zonedEpoch = NodaConstants.UnixEpoch.InZone(NewYork);
            Assert.AreEqual(zonedEpoch, zoned.GetCurrentZonedDateTime());
        }

        [Test]
        public void InZone_WithCalendar()
        {
            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var zoned = clock.InZone(NewYork, CalendarSystem.Julian);
            var zonedEpoch = NodaConstants.UnixEpoch.InZone(NewYork, CalendarSystem.Julian);
            Assert.AreEqual(zonedEpoch, zoned.GetCurrentZonedDateTime());
        }

        [Test]
        public void InUtc()
        {
            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var zoned = clock.InUtc();
            var zonedEpoch = NodaConstants.UnixEpoch.InUtc();
            Assert.AreEqual(zonedEpoch, zoned.GetCurrentZonedDateTime());
        }

        // No tests for system default
    }
}
