#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A date and time in a particular calendar system.
    /// </summary>
    /// <remarks><para>A LocalDateTime value does not
    /// represent an instant on the time line, mostly because it has no associated
    /// time zone: "November 12th 2009 7pm, ISO calendar" occurred at different instants
    /// for different people around the world.
    /// </para>
    /// <para>
    /// This type defaults to using the IsoCalendarSystem unless a different calendar
    /// system is specified.
    /// </para>
    /// </remarks>
    public struct LocalDateTime
        : IEquatable<LocalDateTime>
    {
        private readonly ICalendarSystem calendar;
        private readonly LocalInstant localInstant;

        public LocalDateTime(LocalInstant localInstant)
            : this(localInstant, IsoCalendarSystem.Instance)
        {
        }

        public LocalDateTime(LocalInstant localInstant, ICalendarSystem calendar)
        {
            if (calendar == null)
            {
                throw new ArgumentNullException("calendar");
            }
            this.localInstant = localInstant;
            this.calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute)
            : this(year, month, day, hour, minute, IsoCalendarSystem.Instance)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, ICalendarSystem calendar)
        {
            if (calendar == null) {throw new ArgumentNullException("calendar");}
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute);
            this.calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, IsoCalendarSystem.Instance)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             ICalendarSystem calendar)
        {
            if (calendar == null) { throw new ArgumentNullException("calendar"); }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second);
            this.calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, IsoCalendarSystem.Instance)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond,
                             ICalendarSystem calendar)
            : this(year, month, day, hour, minute, second, millisecond, 0, calendar)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond,
                             int tickWithinMillisecond)
            : this(
                year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, IsoCalendarSystem.Instance)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond,
                             int tickWithinMillisecond, ICalendarSystem calendar)
        {
            if (calendar == null) { throw new ArgumentNullException("calendar"); }
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second, millisecond,
                                                    tickWithinMillisecond);
            this.calendar = calendar;
        }

        public LocalInstant LocalInstant
        {
            get { return localInstant; }
        }

        public ICalendarSystem Calendar
        {
            get { return calendar; }
        }

        public int Era
        {
            get { return calendar.Fields.Era.GetValue(localInstant); }
        }

        public int CenturyOfEra
        {
            get { return calendar.Fields.CenturyOfEra.GetValue(localInstant); }
        }

        public int Year
        {
            get { return calendar.Fields.Year.GetValue(localInstant); }
        }

        public int YearOfCentury
        {
            get { return calendar.Fields.YearOfCentury.GetValue(localInstant); }
        }

        public int YearOfEra
        {
            get { return calendar.Fields.YearOfEra.GetValue(localInstant); }
        }

        public int WeekYear
        {
            get { return calendar.Fields.WeekYear.GetValue(localInstant); }
        }

        public int MonthOfYear
        {
            get { return calendar.Fields.MonthOfYear.GetValue(localInstant); }
        }

        public int WeekOfWeekYear
        {
            get { return calendar.Fields.WeekOfWeekYear.GetValue(localInstant); }
        }

        public int DayOfYear
        {
            get { return calendar.Fields.DayOfYear.GetValue(localInstant); }
        }

        public int DayOfMonth
        {
            get { return calendar.Fields.DayOfMonth.GetValue(localInstant); }
        }

        public int DayOfWeek
        {
            get { return calendar.Fields.DayOfWeek.GetValue(localInstant); }
        }

        public int HourOfDay
        {
            get { return calendar.Fields.HourOfDay.GetValue(localInstant); }
        }

        public int MinuteOfHour
        {
            get { return calendar.Fields.MinuteOfHour.GetValue(localInstant); }
        }

        public int SecondOfMinute
        {
            get { return calendar.Fields.SecondOfMinute.GetValue(localInstant); }
        }

        public int SecondOfDay
        {
            get { return calendar.Fields.SecondOfDay.GetValue(localInstant); }
        }

        public int MillisecondOfSecond
        {
            get { return calendar.Fields.MillisecondOfSecond.GetValue(localInstant); }
        }

        public int MillisecondOfDay
        {
            get { return calendar.Fields.MillisecondOfDay.GetValue(localInstant); }
        }

        public int TickOfMillisecond
        {
            get { return calendar.Fields.TickOfMillisecond.GetValue(localInstant); }
        }

        public long TickOfDay
        {
            get { return calendar.Fields.TickOfDay.GetInt64Value(localInstant); }
        }

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
            return this.localInstant == other.localInstant &&
                   this.calendar.Equals(other.calendar);
        }

        #endregion

        #region operators

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
                return this.Equals((LocalDateTime) obj);
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