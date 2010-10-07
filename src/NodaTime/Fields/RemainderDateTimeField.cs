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
    /// Status: Need AddWrapField
    /// </summary>
    internal sealed class RemainderDateTimeField : DecoratedDateTimeField
    {
        private readonly int divisor;
        private readonly DurationField remainderRangeField;

        internal RemainderDateTimeField(IDateTimeField field, DateTimeFieldType fieldType, int divisor) : base(field, fieldType)
        {
            if (divisor < 2)
            {
                throw new ArgumentOutOfRangeException("divisor", "The divisor must be at least 2");
            }

            DurationField durationField = field.DurationField;
            remainderRangeField = durationField == null ? null : new ScaledDurationField(durationField, fieldType.RangeDurationFieldType.Value, divisor);
            this.divisor = divisor;
        }

        internal RemainderDateTimeField(DividedDateTimeField dividedField) : this(dividedField, dividedField.FieldType)
        {
        }

        internal RemainderDateTimeField(DividedDateTimeField dividedField, DateTimeFieldType fieldType) : base(dividedField.WrappedField, fieldType)
        {
            divisor = dividedField.Divisor;
            remainderRangeField = dividedField.DivisorDurationField;
        }

        internal DurationField RemainderRangeField { get { return remainderRangeField; } }

        internal int Divisor { get { return divisor; } }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value % divisor : (divisor - 1) + ((value + 1) % divisor);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, 0, divisor - 1);
            int wrappedValue = WrappedField.GetValue(localInstant);
            int divided = wrappedValue >= 0 ? wrappedValue / divisor : ((wrappedValue + 1) / divisor) - 1;
            return WrappedField.SetValue(localInstant, divided * divisor + value);
        }

        public override DurationField RangeDurationField { get { return remainderRangeField; } }

        public override long GetMinimumValue()
        {
            return 0;
        }

        public override long GetMaximumValue()
        {
            return divisor - 1;
        }

        // No need to override RoundFloor - it already delegates
        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfFloor(localInstant);
        }

        public override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfCeiling(localInstant);
        }

        public override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfEven(localInstant);
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
    }
}