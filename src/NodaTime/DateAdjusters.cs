// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;
using System;

namespace NodaTime
{
    /// <summary>
    /// Factory class for date adjusters: functions from <see cref="LocalDate"/> to <c>LocalDate</c>,
    /// which can be applied to <see cref="LocalDate"/>, <see cref="LocalDateTime"/>, and <see cref="OffsetDateTime"/>.
    /// </summary>
    public static class DateAdjusters
    {
        /// <summary>
        /// A date adjuster to move to the first day of the current month.
        /// </summary>
        /// <value>
        /// A date adjuster to move to the first day of the current month.
        /// </value>
        public static Func<LocalDate, LocalDate> StartOfMonth { get; } =
            date => new LocalDate(date.Year, date.Month, 1, date.Calendar);

        /// <summary>
        /// A date adjuster to move to the last day of the current month.
        /// </summary>
        /// <value>
        /// A date adjuster to move to the last day of the current month.
        /// </value>
        public static Func<LocalDate, LocalDate> EndOfMonth { get; } =
            date => new LocalDate(date.Year, date.Month, date.Calendar.GetDaysInMonth(date.Year, date.Month), date.Calendar);

        /// <summary>
        /// A date adjuster to move to the specified day of the current month.
        /// </summary>
        /// <remarks>
        /// The returned adjuster will throw an exception if it is applied to a date
        /// that would create an invalid result.
        /// </remarks>
        /// <param name="day">The day of month to adjust dates to.</param>
        /// <returns>An adjuster which changes the day to <paramref name="day"/>,
        /// retaining the same year and month.</returns>
        public static Func<LocalDate, LocalDate> DayOfMonth(int day) =>
            date => new LocalDate(date.Year, date.Month, day, date.Calendar);

        /// <summary>
        /// A date adjuster to move to the same day of the specified month.
        /// </summary>
        /// <remarks>
        /// The returned adjuster will throw an exception if it is applied to a date
        /// that would create an invalid result.
        /// </remarks>
        /// <param name="month">The month to adjust dates to.</param>
        /// <returns>An adjuster which changes the month to <paramref name="month"/>,
        /// retaining the same year and day of month.</returns>
        public static Func<LocalDate, LocalDate> Month(int month) =>
            date => new LocalDate(date.Year, month, date.Day, date.Calendar);

        /// <summary>
        /// A date adjuster to move to the next specified day-of-week, but return the
        /// original date if the day is already correct.
        /// </summary>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the next occurrence of the
        /// specified day-of-week, or the original date if the day is already correct.</returns>
        public static Func<LocalDate, LocalDate> NextOrSame(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }
            return date => date.DayOfWeek == dayOfWeek ? date : date.Next(dayOfWeek);
        }

        /// <summary>
        /// A date adjuster to move to the previous specified day-of-week, but return the
        /// original date if the day is already correct.
        /// </summary>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the previous occurrence of the
        /// specified day-of-week, or the original date if the day is already correct.</returns>
        public static Func<LocalDate, LocalDate> PreviousOrSame(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }
            return date => date.DayOfWeek == dayOfWeek ? date : date.Previous(dayOfWeek);
        }

        /// <summary>
        /// A date adjuster to move to the next specified day-of-week, adding
        /// a week if the day is already correct.
        /// </summary>
        /// <remarks>
        /// This is the adjuster equivalent of <see cref="LocalDate.Next"/>.
        /// </remarks>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the next occurrence of the
        /// specified day-of-week.</returns>
        public static Func<LocalDate, LocalDate> Next(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }
            return date => date.Next(dayOfWeek);
        }

        /// <summary>
        /// A date adjuster to move to the previous specified day-of-week, subtracting
        /// a week if the day is already correct.
        /// </summary>
        /// <remarks>
        /// This is the adjuster equivalent of <see cref="LocalDate.Previous"/>.
        /// </remarks>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the previous occurrence of the
        /// specified day-of-week.</returns>
        public static Func<LocalDate, LocalDate> Previous(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException(nameof(dayOfWeek));
            }
            return date => date.Previous(dayOfWeek);
        }

        /// <summary>
        /// Creates a date adjuster to add the specified period to the date.
        /// </summary>
        /// <remarks>
        /// This is the adjuster equivalent of <see cref="LocalDate.Plus(Period)"/>.
        /// </remarks>
        /// <param name="period">The period to add when the adjuster is invoked. Must not contain any (non-zero) time units.</param>
        /// <returns>An adjuster which adds the specified period.</returns>
        public static Func<LocalDate, LocalDate> AddPeriod(Period period)
        {
            Preconditions.CheckNotNull(period, nameof(period));
            // Perform this validation eagerly. It will be performed on each invocation as well,
            // but it's good to throw an exception now rather than waiting for the first invocation.
            Preconditions.CheckArgument(!period.HasTimeComponent, nameof(period), "Cannot add a period with a time component to a date");
            return date => date + period;
        }
    }
}
