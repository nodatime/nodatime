// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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

        #endregion
    }
}