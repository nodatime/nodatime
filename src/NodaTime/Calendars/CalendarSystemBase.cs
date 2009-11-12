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

namespace NodaTime.Calendars
{
    /// <summary>
    /// Original name: BaseChronology
    /// </summary>
    public abstract class CalendarSystemBase : ICalendarSystem
    {
        public abstract DateTimeField Era { get; }
        public abstract DateTimeField CenturyOfEra { get; }
        public abstract DateTimeField YearOfCentury { get; }
        public abstract DateTimeField YearOfEra { get; }
        public abstract DateTimeField Year { get; }
        public abstract DateTimeField DayOfMonth { get; }
        public abstract DateTimeField MonthOfYear { get; }
        public abstract DateTimeField Weekyear { get; }
        public abstract DateTimeField WeekOfWeekyear { get; }
        public abstract DateTimeField DayOfWeek { get; }
        public abstract DateTimeField DayOfYear { get; }
        public abstract DateTimeField HalfdayOfDay { get; }
        public abstract DateTimeField HourOfHalfday { get; }
        public abstract DateTimeField ClockhourOfDay { get; }
        public abstract DateTimeField ClockhourOfHalfday { get; }
        public abstract DateTimeField HourOfDay { get; }
        public abstract DateTimeField MinuteOfHour { get; }
        public abstract DateTimeField MinuteOfDay { get; }
        public abstract DateTimeField SecondOfMinute { get; }
        public abstract DateTimeField SecondOfDay { get; }
        public abstract DateTimeField MillisecondsOfSecond { get; }
        public abstract DateTimeField MillisecondsOfDay { get; }
    }
}
