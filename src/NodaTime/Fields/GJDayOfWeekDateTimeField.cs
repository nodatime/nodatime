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

using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: need text.
    /// </summary>
    internal sealed class GJDayOfWeekDateTimeField : FixedLengthPeriodDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJDayOfWeekDateTimeField(BasicCalendarSystem calendarSystem, PeriodField days) : base(DateTimeFieldType.DayOfWeek, days)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        internal override PeriodField RangePeriodField { get { return calendarSystem.Fields.Weeks; } }

        internal override long GetMaximumValue()
        {
            return (long)IsoDayOfWeek.Sunday;
        }

        internal override long GetMinimumValue()
        {
            return (long)IsoDayOfWeek.Monday;
        }
    }
}