// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Extensions;
using NodaTime.Utility;
using System;
using System.Globalization;
using static System.Globalization.CalendarWeekRule;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Implementation of <see cref="IWeekYearRule"/> to emulate the behavior of
    /// <see cref="Calendar.GetWeekOfYear(DateTime, CalendarWeekRule, DayOfWeek)"/>.
    /// </summary>
    [Immutable]
    public sealed class BclWeekYearRule : IWeekYearRule
    {
        private readonly CalendarWeekRule rule;
        private readonly IsoDayOfWeek firstDayOfWeek;
        private readonly int minDaysInFirstWeek;

        // TODO: Expose the above as properties?

        private BclWeekYearRule(CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
        {
            this.rule = calendarWeekRule;
            this.firstDayOfWeek = firstDayOfWeek.ToIsoDayOfWeek();
            switch (calendarWeekRule)
            {
                case FirstDay:
                    minDaysInFirstWeek = 1;
                    break;
                case FirstFourDayWeek:
                    minDaysInFirstWeek = 4;
                    break;
                case FirstFullWeek:
                    minDaysInFirstWeek = 7;
                    break;
                default:
                    throw new ArgumentException($"Unsupported CalendarWeekRule: {calendarWeekRule}", nameof(calendarWeekRule));
            }
        }

        /// <summary>
        /// Creates a rule which behaves the same way as the BCL
        /// <see cref="Calendar.GetWeekOfYear(DateTime, CalendarWeekRule, DayOfWeek)"/>
        /// method.
        /// </summary>
        /// <remarks>The BCL week year rules are subtly different to the ISO rules.
        /// In particular, the last few days of the calendar year are always part of the same
        /// week-year in the BCL rules, whereas in the ISO rules they can fall into the next
        /// week-year. (The first few days of the calendar year can be part of the previous
        /// week-year in both kinds of rule.) This means that in the BCL rules, some weeks
        /// are incomplete, whereas ISO weeks are always exactly 7 days long.
        /// </remarks>
        /// <param name="calendarWeekRule">The BCL rule to emulate.</param>
        /// <param name="firstDayOfWeek">The first day of the week to use in the rule.</param>
        public static BclWeekYearRule FromCalendarWeekRule(CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
            => new BclWeekYearRule(calendarWeekRule, firstDayOfWeek);

        /// <inheritdoc />
        public LocalDate GetLocalDate(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek, [NotNull] CalendarSystem calendar)
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
                int startOfWeekYear = GetRegularWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear);
                int daysIntoWeek = ((dayOfWeek - firstDayOfWeek) + 7) % 7;
                int daysIntoWeekYear = (weekOfWeekYear - 1) * 7 + daysIntoWeek;
                int days = startOfWeekYear + daysIntoWeekYear;
                if (days < calendar.MinDays || days > calendar.MaxDays || daysIntoWeekYear < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(weekYear),
                        $"The combination of {nameof(weekYear)}, {nameof(weekOfWeekYear)} and {nameof(dayOfWeek)} is invalid");
                }
                LocalDate ret = new LocalDate(yearMonthDayCalculator.GetYearMonthDay(days).WithCalendar(calendar));
                // The requested week year and the actual calendar year don't match, it could be due to short weeks.
                // The simplest way to find out is just to check what the week year is...
                if (weekYear != ret.Year)
                {
                    if (GetWeekYear(ret) != weekYear)
                    {
                        throw new ArgumentOutOfRangeException(nameof(weekYear),
                            $"The combination of {nameof(weekYear)}, {nameof(weekOfWeekYear)} and {nameof(dayOfWeek)} is invalid");
                    }
                }
                return ret;
            }
        }

        /// <inheritdoc />
        public int GetWeekOfWeekYear(LocalDate date)
        {
            YearMonthDay yearMonthDay = date.YearMonthDay;
            YearMonthDayCalculator yearMonthDayCalculator = date.Calendar.YearMonthDayCalculator;
            // This is a bit inefficient, as we'll be converting forms several times. However, it's
            // understandable... we might want to optimize in the future if it's reported as a bottleneck.
            int weekYear = GetWeekYear(date);
            // Even if this is before the *real* start of the week year, that doesn't change the
            // week-of-week-year, as we've definitely got the right week-year to start with.
            int startOfWeekYear = GetRegularWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear);
            int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
            int zeroBasedDayOfWeekYear = daysSinceEpoch - startOfWeekYear;
            int zeroBasedWeek = zeroBasedDayOfWeekYear / 7;
            return zeroBasedWeek + 1;
        }

        /// <inheritdoc />
        public int GetWeeksInWeekYear(int weekYear, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            YearMonthDayCalculator yearMonthDayCalculator = calendar.YearMonthDayCalculator;
            ValidateWeekYear(weekYear, calendar);
            unchecked
            {
                int startOfWeekYear = GetRegularWeekYearDaysSinceEpoch(yearMonthDayCalculator, weekYear);
                int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);

                // The number of days gained or lost in the week year compared with the calendar year.
                // So if the week year starts on December 31st of the previous calendar year, this will be +1.
                // If the week year starts on January 2nd of this calendar year, this will be -1.
                // If this is positive, then those aren't "real" days in the week year, as the BCL
                // never has the end of a calendar year in the following week year. However, it doesn't
                // matter for the purposes of this calculation.
                int extraDays = startOfCalendarYear - startOfWeekYear;
                int daysInThisYear = yearMonthDayCalculator.GetDaysInYear(weekYear);

                // There are daysInThisYear + extraDays within this week-year, so we just need to round up.
                return (daysInThisYear + extraDays + 6) / 7;
            }
        }

        /// <inheritdoc />
        public int GetWeekYear(LocalDate date)
        {
            YearMonthDay yearMonthDay = date.YearMonthDay;
            YearMonthDayCalculator yearMonthDayCalculator = date.Calendar.YearMonthDayCalculator;
            unchecked
            {
                // Let's guess that it's in the same week year as calendar year, and check that.
                int calendarYear = yearMonthDay.Year;
                int startOfWeekYear = GetRegularWeekYearDaysSinceEpoch(yearMonthDayCalculator, calendarYear);
                int daysSinceEpoch = yearMonthDayCalculator.GetDaysSinceEpoch(yearMonthDay);
                if (daysSinceEpoch < startOfWeekYear)
                {
                    // No, the week-year hadn't started yet. For example, we've been given January 1st 2011...
                    // and the first week of week-year 2011 starts on January 3rd 2011. Therefore the date
                    // must belong to the last week of the previous week-year.
                    return calendarYear - 1;
                }

                // In BCL rules, a day can belong to the *previous* week year, but never the *next* week year.
                // So at this point, we're done.
                return calendarYear;
            }
        }

        /// <summary>
        /// Validate that at least one day in the calendar falls in the given week year.
        /// </summary>
        private void ValidateWeekYear(int weekYear, CalendarSystem calendar)
        {
            // FIXME: Adjust for BCL truncation.
            if (weekYear > calendar.MinYear && weekYear < calendar.MaxYear)
            {
                return;
            }
            int minCalendarYearDays = GetRegularWeekYearDaysSinceEpoch(calendar.YearMonthDayCalculator, calendar.MinYear);
            // If week year X started after calendar year X, then the first days of the calendar year are in the
            // previous week year.
            int minWeekYear = minCalendarYearDays > calendar.MinDays ? calendar.MinYear - 1 : calendar.MinYear;
            int maxCalendarYearDays = GetRegularWeekYearDaysSinceEpoch(calendar.YearMonthDayCalculator, calendar.MaxYear + 1);
            // If week year X + 1 started after the last day in the calendar, then everything is within week year X.
            int maxWeekYear = maxCalendarYearDays > calendar.MaxDays ? calendar.MaxYear : calendar.MaxYear + 1;
            Preconditions.CheckArgumentRange(nameof(weekYear), weekYear, minWeekYear, maxWeekYear);
        }

        /// <summary>
        /// Returns the days at the start of the given week-year. The week-year may be
        /// 1 higher or lower than the max/min calendar year. This returns a "regular" 
        /// start of week, so it may be in the previous calendar year. (This isn't the same
        /// as the start of the week year in the BCL, which is always in the same calendar year,
        /// even if that means that the first week is short.)
        /// </summary>
        private int GetRegularWeekYearDaysSinceEpoch(YearMonthDayCalculator yearMonthDayCalculator, [Trusted] int weekYear)
        {
            unchecked
            {
                // Need to be slightly careful here, as the week-year can reasonably be (just) outside the calendar year range.
                // However, YearMonthDayCalculator.GetStartOfYearInDays already handles min/max -/+ 1.
                int startOfCalendarYear = yearMonthDayCalculator.GetStartOfYearInDays(weekYear);
                // Inlined from CalendarSystem.GetDayOfWeek
                int startOfYearDayOfWeek = unchecked(startOfCalendarYear >= -3 ? 1 + ((startOfCalendarYear + 3) % 7)
                                           : 7 + ((startOfCalendarYear + 4) % 7));

                // How many days have there been from the start of the week containing
                // the first day of the year, until the first day of the year? To put it another
                // way, how many days in the week *containing* the start of the calendar year were
                // in the previous calendar year.
                // (For example, if the start of the calendar year is Friday and the first day of the week is Monday,
                // this will be 4.)
                int daysIntoWeek = ((startOfYearDayOfWeek - (int) firstDayOfWeek) + 7) % 7;
                int startOfWeekContainingStartOfCalendarYear = startOfCalendarYear - daysIntoWeek;

                bool startOfYearIsInWeek1 = (7 - daysIntoWeek >= minDaysInFirstWeek);
                return startOfYearIsInWeek1 
                    ? startOfWeekContainingStartOfCalendarYear
                    : startOfWeekContainingStartOfCalendarYear + 7;
            }
        }
    }
}
