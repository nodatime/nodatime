// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Globalization;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for HebrewYearMonthDayCalculator via the Hebrew CalendarSystem.
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
            var noda = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);

            for (int year = minYear; year <= maxYear; year++)
            {
                Assert.AreEqual(bcl.IsLeapYear(year), noda.IsLeapYear(year));
            }
        }

        /// <summary>
        /// This tests every day for 9000 (ISO) years, testing various aspects of each date,
        /// using the civil month numbering.
        /// </summary>
        [Test, Timeout(300000)] // Can take a long time under NCrunch.
        public void BclThroughHistory_Civil()
        {
            Calendar bcl = new HebrewCalendar();
            var noda = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);

            // The min supported date/time starts part way through the year
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime) + 1;
            // The max supported date/time ends part way through the year
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime) - 1;

            for (int year = minYear; year <= maxYear; year++)
            {
                int months = bcl.GetMonthsInYear(year);
                Assert.AreEqual(months, noda.GetMaxMonth(year));
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
        /// This tests every day for 9000 (ISO) years, testing various aspects of each date,
        /// using the civil month numbering.
        /// </summary>
        [Test, Timeout(300000)] // Can take a long time under NCrunch.
        public void BclThroughHistory_Ecclesiastical()
        {
            Calendar bcl = new HebrewCalendar();
            var noda = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Ecclesiastical);

            // The min supported date/time starts part way through the year
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime) + 1;
            // The max supported date/time ends part way through the year
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime) - 1;

            for (int year = minYear; year <= maxYear; year++)
            {
                int months = bcl.GetMonthsInYear(year);
                Assert.AreEqual(months, noda.GetMaxMonth(year));
                for (int civilMonth = 1; civilMonth <= months; civilMonth++)
                {
                    int ecclesiasticalMonth = HebrewMonthConverter.CivilToEcclesiastical(year, civilMonth);
                    Assert.AreEqual(bcl.GetDaysInMonth(year, civilMonth), noda.GetDaysInMonth(year, ecclesiasticalMonth),
                        "Year: {0}; Month: {1} (civil)", year, civilMonth);
                    for (int day = 1; day < bcl.GetDaysInMonth(year, civilMonth); day++)
                    {
                        DateTime bclDate = new DateTime(year, civilMonth, day, bcl);
                        LocalDate nodaDate = new LocalDate(year, ecclesiasticalMonth, day, noda);
                        Assert.AreEqual(bclDate, nodaDate.AtMidnight().ToDateTimeUnspecified());
                        Assert.AreEqual(nodaDate, LocalDateTime.FromDateTime(bclDate).WithCalendar(noda).Date);
                        Assert.AreEqual(year, nodaDate.Year);
                        Assert.AreEqual(ecclesiasticalMonth, nodaDate.Month);
                        Assert.AreEqual(day, nodaDate.Day);
                    }
                }
            }
        }
    }
}
