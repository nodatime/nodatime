// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;
using static NodaTime.IsoDayOfWeek;
using static NodaTime.CalendarSystem;

namespace NodaTime.Test.Calendars
{
    public class RegularWeekYearRuleTest
    {
        [Test]
        public void RoundtripFirstDay_Iso7()
        {
            // In the Gregorian calendar with a minimum of 7 days in the first
            // week, Tuesday January 1st -9998 is in week year -9999. We should be able to
            // round-trip.
            var rule = RegularWeekYearRule.ForMinDaysInFirstWeek(7);
            var date = new LocalDate(-9998, 1, 1);
            Assert.AreEqual(date, rule.GetLocalDate(
                rule.GetWeekYear(date),
                rule.GetWeekOfWeekYear(date),
                date.IsoDayOfWeek));
        }

        [Test]
        public void RoundtripLastDay_Iso1()
        {
            // In the Gregorian calendar with a minimum of 1 day in the first
            // week, Friday December 31st 9999 is in week year 10000. We should be able to
            // round-trip.
            var rule = RegularWeekYearRule.ForMinDaysInFirstWeek(1);
            var date = new LocalDate(9999, 12, 31);
            Assert.AreEqual(date, rule.GetLocalDate(
                rule.GetWeekYear(date),
                rule.GetWeekOfWeekYear(date),
                date.IsoDayOfWeek));
        }

        [Test]
        public void OutOfRange_ValidWeekYearAndWeek_TooEarly()
        {
            // Gregorian 4: Week year 1 starts on Monday December 31st -9999,
            // and is therefore out of range, even though the week-year
            // and week-of-week-year are valid.
            Assert.Throws<ArgumentOutOfRangeException>(
                () => RegularWeekYearRule.Iso.GetLocalDate(-9998, 1, Monday));

            // Sanity check: no exception for January 1st
            RegularWeekYearRule.Iso.GetLocalDate(-9998, 1, Tuesday);
        }

        [Test]
        public void OutOfRange_ValidWeekYearAndWeek_TooLate()
        {
            // Gregorian 4: December 31st 9999 is a Friday, so the Saturday of the
            // same week is therefore out of range, even though the week-year
            // and week-of-week-year are valid.
            Assert.Throws<ArgumentOutOfRangeException>(
                () => RegularWeekYearRule.Iso.GetLocalDate(9999, 52, Saturday));

            // Sanity check: no exception for December 31st
            RegularWeekYearRule.Iso.GetLocalDate(9999, 52, Friday);
        }

        // Tests ported from IsoCalendarSystemTest and LocalDateTest.Construction
        [Test]
        [TestCase(2011, 1, 1, 2010, 52, Saturday)]
        [TestCase(2012, 12, 31, 2013, 1, Monday)]
            [TestCase(1960, 1, 19, 1960, 3, Tuesday)]
            [TestCase(2012, 10, 19, 2012, 42, Friday)]
            [TestCase(2011, 1, 1, 2010, 52, Saturday)]
            [TestCase(2012, 12, 31, 2013, 1, Monday)]
            [TestCase(2005, 1, 2, 2004, 53, Sunday)]
        public void WeekYearDifferentToYear(int year, int month, int day, int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            var date = new LocalDate(year, month, day);
            Assert.AreEqual(weekYear, RegularWeekYearRule.Iso.GetWeekYear(date));
            Assert.AreEqual(weekOfWeekYear, RegularWeekYearRule.Iso.GetWeekOfWeekYear(date));
            Assert.AreEqual(dayOfWeek, date.IsoDayOfWeek);
            Assert.AreEqual(date, RegularWeekYearRule.Iso.GetLocalDate(weekYear, weekOfWeekYear, dayOfWeek));
        }

        // Ported from CalendarSystemTest.Validation
        [Test]
        [TestCase(2009, 53)]
        [TestCase(2010, 52)]
        [TestCase(2011, 52)]
        [TestCase(2012, 52)]
        [TestCase(2013, 52)]
        [TestCase(2014, 52)]
        [TestCase(2015, 53)]
        [TestCase(2016, 52)]
        [TestCase(2017, 52)]
        [TestCase(2018, 52)]
        [TestCase(2019, 52)]
        public void GetWeeksInWeekYear(int weekYear, int expectedResult)
        {
            Assert.AreEqual(expectedResult, RegularWeekYearRule.Iso.GetWeeksInWeekYear(weekYear));
        }

        // Ported from LocalDateTest.BasicProperties
        // See http://stackoverflow.com/questions/8010125
        [Test]
        [TestCase(2007, 12, 31, 1)]
        [TestCase(2008, 1, 6, 1)]
        [TestCase(2008, 1, 7, 2)]
        [TestCase(2008, 12, 28, 52)]
        [TestCase(2008, 12, 29, 1)]
        [TestCase(2009, 1, 4, 1)]
        [TestCase(2009, 1, 5, 2)]
        [TestCase(2009, 12, 27, 52)]
        [TestCase(2009, 12, 28, 53)]
        [TestCase(2010, 1, 3, 53)]
        [TestCase(2010, 1, 4, 1)]
        public void WeekOfWeekYear_ComparisonWithOracle(int year, int month, int day, int weekOfWeekYear)
        {
            var date = new LocalDate(year, month, day);
            Assert.AreEqual(weekOfWeekYear, RegularWeekYearRule.Iso.GetWeekOfWeekYear(date));
        }

