// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Subclass of YearMonthDayCalculator for calendars with the following attributes:
    /// <list type="bullet">
    /// <item>A fixed number of months</item>
    /// <item>Occasional leap years which are always 1 day longer than non-leap years</item>
    /// <item>The year starting with month 1, day 1 (i.e. naive YearMonthDay comparisons work)</item>
    /// </list>
    /// </summary>
    internal abstract class RegularYearMonthDayCalculator : YearMonthDayCalculator
    {
        private readonly int monthsInYear;

        protected RegularYearMonthDayCalculator(int minYear, int maxYear, int monthsInYear,
            int averageDaysPer10Years, int daysAtStartOfYear1, params Era[] eras)
            : base(minYear, maxYear, averageDaysPer10Years, daysAtStartOfYear1, eras)
        {
            this.monthsInYear = monthsInYear;
        }

        internal override int GetMaxMonth(int year)
        {
            return monthsInYear;
        }

        /// <summary>
        /// Implements a simple year-setting policy, truncating the day
        /// if necessary.
        /// </summary>
        internal override YearMonthDay SetYear(YearMonthDay yearMonthDay, int year)
        {
            // TODO(2.0): All subclasses have the same logic of "detect leap years,
            // and otherwise we're fine". Put it here instead.
            int currentMonth = yearMonthDay.Month;
            int currentDay = yearMonthDay.Day;
            int newDay = GetDaysInMonth(year, currentMonth);
            return new YearMonthDay(year, currentMonth, Math.Min(currentDay, newDay));
        }

        internal override YearMonthDay AddMonths(YearMonthDay yearMonthDay, int months)
        {
            if (months == 0)
            {
                return yearMonthDay;
            }
            // Get the year and month
            int thisYear = yearMonthDay.Year;
            int thisMonth = yearMonthDay.Month;

            // Do not refactor without careful consideration.
            // Order of calculation is important.

            int yearToUse;
            // Initially, monthToUse is zero-based
            int monthToUse = thisMonth - 1 + months;
            if (monthToUse >= 0)
            {
                yearToUse = thisYear + (monthToUse / monthsInYear);
                monthToUse = (monthToUse % monthsInYear) + 1;
            }
            else
            {
                yearToUse = thisYear + (monthToUse / monthsInYear) - 1;
                monthToUse = Math.Abs(monthToUse);
                int remMonthToUse = monthToUse % monthsInYear;
                // Take care of the boundary condition
                if (remMonthToUse == 0)
                {
                    remMonthToUse = monthsInYear;
                }
                monthToUse = monthsInYear - remMonthToUse + 1;
                // Take care of the boundary condition
                if (monthToUse == 1)
                {
                    yearToUse++;
                }
            }
            // End of do not refactor.

            // Quietly force DOM to nearest sane value.
            int dayToUse = yearMonthDay.Day;
            int maxDay = GetDaysInMonth(yearToUse, monthToUse);
            dayToUse = Math.Min(dayToUse, maxDay);
            return new YearMonthDay(yearToUse, monthToUse, dayToUse);
        }

        internal override int MonthsBetween(YearMonthDay minuendDate, YearMonthDay subtrahendDate)
        {
            int minuendYear = minuendDate.Year;
            int subtrahendYear = subtrahendDate.Year;
            int minuendMonth = minuendDate.Month;
            int subtrahendMonth = subtrahendDate.Month;

            int diff = (minuendYear - subtrahendYear) * monthsInYear + minuendMonth - subtrahendMonth;

            // If we just add the difference in months to subtrahendDate, what do we get?
            YearMonthDay simpleAddition = AddMonths(subtrahendDate, diff);

            // Note: this relies on naive comparison of year/month/date values.
            if (subtrahendDate <= minuendDate)
            {
                // Moving forward: if the result of the simple addition is before or equal to the minuend,
                // we're done. Otherwise, rewind a month because we've overshot.
                return simpleAddition <= minuendDate ? diff : diff - 1;
            }
            else
            {
                // Moving backward: if the result of the simple addition (of a non-positive number)
                // is after or equal to the minuend, we're done. Otherwise, increment by a month because
                // we've overshot backwards.
                return simpleAddition >= minuendDate ? diff : diff + 1;
            }
        }
    }
}
