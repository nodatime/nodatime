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
namespace NodaTime.Fields
{
    /// <summary>
    /// Type of a date time field. This is a "smart enum" type.
    /// </summary>
    public sealed class DateTimeFieldType
    {    
        public static readonly DateTimeFieldType Era = new DateTimeFieldType("Era", DurationFieldType.Eras, null, 0);
        public static readonly DateTimeFieldType YearOfEra = new DateTimeFieldType("YearOfEra", DurationFieldType.Years, DurationFieldType.Eras, 1);
        public static readonly DateTimeFieldType CenturyOfEra = new DateTimeFieldType("CenturyOfEra", DurationFieldType.Centuries, DurationFieldType.Eras, 2);
        public static readonly DateTimeFieldType YearOfCentury = new DateTimeFieldType("YearOfCentury", DurationFieldType.Years, DurationFieldType.Centuries, 3);
        public static readonly DateTimeFieldType Year = new DateTimeFieldType("Year", DurationFieldType.Years, null, 4);
        public static readonly DateTimeFieldType DayOfYear = new DateTimeFieldType("DayOfYear", DurationFieldType.Days, DurationFieldType.Years, 5);
        public static readonly DateTimeFieldType MonthOfYear = new DateTimeFieldType("MonthOfYear", DurationFieldType.Months, DurationFieldType.Years, 6);
        public static readonly DateTimeFieldType DayOfMonth = new DateTimeFieldType("DayOfMonth", DurationFieldType.Days, DurationFieldType.Months, 7);
        public static readonly DateTimeFieldType WeekYearOfCentury = new DateTimeFieldType("WeekYearOfCentury", DurationFieldType.WeekYears, DurationFieldType.Centuries, 8);
        public static readonly DateTimeFieldType WeekYear = new DateTimeFieldType("WeekYear", DurationFieldType.WeekYears, null, 9);
        public static readonly DateTimeFieldType WeekOfWeekYear = new DateTimeFieldType("WeekOfWeekYear", DurationFieldType.Weeks, DurationFieldType.WeekYears, 10);
        public static readonly DateTimeFieldType DayOfWeek = new DateTimeFieldType("DayOfWeek", DurationFieldType.Days, DurationFieldType.Weeks, 11);
        public static readonly DateTimeFieldType HalfDayOfDay = new DateTimeFieldType("HalfDayOfDay", DurationFieldType.HalfDays, DurationFieldType.Days, 12);
        public static readonly DateTimeFieldType HourOfHalfDay = new DateTimeFieldType("HourOfHalfDay", DurationFieldType.Hours, DurationFieldType.HalfDays, 13);
        public static readonly DateTimeFieldType ClockHourOfHalfDay = new DateTimeFieldType("ClockHourOfHalfDay", DurationFieldType.Hours, DurationFieldType.Days, 14);
        public static readonly DateTimeFieldType ClockHourOfDay = new DateTimeFieldType("ClockHourOfDay", DurationFieldType.Hours, DurationFieldType.HalfDays, 15);
        public static readonly DateTimeFieldType HourOfDay = new DateTimeFieldType("HourOfDay", DurationFieldType.Hours, DurationFieldType.Days, 16);
        public static readonly DateTimeFieldType MinuteOfDay = new DateTimeFieldType("MinuteOfDay", DurationFieldType.Minutes, DurationFieldType.Days, 17);
        public static readonly DateTimeFieldType MinuteOfHour = new DateTimeFieldType("MinuteOfHour", DurationFieldType.Minutes, DurationFieldType.Hours, 18);
        public static readonly DateTimeFieldType SecondOfMinute = new DateTimeFieldType("SecondOfMinute", DurationFieldType.Seconds, DurationFieldType.Minutes, 19);
        public static readonly DateTimeFieldType SecondOfDay = new DateTimeFieldType("SecondOfDay", DurationFieldType.Seconds, DurationFieldType.Days, 20);
        public static readonly DateTimeFieldType MillisecondOfSecond = new DateTimeFieldType("MillisecondOfSecond", DurationFieldType.Milliseconds, DurationFieldType.Seconds, 21);
        public static readonly DateTimeFieldType MillisecondOfDay = new DateTimeFieldType("MillisecondOfDay", DurationFieldType.Milliseconds, DurationFieldType.Days, 22);
        public static readonly DateTimeFieldType TickOfMillisecond = new DateTimeFieldType("TickOfMillisecond", DurationFieldType.Ticks, DurationFieldType.Milliseconds, 23);
        public static readonly DateTimeFieldType TickOfDay = new DateTimeFieldType("TickOfDay", DurationFieldType.Ticks, DurationFieldType.Days, 24);

        internal static readonly int MaxOrdinal = 24; // Update this if new types are ever added.

        private readonly string name;
        private readonly DurationFieldType durationFieldType;
        private readonly DurationFieldType? rangeDurationFieldType;
        private readonly int ordinal;
        
        private DateTimeFieldType(string name, DurationFieldType durationFieldType, DurationFieldType? rangeDurationFieldType, int ordinal)
        {
            this.name = name;
            this.durationFieldType = durationFieldType;
            this.rangeDurationFieldType = rangeDurationFieldType;
            this.ordinal = ordinal;
        }

        public int Ordinal { get { return ordinal; } }
        public DurationFieldType DurationFieldType { get { return durationFieldType; } }
        public DurationFieldType? RangeDurationFieldType { get { return rangeDurationFieldType; } }

        public override string ToString()
        {
            return name;
        }
    }
}