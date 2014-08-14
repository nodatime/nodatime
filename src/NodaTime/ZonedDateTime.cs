// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

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
#if !PCL
    [Serializable]
#endif
    public struct ZonedDateTime : IEquatable<ZonedDateTime>, IFormattable, IXmlSerializable
#if !PCL
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
            this.zone = Preconditions.CheckNotNull(zone, "zone");
            offsetDateTime = new OffsetDateTime(instant, zone.GetUtcOffset(instant), Preconditions.CheckNotNull(calendar, "calendar"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime" /> struct in the specified time zone
        /// and the ISO calendar.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        public ZonedDateTime(Instant instant, DateTimeZone zone)
        {
            this.zone = Preconditions.CheckNotNull(zone, "zone");
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
            this.zone = Preconditions.CheckNotNull(zone, "zone");
            Instant candidateInstant = localDateTime.ToLocalInstant().Minus(offset);
            Offset correctOffset = zone.GetUtcOffset(candidateInstant);
            // Not using Preconditions, to avoid building the string unnecessarily.
            if (correctOffset != offset)
            {
                throw new ArgumentException("Offset " + offset + " is invalid for local date and time " + localDateTime
                    + " in time zone " + zone.Id, "offset");

            }
            offsetDateTime = new OffsetDateTime(localDateTime, offset);
        }

        /// <summary>Gets the offset of the local representation of this value from UTC.</summary>
        public Offset Offset { get { return offsetDateTime.Offset; } }

        /// <summary>Gets the time zone associated with this value.</summary>
        public DateTimeZone Zone { get { return zone ?? DateTimeZone.Utc; } }

        /// <summary>
        /// Gets the local date and time represented by this zoned date and time. The returned
        /// <see cref="T:NodaTime.LocalDateTime"/> will have the same calendar system and return the same values for
        /// each of the calendar properties (Year, MonthOfYear and so on), but will not be associated with any
        /// particular time zone.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return offsetDateTime.LocalDateTime; } }

        /// <summary>Gets the calendar system associated with this zoned date and time.</summary>
        public CalendarSystem Calendar
        {
            get { return offsetDateTime.Calendar; }
        }

        /// <summary>
        /// Gets the local date represented by this zoned date and time. The returned <see cref="LocalDate"/>
        /// will have the same calendar system and return the same values for each of the date-based calendar
        /// properties (Year, MonthOfYear and so on), but will not be associated with any particular time zone.
        /// </summary>
        public LocalDate Date { get { return offsetDateTime.Date; } }

        /// <summary>
        /// Gets the time portion of this zoned date and time. The returned <see cref="LocalTime"/> will
        /// return the same values for each of the time-based properties (Hour, Minute and so on), but
        /// will not be associated with any particular time zone.
        /// </summary>
        public LocalTime TimeOfDay { get { return offsetDateTime.TimeOfDay; } }

        /// <summary>Gets the era for this zoned date and time.</summary>
        public Era Era { get { return offsetDateTime.Era; } }

        /// <summary>Gets the year of this zoned date and time.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        public int Year { get { return offsetDateTime.Year; } }

        /// <summary>Gets the year of this zoned date and time within its era.</summary>
        public int YearOfEra { get { return offsetDateTime.YearOfEra; } }

        /// <summary>
        /// Gets the "week year" of this date and time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the <see cref="WeekOfWeekYear"/> field.
        /// In the standard ISO-8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year.
        /// The WeekYear allows you to query the effective year for that day.
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of WeekYear 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of WeekYear 2013.
        /// </para>
        /// </remarks>
        public int WeekYear { get { return offsetDateTime.WeekYear; } }

        /// <summary>Gets the month of this zoned date and time within the year.</summary>
        public int Month { get { return offsetDateTime.Month; } }

        /// <summary>Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.</summary>
        public int WeekOfWeekYear { get { return offsetDateTime.WeekOfWeekYear; } }

        /// <summary>Gets the day of this zoned date and time within the year.</summary>
        public int DayOfYear { get { return offsetDateTime.DayOfYear; } }

        /// <summary>
        /// Gets the day of this zoned date and time within the month.
        /// </summary>
        public int Day { get { return offsetDateTime.Day; } }

        /// <summary>
        /// Gets the week day of this zoned date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <seealso cref="DayOfWeek"/>
        public IsoDayOfWeek IsoDayOfWeek { get { return offsetDateTime.IsoDayOfWeek; } }

        /// <summary>
        /// Gets the week day of this zoned date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return offsetDateTime.DayOfWeek; } }

        /// <summary>
        /// Gets the hour of day of this zoned date and time, in the range 0 to 23 inclusive.
        /// </summary>
        public int Hour { get { return offsetDateTime.Hour; } }

        /// <summary>
        /// Gets the hour of the half-day of this zoned date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return offsetDateTime.ClockHourOfHalfDay; } }
        
        /// <summary>
        /// Gets the minute of this zoned date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int Minute { get { return offsetDateTime.Minute; } }

        /// <summary>
        /// Gets the second of this zoned date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int Second { get { return offsetDateTime.Second; } }

        /// <summary>
        /// Gets the millisecond of this zoned date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int Millisecond { get { return offsetDateTime.Millisecond; } }

        /// <summary>
        /// Gets the tick of this zoned date and time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return offsetDateTime.TickOfSecond; } }

        /// <summary>
        /// Gets the tick of this zoned date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return offsetDateTime.TickOfDay; } }

        /// <summary>
        /// Gets the nanosecond of this zoned date and time within the second, in the range 0 to 999,999,999 inclusive.
        /// </summary>
        public int NanosecondOfSecond { get { return offsetDateTime.NanosecondOfSecond; } }

        /// <summary>
        /// Gets the nanosecond of this zoned date and time within the day, in the range 0 to 86,399,999,999,999 inclusive.
        /// </summary>
        public long NanosecondOfDay { get { return offsetDateTime.NanosecondOfDay; } }

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
        public Instant ToInstant()
        {
            return offsetDateTime.ToInstant();
        }

        /// <summary>
        /// Creates a new <see cref="ZonedDateTime"/> representing the same instant in time, in the
        /// same calendar but a different time zone.
        /// </summary>
        /// <param name="targetZone">The target time zone to convert to.</param>
        /// <returns>A new value in the target time zone.</returns>
        [Pure]
        public ZonedDateTime WithZone([NotNull] DateTimeZone targetZone)
        {
            Preconditions.CheckNotNull(targetZone, "targetZone");
            return new ZonedDateTime(ToInstant(), targetZone, Calendar);
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
        public bool Equals(ZonedDateTime other)
        {
            return offsetDateTime == other.offsetDateTime && Zone.Equals(other.Zone);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to.</param> 
        /// <filterpriority>2</filterpriority>
        /// <returns>True if the specified value is a <see cref="ZonedDateTime"/> representing the same instant in the same time zone; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ZonedDateTime)
            {
                return Equals((ZonedDateTime)obj);
            }
            return false;
        }

        /// <summary>
        /// Computes the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, offsetDateTime);
            hash = HashCodeHelper.Hash(hash, Zone);
            return hash;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The second value to compare</param>
        /// <returns>True if the two operands are equal according to <see cref="Equals(ZonedDateTime)"/>; false otherwise</returns>
        public static bool operator ==(ZonedDateTime left, ZonedDateTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The first value to compare</param>
        /// <param name="right">The second value to compare</param>
        /// <returns>False if the two operands are equal according to <see cref="Equals(ZonedDateTime)"/>; true otherwise</returns>
        public static bool operator !=(ZonedDateTime left, ZonedDateTime right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Adds a duration to a zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Addition(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to add the duration to.</param>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Add(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime + duration;
        }

        /// <summary>
        /// Returns the result of adding a duration to this zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Addition(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        [Pure]
        public ZonedDateTime Plus(Duration duration)
        {
            return this + duration;
        }

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
        public static ZonedDateTime operator +(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() + duration, zonedDateTime.Zone, zonedDateTime.Calendar);
        }

        /// <summary>
        /// Subtracts a duration from a zoned date and time.
        /// </summary>
        /// <remarks>
        /// This is an alternative way of calling <see cref="op_Subtraction(ZonedDateTime, Duration)"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to subtract the duration from.</param>
        /// <param name="duration">The duration to subtract.</param>
        /// <returns>A new value with the time "rewound" by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Subtract(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime - duration;
        }

        /// <summary>
        /// Returns the result of subtracting a duration from this zoned date and time, for a fluent alternative to
        /// <see cref="op_Subtraction(ZonedDateTime, Duration)"/>
        /// </summary>
        /// <param name="duration">The duration to subtract</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the subtraction.</returns>
        [Pure]
        public ZonedDateTime Minus(Duration duration)
        {
            return this - duration;
        }

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
        public static ZonedDateTime operator -(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() - duration, zonedDateTime.Zone, zonedDateTime.Calendar);
        }

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
        public static Duration Subtract(ZonedDateTime end, ZonedDateTime start)
        {
            return end - start;
        }

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
        public Duration Minus(ZonedDateTime other)
        {
            return this - other;
        }

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
        public static Duration operator -(ZonedDateTime end, ZonedDateTime start)
        {
            return end.ToInstant() - start.ToInstant();
        }
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
        public ZoneInterval GetZoneInterval()
        {
            return Zone.GetZoneInterval(ToInstant());
        }

        /// <summary>
        /// Indicates whether or not this <see cref="ZonedDateTime"/> is in daylight saving time
        /// for its time zone. This is determined by checking the <see cref="ZoneInterval.Savings"/> property
        /// of the zone interval containing this value.
        /// </summary>
        /// <seealso cref="GetZoneInterval()"/>
        /// <returns><c>true</c> if the zone interval containing this value has a non-zero savings
        /// component; <c>false</c> otherwise.</returns>
        [Pure]
        public bool IsDaylightSavingTime()
        {
            return GetZoneInterval().Savings != Offset.Zero;
        }

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("G"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return ZonedDateTimePattern.Patterns.BclSupport.Format(this, null, CultureInfo.CurrentCulture);
        }

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
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return ZonedDateTimePattern.Patterns.BclSupport.Format(this, patternText, formatProvider);
        }
        #endregion Formatting

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> value with the same local time and offset from
        /// UTC as this value.
        /// </summary>
        /// <remarks>
        /// An offset does not convey as much information as a time zone; a <see cref="DateTimeOffset"/>
        /// represents an instant in time along with an associated local time, but it doesn't allow you
        /// to find out what the local time would be for another instant.
        /// </remarks>
        /// <returns>A <see cref="DateTimeOffset"/> representation of this value.</returns>
        [Pure]
        public DateTimeOffset ToDateTimeOffset()
        {
            return offsetDateTime.ToDateTimeOffset();
        }

        /// <summary>
        /// Returns a new <see cref="ZonedDateTime"/> representing the same instant in time as the given
        /// <see cref="DateTimeOffset"/>.
        /// The time zone used will be a fixed time zone, which uses the same offset throughout time.
        /// </summary>
        /// <param name="dateTimeOffset">Date and time value with an offset.</param>
        /// <returns>A <see cref="ZonedDateTime"/> value representing the same instant in time as the given <see cref="DateTimeOffset"/>.</returns>
        public static ZonedDateTime FromDateTimeOffset(DateTimeOffset dateTimeOffset)
        {
            return new ZonedDateTime(Instant.FromDateTimeOffset(dateTimeOffset),
                new FixedDateTimeZone(Offset.FromTimeSpan(dateTimeOffset.Offset)));
        }

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this <see cref="ZonedDateTime"/> which has a
        /// <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as
        /// this value rather than the same local time.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representation of this value with a "universal" kind, with the same
        /// instant of time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUtc()
        {
            return ToInstant().ToDateTimeUtc();
        }

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this <see cref="ZonedDateTime"/> which has a
        /// <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Unspecified"/> and represents the same local time as
        /// this value rather than the same instant in time.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset.
        /// </remarks>
        /// <returns>A <see cref="DateTime"/> representation of this value with an "unspecified" kind, with the same
        /// local date and time as this value.</returns>
        [Pure]
        public DateTime ToDateTimeUnspecified()
        {
            return LocalDateTime.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Constructs an <see cref="OffsetDateTime"/> with the same local date and time, and the same offset
        /// as this zoned date and time, effectively just "removing" the time zone itself.
        /// </summary>
        /// <returns>An OffsetDateTime with the same local date/time and offset as this value.</returns>
        [Pure]
        public OffsetDateTime ToOffsetDateTime()
        {
            return offsetDateTime;
        }

        #region Comparers
        /// <summary>
        /// Base class for <see cref="ZonedDateTime"/> comparers.
        /// </summary>
        /// <remarks>
        /// Use the static properties of this class to obtain instances. This type is exposed so that the
        /// same value can be used for both equality and ordering comparisons.
        /// </remarks>
        public abstract class Comparer : IComparer<ZonedDateTime>, IEqualityComparer<ZonedDateTime>
        {
            // TODO(2.0): A comparer which compares instants, but in a calendar-sensitive manner?

            /// <summary>
            /// Returns a comparer which compares <see cref="ZonedDateTime"/> values by their local date/time, without reference to
            /// the time zone or offset. Comparisons between two values of different calendar systems will fail with <see cref="ArgumentException"/>.
            /// </summary>
            /// <remarks>
            /// <para>For example, this comparer considers 2013-03-04T20:21:00 (Europe/London) to be later than
            /// 2013-03-04T19:21:00 (America/Los_Angeles) even though the second value represents a later instant in time.</para>
            /// <para>This property will return a reference to the same instance every time it is called.</para>
            /// </remarks>
            public static Comparer Local { get { return LocalComparer.Instance; } }

            /// <summary>
            /// Returns a comparer which compares <see cref="ZonedDateTime"/> values by the instants obtained by applying the offset to
            /// the local date/time, ignoring the calendar system.
            /// </summary>
            /// <remarks>
            /// <para>For example, this comparer considers 2013-03-04T20:21:00 (Europe/London) to be earlier than
            /// 2013-03-04T19:21:00 (America/Los_Angeles) even though the second value has a local time which is earlier; the time zones
            /// mean that the first value occurred earlier in the universal time line.</para>
            /// <para>This property will return a reference to the same instance every time it is called.</para>
            /// </remarks>
            public static Comparer Instant { get { return InstantComparer.Instance; } }

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
            public override int Compare(ZonedDateTime x, ZonedDateTime y)
            {
                return OffsetDateTime.Comparer.Local.Compare(x.offsetDateTime, y.offsetDateTime);
            }

            /// <inheritdoc />
            public override bool Equals(ZonedDateTime x, ZonedDateTime y)
            {
                return OffsetDateTime.Comparer.Local.Equals(x.offsetDateTime, y.offsetDateTime);
            }

            /// <inheritdoc />
            public override int GetHashCode(ZonedDateTime obj)
            {
                return OffsetDateTime.Comparer.Local.GetHashCode(obj.offsetDateTime);
            }
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
            public override int Compare(ZonedDateTime x, ZonedDateTime y)
            {
                return OffsetDateTime.Comparer.Instant.Compare(x.offsetDateTime, y.offsetDateTime);
            }

            /// <inheritdoc />
            public override bool Equals(ZonedDateTime x, ZonedDateTime y)
            {
                return OffsetDateTime.Comparer.Instant.Equals(x.offsetDateTime, y.offsetDateTime);
            }

            /// <inheritdoc />
            public override int GetHashCode(ZonedDateTime obj)
            {
                return OffsetDateTime.Comparer.Instant.GetHashCode(obj.offsetDateTime);
            }
        }
        #endregion

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var pattern = OffsetDateTimePattern.ExtendedIsoPattern;
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
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            writer.WriteAttributeString("zone", Zone.Id);
            if (Calendar != CalendarSystem.Iso)
            {
                writer.WriteAttributeString("calendar", Calendar.Id);
            }
            writer.WriteString(OffsetDateTimePattern.ExtendedIsoPattern.Format(ToOffsetDateTime()));
        }
        #endregion

#if !PCL
        #region Binary serialization
        private const string DaysSerializationName = "days";
        private const string NanosecondOfDaySerializationName = "nanosecondOfDay";
        private const string CalendarIdSerializationName = "calendar";
        private const string OffsetMillisecondsSerializationName = "offsetMilliseconds";
        private const string ZoneIdSerializationName = "zone";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private ZonedDateTime(SerializationInfo info, StreamingContext context)
            // Note: this uses the constructor which explicitly validates that the offset is reasonable.
            : this(new LocalDateTime(new LocalDate(info.GetInt32(DaysSerializationName),
                                                   CalendarSystem.ForId(info.GetString(CalendarIdSerializationName))),
                                     LocalTime.FromNanosecondsSinceMidnight(info.GetInt64(NanosecondOfDaySerializationName))),
                   DateTimeZoneProviders.Serialization[info.GetString(ZoneIdSerializationName)],
                   Offset.FromMilliseconds(info.GetInt32(OffsetMillisecondsSerializationName)))
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
            // FIXME(2.0): Revisit serialization
            info.AddValue(DaysSerializationName, Date.DaysSinceEpoch);
            info.AddValue(NanosecondOfDaySerializationName, TimeOfDay.NanosecondOfDay);
            info.AddValue(CalendarIdSerializationName, Calendar.Id);
            info.AddValue(OffsetMillisecondsSerializationName, Offset.Milliseconds);
            info.AddValue(ZoneIdSerializationName, Zone.Id);
        }
        #endregion
#endif
    }
}
