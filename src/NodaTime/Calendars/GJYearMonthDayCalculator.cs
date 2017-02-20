// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Annotations;

namespace NodaTime.Calendars
{
    internal abstract class GJYearMonthDayCalculator : RegularYearMonthDayCalculator
    {
        // These arrays are NOT public. We trust ourselves not to alter the array.
        // They use zero-based array indexes so the that valid range of months is
        // automatically checked. They are protected so that GregorianYearMonthDayCalculator can
        // read them.
        protected static readonly int[] MinDaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        protected static readonly int[] MaxDaysPerMonth = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        private static readonly int[] MinTotalDaysByMonth;
        private static readonly int[] MaxTotalDaysByMonth;

        static GJYearMonthDayCalculator()
        {
            MinTotalDaysByMonth = new int[12];
            MaxTotalDaysByMonth = new int[12];
            int minSum = 0;
            int maxSum = 0;
            for (int i = 0; i < 11; i++)
            {
                minSum += MinDaysPerMonth[i];
                maxSum += MaxDaysPerMonth[i];
                MinTotalDaysByMonth[i + 1] = minSum;
                MaxTotalDaysByMonth[i + 1] = maxSum;
            }
        }

        protected GJYearMonthDayCalculator(int minYear, int maxYear, int averageDaysPer10Years, int daysAtStartOfYear1)
            : base(minYear, maxYear, 12, averageDaysPer10Years, daysAtStartOfYear1)
        {
        }

        // Note: parameter is renamed to d for brevity. It's still the 1-based day-of-year
        internal override YearMonthDay GetYearMonthDay([Trusted] int year, int d)
        {
            bool isLeap = IsLeapYear(year);

            int startOfMonth;
            // Perform a hard-coded binary search to get the 0-based start day of the month. We can
            // then use that to work out the month... without ever hitting the heap. The values
            // are still MinTotalDaysPerMonth and MaxTotalDaysPerMonth (-1 for convenience), just hard-coded.
            if (isLeap)
            {
                startOfMonth = ((d < 183)
                              ? ((d < 92) ? ((d < 32) ? 0 : (d < 61) ? 31 : 60) : ((d < 122) ? 91 : (d < 153) ? 121 : 152))
                              : ((d < 275)
                                     ? ((d < 214) ? 182 : (d < 245) ? 213 : 244)
                                     : ((d < 306) ? 274 : (d < 336) ? 305 : 335)));
            }
            else
            {
                startOfMonth = ((d < 182)
                              ? ((d < 91) ? ((d < 32) ? 0 : (d < 60) ? 31 : 59) : ((d < 121) ? 90 : (d < 152) ? 120 : 151))
                              : ((d < 274)
                                     ? ((d < 213) ? 181 : (d < 244) ? 212 : 243)
                                     : ((d < 305) ? 273 : (d < 335) ? 304 : 334)));
            }

            int dayOfMonth = d - startOfMonth;
            return new YearMonthDay(year, (startOfMonth / 29) + 1, dayOfMonth);
        }

        internal override int GetDaysInYear([Trusted] int year) => IsLeapYear(year) ? 366 : 365;

        internal sealed override int GetDaysInMonth([Trusted] int year, [Trusted] int month) =>
            // We know that only February differs, so avoid the virtual call for other months.
            month == 2 && IsLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];

        protected override int GetDaysFromStartOfYearToStartOfMonth([Trusted] int year, [Trusted] int month) =>
            IsLeapYear(year) ? MaxTotalDaysByMonth[month - 1] : MinTotalDaysByMonth[month - 1];
    }
}
