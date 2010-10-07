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
    /// Wraps another field such that zero values are replaced with one more than
    /// it's maximum. This is particularly useful for implementing an clockhourOfDay
    /// field, where the midnight value of 0 is replaced with 24.
    /// </summary>
    internal sealed class ZeroIsMaxDateTimeField : DecoratedDateTimeField
    {
        internal ZeroIsMaxDateTimeField(DateTimeFieldBase wrappedField, DateTimeFieldType fieldType) : base(wrappedField, fieldType)
        {
            if (wrappedField.GetMinimumValue() != 0)
            {
                throw new ArgumentException("Wrapped field's minumum value must be zero");
            }
        }

        #region Values
        public override int GetValue(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value == 0 ? (int)GetMaximumValue() : value;
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            long value = WrappedField.GetInt64Value(localInstant);
            return value == 0 ? GetMaximumValue() : value;
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            long max = GetMaximumValue();
            FieldUtils.VerifyValueBounds(this, value, 1, max);
            return WrappedField.SetValue(localInstant, value == max ? 0 : value);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant);
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
            return 1;
        }

        public override long GetMinimumValue(LocalInstant localInstant)
        {
            return 1;
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            return WrappedField.GetMaximumValue(localInstant) + 1;
        }

        public override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue() + 1;
        }
        #endregion

        #region Rounding
        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundFloor(localInstant);
        }

        public override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfCeiling(localInstant);
        }

        public override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfFloor(localInstant);
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