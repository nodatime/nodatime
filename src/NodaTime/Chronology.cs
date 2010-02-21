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

using NodaTime.Calendars;
using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// A chronology is a calendar system with an associated time zone, for example
    /// "the ISO calendar in the Europe/London time zone".
    /// TODO: Make this a struct? The hard work will be done in the calendar system
    /// and time zone classes.
    /// </summary>
    public sealed class Chronology
    {
        private static readonly Chronology isoUtc = new Chronology(DateTimeZones.Utc, IsoCalendarSystem.Instance);

        public static Chronology IsoUtc { get { return isoUtc; } }
        
        private readonly IDateTimeZone zone;
        private readonly ICalendarSystem calendarSystem;
        
        public IDateTimeZone Zone { get { return zone; } }
        public ICalendarSystem Calendar { get { return calendarSystem; } }

        public Chronology(IDateTimeZone zone, ICalendarSystem calendarSystem)
        {
            if (zone == null)
            {
                throw new ArgumentNullException("zone");
            }
            if (calendarSystem == null)
            {
                throw new ArgumentNullException("calendarSystem");
            }
            this.zone = zone;
            this.calendarSystem = calendarSystem;
        }

        /// <summary>
        /// Returns a chronology which uses the ISO chronology in the given time zone.
        /// </summary>
        internal static Chronology IsoForZone(IDateTimeZone zone)
        {
            return new Chronology(zone, IsoCalendarSystem.Instance);
        }
    }
}
