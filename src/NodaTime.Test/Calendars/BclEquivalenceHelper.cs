// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Helper methods for testing calendar systems that should be equivalent to BCL calendars.
    /// </summary>
    internal static class BclEquivalenceHelper
    {
        /// <summary>
        /// Checks that each day from the given start year to the end year (inclusive) is equal
        /// between the BCL and the Noda Time calendar. Additionally, the number of days in each month and year
        /// and the number of months (and leap year status) in each year is checked.
        /// </summary>
        internal static void AssertEquivalent(Calendar bcl, CalendarSystem noda, int fromYear, int toYear)
        {
            // Note: Noda Time stops in 9377, whereas the BCL goes into the start of 9378. This is because
            // Noda Time ensures that the whole year is valid.

            // We avoid asking the BCL to create a DateTime on each iteration, simply
            // because the BCL implementation is so slow. Instead, we just check at the start of each month that
            // we're at the date we expect.
            DateTime bclDate = new DateTime(1, 1, 1, bcl);
            for (int year = fromYear; year <= toYear; year++)
            {
                Assert.AreEqual(bcl.GetDaysInYear(year), noda.GetDaysInYear(year), "Year: {0}", year);
                Assert.AreEqual(bcl.GetMonthsInYear(year), noda.GetMonthsInYear(year), "Year: {0}", year);
                for (int month = 1; month <= noda.GetMonthsInYear(year); month++)
                {
                    // Sanity check at the start of each month.
                    Assert.AreEqual(year, bcl.GetYear(bclDate));
                    Assert.AreEqual(month, bcl.GetMonth(bclDate));
                    Assert.AreEqual(1, bcl.GetDayOfMonth(bclDate));

                    Assert.AreEqual(bcl.GetDaysInMonth(year, month), noda.GetDaysInMonth(year, month),
                        "Year: {0}; Month: {1}", year, month);
                    Assert.AreEqual(bcl.IsLeapYear(year), noda.IsLeapYear(year), "Year: {0}", year);
                    for (int day = 1; day <= noda.GetDaysInMonth(year, month); day++)
                    {
                        LocalDate nodaDate = new LocalDate(year, month, day, noda);
                        Assert.AreEqual(bclDate, nodaDate.AtMidnight().ToDateTimeUnspecified(),
                            "Original calendar system date: {0:yyyy-MM-dd}", nodaDate);
                        Assert.AreEqual(nodaDate, LocalDateTime.FromDateTime(bclDate).WithCalendar(noda).Date);
                        Assert.AreEqual(year, nodaDate.Year);
                        Assert.AreEqual(month, nodaDate.Month);
                        Assert.AreEqual(day, nodaDate.Day);
                        bclDate = bclDate.AddDays(1);
                    }
                }
            }

        }
    }
}
