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
    /// A placeholder implementation to use when a datetime field is not supported.
    /// All methods throw <see cref="NotSupportedException"/>, and the DurationField is always
    /// the unsupported field of the appropriate duration type.
    /// </summary>
    internal class UnsupportedDateTimeField : DateTimeField
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

        private UnsupportedDateTimeField(DateTimeFieldType fieldType) : base(fieldType, UnsupportedDurationField.ForFieldType(fieldType.DurationFieldType), false, false)
        {
        }

        internal override DurationField RangeDurationField { get { return null; } }

        internal override DurationField LeapDurationField { get { return null; } }

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

        internal override bool IsLeap(LocalInstant localInstant)
        {
            throw new NotSupportedException();
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
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