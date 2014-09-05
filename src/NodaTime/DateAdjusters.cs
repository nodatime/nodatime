// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime
{
    /// <summary>
    /// Factory class for time adjusters: functions from <see cref="LocalDate"/> to <c>LocalTime</c>,
    /// which can be applied to <see cref="LocalDate"/>, <see cref="LocalDateTime"/>, and <see cref="OffsetDateTime"/>.
    /// </summary>
    public static class DateAdjusters
    {
        private static readonly Func<LocalDate, LocalDate> startOfMonth = date => new LocalDate(date.Year, date.Month, 1, date.Calendar);
        private static readonly Func<LocalDate, LocalDate> endOfMonth = date => new LocalDate(date.Year, date.Month, date.Calendar.GetDaysInMonth(date.Year, date.Month), date.Calendar);

        /// <summary>
        /// A date adjuster to move to the first day of the current month.
        /// </summary>
        /// <value>
        /// A date adjuster to move to the first day of the current month.
        /// </value>
        public static Func<LocalDate, LocalDate> StartOfMonth { get { return startOfMonth; } }

        /// <summary>
        /// A date adjuster to move to the last day of the current month.
        /// </summary>
        /// <value>
        /// A date adjuster to move to the last day of the current month.
        /// </value>
        public static Func<LocalDate, LocalDate> EndOfMonth { get { return endOfMonth; } }

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
        public static Func<LocalDate, LocalDate> DayOfMonth(int day)
        {
            return date => new LocalDate(date.Year, date.Month, day, date.Calendar);
        }

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
        public static Func<LocalDate, LocalDate> Month(int month)
        {
            return date => new LocalDate(date.Year, month, date.Day, date.Calendar);
        }

        /// <summary>
        /// A date adjuster to move to the next specified day-of-week, but return the
        /// original date if the day is already correct.
        /// </summary>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the next occurrence of the
        /// specified day-of-week, or the original date if the day is already corret.</returns>
        public static Func<LocalDate, LocalDate> NextOrSame(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException("dayOfWeek");
            }
            return date => date.IsoDayOfWeek == dayOfWeek ? date : date.Next(dayOfWeek);
        }

        /// <summary>
        /// A date adjuster to move to the previous specified day-of-week, but return the
        /// original date if the day is already correct.
        /// </summary>
        /// <param name="dayOfWeek">The day-of-week to adjust dates to.</param>
        /// <returns>An adjuster which advances a date to the previous occurrence of the
        /// specified day-of-week, or the original date if the day is already corret.</returns>
        public static Func<LocalDate, LocalDate> PreviousOrSame(IsoDayOfWeek dayOfWeek)
        {
            // Avoids boxing...
            if (dayOfWeek < IsoDayOfWeek.Monday || dayOfWeek > IsoDayOfWeek.Sunday)
            {
                throw new ArgumentOutOfRangeException("dayOfWeek");
            }
            return date => date.IsoDayOfWeek == dayOfWeek ? date : date.Previous(dayOfWeek);
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
                throw new ArgumentOutOfRangeException("dayOfWeek");
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
                throw new ArgumentOutOfRangeException("dayOfWeek");
            }
            return date => date.Previous(dayOfWeek);
        }
    }
}
