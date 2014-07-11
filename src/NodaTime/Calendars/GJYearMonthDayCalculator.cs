// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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
            : base(minYear, maxYear, 12, averageDaysPer10Years, daysAtStartOfYear1, Era.BeforeCommon, Era.Common)
        {
        }

        // Note: parameter is renamed to d for brevity. It's still the 1-based day-of-year
        internal override YearMonthDay GetYearMonthDay(int year, int d)
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

        internal sealed override int GetDaysInMonth(int year, int month)
        {
            // We know that only February differs, so avoid the virtual call for other months.
            return month == 2 && IsLeapYear(year) ? MaxDaysPerMonth[month - 1] : MinDaysPerMonth[month - 1];
        }

        protected override int GetDaysFromStartOfYearToStartOfMonth(int year, int month)
        {
            return IsLeapYear(year) ? MaxTotalDaysByMonth[month - 1] : MinTotalDaysByMonth[month - 1];
        }

        internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
        {
            int month = yearMonthDay.Month;
            int day = yearMonthDay.Day;
            // The only value which might change day is Feb 29th.
            if (month == 2 && day == 29 && !IsLeapYear(year))
            {
                day = 28;
            }
            return new YearMonthDay(year, month, day);
        }

        #region Era handling
        internal override int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - yearOfEra: yearOfEra;
        }

        internal override int GetMaxYearOfEra(int eraIndex)
        {
            // By now the era will have been validated; it's either 0 (BC) or 1 (AD)
            return eraIndex == 0 ? 1 - MinYear : MaxYear;
        }

        internal override int GetYearOfEra(YearMonthDay yearMonthDay)
        {
            int year = yearMonthDay.Year;
            return year <= 0 ? -year + 1 : year;
        }

        internal override int GetEra(YearMonthDay yearMonthDay)
        {
            return yearMonthDay.Year < 1 ? 0 : 1;
        }
        #endregion
    }
}
