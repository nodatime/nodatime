// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for HebrewYearMonthDayCalculator via the Hebrew CalendarSystem.
    /// See http://noda-time.blogspot.co.uk/2014/06/hebrew-calendar-cheat-sheet.html
    /// for sample year information.
    /// </summary>
    [TestFixture]
    public class HebrewCalendarSystemTest
    {
        [Test]
        public void IsLeapYear()
        {
            var bcl = new HebrewCalendar();
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime);
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime);
            var noda = CommonCalendars.CivilHebrew;

            for (int year = minYear; year <= maxYear; year++)
            {
                Assert.AreEqual(bcl.IsLeapYear(year), noda.IsLeapYear(year));
            }
        }

        /// <summary>
        /// This tests every day for the BCL-supported Hebrew calendar range, testing various aspects of each date,
        /// using the civil month numbering.
        /// </summary>
        [Test, Timeout(300000)] // Can take a long time under NCrunch.
        public void BclThroughHistory_Civil()
        {
            Calendar bcl = new HebrewCalendar();
            var noda = CommonCalendars.CivilHebrew;

            // The min supported date/time starts part way through the year
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime) + 1;
            // The max supported date/time ends part way through the year
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime) - 1;

            for (int year = minYear; year <= maxYear; year++)
            {
                int months = bcl.GetMonthsInYear(year);
                Assert.AreEqual(months, noda.GetMonthsInYear(year));
                for (int month = 1; month <= months; month++)
                {
                    Assert.AreEqual(bcl.GetDaysInMonth(year, month), noda.GetDaysInMonth(year, month),
                        "Year: {0}; Month: {1}", year, month);
                    for (int day = 1; day < bcl.GetDaysInMonth(year, month); day++)
                    {
                        DateTime bclDate = new DateTime(year, month, day, bcl);
                        LocalDate nodaDate = new LocalDate(year, month, day, noda);
                        Assert.AreEqual(bclDate, nodaDate.AtMidnight().ToDateTimeUnspecified());
                        Assert.AreEqual(nodaDate, LocalDateTime.FromDateTime(bclDate).WithCalendar(noda).Date);
                        Assert.AreEqual(year, nodaDate.Year);
                        Assert.AreEqual(month, nodaDate.Month);
                        Assert.AreEqual(day, nodaDate.Day);
                    }
                }
            }
        }

        /// <summary>
        /// This tests every day for the BCL-supported Hebrew calendar range, testing various aspects of each date,
        /// using the scriptural month numbering.
        /// </summary>
        [Test, Timeout(300000)] // Can take a long time under NCrunch.
        public void BclThroughHistory_Scriptural()
        {
            Calendar bcl = new HebrewCalendar();
            var noda = CommonCalendars.ScripturalHebrew;

            // The min supported date/time starts part way through the year
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime) + 1;
            // The max supported date/time ends part way through the year
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime) - 1;

            for (int year = minYear; year <= maxYear; year++)
            {
                int months = bcl.GetMonthsInYear(year);
                Assert.AreEqual(months, noda.GetMonthsInYear(year));
                for (int civilMonth = 1; civilMonth <= months; civilMonth++)
                {
                    int scripturalMonth = HebrewMonthConverter.CivilToScriptural(year, civilMonth);
                    Assert.AreEqual(bcl.GetDaysInMonth(year, civilMonth), noda.GetDaysInMonth(year, scripturalMonth),
                        "Year: {0}; Month: {1} (civil)", year, civilMonth);
                    for (int day = 1; day < bcl.GetDaysInMonth(year, civilMonth); day++)
                    {
                        DateTime bclDate = new DateTime(year, civilMonth, day, bcl);
                        LocalDate nodaDate = new LocalDate(year, scripturalMonth, day, noda);
                        Assert.AreEqual(bclDate, nodaDate.AtMidnight().ToDateTimeUnspecified(), "{0}-{1}-{2}", year, scripturalMonth, day);
                        Assert.AreEqual(nodaDate, LocalDateTime.FromDateTime(bclDate).WithCalendar(noda).Date);
                        Assert.AreEqual(year, nodaDate.Year);
                        Assert.AreEqual(scripturalMonth, nodaDate.Month);
                        Assert.AreEqual(day, nodaDate.Day);
                    }
                }
            }
        }

        // Test cases are in scriptural month numbering, but we check both. This is
        // mostly testing the behaviour of SetYear, via LocalDate.PlusYears.
        [Test]
        // Simple case
        [TestCase("5405-02-10", 1, "5406-02-10")]
        // Adar mapping - Adar from non-leap maps to Adar II in leap;
        // Adar I and Adar II both map to Adar in a non-leap, except for the 30th of Adar I
        // which maps to the 1st of Nisan.
        [TestCase("5402-12-05", 1, "5403-12-05")] // Mapping from Adar I to Adar
        [TestCase("5402-13-05", 1, "5403-12-05")] // Mapping from Adar II to Adar
        [TestCase("5402-12-30", 1, "5403-01-01")] // Mapping from 30th of Adar I to 1st of Nisan
        [TestCase("5401-12-05", 1, "5402-13-05")] // Mapping from Adar to Adar II
        // Transfer to another leap year
        [TestCase("5402-12-05", 2, "5404-12-05")] // Adar I to Adar I
        [TestCase("5402-12-30", 2, "5404-12-30")] // 30th of Adar I to 30th of Adar I
        [TestCase("5402-13-05", 2, "5404-13-05")] // Adar II to Adar II
        // Rollover of 30th of Kislev and Heshvan to the 1st of the next month.
        [TestCase("5402-08-30", 1, "5403-09-01")] // Rollover from 30th Heshvan to 1st Kislev
        [TestCase("5400-09-30", 1, "5401-10-01")] // Rollover from 30th Kislev to 1st Tevet
        // No rollover required (target year has 30 days in as well)
        [TestCase("5402-08-30", 3, "5405-08-30")] // No truncation in Heshvan (both 5507 and 5509 are long)
        [TestCase("5400-09-30", 2, "5402-09-30")] // No truncation in Kislev (both 5503 and 5504 are long)
        public void SetYear(string startText, int years, string expectedEndText)
        {
            var civil = CommonCalendars.CivilHebrew;
            var scriptural = CommonCalendars.ScripturalHebrew;
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithTemplateValue(new LocalDate(5774, 1, 1, scriptural)); // Sample value in 2014 ISO

            var start = pattern.Parse(startText).Value;
            var expectedEnd = pattern.Parse(expectedEndText).Value;
            Assert.AreEqual(expectedEnd, start.PlusYears(years));

            // Check civil as well... the date should be the same (year, month, day) even though
            // the numbering is different.
            Assert.AreEqual(expectedEnd.WithCalendar(civil), start.WithCalendar(civil).PlusYears(years));
        }

        [Test]
        [TestCaseSource("AddAndSubtractMonthCases")]
        public void AddMonths_MonthsBetween(string startText, int months, string expectedEndText)
        {
            var civil = CommonCalendars.CivilHebrew;
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithTemplateValue(new LocalDate(5774, 1, 1, civil)); // Sample value in 2014 ISO

            var start = pattern.Parse(startText).Value;
            var expectedEnd = pattern.Parse(expectedEndText).Value;
            Assert.AreEqual(expectedEnd, start.PlusMonths(months));
        }

        [Test]
        [TestCaseSource("AddAndSubtractMonthCases")]
        [TestCaseSource("MonthsBetweenCases")]
        public void MonthsBetween(string startText, int expectedMonths, string endText)
        {
            var civil = CommonCalendars.CivilHebrew;
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithTemplateValue(new LocalDate(5774, 1, 1, civil)); // Sample value in 2014 ISO

            var start = pattern.Parse(startText).Value;
            var end = pattern.Parse(endText).Value;
            Assert.AreEqual(expectedMonths, Period.Between(start, end, PeriodUnits.Months).Months);
        }

        [Test]
        public void MonthsBetween_TimeOfDay()
        {
            var civil = CommonCalendars.CivilHebrew;
            var start = new LocalDateTime(5774, 5, 10, 15, 0, civil); // 3pm
            var end = new LocalDateTime(5774, 7, 10, 5, 0, civil); // 5am
            // Would be 2, but the start time is later than the end time.
            Assert.AreEqual(1, Period.Between(start, end, PeriodUnits.Months).Months);
        }

        [Test]
        [TestCase(HebrewMonthNumbering.Civil)]
        [TestCase(HebrewMonthNumbering.Scriptural)]
        public void DayOfYearAndReverse(HebrewMonthNumbering numbering)
        {
            var calculator = new HebrewYearMonthDayCalculator(numbering);
            for (int year = 5400; year < 5419; year++)
            {
                int daysInYear = calculator.GetDaysInYear(year);
                for (int dayOfYear = 1; dayOfYear <= daysInYear; dayOfYear++)
                {
                    YearMonthDay ymd = calculator.GetYearMonthDay(year, dayOfYear);
                    Assert.AreEqual(dayOfYear, calculator.GetDayOfYear(ymd));
                }
            }
        }

        [Test]
        public void GetDaysSinceEpoch()
        {
            var calculator = new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Scriptural);
            var unixEpoch = new YearMonthDay(5730, 10, 23);
            Assert.AreEqual(0, calculator.GetDaysSinceEpoch(unixEpoch));
        }

        [Test]
        public void DaysAtStartOfYear()
        {
            // These are somewhat random values used when diagnosing an issue.
            var calculator = new HebrewYearMonthDayCalculator(HebrewMonthNumbering.Scriptural);
            Assert.AreEqual(-110, calculator.GetStartOfYearInDays(5730));
            Assert.AreEqual(273, calculator.GetStartOfYearInDays(5731));
            Assert.AreEqual(-140735, calculator.GetStartOfYearInDays(5345));
            Assert.AreEqual(new YearMonthDay(5345, 1, 1), calculator.GetYearMonthDay(-140529));
        }


        // Cases used for adding months and differences between months.
        // 5501 is not a leap year; 5502 is; 5503 is not; 5505 is.
        // Heshvan (civil 2) is long in 5507 and 5509; it is short in 5506 and 5508
        // Kislev (civil 3) is long in 5503-5505; it is short in 5502 and 5506
        // Test cases are in civil month numbering (for the sake of sanity!) - the
        // implementation performs converts to civil for most of the work.
        private static readonly object[] AddAndSubtractMonthCases =
        {
            new object[] {"5502-02-13", 3, "5502-05-13"}, // Simple
            new object[] {"5502-02-13", 238, "5521-05-13"}, // Simple after a 19-year cycle
            new object[] {"5502-05-13", -3, "5502-02-13"}, // Simple (negative)
            new object[] {"5521-05-13", -238, "5502-02-13"}, // Simple after a 19-year cycle (negative)
            new object[] {"5501-02-13", 12, "5502-02-13"}, // Not a leap year
            new object[] {"5502-02-13", 13, "5503-02-13"}, // Leap year
            new object[] {"5501-02-13", 26, "5503-03-13"}, // Traversing both (and then an extra month)
            new object[] {"5502-02-13", -12, "5501-02-13"}, // Not a leap year (negative)
            new object[] {"5503-02-13", -13, "5502-02-13"}, // Leap year (negative)
            new object[] {"5503-03-13", -26, "5501-02-13"}, // Traversing both (and then an extra month) (negative)
            new object[] {"5507-01-30", 1, "5507-02-30"}, // Long Heshvan
            new object[] {"5506-01-30", 1, "5506-02-29"}, // Short Heshvan
            new object[] {"5505-01-30", 2, "5505-03-30"}, // Long Kislev
            new object[] {"5506-01-30", 2, "5506-03-29"}, // Short Kislev
        };

        // Test cases only used for testing MonthsBetween, in the same format as AddAndSubtractMonthCases
        // for simplicity.
        private static readonly object[] MonthsBetweenCases =
        {
            new object[] {"5502-02-13", 1, "5502-03-15"},
            new object[] {"5502-02-13", 0, "5502-03-05"},
            new object[] {"5502-02-13", 0, "5502-02-15"},
            new object[] {"5502-02-13", 0, "5502-02-05"},
            new object[] {"5502-02-13", 0, "5502-01-15"},
            new object[] {"5502-02-13", -1, "5502-01-05"},
        };
    }
}
