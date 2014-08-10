// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class YearMonthDayCalendarTest
    {
        [Test]
        public void AllYears()
        {
            // Range of years we actually care about. We support more, but that's okay.
            for (int year = -9999; year <= 9999; year++)
            {
                var ymdc = new YearMonthDayCalendar(year, 5, 20, 0);
                Assert.AreEqual(year, ymdc.Year);
                Assert.AreEqual(5, ymdc.Month);
                Assert.AreEqual(20, ymdc.Day);
                Assert.AreEqual(CalendarOrdinal.Iso, ymdc.CalendarOrdinal);
            }
        }

        [Test]
        public void AllMonths()
        {
            // We'll never actually need 16 months, but we support that many...
            for (int month = 1; month <= 16; month++)
            {
                var ymdc = new YearMonthDayCalendar(-123, month, 20, CalendarOrdinal.HebrewCivil);
                Assert.AreEqual(-123, ymdc.Year);
                Assert.AreEqual(month, ymdc.Month);
                Assert.AreEqual(20, ymdc.Day);
                Assert.AreEqual(CalendarOrdinal.HebrewCivil, ymdc.CalendarOrdinal);
            }
        }

        [Test]
        public void AllDays()
        {
            // We'll never actually need 64 days, but we support that many...
            for (int day = 1; day <= 64; day++)
            {
                var ymdc = new YearMonthDayCalendar(-123, 12, day, CalendarOrdinal.IslamicAstronomicalBase15);
                Assert.AreEqual(-123, ymdc.Year);
                Assert.AreEqual(12, ymdc.Month);
                Assert.AreEqual(day, ymdc.Day);
                Assert.AreEqual(CalendarOrdinal.IslamicAstronomicalBase15, ymdc.CalendarOrdinal);
            }
        }

        [Test]
        public void AllCalendars()
        {
            for (int ordinal = 0; ordinal < 128; ordinal++)
            {
                CalendarOrdinal calendar = (CalendarOrdinal) ordinal;
                var ymdc = new YearMonthDayCalendar(-123, 12, 64, calendar);
                Assert.AreEqual(-123, ymdc.Year);
                Assert.AreEqual(12, ymdc.Month);
                Assert.AreEqual(64, ymdc.Day);
                Assert.AreEqual(calendar, ymdc.CalendarOrdinal);
            }
        }

        [Test]
        public void Equality()
        {
            var original = new YearMonthDayCalendar(1000, 12, 20, CalendarOrdinal.Coptic);
            TestHelper.TestEqualsStruct(original, original, new YearMonthDayCalendar(original.Year + 1, original.Month, original.Day, original.CalendarOrdinal));
            TestHelper.TestEqualsStruct(original, original, new YearMonthDayCalendar(original.Year, original.Month + 1, original.Day, original.CalendarOrdinal));
            TestHelper.TestEqualsStruct(original, original, new YearMonthDayCalendar(original.Year, original.Month, original.Day + 1, original.CalendarOrdinal));
            TestHelper.TestEqualsStruct(original, original, new YearMonthDayCalendar(original.Year, original.Month, original.Day, CalendarOrdinal.Gregorian1));
        }
    }
}
