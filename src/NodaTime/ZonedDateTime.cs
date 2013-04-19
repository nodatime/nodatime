// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A <see cref="LocalDateTime"/> in a specific time zone and with a particular offset to distinguish between otherwise-ambiguous
    /// instants. A <see cref="ZonedDateTime"/> is global, in that it maps to a single <see cref="Instant"/>.
    /// </summary>
    /// <remarks>
    /// <para>Although <see cref="ZonedDateTime" /> includes both local and global concepts, it only supports
    /// duration-based - and not calendar-based - arithmetic. This avoids ambiguities
    /// and skipped date/time values becoming a problem within a series of calculations; instead,
    /// these can be considered just once, at the point of conversion to a <see cref="ZonedDateTime"/>.
    /// </para>
    /// <para>Comparisons of values can be handled in a way which is either calendar and zone sensitive or insensitive.
    /// Noda Time implements all the operators (and the <see cref="Equals(ZonedDateTime)"/> method) such that all operators other than <see cref="op_Inequality"/>
    /// will return false if asked to compare two values in different calendar systems or time zones.
    /// </para>
    /// <para>
    /// However, the <see cref="CompareTo"/> method (implementing <see cref="IComparable{T}"/>) is calendar and zone insensitive; it compares the two
    /// global instants in terms of when they actually occurred.
    /// </para>
    /// <para>
    /// It's unclear at the time of this writing whether this is the most appropriate approach, and it may change in future versions. In general,
    /// it would be a good idea for users to avoid comparing dates in different calendar systems, and indeed most users are unlikely to ever explicitly
    /// consider which calendar system they're working in anyway.
    /// </para>
    /// <para>
    /// Currently there is no real text handling support for this type.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public struct ZonedDateTime : IEquatable<ZonedDateTime>, IComparable<ZonedDateTime>, IComparable, IXmlSerializable
    {
        private readonly LocalDateTime localDateTime;
        private readonly DateTimeZone zone;
        private readonly Offset offset;

        /// <summary>
        /// Internal constructor used by other code that has already validated and 
        /// computed the appropriate field values. No further validation is performed.
        /// </summary>
        internal ZonedDateTime(LocalDateTime localDateTime, Offset offset, DateTimeZone zone)
        {
            this.localDateTime = localDateTime;
            this.offset = offset;
            this.zone = zone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime"/> struct.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        /// <param name="calendar">The calendar system.</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> or <paramref name="calendar"/> is null.</exception>
        public ZonedDateTime(Instant instant, DateTimeZone zone, CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Preconditions.CheckNotNull(calendar, "calendar");
            offset = zone.GetUtcOffset(instant);
            localDateTime = new LocalDateTime(instant.Plus(offset), calendar);
            this.zone = zone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime" /> struct in the specified time zone
        /// and the ISO calendar.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        public ZonedDateTime(Instant instant, DateTimeZone zone) : this(instant, zone, CalendarSystem.Iso)
        {
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
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is not a valid offset at the given
        /// local date and time</exception>
        public ZonedDateTime(LocalDateTime localDateTime, DateTimeZone zone, Offset offset)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Instant candidateInstant = localDateTime.LocalInstant.Minus(offset);
            Offset correctOffset = zone.GetUtcOffset(candidateInstant);
            // Not using Preconditions, to avoid building the string unnecessarily.
            if (correctOffset != offset)
            {
                throw new ArgumentException("Offset " + offset + " is invalid for local date and time " + localDateTime
                    + " in time zone " + zone.Id, "offset");
            }
            this.localDateTime = localDateTime;
            this.offset = offset;
            this.zone = zone;
        }

        /// <summary>Gets the offset of the local representation of this value from UTC.</summary>
        public Offset Offset { get { return offset; } }

        /// <summary>Gets the time zone associated with this value.</summary>
        public DateTimeZone Zone { get { return zone ?? DateTimeZone.Utc; } }

        /// <summary>Gets the local instant associated with this value.</summary>
        internal LocalInstant LocalInstant { get { return localDateTime.LocalInstant; } }

        /// <summary>
        /// Gets the local date and time represented by this zoned date and time. The returned <see cref="LocalDateTime"/>
        /// will have the same calendar system and return the same values for each of the calendar properties
        /// (Year, MonthOfYear and so on), but will not be associated with any particular time zone.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return localDateTime; } }

        /// <summary>Gets the calendar system associated with this zoned date and time.</summary>
        public CalendarSystem Calendar
        {
            get { return localDateTime.Calendar; }
        }

        /// <summary>
        /// Gets the local date represented by this zoned date and time. The returned <see cref="LocalDate"/>
        /// will have the same calendar system and return the same values for each of the date-based calendar
        /// properties (Year, MonthOfYear and so on), but will not be associated with any particular time zone.
        /// </summary>
        public LocalDate Date { get { return localDateTime.Date; } }

        /// <summary>
        /// Gets the time portion of this zoned date and time. The returned <see cref="LocalTime"/> will
        /// return the same values for each of the time-based properties (Hour, Minute and so on), but
        /// will not be associated with any particular time zone.
        /// </summary>
        public LocalTime TimeOfDay { get { return localDateTime.TimeOfDay; } }

        /// <summary>Gets the era for this zoned date and time.</summary>
        public Era Era { get { return LocalDateTime.Era; } }

        /// <summary>Gets the century within the era of this zoned date and time.</summary>
        public int CenturyOfEra { get { return LocalDateTime.CenturyOfEra; } }

        /// <summary>Gets the year of this zoned date and time.</summary>
        /// <remarks>This returns the "absolute year", so, for the ISO calendar,
        /// a value of 0 means 1 BC, for example.</remarks>
        public int Year { get { return LocalDateTime.Year; } }

        /// <summary>Gets the year of this zoned date and time within its century.</summary>
        /// <remarks>This always returns a value in the range 0 to 99 inclusive.</remarks>
        public int YearOfCentury { get { return LocalDateTime.YearOfCentury; } }

        /// <summary>Gets the year of this zoned date and time within its era.</summary>
        public int YearOfEra { get { return LocalDateTime.YearOfEra; } }

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
        public int WeekYear { get { return LocalDateTime.WeekYear; } }

        /// <summary>Gets the month of this zoned date and time within the year.</summary>
        public int Month { get { return LocalDateTime.Month; } }

        /// <summary>Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.</summary>
        public int WeekOfWeekYear { get { return LocalDateTime.WeekOfWeekYear; } }

        /// <summary>Gets the day of this zoned date and time within the year.</summary>
        public int DayOfYear { get { return LocalDateTime.DayOfYear; } }

        /// <summary>
        /// Gets the day of this zoned date and time within the month.
        /// </summary>
        public int Day { get { return LocalDateTime.Day; } }

        /// <summary>
        /// Gets the week day of this zoned date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        /// <seealso cref="DayOfWeek"/>
        public IsoDayOfWeek IsoDayOfWeek { get { return LocalDateTime.IsoDayOfWeek; } }

        /// <summary>
        /// Gets the week day of this zoned date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return LocalDateTime.DayOfWeek; } }

        /// <summary>
        /// Gets the hour of day of this zoned date and time, in the range 0 to 23 inclusive.
        /// </summary>
        public int Hour { get { return LocalDateTime.Hour; } }

        /// <summary>
        /// Gets the hour of the half-day of this zoned date and time, in the range 1 to 12 inclusive.
        /// </summary>
        public int ClockHourOfHalfDay { get { return LocalDateTime.ClockHourOfHalfDay; } }
        
        /// <summary>
        /// Gets the minute of this zoned date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int Minute { get { return LocalDateTime.Minute; } }

        /// <summary>
        /// Gets the second of this zoned date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int Second { get { return LocalDateTime.Second; } }

        /// <summary>
        /// Gets the millisecond of this zoned date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int Millisecond { get { return LocalDateTime.Millisecond; } }

        /// <summary>
        /// Gets the tick of this zoned date and time within the second, in the range 0 to 9,999,999 inclusive.
        /// </summary>
        public int TickOfSecond { get { return LocalDateTime.TickOfSecond; } }

        /// <summary>
        /// Gets the tick of this zoned date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return LocalDateTime.TickOfDay; } }

        /// <summary>
        /// Converts this value to the instant it represents on the time line.
        /// </summary>
        /// <remarks>
        /// This is always an unambiguous conversion. Any difficulties due to daylight saving
        /// transitions or other changes in time zone are handled when converting from a <see cref="LocalDateTime"/>
        /// to a <see cref="ZonedDateTime"/>; the <c>ZonedDateTime</c> remembers the actual offset from UTC to local time,
        /// so it always knows the exact instant represented.
        /// </remarks>
        /// <returns>The instant corresponding to this value.</returns>
        public Instant ToInstant()
        {
            return localDateTime.LocalInstant.Minus(offset);
        }

        /// <summary>
        /// Creates a new <see cref="ZonedDateTime"/> representing the same instant in time, in the
        /// same calendar but a different time zone.
        /// </summary>
        /// <param name="targetZone">The target time zone to convert to.</param>
        /// <returns>A new value in the target time zone.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetZone"/> is null.</exception>
        public ZonedDateTime WithZone(DateTimeZone targetZone)
        {
            Preconditions.CheckNotNull(targetZone, "targetZone");
            return new ZonedDateTime(ToInstant(), targetZone, localDateTime.Calendar);
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
            return LocalDateTime == other.LocalDateTime && Offset == other.Offset && Zone.Equals(other.Zone);
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
            hash = HashCodeHelper.Hash(hash, LocalInstant);
            hash = HashCodeHelper.Hash(hash, Offset);
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
        /// Compares two <see cref="ZonedDateTime"/> values to see if the left one is strictly earlier than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars or time zones.
        /// See the top-level type documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly earlier than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() < rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        /// <summary>
        /// Compares two <see cref="ZonedDateTime"/> values to see if the left one is earlier than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars or time zones.
        /// See the top-level type documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is earlier than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator <=(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() <= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        /// <summary>
        /// Compares two <see cref="ZonedDateTime"/> values to see if the left one is strictly later than the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars or time zones.
        /// See the top-level type documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is strictly later than <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() > rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        /// <summary>
        /// Compares two <see cref="ZonedDateTime"/> values to see if the left one is later than or equal to the right
        /// one.
        /// </summary>
        /// <remarks>
        /// This operator always returns false if the two operands have different calendars or time zones.
        /// See the top-level type documentation for more information about comparisons.
        /// </remarks>
        /// <param name="lhs">First operand of the comparison</param>
        /// <param name="rhs">Second operand of the comparison</param>
        /// <returns>true if the <paramref name="lhs"/> is later than or equal to <paramref name="rhs"/>, false otherwise.</returns>
        public static bool operator >=(ZonedDateTime lhs, ZonedDateTime rhs)
        {
            return lhs.ToInstant() >= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.Zone, rhs.Zone);
        }

        /// <summary>
        /// Indicates whether this date/time is earlier, later or the same as another one.
        /// </summary>
        /// <remarks>
        /// This is purely done in terms of the instant represented; the calendar system and time zone are ignored.
        /// </remarks>
        /// <param name="other">The other zoned date/time to compare this one with</param>
        /// <returns>A value less than zero if the instant represented by this zoned date/time is earlier than the one in
        /// <paramref name="other"/>; zero if the instant is the same as the one in <paramref name="other"/>;
        /// a value greater than zero if the instant is later than the one in <paramref name="other"/>.</returns>
        public int CompareTo(ZonedDateTime other)
        {
            return ToInstant().CompareTo(other.ToInstant());
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two ZonedDateTimes.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="ZonedDateTime"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this ZonedDateTime with another one; see <see cref="CompareTo(NodaTime.ZonedDateTime)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is ZonedDateTime, "obj", "Object must be of type NodaTime.ZonedDateTime.");
            return CompareTo((ZonedDateTime)obj);
        }

        /// <summary>
        /// Returns a new <see cref="ZonedDateTime"/> with the time advanced by the given duration. Note that
        /// due to daylight saving time changes this may not advance the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value retains the calendar system and time zone of the <see cref="ZonedDateTime"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The <see cref="ZonedDateTime"/> to add the duration to.</param>
        /// <param name="duration">The duration to add.</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime operator +(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() + duration, zonedDateTime.Zone, zonedDateTime.LocalDateTime.Calendar);
        }

        /// <summary>
        /// Adds a duration to a zoned date and time. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="zonedDateTime">The value to add the duration to.</param>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Add(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime + duration;
        }

        /// <summary>
        /// Returns the result of adding a duration to this zoned date and time, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the addition.</returns>
        public ZonedDateTime Plus(Duration duration)
        {
            return this + duration;
        }


        /// <summary>
        /// Subtracts a duration from a zoned date and time. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="zonedDateTime">The value to subtract the duration from.</param>
        /// <param name="duration">The duration to subtract.</param>
        /// <returns>A new value with the time "rewound" by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime Subtract(ZonedDateTime zonedDateTime, Duration duration)
        {
            return zonedDateTime - duration;
        }

        /// <summary>
        /// Returns the result of subtracting a duration from this zoned date and time, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="duration">The duration to subtract</param>
        /// <returns>A new <see cref="ZonedDateTime" /> representing the result of the subtraction.</returns>
        public ZonedDateTime Minus(Duration duration)
        {
            return this - duration;
        }

        /// <summary>
        /// Returns a new ZonedDateTime with the duration subtracted. Note that
        /// due to daylight saving time changes this may not change the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value retains the calendar system and time zone of the <see cref="ZonedDateTime"/>.
        /// </remarks>
        /// <param name="zonedDateTime">The value to subtract the duration from.</param>
        /// <param name="duration">The duration to subtract.</param>
        /// <returns>A new value with the time "rewound" by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime operator -(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() - duration, zonedDateTime.Zone, zonedDateTime.LocalDateTime.Calendar);
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
        public ZoneInterval GetZoneInterval()
        {
            return Zone.GetZoneInterval(ToInstant());
        }

        /// <summary>
        /// Currently returns a string representation of this value indicating the local time,
        /// offset and time zone separately. The default <c>ToString</c> method of each component is used,
        /// which will render the local time and offset in the "general" pattern for the current thread's culture,
        /// and simply include the ID for most time zone implementations.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This representation is a temporary measure until full support for parsing and formatting
        /// <see cref="ZonedDateTime" /> values is implemented. It is provided in order to make diagnostics
        /// simpler, but is likely to be changed in future releases.
        /// </para>
        /// </remarks>
        /// <returns>A string representation of this value.</returns>
        public override string ToString()
        {
            return "Local: " + localDateTime + " Offset: " + offset + " Zone: " + Zone;
        }

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
        public DateTimeOffset ToDateTimeOffset()
        {
            return new DateTimeOffset(LocalInstant.Ticks - NodaConstants.BclEpoch.Ticks, Offset.ToTimeSpan());
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
        public DateTime ToDateTimeUnspecified()
        {
            return LocalInstant.ToDateTimeUnspecified();
        }

        /// <summary>
        /// Constructs an <see cref="OffsetDateTime"/> with the same local date and time, and the same offset
        /// as this zoned date and time, effectively just "removing" the time zone itself.
        /// </summary>
        /// <returns>An OffsetDateTime with the same local date/time and offset as this value.</returns>
        public OffsetDateTime ToOffsetDateTime()
        {
            return new OffsetDateTime(localDateTime, offset);
        }

        #region Comparers
        /// <summary>
        /// Base class for <see cref="ZonedDateTime"/> comparers.
        /// </summary>
        /// <remarks>
        /// <para>Use the static properties of this class to obtain instances.</para>
        /// <para>For the curious: this class only exists so that in the future, it can expose more functionality - probably
        /// implementing <see cref="IEqualityComparer{T}"/>. If we simply provided properties on ZonedDateTime of type
        /// <see cref="IComparer{T}"/> we'd have no backward-compatible way of adding to the set of implemented interfaces.</para>
        /// </remarks>
        public abstract class Comparer : IComparer<ZonedDateTime>
        {
            /// <summary>
            /// Returns a comparer which compares <see cref="ZonedDateTime"/> values by their local date/time, without reference to
            /// the time zone, offset or the calendar system.
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
            /// <para>This comparer behaves the same way as the <see cref="CompareTo"/> method; it is provided for symmetry with <see cref="LocalComparer"/>.</para>
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

            /// <inheritdoc />
            public abstract int Compare(ZonedDateTime x, ZonedDateTime y);
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
                return x.localDateTime.LocalInstant.CompareTo(y.localDateTime.LocalInstant);
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
                return x.ToInstant().CompareTo(y.ToInstant());
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
                // TODO(1.2): Work out if this is actually a reasonable exception. Maybe we
                // should use UTC instead.
                throw new ArgumentException("No zone specified in XML for ZonedDateTime");
            }
            DateTimeZone newZone = DateTimeZoneProviders.XmlSerialization[reader.Value];
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
                ParseResult<ZonedDateTime>.InvalidOffset.GetValueOrThrow();
            }
            // Use the constructor which doesn't validate the offset, as we've already done that.
            this = new ZonedDateTime(offsetDateTime.LocalDateTime, offsetDateTime.Offset, newZone);
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
    }
}
