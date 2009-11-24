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

using NodaTime.Calendars;

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
        private readonly IDateTimeZone zone;
        private readonly ICalendarSystem calendarSystem;

        public static Chronology IsoUtc { get { throw new NotImplementedException(); } }

        public IDateTimeZone Zone { get { return zone; } }
        public ICalendarSystem CalendarSystem { get { return calendarSystem; } }

        public Chronology(IDateTimeZone zone, ICalendarSystem calendarSystem)
        {
            this.zone = zone;
            this.calendarSystem = calendarSystem;
        }

        /*
        DateTimeZone Zone { get; }
        DateTimeField Era { get; }
        DateTimeField CenturyOfEra { get; }
        DateTimeField YearOfCentury { get; }
        DateTimeField YearOfEra { get; }
        DateTimeField Year { get; }
        DateTimeField DayOfMonth { get; }
        DateTimeField MonthOfYear { get; }
        DateTimeField Weekyear { get; }
        DateTimeField WeekOfWeekyear { get; }
        DateTimeField DayOfWeek { get; }
        DateTimeField DayOfYear { get; }
        DateTimeField HalfdayOfDay { get; }
        DateTimeField HourOfHalfday { get; }
        DateTimeField ClockhourOfDay { get; }
        DateTimeField ClockhourOfHalfday { get; }
        DateTimeField HourOfDay { get; }
        DateTimeField MinuteOfHour { get; }
        DateTimeField MinuteOfDay { get; }
        DateTimeField SecondOfMinute { get; }
        DateTimeField SecondOfDay { get; }
        DateTimeField MillisecondsOfSecond { get; }
        DateTimeField MillisecondsOfDay { get; }
         */
    }
}
