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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Original name: BaseChronology
    /// </summary>
    public abstract class CalendarSystemBase : ICalendarSystem
    {
        public abstract IDateTimeField Era { get; }
        public abstract IDateTimeField CenturyOfEra { get; }
        public abstract IDateTimeField YearOfCentury { get; }
        public abstract IDateTimeField YearOfEra { get; }
        public abstract IDateTimeField Year { get; }
        public abstract IDateTimeField DayOfMonth { get; }
        public abstract IDateTimeField MonthOfYear { get; }
        public abstract IDateTimeField Weekyear { get; }
        public abstract IDateTimeField WeekOfWeekyear { get; }
        public abstract IDateTimeField DayOfWeek { get; }
        public abstract IDateTimeField DayOfYear { get; }
        public abstract IDateTimeField HalfdayOfDay { get; }
        public abstract IDateTimeField HourOfHalfday { get; }
        public abstract IDateTimeField ClockhourOfDay { get; }
        public abstract IDateTimeField ClockhourOfHalfday { get; }
        public abstract IDateTimeField HourOfDay { get; }
        public abstract IDateTimeField MinuteOfHour { get; }
        public abstract IDateTimeField MinuteOfDay { get; }
        public abstract IDateTimeField SecondOfMinute { get; }
        public abstract IDateTimeField SecondOfDay { get; }
        public abstract IDateTimeField MillisecondsOfSecond { get; }
        public abstract IDateTimeField MillisecondsOfDay { get; }

        public LocalInstant GetLocalInstant(int year, int month, int day, int hour, 
            int minute, int second, int millisecond, int tickWithinMillisecond)
        {
            throw new NotImplementedException();
        }

        public FieldSet Fields
        {
            get { throw new NotImplementedException(); }
        }
    }
}
