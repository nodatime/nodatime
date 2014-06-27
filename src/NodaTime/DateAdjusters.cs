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
        public static Func<LocalDate, LocalDate> StartOfMonth { get { return startOfMonth; } }

        /// <summary>
        /// A date adjuster to move to the last day of the current month.
        /// </summary>
        public static Func<LocalDate, LocalDate> EndOfMonth { get { return endOfMonth; } }

        /// <summary>
        /// A date adjuster to move to the specified day of the current month.
        /// </summary>
        public static Func<LocalDate, LocalDate> DayOfMonth(int day)
        {
            return date => new LocalDate(date.Year, date.Month, day, date.Calendar);
        }

        /// <summary>
        /// A date adjuster to move to the specified day of the current month.
        /// </summary>
        public static Func<LocalDate, LocalDate> Month(int month)
        {
            return date => new LocalDate(date.Year, month, date.Day, date.Calendar);
        }
    }
}
