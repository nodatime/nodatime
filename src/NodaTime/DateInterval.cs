// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Text;
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
    /// The end date is deemed to be part of the range, as this matches many real life uses of
    /// date ranges. For example, if someone says "I'm going to be on holiday from Monday to Friday," they
    /// usually mean that Friday is part of their holiday.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class DateInterval : IEquatable<DateInterval>
    {
        /// <summary>
        /// Gets the start date of the interval.
        /// </summary>
        /// <value>The start date of the interval.</value>
        public LocalDate Start { get; }

        /// <summary>
        /// Gets the end date of the interval.
        /// </summary>
        /// <value>The end date of the interval.</value>
        public LocalDate End { get; }

        /// <summary>
        /// Constructs a date interval from a start date and an end date, both of which are included
        /// in the interval.
        /// </summary>
        /// <param name="start">Start date of the interval</param>
        /// <param name="end">End date of the interval</param>
        /// <exception cref="ArgumentException"><paramref name="end"/> is earlier than <paramref name="start"/>
        /// or the two dates are in different calendars.
        /// </exception>
        /// <returns>A date interval between the specified dates.</returns>
        public DateInterval(LocalDate start, LocalDate end)
        {
            Preconditions.CheckArgument(start.Calendar.Equals(end.Calendar), nameof(end),
                "Calendars of start and end dates must be the same.");
            Preconditions.CheckArgument(!(end < start), nameof(end), "End date must not be earlier than the start date");
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Returns the hash code for this interval, consistent with <see cref="Equals(DateInterval)"/>.
        /// </summary>
        /// <returns>The hash code for this interval.</returns>
        public override int GetHashCode() =>
            HashCodeHelper.Initialize()
                .Hash(Start)
                .Hash(End)
                .Value;

        /// <summary>
        /// Compares two <see cref="DateInterval" /> values for equality.
        /// </summary>
        /// <remarks>
        /// Date intervals are equal if they have the same start and end dates and are both end-inclusive or both end-exclusive:
        /// an end-exclusive date interval of [2001-01-01, 2001-02-01) is not equal to the end-inclusive date interval of
        /// [2001-01-01, 2001-01-31], even though both contain the same range of dates.
        /// </remarks>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>True if the two date intervals have the same properties; false otherwise.</returns>
        public static bool operator ==(DateInterval lhs, DateInterval rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
            {
                return false;
            }
            return lhs.Start == rhs.Start && lhs.End == rhs.End;
        }

        /// <summary>
        /// Compares two <see cref="DateInterval" /> values for inequality.
        /// </summary>
        /// <remarks>
        /// Date intervals are equal if they have the same start and end dates and are both end-inclusive or both exclusive:
        /// an end-exclusive date interval of [2001-01-01, 2001-02-01) is not equal to the end-inclusive date interval of
        /// [2001-01-01, 2001-01-31], even though both contain the same range of dates.
        /// </remarks>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>False if the two date intervals have the same properties; true otherwise.</returns>
        public static bool operator !=(DateInterval lhs, DateInterval rhs) => !(lhs == rhs);

        /// <summary>
        /// Compares the given date interval for equality with this one.
        /// </summary>
        /// <remarks>
        /// Date intervals are equal if they have the same start and end dates and are both end-inclusive or both end-exclusive:
        /// an end-exclusive date interval of [2001-01-01, 2001-02-01) is not equal to the end-inclusive date interval of
        /// [2001-01-01, 2001-01-31], even though both contain the same range of dates.
        /// </remarks>
        /// <param name="other">The date interval to compare this one with.</param>
        /// <returns>True if this date interval has the same properties as the one specified.</returns>
        public bool Equals(DateInterval other) => this == other;

        /// <summary>
        /// Compares the given object for equality with this one, as per <see cref="Equals(DateInterval)"/>.
        /// </summary>
        /// <param name="obj">The value to compare this one with.</param>
        /// <returns>true if the other object is a date interval equal to this one, consistent with <see cref="Equals(DateInterval)"/>.</returns>
        public override bool Equals(object obj) => this == (obj as DateInterval);

        /// <summary>
        /// Checks whether the given date is within this date interval. This requires
        /// that the date is not earlier than the start date, and not later than the end
        /// date.
        /// </summary>
        /// <param name="date">The date to check for containment within this interval.</param>
        /// <exception cref="ArgumentException"><paramref name="date"/> is not in the same
        /// calendar as the start and end date of this interval.</exception>
        /// <returns><c>true</c> if <paramref name="date"/> is within this interval; <c>false</c> otherwise.</returns>
        public bool Contains(LocalDate date)
        {
            Preconditions.CheckArgument(date.Calendar.Equals(Start.Calendar), nameof(date),
                "The date to check must be in the same calendar as the start and end dates");
            return Start <= date && date <= End;
        }

        /// <summary>
        /// Gets the length of this date interval in days. This will always be at least 1.
        /// </summary>
        /// <value>The length of this date interval in days.</value>
        public int Length =>
            // Period.Between will give us the exclusive result, so we need to add 1
            // to include the end date.
            Period.Between(Start, End, PeriodUnits.Days).Days + 1;

        /// <summary>
        /// Returns a string representation of this interval.
        /// </summary>
        /// <returns>
        /// A string representation of this interval, as <c>[start, end]</c>,
        /// where "start" and "end" are the dates formatted using an ISO-8601 compatible pattern.
        /// </returns>
        public override string ToString()
        {
            string start = LocalDatePattern.Iso.Format(Start);
            string end = LocalDatePattern.Iso.Format(End);
            return $"[{start}, {end}]";
        }
    }
}
