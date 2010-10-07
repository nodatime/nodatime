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
    /// Porting status: need AddWrapField. Also consider making min/max ints - check usage.
    /// </summary>
    internal class DividedDateTimeField : DecoratedDateTimeField
    {
        private readonly int divisor;
        private readonly DurationField divisorDurationField;
        private readonly long min;
        private readonly long max;

        internal DividedDateTimeField(IDateTimeField field, DateTimeFieldType fieldType, int divisor) : base(field, fieldType)
        {
            if (divisor < 2)
            {
                throw new ArgumentOutOfRangeException("divisor", "The divisor must be at least 2");
            }

            DurationField unitField = field.DurationField;
            divisorDurationField = unitField == null ? null : new ScaledDurationField(unitField, fieldType.DurationFieldType, divisor);
            this.divisor = divisor;

            long fieldMin = field.GetMinimumValue();
            min = fieldMin >= 0 ? fieldMin / divisor : ((fieldMin + 1) / divisor - 1);

            long fieldMax = field.GetMinimumValue();
            max = fieldMax >= 0 ? fieldMax / divisor : ((fieldMax + 1) / divisor - 1);
        }

        internal DividedDateTimeField(RemainderDateTimeField remainderField, DateTimeFieldType fieldType) : base(remainderField.WrappedField, fieldType)
        {
            divisor = remainderField.Divisor;
            divisorDurationField = remainderField.RemainderRangeField;
            IDateTimeField field = WrappedField;

            long fieldMin = field.GetMinimumValue();
            min = fieldMin >= 0 ? fieldMin / divisor : ((fieldMin + 1) / divisor - 1);

            long fieldMax = field.GetMinimumValue();
            max = fieldMax >= 0 ? fieldMax / divisor : ((fieldMax + 1) / divisor - 1);
        }

        internal int Divisor { get { return divisor; } }
        internal DurationField DivisorDurationField { get { return divisorDurationField; } }

        public override int GetValue(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value / divisor : ((value + 1) / divisor) - 1;
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            long value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value / divisor : ((value + 1) / divisor) - 1;
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return WrappedField.Add(localInstant, value * divisor);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return WrappedField.Add(localInstant, value * divisor);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant) / divisor;
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant) / divisor;
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, min, max);
            long wrappedValue = WrappedField.GetInt64Value(localInstant);
            long remainder = wrappedValue >= 0 ? wrappedValue % divisor : (divisor - 1) + ((wrappedValue + 1) % divisor);
            return WrappedField.SetValue(localInstant, value * divisor + remainder);
        }

        public override DurationField DurationField { get { return divisorDurationField; } }

        public override long GetMinimumValue()
        {
            return min;
        }

        public override long GetMaximumValue()
        {
            return max;
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            IDateTimeField field = WrappedField;
            return field.RoundFloor(field.SetValue(localInstant, GetInt64Value(localInstant) * divisor));
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            // TODO: Check this - it looks very odd to me.
            Duration wrappedRemainder = WrappedField.Remainder(localInstant);
            return new Duration(SetValue(localInstant, GetValue(new LocalInstant(wrappedRemainder.Ticks))).Ticks);
        }
    }
}