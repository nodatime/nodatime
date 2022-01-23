// Copyright 2020 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Test.Calendars;
using NodaTime.Test.Text;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test
{
    // Tests for YearMonth which don't naturally fall into other categories
    public partial class YearMonthTest
    {
        [Test]
        [TestCase(CalendarOrdinal.Iso, 1904, 2, 29)]
        [TestCase(CalendarOrdinal.Julian, 1900, 2, 29)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5402, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5402, 3, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5402, 8, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5402, 9, 30)]
        public void OnDayOfMonth_Valid(int calendarOrdinal, int year, int month, int day)
        {
            var calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            var yearMonth = new YearMonth(year, month, calendar);
            var actualDate = yearMonth.OnDayOfMonth(day);
            var expectedDate = new LocalDate(year, month, day, calendar);
            Assert.AreEqual(expectedDate, actualDate);
        }

        [Test]
        [TestCase(CalendarOrdinal.Iso, 1900, 2, 29)]
        [TestCase(CalendarOrdinal.Julian, 1900, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5401, 2, 30)]
        [TestCase(CalendarOrdinal.HebrewCivil, 5401, 3, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5401, 8, 30)]
        [TestCase(CalendarOrdinal.HebrewScriptural, 5401, 9, 30)]
        public void OnDayOfMonth_Invalid(int calendarOrdinal, int year, int month, int day)
        {
            var calendar = CalendarSystem.ForOrdinal((CalendarOrdinal) calendarOrdinal);
            var yearMonth = new YearMonth(year, month, calendar);
            Assert.Throws<ArgumentOutOfRangeException>(() => yearMonth.OnDayOfMonth(day));
        }
               
        [Test]
        [TestCase(2014, 8, 4, 2014, 12)]
        [TestCase(2014, 8, 5, 2015, 1)]
        [TestCase(2014, 8, 0, 2014, 8)]
        [TestCase(2014, 8, -1, 2014, 7)]
        [TestCase(2014, 8, -8, 2013, 12)]
        public void PlusMonths(int year, int month, int monthsToAdd, int expectedYear, int expectedMonth)
        {
            var yearMonth = new YearMonth(year, month);
            var expected = new YearMonth(expectedYear, expectedMonth);
            Assert.AreEqual(expected, yearMonth.PlusMonths(monthsToAdd));
        }

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void ToStringTest_Iso(CultureInfo culture)
        {
            var yearMonth = new YearMonth(2022, 1);
            using (CultureSaver.SetCultures(culture))
            {
                Assert.AreEqual(yearMonth.ToString(culture.DateTimeFormat.YearMonthPattern, culture), yearMonth.ToString());
            }
        }

        [Test]
        [TestCaseSource(typeof(Cultures), nameof(Cultures.AllCultures))]
        public void ToStringBclEquality(CultureInfo culture)
        {
            // The BCL *sometimes* use the genitive month names for year/month, even though I don't
            // think it should. There may be more complexity here, but for the moment, we'll just skip the cultures we
            // expect to fail. (We would definitely want to know if new cultures started failing.)
            string[] expectedFailures = { "ast-ES", "ca-AD", "ca-ES", "ca-ES-valencia", "ca-FR", "ca-IT", "es-PE", "es-UY", "gl-ES", "oc-FR" };
            if (expectedFailures.Contains(culture.Name))
            {
                return;
            }

            Calendar calendar = culture.Calendar;

            var calendarSystem = BclCalendars.CalendarSystemForCalendar(calendar);
            if (calendarSystem is null)
            {
                // We can't map this calendar system correctly yet; the test would be invalid.
                return;
            }

            // Use the year/month containing "January 1st 2022 ISO" but in the target culture's calendar.
            var date = new LocalDate(2022, 1, 1).WithCalendar(calendarSystem);
            var yearMonth = date.ToYearMonth();

            using (CultureSaver.SetCultures(culture))
            {
                var bclText = date.ToDateTimeUnspecified().ToString(culture.DateTimeFormat.YearMonthPattern, culture);
                var nodaText1 = yearMonth.ToString();
                var nodaText2 = yearMonth.ToString("G", culture);
                Assert.AreEqual(bclText, nodaText1);
                Assert.AreEqual(nodaText1, nodaText2);
            }
        }
    }
}