// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class UmAlQuraYearMonthDayCalculatorTest
    {
        private static readonly Calendar BclCalendar = GetBclCalendar();

        private static readonly UmAlQuraYearMonthDayCalculator Calculator = UmAlQuraYearMonthDayCalculator.IsSupported ? new UmAlQuraYearMonthDayCalculator() : null;

        // Horrible way to conditionalize tests at execution time...
        private static readonly IEnumerable<string> Supported =
            UmAlQuraYearMonthDayCalculator.IsSupported ? new[] { "(Supported)" } : new string[0];

        private static Calendar GetBclCalendar()
        {
            // Always get it with reflection in the test, just for simplicity.
            try
            {
                var type = typeof(Calendar).Assembly.GetType("System.Globalization.UmAlQuraCalendar");
                if (type == null)
                {
                    return null;
                }
                return (Calendar) Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        [Test]
        public void CannotInstantiateOnUnsupportedPlatform()
        {
            if (!UmAlQuraYearMonthDayCalculator.IsSupported)
            {
                Assert.Throws<NotSupportedException>(() => new UmAlQuraYearMonthDayCalculator());
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetDaysInMonth(string ignored)
        {
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    Assert.AreEqual(BclCalendar.GetDaysInMonth(year, month), Calculator.GetDaysInMonth(year, month), "year={0}; month={1}", year, month);
                }
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetDaysInYear(string ignored)
        {
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                Assert.AreEqual(BclCalendar.GetDaysInYear(year), Calculator.GetDaysInYear(year), "year={0}", year);
            }
        }

        [Test, TestCaseSource("Supported")]
        public void IsLeapYear(string ignored)
        {
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                Assert.AreEqual(BclCalendar.IsLeapYear(year), Calculator.IsLeapYear(year), "year={0}", year);
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetStartOfYearInDays(string ignored)
        {
            // This exercises CalculateStartOfYearInDays too.
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                var bcl = new DateTime(year, 1, 1, BclCalendar);
                var days = (bcl - new DateTime(1970, 1, 1)).Days;
                Assert.AreEqual(days, Calculator.GetStartOfYearInDays(year), "year={0}", year);
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetYearMonthDay_DaysSinceEpoch(string ignored)
        {
            int daysSinceEpoch = Calculator.GetStartOfYearInDays(Calculator.MinYear);
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= Calculator.GetDaysInMonth(year, month); day++)
                    {
                        var actual = Calculator.GetYearMonthDay(daysSinceEpoch);
                        var expected = new YearMonthDay(year, month, day);
                        Assert.AreEqual(expected, actual, "daysSinceEpoch={0}", daysSinceEpoch);
                        daysSinceEpoch++;
                    }
                }
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetYearMonthDay_YearAndDayOfYear(string ignored)
        {
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                int dayOfYear = 1;
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= Calculator.GetDaysInMonth(year, month); day++)
                    {
                        var actual = Calculator.GetYearMonthDay(year, dayOfYear);
                        var expected = new YearMonthDay(year, month, day);
                        Assert.AreEqual(expected, actual, "year={0}; dayOfYear={1}", year, dayOfYear);
                        dayOfYear++;
                    }
                }
            }
        }

        [Test, TestCaseSource("Supported")]
        public void GetDaysFromStartOfYearToStartOfMonth(string ignored)
        {
            for (int year = Calculator.MinYear; year <= Calculator.MaxYear; year++)
            {
                int dayOfYear = 1;
                for (int month = 1; month <= 12; month++)
                {
                    // This delegates to GetDaysFromStartOfYearToStartOfMonth (which is protected).
                    Assert.AreEqual(dayOfYear, Calculator.GetDayOfYear(new YearMonthDay(year, month, 1)), "year={0}; month={1}", year, month);
                    dayOfYear += Calculator.GetDaysInMonth(year, month);
                }
            }
        }
    }
}
