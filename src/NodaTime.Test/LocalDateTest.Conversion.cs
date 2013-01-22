// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void AtMidnight()
        {
            LocalDate date = new LocalDate(2011, 6, 29);
            LocalDateTime expected = new LocalDateTime(2011, 6, 29, 0, 0, 0);
            Assert.AreEqual(expected, date.AtMidnight());
        }

        [Test]
        public void WithCalendar()
        {
            LocalDate isoEpoch = new LocalDate(1970, 1, 1);
            LocalDate julianEpoch = isoEpoch.WithCalendar(CalendarSystem.GetJulianCalendar(4));
            Assert.AreEqual(1969, julianEpoch.Year);
            Assert.AreEqual(12, julianEpoch.Month);
            Assert.AreEqual(19, julianEpoch.Day);
        }
    }
}
