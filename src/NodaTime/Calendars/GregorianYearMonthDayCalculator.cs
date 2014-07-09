// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    internal class GregorianYearMonthDayCalculator : GJYearMonthDayCalculator
    {
        internal const int MinGregorianYear = -27255;
        internal const int MaxGregorianYear = 31195;

        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;
        // The 0-based days-since-unix-epoch for the start of each month
        private static readonly int[] MonthStartDays = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        // The 1-based days-since-unix-epoch for the start of each year
        private static readonly int[] YearStartDays = new int[LastOptimizedYear + 1 - FirstOptimizedYear];

        private const int DaysFrom0000To1970 = 719527;
        private const int AverageDaysPer10Years = 3652; // Ideally 365.2425 per year...

        static GregorianYearMonthDayCalculator()
        {
            // It's generally a really bad idea to create an instance before the static initializer
            // has completed, but we know its safe because we're only using a very restricted set of methods.
            var instance = new GregorianYearMonthDayCalculator();
            for (int year = FirstOptimizedYear; year <= LastOptimizedYear; year++)
            {
                int yearStart = instance.CalculateStartOfYearDays(year);
                YearStartDays[year - FirstOptimizedYear] = yearStart;
                int monthStartDay = yearStart - 1; // See field description
                int yearMonthIndex = (year - FirstOptimizedYear) * 12;
                for (int month = 1; month <= 12; month++)
                {
                    yearMonthIndex++;
                    int monthLength = instance.GetDaysInMonth(year, month);
                    MonthStartDays[yearMonthIndex] = monthStartDay;
                    monthStartDay += monthLength;
                }
            }
        }

        internal GregorianYearMonthDayCalculator()
            : base(MinGregorianYear, MaxGregorianYear, AverageDaysPer10Years, -719162)
        {
        }

        internal override int GetStartOfYearInDays(int year)
        {
            // 2014-06-28: Tried removing this entirely (optimized: 5ns => 8ns; unoptimized: 11ns => 8ns)
            // Decided to leave it in, as the optimized case is so much more common.
            if (year < FirstOptimizedYear || year > LastOptimizedYear)
            {
                return base.GetStartOfYearInDays(year);
            }
            return YearStartDays[year - FirstOptimizedYear];
        }

        internal override int GetDaysSinceEpoch(YearMonthDay yearMonthDay)
        {
            // 2014-06-28: Tried removing this entirely (optimized: 8ns => 13ns; unoptimized: 23ns => 19ns)
            // Also tried computing everything lazily - it's a wash.
            // Removed validation, however - we assume that the parameter is already valid by now.
            unchecked
            {
                int year = yearMonthDay.Year;
                int monthOfYear = yearMonthDay.Month;
                int dayOfMonth = yearMonthDay.Day;
                int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
                if (year < FirstOptimizedYear || year > LastOptimizedYear - 1)
                {
                    return base.GetDaysSinceEpoch(yearMonthDay);
                }
                return MonthStartDays[yearMonthIndex] + dayOfMonth;
            }
        }

        internal override void ValidateYearMonthDay(int year, int month, int day)
        {
            ValidateGregorianYearMonthDay(year, month, day);
        }

        internal static void ValidateGregorianYearMonthDay(int year, int month, int day)
        {
            // Perform quick validation without calling Preconditions, then do it properly if we're going to throw
            // an exception. Avoiding the method call is pretty extreme, but it does help.
            if (year < MinGregorianYear || year > MaxGregorianYear || month < 1 || month > 12)
            {
                Preconditions.CheckArgumentRange("year", year, MinGregorianYear, MaxGregorianYear);
                Preconditions.CheckArgumentRange("month", month, 1, 12);
            }
            // If we've been asked for day 1-28, we're definitely okay regardless of month.
            if (day >= 1 && day <= 28)
            {
                return;
            }
            int daysInMonth = month == 2 && IsGregorianLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];
            if (day > daysInMonth)
            {
                Preconditions.CheckArgumentRange("day", day, 1, daysInMonth);
            }
        }

        protected override int CalculateStartOfYearDays(int year)
        {
            // Initial value is just temporary.
            int leapYears = year / 100;
            if (year < 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers. When the expression is written as
                // (year / 4) - (year / 100) + (year / 400),
                // it works for both positive and negative values, except this optimization
                // eliminates two divisions.
                leapYears = ((year + 3) >> 2) - leapYears + ((leapYears + 3) >> 2) - 1;
            }
            else
            {
                leapYears = (year >> 2) - leapYears + (leapYears >> 2);
                if (IsLeapYear(year))
                {
                    leapYears--;
                }
            }

            return year * 365 + (leapYears - DaysFrom0000To1970);
        }

        internal override bool IsLeapYear(int year)
        {
            return IsGregorianLeapYear(year);
        }

        private static bool IsGregorianLeapYear(int year)
        {
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0); ;
        }
    }
}
