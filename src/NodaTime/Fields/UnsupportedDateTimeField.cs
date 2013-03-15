// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// A placeholder implementation to use when a datetime field is not supported.
    /// All methods throw <see cref="NotSupportedException"/>, and the PeriodField is always
    /// the unsupported field of the appropriate duration type.
    /// </summary>
    internal sealed class UnsupportedDateTimeField : DateTimeField
    {
        // Convenience fields
        public static readonly UnsupportedDateTimeField Era = new UnsupportedDateTimeField(DateTimeFieldType.Era);
        public static readonly UnsupportedDateTimeField YearOfEra = new UnsupportedDateTimeField(DateTimeFieldType.YearOfEra);
        public static readonly UnsupportedDateTimeField CenturyOfEra = new UnsupportedDateTimeField(DateTimeFieldType.CenturyOfEra);
        public static readonly UnsupportedDateTimeField YearOfCentury = new UnsupportedDateTimeField(DateTimeFieldType.YearOfCentury);
        public static readonly UnsupportedDateTimeField Year = new UnsupportedDateTimeField(DateTimeFieldType.Year);
        public static readonly UnsupportedDateTimeField DayOfYear = new UnsupportedDateTimeField(DateTimeFieldType.DayOfYear);
        public static readonly UnsupportedDateTimeField MonthOfYear = new UnsupportedDateTimeField(DateTimeFieldType.MonthOfYear);
        public static readonly UnsupportedDateTimeField DayOfMonth = new UnsupportedDateTimeField(DateTimeFieldType.DayOfMonth);
        public static readonly UnsupportedDateTimeField WeekYearOfCentury = new UnsupportedDateTimeField(DateTimeFieldType.WeekYearOfCentury);
        public static readonly UnsupportedDateTimeField WeekYear = new UnsupportedDateTimeField(DateTimeFieldType.WeekYear);
        public static readonly UnsupportedDateTimeField WeekOfWeekYear = new UnsupportedDateTimeField(DateTimeFieldType.WeekOfWeekYear);
        public static readonly UnsupportedDateTimeField DayOfWeek = new UnsupportedDateTimeField(DateTimeFieldType.DayOfWeek);
        public static readonly UnsupportedDateTimeField HalfDayOfDay = new UnsupportedDateTimeField(DateTimeFieldType.HalfDayOfDay);
        public static readonly UnsupportedDateTimeField HourOfHalfDay = new UnsupportedDateTimeField(DateTimeFieldType.HourOfHalfDay);
        public static readonly UnsupportedDateTimeField ClockHourOfHalfDay = new UnsupportedDateTimeField(DateTimeFieldType.ClockHourOfHalfDay);
        public static readonly UnsupportedDateTimeField ClockHourOfDay = new UnsupportedDateTimeField(DateTimeFieldType.ClockHourOfDay);
        public static readonly UnsupportedDateTimeField HourOfDay = new UnsupportedDateTimeField(DateTimeFieldType.HourOfDay);
        public static readonly UnsupportedDateTimeField MinuteOfDay = new UnsupportedDateTimeField(DateTimeFieldType.MinuteOfDay);
        public static readonly UnsupportedDateTimeField MinuteOfHour = new UnsupportedDateTimeField(DateTimeFieldType.MinuteOfHour);
        public static readonly UnsupportedDateTimeField SecondOfMinute = new UnsupportedDateTimeField(DateTimeFieldType.SecondOfMinute);
        public static readonly UnsupportedDateTimeField SecondOfDay = new UnsupportedDateTimeField(DateTimeFieldType.SecondOfDay);
        public static readonly UnsupportedDateTimeField MillisecondOfSecond = new UnsupportedDateTimeField(DateTimeFieldType.MillisecondOfSecond);
        public static readonly UnsupportedDateTimeField MillisecondOfDay = new UnsupportedDateTimeField(DateTimeFieldType.MillisecondOfDay);
        public static readonly UnsupportedDateTimeField TickOfMillisecond = new UnsupportedDateTimeField(DateTimeFieldType.TickOfMillisecond);
        public static readonly UnsupportedDateTimeField TickOfDay = new UnsupportedDateTimeField(DateTimeFieldType.TickOfDay);
        public static readonly UnsupportedDateTimeField TickOfSecond = new UnsupportedDateTimeField(DateTimeFieldType.TickOfSecond);

        private UnsupportedDateTimeField(DateTimeFieldType fieldType) : base(fieldType, UnsupportedPeriodField.ForFieldType(fieldType.PeriodFieldType), false)
        {
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            throw new NotSupportedException();
        }

        internal override long GetMaximumValue()
        {
            throw new NotSupportedException();
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override long GetMinimumValue()
        {
            throw new NotSupportedException();
        }

        internal override long GetMinimumValue(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }
    }
}