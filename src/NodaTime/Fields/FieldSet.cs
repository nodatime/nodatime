#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
    internal sealed class FieldSet
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

        private readonly DateTimeField tickOfSecond;
        private readonly DateTimeField tickOfMillisecond;
        private readonly DateTimeField tickOfDay;
        private readonly DateTimeField millisecondOfSecond;
        private readonly DateTimeField millisecondOfDay;
        private readonly DateTimeField secondOfMinute;
        private readonly DateTimeField secondOfDay;
        private readonly DateTimeField minuteOfHour;
        private readonly DateTimeField minuteOfDay;
        private readonly DateTimeField hourOfDay;
        private readonly DateTimeField clockHourOfDay;
        private readonly DateTimeField hourOfHalfDay;
        private readonly DateTimeField clockHourOfHalfDay;
        private readonly DateTimeField halfDayOfDay;
        private readonly DateTimeField dayOfWeek;
        private readonly DateTimeField dayOfMonth;
        private readonly DateTimeField dayOfYear;
        private readonly DateTimeField weekOfWeekYear;
        private readonly DateTimeField weekYear;
        private readonly DateTimeField weekYearOfCentury;
        private readonly DateTimeField monthOfYear;
        private readonly DateTimeField year;
        private readonly DateTimeField yearOfCentury;
        private readonly DateTimeField yearOfEra;
        private readonly DateTimeField centruryOfEra;
        private readonly DateTimeField era;

        internal DurationField Ticks { get { return ticks; } }
        internal DurationField Milliseconds { get { return milliseconds; } }
        internal DurationField Seconds { get { return seconds; } }
        internal DurationField Minutes { get { return minutes; } }
        internal DurationField Hours { get { return hours; } }
        internal DurationField HalfDays { get { return halfDays; } }
        internal DurationField Days { get { return days; } }
        internal DurationField Weeks { get { return weeks; } }
        internal DurationField WeekYears { get { return weekYears; } }
        internal DurationField Months { get { return months; } }
        internal DurationField Years { get { return years; } }
        internal DurationField Centuries { get { return centuries; } }
        internal DurationField Eras { get { return eras; } }

        internal DateTimeField TickOfSecond { get { return tickOfSecond; } }
        internal DateTimeField TickOfMillisecond { get { return tickOfMillisecond; } }
        internal DateTimeField TickOfDay { get { return tickOfDay; } }
        internal DateTimeField MillisecondOfSecond { get { return millisecondOfSecond; } }
        internal DateTimeField MillisecondOfDay { get { return millisecondOfDay; } }
        internal DateTimeField SecondOfMinute { get { return secondOfMinute; } }
        internal DateTimeField SecondOfDay { get { return secondOfDay; } }
        internal DateTimeField MinuteOfHour { get { return minuteOfHour; } }
        internal DateTimeField MinuteOfDay { get { return minuteOfDay; } }
        internal DateTimeField HourOfDay { get { return hourOfDay; } }
        internal DateTimeField ClockHourOfDay { get { return clockHourOfDay; } }
        internal DateTimeField HourOfHalfDay { get { return hourOfHalfDay; } }
        internal DateTimeField ClockHourOfHalfDay { get { return clockHourOfHalfDay; } }
        internal DateTimeField HalfDayOfDay { get { return halfDayOfDay; } }
        internal DateTimeField DayOfWeek { get { return dayOfWeek; } }
        internal DateTimeField DayOfMonth { get { return dayOfMonth; } }
        internal DateTimeField DayOfYear { get { return dayOfYear; } }
        internal DateTimeField WeekOfWeekYear { get { return weekOfWeekYear; } }
        internal DateTimeField WeekYear { get { return weekYear; } }
        internal DateTimeField WeekYearOfCentury { get { return weekYearOfCentury; } }
        internal DateTimeField MonthOfYear { get { return monthOfYear; } }
        internal DateTimeField Year { get { return year; } }
        internal DateTimeField YearOfCentury { get { return yearOfCentury; } }
        internal DateTimeField YearOfEra { get { return yearOfEra; } }
        internal DateTimeField CenturyOfEra { get { return centruryOfEra; } }
        internal DateTimeField Era { get { return era; } }

        private FieldSet(Builder builder)
        {
            ticks = builder.Ticks ?? UnsupportedDurationField.Ticks;
            milliseconds = builder.Milliseconds ?? UnsupportedDurationField.Milliseconds;
            seconds = builder.Seconds ?? UnsupportedDurationField.Seconds;
            minutes = builder.Minutes ?? UnsupportedDurationField.Minutes;
            hours = builder.Hours ?? UnsupportedDurationField.Hours;
            halfDays = builder.HalfDays ?? UnsupportedDurationField.HalfDays;
            days = builder.Days ?? UnsupportedDurationField.Days;
            weeks = builder.Weeks ?? UnsupportedDurationField.Weeks;
            weekYears = builder.WeekYears ?? UnsupportedDurationField.WeekYears;
            months = builder.Months ?? UnsupportedDurationField.Months;
            years = builder.Years ?? UnsupportedDurationField.Years;
            centuries = builder.Centuries ?? UnsupportedDurationField.Centuries;
            eras = builder.Eras ?? UnsupportedDurationField.Eras;

            tickOfSecond = builder.TickOfSecond ?? UnsupportedDateTimeField.TickOfSecond;
            tickOfMillisecond = builder.TickOfMillisecond ?? UnsupportedDateTimeField.TickOfMillisecond;
            tickOfDay = builder.TickOfDay ?? UnsupportedDateTimeField.TickOfDay;
            millisecondOfSecond = builder.MillisecondOfSecond ?? UnsupportedDateTimeField.MillisecondOfSecond;
            millisecondOfDay = builder.MillisecondOfDay ?? UnsupportedDateTimeField.MillisecondOfDay;
            secondOfMinute = builder.SecondOfMinute ?? UnsupportedDateTimeField.SecondOfMinute;
            secondOfDay = builder.SecondOfDay ?? UnsupportedDateTimeField.SecondOfDay;
            minuteOfHour = builder.MinuteOfHour ?? UnsupportedDateTimeField.MinuteOfHour;
            minuteOfDay = builder.MinuteOfDay ?? UnsupportedDateTimeField.MinuteOfDay;
            hourOfDay = builder.HourOfDay ?? UnsupportedDateTimeField.HourOfDay;
            clockHourOfDay = builder.ClockHourOfDay ?? UnsupportedDateTimeField.ClockHourOfDay;
            hourOfHalfDay = builder.HourOfHalfDay ?? UnsupportedDateTimeField.HourOfHalfDay;
            clockHourOfHalfDay = builder.ClockHourOfHalfDay ?? UnsupportedDateTimeField.ClockHourOfHalfDay;
            halfDayOfDay = builder.HalfDayOfDay ?? UnsupportedDateTimeField.HalfDayOfDay;
            dayOfWeek = builder.DayOfWeek ?? UnsupportedDateTimeField.DayOfWeek;
            dayOfMonth = builder.DayOfMonth ?? UnsupportedDateTimeField.DayOfMonth;
            dayOfYear = builder.DayOfYear ?? UnsupportedDateTimeField.DayOfYear;
            weekOfWeekYear = builder.WeekOfWeekYear ?? UnsupportedDateTimeField.WeekOfWeekYear;
            weekYear = builder.WeekYear ?? UnsupportedDateTimeField.WeekYear;
            weekYearOfCentury = builder.WeekYearOfCentury ?? UnsupportedDateTimeField.WeekYearOfCentury;
            monthOfYear = builder.MonthOfYear ?? UnsupportedDateTimeField.MonthOfYear;
            year = builder.Year ?? UnsupportedDateTimeField.Year;
            yearOfCentury = builder.YearOfCentury ?? UnsupportedDateTimeField.YearOfCentury;
            yearOfEra = builder.YearOfEra ?? UnsupportedDateTimeField.YearOfEra;
            centruryOfEra = builder.CenturyOfEra ?? UnsupportedDateTimeField.CenturyOfEra;
            era = builder.Era ?? UnsupportedDateTimeField.Era;
        }

        /// <summary>
        /// Convenience method to create a new field set with
        /// the current field set as a "base" overridden with
        /// supported fields from the given set.
        /// </summary>
        internal FieldSet WithSupportedFieldsFrom(FieldSet fields)
        {
            return new Builder(this).WithSupportedFieldsFrom(fields).Build();
        }

        /// <summary>
        /// Mutable set of fields which can be built into a full, immutable FieldSet.
        /// </summary>
        internal class Builder
        {
            internal DurationField Ticks { get; set; }
            internal DurationField Milliseconds { get; set; }
            internal DurationField Seconds { get; set; }
            internal DurationField Minutes { get; set; }
            internal DurationField Hours { get; set; }
            internal DurationField HalfDays { get; set; }
            internal DurationField Days { get; set; }
            internal DurationField Weeks { get; set; }
            internal DurationField WeekYears { get; set; }
            internal DurationField Months { get; set; }
            internal DurationField Years { get; set; }
            internal DurationField Centuries { get; set; }
            internal DurationField Eras { get; set; }

            internal DateTimeField TickOfSecond { get; set; }
            internal DateTimeField TickOfMillisecond { get; set; }
            internal DateTimeField TickOfDay { get; set; }
            internal DateTimeField MillisecondOfSecond { get; set; }
            internal DateTimeField MillisecondOfDay { get; set; }
            internal DateTimeField SecondOfMinute { get; set; }
            internal DateTimeField SecondOfDay { get; set; }
            internal DateTimeField MinuteOfHour { get; set; }
            internal DateTimeField MinuteOfDay { get; set; }
            internal DateTimeField HourOfDay { get; set; }
            internal DateTimeField ClockHourOfDay { get; set; }
            internal DateTimeField HourOfHalfDay { get; set; }
            internal DateTimeField ClockHourOfHalfDay { get; set; }
            internal DateTimeField HalfDayOfDay { get; set; }
            internal DateTimeField DayOfWeek { get; set; }
            internal DateTimeField DayOfMonth { get; set; }
            internal DateTimeField DayOfYear { get; set; }
            internal DateTimeField WeekOfWeekYear { get; set; }
            internal DateTimeField WeekYear { get; set; }
            internal DateTimeField WeekYearOfCentury { get; set; }
            internal DateTimeField MonthOfYear { get; set; }
            internal DateTimeField Year { get; set; }
            internal DateTimeField YearOfCentury { get; set; }
            internal DateTimeField YearOfEra { get; set; }
            internal DateTimeField CenturyOfEra { get; set; }
            internal DateTimeField Era { get; set; }

            internal Builder()
            {
            }

            internal Builder(FieldSet baseSet)
            {
                if (baseSet == null)
                {
                    throw new ArgumentNullException("baseSet");
                }
                Ticks = baseSet.Ticks;
                Milliseconds = baseSet.Milliseconds;
                Seconds = baseSet.Seconds;
                Minutes = baseSet.Minutes;
                Hours = baseSet.Hours;
                HalfDays = baseSet.HalfDays;
                Days = baseSet.Days;
                Weeks = baseSet.Weeks;
                WeekYears = baseSet.WeekYears;
                Months = baseSet.Months;
                Years = baseSet.Years;
                Centuries = baseSet.Centuries;
                Eras = baseSet.Eras;

                TickOfSecond = baseSet.TickOfSecond;
                TickOfMillisecond = baseSet.TickOfMillisecond;
                TickOfDay = baseSet.TickOfDay;
                MillisecondOfSecond = baseSet.MillisecondOfSecond;
                MillisecondOfDay = baseSet.MillisecondOfDay;
                SecondOfMinute = baseSet.SecondOfMinute;
                SecondOfDay = baseSet.SecondOfDay;
                MinuteOfHour = baseSet.MinuteOfHour;
                MinuteOfDay = baseSet.MinuteOfDay;
                HourOfDay = baseSet.HourOfDay;
                ClockHourOfDay = baseSet.ClockHourOfDay;
                HourOfHalfDay = baseSet.HourOfHalfDay;
                ClockHourOfHalfDay = baseSet.ClockHourOfHalfDay;
                HalfDayOfDay = baseSet.HalfDayOfDay;
                DayOfWeek = baseSet.DayOfWeek;
                DayOfMonth = baseSet.DayOfMonth;
                DayOfYear = baseSet.DayOfYear;
                WeekOfWeekYear = baseSet.WeekOfWeekYear;
                WeekYear = baseSet.WeekYear;
                WeekYearOfCentury = baseSet.WeekYearOfCentury;
                MonthOfYear = baseSet.MonthOfYear;
                Year = baseSet.Year;
                YearOfCentury = baseSet.YearOfCentury;
                YearOfEra = baseSet.YearOfEra;
                CenturyOfEra = baseSet.CenturyOfEra;
                Era = baseSet.Era;
            }

            /// <summary>
            /// Copies just the supported fields from the specified set into this builder,
            /// and returns this builder again (for fluent building).
            /// </summary>
            /// <param name="other">The set of fields to copy.</param>
            internal Builder WithSupportedFieldsFrom(FieldSet other)
            {
                if (other == null)
                {
                    throw new ArgumentNullException("other");
                }
                Ticks = other.Ticks.IsSupported ? other.Ticks : Ticks;
                Milliseconds = other.Milliseconds.IsSupported ? other.Milliseconds : Milliseconds;
                Seconds = other.Seconds.IsSupported ? other.Seconds : Seconds;
                Minutes = other.Minutes.IsSupported ? other.Minutes : Minutes;
                Hours = other.Hours.IsSupported ? other.Hours : Hours;
                HalfDays = other.HalfDays.IsSupported ? other.HalfDays : HalfDays;
                Days = other.Days.IsSupported ? other.Days : Days;
                Weeks = other.Weeks.IsSupported ? other.Weeks : Weeks;
                WeekYears = other.WeekYears.IsSupported ? other.WeekYears : WeekYears;
                Months = other.Months.IsSupported ? other.Months : Months;
                Years = other.Years.IsSupported ? other.Years : Years;
                Centuries = other.Centuries.IsSupported ? other.Centuries : Centuries;
                Eras = other.Eras.IsSupported ? other.Eras : Eras;

                TickOfSecond = other.TickOfSecond.IsSupported ? other.TickOfSecond : TickOfSecond;
                TickOfMillisecond = other.TickOfMillisecond.IsSupported ? other.TickOfMillisecond : TickOfMillisecond;
                TickOfDay = other.TickOfDay.IsSupported ? other.TickOfDay : TickOfDay;
                MillisecondOfSecond = other.MillisecondOfSecond.IsSupported ? other.MillisecondOfSecond : MillisecondOfSecond;
                MillisecondOfDay = other.MillisecondOfDay.IsSupported ? other.MillisecondOfDay : MillisecondOfDay;
                SecondOfMinute = other.SecondOfMinute.IsSupported ? other.SecondOfMinute : SecondOfMinute;
                SecondOfDay = other.SecondOfDay.IsSupported ? other.SecondOfDay : SecondOfDay;
                MinuteOfHour = other.MinuteOfHour.IsSupported ? other.MinuteOfHour : MinuteOfHour;
                MinuteOfDay = other.MinuteOfDay.IsSupported ? other.MinuteOfDay : MinuteOfDay;
                HourOfDay = other.HourOfDay.IsSupported ? other.HourOfDay : HourOfDay;
                ClockHourOfDay = other.ClockHourOfDay.IsSupported ? other.ClockHourOfDay : ClockHourOfDay;
                HourOfHalfDay = other.HourOfHalfDay.IsSupported ? other.HourOfHalfDay : HourOfHalfDay;
                ClockHourOfHalfDay = other.ClockHourOfHalfDay.IsSupported ? other.ClockHourOfHalfDay : ClockHourOfHalfDay;
                HalfDayOfDay = other.HalfDayOfDay.IsSupported ? other.HalfDayOfDay : HalfDayOfDay;
                DayOfWeek = other.DayOfWeek.IsSupported ? other.DayOfWeek : DayOfWeek;
                DayOfMonth = other.DayOfMonth.IsSupported ? other.DayOfMonth : DayOfMonth;
                DayOfYear = other.DayOfYear.IsSupported ? other.DayOfYear : DayOfYear;
                WeekOfWeekYear = other.WeekOfWeekYear.IsSupported ? other.WeekOfWeekYear : WeekOfWeekYear;
                WeekYear = other.WeekYear.IsSupported ? other.WeekYear : WeekYear;
                WeekYearOfCentury = other.WeekYearOfCentury.IsSupported ? other.WeekYearOfCentury : WeekYearOfCentury;
                MonthOfYear = other.MonthOfYear.IsSupported ? other.MonthOfYear : MonthOfYear;
                Year = other.Year.IsSupported ? other.Year : Year;
                YearOfCentury = other.YearOfCentury.IsSupported ? other.YearOfCentury : YearOfCentury;
                YearOfEra = other.YearOfEra.IsSupported ? other.YearOfEra : YearOfEra;
                CenturyOfEra = other.CenturyOfEra.IsSupported ? other.CenturyOfEra : CenturyOfEra;
                Era = other.Era.IsSupported ? other.Era : Era;
                return this;
            }

            internal FieldSet Build()
            {
                return new FieldSet(this);
            }
        }
    }
}