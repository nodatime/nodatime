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

namespace NodaTime.Fields
{
    /// <summary>
    /// An immutable collection of date/time and duration fields.
    /// </summary>
    public sealed class FieldSet
    {
        private readonly DurationField ticks;
        private readonly DurationField milliseconds;
        private readonly DurationField seconds;
        private readonly DurationField minutes;
        private readonly DurationField hours;
        private readonly DurationField halfDays;
        private readonly DurationField days;
        private readonly DurationField weeks;
        private readonly DurationField weekYears;
        private readonly DurationField months;
        private readonly DurationField years;
        private readonly DurationField centuries;
        private readonly DurationField eras;

        private readonly IDateTimeField tickOfMillisecond;
        private readonly IDateTimeField tickOfDay;
        private readonly IDateTimeField millisecondOfSecond;
        private readonly IDateTimeField millisecondOfDay;
        private readonly IDateTimeField secondOfMinute;
        private readonly IDateTimeField secondOfDay;
        private readonly IDateTimeField minuteOfHour;
        private readonly IDateTimeField minuteOfDay;
        private readonly IDateTimeField hourOfDay;
        private readonly IDateTimeField clockHourOfDay;
        private readonly IDateTimeField hourOfHalfDay;
        private readonly IDateTimeField clockHourOfHalfDay;
        private readonly IDateTimeField halfDayOfDay;
        private readonly IDateTimeField dayOfWeek;
        private readonly IDateTimeField dayOfMonth;
        private readonly IDateTimeField dayOfYear;
        private readonly IDateTimeField weekOfWeekYear;
        private readonly IDateTimeField weekYear;
        private readonly IDateTimeField weekYearOfCentury;
        private readonly IDateTimeField monthOfYear;
        private readonly IDateTimeField year;
        private readonly IDateTimeField yearOfCentury;
        private readonly IDateTimeField yearOfEra;
        private readonly IDateTimeField centruryOfEra;
        private readonly IDateTimeField era;

        public DurationField Ticks { get { return ticks; } }
        public DurationField Milliseconds { get { return milliseconds; } }
        public DurationField Seconds { get { return seconds; } }
        public DurationField Minutes { get { return minutes; } }
        public DurationField Hours { get { return hours; } }
        public DurationField HalfDays { get { return halfDays; } }
        public DurationField Days { get { return days; } }
        public DurationField Weeks { get { return weeks; } }
        public DurationField WeekYears { get { return weekYears; } }
        public DurationField Months { get { return months; } }
        public DurationField Years { get { return years; } }
        public DurationField Centuries { get { return centuries; } }
        public DurationField Eras { get { return eras; } }

        public IDateTimeField TickOfMillisecond { get { return tickOfMillisecond; } }
        public IDateTimeField TickOfDay { get { return tickOfDay; } }
        public IDateTimeField MillisecondOfSecond { get { return millisecondOfSecond; } }
        public IDateTimeField MillisecondOfDay { get { return millisecondOfDay; } }
        public IDateTimeField SecondOfMinute { get { return secondOfMinute; } }
        public IDateTimeField SecondOfDay { get { return secondOfDay; } }
        public IDateTimeField MinuteOfHour { get { return minuteOfHour; } }
        public IDateTimeField MinuteOfDay { get { return minuteOfDay; } }
        public IDateTimeField HourOfDay { get { return hourOfDay; } }
        public IDateTimeField ClockHourOfDay { get { return clockHourOfDay; } }
        public IDateTimeField HourOfHalfDay { get { return hourOfHalfDay; } }
        public IDateTimeField ClockHourOfHalfDay { get { return clockHourOfHalfDay; } }
        public IDateTimeField HalfDayOfDay { get { return halfDayOfDay; } }
        public IDateTimeField DayOfWeek { get { return dayOfWeek; } }
        public IDateTimeField DayOfMonth { get { return dayOfMonth; } }
        public IDateTimeField DayOfYear { get { return dayOfYear; } }
        public IDateTimeField WeekOfWeekYear { get { return weekOfWeekYear; } }
        public IDateTimeField WeekYear { get { return weekYear; } }
        public IDateTimeField WeekYearOfCentury { get { return weekYearOfCentury; } }
        public IDateTimeField MonthOfYear { get { return monthOfYear; } }
        public IDateTimeField Year { get { return year; } }
        public IDateTimeField YearOfCentury { get { return yearOfCentury; } }
        public IDateTimeField YearOfEra { get { return yearOfEra; } }
        public IDateTimeField CenturyOfEra { get { return centruryOfEra; } }
        public IDateTimeField Era { get { return era; } }

