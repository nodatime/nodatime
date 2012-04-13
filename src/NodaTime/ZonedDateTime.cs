#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A <see cref="LocalDateTime"/> in a specific time zone and with a particular offset to distinguish between otherwise-ambiguous
    /// instants. A ZonedDateTime is global, in that it maps to a single <see cref="Instant"/>.
    /// </summary>
    /// <remarks>
    /// <para>Comparisons of values can be handled in a way which is either calendar and zone sensitive or insensitive.
    /// Noda Time implements all the operators (and the <see cref="Equals(ZonedDateTime)"/> method) such that all operators other than <see cref="op_Inequality"/>
    /// will return false if asked to compare two values in different calendar systems and time zones.
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
    public struct ZonedDateTime : IEquatable<ZonedDateTime>, IComparable<ZonedDateTime>
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

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
        /// <param name="calendar">The calendar system.</param>
        /// <param name="instant">The instant.</param>
        /// <param name="zone">The time zone.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="calendar"/> or <paramref name="zone"/> is <c>null</c>.</exception>
        public ZonedDateTime(Instant instant, DateTimeZone zone, CalendarSystem calendar)
        {
            if (zone == null)
            {
                throw new ArgumentNullException("zone");
            }
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            offset = zone.GetOffsetFromUtc(instant);
            localDateTime = new LocalDateTime(instant.Plus(offset), calendar);
            this.zone = zone;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime" /> struct in the specified time zone
        /// and the ISO calendar.
        /// </summary>
        /// <param name="instant">The instant of time to represent.</param>
        /// <param name="zone">The time zone to represent the instant within.</param>
        public ZonedDateTime(Instant instant, DateTimeZone zone) : this(instant, zone, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime"/> struct in the specified time zone
        /// from a given local time and offset. The offset is validated to be correct as part of initialization.
        /// In most cases a local time can only map to a single instant anyway, but the offset is included here for cases
        /// where the local time is ambiguous, usually due to daylight saving transitions.
        /// </summary>
        /// <param name="localDateTime">The local date and time to represent</param>
        /// <param name="zone">The time zone to represent the local date and time within</param>
        /// <param name="offset">The offset between UTC and local time at the desired instant</param>
        /// <exception cref="ArgumentNullException"><paramref name="zone"/> is null</exception>
        /// <exception cref="ArgumentException"><paramref name="offset"/> is not a valid offset at the given
        /// local date and time</exception>
        public ZonedDateTime(LocalDateTime localDateTime, DateTimeZone zone, Offset offset)
        {
            Preconditions.CheckNotNull(zone, "zone");
            Instant candidateInstant = localDateTime.LocalInstant.Minus(offset);
            Offset correctOffset = zone.GetOffsetFromUtc(candidateInstant);
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
        public DateTimeZone Zone { get { return zone; } }

        /// <summary>Gets the local instant associated with this value.</summary>
        internal LocalInstant LocalInstant { get { return localDateTime.LocalInstant; } }

        /// <summary>
        /// Gets the local date and time represented by this zoned date and time. The returned <see cref="LocalDateTime"/>
        /// will have the same calendar system and return the same values for each of the calendar properties
        /// (Year, MonthOfYear and so on), but will not be associated with any particular time zone.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return localDateTime; } }

        /// <summary>Gets the era for this date and time.</summary>
        public Era Era { get { return LocalDateTime.Era; } }

        /// <summary>Gets the century within the era of this date and time.</summary>
        public int CenturyOfEra { get { return LocalDateTime.CenturyOfEra; } }

        /// <summary>Gets the year of this date and time.</summary>
        public int Year { get { return LocalDateTime.Year; } }

        /// <summary>Gets the year of this date and time within its century.</summary>
        public int YearOfCentury { get { return LocalDateTime.YearOfCentury; } }

        /// <summary>Gets the year of this date and time within its era.</summary>
        public int YearOfEra { get { return LocalDateTime.YearOfEra; } }

        /// <summary>
        /// Gets the "week year" of this date and time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the WeekOfWeekYear field.
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

        /// <summary>Gets the month of this date and time within the year.</summary>
        public int Month { get { return LocalDateTime.Month; } }

        /// <summary>Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.</summary>
        public int WeekOfWeekYear { get { return LocalDateTime.WeekOfWeekYear; } }

        /// <summary>Gets the day of this date and time within the year.</summary>
        public int DayOfYear { get { return LocalDateTime.DayOfYear; } }

        /// <summary>
        /// Gets the day of this date and time within the month.
        /// </summary>
        public int Day { get { return LocalDateTime.Day; } }

        /// <summary>
        /// Gets the week day of this date and time expressed as an <see cref="NodaTime.IsoDayOfWeek"/> value,
        /// for calendars which use ISO days of the week.
        /// </summary>
        /// <exception cref="InvalidOperationException">The underlying calendar doesn't use ISO days of the week.</exception>
        public IsoDayOfWeek IsoDayOfWeek { get { return LocalDateTime.IsoDayOfWeek; } }

        /// <summary>
        /// Gets the week day of this date and time as a number.
        /// </summary>
        /// <remarks>
        /// For calendars using ISO week days, this gives 1 for Monday to 7 for Sunday.
        /// </remarks>
        /// <seealso cref="IsoDayOfWeek"/>
        public int DayOfWeek { get { return LocalDateTime.DayOfWeek; } }

        /// <summary>Gets the hour of day of this date and time, in the range 0 to 23 inclusive.</summary>
        public int Hour { get { return LocalDateTime.Hour; } }

        /// <summary>Gets the hour of the half-day of this date and time, in the range 1 to 12 inclusive.</summary>
        public int ClockHourOfHalfDay { get { return LocalDateTime.ClockHourOfHalfDay; } }
        
        /// <summary>Gets the minute of this date and time, in the range 0 to 59 inclusive.</summary>
        public int Minute { get { return LocalDateTime.Minute; } }

        /// <summary>Gets the second of this date and time within the minute, in the range 0 to 59 inclusive.</summary>
        public int Second { get { return LocalDateTime.Second; } }

        /// <summary>Gets the second of this date and time within the day, in the range 0 to 86,399 inclusive.</summary>
        public int SecondOfDay { get { return LocalDateTime.SecondOfDay; } }

        /// <summary>Gets the millisecond of this date and time within the second, in the range 0 to 999 inclusive.</summary>
        public int Millisecond { get { return LocalDateTime.Millisecond; } }

        /// <summary>Gets the millisecond of this date and time within the day, in the range 0 to 86,399,999 inclusive.</summary>
        public int MillisecondOfDay { get { return LocalDateTime.MillisecondOfDay; } }

        /// <summary>Gets the tick of this date and time within the millisecond, in the range 0 to 9,999 inclusive.</summary>
        public int Tick { get { return LocalDateTime.Tick; } }

        /// <summary>Gets the tick of this local time within the second, in the range 0 to 9,999,999 inclusive.</summary>
        public int TickOfSecond { get { return LocalDateTime.TickOfSecond; } }

        /// <summary>Gets the tick of this date and time within the day, in the range 0 to 863,999,999,999 inclusive.</summary>
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
        /// <param name="targetZone">The target time zone to convert to. Must not be null.</param>
        /// <returns>A new value in the target time zone.</returns>
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
        /// Compares two ZonedDateTime values to see if the left one is strictly earlier than the right
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
            return lhs.ToInstant() < rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.zone, rhs.zone);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is earlier than or equal to the right
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
            return lhs.ToInstant() <= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.zone, rhs.zone);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is strictly later than the right
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
            return lhs.ToInstant() > rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.zone, rhs.zone);
        }

        /// <summary>
        /// Compares two LocalDateTime values to see if the left one is later than or equal to the right
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
            return lhs.ToInstant() >= rhs.ToInstant() && Equals(lhs.LocalDateTime.Calendar, rhs.LocalDateTime.Calendar) && Equals(lhs.zone, rhs.zone);
        }

        /// <summary>
        /// Indicates whether this date/time is earlier, later or the same as another one. This is purely
        /// done in terms of the instant represented; the calendar system and time zone are ignored.
        /// </summary>
        /// <param name="other">The other zoned date/time to compare this one with</param>
        /// <returns>A value less than zero if the instant represented by this zoned date/time is earlier than the one in
        /// <paramref name="other"/>; zero if the instant is the same as the one in <paramref name="other"/>;
        /// a value greater than zero if the instant is later than the one in <paramref name="other"/>.</returns>
        public int CompareTo(ZonedDateTime other)
        {
            return ToInstant().CompareTo(other.ToInstant());
        }

        /// <summary>
        /// Returns a new ZonedDateTime with the time advanced by the given duration. Note that
        /// due to daylight saving time changes this may not advance the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value uses the same calendar system and time zone as the left operand.
        /// </remarks>
        /// <param name="zonedDateTime">The ZonedDateTime to add the duration to.</param>
        /// <param name="duration">The duration to add.</param>
        /// <returns>A new value with the time advanced by the given duration, in the same calendar system and time zone.</returns>
        public static ZonedDateTime operator +(ZonedDateTime zonedDateTime, Duration duration)
        {
            return new ZonedDateTime(zonedDateTime.ToInstant() + duration, zonedDateTime.Zone, zonedDateTime.LocalDateTime.Calendar);
        }

        /// <summary>
        /// Adds a duration to zoned date and time. Friendly alternative to <c>operator+()</c>.
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
        /// Subtracts a duration from zoned date and time. Friendly alternative to <c>operator-()</c>.
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
        /// The returned value uses the same calendar system and time zone as the left operand.
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
        /// Converts this date and time to text according to the default formatting for the culture.
        /// </summary>
        /// <returns>A text representation of this value.</returns>
        // TODO(Post-V1): Proper formatting support.
        public override string ToString()
        {
            return "Local: " + localDateTime + " Offset: " + offset + " Zone: " + zone;
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
            return new DateTimeOffset(NodaConstants.DateTimeEpochTicks + LocalInstant.Ticks, Offset.ToTimeSpan());
        }

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> into a new ZonedDateTime representing the same instant in time, with
        /// the same offset. The time zone used will be a fixed time zone, which uses the same offset throughout time.
        /// </summary>
        /// <param name="dateTimeOffset">Date and time value with an offset.</param>
        /// <returns>A <see cref="ZonedDateTime"/> value representing the same instant in time as the given <see cref="DateTimeOffset"/>.</returns>
        public static ZonedDateTime FromDateTimeOffset(DateTimeOffset dateTimeOffset)
        {
            return new ZonedDateTime(Instant.FromDateTimeOffset(dateTimeOffset),
                new FixedDateTimeZone(Offset.FromTimeSpan(dateTimeOffset.Offset)));
        }

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this ZonedDateTime which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as this value
        /// rather than the same local time.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representation of this value with a "universal" kind, with the same
        /// instant of time as this value.</returns>
        public DateTime ToDateTimeUtc()
        {
            return ToInstant().ToDateTimeUtc();
        }

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this ZonedDateTime which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Unspecified"/> and represents the same local time as this value
        /// rather than the same instant in time.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset, which makes it as close to
        /// the Noda Time non-system-specific "local" concept as exists in .NET.
        /// </remarks>
        /// <returns>A <see cref="DateTime"/> representation of this value with an "unspecified" kind, with the same
        /// local date and time as this value.</returns>
        public DateTime ToDateTimeUnspecified()
        {
            return LocalInstant.ToDateTimeUnspecified();
        }
    }
}
