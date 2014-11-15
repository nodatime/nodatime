// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// An interval between two dates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The two dates must be in the same calendar, and the end date must not be earlier than the start date.
    /// </para>
    /// <para>
    /// By default, the end date is deemed to be part of the range, as this matches many real life uses of
    /// date ranges. For example, if someone says "I'm going to be on holiday from Monday to Friday," they
    /// usually mean that Friday is part of their holiday. This can be configured via a constructor parameter,
    /// as occasionally an exclusive end date can be useful. For example, to create an interval covering a
    /// whole month, you can simply provide the first day of the month as the start and the first day of the
    /// next month as the exclusive end.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class DateInterval
    {
        /// <summary>
        /// Gets the start date of the interval, which is always included in the interval.
        /// </summary>
        /// <value>The start date of the interval.</value>
        public LocalDate Start { get; }

        /// <summary>
        /// Gets the end date of the interval.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="Inclusive"/> property to determine whether or not the end
        /// date is considered part of the interval.
        /// </remarks>
        /// <value>The end date of the interval.</value>
        public LocalDate End { get; }

        /// <summary>
        /// Indicates whether or not this interval includes its end date.
        /// </summary>
        /// <value>Whether or not this </value>
        public bool Inclusive { get; }

        /// <summary>
        /// Constructs a date interval from a start date and an end date, and an indication
        /// of whether the end date should be included in the interval.
        /// </summary>
        /// <param name="start">Start date of the interval</param>
        /// <param name="end">End date of the interval</param>
        /// <param name="inclusive"><c>true</c> to include the end date in the interval;
        /// <c>false</c> to exclude it.
        /// </param>
        /// <exception cref="ArgumentException"><paramref name="end"/> is earlier than <paramref name="start"/>
        /// or the two dates are in different calendars.
        /// </exception>
        /// <returns>A date interval between the specified dates, with the specified inclusivity.</returns>
        public DateInterval(LocalDate start, LocalDate end, bool inclusive)
        {
            Preconditions.CheckArgument(start.Calendar.Equals(end.Calendar), "end",
                "Calendars of start and end dates must be the same.");
            Preconditions.CheckArgument(!(end < start), "end", "End date must not be earlier than the start date");
            this.Start = start;
            this.End = end;
            this.Inclusive = inclusive;
        }

        /// <summary>
        /// Constructs a date interval from a start date and an inclusive end date.
        /// </summary>
        /// <param name="start">Start date of the interval</param>
        /// <param name="end">End date of the interval, inclusive</param>
        /// <exception cref="ArgumentException"><paramref name="end"/> is earlier than <paramref name="start"/>
        /// or the two dates are in different calendars.
        /// </exception>
        /// <returns>An inclusive date interval between the specified dates.</returns>
        public DateInterval(LocalDate start, LocalDate end)
            : this(start, end, true)
        {
        }

        /// <summary>
        /// Checks whether the given date is within this date interval. This requires
        /// that the date is not earlier than the start date, and not later than the end
        /// date. If the given date is exactly equal to the end date, it is considered
        /// to be within the interval if and only if the interval is <see cref="Inclusive"/>.
        /// </summary>
        /// <param name="date">The date to check for containment within this interval.</param>
        /// <exception cref="ArgumentException"><paramref name="date"/> is not in the same
        /// calendar as the start and end date of this interval.</exception>
        /// <returns><c>true</c> if <paramref name="date"/> is within this interval; <c>false</c> otherwise.</returns>
        public bool Contains(LocalDate date)
        {
            Preconditions.CheckArgument(date.Calendar.Equals(Start.Calendar), "date",
                "The date to check must be in the same calendar as the start and end dates");
            return Start <= date && (Inclusive ? date <= End : date < End);
        }

        /// <summary>
        /// Gets the length of this date interval in days.
        /// </summary>
        /// <remarks>
        /// The end date is included or excluded according to the <see cref="Inclusive"/>
        /// property. For example, an inclusive interval where the start and end date are the
        /// same has a length of 1, whereas an exclusive interval for the same dates has a
        /// length of 0.
        /// </remarks>
        /// <value>The length of this date interval in days.</value>
        public int Length =>
            // Period.Between will give us the exclusive result, so we need to add 1
            // if this period is inclusive.
            Period.Between(Start, End, PeriodUnits.Days).Days + (Inclusive ? 1 : 0);
    }
}
