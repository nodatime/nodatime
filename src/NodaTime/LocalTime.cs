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

using NodaTime.Format;

namespace NodaTime
{
    /// <summary>
    /// LocalTime is an immutable struct representing a time of day, with no reference
    /// to a particular calendar, time zone or date.
    /// </summary>
    public struct LocalTime
    {
        private readonly LocalInstant localInstant;

        public LocalTime(int hour, int minute, int second) : this(hour, minute, second, 0, 0)
        {
        }

        public LocalTime(int hour, int minute, int second, int millisecond) : this(hour, minute, second, millisecond, 0)
        {
        }

        public LocalTime(int hour, int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            localInstant = new LocalDateTime(1970, 1, 1, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystem.Iso).LocalInstant;
        }

        private LocalTime(LocalInstant localInstant)
        {
            this.localInstant = localInstant;
        }

        public int HourOfDay { get { return LocalDateTime.HourOfDay; } }
        public int MinuteOfHour { get { return LocalDateTime.MinuteOfHour; } }
        public int SecondOfMinute { get { return LocalDateTime.SecondOfMinute; } }
        public int SecondOfDay { get { return LocalDateTime.SecondOfDay; } }
        public int MillisecondOfSecond { get { return LocalDateTime.MillisecondOfSecond; } }
        public int MillisecondOfDay { get { return LocalDateTime.MillisecondOfDay; } }
        public int TickOfMillisecond { get { return LocalDateTime.TickOfMillisecond; } }
        public long TickOfDay { get { return LocalDateTime.TickOfDay; } }

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

        public static bool operator ==(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant == rhs.localInstant;
        }

        public static bool operator !=(LocalTime lhs, LocalTime rhs)
        {
            return lhs.localInstant != rhs.localInstant;
        }

        // TODO: Implement IEquatable etc

        public override string ToString()
        {
            // TODO: Shouldn't need to build a ZonedDateTime!
            return IsoDateTimeFormats.TimeNoZone.Print(DateTimeZone.Utc.AtExactly(this.LocalDateTime));
        }

        public override int GetHashCode()
        {
            return localInstant.GetHashCode();
        }

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