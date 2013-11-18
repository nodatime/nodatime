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

        internal LocalInstant GetLocalInstant(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
        {
            Preconditions.CheckArgumentRange("weekYear", weekYear, yearMonthDayCalculator.MinYear, yearMonthDayCalculator.MaxYear);
            Preconditions.CheckArgumentRange("weekOfWeekYear", weekOfWeekYear, 1, GetWeeksInWeekYear(weekYear));
            // TODO: Work out what argument validation we actually want here.
            Preconditions.CheckArgumentRange("dayOfWeek", (int)dayOfWeek, 1, 7);
            long ticks = GetWeekYearTicks(weekYear)
                + (weekOfWeekYear - 1) * NodaConstants.TicksPerStandardWeek
                + ((int) dayOfWeek - 1) * NodaConstants.TicksPerStandardDay;
            return new LocalInstant(ticks);
        }

        internal static int GetDayOfWeek(LocalInstant localInstant)
        {
            // 1970-01-01 is day of week 4, Thursday.

            long daysSince19700101;
            long ticks = localInstant.Ticks;
            if (ticks >= 0)
            {
                daysSince19700101 = ticks / NodaConstants.TicksPerStandardDay;
            }
            else
            {
                daysSince19700101 = (ticks - (NodaConstants.TicksPerStandardDay - 1)) / NodaConstants.TicksPerStandardDay;
                if (daysSince19700101 < -3)
                {
                    return 7 + (int)((daysSince19700101 + 4) % 7);
                }
            }

            return 1 + (int)((daysSince19700101 + 3) % 7);
        }

        /// <summary>
        /// Finds the week-of-week year containing the given local instant, by finding out when the week year
        /// started, and then simply dividing "how far we are through the year" by "the number of ticks in a week".
        /// </summary>
        internal int GetWeekOfWeekYear(LocalInstant localInstant)
        {
            int weekYear = GetWeekYear(localInstant);
            long startOfWeekYear = GetWeekYearTicks(weekYear);
            long ticksIntoYear = localInstant.Ticks - startOfWeekYear;
            int zeroBasedWeek = (int)(ticksIntoYear / NodaConstants.TicksPerStandardWeek);
            return zeroBasedWeek + 1;
        }

        private int GetWeeksInWeekYear(int weekYear)
        {
            long startOfWeekYear = GetWeekYearTicks(weekYear);
            long startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInTicks(weekYear);
            // The number of days gained or lost in the week year compared with the calendar year.
            // So if the week year starts on December 31st of the previous calendar year, this will be +1.
            // If the week year starts on January 2nd of this calendar year, this will be -1.

            int extraDays = (int)((startOfCalendarYear - startOfWeekYear) / NodaConstants.TicksPerStandardDay);
            int daysInThisYear = yearMonthDayCalculator.GetDaysInYear(weekYear);

            // We can have up to "minDaysInFirstWeek - 1" days of the next year, too.
            return (daysInThisYear + extraDays + (minDaysInFirstWeek - 1)) / 7;
        }

        /// <summary>
        /// Returns the ticks at the start of the given week-year.
        /// </summary>
        private long GetWeekYearTicks(int weekYear)
        {
            // Need to be slightly careful here, as the week-year can reasonably be outside the calendar year range.
            long jan1Millis = yearMonthDayCalculator.GetStartOfYearInTicks(weekYear);
            int jan1DayOfWeek = GetDayOfWeek(new LocalInstant(jan1Millis));

            if (jan1DayOfWeek > (8 - minDaysInFirstWeek))
            {
                // First week is end of previous year because it doesn't have enough days.
                return jan1Millis + (8 - jan1DayOfWeek) * NodaConstants.TicksPerStandardDay;
            }
            else
            {
                // First week is start of this year because it has enough days.
                return jan1Millis - (jan1DayOfWeek - 1) * NodaConstants.TicksPerStandardDay;
            }
        }

        /// <summary>
        /// Finds the week-year containing the given local instant.
        /// </summary>
        internal int GetWeekYear(LocalInstant localInstant)
        {
            // Let's guess that it's in the same week year as calendar year, and check that.
            int calendarYear = yearMonthDayCalculator.GetYear(localInstant);
            long startOfWeekYear = GetWeekYearTicks(calendarYear);
            if (localInstant.Ticks < startOfWeekYear)
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
            long startOfNextCalendarYear = startOfWeekYear + weeksInWeekYear * NodaConstants.TicksPerStandardWeek;
            return localInstant.Ticks < startOfNextCalendarYear ? calendarYear : calendarYear + 1;
        }
    }
}
