#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    /// A date and time with an associated chronology - or to look at it
    /// a different way, a LocalDateTime plus a time zone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some seemingly valid values do not represent a valid instant due to
    /// the local time moving forward during a daylight saving transition, thus "skipping"
    /// the given value. Other values occur twice (due to local time moving backward),
    /// in which case a ZonedDateTime will always represent the later of the two
    /// possible times, when converting it to an instant.
    /// </para>
    /// <para>
    /// A value constructed with "new ZonedDateTime()" will represent the Unix epoch
    /// in the ISO calendar system in the UTC time zone. That is the only situation in which
    /// the chronology is assumed rather than specified.
    /// </para>
    /// </remarks>
    public struct ZonedDateTime
    {
        private readonly Instant instant;
        private readonly Chronology chronology;

        public ZonedDateTime(LocalDateTime localDateTime, IDateTimeZone zone)
        {
            if (zone == null)
            {
                throw new ArgumentNullException("zone");
            } 
            instant = ConvertLocalToUtc(localDateTime.LocalInstant, zone);
            chronology = new Chronology(zone, localDateTime.Calendar);
        }

        public ZonedDateTime(Instant instant, Chronology chronology)
        {
            if (chronology == null)
            {
                throw new ArgumentNullException("chronology");
            }
            this.instant = instant;
            this.chronology = chronology;
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             IDateTimeZone zone)
            : this(year, month, day, hour, minute, second, Chronology.IsoForZone(zone))
        {
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             Chronology chronology)
            : this(year, month, day, hour, minute, second, 0, 0, chronology)
        {
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, IDateTimeZone zone)
            : this(year, month, day, hour, minute, second, millisecond, Chronology.IsoForZone(zone))
        {
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, Chronology chronology)
            : this(year, month, day, hour, minute, second, millisecond, 0, chronology)
        {
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, int tick, IDateTimeZone zone)
            : this(year, month, day, hour, minute, second, millisecond, tick, Chronology.IsoForZone(zone))
        {
        }

        public ZonedDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             int millisecond, int tick, Chronology chronology)
        {
            if (chronology == null)
            {
                throw new ArgumentNullException("chronology");
            }
            LocalInstant localInstant = chronology.Calendar.GetLocalInstant(year, month, day, hour, minute, second, millisecond, tick);
            instant = ConvertLocalToUtc(localInstant, chronology.Zone);
            this.chronology = chronology;
        }

        private static Instant ConvertLocalToUtc(LocalInstant localInstant, IDateTimeZone zone)
        {
            Offset offset = zone.GetOffsetFromLocal(localInstant);
            Instant instant = localInstant - offset;
            if (offset != zone.GetOffsetFromUtc(instant))
            {
                throw new ArgumentException();
            }
            return instant;
        }

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
            return instant;
        }

        /// <summary>
        /// Returns the chronology associated with this date and time.
        /// </summary>
        public Chronology Chronology { get { return chronology; } }

        public IDateTimeZone Zone { get { return Chronology.Zone; } }

        public LocalInstant LocalInstant { get { return instant + Zone.GetOffsetFromUtc(instant); } }

        public LocalDateTime LocalDateTime { get { return new LocalDateTime(LocalInstant, chronology.Calendar); } }

        public int Era { get { return LocalDateTime.Era; } }
        public int CenturyOfEra { get { return LocalDateTime.CenturyOfEra; } }
        public int Year { get { return LocalDateTime.Year; } }
        public int YearOfCentury { get { return LocalDateTime.YearOfCentury; } }
        public int YearOfEra { get { return LocalDateTime.YearOfEra; } }
        public int WeekYear { get { return LocalDateTime.WeekYear; } }
        public int MonthOfYear { get { return LocalDateTime.MonthOfYear; } }
        public int WeekOfWeekYear { get { return LocalDateTime.WeekOfWeekYear; } }
        public int DayOfYear { get { return LocalDateTime.DayOfYear; } }
        public int DayOfMonth { get { return LocalDateTime.DayOfMonth; } }
        public int DayOfWeek { get { return LocalDateTime.DayOfWeek; } }
        public int HourOfDay { get { return LocalDateTime.HourOfDay; } }
        public int MinuteOfHour { get { return LocalDateTime.MinuteOfHour; } }
        public int SecondOfMinute { get { return LocalDateTime.SecondOfMinute; } }
        public int SecondOfDay { get { return LocalDateTime.SecondOfDay; } }
        public int MillisecondOfSecond { get { return LocalDateTime.MillisecondOfSecond; } }
        public int MillisecondOfDay { get { return LocalDateTime.MillisecondOfDay; } }
        public int TickOfMillisecond { get { return LocalDateTime.TickOfMillisecond; } }
        public long TickOfDay { get { return LocalDateTime.TickOfDay; } }
    }
}
