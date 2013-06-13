// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// An immutable collection of date/time and period fields.
    /// </summary>
    internal sealed class FieldSet
    {
        private readonly IPeriodField ticks;
        private readonly IPeriodField milliseconds;
        private readonly IPeriodField seconds;
        private readonly IPeriodField minutes;
        private readonly IPeriodField hours;
        private readonly IPeriodField halfDays;
        private readonly IPeriodField days;
        private readonly IPeriodField weeks;
        private readonly IPeriodField months;
        private readonly IPeriodField years;

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
        private readonly DateTimeField hourOfHalfDay;
        private readonly DateTimeField clockHourOfHalfDay;
        private readonly DateTimeField dayOfWeek;
        private readonly DateTimeField dayOfMonth;
        private readonly DateTimeField dayOfYear;
        private readonly DateTimeField weekOfWeekYear;
        private readonly DateTimeField weekYear;
        private readonly DateTimeField monthOfYear;
        private readonly DateTimeField year;
        private readonly DateTimeField yearOfCentury;
        private readonly DateTimeField yearOfEra;
        private readonly DateTimeField centruryOfEra;
        private readonly DateTimeField era;

        internal IPeriodField Ticks { get { return ticks; } }
        internal IPeriodField Milliseconds { get { return milliseconds; } }
        internal IPeriodField Seconds { get { return seconds; } }
        internal IPeriodField Minutes { get { return minutes; } }
        internal IPeriodField Hours { get { return hours; } }
        internal IPeriodField HalfDays { get { return halfDays; } }
        internal IPeriodField Days { get { return days; } }
        internal IPeriodField Weeks { get { return weeks; } }
        internal IPeriodField Months { get { return months; } }
        internal IPeriodField Years { get { return years; } }

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
        internal DateTimeField HourOfHalfDay { get { return hourOfHalfDay; } }
        internal DateTimeField ClockHourOfHalfDay { get { return clockHourOfHalfDay; } }
        internal DateTimeField DayOfWeek { get { return dayOfWeek; } }
        internal DateTimeField DayOfMonth { get { return dayOfMonth; } }
        internal DateTimeField DayOfYear { get { return dayOfYear; } }
        internal DateTimeField WeekOfWeekYear { get { return weekOfWeekYear; } }
        internal DateTimeField WeekYear { get { return weekYear; } }
        internal DateTimeField MonthOfYear { get { return monthOfYear; } }
        internal DateTimeField Year { get { return year; } }
        internal DateTimeField YearOfCentury { get { return yearOfCentury; } }
        internal DateTimeField YearOfEra { get { return yearOfEra; } }
        internal DateTimeField CenturyOfEra { get { return centruryOfEra; } }
        internal DateTimeField Era { get { return era; } }

        private FieldSet(Builder builder)
        {
            ticks = builder.Ticks ?? UnsupportedPeriodField.Ticks;
            milliseconds = builder.Milliseconds ?? UnsupportedPeriodField.Milliseconds;
            seconds = builder.Seconds ?? UnsupportedPeriodField.Seconds;
            minutes = builder.Minutes ?? UnsupportedPeriodField.Minutes;
            hours = builder.Hours ?? UnsupportedPeriodField.Hours;
            halfDays = builder.HalfDays ?? UnsupportedPeriodField.HalfDays;
            days = builder.Days ?? UnsupportedPeriodField.Days;
            weeks = builder.Weeks ?? UnsupportedPeriodField.Weeks;
            months = builder.Months ?? UnsupportedPeriodField.Months;
            years = builder.Years ?? UnsupportedPeriodField.Years;

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
            hourOfHalfDay = builder.HourOfHalfDay ?? UnsupportedDateTimeField.HourOfHalfDay;
            clockHourOfHalfDay = builder.ClockHourOfHalfDay ?? UnsupportedDateTimeField.ClockHourOfHalfDay;
            dayOfWeek = builder.DayOfWeek ?? UnsupportedDateTimeField.DayOfWeek;
            dayOfMonth = builder.DayOfMonth ?? UnsupportedDateTimeField.DayOfMonth;
            dayOfYear = builder.DayOfYear ?? UnsupportedDateTimeField.DayOfYear;
            weekOfWeekYear = builder.WeekOfWeekYear ?? UnsupportedDateTimeField.WeekOfWeekYear;
            weekYear = builder.WeekYear ?? UnsupportedDateTimeField.WeekYear;
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
        internal sealed class Builder
        {
            internal IPeriodField Ticks { get; set; }
            internal IPeriodField Milliseconds { get; set; }
            internal IPeriodField Seconds { get; set; }
            internal IPeriodField Minutes { get; set; }
            internal IPeriodField Hours { get; set; }
            internal IPeriodField HalfDays { get; set; }
            internal IPeriodField Days { get; set; }
            internal IPeriodField Weeks { get; set; }
            internal IPeriodField Months { get; set; }
            internal IPeriodField Years { get; set; }

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
            internal DateTimeField HourOfHalfDay { get; set; }
            internal DateTimeField ClockHourOfHalfDay { get; set; }
            internal DateTimeField DayOfWeek { get; set; }
            internal DateTimeField DayOfMonth { get; set; }
            internal DateTimeField DayOfYear { get; set; }
            internal DateTimeField WeekOfWeekYear { get; set; }
            internal DateTimeField WeekYear { get; set; }
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
                Preconditions.CheckNotNull(baseSet, "baseSet");
                Ticks = baseSet.Ticks;
                Milliseconds = baseSet.Milliseconds;
                Seconds = baseSet.Seconds;
                Minutes = baseSet.Minutes;
                Hours = baseSet.Hours;
                HalfDays = baseSet.HalfDays;
                Days = baseSet.Days;
                Weeks = baseSet.Weeks;
                Months = baseSet.Months;
                Years = baseSet.Years;

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
                HourOfHalfDay = baseSet.HourOfHalfDay;
                ClockHourOfHalfDay = baseSet.ClockHourOfHalfDay;
                DayOfWeek = baseSet.DayOfWeek;
                DayOfMonth = baseSet.DayOfMonth;
                DayOfYear = baseSet.DayOfYear;
                WeekOfWeekYear = baseSet.WeekOfWeekYear;
                WeekYear = baseSet.WeekYear;
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
                Preconditions.CheckNotNull(other, "other");
                Ticks = FirstSupported(other.Ticks, Ticks);
                Milliseconds = FirstSupported(other.Milliseconds, Milliseconds);
                Seconds = FirstSupported(other.Seconds, Seconds);
                Minutes = FirstSupported(other.Minutes, Minutes);
                Hours = FirstSupported(other.Hours, Hours);
                HalfDays = FirstSupported(other.HalfDays, HalfDays);
                Days = FirstSupported(other.Days, Days);
                Weeks = FirstSupported(other.Weeks, Weeks);
                Months = FirstSupported(other.Months, Months);
                Years = FirstSupported(other.Years, Years);

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
                HourOfHalfDay = other.HourOfHalfDay.IsSupported ? other.HourOfHalfDay : HourOfHalfDay;
                ClockHourOfHalfDay = other.ClockHourOfHalfDay.IsSupported ? other.ClockHourOfHalfDay : ClockHourOfHalfDay;
                DayOfWeek = other.DayOfWeek.IsSupported ? other.DayOfWeek : DayOfWeek;
                DayOfMonth = other.DayOfMonth.IsSupported ? other.DayOfMonth : DayOfMonth;
                DayOfYear = other.DayOfYear.IsSupported ? other.DayOfYear : DayOfYear;
                WeekOfWeekYear = other.WeekOfWeekYear.IsSupported ? other.WeekOfWeekYear : WeekOfWeekYear;
                WeekYear = other.WeekYear.IsSupported ? other.WeekYear : WeekYear;
                MonthOfYear = other.MonthOfYear.IsSupported ? other.MonthOfYear : MonthOfYear;
                Year = other.Year.IsSupported ? other.Year : Year;
                YearOfCentury = other.YearOfCentury.IsSupported ? other.YearOfCentury : YearOfCentury;
                YearOfEra = other.YearOfEra.IsSupported ? other.YearOfEra : YearOfEra;
                CenturyOfEra = other.CenturyOfEra.IsSupported ? other.CenturyOfEra : CenturyOfEra;
                Era = other.Era.IsSupported ? other.Era : Era;
                return this;
            }

            // TODO: This needs to go away...
            private static IPeriodField FirstSupported(IPeriodField first, IPeriodField second)
            {
                // We treat any non-PeriodField value as supported.
                return (first is PeriodField && !((PeriodField)first).IsSupported) ? second : first;
            }

            internal FieldSet Build()
            {
                return new FieldSet(this);
            }
        }
    }
}