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

using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// Precise datetime field, composed of two precise duration fields.
    /// </summary>
    /// <remarks>
    /// This DateTimeField is useful for defining fields that are composed
    /// of precise durations, like time of day fields. If either duration field is
    /// imprecise, then an ImpreciseDateTimeField may be used instead.
    /// </remarks>
    internal sealed class PreciseDateTimeField : PreciseDurationDateTimeField
    {
        private readonly DurationField rangeField;
        private readonly long effectiveRange;

        internal PreciseDateTimeField(DateTimeFieldType type, DurationField unit, DurationField rangeField) : base(type, unit)
        {
            if (rangeField == null)
            {
                throw new ArgumentNullException("rangeField");
            }
            if (!rangeField.IsPrecise)
            {
                throw new ArgumentException("Range duration field must be precise");
            }
            effectiveRange = rangeField.UnitTicks / unit.UnitTicks;
            if (effectiveRange < 2)
            {
                throw new ArgumentException("The effective range must be at least 2.");
            }
            this.rangeField = rangeField;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return ticks >= 0 ? (ticks / UnitTicks) % effectiveRange : effectiveRange - 1 + (((ticks + 1) / UnitTicks) % effectiveRange);
        }

        // TODO: addWrapField

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, GetMinimumValue(), GetMaximumValue());
            long ticks = localInstant.Ticks;
            return new LocalInstant(ticks + (value - GetInt64Value(localInstant)) * UnitTicks);
        }

        internal override DurationField RangeDurationField { get { return rangeField; } }

        internal override long GetMaximumValue()
        {
            return effectiveRange - 1;
        }
    }
}