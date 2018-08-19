// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.Utility;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NodaTime
{
    /// <summary>
    /// A combination of a <see cref="LocalDate"/> and an <see cref="Offset"/>, to represent
    /// a date at a specific offset from UTC but without any time-of-day information.
    /// </summary>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !NETSTANDARD
    [Serializable]
#endif
    public struct OffsetDate : IEquatable<OffsetDate>, IXmlSerializable, IFormattable
#if !NETSTANDARD
        , ISerializable
#endif
    {
        [ReadWriteForEfficiency] private LocalDate date;
        [ReadWriteForEfficiency] private Offset offset;

        /// <summary>
        /// Constructs an instance of the specified date and offset.
        /// </summary>
        /// <param name="date">The date part of the value.</param>
        /// <param name="offset">The offset part of the value.</param>
        public OffsetDate(LocalDate date, Offset offset)
        {
            this.date = date;
            this.offset = offset;
        }

        /// <summary>
        /// Gets the local date represented by this value.
        /// </summary>
        /// <value>The local date represented by this value.</value>
        public LocalDate Date => date;

        /// <summary>
        /// Gets the offset from UTC of this value.
        /// </summary>
        /// <value>The offset from UTC of this value.</value>
        public Offset Offset => offset;

        /// <summary>Gets the calendar system associated with this offset date.</summary>
        /// <value>The calendar system associated with this offset date.</value>
        [NotNull] public CalendarSystem Calendar => Date.Calendar;

        /// <summary>Gets the year of this offset date.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        /// <value>The year of this offset date.</value>
        public int Year => Date.Year;

        /// <summary>Gets the month of this offset date within the year.</summary>
        /// <value>The month of this offset date within the year.</value>
        public int Month => Date.Month;

        /// <summary>Gets the day of this offset date within the month.</summary>
        /// <value>The day of this offset date within the month.</value>
        public int Day => Date.Day;

        /// <summary>
        /// Gets the week day of this offset date expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value.
        /// </summary>
        /// <value>The week day of this offset date expressed as an <c>IsoDayOfWeek</c>.</value>
        public IsoDayOfWeek DayOfWeek => Date.DayOfWeek;

        /// <summary>Gets the year of this offset date within the era.</summary>
        /// <value>The year of this offset date within the era.</value>
        public int YearOfEra => Date.YearOfEra;

        /// <summary>Gets the era of this offset date.</summary>
        /// <value>The era of this offset date.</value>
        [NotNull] public Era Era => Date.Era;

        /// <summary>Gets the day of this offset date within the year.</summary>
        /// <value>The day of this offset date within the year.</value>
        public int DayOfYear => Date.DayOfYear;

        /// <summary>
        /// Creates a new <see cref="OffsetDate"/> for the same date, but with the specified UTC offset.
        /// </summary>
        /// <param name="offset">The new UTC offset.</param>
        /// <returns>A new <c>OffsetDate</c> for the same date, but with the specified UTC offset.</returns>
        [Pure]
        public OffsetDate WithOffset(Offset offset) => new OffsetDate(this.date, offset);

        /// <summary>
        /// Returns this offset date, with the given date adjuster applied to it, maintaining the existing offset.
        /// </summary>
        /// <remarks>
        /// If the adjuster attempts to construct an
        /// invalid date (such as by trying to set a day-of-month of 30 in February), any exception thrown by
        /// that construction attempt will be propagated through this method.
        /// </remarks>
        /// <param name="adjuster">The adjuster to apply.</param>
        /// <returns>The adjusted offset date.</returns>
        [Pure]
        public OffsetDate With([NotNull] Func<LocalDate, LocalDate> adjuster) =>
            new OffsetDate(Date.With(adjuster), offset);

        /// <summary>
        /// Creates a new <see cref="OffsetDate"/> representing the same physical date and offset, but in a different calendar.
        /// The returned value is likely to have different date field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendar">The calendar system to convert this offset date to.</param>
        /// <returns>The converted <c>OffsetDate</c>.</returns>
        [Pure]
        public OffsetDate WithCalendar([NotNull] CalendarSystem calendar) =>
            new OffsetDate(Date.WithCalendar(calendar), offset);

        /// <summary>
        /// Combines this <see cref="OffsetDate"/> with the given <see cref="LocalTime"/>
        /// into an <see cref="OffsetDateTime"/>.
        /// </summary>
        /// <param name="time">The time to combine with this date.</param>
        /// <returns>The <see cref="OffsetDateTime"/> representation of the given time on this date.</returns>
        [Pure]
        public OffsetDateTime At(LocalTime time) => new OffsetDateTime(Date.At(time), Offset);

        /// <summary>
        /// Returns a hash code for this offset date.
        /// </summary>
        /// <returns>A hash code for this offset date.</returns>
        public override int GetHashCode() => HashCodeHelper.Hash(Date, Offset);

        /// <summary>
        /// Compares two <see cref="OffsetDate"/> values for equality. This requires
        /// that the date values be the same (in the same calendar) and the offsets.
        /// </summary>
        /// <param name="obj">The object to compare this offset date with.</param>
        /// <returns>True if the given value is another offset date equal to this one; false otherwise.</returns>
        public override bool Equals(object obj) => obj is OffsetDate other && Equals(other);

        /// <summary>
        /// Compares two <see cref="OffsetDate"/> values for equality. This requires
        /// that the date values be the same (in the same calendar) and the offsets.
        /// </summary>
        /// <param name="other">The value to compare this offset date with.</param>
        /// <returns>True if the given value is another offset date equal to this one; false otherwise.</returns>
        public bool Equals(OffsetDate other) => Date == other.Date && Offset == other.Offset;

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(OffsetDate left, OffsetDate right) => left.Equals(right);

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(OffsetDate left, OffsetDate right) => !(left == right);

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("G"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() => OffsetDatePattern.Patterns.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("G").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider) =>
            OffsetDatePattern.Patterns.BclSupport.Format(this, patternText, formatProvider);

        /// <summary>
        /// Deconstruct this value into its components.
        /// </summary>
        /// <param name="localDate">The <see cref="LocalDate"/> component.</param>
        /// <param name="offset">The <see cref="Offset"/> component.</param>
        [Pure]
        public void Deconstruct(out LocalDate localDate, out Offset offset)
        {
            localDate = Date;
            offset = Offset;
        }


        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = OffsetDatePattern.GeneralIso;
            if (reader.MoveToAttribute("calendar"))
            {
                string newCalendarId = reader.Value;
                CalendarSystem newCalendar = CalendarSystem.ForId(newCalendarId);
                var newTemplateValue = pattern.TemplateValue.WithCalendar(newCalendar);
                pattern = pattern.WithTemplateValue(newTemplateValue);
                reader.MoveToElement();
            }
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(OffsetDatePattern.GeneralIso.Format(this));
        }
        #endregion

#if !NETSTANDARD
        #region Binary serialization
        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private OffsetDate([NotNull] SerializationInfo info, StreamingContext context)
            : this(new LocalDate(info), new Offset(info))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            date.Serialize(info);
            offset.Serialize(info);
        }
        #endregion
#endif
    }
}
