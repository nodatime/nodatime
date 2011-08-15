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
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A date and time with an associated chronology - or to look at it a different way, a
    /// LocalDateTime plus a time zone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some seemingly valid values do not represent a valid instant due to the local time moving
    /// forward during a daylight saving transition, thus "skipping" the given value. Other values
    /// occur twice (due to local time moving backward), in which case a ZonedDateTime will always
    /// represent the later of the two possible times, when converting it to an instant.
    /// </para>
    /// <para>
    /// A value constructed with "new ZonedDateTime()" will represent the Unix epoch in the ISO
    /// calendar system in the UTC time zone. That is the only situation in which the chronology is
    /// assumed rather than specified.
    /// </para>
    /// </remarks>
    public struct ZonedDateTime : IEquatable<ZonedDateTime>
    {
        private readonly Chronology chronology;
        private readonly LocalInstant localInstant;
        private readonly Offset offset;

        /// <summary>
        /// Internal constructor used by other code that has already validated and 
        /// computed the appropriate field values. No further validation is performed.
        /// </summary>
        internal ZonedDateTime(LocalInstant localInstant, Offset offset, Chronology chronology)
        {
            this.localInstant = localInstant;
            this.offset = offset;
            this.chronology = chronology;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime"/> struct.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="chronology">The chronology.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="chronology"/> is <c>null</c>.</exception>
        public ZonedDateTime(Instant instant, Chronology chronology)
        {
            if (chronology == null)
            {
                throw new ArgumentNullException("chronology");
            }
            offset = chronology.Zone.GetOffsetFromUtc(instant);
            localInstant = instant.Plus(offset);
            this.chronology = chronology;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZonedDateTime" /> struct in the specified time zone
        /// and the ISO calendar.
        /// </summary>
        /// <param name="instant">The instant of time to represent.</param>
        /// <param name="zone">The time zone to represent the instant within.</param>
        public ZonedDateTime(Instant instant, DateTimeZone zone) : this(instant, ValidateZone(zone).ToIsoChronology())
        {
        }

        /// <summary>
        /// Used by the constructor above to allow us to perform argument validation
        /// but still chain to another constructor.
        /// </summary>
        private static DateTimeZone ValidateZone(DateTimeZone zone)
        {
            if (zone == null)
            {
                throw new ArgumentNullException("zone");
            }
            return zone;
        }


        /// <summary>
        /// Gets the chronology.
        /// </summary>
        public Chronology Chronology { get { return chronology; } }

        /// <summary>
        /// Gets the offset 
        /// </summary>
        public Offset Offset { get { return offset; } }

        /// <summary>
        /// Gets the time zone 
        /// </summary>
        public DateTimeZone Zone { get { return Chronology.Zone; } }

        /// <summary>
        /// Gets the local instant.
        /// </summary>
        internal LocalInstant LocalInstant { get { return localInstant; } }

        /// <summary>
        /// Gets the local date and time represented by this zoned date and time. The returned <see cref="LocalDateTime"/>
        /// will have the same calendar system and return the same values for each of the calendar properties
        /// (Year, MonthOfYear and so on), but not be associated with any particular time zone.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return new LocalDateTime(LocalInstant, chronology.Calendar); } }

        /// <summary>
        /// Gets the era for this date and time. The precise meaning of this value depends on the calendar
        /// system in use.
        /// </summary>
        public int Era { get { return LocalDateTime.Era; } }

        /// <summary>
        /// Gets the century within the era of this date and time.
        /// </summary>
        public int CenturyOfEra { get { return LocalDateTime.CenturyOfEra; } }

        /// <summary>
        /// Gets the year of this date and time.
        /// </summary>
        public int Year { get { return LocalDateTime.Year; } }

        /// <summary>
        /// Gets the year of this date and time within its century.
        /// </summary>
        public int YearOfCentury { get { return LocalDateTime.YearOfCentury; } }

        /// <summary>
        /// Gets the year of this date and time within its era.
        /// </summary>
        public int YearOfEra { get { return LocalDateTime.YearOfEra; } }

        /// <summary>
        /// Gets the "week year" of this date and time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The WeekYear is the year that matches with the WeekOfWeekYear field.
        /// In the standard ISO8601 week algorithm, the first week of the year
        /// is that in which at least 4 days are in the year. As a result of this
        /// definition, day 1 of the first week may be in the previous year.
        /// The WeekYear allows you to query the effective year for that day
        /// </para>
        /// <para>
        /// For example, January 1st 2011 was a Saturday, so only two days of that week
        /// (Saturday and Sunday) were in 2011. Therefore January 1st is part of
        /// week 52 of WeekYear 2010. Conversely, December 31st 2012 is a Monday,
        /// so is part of week 1 of WeekYear 2013.
        /// </para>
        /// </remarks>
        public int WeekYear { get { return LocalDateTime.WeekYear; } }

        /// <summary>
        /// Gets the month of this date and time within the year.
        /// </summary>
        public int MonthOfYear { get { return LocalDateTime.MonthOfYear; } }

        /// <summary>
        /// Gets the week within the WeekYear. See <see cref="WeekYear"/> for more details.
        /// </summary>
        public int WeekOfWeekYear { get { return LocalDateTime.WeekOfWeekYear; } }

        /// <summary>
        /// Gets the day of this date and time within the year.
        /// </summary>
        public int DayOfYear { get { return LocalDateTime.DayOfYear; } }

        /// <summary>
        /// Gets the day of this date and time within the month.
        /// </summary>
        public int DayOfMonth { get { return LocalDateTime.DayOfMonth; } }

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

        /// <summary>
        /// Gets the hour of day of this date and time, in the range 0 to 23 inclusive.
        /// </summary>
        public int HourOfDay { get { return LocalDateTime.HourOfDay; } }

        /// <summary>
        /// Gets the minute of this date and time, in the range 0 to 59 inclusive.
        /// </summary>
        public int MinuteOfHour { get { return LocalDateTime.MinuteOfHour; } }

        /// <summary>
        /// Gets the second of this date and time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int SecondOfMinute { get { return LocalDateTime.SecondOfMinute; } }

        /// <summary>
        /// Gets the second of this date and time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return LocalDateTime.SecondOfDay; } }

        /// <summary>
        /// Gets the millisecond of this date and time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int MillisecondOfSecond { get { return LocalDateTime.MillisecondOfSecond; } }

        /// <summary>
        /// Gets the millisecond of this date and time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return LocalDateTime.MillisecondOfDay; } }

        /// <summary>
        /// Gets the tick of this date and time within the millisceond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int TickOfMillisecond { get { return LocalDateTime.TickOfMillisecond; } }

        /// <summary>
        /// Gets the tick of this date and time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return LocalDateTime.TickOfDay; } }

        /// <summary>
        /// Converts this value to the instant it represents on the time line.
        /// If two instants are represented by the same set of values, the later
        /// instant is returned.
        /// </summary>
        /// <remarks>
        /// Conceptually this is a conversion (which is why it's not a property) but
        /// in reality the conversion is done at the point of construction.
        /// </remarks>
        public Instant ToInstant()
        {
            return localInstant.Minus(offset);
        }

        #region Equality
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(ZonedDateTime other)
        {
            return LocalInstant == other.LocalInstant && Offset == other.Offset && Chronology == other.Chronology;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. 
        ///                 </param><filterpriority>2</filterpriority>
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
            hash = HashCodeHelper.Hash(hash, Chronology);
            return hash;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        public static bool operator ==(ZonedDateTime left, ZonedDateTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        public static bool operator !=(ZonedDateTime left, ZonedDateTime right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a new ZonedDateTime with the time advanced by the given duration. Note that
        /// due to daylight saving time changes this may not advance the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value uses the same calendar system and time zone as the left operand.
        /// </remarks>
        /// <param name="left">The ZonedDateTime to add the duration to.</param>
        /// <param name="right">The duration to add.</param>
        public static ZonedDateTime operator +(ZonedDateTime left, Duration right)
        {
            return new ZonedDateTime(left.ToInstant() + right, left.Chronology);
        }

        /// <summary>
        /// Adds a duration to zoned date and time. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        public static ZonedDateTime Add(ZonedDateTime left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        /// Subtracts a duration from zoned date and time. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        public static ZonedDateTime Subtract(ZonedDateTime left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        /// Returns a new ZonedDateTime with the duration subtracted. Note that
        /// due to daylight saving time changes this may not change the local time by the same amount.
        /// </summary>
        /// <remarks>
        /// The returned value uses the same calendar system and time zone as the left operand.
        /// </remarks>
        /// <param name="left">The ZonedDateTime to subtract the duration from.</param>
        /// <param name="right">The duration to add.</param>
        public static ZonedDateTime operator -(ZonedDateTime left, Duration right)
        {
            return new ZonedDateTime(left.ToInstant() - right, left.Chronology);
        }
        #endregion

        /// <summary>
        /// Converts this date and time to text according to the default formatting for the culture.
        /// </summary>
        // TODO: Improve description and make the implementation match the documentation :)
        public override string ToString()
        {
            return "Local: " + localInstant + " Offset: " + offset + " Zone: " + chronology.Zone;
        }
    }
}
