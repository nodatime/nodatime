// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Testing;
using NodaTime.Testing.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class ZonedClockTest
    {
        private static readonly DateTimeZone SampleZone = new SingleTransitionDateTimeZone(NodaConstants.UnixEpoch, 1, 2);

        [Test]
        public void GetCurrent()
        {
            FakeClock underlyingClock = FakeClock.FromUtc(2000, 1, 1);
            ZonedClock zonedClock = new ZonedClock(underlyingClock, SampleZone, CalendarSystem.Iso);
            Assert.AreEqual(underlyingClock.Now, zonedClock.GetCurrentInstant());
            Assert.AreEqual(new ZonedDateTime(underlyingClock.Now, SampleZone), zonedClock.GetCurrentZonedDateTime());
            Assert.AreEqual(new LocalDateTime(2000, 1, 1, 2, 0), zonedClock.GetCurrentLocalDateTime());
            Assert.AreEqual(new LocalDateTime(2000, 1, 1, 2, 0).WithOffset(Offset.FromHours(2)),
                zonedClock.GetCurrentOffsetDateTime());
        }

        [Test]
        public void WithZone()
        {
            FakeClock underlyingClock = FakeClock.FromUtc(2000, 1, 1);
            ZonedClock zonedClock = new ZonedClock(underlyingClock, SampleZone, CalendarSystem.Iso).WithZone(DateTimeZone.Utc);
            Assert.AreEqual(new LocalDateTime(2000, 1, 1, 0, 0), zonedClock.GetCurrentLocalDateTime());
            Assert.AreEqual(new LocalDateTime(2000, 1, 1, 0, 0).WithOffset(Offset.Zero),
                zonedClock.GetCurrentOffsetDateTime());
        }

        [Test]
        public void WithCalendar()
        {
            var julian = CommonCalendars.Julian;
            FakeClock underlyingClock = new FakeClock(NodaConstants.UnixEpoch);
            ZonedClock zonedClock = new ZonedClock(underlyingClock, SampleZone, CalendarSystem.Iso).WithCalendar(julian);
            Assert.AreEqual(new LocalDateTime(1969, 12, 19, 2, 0, julian), zonedClock.GetCurrentLocalDateTime());
            Assert.AreEqual(new LocalDateTime(1969, 12, 19, 2, 0, julian).WithOffset(Offset.FromHours(2)),
                zonedClock.GetCurrentOffsetDateTime());
        }
    }
}
