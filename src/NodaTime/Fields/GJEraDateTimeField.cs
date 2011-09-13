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
    /// Porting status: needs text
    /// TODO: Rename to "GregulianEraDateTimeField" or something similar?
    /// </summary>
    internal sealed class GJEraDateTimeField : DateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJEraDateTimeField(BasicCalendarSystem calendarSystem) 
            : base(DateTimeFieldType.Era, UnsupportedDurationField.Eras)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override DurationField RangeDurationField { get { return null; } }

        #region Values
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant) <= 0 ? NodaConstants.BeforeCommonEra : NodaConstants.CommonEra;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, NodaConstants.BCE, NodaConstants.CE);

            int oldEra = GetValue(localInstant);
            if (oldEra != value)
            {
                int year = calendarSystem.GetYear(localInstant);
                return calendarSystem.SetYear(localInstant, -year);
            }
            else
            {
                return localInstant;
            }
        }
        #endregion

        #region Ranges
        internal override long GetMaximumValue()
        {
            return NodaConstants.CommonEra;
        }

        internal override long GetMinimumValue()
        {
            return NodaConstants.BeforeCommonEra;
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return GetValue(localInstant) == NodaConstants.CommonEra ? calendarSystem.SetYear(LocalInstant.LocalUnixEpoch, 1) : new LocalInstant(long.MinValue);
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return GetValue(localInstant) == NodaConstants.BeforeCommonEra
                       ? calendarSystem.SetYear(LocalInstant.LocalUnixEpoch, 1)
                       : new LocalInstant(long.MaxValue);
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }
        #endregion
    }
}