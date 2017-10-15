// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Utility;
using System;

namespace NodaTime.Calendars
{
    /// <summary>
    /// A rule determining how "week years" are arranged, including the weeks within the week year.
    /// Implementations provided by Noda Time itself can be obtained via the <see cref="WeekYearRules"/>
    /// class.
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
    /// <para>
    /// All implementations within Noda Time are immutable, and it is advised that any external implementations
    /// should be immutable too.
    /// </para>
    /// </remarks>
    public interface IWeekYearRule
    {
        /// <summary>
        /// Creates a <see cref="LocalDate" /> from a given week-year, week within that week-year,
        /// and day-of-week, for the specified calendar system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Wherever reasonable, implementations should ensure that all valid dates
        /// can be constructed via this method. In other words, given a <see cref="LocalDate"/> <c>date</c>,
        /// <c>rule.GetLocalDate(rule.GetWeekYear(date), rule.GetWeekOfWeekYear(date), date.IsoDayOfWeek, date.Calendar)</c>
        /// should always return <c>date</c>. This is true for all rules within Noda Time, but third party
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
        LocalDate GetLocalDate(int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek, [NotNull] CalendarSystem calendar);

        /// <summary>
        /// Calculates the week-year in which the given date occurs, according to this rule.
        /// </summary>
        /// <param name="date">The date to compute the week-year of.</param>
        /// <returns>The week-year of <paramref name="date"/>, according to this rule.</returns>
        int GetWeekYear(LocalDate date);

        /// <summary>
        /// Calculates the week of the week-year in which the given date occurs, according to this rule.
        /// </summary>
        /// <param name="date">The date to compute the week of.</param>
        /// <returns>The week of the week-year of <paramref name="date"/>, according to this rule.</returns>
        int GetWeekOfWeekYear(LocalDate date);

        /// <summary>
        /// Returns the number of weeks in the given week-year, within the specified calendar system.
        /// </summary>
        /// <param name="weekYear">The week-year to find the range of.</param>
        /// <param name="calendar">The calendar system the calculation is relative to.</param>
        /// <returns>The number of weeks in the given week-year within the given calendar.</returns>
        int GetWeeksInWeekYear(int weekYear, [NotNull] CalendarSystem calendar);
    }

    /// <summary>
    /// Extension methods on <see cref="IWeekYearRule"/>.
    /// </summary>
    public static class WeekYearRuleExtensions
    {
        /// <summary>
        /// Convenience method to call <see cref="IWeekYearRule.GetLocalDate(int, int, IsoDayOfWeek, CalendarSystem)"/>
        /// passing in the ISO calendar system.
        /// </summary>
        /// <param name="rule">The rule to delegate the call to.</param>
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
        public static LocalDate GetLocalDate([NotNull] this IWeekYearRule rule, int weekYear, int weekOfWeekYear, IsoDayOfWeek dayOfWeek) =>
            Preconditions.CheckNotNull(rule, nameof(rule)).GetLocalDate(weekYear, weekOfWeekYear, dayOfWeek, CalendarSystem.Iso);

        /// <summary>
        /// Convenience overload to call <see cref="IWeekYearRule.GetWeeksInWeekYear(int, CalendarSystem)"/> with
        /// the ISO calendar system.
        /// </summary>
        /// <param name="rule">The rule to delegate the call to.</param>
        /// <param name="weekYear">The week year to calculate the number of contained weeks.</param>
        /// <returns>The number of weeks in the given week year.</returns>
        public static int GetWeeksInWeekYear([NotNull] this IWeekYearRule rule, int weekYear) =>
            Preconditions.CheckNotNull(rule, nameof(rule)).GetWeeksInWeekYear(weekYear, CalendarSystem.Iso);
    }
}
