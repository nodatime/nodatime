// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;
using System.Globalization;
using static System.Globalization.CalendarWeekRule;
using static System.DayOfWeek;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    public class BclWeekYearRuleTest
    {
        /// <summary>
        /// For each calendar and rule combination, check everything we can about every date
        /// from mid December to mid January around each year between 2016 and 2046.
        /// (For non-Gregorian calendars, the rough equivalent is used...)
        /// That should give us plenty of coverage.
        /// </summary>
        [Test]
        [Combinatorial]
        public void BclEquivalence(
            [ValueSource(typeof(BclCalendars), nameof(BclCalendars.MappedCalendars))] Calendar calendar,
            [Values(FirstDay, FirstFourDayWeek, FirstFullWeek)] CalendarWeekRule bclRule,
            [Values(Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)]  DayOfWeek firstDayOfWeek)
        {
            var nodaCalendar = BclCalendars.CalendarSystemForCalendar(calendar);
            var nodaRule = BclWeekYearRule.FromCalendarWeekRule(bclRule, firstDayOfWeek);
            var startYear = new LocalDate(2016, 1, 1).WithCalendar(nodaCalendar).Year;

            for (int year = startYear; year < startYear + 30; year++)
            {
                var startDate = new LocalDate(year, 1, 1, nodaCalendar).PlusDays(-15);
                for (int day = 0; day < 30; day++)
                {
                    var date = startDate.PlusDays(day);
                    var bclDate = date.ToDateTimeUnspecified();
                    var bclWeek = calendar.GetWeekOfYear(bclDate, bclRule, firstDayOfWeek);
                    // Weird... the BCL doesn't have a way of finding out which week-year we're in.
                    // We're starting at "start of year - 15 days", so a "small" week-of-year
                    // value means we're in "year", whereas a "large" week-of-year value means
                    // we're in the "year-1".
                    var bclWeekYear = bclWeek < 10 ? year : year - 1;

                    Assert.AreEqual(bclWeek, nodaRule.GetWeekOfWeekYear(date), "Date: {0}", date);
                    Assert.AreEqual(bclWeekYear, nodaRule.GetWeekYear(date), "Date: {0}", date);
                    Assert.AreEqual(date, nodaRule.GetLocalDate(bclWeekYear, bclWeek, date.IsoDayOfWeek, nodaCalendar),
                        "Week-year:{0}; Week: {1}; Day: {2}", bclWeekYear, bclWeek, date.IsoDayOfWeek);
                }
            }
        }

        /// <summary>
        /// The number of weeks in the year is equal to the week-of-week-year for the last
        /// day of the year.
        /// </summary>
        [Test]
        [Combinatorial]
        public void GetWeeksInWeekYear(
            [ValueSource(typeof(BclCalendars), nameof(BclCalendars.MappedCalendars))] Calendar calendar,
            [Values(FirstDay, FirstFourDayWeek, FirstFullWeek)] CalendarWeekRule bclRule,
            [Values(Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday)]  DayOfWeek firstDayOfWeek)
        {
            var nodaCalendar = BclCalendars.CalendarSystemForCalendar(calendar);
            var nodaRule = BclWeekYearRule.FromCalendarWeekRule(bclRule, firstDayOfWeek);
            var startYear = new LocalDate(2016, 1, 1).WithCalendar(nodaCalendar).Year;

            for (int year = startYear; year < startYear + 30; year++)
            {
                var bclDate = new LocalDate(year + 1, 1, 1, nodaCalendar).PlusDays(-1).ToDateTimeUnspecified();
                Assert.AreEqual(calendar.GetWeekOfYear(bclDate, bclRule, firstDayOfWeek),
                    nodaRule.GetWeeksInWeekYear(year, nodaCalendar), "Year {0}", year);
            }
        }

        // Tests where we ask for an invalid combination of week-year/week/day-of-week due to a week being "short"
        // in BCL rules.
        // Jan 1st 2016 = Friday
        [Test]
        [TestCase(FirstDay, Monday, 2015, 53, IsoDayOfWeek.Saturday)]
        [TestCase(FirstDay, Monday, 2016, 1, IsoDayOfWeek.Thursday)]
        public void GetLocalDate_Invalid(
            CalendarWeekRule bclRule, DayOfWeek firstDayOfWeek,
            int weekYear, int week, IsoDayOfWeek dayOfWeek)
        {
            var nodaRule = BclWeekYearRule.FromCalendarWeekRule(bclRule, firstDayOfWeek);
            Assert.Throws<ArgumentOutOfRangeException>(() => nodaRule.GetLocalDate(weekYear, week, dayOfWeek));
        }
    }
}
