// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    public class NodaConstantsTest
    {
        [Test]
        public void JulianEpoch()
        {
            // Compute the Julian epoch using the Julian calendar, instead of the
            // Gregorian version.
            var localEpoch = new LocalDateTime(-4712, 1, 1, 12, 0, CalendarSystem.Julian);
            var epoch = localEpoch.InZoneStrictly(DateTimeZone.Utc).ToInstant();
            Assert.AreEqual(epoch, NodaConstants.JulianEpoch);
        }
    }
}
