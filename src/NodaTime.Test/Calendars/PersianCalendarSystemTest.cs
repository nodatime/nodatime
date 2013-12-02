// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for PersianYearMonthDayCalculator via the Persian CalendarSystem.
    /// </summary>
    [TestFixture]
    public class PersianCalendarSystemTest
    {
        [Test]
        public void IsLeapYear()
        {
            var bcl = new PersianCalendar();
            // TODO: Use CalendarSystem.Persian
            var noda = new PersianYearMonthDayCalendar();

            for (int year = 1; year < 9379; year++)
            {
                Assert.AreEqual(bcl.IsLeapYear(year), noda.IsLeapYear(year));
            }
        }

        /// <summary>
        /// This tests every day for 9000 (ISO) years, testing various aspects of each date.
        /// </summary>
        [Test]
        public void BclThroughHistory()
        {
            Calendar bcl = new PersianCalendar();
            CalendarSystem noda = CalendarSystem.GetPersianCalendar();

            for (int year = 1; year < 9378; year++)
            {
                for (int month = 1; month < 13; month++)
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
    }
}
