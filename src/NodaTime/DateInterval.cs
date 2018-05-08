// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Text;
using NodaTime.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

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
    public sealed class DateInterval : IEquatable<DateInterval>, IEnumerable<LocalDate>
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
        /// Date intervals are equal if they have the same start and end dates.
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
        /// Date intervals are equal if they have the same start and end dates.
        /// </remarks>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>False if the two date intervals have the same start and end date; true otherwise.</returns>
        public static bool operator !=(DateInterval lhs, DateInterval rhs) => !(lhs == rhs);

        /// <summary>
        /// Compares the given date interval for equality with this one.
        /// </summary>
        /// <remarks>
        /// Date intervals are equal if they have the same start and end dates.
        /// </remarks>
        /// <param name="other">The date interval to compare this one with.</param>
        /// <returns>True if this date interval has the same same start and end date as the one specified.</returns>
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
        /// Checks whether the given interval is within this interval. This requires that the start date of the specified
        /// interval is not earlier than the start date of this interval, and the end date of the specified interval is not
        /// later than the end date of this interval.
        /// </summary>
        /// <remarks>
        /// An interval contains another interval with same start and end dates, or itself.
        /// </remarks>
        /// <param name="interval">The interval to check for containment within this interval.</param>
        /// <exception cref="ArgumentException"><paramref name="interval" /> uses a different
        /// calendar to this date interval.</exception>
        /// <returns><c>true</c> if <paramref name="interval"/> is within this interval; <c>false</c> otherwise.</returns>
        public bool Contains([NotNull] DateInterval interval)
        {
            ValidateInterval(interval);
            return Contains(interval.Start) && Contains(interval.End);
        }

        /// <summary>
        /// Gets the length of this date interval in days. This will always be at least 1.
        /// </summary>
        /// <value>The length of this date interval in days.</value>
        public int Length =>
            // Period.DaysBetween will give us the exclusive result, so we need to add 1
            // to include the end date.
            Period.DaysBetween(Start, End) + 1;

        /// <summary>
        /// Gets the calendar system of the dates in this interval.
        /// </summary>
        /// <value>The calendar system of the dates in this interval.</value>
        [NotNull]
        public CalendarSystem Calendar => Start.Calendar;

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

        /// <summary>
        /// Deconstruct this date interval into its components.
        /// </summary>
        /// <param name="start">The <see cref="LocalDate"/> representing the start of the interval.</param>
        /// <param name="end">The <see cref="LocalDate"/> representing the end of the interval.</param>
        public void Deconstruct(out LocalDate start, out LocalDate end)
        {
            start = Start;
            end = End;
        }

        /// <summary>
        /// Returns the intersection between the given interval and this interval.
        /// </summary>
        /// <param name="interval">
        /// The specified interval to intersect with this one.
        /// </param>
        /// <returns>
        /// A <see cref="DateInterval"/> corresponding to the intersection between the given interval and the current
        /// instance. If there is no intersection, a null reference is returned.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="interval" /> uses a different
        /// calendar to this date interval.</exception>
        [CanBeNull]
        public DateInterval Intersection([NotNull]DateInterval interval)
        {
            return Contains(interval) ? interval
                : interval.Contains(this) ? this
                : interval.Contains(Start) ? new DateInterval(Start, interval.End)
                : interval.Contains(End) ? new DateInterval(interval.Start, End)
                : null;
        }

        /// <summary>
        /// Returns the union between the given interval and this interval, as long as they're overlapping or contiguous.
        /// </summary>
        /// <param name="interval">The specified interval from which to generate the union interval.</param>
        /// <returns>
        /// A <see cref="DateInterval"/> corresponding to the union between the given interval and the current
        /// instance, in the case the intervals overlap or are contiguous; a null reference otherwise.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="interval" /> uses a different calendar to this date interval.</exception>
        [CanBeNull]
        public DateInterval Union([NotNull] DateInterval interval)
        {
            ValidateInterval(interval);

            var start = LocalDate.Min(Start, interval.Start);
            var end = LocalDate.Max(End, interval.End);

            // Check whether the length of the interval we *would* construct is greater
            // than the sum of the lengths - if it is, there's a day in that candidate union
            // that isn't in either interval. Note the absence of "+ 1" and the use of >=
            // - it's equivalent to Period.DaysBetween(...) + 1 > Length + interval.Length,
            // but with fewer operations.
            return Period.DaysBetween(start, end) >= Length + interval.Length
                ? null
                : new DateInterval(start, end);
        }

        private void ValidateInterval(DateInterval interval)
        {
            Preconditions.CheckNotNull(interval, nameof(interval));
            Preconditions.CheckArgument(interval.Calendar.Equals(Start.Calendar), nameof(interval),
                "The specified interval uses a different calendar system to this one");
        }

        /// <summary>
        /// Returns an enumerator for the dates in the interval, including both <see cref="Start"/> and <see cref="End"/>.
        /// </summary>
        /// <returns>An enumerator for the interval.</returns>
        [NotNull]
        public IEnumerator<LocalDate> GetEnumerator()
        {
            // Stop when we know we've reach End, and then yield that.
            // We can't use a <= condition, as otherwise we'd try to create a date past End, which may be invalid.
            // We could use < but that's significantly less efficient than !=
            // We know that adding a day at a time we'll eventually reach End (because they're validated to be in the same calendar
            // system, with Start <= End), so that's the simplest way to go.
            for (var date = Start; date != End; date = date.PlusDays(1))
            {
                yield return date;
            }
            yield return End;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}