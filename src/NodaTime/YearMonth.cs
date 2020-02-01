// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.Utility;
using System;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static NodaTime.Calendars.GregorianYearMonthDayCalculator;

namespace NodaTime
{
    /// <summary>
    /// A year and month in a particular calendar. This is effectively
    /// <see cref="LocalDate"/> without the day-of-month component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values can freely be compared for equality: a value in a different calendar system is not equal to
    /// a value in a different calendar system. However, ordering comparisons
    /// fail with <see cref="ArgumentException"/>; attempting to compare values in different calendars
    /// almost always indicates a bug in the calling code.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public struct YearMonth : IEquatable<YearMonth>, IComparable<YearMonth>, IComparable, IFormattable, IXmlSerializable
    {
        /// <summary>
        /// The start of month. This is used as our base representation as we already have
        /// plenty of other code that integrates it, and it implements a compact representation
        /// without us having to duplicate any of the logic.
        /// </summary>
        private readonly YearMonthDayCalendar startOfMonth;

        /// <summary>Gets the calendar system associated with this year/month.</summary>
        /// <value>The calendar system associated with this year/month.</value>
        public CalendarSystem Calendar => CalendarSystem.ForOrdinal(startOfMonth.CalendarOrdinal);

        /// <summary>Gets the year of this year/month.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        /// <value>The year of this year/month.</value>
        public int Year => startOfMonth.Year;

        /// <summary>Gets the month of this year/month within the year.</summary>
        /// <value>The month of this year/month within the year.</value>
        public int Month => startOfMonth.Month;

        /// <summary>Gets the year of this value within the era.</summary>
        /// <value>The year of this value within the era.</value>
        public int YearOfEra => Calendar.GetYearOfEra(startOfMonth.Year);

        /// <summary>Gets the era of this year/month.</summary>
        /// <value>The era of this year/month.</value>
        public Era Era => Calendar.GetEra(startOfMonth.Year);

        // Note: we could easily make these properties public later if we wanted.
        /// <summary>
        /// Returns the date of the start of this year/month.
        /// </summary>
        internal LocalDate StartDate => new LocalDate(startOfMonth);

        /// <summary>
        /// Returns the date of the end of this year/month.
        /// </summary>
        internal LocalDate EndDate => new LocalDate(Year, Month, Calendar.GetDaysInMonth(Year, Month), Calendar);

        private YearMonthDay YearMonthDay => startOfMonth.ToYearMonthDay();

        /// <summary>
        /// Constructs an instance for the given year and month in the ISO calendar.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <returns>The resulting year/month.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid year/month.</exception>
        public YearMonth(int year, int month)
        {
            // Note: not delegating to (year, month, calendar) to optimize performance.
            if (year < MinGregorianYear || year > MaxGregorianYear || month < 1 || month > 12)
            {
                Preconditions.CheckArgumentRange(nameof(year), year, MinGregorianYear, MaxGregorianYear);
                Preconditions.CheckArgumentRange(nameof(month), month, 1, 12);
            }
            startOfMonth = new YearMonthDayCalendar(year, month, 1, CalendarOrdinal.Iso);
        }

        /// <summary>
        /// Constructs an instance for the given year, month and day in the specified calendar.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year", so, for
        /// the ISO calendar, a value of 0 means 1 BC, for example.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="calendar">Calendar system in which to create the year/month.</param>
        /// <returns>The resulting year/month.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid year/month.</exception>
        public YearMonth(int year, int month, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            calendar.ValidateYearMonthDay(year, month, 1);
            startOfMonth = new YearMonthDayCalendar(year, month, 1, calendar.Ordinal);
        }

        /// <summary>
        /// Constructs an instance for the given era, year of era and month in the ISO calendar.
        /// </summary>
        /// <param name="era">The era within which to create a year/month. Must be a valid era within the ISO calendar.</param>
        /// <param name="yearOfEra">The year of era.</param>
        /// <param name="month">The month of year.</param>
        /// <returns>The resulting year/month.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid year/month.</exception>
        public YearMonth(Era era, int yearOfEra, int month)
            : this(era, yearOfEra, month, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Constructs an instance for the given era, year of era and month in the specified calendar.
        /// </summary>
        /// <param name="era">The era within which to create a year/month. Must be a valid era within the specified calendar.</param>
        /// <param name="yearOfEra">The year of era.</param>
        /// <param name="month">The month of year.</param>
        /// <param name="calendar">Calendar system in which to create the year/month.</param>
        /// <returns>The resulting year/month.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The parameters do not form a valid year/month.</exception>
        public YearMonth(Era era, int yearOfEra, int month, CalendarSystem calendar)
            : this(Preconditions.CheckNotNull(calendar, nameof(calendar)).GetAbsoluteYear(yearOfEra, era), month, calendar)
        {
        }

        /// <summary>
        /// Returns a <see cref="DateInterval"/> covering the month represented by this value.
        /// </summary>
        /// <returns>A <see cref="DateInterval"/> covering the month represented by this value.</returns>
        [Pure]
        public DateInterval ToDateInterval() => new DateInterval(StartDate, EndDate);

        /// <summary>
        /// Returns a <see cref="LocalDate"/> with the year/month of this value, and the given day of month.
        /// </summary>
        /// <param name="day">The day within the month.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="day"/> does not fall within the
        /// month represented by this value.</exception>
        /// <returns>The result of combining this year and month with <paramref name="day"/>.</returns>
        [Pure]
        public LocalDate OnDayOfMonth(int day)
        {
            Preconditions.CheckArgumentRange(nameof(day), day, 1, Calendar.GetDaysInMonth(Year, Month));
            return new LocalDate(Year, Month, day, Calendar);
        }

        /// <summary>
        /// Indicates whether this year/month is earlier, later or the same as another one.
        /// See the type documentation for a description of ordering semantics.
        /// </summary>
        /// <param name="other">The other year/month to compare this one with</param>
        /// <exception cref="ArgumentException">The calendar system of <paramref name="other"/> is not the
        /// same as the calendar system of this value.</exception>
        /// <returns>A value less than zero if this year/month is earlier than <paramref name="other"/>;
        /// zero if this year/month is the same as <paramref name="other"/>; a value greater than zero if this date is
        /// later than <paramref name="other"/>.</returns>
        public int CompareTo(YearMonth other)
        {
            Preconditions.CheckArgument(Calendar.Equals(other.Calendar), nameof(other), "Only values with the same calendar system can be compared");
            return Calendar.Compare(YearMonthDay, other.YearMonthDay);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two YearMonth values.
        /// See the type documentation for a description of ordering semantics.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="YearMonth"/>, or refers
        /// to a value in a different calendar system.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this YearMonth with another one.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj is null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is YearMonth, nameof(obj), "Object must be of type NodaTime.YearMonth.");
            return CompareTo((YearMonth) obj);
        }

        /// <summary>
        /// Returns a hash code for this year/month.
        /// See the type documentation for a description of equality semantics.
        /// </summary>
        /// <returns>A hash code for this value.</returns>
        public override int GetHashCode() => startOfMonth.GetHashCode();

        /// <summary>
        /// Compares two <see cref="YearMonth"/> values for equality.
        /// See the type documentation for a description of equality semantics.
        /// </summary>
        /// <param name="obj">The object to compare this year/month with.</param>
        /// <returns>True if the given value is another year/month equal to this one; false otherwise.</returns>
        public override bool Equals(object? obj) => obj is YearMonth other && this == other;

        /// <summary>
        /// Compares two <see cref="YearMonth"/> values for equality.
        /// See the type documentation for a description of ordering semantics.
        /// </summary>
        /// <param name="other">The value to compare this year/month with.</param>
        /// <returns>True if the given value is another year/month equal to this one; false otherwise.</returns>
        public bool Equals(YearMonth other) => this == other;

        /// <summary>
        /// Compares two <see cref="YearMonth" /> values for equality.
        /// See the type documentation for a description of equality semantics.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>True if the two year/month values are the same and in the same calendar; false otherwise</returns>
        public static bool operator ==(YearMonth lhs, YearMonth rhs) => lhs.startOfMonth == rhs.startOfMonth;

        /// <summary>
        /// Compares two <see cref="YearMonth" /> values for inequality.
        /// See the type documentation for a description of equality semantics.
        /// </summary>
        /// <param name="lhs">The first value to compare</param>
        /// <param name="rhs">The second value to compare</param>
        /// <returns>False if the two year/month values are the same and in the same calendar; true otherwise</returns>
        public static bool operator !=(YearMonth lhs, YearMonth rhs) => !(lhs == rhs);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("D").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string? patternText, IFormatProvider? formatProvider) =>
            YearMonthPattern.BclSupport.Format(this, patternText, formatProvider);

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null!; // TODO(nullable): Return XmlSchema? when docfx works with that

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = YearMonthPattern.Iso;
            if (reader.MoveToAttribute("calendar"))
            {
                string newCalendarId = reader.Value;
                CalendarSystem newCalendar = CalendarSystem.ForId(newCalendarId);
                // We don't have WithCalendar in YearMonth, because that would be odd. Instead,
                // let's take the start of the default template value, convert that to the
                // target calendar, then use the year/month of the result as the template value.
                var newTemplateValue = YearMonthPattern.DefaultTemplateValue.StartDate.WithCalendar(newCalendar).ToYearMonth();
                pattern = pattern.WithTemplateValue(newTemplateValue);
                reader.MoveToElement();
            }
            string text = reader.ReadElementContentAsString();
            Unsafe.AsRef(this) = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(YearMonthPattern.Iso.Format(this));
        }
        #endregion
    }
}
