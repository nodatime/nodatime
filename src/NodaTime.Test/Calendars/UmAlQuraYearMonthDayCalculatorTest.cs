// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Calendars;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test.Calendars
{
    public class UmAlQuraYearMonthDayCalculatorTest
    {
        [Test]
        public void BclEquivalence()
        {
            BclEquivalenceHelper.AssertEquivalent(BclCalendars.UmAlQura, CalendarSystem.UmAlQura);
        }

        [Test]
        public void GetStartOfYearInDays()
        {
            // This exercises CalculateStartOfYearInDays too.
            var calculator = new UmAlQuraYearMonthDayCalculator();
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                var bcl = BclCalendars.UmAlQura.ToDateTime(year, 1, 1, 0, 0, 0, 0);
                var days = (bcl - new DateTime(1970, 1, 1)).Days;
                Assert.AreEqual(days, calculator.GetStartOfYearInDays(year), "year={0}", year);
            }
        }

        [Test]
        public void GetYearMonthDay_DaysSinceEpoch()
        {
            var calculator = new UmAlQuraYearMonthDayCalculator();
            int daysSinceEpoch = calculator.GetStartOfYearInDays(calculator.MinYear);
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= calculator.GetDaysInMonth(year, month); day++)
                    {
                        var actual = calculator.GetYearMonthDay(daysSinceEpoch);
                        var expected = new YearMonthDay(year, month, day);
                        Assert.AreEqual(expected, actual, "daysSinceEpoch={0}", daysSinceEpoch);
                        daysSinceEpoch++;
                    }
                }
            }
        }

        [Test]
        public void GetYearMonthDay_YearAndDayOfYear()
        {
            var calculator = new UmAlQuraYearMonthDayCalculator();
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                int dayOfYear = 1;
                for (int month = 1; month <= 12; month++)
                {
                    for (int day = 1; day <= calculator.GetDaysInMonth(year, month); day++)
                    {
                        var actual = calculator.GetYearMonthDay(year, dayOfYear);
                        var expected = new YearMonthDay(year, month, day);
                        Assert.AreEqual(expected, actual, "year={0}; dayOfYear={1}", year, dayOfYear);
                        dayOfYear++;
                    }
                }
            }
        }

        [Test]
        public void GetDaysFromStartOfYearToStartOfMonth()
        {
            var calculator = new UmAlQuraYearMonthDayCalculator();
            for (int year = calculator.MinYear; year <= calculator.MaxYear; year++)
            {
                int dayOfYear = 1;
                for (int month = 1; month <= 12; month++)
                {
                    // This delegates to GetDaysFromStartOfYearToStartOfMonth (which is protected).
                    Assert.AreEqual(dayOfYear, calculator.GetDayOfYear(new YearMonthDay(year, month, 1)), "year={0}; month={1}", year, month);
                    dayOfYear += calculator.GetDaysInMonth(year, month);
                }
            }
        }


        [Test]
        public void GetYearMonthDay_InvalidValueForCoverage()
        {
            var calculator = new UmAlQuraYearMonthDayCalculator();
            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.GetYearMonthDay(calculator.MinYear, 1000));
        }

#if DEBUG && !NETCORE
        [Test, Explicit]
        public void GenerateData()
        {
            var bclCalendar = new UmAlQuraCalendar();
            DateTime minDateTime = bclCalendar.MinSupportedDateTime;            

            // Work out the min and max supported years, ensuring that we support complete years.
            var minYear = bclCalendar.GetYear(minDateTime);
            if (bclCalendar.GetMonth(minDateTime) != 1 || bclCalendar.GetDayOfMonth(minDateTime) != 1)
            {
                minYear++;
            }

            DateTime maxDateTime = bclCalendar.MaxSupportedDateTime;
            var maxYear = bclCalendar.GetYear(maxDateTime);
            if (bclCalendar.GetMonth(maxDateTime) != 12 || bclCalendar.GetDayOfMonth(maxDateTime) != bclCalendar.GetDaysInMonth(maxYear, 12))
            {
                maxYear--;
            }

            // This is two elements longer than it needs to be, but it's simpler this way.
            var monthLengths = new ushort[maxYear - minYear + 3];
            for (int year = minYear; year <= maxYear; year++)
            {
                int yearIndex = year - minYear + 1;
                ushort monthBits = 0;
                for (int month = 1; month <= 12; month++)
                {
                    if (bclCalendar.GetDaysInMonth(year, month) == 30)
                    {
                        monthBits |= (ushort) (1 << month);
                    }
                }
                monthLengths[yearIndex] = monthBits;
            }
            byte[] data = monthLengths.SelectMany(value => new[] { (byte)(value >> 8), (byte)(value & 0xff) }).ToArray();

            // Assume every 10 years before minDateTime has exactly 3544 days... it doesn't matter whether or not that's
            // correct, but it gets roughly the right estimate. It doesn't matter that startOfMinYear isn't in UTC; we're only
            // taking the Ticks property, which doesn't take account of the Kind.
            DateTime startOfMinYear = bclCalendar.ToDateTime(minYear, 1, 1, 0, 0, 0, 0);
            var computedDaysAtStartOfMinYear = (int)((startOfMinYear.Ticks - NodaConstants.BclTicksAtUnixEpoch) / NodaConstants.TicksPerDay);

            Console.WriteLine($"private const int ComputedMinYear = {minYear};");
            Console.WriteLine($"private const int ComputedMaxYear = {maxYear};");
            Console.WriteLine($"private const int ComputedDaysAtStartOfMinYear = {computedDaysAtStartOfMinYear};");
            Console.WriteLine("private const string GeneratedData =");

            // Adapted from PersianCalendarSystemTest. If we do this any more, we should
            // put it somewhere common.
            var base64 = Convert.ToBase64String(data);
            var lineLength = 80;
            for (int start = 0; start < base64.Length; start += lineLength)
            {
                var line = base64.Substring(start, Math.Min(lineLength, base64.Length - start));
                var last = start + lineLength >= base64.Length;
                Console.WriteLine($"    \"{line}\"{(last ? ";" : " +")}");
            }
            Console.WriteLine();
        }
#endif
    }
}
