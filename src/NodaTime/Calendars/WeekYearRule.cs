// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;
using System.Linq;

namespace NodaTime.Calendars
{
    /// <summary>
    /// A rule determining how "week years" are arranged, including the weeks within the week year.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Dates are usually identified within a calendar system by a calendar year, a month within that
    /// calendar year, and a day within that month. For example, the date of birth of Ada Lovelace can be identified
    /// within the Gregorian calendar system as the year 1815, the month December (12), and the day 10. However,
    /// dates can also be identified (again within a calendar system) by week-year, week and day-of-week. How
    /// that identification occurs depends on which rule you use - but again as an example, within the Gregorian
    /// calendar system, using the ISO-8601 week year rule, the date of Ada Lovelace's birth is week-year 1815,
    /// week 49, day-of-week Sunday.
    /// </para>
    /// <para>
    /// The calendar year of a date and the week-year of a date are the same in most rules for most dates, but aren't
    /// always. When they differ, it is usually because a day near the start of the calendar year is deemed to belong
    /// to the last week of the previous week-year - or conversely because a day near the end of the calendar year is
    /// deemed to belong to the first week of the following week-year. Some rules may be more radical -
    /// a UK tax year rule could number weeks from April 6th onwards, such that any date earlier than that in the calendar
    /// year would belong to the previous week-year.
    /// </para>
    /// <para>
    /// The mapping of dates into week-year, week and day-of-week is always relative to a specific calendar system.
    /// For example, years in the Hebrew calendar system vary very significantly in length due to leap months, and this
    /// is reflected in the number of weeks within the week-years - as low as 50, and as high as 55.
    /// </para>
    /// <para>
    /// This class allows conversions between the two schemes of identifying dates: <see cref="GetWeekYear(LocalDate)"/>
    /// and <see cref="GetWeekOfWeekYear(LocalDate)"/> allow the week-year and week to be obtained for a date, and
    /// <see cref="GetLocalDate(int, int, IsoDayOfWeek, CalendarSystem)"/> allows the reverse mapping. Note that
    /// the calendar system does not need to be specified in the former methods as a <see cref="LocalDate"/> already
    /// contains calendar information, and there is no method to obtain the day-of-week as that is not affected by the
    /// week year rule being used.
    /// </para>
    /// <para>Specific rules are obtained using the static factory methods within this class, or with the
    /// <see cref="Iso"/> property.</para>
    /// <para>
    /// All implementations within Noda Time are immutable, and it is advised that any external implementations
    /// should be immutable too.
    /// </para>
    /// </remarks>
    [Immutable]
    public abstract class WeekYearRule
    {
        private static readonly WeekYearRule[] isoBasedRules = Enumerable.Range(1, 7).Select(x => new IsoBasedWeekYearRule(x)).ToArray();

        /// <summary>
        /// Returns a <see cref="WeekYearRule"/> similar to ISO-8601, but allowing the minimum number of
        /// days in the first week to vary from the value of 4 specified by ISO-8601. Weeks are always
        /// deemed to begin on an Monday.
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
        /// <returns>A <see cref="WeekYearRule"/> with the specified minimum number of days in the first
        /// week.</returns>
        public static WeekYearRule ForMinDaysInFirstWeek(int minDaysInFirstWeek)
        {
            Preconditions.CheckArgumentRange(nameof(minDaysInFirstWeek), minDaysInFirstWeek, 1, 7);
            return isoBasedRules[minDaysInFirstWeek - 1];
        }

        /// <summary>
        /// Returns a <see cref="WeekYearRule"/> consistent with ISO-8601.
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
        /// <value>A <see cref="WeekYearRule"/> consistent with ISO-8601.</value>
        public static WeekYearRule Iso { get; } = isoBasedRules[3];

        // TODO: public static WeekYearRule FromBclRule(CalendarWeekRule rule, IsoDayOfWeek firstDayOfWeek) => null;
        // TODO: public static WeekYearRule FromBclRule(CalendarWeekRule rule, DayOfWeek firstDayOfWeek)
        //    => FromBclRule(rule, BclConversions.ToIsoDayOfWeek(firstDayOfWeek));

        // TODO: Make IsoBasedWeekYearRule more flexible, allowing non-Monday-to-Sunday weeks?

