// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Calculator for week-year, week-of-week-year and day-of-week-based calculations.
    /// </summary>
    internal sealed class WeekYearCalculator
    {
        private readonly YearMonthDayCalculator yearMonthDayCalculator;
        private readonly int minDaysInFirstWeek;

        internal WeekYearCalculator(YearMonthDayCalculator yearMonthDayCalculator, int minDaysInFirstWeek)
        {
            this.yearMonthDayCalculator = yearMonthDayCalculator;
            this.minDaysInFirstWeek = minDaysInFirstWeek;
        }

        internal YearMonthDay GetYearMonthDay(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            Preconditions.CheckArgumentRange("weekYear", weekYear, yearMonthDayCalculator.MinYear, yearMonthDayCalculator.MaxYear);
            Preconditions.CheckArgumentRange("weekOfWeekYear", weekOfWeekYear, 1, GetWeeksInWeekYear(weekYear));
            // TODO: Work out what argument validation we actually want here.
            Preconditions.CheckArgumentRange("dayOfWeek", (int)dayOfWeek, 1, 7);
            int days = GetWeekYearDaysSinceEpoch(weekYear) + (weekOfWeekYear - 1) * 7 + ((int) dayOfWeek - 1);
            return yearMonthDayCalculator.GetYearMonthDay(days);
        }

        internal int GetDayOfWeek(YearMonthDay yearMonthDay)
        {
            int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            return GetDayOfWeek(daysSinceEpoch);
        }

        /// <summary>
        /// Finds the week-of-week year containing the given local instant, by finding out when the week year
        /// started, and then simply dividing "how far we are through the year" by "the number of ticks in a week".
        /// </summary>
        internal int GetWeekOfWeekYear(YearMonthDay yearMonthDay)
        {
            // TODO(2.0): This is a bit inefficient, as we'll be converting forms several times. We might want to
            // optimize.
            int weekYear = GetWeekYear(yearMonthDay);
            int startOfWeekYear = GetWeekYearDaysSinceEpoch(weekYear);
            int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            int zeroBasedDayOfWeekYear = daysSinceEpoch - startOfWeekYear;
            int zeroBasedWeek = zeroBasedDayOfWeekYear / 7;
            return zeroBasedWeek + 1;
        }

        /// <summary>
        /// Finds the week-year containing the given local instant.
        /// </summary>
        internal int GetWeekYear(YearMonthDay yearMonthDay)
        {
            // Let's guess that it's in the same week year as calendar year, and check that.
            int calendarYear = yearMonthDay.Year;
            int startOfWeekYear = GetWeekYearDaysSinceEpoch(calendarYear);
            int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            if (daysSinceEpoch < startOfWeekYear)
            {
                // No, the week-year hadn't started yet. For example, we've been given January 1st 2011...
                // and the first week of week-year 2011 starts on January 3rd 2011. Therefore the local instant
                // must belong to the last week of the previous week-year.
                return calendarYear - 1;
            }

            // By now, we know it's either calendarYear or calendarYear + 1. Check using the number of
            // weeks in the year. Note that this will fetch the start of the calendar year and the week year
            // again, so could be optimized by copying some logic here - but only when we find we need to.
            int weeksInWeekYear = GetWeeksInWeekYear(calendarYear);

            // We assume that even for the maximum year, we've got just about enough leeway to get to the
            // start of the week year. (If not, we should adjust the maximum.)
            int startOfNextWeekYear = startOfWeekYear + weeksInWeekYear * 7;
            return daysSinceEpoch < startOfNextWeekYear ? calendarYear : calendarYear + 1;
        }

        private int GetDayOfWeek(int daysSinceEpoch)
        {
            return daysSinceEpoch >= -3 ? 1 + ((daysSinceEpoch + 3) % 7)
                                        : 7 + ((daysSinceEpoch + 4) % 7);
        }

        private int GetWeeksInWeekYear(int weekYear)
        {
            int startOfWeekYear = GetWeekYearDaysSinceEpoch(weekYear);
            int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);
            // The number of days gained or lost in the week year compared with the calendar year.
            // So if the week year starts on December 31st of the previous calendar year, this will be +1.
            // If the week year starts on January 2nd of this calendar year, this will be -1.

            int extraDays = startOfCalendarYear - startOfWeekYear;
            int daysInThisYear = yearMonthDayCalculator.GetDaysInYear(weekYear);

            // We can have up to "minDaysInFirstWeek - 1" days of the next year, too.
            return (daysInThisYear + extraDays + (minDaysInFirstWeek - 1)) / 7;
        }

        /// <summary>
        /// Returns the ticks at the start of the given week-year.
        /// </summary>
        private int GetWeekYearDaysSinceEpoch(int weekYear)
        {
            // Need to be slightly careful here, as the week-year can reasonably be outside the calendar year range.
            int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);
            int jan1DayOfWeek = GetDayOfWeek(startOfCalendarYear);

            if (jan1DayOfWeek > (8 - minDaysInFirstWeek))
            {
                // First week is end of previous year because it doesn't have enough days.
                return startOfCalendarYear + (8 - jan1DayOfWeek);
            }
            else
            {
                // First week is start of this year because it has enough days.
                return startOfCalendarYear - (jan1DayOfWeek - 1);
            }
        }
    }
}
