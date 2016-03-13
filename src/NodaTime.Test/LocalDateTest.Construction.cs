// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        [TestCase(1620)] // Leap year in non-optimized period
        [TestCase(1621)] // Non-leap year in non-optimized period
        [TestCase(1980)] // Leap year in optimized period
        [TestCase(1981)] // Non-leap year in optimized period
        public void Constructor_WithDays(int year)
        {
            LocalDate start = new LocalDate(year, 1, 1);
            int startDays = start.DaysSinceEpoch;
            for (int i = 0; i < 366; i++)
            {
                Assert.AreEqual(start.PlusDays(i), new LocalDate(startDays + i));
            }
        }

        [Test]
        [TestCase(1620)] // Leap year in non-optimized period
        [TestCase(1621)] // Non-leap year in non-optimized period
        [TestCase(1980)] // Leap year in optimized period
        [TestCase(1981)] // Non-leap year in optimized period
        public void Constructor_WithDaysAndCalendar(int year)
        {
            LocalDate start = new LocalDate(year, 1, 1);
            int startDays = start.DaysSinceEpoch;
            for (int i = 0; i < 366; i++)
            {
                Assert.AreEqual(start.PlusDays(i), new LocalDate(startDays + i, CalendarSystem.Iso));
            }
        }

        [Test]
        public void Constructor_CalendarDefaultsToIso()
        {
            LocalDate date = new LocalDate(2000, 1, 1);
            Assert.AreEqual(CalendarSystem.Iso, date.Calendar);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip()
        {
            LocalDate date = new LocalDate(2023, 7, 27);
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.Month);
            Assert.AreEqual(27, date.Day);
        }

        [Test]
        public void Constructor_PropertiesRoundTrip_CustomCalendar()
        {
            LocalDate date = new LocalDate(2023, 7, 27, CalendarSystem.Julian);
            Assert.AreEqual(2023, date.Year);
            Assert.AreEqual(7, date.Month);
            Assert.AreEqual(27, date.Day);
        }

        [Test]
        [TestCase(GregorianYearMonthDayCalculator.MaxGregorianYear + 1, 1, 1)]
        [TestCase(2010, 13, 1), TestCase(2010, 1, 100), TestCase(2010, 2, 30)]
        public void Constructor_Invalid(int year, int month, int day)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(year, month, day));
        }

        [Test]
        public void Constructor_InvalidYearOfEra()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.Common, 0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.BeforeCommon, 0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.Common, 10000, 1, 1));
            // Although our minimum year is -9998, that's 9999 BC.
            Assert.Throws<ArgumentOutOfRangeException>(() => new LocalDate(Era.BeforeCommon, 10000, 1, 1));
        }

        [Test]
        public void Constructor_WithYearOfEra_BC()
        {
            LocalDate absolute = new LocalDate(-10, 1, 1);
            LocalDate withEra = new LocalDate(Era.BeforeCommon, 11, 1, 1);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_AD()
        {
            LocalDate absolute = new LocalDate(50, 6, 19);
            LocalDate withEra = new LocalDate(Era.Common, 50, 6, 19);
            Assert.AreEqual(absolute, withEra);
        }

        [Test]
        public void Constructor_WithYearOfEra_NonIsoCalendar()
        {
            var calendar = CalendarSystem.Coptic;
            LocalDate absolute = new LocalDate(50, 6, 19, calendar);
            LocalDate withEra = new LocalDate(Era.AnnoMartyrum, 50, 6, 19, calendar);
            Assert.AreEqual(absolute, withEra);
        }

        // Most tests are in IsoBasedWeekYearRuleTest.
        [Test]
        public void FromWeekYearWeekAndDay_InvalidWeek53()
        {
            // Week year 2005 only has 52 weeks
            Assert.Throws<ArgumentOutOfRangeException>(() => LocalDate.FromWeekYearWeekAndDay(2005, 53, IsoDayOfWeek.Sunday));
        }

        [Test]
        [TestCase(2014, 8, 3, IsoDayOfWeek.Sunday, 17)]
        [TestCase(2014, 8, 3, IsoDayOfWeek.Friday, 15)]
        // Needs "rewind" logic as August 1st 2014 is a Friday
        [TestCase(2014, 8, 3, IsoDayOfWeek.Thursday, 21)]
        [TestCase(2014, 8, 5, IsoDayOfWeek.Sunday, 31)]
        // Only 4 Mondays in August in 2014.
        [TestCase(2014, 8, 5, IsoDayOfWeek.Monday, 25)]
        public void FromYearMonthWeekAndDay(int year, int month, int occurrence, IsoDayOfWeek dayOfWeek, int expectedDay)
        {
            var date = LocalDate.FromYearMonthWeekAndDay(year, month, occurrence, dayOfWeek);
            Assert.AreEqual(year, date.Year);
            Assert.AreEqual(month, date.Month);
            Assert.AreEqual(expectedDay, date.Day);
        }
    }
}
