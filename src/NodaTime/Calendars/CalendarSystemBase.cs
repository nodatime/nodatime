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
using NodaTime.Fields;

namespace NodaTime.Calendars
{
    /// <summary>
    /// Original name: BaseChronology
    /// </summary>
    public abstract class CalendarSystemBase : ICalendarSystem
    {
        private readonly FieldSet fields;

        protected CalendarSystemBase(FieldSet fields)
        {
            this.fields = fields;
        }

        public LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int tickOfDay)
        {
            LocalInstant instant = Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            return Fields.TickOfDay.SetValue(instant, tickOfDay);
        }

        public LocalInstant GetLocalInstant(int year, int monthOfYear, int dayOfMonth, int hourOfDay,
            int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            LocalInstant instant = Fields.Year.SetValue(LocalInstant.LocalUnixEpoch, year);
            instant = Fields.MonthOfYear.SetValue(instant, monthOfYear);
            instant = Fields.DayOfMonth.SetValue(instant, dayOfMonth);
            instant = Fields.HourOfDay.SetValue(instant, hourOfDay);
            instant = Fields.MinuteOfHour.SetValue(instant, minuteOfHour);
            instant = Fields.SecondOfMinute.SetValue(instant, secondOfMinute);
            instant = Fields.MillisecondOfSecond.SetValue(instant, millisecondOfSecond);
            return Fields.TickOfMillisecond.SetValue(instant, tickOfMillisecond);
        }

        public LocalInstant GetLocalInstant(LocalInstant localInstant, 
            int hourOfDay, int minuteOfHour, int secondOfMinute, int millisecondOfSecond, int tickOfMillisecond)
        {
            localInstant = Fields.HourOfDay.SetValue(localInstant, hourOfDay);
            localInstant = Fields.MinuteOfHour.SetValue(localInstant, minuteOfHour);
            localInstant = Fields.SecondOfMinute.SetValue(localInstant, secondOfMinute);
            localInstant = Fields.MillisecondOfSecond.SetValue(localInstant, millisecondOfSecond);
            return Fields.TickOfMillisecond.SetValue(localInstant, tickOfMillisecond);
        }

        public FieldSet Fields { get { return fields; } }

        public void Validate(IPartial partial, int[] values)
        {
            throw new NotImplementedException();
        }

        public int[] GetPartialValues(IPartial partial, LocalInstant instant)
        {
            throw new NotImplementedException();
        }

        public LocalInstant SetPartial(IPartial partial, LocalInstant localInstant)
        {
            throw new NotImplementedException();
        }

        public int[] GetPeriodValues(IPeriod period, LocalInstant start, LocalInstant end)
        {
            throw new NotImplementedException();
        }

        public int[] GetPeriodValues(IPeriod period, Duration duration)
        {
            throw new NotImplementedException();
        }

        public LocalInstant Add(IPeriod period, LocalInstant localInstant, int scalar)
        {
            throw new NotImplementedException();
        }

        public LocalInstant Add(IPeriod period, Duration duration, int scalar)
        {
            throw new NotImplementedException();
        }
    }
}