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
using NodaTime.Fields;

namespace NodaTime
{
    /// <summary>
    /// LocalTime is an immutable struct representing a time of day, with no reference
    /// to a particular calendar, time zone or date.
    /// </summary>
    public struct LocalTime : IEquatable<LocalTime>
    {
        private static readonly FieldSet IsoFields = CalendarSystem.Iso.Fields;

        private readonly LocalInstant localInstant;

        /// <summary>
        /// Creates a local time at the given hour, minute and second,
        /// with millisecond-of-second and tick-of-millisecond values of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        public LocalTime(int hour, int minute, int second) : this(hour, minute, second, 0, 0)
        {
        }

        /// <summary>
        /// Creates a local time at the given hour, minute, second and millisecond,
        /// with a tick-of-millisecond value of zero.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <param name="millisecond">The millisecond of the second.</param>
        public LocalTime(int hour, int minute, int second, int millisecond)
            : this(hour, minute, second, millisecond, 0)
        {
        }

        /// <summary>
        /// Creates a local time at the given hour, minute, second, millisecond and tick within millisecond.
        /// </summary>
        /// <param name="hour">The hour of day.</param>
        /// <param name="minute">The minute of the hour.</param>
        /// <param name="second">The second of the minute.</param>
        /// <param name="millisecond">The millisecond of the second.</param>
        /// <param name="tickWithinMillisecond">The tick within the millisecond.</param>
        public LocalTime(int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            localInstant = CalendarSystem.Iso.GetLocalInstant(1970, 1, 1, hour, minute, second, millisecond, tickWithinMillisecond);
        }

        /// <summary>
        /// Constructor only called from other parts of Noda Time - trusted to be within January 1st 1970 UTC.
        /// </summary>
        internal LocalTime(LocalInstant localInstant)
        {
            this.localInstant = localInstant;
        }

        /// <summary>
        /// Gets the hour of day of this local time, in the range 0 to 23 inclusive.
        /// </summary>
        public int HourOfDay { get { return IsoFields.HourOfDay.GetValue(localInstant); } }
        
        /// <summary>
        /// Gets the minute of this local time, in the range 0 to 59 inclusive.
        /// </summary>
        public int MinuteOfHour { get { return IsoFields.MinuteOfHour.GetValue(localInstant); ; } }

        /// <summary>
        /// Gets the second of this local time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int SecondOfMinute { get { return IsoFields.SecondOfMinute.GetValue(localInstant); } }

        /// <summary>
        /// Gets the second of this local time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return IsoFields.SecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int MillisecondOfSecond { get { return IsoFields.MillisecondOfSecond.GetValue(localInstant); } }

        /// <summary>
        /// Gets the millisecond of this local time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return IsoFields.MillisecondOfDay.GetValue(localInstant); } }

        /// <summary>
        /// Gets the tick of this local time within the millisceond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int TickOfMillisecond { get { return IsoFields.TickOfMillisecond.GetValue(localInstant); ; } }

        /// <summary>
        /// Gets the tick of this local time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return IsoFields.TickOfDay.GetInt64Value(localInstant); ; } }

        /// <summary>
        /// Returns a LocalDateTime with this local time, on January 1st 1970 in the ISO calendar.
        /// </summary>
        public LocalDateTime LocalDateTime { get { return new LocalDateTime(localInstant); } }

        /// <summary>
        /// TODO: Assert no units as large a day
        /// </summary>
        public static LocalTime operator +(LocalTime time, Period period)
        {
            return (time.LocalDateTime + period).TimeOfDay;
        }

        /// <summary>
        /// TODO: Assert no units as large as a day
        /// </summary>
        public static LocalTime operator -(LocalTime time, Period period)
        {
            return (time.LocalDateTime - period).TimeOfDay;
        }

        /// <summary>
        /// Compares two local times for equality, by checking whether they represent
        /// the exact same local time, down to the tick.
        /// </summary>
        public static bool operator ==(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant == rhs.localInstant;
        }

        /// <summary>
        /// Compares two local times for inequality.
        /// </summary>
        public static bool operator !=(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant != rhs.localInstant;
        }

        /// <summary>
        /// Converts this local time to text form, using the current format provider's default
        /// formatting information.
        /// </summary>
        public override string ToString()
        {
            // TODO: Implement as part of general formatting work
            return string.Format("{0:00}:{1:00}:{2:00}", HourOfDay, MinuteOfHour, SecondOfMinute);
        }

        /// <summary>
        /// Returns a hash code for this local time.
        /// </summary>
        public override int GetHashCode()
        {
            return localInstant.GetHashCode();
        }

        /// <summary>
        /// Compares this local time with the specified one for equality,
        /// by checking whether the two values represent the exact same local time, down to the tick.
        /// </summary>
        public bool Equals(LocalTime other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares this local time with the specified reference. A local time is
        /// only equal to another local time with the same underlying tick value.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is LocalTime))
            {
                return false;
            }
            return this == (LocalTime)obj;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of hours added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11pm plus two hours is 1am, for example.
        /// </remarks>
        /// <param name="hours">The number of hours to add</param>
        /// <returns>The current value plus the given number of hours.</returns>
        public LocalTime AddHours(long hours)
        {
            return LocalDateTime.AddHours(hours).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of minutes added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11pm plus 120 minutes is 1am, for example.
        /// </remarks>
        /// <param name="minutes">The number of minutes to add</param>
        /// <returns>The current value plus the given number of minutes.</returns>
        public LocalTime AddMinutes(long minutes)
        {
            return LocalDateTime.AddMinutes(minutes).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <remarks>
        /// If the value goes past the start or end of the day, it wraps - so 11:59pm plus 120 seconds is 12:01am, for example.
        /// </remarks>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime AddSeconds(long seconds)
        {
            return LocalDateTime.AddSeconds(seconds).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of seconds added.
        /// </summary>
        /// <param name="seconds">The number of seconds to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime AddMilliseconds(long milliseconds)
        {
            return LocalDateTime.AddMilliseconds(milliseconds).TimeOfDay;
        }

        /// <summary>
        /// Returns a new LocalTime representing the current value with the given number of ticks added.
        /// </summary>
        /// <param name="ticks">The number of ticks to add</param>
        /// <returns>The current value plus the given number of seconds.</returns>
        public LocalTime AddTicks(long ticks)
        {
            return LocalDateTime.AddTicks(ticks).TimeOfDay;
        }
    }
}
