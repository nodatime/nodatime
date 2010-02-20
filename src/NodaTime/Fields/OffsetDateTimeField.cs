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

namespace NodaTime.Fields
{
    /// <summary>
    /// Generic offset adjusting datetime field.
    /// </summary>
    internal sealed class OffsetDateTimeField : DecoratedDateTimeField
    {
        private readonly int offset;
        private readonly int min;
        private readonly int max;

        internal OffsetDateTimeField(IDateTimeField field, int offset)
            // If the field is null, we want to let the 
            // base constructor throw the exception, rather than
            // fail to dereference it properly here.
            : this(field, field == null ? null : field.FieldType,
                offset, int.MinValue, int.MaxValue)
        {
        }

        internal OffsetDateTimeField(IDateTimeField field, 
            DateTimeFieldType fieldType, int offset)
            : this(field, fieldType, offset, int.MinValue, int.MaxValue)
        {
        }

        public OffsetDateTimeField(IDateTimeField field, 
            DateTimeFieldType fieldType, int offset, int minValue, int maxValue)
            : base(field, fieldType)
        {
            if (offset == 0)
            {
                throw new ArgumentOutOfRangeException("offset", "The offset cannot be zero");
            }

            this.offset = offset;
            // This field is only really used for weeks etc - not ticks -
            // so casting the min and max to int should be fine.
            this.min = Math.Max(minValue, (int) field.GetMinimumValue() + offset);
            this.max = Math.Min(maxValue, (int) field.GetMaximumValue() + offset);
        }

        // Note: no need to override GetValue, as that delegates to GetInt64Value.

        #region Values

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return base.GetInt64Value(localInstant) + offset;
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            localInstant = base.Add(localInstant, value);
            FieldUtils.VerifyValueBounds(this, GetInt64Value(localInstant), min, max);
            return localInstant;
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            localInstant = base.Add(localInstant, value);
            FieldUtils.VerifyValueBounds(this, GetInt64Value(localInstant), min, max);
            return localInstant;
        }

        public override LocalInstant AddWrapField(LocalInstant localInstant, int value)
        {
            return SetValue(localInstant, FieldUtils.GetWrappedValue(GetValue(localInstant), value, min, max));

        }
        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, min, max);
            return base.SetValue(localInstant, value - offset);
        }

        #endregion

        #region Leap

        public override bool IsLeap(LocalInstant localInstant)
        {
            return WrappedField.IsLeap(localInstant);
        }

        public override int GetLeapAmount(LocalInstant localInstant)
        {
            return WrappedField.GetLeapAmount(localInstant);
        }

        public override DurationField LeapDurationField { get { return WrappedField.LeapDurationField; } }

        #endregion

        #region Ranges

        public override long GetMinimumValue()
        {
            return min;
        }

        public override long GetMaximumValue()
        {
            return max;
        }

        #endregion

        #region Rounding

        // No need to override RoundFloor again - it already just delegates.

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
        #endregion

    }
}
