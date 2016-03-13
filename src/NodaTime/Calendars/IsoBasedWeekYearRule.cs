// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.Calendars
{
    // Possible future feature: allow the first day of the week to vary too.

    /// <summary>
    /// WeekYearRule implementing the rules from Noda Time 1.x. Basically this is the ISO
    /// rules, but with a variable "minimum number of days in the first week".
    /// </summary>
    internal sealed class IsoBasedWeekYearRule : WeekYearRule
    {
        private readonly int minDaysInFirstWeek;

        internal IsoBasedWeekYearRule(int minDaysInFirstWeek)
        {
            Preconditions.DebugCheckArgumentRange(nameof(minDaysInFirstWeek), minDaysInFirstWeek, 1, 7);
            this.minDaysInFirstWeek = minDaysInFirstWeek;
        }

        /// <inheritdoc />
        public override LocalDate GetLocalDate(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            ValidateWeekYear(weekYear, calendar);

            // The actual message for this won't be ideal, but it's clear enough.
            Preconditions.CheckArgumentRange(nameof(dayOfWeek), (int)dayOfWeek, 1, 7);

            var yearMonthDayCalculator = calendar.YearMonthDayCalculator;
            var maxWeeks = GetWeeksInWeekYear(weekYear, calendar);
            if (weekOfWeekYear < 1 || weekOfWeekYear > maxWeeks)
            {
                throw new ArgumentOutOfRangeException(nameof(weekOfWeekYear));
            }
            
            unchecked
            {
                int days = GetWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear) + (weekOfWeekYear - 1) * 7 + ((int)dayOfWeek - 1);
                if (days < calendar.MinDays || days > calendar.MaxDays)
                {
                    throw new ArgumentOutOfRangeException(nameof(weekYear),
                        $"The combination of {nameof(weekYear)}, {nameof(weekOfWeekYear)} and {nameof(dayOfWeek)} is invalid");
                }
                return new LocalDate(yearMonthDayCalculator.GetYearMonthDay(days).WithCalendar(calendar));
            }
        }

        /// <inheritdoc />
        public override int GetWeekOfWeekYear(LocalDate date)
        {
            YearMonthDay yearMonthDay = date.YearMonthDay;
            YearMonthDayCalculator yearMonthDayCalculator = date.Calendar.YearMonthDayCalculator;
            // This is a bit inefficient, as we'll be converting forms several times. However, it's
            // understandable... we might want to optimize in the future if it's reported as a bottleneck.
            int weekYear = GetWeekYear(date);
            int startOfWeekYear = GetWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear);
            int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            int zeroBasedDayOfWeekYear = daysSinceEpoch - startOfWeekYear;
            int zeroBasedWeek = zeroBasedDayOfWeekYear / 7;
            return zeroBasedWeek + 1;
        }

        /// <inheritdoc />
        public override int GetWeeksInWeekYear(int weekYear, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            YearMonthDayCalculator yearMonthDayCalculator = calendar.YearMonthDayCalculator;
            ValidateWeekYear(weekYear, calendar);
            unchecked
            {
                int startOfWeekYear = GetWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear);
                int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);
                // The number of days gained or lost in the week year compared with the calendar year.
                // So if the week year starts on December 31st of the previous calendar year, this will be +1.
                // If the week year starts on January 2nd of this calendar year, this will be -1.

                int extraDays = startOfCalendarYear - startOfWeekYear;
                int daysInThisYear = yearMonthDayCalculator.GetDaysInYear(weekYear);

                // We can have up to "minDaysInFirstWeek - 1" days of the next year, too.
                return (daysInThisYear + extraDays + (minDaysInFirstWeek - 1)) / 7;
            }
        }

        /// <inheritdoc />
        public override int GetWeekYear(LocalDate date)
        {
            YearMonthDay yearMonthDay = date.YearMonthDay;
            YearMonthDayCalculator yearMonthDayCalculator = date.Calendar.YearMonthDayCalculator;
            unchecked
            {
                // Let's guess that it's in the same week year as calendar year, and check that.
                int calendarYear = yearMonthDay.Year;
                int startOfWeekYear = GetWeekYearDaysSinceEpoch(yearMonthDayCalculator, calendarYear);
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
                int weeksInWeekYear = GetWeeksInWeekYear(calendarYear, date.Calendar);

                // We assume that even for the maximum year, we've got just about enough leeway to get to the
                // start of the week year. (If not, we should adjust the maximum.)
                int startOfNextWeekYear = startOfWeekYear + weeksInWeekYear * 7;
                return daysSinceEpoch < startOfNextWeekYear ? calendarYear : calendarYear + 1;
            }
        }

        /// <summary>
        /// Validate that at least one day in the calendar falls in the given week year.
        /// </summary>
        private void ValidateWeekYear(int weekYear, CalendarSystem calendar)
        {
            if (weekYear > calendar.MinYear && weekYear < calendar.MaxYear)
            {
                return;
            }
            int minCalendarYearDays = GetWeekYearDaysSinceEpoch(calendar.YearMonthDayCalculator, calendar.MinYear);
            // If week year X started after calendar year X, then the first days of the calendar year are in the
            // previous week year.
            int minWeekYear = minCalendarYearDays > calendar.MinDays ? calendar.MinYear - 1 : calendar.MinYear;
            int maxCalendarYearDays = GetWeekYearDaysSinceEpoch(calendar.YearMonthDayCalculator, calendar.MaxYear + 1);
            // If week year X + 1 started after the last day in the calendar, then everything is within week year X.
            int maxWeekYear = maxCalendarYearDays > calendar.MaxDays ? calendar.MaxYear : calendar.MaxYear + 1;
            Preconditions.CheckArgumentRange(nameof(weekYear), weekYear, minWeekYear, maxWeekYear);
        }

        /// <summary>
        /// Returns the days at the start of the given week-year. The week-year may be
        /// 1 higher or lower than the max/min calendar year.
        /// </summary>
        private int GetWeekYearDaysSinceEpoch(YearMonthDayCalculator yearMonthDayCalculator, [Trusted] int weekYear)
        {
            unchecked
            {
                // Need to be slightly careful here, as the week-year can reasonably be (just) outside the calendar year range.
                // However, YearMonthDayCalculator.GetStartOfYearInDays already handles min/max -/+ 1.
                int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);
                int startOfYearDayOfWeek = unchecked(startOfCalendarYear >= -3 ? 1 + ((startOfCalendarYear + 3) % 7)
                                           : 7 + ((startOfCalendarYear + 4) % 7));

                if (startOfYearDayOfWeek > (8 - minDaysInFirstWeek))
                {
                    // First week is end of previous year because it doesn't have enough days.
                    return startOfCalendarYear + (8 - startOfYearDayOfWeek);
                }
                else
                {
                    // First week is start of this year because it has enough days.
                    return startOfCalendarYear - (startOfYearDayOfWeek - 1);
                }
            }
        }
    }
}
