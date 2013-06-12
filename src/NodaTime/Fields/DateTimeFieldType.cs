// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Fields
{
    /// <summary>
    /// Type of a date time field. This is a "smart enum" type.
    /// </summary>
    internal sealed class DateTimeFieldType
    {
        public static readonly DateTimeFieldType Era = new DateTimeFieldType("Era", PeriodFieldType.Eras, null);
        public static readonly DateTimeFieldType YearOfEra = new DateTimeFieldType("YearOfEra", PeriodFieldType.Years, PeriodFieldType.Eras);
        public static readonly DateTimeFieldType CenturyOfEra = new DateTimeFieldType("CenturyOfEra", PeriodFieldType.Centuries, PeriodFieldType.Eras);
        public static readonly DateTimeFieldType YearOfCentury = new DateTimeFieldType("YearOfCentury", PeriodFieldType.Years, PeriodFieldType.Centuries);
        public static readonly DateTimeFieldType Year = new DateTimeFieldType("Year", PeriodFieldType.Years, null);
        public static readonly DateTimeFieldType DayOfYear = new DateTimeFieldType("DayOfYear", PeriodFieldType.Days, PeriodFieldType.Years);
        public static readonly DateTimeFieldType MonthOfYear = new DateTimeFieldType("MonthOfYear", PeriodFieldType.Months, PeriodFieldType.Years);
        public static readonly DateTimeFieldType DayOfMonth = new DateTimeFieldType("DayOfMonth", PeriodFieldType.Days, PeriodFieldType.Months);
        public static readonly DateTimeFieldType WeekYearOfCentury = new DateTimeFieldType("WeekYearOfCentury", PeriodFieldType.WeekYears, PeriodFieldType.Centuries);
        public static readonly DateTimeFieldType WeekYear = new DateTimeFieldType("WeekYear", PeriodFieldType.WeekYears, null);
        public static readonly DateTimeFieldType WeekOfWeekYear = new DateTimeFieldType("WeekOfWeekYear", PeriodFieldType.Weeks, PeriodFieldType.WeekYears);
        public static readonly DateTimeFieldType DayOfWeek = new DateTimeFieldType("DayOfWeek", PeriodFieldType.Days, PeriodFieldType.Weeks);
        public static readonly DateTimeFieldType HourOfHalfDay = new DateTimeFieldType("HourOfHalfDay", PeriodFieldType.Hours, PeriodFieldType.HalfDays);
        public static readonly DateTimeFieldType ClockHourOfHalfDay = new DateTimeFieldType("ClockHourOfHalfDay", PeriodFieldType.Hours, PeriodFieldType.Days);
        public static readonly DateTimeFieldType ClockHourOfDay = new DateTimeFieldType("ClockHourOfDay", PeriodFieldType.Hours, PeriodFieldType.HalfDays);
        public static readonly DateTimeFieldType HourOfDay = new DateTimeFieldType("HourOfDay", PeriodFieldType.Hours, PeriodFieldType.Days);
        public static readonly DateTimeFieldType MinuteOfDay = new DateTimeFieldType("MinuteOfDay", PeriodFieldType.Minutes, PeriodFieldType.Days);
        public static readonly DateTimeFieldType MinuteOfHour = new DateTimeFieldType("MinuteOfHour", PeriodFieldType.Minutes, PeriodFieldType.Hours);
        public static readonly DateTimeFieldType SecondOfMinute = new DateTimeFieldType("SecondOfMinute", PeriodFieldType.Seconds, PeriodFieldType.Minutes);
        public static readonly DateTimeFieldType SecondOfDay = new DateTimeFieldType("SecondOfDay", PeriodFieldType.Seconds, PeriodFieldType.Days);
        public static readonly DateTimeFieldType MillisecondOfSecond = new DateTimeFieldType("MillisecondOfSecond", PeriodFieldType.Milliseconds, PeriodFieldType.Seconds);
        public static readonly DateTimeFieldType MillisecondOfDay = new DateTimeFieldType("MillisecondOfDay", PeriodFieldType.Milliseconds, PeriodFieldType.Days);
        public static readonly DateTimeFieldType TickOfMillisecond = new DateTimeFieldType("TickOfMillisecond", PeriodFieldType.Ticks, PeriodFieldType.Milliseconds);
        public static readonly DateTimeFieldType TickOfDay = new DateTimeFieldType("TickOfDay", PeriodFieldType.Ticks, PeriodFieldType.Days);
        public static readonly DateTimeFieldType TickOfSecond = new DateTimeFieldType("TickOfSecond", PeriodFieldType.Ticks, PeriodFieldType.Seconds);

        private readonly string name;
        private readonly PeriodFieldType periodFieldType;
        private readonly PeriodFieldType? rangePeriodFieldType;

        private DateTimeFieldType(string name, PeriodFieldType periodFieldType, PeriodFieldType? rangePeriodFieldType)
        {
            this.name = name;
            this.periodFieldType = periodFieldType;
            this.rangePeriodFieldType = rangePeriodFieldType;
        }

        public PeriodFieldType PeriodFieldType { get { return periodFieldType; } }
        public PeriodFieldType? RangePeriodFieldType { get { return rangePeriodFieldType; } }

        public override string ToString()
        {
            return name;
        }
    }
}
