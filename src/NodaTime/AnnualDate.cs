// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Calendars;
using NodaTime.Utility;
using System;
using System.Globalization;

namespace NodaTime
{
    /// <summary>
    /// Represents an annual date (month and day) in the ISO calendar but without a specific year,
    /// typically for recurrent events such as birthdays, anniversaries, and deadlines.
    /// </summary>
    /// <remarks>In the future, this struct may be expanded to support other calendar systems,
    /// but this does not generalize terribly cleanly, particularly to the Hebrew calendar system
    /// with its leap month.</remarks>
    public struct AnnualDate : IEquatable<AnnualDate>, IComparable<AnnualDate>
    {
        // The underlying value, which will have a year of 2000, to ensure validity in the face
        // of February 29th.
        private readonly YearMonthDay value;

        /// <summary>
        /// Constructs an instance for the given month and day in the ISO calendar.
        /// </summary>
        /// <param name="month">The month of year.</param>
        /// <param name="day">The day of month.</param>
        public AnnualDate(int month, int day)
        {
            GregorianYearMonthDayCalculator.ValidateGregorianYearMonthDay(2000, month, day);
            value = new YearMonthDay(2000, month, day);
        }

        /// <summary>
        /// Gets the month of year.
        /// </summary>
        /// <value>The month of year.</value>
        public int Month => value.Month;

        /// <summary>
        /// Gets the day of month.
        /// </summary>
        /// <value>The day of month.</value>
        public int Day => value.Day;

        /// <summary>
        /// Returns this annual date in a particular year, as a <see cref="LocalDate"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value represents February 29th, and the specified year is not a leap
        /// year, the returned value will be February 28th of that year.
        /// </para>
        /// </remarks>
        /// <param name="year">The year component of the required date.</param>
        /// <returns>A date in the given year, suitable for this annual date.</returns>
        [Pure]
        public LocalDate InYear(int year)
        {
            Preconditions.CheckArgumentRange(nameof(year), year,
                GregorianYearMonthDayCalculator.MinGregorianYear,
                GregorianYearMonthDayCalculator.MaxGregorianYear);
            var ymd  = CalendarSystem.Iso.YearMonthDayCalculator.SetYear(value, year);
            return new LocalDate(ymd.WithCalendarOrdinal(0)); // ISO calendar
        }

        /// <summary>
        /// Compares this annual date with the specified reference. An annual date is
        /// only equal to another annual date with the same month and day values.
        /// </summary>
        /// <param name="obj">The object to compare this one with</param>
        /// <returns>True if the specified value is an annual date which is equal to this one; false otherwise</returns>

        public override bool Equals(object obj) => obj is AnnualDate && Equals((AnnualDate)obj);

        /// <summary>
        /// Returns a hash code for this annual date.
        /// </summary>
        /// <returns>A hash code for this annual date.</returns>
        public override int GetHashCode()
        {
            return value.RawValue;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance, in the form MM-dd.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:00}-{1:00}", value.Month, value.Day);
        }

        /// <summary>
        /// Compares this annual date with the specified one for equality,
        /// by checking whether the two values represent the same annual date - the same month and day.
        /// </summary>
        /// <param name="other">The other annual date to compare this one with</param>
        /// <returns>True if the specified annual date is equal to this one; false otherwise</returns>
        public bool Equals(AnnualDate other)
        {
            return value == other.value;
        }

        /// <summary>
        /// Indicates whether this annual date is earlier, later or the same as another one.
        /// </summary>
        /// <param name="other">The other annual date to compare this one with</param>
        /// <returns>A value less than zero if this annual date is earlier than <paramref name="other"/>;
        /// zero if this time is the same as <paramref name="other"/>; a value greater than zero if this annual date is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(AnnualDate other)
        {
            return value.CompareTo(other.value);
        }

        // TODO(code review): Overload ==, !=, <=, >=, < and >? Feels a bit like overkill.
        // (And what about serialization? Suggest not...)
    }
}
