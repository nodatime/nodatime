// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
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
            LocalDate julianEpoch = isoEpoch.WithCalendar(CalendarSystem.Julian);
            Assert.AreEqual(1969, julianEpoch.Year);
            Assert.AreEqual(12, julianEpoch.Month);
            Assert.AreEqual(19, julianEpoch.Day);
        }

        [Test]
        public void WithOffset()
        {
            var date = new LocalDate(2011, 6, 29);
            var offset = Offset.FromHours(5);
            var expected = new OffsetDate(date, offset);
            Assert.AreEqual(expected, date.WithOffset(offset));
        }

        [Test]
        public void ToDateTimeUnspecified()
        {
            LocalDate noda = new LocalDate(2015, 4, 2);
            DateTime bcl = new DateTime(2015, 4, 2, 0, 0, 0, DateTimeKind.Unspecified);
            Assert.AreEqual(bcl, noda.ToDateTimeUnspecified());
        }

        [Test]
        public void FromDateTime()
        {
            var expected = new LocalDate(2011, 08, 18);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                var bcl = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                var actual = LocalDate.FromDateTime(bcl);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void FromDateTime_WithCalendar()
        {
            // Julian calendar is 13 days behind Gregorian calendar in the 21st century
            var expected = new LocalDate(2011, 08, 05, CalendarSystem.Julian);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                var bcl = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                var actual = LocalDate.FromDateTime(bcl, CalendarSystem.Julian);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void WithCalendar_OutOfRange()
        {
            LocalDate start = new LocalDate(1, 1, 1);
            Assert.Throws<ArgumentOutOfRangeException>(() => start.WithCalendar(CalendarSystem.PersianSimple));
        }
    }
}
