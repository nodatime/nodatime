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
    /// A year field suitable for many calendars.
    /// </summary>
    internal sealed class BasicYearDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;
        private readonly DurationField durationField;

        internal BasicYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.Year, new BasicYearDurationField(calendarSystem))
        {
            this.calendarSystem = calendarSystem;
            durationField = new BasicYearDurationField(calendarSystem);
        }

        /// <summary>
        /// Always returns null (not supported)
        /// </summary>
        internal override DurationField RangeDurationField { get { return null; } }

        #region Values
        /// <summary>
        /// Get the Year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }

        /// <summary>
        /// Get the Year component of the specified local instant.
        /// </summary>
        /// <param name="localInstant">The local instant to query</param>
        /// <returns>The year extracted from the input.</returns>
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }

        internal override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            if (value == 0)
            {
                return localInstant;
            }

            int thisYear = calendarSystem.GetYear(localInstant);
            int wrappedYear = FieldUtils.GetWrappedValue(thisYear, value, calendarSystem.MinYear, calendarSystem.MaxYear);

            return SetValue(localInstant, wrappedYear);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, calendarSystem.MinYear, calendarSystem.MaxYear);
            return calendarSystem.SetYear(localInstant, (int)value);
        }
        #endregion

        #region Leap
        internal override bool IsLeap(LocalInstant localInstant)
        {
            return calendarSystem.IsLeapYear(GetValue(localInstant));
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return IsLeap(localInstant) ? 1 : 0;
        }

        internal override DurationField LeapDurationField { get { return calendarSystem.Fields.Days; } }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return calendarSystem.MinYear;
        }

        internal override long GetMaximumValue()
        {
            return calendarSystem.MaxYear;
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return new LocalInstant(calendarSystem.GetYearTicks(GetValue(localInstant)));
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            int year = GetValue(localInstant);
            long yearStartTicks = calendarSystem.GetYearTicks(year);
            return localInstant.Ticks == yearStartTicks ? localInstant : new LocalInstant(calendarSystem.GetYearTicks(year + 1));
        }
        #endregion
    }
}