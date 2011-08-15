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
namespace NodaTime
{
    /// <summary>
    /// LocalTime is an immutable struct representing a time of day, with no reference
    /// to a particular calendar, time zone or date.
    /// </summary>
    public struct LocalTime : IEquatable<LocalTime>
    {
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
            localInstant = new LocalDateTime(1970, 1, 1, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso).LocalInstant;
        }

        private LocalTime(LocalInstant localInstant)
        {
            this.localInstant = localInstant;
        }

        /// <summary>
        /// Gets the hour of day of this local time, in the range 0 to 23 inclusive.
        /// </summary>
        public int HourOfDay { get { return LocalDateTime.HourOfDay; } }
        
        /// <summary>
        /// Gets the minute of this local time, in the range 0 to 59 inclusive.
        /// </summary>
        public int MinuteOfHour { get { return LocalDateTime.MinuteOfHour; } }

        /// <summary>
        /// Gets the second of this local time within the minute, in the range 0 to 59 inclusive.
        /// </summary>
        public int SecondOfMinute { get { return LocalDateTime.SecondOfMinute; } }

        /// <summary>
        /// Gets the second of this local time within the day, in the range 0 to 86,399 inclusive.
        /// </summary>
        public int SecondOfDay { get { return LocalDateTime.SecondOfDay; } }

        /// <summary>
        /// Gets the millisecond of this local time within the second, in the range 0 to 999 inclusive.
        /// </summary>
        public int MillisecondOfSecond { get { return LocalDateTime.MillisecondOfSecond; } }

        /// <summary>
        /// Gets the millisecond of this local time within the day, in the range 0 to 86,399,999 inclusive.
        /// </summary>
        public int MillisecondOfDay { get { return LocalDateTime.MillisecondOfDay; } }

        /// <summary>
        /// Gets the tick of this local time within the millisceond, in the range 0 to 9,999 inclusive.
        /// </summary>
        public int TickOfMillisecond { get { return LocalDateTime.TickOfMillisecond; } }

        /// <summary>
        /// Gets the tick of this local time within the day, in the range 0 to 863,999,999,999 inclusive.
        /// </summary>
        public long TickOfDay { get { return LocalDateTime.TickOfDay; } }

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
    }
}
