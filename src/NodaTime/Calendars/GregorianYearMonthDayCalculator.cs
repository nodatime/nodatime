// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    internal sealed class GregorianYearMonthDayCalculator : GJYearMonthDayCalculator
    {
        internal const int MinGregorianYear = -9998;
        internal const int MaxGregorianYear = 9999;

        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;
        private const int FirstOptimizedDay = -25567;
        private const int LastOptimizedDay = 47846;
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

        /// <summary>
        /// Specifically Gregorian-optimized conversion from "days since epoch" to year/month/day.
        /// </summary>
        internal static YearMonthDayCalendar GetGregorianYearMonthDayCalendarFromDaysSinceEpoch(int daysSinceEpoch)
        {
            unchecked
            {
                if (daysSinceEpoch < FirstOptimizedDay || daysSinceEpoch > LastOptimizedDay)
                {
                    return CalendarSystem.Iso.GetYearMonthDayFromDaysSinceEpoch(daysSinceEpoch).WithCalendarOrdinal(CalendarOrdinal.Iso);
                }
                // Divide by more than we need to, in order to guarantee that we only need to move forward.
                // We can still only be out by 1 year.
                int yearIndex = (daysSinceEpoch - FirstOptimizedDay) / 366;
                int indexValue = YearStartDays[yearIndex];
                // Zero-based day of year
                int d = daysSinceEpoch - indexValue;
                int year = yearIndex + FirstOptimizedYear;
                bool isLeap = IsGregorianLeapYear(year);
                int daysInYear = isLeap ? 366 : 365;
                if (d >= daysInYear)
                {
                    year++;
                    d -= daysInYear;
                    isLeap = IsGregorianLeapYear(year);
                }

                // The remaining code is copied from GJYearMonthDayCalculator (and tweaked)

                int startOfMonth;
                // Perform a hard-coded binary search to get the month.
                if (isLeap)
                {
                    startOfMonth = ((d < 182)
                                  ? ((d < 91) ? ((d < 31) ? -1 : (d < 60) ? 30 : 59) : ((d < 121) ? 90 : (d < 152) ? 120 : 151))
                                  : ((d < 274)
                                         ? ((d < 213) ? 181 : (d < 244) ? 212 : 243)
                                         : ((d < 305) ? 273 : (d < 335) ? 304 : 334)));
                }
                else
                {
                    startOfMonth = ((d < 181)
                                  ? ((d < 90) ? ((d < 31) ? -1 : (d < 59) ? 30 : 58) : ((d < 120) ? 89 : (d < 151) ? 119 : 150))
                                  : ((d < 273)
                                         ? ((d < 212) ? 180 : (d < 243) ? 211 : 242)
                                         : ((d < 304) ? 272 : (d < 334) ? 303 : 333)));
                }
                int month = startOfMonth / 29 + 1;
                int dayOfMonth = d - startOfMonth;
                // TODO(2.0): Consider an overload which doesn't take the ordinal. That would save a single bitwise OR, and an argument. Doubt that it's worth it...
                return new YearMonthDayCalendar(year, month, dayOfMonth, CalendarOrdinal.Iso);
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

        internal override int GetDaysSinceEpoch([Trusted] YearMonthDay yearMonthDay)
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

        internal override void ValidateYearMonthDay(int year, int month, int day) => ValidateGregorianYearMonthDay(year, month, day);

        internal static void ValidateGregorianYearMonthDay(int year, int month, int day)
        {
            // Perform quick validation without calling Preconditions, then do it properly if we're going to throw
            // an exception. Avoiding the method call is pretty extreme, but it does help.
            if (year < MinGregorianYear || year > MaxGregorianYear || month < 1 || month > 12)
            {
                Preconditions.CheckArgumentRange(nameof(year), year, MinGregorianYear, MaxGregorianYear);
                Preconditions.CheckArgumentRange(nameof(month), month, 1, 12);
            }
            // If we've been asked for day 1-28, we're definitely okay regardless of month.
            if (day >= 1 && day <= 28)
            {
                return;
            }
            int daysInMonth = month == 2 && IsGregorianLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];
            if (day > daysInMonth)
            {
                Preconditions.CheckArgumentRange(nameof(day), day, 1, daysInMonth);
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

        // Override GetDaysInYear so we can avoid a pointless virtual method call.
        internal override int GetDaysInYear(int year) => IsGregorianLeapYear(year) ? 366 : 365;

        internal override bool IsLeapYear(int year) => IsGregorianLeapYear(year);

        private static bool IsGregorianLeapYear(int year) => ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
    }
}
