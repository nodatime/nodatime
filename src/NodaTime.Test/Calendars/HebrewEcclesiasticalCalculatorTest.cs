// Copyright 2014 The Noda Time Authors. All rights reserved.
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
    [TestFixture]
    public class HebrewScripturalCalculatorTest
    {
        [Test]
        public void DaysInYear()
        {
            var bcl = new HebrewCalendar();
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime);
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime);

            for (int year = minYear; year <= maxYear; year++)
            {
                Assert.AreEqual(bcl.GetDaysInYear(year), HebrewScripturalCalculator.DaysInYear(year));
            }
        }

        [Test]
        public void DaysInMonth()
        {
            var bcl = new HebrewCalendar();
            // Not all months in the min/max years are supported
            var minYear = bcl.GetYear(bcl.MinSupportedDateTime) + 1;
            var maxYear = bcl.GetYear(bcl.MaxSupportedDateTime) - 1;
            
            for (int year = minYear; year <= maxYear; year++)
            {
                int months = bcl.GetMonthsInYear(year);
                for (int month = 1; month <= months; month++)
                {
                    int scripturalMonth = HebrewMonthConverter.CivilToScriptural(year, month);
                    int bclDays = bcl.GetDaysInMonth(year, month);
                    int nodaDays = HebrewScripturalCalculator.DaysInMonth(year, scripturalMonth);
                    Assert.AreEqual(bclDays, nodaDays);
                }
            }
        }
    }
}
