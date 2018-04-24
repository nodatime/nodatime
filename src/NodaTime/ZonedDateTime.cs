// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NodaTime
{
    // Note: documentation that refers to the LocalDateTime type within this class must use the fully-qualified
    // reference to avoid being resolved to the LocalDateTime property instead.

    /// <summary>
    /// A <see cref="T:NodaTime.LocalDateTime" /> in a specific time zone and with a particular offset to distinguish
    /// between otherwise-ambiguous instants. A <see cref="ZonedDateTime"/> is global, in that it maps to a single
    /// <see cref="Instant"/>.
    /// </summary>
    /// <remarks>
    /// <para>Although <see cref="ZonedDateTime" /> includes both local and global concepts, it only supports
    /// duration-based - and not calendar-based - arithmetic. This avoids ambiguities
    /// and skipped date/time values becoming a problem within a series of calculations; instead,
    /// these can be considered just once, at the point of conversion to a <see cref="ZonedDateTime"/>.
    /// </para>
    /// <para>
    /// <c>ZonedDateTime</c> does not implement ordered comparison operators, as there is no obvious natural ordering that works in all cases. 
    /// Equality is supported however, requiring equality of zone, calendar and date/time. If you want to sort <c>ZonedDateTime</c>
    /// values, you should explicitly choose one of the orderings provided via the static properties in the
    /// <see cref="ZonedDateTime.Comparer"/> nested class (or implement your own comparison).
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !NETSTANDARD1_3
    [Serializable]
#endif
    public struct ZonedDateTime : IEquatable<ZonedDateTime>, IFormattable, IXmlSerializable
#if !NETSTANDARD1_3
        , ISerializable
#endif
    {
        [ReadWriteForEfficiency] private OffsetDateTime offsetDateTime;
        private readonly DateTimeZone zone;

        /// <summary>
        /// Internal constructor from pre-validated values.
        /// </summary>
        internal ZonedDateTime(OffsetDateTime offsetDateTime, DateTimeZone zone)
        {
            this.offsetDateTime = offsetDateTime;
            this.zone = zone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime"/> struct.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        /// <param name="calendar">The calendar system.</param>
        public ZonedDateTime(Instant instant, [NotNull] DateTimeZone zone, [NotNull] CalendarSystem calendar)
        {
            this.zone = Preconditions.CheckNotNull(zone, nameof(zone));
            offsetDateTime = new OffsetDateTime(instant, zone.GetUtcOffset(instant), Preconditions.CheckNotNull(calendar, nameof(calendar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime" /> struct in the specified time zone
        /// and the ISO calendar.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        public ZonedDateTime(Instant instant, [NotNull] DateTimeZone zone)
        {
            this.zone = Preconditions.CheckNotNull(zone, nameof(zone));
            offsetDateTime = new OffsetDateTime(instant, zone.GetUtcOffset(instant));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime"/> struct in the specified time zone
        /// from a given local time and offset. The offset is validated to be correct as part of initialization.
        /// In most cases a local time can only map to a single instant anyway, but the offset is included here for cases
        /// where the local time is ambiguous, usually due to daylight saving transitions.
        /// </summary>
        /// <param name="localDateTime">The local date and time.</param>
        /// <param name="zone">The time zone.</param>
        /// <param name="offset">The offset between UTC and local time at the desired instant.</param>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is not a valid offset at the given
        /// local date and time.</exception>
        public ZonedDateTime(LocalDateTime localDateTime, [NotNull] DateTimeZone zone, Offset offset)
        {
            this.zone = Preconditions.CheckNotNull(zone, nameof(zone));
            Instant candidateInstant = localDateTime.ToLocalInstant().Minus(offset);
            Offset correctOffset = zone.GetUtcOffset(candidateInstant);
            // Not using Preconditions, to avoid building the string unnecessarily.
            if (correctOffset != offset)
            {
                throw new ArgumentException("Offset " + offset + " is invalid for local date and time " + localDateTime
                    + " in time zone " + zone.Id, nameof(offset));

            }
            offsetDateTime = new OffsetDateTime(localDateTime, offset);
        }

        /// <summary>Gets the offset of the local representation of this value from UTC.</summary>
        /// <value>The offset of the local representation of this value from UTC.</value>
        public Offset Offset => offsetDateTime.Offset;

        /// <summary>Gets the time zone associated with this value.</summary>
        /// <value>The time zone associated with this value.</value>
        [NotNull] public DateTimeZone Zone => zone ?? DateTimeZone.Utc;

        /// <summary>
        /// Gets the local date and time represented by this zoned date and time.
        /// </summary>
        /// <remarks>
        /// The returned
        /// <see cref="T:NodaTime.LocalDateTime"/> will have the same calendar system and return the same values for
        /// each of the calendar properties (Year, MonthOfYear and so on), but will not be associated with any
        /// particular time zone.
        /// </remarks>
        /// <value>The local date and time represented by this zoned date and time.</value>
        public LocalDateTime LocalDateTime => offsetDateTime.LocalDateTime;

        /// <summary>Gets the calendar system associated with this zoned date and time.</summary>
        /// <value>The calendar system associated with this zoned date and time.</value>
        [NotNull] public CalendarSystem Calendar => offsetDateTime.Calendar;

        /// <summary>
        /// Gets the local date represented by this zoned date and time.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="LocalDate"/>
        /// will have the same calendar system and return the same values for each of the date-based calendar
        /// properties (Year, MonthOfYear and so on), but will not be associated with any particular time zone.
        /// </remarks>
        /// <value>The local date represented by this zoned date and time.</value>
        public LocalDate Date => offsetDateTime.Date;

        /// <summary>
        /// Gets the time portion of this zoned date and time.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="LocalTime"/> will
        /// return the same values for each of the time-based properties (Hour, Minute and so on), but
        /// will not be associated with any particular time zone.
        /// </remarks>
        /// <value>The time portion of this zoned date and time.</value>
        public LocalTime TimeOfDay => offsetDateTime.TimeOfDay;

        /// <summary>Gets the era for this zoned date and time.</summary>
        /// <value>The era for this zoned date and time.</value>
        [NotNull] public Era Era => offsetDateTime.Era;

        /// <summary>Gets the year of this zoned date and time.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        /// <value>The year of this zoned date and time.</value>
        public int Year => offsetDateTime.Year;

        /// <summary>Gets the year of this zoned date and time within its era.</summary>
        /// <value>The year of this zoned date and time within its era.</value>
        public int YearOfEra => offsetDateTime.YearOfEra;

        /// <summary>Gets the month of this zoned date and time within the year.</summary>
        /// <value>The month of this zoned date and time within the year.</value>
        public int Month => offsetDateTime.Month;

        /// <summary>Gets the day of this zoned date and time within the year.</summary>
        /// <value>The day of this zoned date and time within the year.</value>
        public int DayOfYear => offsetDateTime.DayOfYear;

        /// <summary>
        /// Gets the day of this zoned date and time within the month.
        /// </summary>
        /// <value>The day of this zoned date and time within the month.</value>
        public int Day => offsetDateTime.Day;

        /// <summary>
        /// Gets the week day of this zoned date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value.
        /// </summary>
        /// <value>The week day of this zoned date and time expressed as an <c>IsoDayOfWeek</c> value.</value>
        public IsoDayOfWeek DayOfWeek => offsetDateTime.DayOfWeek;

        /// <summary>
        /// Gets the hour of day of this zoned date and time, in the range 0 to 23 inclusive.
        /// </summary>
        /// <value>The hour of day of this zoned date and time, in the range 0 to 23 inclusive.</value>
        public int Hour => offsetDateTime.Hour;

        /// <summary>
        /// Gets the hour of the half-day of this zoned date and time, in the range 1 to 12 inclusive.
        /// </summary>
        /// <value>The hour of the half-day of this zoned date and time, in the range 1 to 12 inclusive.</value>
        public int ClockHourOfHalfDay => offsetDateTime.ClockHourOfHalfDay;

        /// <summary>
        /// Gets the minute of this zoned date and time, in the range 0 to 59 inclusive.
        /// </summary>
        /// <value>The minute of this zoned date and time, in the range 0 to 59 inclusive.</value>
        public int Minute => offsetDateTime.Minute;

        /// <summary>
        /// Gets the second of this zoned date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        /// <value>The second of this zoned date and time within the minute, in the range 0 to 59 inclusive.</value>
        public int Second => offsetDateTime.Second;

        /// <summary>
        /// Gets the millisecond of this zoned date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        /// <value>The millisecond of this zoned date and time within the second, in the range 0 to 999 inclusive.</value>
        public int Millisecond => offsetDateTime.Millisecond;

        /// <summary>
        /// Gets the tick of this zoned date and time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        /// <value>The tick of this zoned date and time within the second, in the range 0 to 9,999,999 inclusive.</value>
        public int TickOfSecond => offsetDateTime.TickOfSecond;

        /// <summary>
        /// Gets the tick of this zoned date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        /// <remarks>
        /// This is the TickOfDay portion of the contained <see cref="OffsetDateTime"/>.
        /// On daylight saving time transition dates, it may not be the same as the number of ticks elapsed since the beginning of the day.
        /// </remarks>
        /// <value>The tick of this zoned date and time within the day, in the range 0 to 863,999,999,999 inclusive.</value>
        public long TickOfDay => offsetDateTime.TickOfDay;

        /// <summary>
        /// Gets the nanosecond of this zoned date and time within the second, in the range 0 to 999,999,999 inclusive.
        /// </summary>
        /// <value>The nanosecond of this zoned date and time within the second, in the range 0 to 999,999,999 inclusive.</value>
        public int NanosecondOfSecond => offsetDateTime.NanosecondOfSecond;

        /// <summary>
        /// Gets the nanosecond of this zoned date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.
        /// </summary>
        /// <remarks>
        /// This is the NanosecondOfDay portion of the contained <see cref="OffsetDateTime"/>.
        /// On daylight saving time transition dates, it may not be the same as the number of nanoseconds elapsed since the beginning of the day.
        /// </remarks>
        /// <value>The nanosecond of this zoned date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.</value>
        public long NanosecondOfDay => offsetDateTime.NanosecondOfDay;

        /// <summary>
        /// Converts this value to the instant it represents on the time line.
        /// </summary>
        /// <remarks>
        /// This is always an unambiguous conversion. Any difficulties due to daylight saving
        /// transitions or other changes in time zone are handled when converting from a
        /// <see cref="T:NodaTime.LocalDateTime" /> to a <see cref="ZonedDateTime"/>; the <c>ZonedDateTime</c> remembers
        /// the actual offset from UTC to local time, so it always knows the exact instant represented.
        /// </remarks>
        /// <returns>The instant corresponding to this value.</returns>
        [Pure]
        public Instant ToInstant() => offsetDateTime.ToInstant();

        /// <summary>
        /// Creates a new <see cref="ZonedDateTime"/> representing the same instant in time, in the
        /// same calendar but a different time zone.
        /// </summary>
        /// <param name="targetZone">The target time zone to convert to.</param>
        /// <returns>A new value in the target time zone.</returns>
        [Pure]
        public ZonedDateTime WithZone([NotNull] DateTimeZone targetZone)
        {
            Preconditions.CheckNotNull(targetZone, nameof(targetZone));
            return new ZonedDateTime(ToInstant(), targetZone, Calendar);
        }

        /// <summary>
        /// Creates a new ZonedDateTime representing the same physical date, time and offset, but in a different calendar.
        /// The returned ZonedDateTime is likely to have different date field values to this one.
        /// For example, January 1st 1970 in the Gregorian calendar was December 19th 1969 in the Julian calendar.
        /// </summary>
        /// <param name="calendar">The calendar system to convert this zoned date and time to.</param>
        /// <returns>The converted ZonedDateTime.</returns>
        [Pure]
        public ZonedDateTime WithCalendar([NotNull] CalendarSystem calendar)
        {
            return new ZonedDateTime(offsetDateTime.WithCalendar(calendar), zone);
        }

        #region Equality
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>True if the specified value is the same instant in the same time zone; false otherwise.</returns>
        public bool Equals(ZonedDateTime other) => offsetDateTime == other.offsetDateTime && Zone.Equals(other.Zone);

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param> 
        /// <filterpriority>2</filterpriority>
        /// <returns>True if the specified value is a <see cref="ZonedDateTime"/> representing the same instant in the same time zone; false otherwise.</returns>
        public override bool Equals(object obj) => obj is ZonedDateTime && Equals((ZonedDateTime)obj);

        /// <summary>
        /// Computes the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode() => HashCodeHelper.Hash(offsetDateTime, Zone);
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The second value to compare</param>
        /// <returns>True if the two operands are equal according to <see cref="Equals(ZonedDateTime)"/>; false otherwise</returns>
        public static bool operator ==(ZonedDateTime left, ZonedDateTime right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The second value to compare</param>
        /// <returns>False if the two operands are equal according to <see cref="Equals(ZonedDateTime)"/>; true otherwise</returns>
        public static bool operator !=(ZonedDateTime left, ZonedDateTime right) => !(left == right);

        /// <summary>
        /// Adds a duration to a zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Addition(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to add the duration to.</param>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Add(ZonedDateTime zonedDateTime, Duration duration) => zonedDateTime + duration;

        /// <summary>
        /// Returns the result of adding a duration to this zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Addition(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime Plus(Duration duration) => this + duration;

        /// <summary>
        /// Returns the result of adding a increment of hours to this zoned date and time
        /// </summary>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusHours(int hours) => this + Duration.FromHours(hours);

        /// <summary>
        /// Returns the result of adding an increment of minutes to this zoned date and time
        /// </summary>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusMinutes(int minutes) => this + Duration.FromMinutes(minutes);

        /// <summary>
        /// Returns the result of adding an increment of seconds to this zoned date and time
        /// </summary>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusSeconds(long seconds) => this + Duration.FromSeconds(seconds);

        /// <summary>
        /// Returns the result of adding an increment of milliseconds to this zoned date and time
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusMilliseconds(long milliseconds) => this + Duration.FromMilliseconds(milliseconds);

        /// <summary>
        /// Returns the result of adding an increment of ticks to this zoned date and time
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusTicks(long ticks) => this + Duration.FromTicks(ticks);

        /// <summary>
        /// Returns the result of adding an increment of nanoseconds to this zoned date and time
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime PlusNanoseconds(long nanoseconds) => this + Duration.FromNanoseconds(nanoseconds);

        /// <summary>
        /// Returns a new <see cref="ZonedDateTime"/> with the time advanced by the given duration. Note that
        /// due to daylight saving time changes this may not advance the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value retains the calendar system and time zone of <paramref name="zonedDateTime"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The <see cref="ZonedDateTime"/> to add the duration to.</param>
        /// <param name="duration">The duration to add.</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime operator +(ZonedDateTime zonedDateTime, Duration duration) =>
            new ZonedDateTime(zonedDateTime.ToInstant() + duration, zonedDateTime.Zone, zonedDateTime.Calendar);

        /// <summary>
        /// Subtracts a duration from a zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Subtraction(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to subtract the duration from.</param>
        /// <param name="duration">The duration to subtract.</param>
        /// <returns>A new value with the time "rewound" by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Subtract(ZonedDateTime zonedDateTime, Duration duration) => zonedDateTime - duration;

        /// <summary>
        /// Returns the result of subtracting a duration from this zoned date and time, for a fluent alternative to
        /// <see cref="op_Subtraction(ZonedDateTime, Duration)"/>
        /// </summary>
        /// <param name="duration">The duration to subtract</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the subtraction.</returns>
        [Pure]
        public ZonedDateTime Minus(Duration duration) => this - duration;

        /// <summary>
        /// Returns a new <see cref="ZonedDateTime"/> with the duration subtracted. Note that
        /// due to daylight saving time changes this may not change the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value retains the calendar system and time zone of <paramref name="zonedDateTime"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to subtract the duration from.</param>
        /// <param name="duration">The duration to subtract.</param>
        /// <returns>A new value with the time "rewound" by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime operator -(ZonedDateTime zonedDateTime, Duration duration) =>
            new ZonedDateTime(zonedDateTime.ToInstant() - duration, zonedDateTime.Zone, zonedDateTime.Calendar);

        /// <summary>
        /// Subtracts one zoned date and time from another, returning an elapsed duration.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Subtraction(ZonedDateTime, ZonedDateTime)"/>.
        /// </remarks>
        /// <param name="end">The zoned date and time value to subtract from; if this is later than <paramref name="start"/>
        /// then the result will be positive.</param>
        /// <param name="start">The zoned date and time to subtract from <paramref name="end"/>.</param>
        /// <returns>The elapsed duration from <paramref name="start"/> to <paramref name="end"/>.</returns>
        public static Duration Subtract(ZonedDateTime end, ZonedDateTime start) => end - start;

        /// <summary>
        /// Returns the result of subtracting another zoned date and time from this one, resulting in the elapsed duration
        /// between the two instants represented in the values.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Subtraction(ZonedDateTime, ZonedDateTime)"/>.
        /// </remarks>
        /// <param name="other">The zoned date and time to subtract from this one.</param>
        /// <returns>The elapsed duration from <paramref name="other"/> to this value.</returns>
        [Pure]
        public Duration Minus(ZonedDateTime other) => this - other;

        /// <summary>
        /// Subtracts one <see cref="ZonedDateTime"/> from another, resulting in the elapsed time between
        /// the two values.
        /// </summary>
        /// <remarks>
        /// This is equivalent to <c>end.ToInstant() - start.ToInstant()</c>; in particular:
        /// <list type="bullet">
        ///   <item><description>The two values can use different calendar systems</description></item>
        ///   <item><description>The two values can be in different time zones</description></item>
        ///   <item><description>The two values can have different UTC offsets</description></item>
        /// </list>
        /// </remarks>
        /// <param name="end">The zoned date and time value to subtract from; if this is later than <paramref name="start"/>
        /// then the result will be positive.</param>
        /// <param name="start">The zoned date and time to subtract from <paramref name="end"/>.</param>
        /// <returns>The elapsed duration from <paramref name="start"/> to <paramref name="end"/>.</returns>
        public static Duration operator -(ZonedDateTime end, ZonedDateTime start) => end.ToInstant() - start.ToInstant();
        #endregion

        /// <summary>
        /// Returns the <see cref="ZoneInterval"/> containing this value, in the time zone this
        /// value refers to.
        /// </summary>
        /// <remarks>
        /// This is simply a convenience method - it is logically equivalent to converting this
        /// value to an <see cref="Instant"/> and then asking the appropriate <see cref="DateTimeZone"/>
        /// for the <c>ZoneInterval</c> containing that instant.
        /// </remarks>
        /// <returns>The <c>ZoneInterval</c> containing this value.</returns>
        [Pure]
        [NotNull]
        public ZoneInterval GetZoneInterval() => Zone.GetZoneInterval(ToInstant());

        /// <summary>
        /// Indicates whether or not this <see cref="ZonedDateTime"/> is in daylight saving time
        /// for its time zone. This is determined by checking the <see cref="ZoneInterval.Savings"/> property
        /// of the zone interval containing this value.
        /// </summary>
        /// <seealso cref="GetZoneInterval()"/>
        /// <returns><c>true</c> if the zone interval containing this value has a non-zero savings
        /// component; <c>false</c> otherwise.</returns>
        [Pure]
        public bool IsDaylightSavingTime() => GetZoneInterval().Savings != Offset.Zero;

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("G"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() =>
            ZonedDateTimePattern.Patterns.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

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
            ZonedDateTimePattern.Patterns.BclSupport.Format(this, patternText, formatProvider);
        #endregion Formatting

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> value with the same local time and offset from
        /// UTC as this value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An offset does not convey as much information as a time zone; a <see cref="DateTimeOffset"/>
        /// represents an instant in time along with an associated local time, but it doesn't allow you
        /// to find out what the local time would be for another instant.
        /// </para>
        /// <para>
        /// If the date and time is not on a tick boundary (the unit of granularity of DateTime) the value will be truncated
        /// towards the start of time.
        /// </para>
        /// <para>
        /// If the offset has a non-zero second component, this is truncated as <c>DateTimeOffset</c> has an offset
        /// granularity of minutes.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The date/time is outside the range of <c>DateTimeOffset</c>,
        /// or the offset is outside the range of +/-14 hours (the range supported by <c>DateTimeOffset</c>).</exception>
        /// <returns>A <c>DateTimeOffset</c> with the same local date/time and offset as this. The <see cref="DateTime"/> part of
        /// the result always has a "kind" of Unspecified.</returns>
        [Pure]
        public DateTimeOffset ToDateTimeOffset() => offsetDateTime.ToDateTimeOffset();

        /// <summary>
        /// Returns a new <see cref="ZonedDateTime"/> representing the same instant in time as the given
        /// <see cref="DateTimeOffset"/>.
        /// The time zone used will be a fixed time zone, which uses the same offset throughout time.
        /// </summary>
        /// <param name="dateTimeOffset">Date and time value with an offset.</param>
        /// <returns>A <see cref="ZonedDateTime"/> value representing the same instant in time as the given <see cref="DateTimeOffset"/>.</returns>
        public static ZonedDateTime FromDateTimeOffset(DateTimeOffset dateTimeOffset) =>
            new ZonedDateTime(Instant.FromDateTimeOffset(dateTimeOffset),
                new FixedDateTimeZone(Offset.FromTimeSpan(dateTimeOffset.Offset)));

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this <see cref="ZonedDateTime"/> which has a
        /// <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as
        /// this value rather than the same local time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the date and time is not on a tick boundary (the unit of granularity of DateTime) the value will be truncated
        /// towards the start of time.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The final date/time is outside the range of <c>DateTime</c>.</exception>
        /// <returns>A <see cref="DateTime"/> representation of this value with a "universal" kind, with the same
        /// instant of time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUtc() => ToInstant().ToDateTimeUtc();

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this <see cref="ZonedDateTime"/> which has a
        /// <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Unspecified"/> and represents the same local time as
        /// this value rather than the same instant in time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset.
        /// </para>
        /// <para>
        /// If the date and time is not on a tick boundary (the unit of granularity of DateTime) the value will be truncated
        /// towards the start of time.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The date/time is outside the range of <c>DateTime</c>.</exception>
        /// <returns>A <see cref="DateTime"/> representation of this value with an "unspecified" kind, with the same
        /// local date and time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUnspecified() => LocalDateTime.ToDateTimeUnspecified();

        /// <summary>
        /// Constructs an <see cref="OffsetDateTime"/> with the same local date and time, and the same offset
        /// as this zoned date and time, effectively just "removing" the time zone itself.
        /// </summary>
        /// <returns>An OffsetDateTime with the same local date/time and offset as this value.</returns>
        [Pure]
        public OffsetDateTime ToOffsetDateTime() => offsetDateTime;

        /// <summary>
        /// Deconstruct this <see cref="ZonedDateTime"/> into its components.
        /// </summary>
        /// <param name="localDateTime">The <see cref="LocalDateTime"/> component.</param>
        /// <param name="dateTimeZone">The <see cref="DateTimeZone"/> component.</param>
        /// <param name="offset">The <see cref="Offset"/> component.</param>
        [Pure]
        public void Deconstruct(out LocalDateTime localDateTime, [NotNull] out DateTimeZone dateTimeZone, out Offset offset)
        {
            localDateTime = LocalDateTime;
            dateTimeZone = Zone;
            offset = Offset;
        }

        #region Comparers
        /// <summary>
        /// Base class for <see cref="ZonedDateTime"/> comparers.
        /// </summary>
        /// <remarks>
        /// Use the static properties of this class to obtain instances. This type is exposed so that the
        /// same value can be used for both equality and ordering comparisons.
        /// </remarks>
        [Immutable]
        public abstract class Comparer : IComparer<ZonedDateTime>, IEqualityComparer<ZonedDateTime>
        {
            // TODO(feature): A comparer which compares instants, but in a calendar-sensitive manner?

            /// <summary>
            /// Gets a comparer which compares <see cref="ZonedDateTime"/> values by their local date/time, without reference to
            /// the time zone or offset. Comparisons between two values of different calendar systems will fail with <see cref="ArgumentException"/>.
            /// </summary>
            /// <remarks>
            /// <para>For example, this comparer considers 2013-03-04T20:21:00 (Europe/London) to be later than
            /// 2013-03-04T19:21:00 (America/Los_Angeles) even though the second value represents a later instant in time.</para>
            /// <para>This property will return a reference to the same instance every time it is called.</para>
            /// </remarks>
            /// <value>A comparer which compares values by their local date/time.</value>
            [NotNull] public static Comparer Local => LocalComparer.Instance;

            /// <summary>
            /// Gets a comparer which compares <see cref="ZonedDateTime"/> values by the instants obtained by applying the offset to
            /// the local date/time, ignoring the calendar system.
            /// </summary>
            /// <remarks>
            /// <para>For example, this comparer considers 2013-03-04T20:21:00 (Europe/London) to be earlier than
            /// 2013-03-04T19:21:00 (America/Los_Angeles) even though the second value has a local time which is earlier; the time zones
            /// mean that the first value occurred earlier in the universal time line.</para>
            /// <para>This property will return a reference to the same instance every time it is called.</para>
            /// </remarks>
            /// <value>A comparer which compares values by the instants obtained by applying the offset to
            /// the local date/time, ignoring the calendar system.</value>
            [NotNull] public static Comparer Instant => InstantComparer.Instance;

            /// <summary>
            /// Internal constructor to prevent external classes from deriving from this.
            /// (That means we can add more abstract members in the future.)
            /// </summary>
            internal Comparer()
            {
            }

            /// <summary>
            /// Compares two <see cref="ZonedDateTime"/> values and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first value to compare.</param>
            /// <param name="y">The second value to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
            ///   <list type = "table">
            ///     <listheader>
            ///       <term>Value</term>
            ///       <description>Meaning</description>
            ///     </listheader>
            ///     <item>
            ///       <term>Less than zero</term>
            ///       <description><paramref name="x"/> is less than <paramref name="y"/>.</description>
            ///     </item>
            ///     <item>
            ///       <term>Zero</term>
            ///       <description><paramref name="x"/> is equals to <paramref name="y"/>.</description>
            ///     </item>
            ///     <item>
            ///       <term>Greater than zero</term>
            ///       <description><paramref name="x"/> is greater than <paramref name="y"/>.</description>
            ///     </item>
            ///   </list>
            /// </returns>
            public abstract int Compare(ZonedDateTime x, ZonedDateTime y);

            /// <summary>
            /// Determines whether the specified <c>ZonedDateTime</c> values are equal.
            /// </summary>
            /// <param name="x">The first <c>ZonedDateTime</c> to compare.</param>
            /// <param name="y">The second <c>ZonedDateTime</c> to compare.</param>
            /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
            public abstract bool Equals(ZonedDateTime x, ZonedDateTime y);

            /// <summary>
            /// Returns a hash code for the specified <c>ZonedDateTime</c>.
            /// </summary>
            /// <param name="obj">The <c>ZonedDateTime</c> for which a hash code is to be returned.</param>
            /// <returns>A hash code for the specified value.</returns>
            public abstract int GetHashCode(ZonedDateTime obj);
        }

        /// <summary>
        /// Implementation for <see cref="Comparer.Local"/>.
        /// </summary>
        private sealed class LocalComparer : Comparer
        {
            internal static readonly Comparer Instance = new LocalComparer();

            private LocalComparer()
            {
            }

            /// <inheritdoc />
            public override int Compare(ZonedDateTime x, ZonedDateTime y) =>
                OffsetDateTime.Comparer.Local.Compare(x.offsetDateTime, y.offsetDateTime);

            /// <inheritdoc />
            public override bool Equals(ZonedDateTime x, ZonedDateTime y) =>
                OffsetDateTime.Comparer.Local.Equals(x.offsetDateTime, y.offsetDateTime);

            /// <inheritdoc />
            public override int GetHashCode(ZonedDateTime obj) =>
                OffsetDateTime.Comparer.Local.GetHashCode(obj.offsetDateTime);
        }

        /// <summary>
        /// Implementation for <see cref="Comparer.Instant"/>.
        /// </summary>
        private sealed class InstantComparer : Comparer
        {
            internal static readonly Comparer Instance = new InstantComparer();

            private InstantComparer()
            {
            }

            /// <inheritdoc />
            public override int Compare(ZonedDateTime x, ZonedDateTime y) =>
                OffsetDateTime.Comparer.Instant.Compare(x.offsetDateTime, y.offsetDateTime);

            /// <inheritdoc />
            public override bool Equals(ZonedDateTime x, ZonedDateTime y) =>
                OffsetDateTime.Comparer.Instant.Equals(x.offsetDateTime, y.offsetDateTime);

            /// <inheritdoc />
            public override int GetHashCode(ZonedDateTime obj) =>
                OffsetDateTime.Comparer.Instant.GetHashCode(obj.offsetDateTime);
        }
        #endregion

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = OffsetDateTimePattern.ExtendedIso;
            if (!reader.MoveToAttribute("zone"))
            {
                throw new ArgumentException("No zone specified in XML for ZonedDateTime");
            }
            DateTimeZone newZone = DateTimeZoneProviders.Serialization[reader.Value];
            if (reader.MoveToAttribute("calendar"))
            {
                string newCalendarId = reader.Value;
                CalendarSystem newCalendar = CalendarSystem.ForId(newCalendarId);
                var newTemplateValue = pattern.TemplateValue.WithCalendar(newCalendar);
                pattern = pattern.WithTemplateValue(newTemplateValue);
            }
            reader.MoveToElement();
            string text = reader.ReadElementContentAsString();
            OffsetDateTime offsetDateTime = pattern.Parse(text).Value;
            if (newZone.GetUtcOffset(offsetDateTime.ToInstant()) != offsetDateTime.Offset)
            {
                // Might as well use the exception we've already got...
                ParseResult<ZonedDateTime>.InvalidOffset(text).GetValueOrThrow();
            }
            // Use the constructor which doesn't validate the offset, as we've already done that.
            this = new ZonedDateTime(offsetDateTime, newZone);
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteAttributeString("zone", Zone.Id);
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(OffsetDateTimePattern.ExtendedIso.Format(ToOffsetDateTime()));
        }
        #endregion

#if !NETSTANDARD1_3
        #region Binary serialization

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private ZonedDateTime(SerializationInfo info, StreamingContext context)
            // Note: this uses the constructor which explicitly validates that the offset is reasonable.
            : this(new LocalDateTime(info),
                   DateTimeZoneProviders.Serialization[info.GetString(BinaryFormattingConstants.ZoneIdSerializationName)],
                   new Offset(info))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            LocalDateTime.Serialize(info);
            Offset.Serialize(info);
            info.AddValue(BinaryFormattingConstants.ZoneIdSerializationName, Zone.Id);
        }
        #endregion
#endif
    }
}
