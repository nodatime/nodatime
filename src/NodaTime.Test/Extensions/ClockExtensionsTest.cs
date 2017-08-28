// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NodaTime.Test.TimeZones;
using NodaTime.Testing;
using NUnit.Framework;
using System;

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

#if !NETCOREAPP1_0
        [Test]
        public void InTzdbSystemDefaultZone()
        {
            var fakeGreenlandZone = TimeZoneInfo.CreateCustomTimeZone("Greenland Standard Time", TimeSpan.FromHours(-3), "Godthab", "Godthab");
            using (TimeZoneInfoReplacer.Replace(fakeGreenlandZone, fakeGreenlandZone))
            {
                var clock = new FakeClock(NodaConstants.UnixEpoch);
                var zonedClock = clock.InTzdbSystemDefaultZone();
                var expected = NodaConstants.UnixEpoch.InZone(DateTimeZoneProviders.Tzdb["America/Godthab"]);
                Assert.AreEqual(expected, zonedClock.GetCurrentZonedDateTime());
            }
        }
#endif

#if NET451
        [Test]
        public void InBclSystemDefaultZone()
        {
            // We can't replace DateTimeZoneProviders.Bcl with TimeZoneInfoReplacer, so we have to
            // just trust that there's a suitable system default zone.

            var zone = DateTimeZoneProviders.Bcl.GetSystemDefault();
            var clock = new FakeClock(NodaConstants.UnixEpoch);
            var zonedClock = clock.InBclSystemDefaultZone();
            var expected = NodaConstants.UnixEpoch.InZone(zone);
            Assert.AreEqual(expected, zonedClock.GetCurrentZonedDateTime());
        }
#endif
    }
}
