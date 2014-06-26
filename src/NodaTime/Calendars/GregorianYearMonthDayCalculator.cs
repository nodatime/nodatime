// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    internal class GregorianYearMonthDayCalculator : GJYearMonthDayCalculator
    {
        // We precompute useful values for each month between these years, as we anticipate most
        // dates will be in this range.
        private const int FirstOptimizedYear = 1900;
        private const int LastOptimizedYear = 2100;
        private static readonly int[] MonthStartDays = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
        private static readonly int[] MonthLengths = new int[(LastOptimizedYear + 1 - FirstOptimizedYear) * 12 + 1];
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
                YearStartDays[year - FirstOptimizedYear] = instance.CalculateStartOfYearDays(year);
                for (int month = 1; month <= 12; month++)
                {
                    int yearMonthIndex = (year - FirstOptimizedYear) * 12 + month;
                    MonthStartDays[yearMonthIndex] = instance.GetYearMonthDays(year, month);
                    MonthLengths[yearMonthIndex] = instance.GetDaysInMonth(year, month);
                }
            }
        }

        internal GregorianYearMonthDayCalculator()
            : base(-27255, 31195, AverageDaysPer10Years, -719162)
        {
        }

        // TODO(2.0): Check that this is worth doing, given our normal cache.
        internal override int GetStartOfYearInDays(int year)
        {
            if (year < FirstOptimizedYear || year > LastOptimizedYear)
            {
                return base.GetStartOfYearInDays(year);
            }
            return YearStartDays[year - FirstOptimizedYear];
        }

        // FIXME: Should I remove this? Could get called in the optimization...
        /*
        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + month;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || month < 1 || month > 12)
            {
                return base.GetDaysFromStartOfYearToStartOfMonth(year, month);
            }
            return MonthStartDays[yearMonthIndex];
        }&*/

        internal override int GetDaysSinceEpoch(YearMonthDay yearMonthDay)
        {
            int year = yearMonthDay.Year;
            int monthOfYear = yearMonthDay.Month;
            int dayOfMonth = yearMonthDay.Day;
            int yearMonthIndex = (year - FirstOptimizedYear) * 12 + monthOfYear;
            if (year < FirstOptimizedYear || year > LastOptimizedYear - 1 || monthOfYear < 1 || monthOfYear > 12 || dayOfMonth < 1 ||
                dayOfMonth > MonthLengths[yearMonthIndex])
            {
                return base.GetDaysSinceEpoch(yearMonthDay);
            }
            // This is guaranteed not to overflow, as we've already validated the arguments
            return unchecked(MonthStartDays[yearMonthIndex] + (dayOfMonth - 1));
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
            return ((year & 3) == 0) && ((year % 100) != 0 || (year % 400) == 0);
        }
    }
}
