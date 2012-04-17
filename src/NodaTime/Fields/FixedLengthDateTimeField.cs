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

using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// "Fixed length" date/time field, composed of two fixed length period fields.
    /// </summary>
    /// <remarks>
    /// This DateTimeField is useful for defining fields that are composed
    /// of fixed length periods, like time of day fields. If the length of either period field
    /// varies, then a VariableLengthDateTimeField may be used instead.
    /// </remarks>
    internal sealed class FixedLengthDateTimeField : FixedLengthPeriodDateTimeField
    {
        private readonly PeriodField rangeField;
        private readonly long effectiveRange;

        internal FixedLengthDateTimeField(DateTimeFieldType type, PeriodField unit, PeriodField rangeField) : base(type, unit)
        {
            Preconditions.CheckNotNull(rangeField, "rangeField");
            Preconditions.CheckArgument(rangeField.IsFixedLength, "rangeField", "Range period field must have a fixed length");
            effectiveRange = rangeField.UnitTicks / unit.UnitTicks;
            Preconditions.CheckArgument(effectiveRange >= 2, "rangeField", "The effective range must be at least 2");
            this.rangeField = rangeField;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            long ticks = localInstant.Ticks;
            return ticks >= 0 ? (ticks / UnitTicks) % effectiveRange : effectiveRange - 1 + (((ticks + 1) / UnitTicks) % effectiveRange);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, GetMinimumValue(), GetMaximumValue());
            long ticks = localInstant.Ticks;
            return new LocalInstant(ticks + (value - GetInt64Value(localInstant)) * UnitTicks);
        }

        internal override PeriodField RangePeriodField { get { return rangeField; } }

        internal override long GetMaximumValue()
        {
            return effectiveRange - 1;
        }
    }
}