        /// <summary>
        /// Creates a <see cref="LocalDate" /> from a given week-year, week within that week-year,
        /// and day-of-week, for the specified calendar system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Wherever reasonable, implementations should ensure that all valid dates
        /// can be constructed via this method. In other words, given a <see cref="LocalDate"/> <code>date</code>,
        /// <code>rule.GetLocalDate(rule.GetWeekYear(date), rule.GetWeekOfWeekYear(date), date.IsoDayOfWeek, date.Calendar)</code>
        /// should always return <code>date</code>. This is true for all rules within Noda Time, but third party
        /// implementations may choose to simplify their implementations by restricting them to appropriate portions
        /// of time.
        /// </para>
        /// <para>
        /// Implementations may restrict which calendar systems supplied here, but the implementations provided by
        /// Noda Time work with all available calendar systems.
        /// </para>
        /// </remarks>
        /// <param name="weekYear">The week-year of the new date. Implementations provided by Noda Time allow any
        /// year which is a valid calendar year, and sometimes one less than the minimum calendar year
        /// and/or one more than the maximum calendar year, to allow for dates near the start of a calendar
        /// year to fall in the previous week year, and similarly for dates near the end of a calendar year.</param>
        /// <param name="weekOfWeekYear">The week of week-year of the new date. Valid values for this parameter
        /// may vary depending on <paramref name="weekYear"/>, as the length of a year in weeks varies.</param>
        /// <param name="dayOfWeek">The day-of-week of the new date. Valid values for this parameter may vary
        /// depending on <paramref name="weekYear"/> and <paramref name="weekOfWeekYear"/>.</param>
        /// <param name="calendar">The calendar system for the date.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not combine to form a valid date.</exception>
        /// <returns>A <see cref="LocalDate"/> corresponding to the specified values.</returns>
        public abstract LocalDate GetLocalDate(
            int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek, [NotNull] CalendarSystem calendar);

        /// <summary>
        /// Convenience overload to call <see cref="GetLocalDate(int, int, IsoDayOfWeek, CalendarSystem)"/>
        /// passing in the ISO calendar system.
        /// </summary>
        /// <param name="weekYear">The week-year of the new date. Implementations provided by Noda Time allow any
        /// year which is a valid calendar year, and sometimes one less than the minimum calendar year
        /// and/or one more than the maximum calendar year, to allow for dates near the start of a calendar
        /// year to fall in the previous week year, and similarly for dates near the end of a calendar year.</param>
        /// <param name="weekOfWeekYear">The week of week-year of the new date. Valid values for this parameter
        /// may vary depending on <paramref name="weekYear"/>, as the length of a year in weeks varies.</param>
        /// <param name="dayOfWeek">The day-of-week of the new date. Valid values for this parameter may vary
        /// depending on <paramref name="weekYear"/> and <paramref name="weekOfWeekYear"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not combine to form a valid date.</exception>
        /// <returns>A <see cref="LocalDate"/> corresponding to the specified values.</returns>
        public LocalDate GetLocalDate(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek)
            => GetLocalDate(weekYear, weekOfWeekYear, dayOfWeek, CalendarSystem.Iso);

        /// <summary>
        /// Calculates the week-year in which the given date occurs, according to this rule.
        /// </summary>
        /// <param name="date">The date to compute the week-year of.</param>
        /// <returns>The week-year of <paramref name="date"/>, according to this rule.</returns>
        public abstract int GetWeekYear(LocalDate date);

        /// <summary>
        /// Calculates the week of the week-year in which the given date occurs, according to this rule.
        /// </summary>
        /// <param name="date">The date to compute the week of.</param>
        /// <returns>The week of the week-year of <paramref name="date"/>, according to this rule.</returns>
        public abstract int GetWeekOfWeekYear(LocalDate date);

        /// <summary>
        /// Returns the number of weeks in the given week-year, within the specified calendar system.
        /// </summary>
        /// <param name="weekYear">The week-year to find the range of.</param>
        /// <param name="calendar">The calendar system the calculation is relative to.</param>
        /// <returns>The number of weeks in the given week-year within the given calendar.</returns>
        public abstract int GetWeeksInWeekYear(int weekYear, [NotNull] CalendarSystem calendar);

        /// <summary>
        /// Convenience overload to call <see cref="GetWeeksInWeekYear(int, CalendarSystem)"/> with
        /// the ISO calendar system.
        /// </summary>
        /// <param name="weekYear">The week year to calculate the number of contained weeks.</param>
        /// <returns>The number of weeks in the given week year.</returns>
        public int GetWeeksInWeekYear(int weekYear) => GetWeeksInWeekYear(weekYear, CalendarSystem.Iso);
    }
}
