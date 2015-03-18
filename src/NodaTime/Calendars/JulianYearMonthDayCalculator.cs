// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Calendars
{
    internal sealed class JulianYearMonthDayCalculator : GJYearMonthDayCalculator
    {
        private const int AverageDaysPer10JulianYears = 3653; // Ideally 365.25 per year

        internal JulianYearMonthDayCalculator()
            : base(-9997, 9998, AverageDaysPer10JulianYears, -719164)
        {
        }

        internal override bool IsLeapYear(int year) => (year & 3) == 0;

        protected override int CalculateStartOfYearDays(int year)
        {
            // Unix epoch is 1970-01-01 Gregorian which is 1969-12-19 Julian.
            // Calculate relative to the nearest leap year and account for the
            // difference later.

            int relativeYear = year - 1968;
            int leapYears;
            if (relativeYear <= 0)
            {
                // Add 3 before shifting right since /4 and >>2 behave differently
                // on negative numbers.
                leapYears = (relativeYear + 3) >> 2;
            }
            else
            {
                leapYears = relativeYear >> 2;
                // For post 1968 an adjustment is needed as jan1st is before leap day
                if (!IsLeapYear(year))
                {
                    leapYears++;
                }
            }

            // Accounts for the difference between January 1st 1968 and December 19th 1969.
            return (relativeYear * 365 + leapYears - (366 + 352));
        }
    }
}
