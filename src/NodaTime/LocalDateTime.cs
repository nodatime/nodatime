#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using NodaTime.Partials;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A LocalDateTime value does not represent an instant on the time line, mostly because it has
    /// no associated time zone: "November 12th 2009 7pm, ISO calendar" occurred at different
    /// instants for different people around the world.
    /// </para>
    /// <para>
    /// This type defaults to using the IsoCalendarSystem unless a different calendar system is
    /// specified.
    /// </para>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    /// </remarks>
    public struct LocalDateTime : IEquatable<LocalDateTime>
    {
        private readonly CalendarSystem calendar;
        private readonly LocalInstant localInstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO
        /// calendar system.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        internal LocalDateTime(LocalInstant localInstant) : this(localInstant, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="localInstant">The local instant.</param>
        /// <param name="calendar">The calendar system.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        internal LocalDateTime(LocalInstant localInstant, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            this.localInstant = localInstant;
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute) : this(year, month, day, hour, minute, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second) : this(year, month, day, hour, minute, second, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second);
            this.calendar = calendar;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct using the ISO calendar system.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, CalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
            : this(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDateTime"/> struct.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="day">The day.</param>
        /// <param name="hour">The hour.</param>
        /// <param name="minute">The minute.</param>
        /// <param name="second">The second.</param>
        /// <param name="millisecond">The millisecond.</param>
        /// <param name="tickWithinMillisecond">The tick within millisecond.</param>
        /// <param name="calendar">The calendar.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is <c>null</c>.</exception>
        public LocalDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int tickWithinMillisecond, CalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond);
            this.calendar = calendar;
        }

        /// <summary>
        /// Gets the local instant.
        /// </summary>
        /// <value>The local instant.</value>
        internal LocalInstant LocalInstant { get { return localInstant; } }

        /// <summary>
        /// Gets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        public CalendarSystem Calendar { get { return calendar; } }

        public int Era { get { return calendar.Fields.Era.GetValue(localInstant); } }

        public int CenturyOfEra { get { return calendar.Fields.CenturyOfEra.GetValue(localInstant); } }

        public int Year { get { return calendar.Fields.Year.GetValue(localInstant); } }

        public int YearOfCentury { get { return calendar.Fields.YearOfCentury.GetValue(localInstant); } }

        public int YearOfEra { get { return calendar.Fields.YearOfEra.GetValue(localInstant); } }

        public int WeekYear { get { return calendar.Fields.WeekYear.GetValue(localInstant); } }

        public int MonthOfYear { get { return calendar.Fields.MonthOfYear.GetValue(localInstant); } }

        public int WeekOfWeekYear { get { return calendar.Fields.WeekOfWeekYear.GetValue(localInstant); } }

        public int DayOfYear { get { return calendar.Fields.DayOfYear.GetValue(localInstant); } }

        public int DayOfMonth { get { return calendar.Fields.DayOfMonth.GetValue(localInstant); } }

        public IsoDayOfWeek IsoDayOfWeek { get { return calendar.GetIsoDayOfWeek(localInstant); } }

        public int DayOfWeek { get { return calendar.Fields.DayOfWeek.GetValue(localInstant); } }

        public int HourOfDay { get { return calendar.Fields.HourOfDay.GetValue(localInstant); } }

        public int MinuteOfHour { get { return calendar.Fields.MinuteOfHour.GetValue(localInstant); } }

        public int SecondOfMinute { get { return calendar.Fields.SecondOfMinute.GetValue(localInstant); } }

        public int SecondOfDay { get { return calendar.Fields.SecondOfDay.GetValue(localInstant); } }

        public int MillisecondOfSecond { get { return calendar.Fields.MillisecondOfSecond.GetValue(localInstant); } }

        public int MillisecondOfDay { get { return calendar.Fields.MillisecondOfDay.GetValue(localInstant); } }

        public int TickOfMillisecond { get { return calendar.Fields.TickOfMillisecond.GetValue(localInstant); } }

        public long TickOfDay { get { return calendar.Fields.TickOfDay.GetInt64Value(localInstant); } }

        public LocalTime TimeOfDay { get { return new LocalTime(HourOfDay, MinuteOfHour, SecondOfMinute, MillisecondOfSecond, TickOfMillisecond); } }

        public LocalDate Date { get { return new LocalDate(Year, MonthOfYear, DayOfMonth); } }

        #region Implementation of IEquatable<LocalDateTime>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        ///                 </param>
        public bool Equals(LocalDateTime other)
        {
            return localInstant == other.localInstant && calendar.Equals(other.calendar);
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(LocalDateTime left, LocalDateTime right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(LocalDateTime left, LocalDateTime right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Adds a period to a local date/time. Fields are added in the order provided by the period.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to add</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator +(LocalDateTime localDateTime, Period2 period)
        {
            CalendarSystem calendar = localDateTime.Calendar;
            return new LocalDateTime(calendar.Add(period, localDateTime.LocalInstant, 1), calendar);
        }

        /// <summary>
        /// Subtracts a period from a local date/time. Fields are subtracted in the order provided by the period.
        /// </summary>
        /// <param name="localDateTime">Initial local date and time</param>
        /// <param name="period">Period to subtract</param>
        /// <returns>The resulting local date and time</returns>
        public static LocalDateTime operator -(LocalDateTime localDateTime, Period2 period)
        {
            CalendarSystem calendar = localDateTime.Calendar;
            return new LocalDateTime(calendar.Add(period, localDateTime.LocalInstant, -1), calendar);
        }
        #endregion

        #region object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is LocalDateTime)
            {
                return Equals((LocalDateTime)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, LocalInstant);
            hash = HashCodeHelper.Hash(hash, Calendar);
            return hash;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Calendar + ": " + LocalInstant;
        }
        #endregion
    }
}