        [Test]
        [TestCase(2000, Saturday, 2)]
        [TestCase(2001, Monday, 7)]
        [TestCase(2002, Tuesday, 6)]
        [TestCase(2003, Wednesday, 5)]
        [TestCase(2004, Thursday, 4)]
        [TestCase(2005, Saturday, 2)]
        [TestCase(2006, Sunday, 1)]
        public void Gregorian(int year, IsoDayOfWeek firstDayOfYear, int maxMinDaysInFirstWeekForSameWeekYear)
        {
            var startOfCalendarYear = new LocalDate(year, 1, 1);
            Assert.AreEqual(firstDayOfYear, startOfCalendarYear.IsoDayOfWeek);

            // Rules which put the first day of the calendar year into the same week year
            for (int i = 1; i <= maxMinDaysInFirstWeekForSameWeekYear; i++)
            {
                var rule = RegularWeekYearRule.ForMinDaysInFirstWeek(i);
                Assert.AreEqual(year, rule.GetWeekYear(startOfCalendarYear));
                Assert.AreEqual(1, rule.GetWeekOfWeekYear(startOfCalendarYear));
            }
            // Rules which put the first day of the calendar year into the previous week year
            for (int i = maxMinDaysInFirstWeekForSameWeekYear + 1; i <= 7; i++)
            {
                var rule = RegularWeekYearRule.ForMinDaysInFirstWeek(i);
                Assert.AreEqual(year - 1, rule.GetWeekYear(startOfCalendarYear));
                Assert.AreEqual(rule.GetWeeksInWeekYear(year - 1), rule.GetWeekOfWeekYear(startOfCalendarYear));
            }
        }

        // Test cases from https://blogs.msdn.microsoft.com/shawnste/2006/01/24/iso-8601-week-of-year-format-in-microsoft-net/
        // which distinguish our ISO option from the BCL. When we implement the BCL equivalents, we should have similar
        // tests there...
        [Test]
        [TestCase(2000, 12, 31, 2000, 52, Sunday)]
        [TestCase(2001, 1, 1, 2001, 1, Monday)]
        [TestCase(2005, 1, 1, 2004, 53, Saturday)]
        [TestCase(2007, 12, 31, 2008, 1, Monday)]
        public void Iso(int year, int month, int day, int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            var viaCalendar = new LocalDate(year, month, day);
            var rule = RegularWeekYearRule.Iso;
            Assert.AreEqual(weekYear, rule.GetWeekYear(viaCalendar));
            Assert.AreEqual(weekOfWeekYear, rule.GetWeekOfWeekYear(viaCalendar));
            Assert.AreEqual(dayOfWeek, viaCalendar.IsoDayOfWeek);
            var viaRule = rule.GetLocalDate(weekYear, weekOfWeekYear, dayOfWeek);
            Assert.AreEqual(viaCalendar, viaRule);
        }

        /// <summary>
        /// Just a sample test of not using the Gregorian/ISO calendar system.
        /// </summary>
        [Test]
        [TestCase(5400, Thursday, 1639, 9, 29, 51, 5400, 1)]
        [TestCase(5401, Monday, 1640, 9, 17, 50, 5401, 1)]
        [TestCase(5402, Thursday, 1641, 9, 5, 55, 5402, 1)]
        [TestCase(5403, Thursday, 1642, 9, 25, 51, 5403, 1)]
        [TestCase(5404, Monday, 1643, 9, 14, 55, 5404, 1)]
        [TestCase(5405, Saturday, 1644, 10, 1, 50, 5404, 55)]
        [TestCase(5406, Thursday, 1645, 9, 21, 51, 5406, 1)]
        [TestCase(5407, Monday, 1646, 9, 10, 55, 5407, 1)]
        [TestCase(5408, Monday, 1647, 9, 30, 50, 5408, 1)]
        [TestCase(5409, Thursday, 1648, 9, 17, 51, 5409, 1)]
        [TestCase(5410, Tuesday, 1649, 9, 7, 55, 5410, 1)]
        public void HebrewCalendar(int year, IsoDayOfWeek expectedFirstDay,
            int isoYear, int isoMonth, int isoDay, // Mostly for documentation
            int expectedWeeks, int expectedWeekYearOfFirstDay, int expectedWeekOfWeekYearOfFirstDay)
        {
            var civilDate = new LocalDate(year, 1, 1, HebrewCivil);
            var rule = RegularWeekYearRule.Iso;
            Assert.AreEqual(expectedFirstDay, civilDate.IsoDayOfWeek);
            Assert.AreEqual(civilDate.WithCalendar(CalendarSystem.Iso), new LocalDate(isoYear, isoMonth, isoDay));
            Assert.AreEqual(expectedWeeks, rule.GetWeeksInWeekYear(year, HebrewCivil));
            Assert.AreEqual(expectedWeekYearOfFirstDay, rule.GetWeekYear(civilDate));
            Assert.AreEqual(expectedWeekOfWeekYearOfFirstDay, rule.GetWeekOfWeekYear(civilDate));
            Assert.AreEqual(civilDate,
                rule.GetLocalDate(expectedWeekYearOfFirstDay, expectedWeekOfWeekYearOfFirstDay, expectedFirstDay, HebrewCivil));

            // The scriptural month numbering system should have the same week-year and week-of-week-year.
            var scripturalDate = civilDate.WithCalendar(HebrewScriptural);
            Assert.AreEqual(expectedWeeks, rule.GetWeeksInWeekYear(year, HebrewScriptural));
            Assert.AreEqual(expectedWeekYearOfFirstDay, rule.GetWeekYear(scripturalDate));
            Assert.AreEqual(expectedWeekOfWeekYearOfFirstDay, rule.GetWeekOfWeekYear(scripturalDate));
            Assert.AreEqual(scripturalDate,
                rule.GetLocalDate(expectedWeekYearOfFirstDay, expectedWeekOfWeekYearOfFirstDay, expectedFirstDay, HebrewScriptural));
        }
    }
}
