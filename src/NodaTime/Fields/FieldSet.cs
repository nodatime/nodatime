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

        public DateTimeField TickOfMillisecond { get { return tickOfMillisecond; } }
        public DateTimeField TickOfDay { get { return tickOfDay; } }
        public DateTimeField MillisecondOfSecond { get { return millisecondOfSecond; } }
        public DateTimeField MillisecondOfDay { get { return millisecondOfDay; } }
        public DateTimeField SecondOfMinute { get { return secondOfMinute; } }
        public DateTimeField SecondOfDay { get { return secondOfDay; } }
        public DateTimeField MinuteOfHour { get { return minuteOfHour; } }
        public DateTimeField MinuteOfDay { get { return minuteOfDay; } }
        public DateTimeField HourOfDay { get { return hourOfDay; } }
        public DateTimeField ClockHourOfDay { get { return clockHourOfDay; } }
        public DateTimeField HourOfHalfDay { get { return hourOfHalfDay; } }
        public DateTimeField ClockHourOfHalfDay { get { return clockHourOfHalfDay; } }
        public DateTimeField HalfDayOfDay { get { return halfDayOfDay; } }
        public DateTimeField DayOfWeek { get { return dayOfWeek; } }
        public DateTimeField DayOfMonth { get { return dayOfMonth; } }
        public DateTimeField DayOfYear { get { return dayOfYear; } }
        public DateTimeField WeekOfWeekYear { get { return weekOfWeekYear; } }
        public DateTimeField WeekYear { get { return weekYear; } }
        public DateTimeField WeekYearOfCentury { get { return weekYearOfCentury; } }
        public DateTimeField MonthOfYear { get { return monthOfYear; } }
        public DateTimeField Year { get { return year; } }
        public DateTimeField YearOfCentury { get { return yearOfCentury; } }
        public DateTimeField YearOfEra { get { return yearOfEra; } }
        public DateTimeField CenturyOfEra { get { return centruryOfEra; } }
        public DateTimeField Era { get { return era; } }

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

            tickOfMillisecond = builder.TickOfMillisecond ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.TickOfMillisecond, ticks);
            tickOfDay = builder.TickOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.TickOfDay, ticks);
            millisecondOfSecond = builder.MillisecondOfSecond ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MillisecondOfSecond, milliseconds);
            millisecondOfDay = builder.MillisecondOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MillisecondOfDay, milliseconds);
            secondOfMinute = builder.SecondOfMinute ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.SecondOfMinute, seconds);
            secondOfDay = builder.SecondOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.SecondOfDay, seconds);
            minuteOfHour = builder.MinuteOfHour ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MinuteOfHour, minutes);
            minuteOfDay = builder.MinuteOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MinuteOfDay, minutes);
            hourOfDay = builder.HourOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.HourOfDay, hours);
            clockHourOfDay = builder.ClockHourOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.ClockHourOfDay, hours);
            hourOfHalfDay = builder.HourOfHalfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.HourOfHalfDay, hours);
            clockHourOfHalfDay = builder.ClockHourOfHalfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.ClockHourOfHalfDay, hours);
            halfDayOfDay = builder.HalfDayOfDay ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.HalfDayOfDay, halfDays);
            dayOfWeek = builder.DayOfWeek ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.DayOfWeek, days);
            dayOfMonth = builder.DayOfMonth ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.DayOfMonth, days);
            dayOfYear = builder.DayOfYear ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.DayOfYear, days);
            weekOfWeekYear = builder.WeekOfWeekYear ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.WeekOfWeekYear, weeks);
            weekYear = builder.WeekYear ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.WeekYear, years);
            weekYearOfCentury = builder.WeekYearOfCentury ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.WeekYearOfCentury, weekYears);
            monthOfYear = builder.MonthOfYear ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.MonthOfYear, months);
            year = builder.Year ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.Year, years);
            yearOfCentury = builder.YearOfCentury ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.YearOfCentury, years);
            yearOfEra = builder.YearOfEra ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.YearOfEra, years);
            centruryOfEra = builder.CenturyOfEra ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.CenturyOfEra, centuries);
            era = builder.Era ?? UnsupportedDateTimeField.GetInstance(DateTimeFieldType.Era, eras);
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

            public DateTimeField TickOfMillisecond { get; set; }
            public DateTimeField TickOfDay { get; set; }
            public DateTimeField MillisecondOfSecond { get; set; }
            public DateTimeField MillisecondOfDay { get; set; }
            public DateTimeField SecondOfMinute { get; set; }
            public DateTimeField SecondOfDay { get; set; }
            public DateTimeField MinuteOfHour { get; set; }
            public DateTimeField MinuteOfDay { get; set; }
            public DateTimeField HourOfDay { get; set; }
            public DateTimeField ClockHourOfDay { get; set; }
            public DateTimeField HourOfHalfDay { get; set; }
            public DateTimeField ClockHourOfHalfDay { get; set; }
            public DateTimeField HalfDayOfDay { get; set; }
            public DateTimeField DayOfWeek { get; set; }
            public DateTimeField DayOfMonth { get; set; }
            public DateTimeField DayOfYear { get; set; }
            public DateTimeField WeekOfWeekYear { get; set; }
            public DateTimeField WeekYear { get; set; }
            public DateTimeField WeekYearOfCentury { get; set; }
            public DateTimeField MonthOfYear { get; set; }
            public DateTimeField Year { get; set; }
            public DateTimeField YearOfCentury { get; set; }
            public DateTimeField YearOfEra { get; set; }
            public DateTimeField CenturyOfEra { get; set; }
            public DateTimeField Era { get; set; }

            public Builder()
            {
            }

            public Builder(FieldSet baseSet)
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
            public Builder WithSupportedFieldsFrom(FieldSet other)
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

            public FieldSet Build()
            {
                return new FieldSet(this);
            }
        }
    }
}