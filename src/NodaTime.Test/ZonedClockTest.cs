// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
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
            var julian = CommonCalendars.Julian;
            FakeClock underlyingClock = new FakeClock(NodaConstants.UnixEpoch);
            ZonedClock zonedClock = underlyingClock.InZone(SampleZone, julian);
            Assert.AreEqual(NodaConstants.UnixEpoch, zonedClock.GetCurrentInstant());
            Assert.AreEqual(new ZonedDateTime(underlyingClock.GetCurrentInstant(), SampleZone, julian),
                zonedClock.GetCurrentZonedDateTime());
            Assert.AreEqual(new LocalDateTime(1969, 12, 19, 2, 0, julian), zonedClock.GetCurrentLocalDateTime());
            Assert.AreEqual(new LocalDateTime(1969, 12, 19, 2, 0, julian).WithOffset(Offset.FromHours(2)),
                zonedClock.GetCurrentOffsetDateTime());
            Assert.AreEqual(new LocalDate(1969, 12, 19, julian), zonedClock.GetCurrentDate());
            Assert.AreEqual(new LocalTime(2, 0, 0), zonedClock.GetCurrentTimeOfDay());
        }
    }
}
