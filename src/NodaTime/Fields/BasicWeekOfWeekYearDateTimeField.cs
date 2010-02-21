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
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Needs partial and max for set support.
    /// </summary>
    internal sealed class BasicWeekOfWeekYearDateTimeField : PreciseDurationDateTimeField
    {
        private static readonly Duration ThreeDays = Duration.StandardDays(3);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekOfWeekYearDateTimeField(BasicCalendarSystem calendarSystem, DurationField weeks)
            : base(DateTimeFieldType.WeekOfWeekYear, weeks)
        {
            this.calendarSystem = calendarSystem;
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        public override DurationField RangeDurationField { get { return calendarSystem.Fields.WeekYears; } }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return base.RoundFloor(localInstant + ThreeDays) - ThreeDays;
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return base.RoundCeiling(localInstant + ThreeDays) - ThreeDays;
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return base.Remainder(localInstant + ThreeDays);
        }

        public override long GetMinimumValue()
        {
            return 1;
        }

        public override long GetMaximumValue()
        {
            return 53;
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            int weekyear = calendarSystem.GetWeekYear(localInstant);
            return calendarSystem.GetWeeksInYear(weekyear);
        }
    }
}
