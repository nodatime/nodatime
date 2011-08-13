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

using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Abstract calendar system that enables calendar systems to be assembled from
    /// a container of fields.
    /// </summary>
    internal abstract class AssembledCalendarSystem : CalendarSystem
    {
        private readonly FieldSet fields;

        private readonly bool useBaseTimeOfDayFields;
        private readonly bool useBaseTickOfDayFields;
        private readonly bool useBaseYearMonthDayFields;

        private readonly CalendarSystem baseCalendar;

        protected AssembledCalendarSystem(string name, CalendarSystem baseCalendar) : base(name)
        {
            this.baseCalendar = baseCalendar;
            fields = ConstructFields();

            if (baseCalendar != null)
            {
                // Work out which fields from the base are still valid, so we can
                // optimize by calling directly to the base calendar sometimes
                FieldSet baseFields = baseCalendar.Fields;
                useBaseTimeOfDayFields = baseFields.HourOfDay == fields.HourOfDay && baseFields.MinuteOfHour == fields.MinuteOfHour &&
                                         baseFields.SecondOfMinute == fields.SecondOfMinute && baseFields.MillisecondOfSecond == fields.MillisecondOfSecond &&
                                         baseFields.TickOfMillisecond == fields.TickOfMillisecond;
                useBaseTickOfDayFields = baseFields.TickOfDay == fields.TickOfDay;
                useBaseYearMonthDayFields = baseFields.Year == fields.Year && baseFields.MonthOfYear == fields.MonthOfYear &&
                                            baseFields.DayOfMonth == fields.DayOfMonth;
            }
            else
            {
                useBaseYearMonthDayFields = false;
                useBaseTimeOfDayFields = false;
                useBaseYearMonthDayFields = false;
            }
        }

        internal override sealed FieldSet Fields { get { return fields; } }

        /// <summary>
        /// The calendar system on top of which this calendar is assembled.
        /// </summary>
        internal CalendarSystem Calendar { get { return baseCalendar; } }

        private FieldSet ConstructFields()
        {
            FieldSet.Builder builder = new FieldSet.Builder();
            if (Calendar != null)
            {
                builder.WithSupportedFieldsFrom(baseCalendar.Fields);
            }
            AssembleFields(builder);
            return builder.Build();
        }

        /// <summary>
        /// I would really like to work out a way of avoiding this at some
        /// point - abstract/virtual calls in the constructor are awful.
        /// However, it's hard to work around this at the moment.
        /// </summary>
        protected abstract void AssembleFields(FieldSet.Builder builder);

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute,
                                                       int millisecondOfSecond, int tickOfMillisecond)
        {
            if (useBaseYearMonthDayFields && useBaseTimeOfDayFields)
            {
                // Only call specialized implementation if applicable fields are the same.
                return baseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond,
                                                    tickOfMillisecond);
            }
            return base.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
        }

        internal override LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, long tickOfDay)
        {
            if (useBaseTickOfDayFields && useBaseYearMonthDayFields)
            {
                // Only call specialized implementation if applicable fields are the same.
                return baseCalendar.GetLocalInstant(year, monthOfYear, dayOfMonth, tickOfDay);
            }
            return base.GetLocalInstant(year, monthOfYear, dayOfMonth, tickOfDay);
        }

        internal override LocalInstant GetLocalInstant(LocalInstant localInstant, int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond,
                                                       int tickOfMillisecond)
        {
            if (useBaseTimeOfDayFields)
            {
                // Only call specialized implementation if applicable fields are the same.
                return baseCalendar.GetLocalInstant(localInstant, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
            }

            return base.GetLocalInstant(localInstant, hourOfDay, minuteOfHour, secondOfMinute, millisecondOfSecond, tickOfMillisecond);
        }
    }
}