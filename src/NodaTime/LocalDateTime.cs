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

using NodaTime.Calendars;

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
    /// </summary>
    public struct LocalDateTime
    {
        private readonly LocalInstant localInstant;
        private ICalendarSystem calendar;

        public LocalDateTime(LocalInstant localInstant)
            : this(localInstant, CalendarSystems.Iso)
        {
        }

        public LocalDateTime(LocalInstant localInstant, ICalendarSystem calendar)
        {
            this.localInstant = localInstant;
            this.calendar = calendar;
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute)
            : this(year, month, day, hour, minute, 0, 0, 0, CalendarSystems.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, ICalendarSystem calendar)
            : this(year, month, day, hour, minute, 0, 0, 0, calendar)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second)
            : this(year, month, day, hour, minute, second, 0, 0, CalendarSystems.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second,
                             ICalendarSystem calendar)
            : this(year, month, day, hour, minute, second, 0, 0, calendar)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond)
            : this(year, month, day, hour, minute, second, millisecond, 0, CalendarSystems.Iso)
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
            : this(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond, CalendarSystems.Iso)
        {
        }

        public LocalDateTime(int year, int month, int day,
                             int hour, int minute, int second, int millisecond,
                             int tickWithinMillisecond, ICalendarSystem calendar)
        {
            localInstant = calendar.GetLocalInstant(year, month, day, hour, minute, second, millisecond, tickWithinMillisecond);
            this.calendar = calendar;
        }

        public int Era { get { return calendar.DateTimeFields[DateTimeFieldType.Era].GetValue(localInstant); } }
        public int CenturyOfEra { get { return calendar.DateTimeFields[DateTimeFieldType.CenturyOfEra].GetValue(localInstant); } }
        public int Year { get { return calendar.DateTimeFields[DateTimeFieldType.Year].GetValue(localInstant); } }
        public int YearOfCentury { get { return calendar.DateTimeFields[DateTimeFieldType.YearOfCentury].GetValue(localInstant); } }
        public int YearOfEra { get { return calendar.DateTimeFields[DateTimeFieldType.YearOfEra].GetValue(localInstant); } }
        public int WeekYear { get { return calendar.DateTimeFields[DateTimeFieldType.Weekyear].GetValue(localInstant); } }
        public int MonthOfYear { get { return calendar.DateTimeFields[DateTimeFieldType.MonthOfYear].GetValue(localInstant); } }
        public int WeekOfWeekYear { get { return calendar.DateTimeFields[DateTimeFieldType.WeekOfWeekyear].GetValue(localInstant); } }
        public int DayOfYear { get { return calendar.DateTimeFields[DateTimeFieldType.DayOfYear].GetValue(localInstant); } }
        public int DayOfMonth { get { return calendar.DateTimeFields[DateTimeFieldType.DayOfMonth].GetValue(localInstant); } }
        public int DayOfWeek { get { return calendar.DateTimeFields[DateTimeFieldType.DayOfWeek].GetValue(localInstant); } }
        public int HourOfDay { get { return calendar.DateTimeFields[DateTimeFieldType.HourOfDay].GetValue(localInstant); } }
        public int MinuteOfHour { get { return calendar.DateTimeFields[DateTimeFieldType.MinuteOfHour].GetValue(localInstant); } }
        public int SecondOfMinute { get { return calendar.DateTimeFields[DateTimeFieldType.SecondOfMinute].GetValue(localInstant); } }
        public int MillisecondsOfSecond { get { return calendar.DateTimeFields[DateTimeFieldType.MillisecondOfSecond].GetValue(localInstant); } }
        public int TicksOfMillicond { get { return calendar.DateTimeFields[DateTimeFieldType.TickOfMillisecond].GetValue(localInstant); } }
        public int TicksOfDay { get { return calendar.DateTimeFields[DateTimeFieldType.TickOfDay].GetValue(localInstant); } }
    }
}
