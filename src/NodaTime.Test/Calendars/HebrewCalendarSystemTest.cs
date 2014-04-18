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
        /// This tests every day for the BCL-supported Hebrew calendar range, testing various aspects of each date,
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
        /// This tests every day for the BCL-supported Hebrew calendar range, testing various aspects of each date,
        /// using the ecclesiastical month numbering.
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

        // 5501 is not a leap year; 5502 is; 5503 is not; 5505 is.
        // Heshvan (ecclesiastical 8) is long in 5507 and 5509; it is short in 5506 and 5508
        // Kislev (ecclesiastical 9) is long in 5503-5505; it is short in 5502 and 5506
        // Test cases are in ecclesiastical month numbering, but we check both. This is
        // mostly testing the behaviour of SetYear, via LocalDate.PlusYears.
        [Test]
        [TestCase("5505-02-10", 1, "5506-02-10")] // No truncation required
        [TestCase("5502-13-05", 1, "5503-12-29")] // Truncation from Adar II to end of Adar
        [TestCase("5502-13-05", -1, "5501-12-29")] // And again, but in reverse (subtracting a year)
        [TestCase("5502-12-05", 1, "5503-12-05")] // In Adar, but no truncation required
        [TestCase("5502-13-05", 3, "5505-13-05")] // In Adar II, but no truncation required as we transferring to a leap year
        [TestCase("5507-08-30", 1, "5508-08-29")] // Truncation in Heshvan
        [TestCase("5507-08-30", 2, "5509-08-30")] // No truncation in Heshvan (both 5507 and 5509 are long)
        [TestCase("5503-09-30", 1, "5504-09-30")] // No truncation in Kislev (both 5503 and 5504 are long)
        [TestCase("5503-09-30", 3, "5506-09-29")] // Truncation in Kislev
        public void SetYear(string startText, int years, string expectedEndText)
        {
            var civil = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);
            var ecclesiastical = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Ecclesiastical);
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithTemplateValue(new LocalDate(5774, 1, 1, ecclesiastical)); // Sample value in 2014 ISO

            var start = pattern.Parse(startText).Value;
            var expectedEnd = pattern.Parse(expectedEndText).Value;
            Assert.AreEqual(expectedEnd, start.PlusYears(years));

            // Check civil as well... the date should be the same (year, month, day) even though
            // the numbering is different.
            Assert.AreEqual(expectedEnd.WithCalendar(civil), start.WithCalendar(civil).PlusYears(years));
        }

        [Test]
        // 5501 is not a leap year; 5502 is; 5503 is not; 5505 is.
        // Heshvan (civil 2) is long in 5507 and 5509; it is short in 5506 and 5508
        // Kislev (civil 3) is long in 5503-5505; it is short in 5502 and 5506
        // Test cases are in civil month numbering (for the sake of sanity!) - the
        // implementation performs converts to civil for most of the work.
        [TestCase("5502-02-13", 3, "5502-05-13")] // Simple
        [TestCase("5502-02-13", 238, "5521-05-13")] // Simple after a 19-year cycle
        [TestCase("5502-05-13", -3, "5502-02-13")] // Simple (negative)
        [TestCase("5521-05-13", -238, "5502-02-13")] // Simple after a 19-year cycle (negative)
        [TestCase("5501-02-13", 12, "5502-02-13")] // Not a leap year
        [TestCase("5502-02-13", 13, "5503-02-13")] // Leap year
        [TestCase("5501-02-13", 26, "5503-03-13")] // Traversing both (and then an extra month)
        [TestCase("5502-02-13", -12, "5501-02-13")] // Not a leap year (negative)
        [TestCase("5503-02-13", -13, "5502-02-13")] // Leap year (negative)
        [TestCase("5503-03-13", -26, "5501-02-13")] // Traversing both (and then an extra month) (negative)
        [TestCase("5507-01-30", 1, "5507-02-30")] // Long Heshvan
        [TestCase("5506-01-30", 1, "5506-02-29")] // Short Heshvan
        [TestCase("5505-01-30", 2, "5505-03-30")] // Long Kislev
        [TestCase("5506-01-30", 2, "5506-03-29")] // Short Kislev
        public void AddMonths(string startText, int months, string expectedEndText)
        {
            var civil = CalendarSystem.GetHebrewCalendar(HebrewMonthNumbering.Civil);
            var pattern = LocalDatePattern.CreateWithInvariantCulture("yyyy-MM-dd")
                .WithTemplateValue(new LocalDate(5774, 1, 1, civil)); // Sample value in 2014 ISO

            var start = pattern.Parse(startText).Value;
            var expectedEnd = pattern.Parse(expectedEndText).Value;
            Assert.AreEqual(expectedEnd, start.PlusMonths(months));
        }
    }
}
