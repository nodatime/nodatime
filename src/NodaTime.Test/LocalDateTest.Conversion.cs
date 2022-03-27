// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Linq;
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
        public void ToDateTimeUnspecified_JulianCalendar()
        {
            // Non-Gregorian calendar systems are handled by converting to the same
            // date, just like the DateTime constructor does.
            LocalDate noda = new LocalDate(2015, 4, 2, CalendarSystem.Julian);
            DateTime bcl = new DateTime(2015, 4, 2, 0, 0, 0, 0, new JulianCalendar(), DateTimeKind.Unspecified);
            Assert.AreEqual(bcl, noda.ToDateTimeUnspecified());
        }

        [Test]
        public void FromDateTime()
        {
            var expected = new LocalDate(2011, 08, 18);
            foreach (var kind in Enum.GetValues(typeof(DateTimeKind)).Cast<DateTimeKind>())
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
            foreach (var kind in Enum.GetValues(typeof(DateTimeKind)).Cast<DateTimeKind>())
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
#if NET6_0_OR_GREATER
        [Test]
        public void ToDateOnly_Gregorian()
        {
            var date = new LocalDate(2011, 8, 5);
            var expected = new DateOnly(2011, 8, 5);
            var actual = date.ToDateOnly();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDateOnly_NonGregorian()
        {
            // Julian calendar is 13 days behind Gregorian calendar in the 21st century
            var date = new LocalDate(2011, 8, 5, CalendarSystem.Julian);
            var expected = new DateOnly(2011, 8, 5, new JulianCalendar());
            var actual = date.ToDateOnly();
            Assert.AreEqual(expected, actual);
            var expectedGregorian = new DateOnly(2011, 8, 18);
            Assert.AreEqual(expectedGregorian, actual);
        }

        [Test]
        public void ToDateOnly_OutOfRange()
        {
            var date = new LocalDate(0, 12, 31);
            // While ArgumentOutOfRangeException may not be the absolute ideal exception, it conveys
            // the right impression, and is consistent with what we do elsewhere.
            Assert.Throws<ArgumentOutOfRangeException>(() => date.ToDateOnly());
        }

        [Test]
        public void FromDateOnly()
        {
            var dateOnly = new DateOnly(2011, 8, 18);
            var expected = new LocalDate(2011, 8, 18);
            var actual = LocalDate.FromDateOnly(dateOnly);
            Assert.AreEqual(expected, actual);
        }
#endif
    }
}
