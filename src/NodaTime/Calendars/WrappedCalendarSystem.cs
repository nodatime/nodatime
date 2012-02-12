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
using System.Collections.Generic;
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// A calendar system which wraps another one, allowing the wrapped system to be used for some operations.
    /// </summary>
    // TODO(Post-V1): Decide whether or not this is actually useful. It may only be used by the Buddhist calendar system...
    internal abstract class WrappedCalendarSystem : CalendarSystem
    {
        private readonly CalendarSystem baseCalendar;
        private readonly bool useBaseTimeOfDayFields;
        private readonly bool useBaseTickOfDayFields;
        private readonly bool useBaseYearMonthDayFields;

        /// <summary>
        /// The calendar system wrapped by this one.
        /// </summary>
        internal CalendarSystem Calendar { get { return baseCalendar; } }

        public override int MaxYear { get { return baseCalendar.MaxYear; } }
        public override int MinYear { get { return baseCalendar.MinYear; } }

        protected WrappedCalendarSystem(string name, CalendarSystem baseCalendar, FieldAssembler assembler, IEnumerable<Era> eras)
            : base(name, (builder, @this) => AssembleFields(builder, @this, baseCalendar, assembler), eras)
        {
            // Quick sanity check - the point of separating out this class is to only use it in
            // situations where we really have a calendar to wrap.
            if (baseCalendar == null)
            {
                throw new ArgumentNullException("baseCalendar");
            }
            this.baseCalendar = baseCalendar;
            // Work out which fields from the base are still valid, so we can
            // optimize by calling directly to the base calendar sometimes.
            FieldSet baseFields = baseCalendar.Fields;
            useBaseTimeOfDayFields = baseFields.HourOfDay == Fields.HourOfDay && 
                                        baseFields.MinuteOfHour == Fields.MinuteOfHour &&
                                        baseFields.SecondOfMinute == Fields.SecondOfMinute && 
                                        baseFields.MillisecondOfSecond == Fields.MillisecondOfSecond &&
                                        baseFields.TickOfMillisecond == Fields.TickOfMillisecond &&
                                        baseFields.TickOfSecond == Fields.TickOfSecond;
            useBaseTickOfDayFields = baseFields.TickOfDay == Fields.TickOfDay;
            useBaseYearMonthDayFields = baseFields.Year == Fields.Year &&
                                        baseFields.MonthOfYear == Fields.MonthOfYear &&
                                        baseFields.DayOfMonth == Fields.DayOfMonth;
        }

        private static void AssembleFields(FieldSet.Builder builder, CalendarSystem @this, CalendarSystem baseCalendar, FieldAssembler assembler)
        {
            builder.WithSupportedFieldsFrom(baseCalendar.Fields);
            assembler(builder, @this);
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

        // Delegate era handling to wrapped calendar.
        internal override int GetMinYearOfEra(int eraIndex)
        {
            return baseCalendar.GetMinYearOfEra(eraIndex);
        }

        internal override int GetMaxYearOfEra(int eraIndex)
        {
            return baseCalendar.GetMaxYearOfEra(eraIndex);
        }

        internal override int GetAbsoluteYear(int yearOfEra, int eraIndex)
        {
             return baseCalendar.GetAbsoluteYear(yearOfEra, eraIndex);
        }

        public override int GetMaxMonth(int year)
        {
            return baseCalendar.GetMaxMonth(year);
        }
    }
}
