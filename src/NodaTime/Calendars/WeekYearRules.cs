// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Extensions;
using System;
using System.Globalization;
using static System.Globalization.CalendarWeekRule;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Factory methods to construct week-year rules supported by Noda Time.
    /// </summary>
    public static class WeekYearRules
    {
        /// <summary>
        /// Returns an <see cref="IWeekYearRule"/> consistent with ISO-8601.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the standard ISO-8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year. In ISO-8601,
        /// weeks always begin on a Monday, so this rule is equivalent to the first Thursday
        /// being in the first Monday-to-Sunday week of the year.
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of week-year 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of week-year 2013.
        /// </para>
        /// </remarks>
        /// <value>A <see cref="IWeekYearRule"/> consistent with ISO-8601.</value>
        [NotNull] public static IWeekYearRule Iso { get; } = new SimpleWeekYearRule(4, IsoDayOfWeek.Monday, false);

        /// <summary>
        /// Creates a week year rule where the boundary between one week-year and the next
        /// is parameterized in terms of how many days of the first week of the week
        /// year have to be in the new calendar year. In rules created by this method, 
        /// weeks are always deemed to begin on an Monday.
        /// </summary>
        /// <remarks>
        /// <paramref name="minDaysInFirstWeek"/> determines when the first week of the week-year starts.
        /// For any given calendar year X, consider the Monday-to-Sunday week that includes the first day of the
        /// calendar year. Usually, some days of that week are in calendar year X, and some are in calendar year 
        /// X-1. If <paramref name="minDaysInFirstWeek"/> or more of the days are in year X, then the week is
        /// deemed to be the first week of week-year X. Otherwise, the week is deemed to be the last week of
        /// week-year X-1, and the first week of week-year X starts on the following Monday.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first Monday-to-Sunday week
        /// which have to be in the new calendar year for that week to count as being in that week-year.
        /// Must be in the range 1 to 7 inclusive.
        /// </param>
        /// <returns>A <see cref="SimpleWeekYearRule"/> with the specified minimum number of days in the first
        /// week.</returns>
        [NotNull] public static IWeekYearRule ForMinDaysInFirstWeek(int minDaysInFirstWeek)
            => ForMinDaysInFirstWeek(minDaysInFirstWeek, IsoDayOfWeek.Monday);

        /// <summary>
        /// Creates a week year rule where the boundary between one week-year and the next
        /// is parameterized in terms of how many days of the first week of the week
        /// year have to be in the new calendar year, and also by which day is deemed
        /// to be the first day of the week.
        /// </summary>
        /// <remarks>
        /// <paramref name="minDaysInFirstWeek"/> determines when the first week of the week-year starts.
        /// For any given calendar year X, consider the week that includes the first day of the
        /// calendar year. Usually, some days of that week are in calendar year X, and some are in calendar year 
        /// X-1. If <paramref name="minDaysInFirstWeek"/> or more of the days are in year X, then the week is
        /// deemed to be the first week of week-year X. Otherwise, the week is deemed to be the last week of
        /// week-year X-1, and the first week of week-year X starts on the following <paramref name="firstDayOfWeek"/>.
        /// </remarks>
        /// <param name="minDaysInFirstWeek">The minimum number of days in the first week (starting on
        /// <paramref name="firstDayOfWeek" />) which have to be in the new calendar year for that week
        /// to count as being in that week-year. Must be in the range 1 to 7 inclusive.
        /// </param>
        /// <param name="firstDayOfWeek">The first day of the week.</param>
        /// <returns>A <see cref="SimpleWeekYearRule"/> with the specified minimum number of days in the first
        /// week and first day of the week.</returns>
        [NotNull] public static IWeekYearRule ForMinDaysInFirstWeek(int minDaysInFirstWeek, IsoDayOfWeek firstDayOfWeek)
            => new SimpleWeekYearRule(minDaysInFirstWeek, firstDayOfWeek, false);

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
        /// <returns>A rule which behaves the same way as the BCL
        /// <see cref="Calendar.GetWeekOfYear(DateTime, CalendarWeekRule, DayOfWeek)"/>
        /// method.</returns>
        [NotNull] public static IWeekYearRule FromCalendarWeekRule(CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
        {
            int minDaysInFirstWeek;
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
            return new SimpleWeekYearRule(minDaysInFirstWeek, firstDayOfWeek.ToIsoDayOfWeek(), true);
        }
    }
}
