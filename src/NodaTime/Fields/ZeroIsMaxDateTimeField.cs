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

using System;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Wraps another field such that zero values are replaced with one more than
    /// it's maximum. This is particularly useful for implementing an clockhourOfDay
    /// field, where the midnight value of 0 is replaced with 24.
    /// </summary>
    internal sealed class ZeroIsMaxDateTimeField : DecoratedDateTimeField
    {
        internal ZeroIsMaxDateTimeField(DateTimeField wrappedField, DateTimeFieldType fieldType) : base(wrappedField, fieldType)
        {
            Preconditions.CheckArgument(wrappedField.GetMinimumValue() == 0, "wrappedField", "Wrapped field's minumum value must be zero");
        }

        #region Values
        internal override int GetValue(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value == 0 ? (int)GetMaximumValue() : value;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            long value = WrappedField.GetInt64Value(localInstant);
            return value == 0 ? GetMaximumValue() : value;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            long max = GetMaximumValue();
            Preconditions.CheckArgumentRange("value", value, 1, max);
            return WrappedField.SetValue(localInstant, value == max ? 0 : value);
        }
        #endregion

        #region Leap
        internal override bool IsLeap(LocalInstant localInstant)
        {
            return WrappedField.IsLeap(localInstant);
        }

        internal override int GetLeapAmount(LocalInstant localInstant)
        {
            return WrappedField.GetLeapAmount(localInstant);
        }

        internal override PeriodField LeapPeriodField { get { return WrappedField.LeapPeriodField; } }
        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return 1;
        }

        internal override long GetMinimumValue(LocalInstant localInstant)
        {
            return 1;
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            return WrappedField.GetMaximumValue(localInstant) + 1;
        }

        internal override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue() + 1;
        }
        #endregion

        #region Rounding
        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundFloor(localInstant);
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfCeiling(localInstant);
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfFloor(localInstant);
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfEven(localInstant);
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
        #endregion
    }
}