        private FieldSet(Builder builder)
        {
            ticks = builder.Ticks;
            milliseconds = builder.Milliseconds;
            seconds = builder.Seconds;
            minutes = builder.Minutes;
            hours = builder.Hours;
            halfDays = builder.HalfDays;
            days = builder.Days;
            weeks = builder.Weeks;
            weekYears = builder.WeekYears;
            months = builder.Months;
            years = builder.Years;
            centuries = builder.Centuries;
            eras = builder.Eras;

            tickOfMillisecond = builder.TickOfMillisecond;
            tickOfDay = builder.TickOfDay;
            millisecondOfSecond = builder.MillisecondOfSecond;
            millisecondOfDay = builder.MillisecondOfDay;
            secondOfMinute = builder.SecondOfMinute;
            secondOfDay = builder.SecondOfDay;
            minuteOfHour = builder.MinuteOfHour;
            minuteOfDay = builder.MinuteOfDay;
            hourOfDay = builder.HourOfDay;
            clockHourOfDay = builder.ClockHourOfDay;
            hourOfHalfDay = builder.HourOfHalfDay;
            clockHourOfHalfDay = builder.ClockHourOfHalfDay;
            halfDayOfDay = builder.HalfDayOfDay;
            dayOfWeek = builder.DayOfWeek;
            dayOfMonth = builder.DayOfMonth;
            dayOfYear = builder.DayOfYear;
            weekOfWeekYear = builder.WeekOfWeekYear;
            weekYear = builder.WeekYear;
            weekYearOfCentury = builder.WeekYearOfCentury;
            monthOfYear = builder.MonthOfYear;
            year = builder.Year;
            yearOfCentury = builder.YearOfCentury;
            yearOfEra = builder.YearOfEra;
            centruryOfEra = builder.CenturyOfEra;
            era = builder.Era;
        }

        // TODO: Consider making FieldSet privately mutable and mutate it directly in the builder.
        // Pros: Less copying
        // Cons: Builders aren't reusable, and FieldSet isn't as obviously thread-safe.
        public class Builder
        {
            public DurationField Ticks { get; set; }
            public DurationField Milliseconds { get; set; }
            public DurationField Seconds { get; set; }
            public DurationField Minutes { get; set; }
            public DurationField Hours { get; set; }
            public DurationField HalfDays { get; set; }
            public DurationField Days { get; set; }
            public DurationField Weeks { get; set; }
            public DurationField WeekYears { get; set; }
            public DurationField Months { get; set; }
            public DurationField Years { get; set; }
            public DurationField Centuries { get; set; }
            public DurationField Eras { get; set; }

            public IDateTimeField TickOfMillisecond { get; set; }
            public IDateTimeField TickOfDay { get; set; }
            public IDateTimeField MillisecondOfSecond { get; set; }
            public IDateTimeField MillisecondOfDay { get; set; }
            public IDateTimeField SecondOfMinute { get; set; }
            public IDateTimeField SecondOfDay { get; set; }
            public IDateTimeField MinuteOfHour { get; set; }
            public IDateTimeField MinuteOfDay { get; set; }
            public IDateTimeField HourOfDay { get; set; }
            public IDateTimeField ClockHourOfDay { get; set; }
            public IDateTimeField HourOfHalfDay { get; set; }
            public IDateTimeField ClockHourOfHalfDay { get; set; }
            public IDateTimeField HalfDayOfDay { get; set; }
            public IDateTimeField DayOfWeek { get; set; }
            public IDateTimeField DayOfMonth { get; set; }
            public IDateTimeField DayOfYear { get; set; }
            public IDateTimeField WeekOfWeekYear { get; set; }
            public IDateTimeField WeekYear { get; set; }
            public IDateTimeField WeekYearOfCentury { get; set; }
            public IDateTimeField MonthOfYear { get; set; }
            public IDateTimeField Year { get; set; }
            public IDateTimeField YearOfCentury { get; set; }
            public IDateTimeField YearOfEra { get; set; }
            public IDateTimeField CenturyOfEra { get; set; }
            public IDateTimeField Era { get; set; }

            public FieldSet ToFieldSet()
            {
                return new FieldSet(this);
            }
        }
    }
}
