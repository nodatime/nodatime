// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

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

        internal OffsetDateTimeField(DateTimeField field, int offset)
            // If the field is null, we want to let the 
            // base constructor throw the exception, rather than
            // fail to dereference it properly here.
            : this(Preconditions.CheckNotNull(field, "field"), field.FieldType, offset, int.MinValue, int.MaxValue)
        {
        }

        internal OffsetDateTimeField(DateTimeField field, DateTimeFieldType fieldType, int offset) : this(field, fieldType, offset, int.MinValue, int.MaxValue)
        {
        }

        private OffsetDateTimeField(DateTimeField field, DateTimeFieldType fieldType, int offset, int minValue, int maxValue)
            : base(field, fieldType)
        {
            if (offset == 0)
            {
                throw new ArgumentOutOfRangeException("offset", "The offset cannot be zero");
            }

            this.offset = offset;
            // This field is only really used for weeks etc - not ticks -
            // so casting the min and max to int should be fine.
            min = Math.Max(minValue, (int)field.GetMinimumValue() + offset);
            max = Math.Min(maxValue, (int)field.GetMaximumValue() + offset);
        }

        // Note: no need to override GetValue, as that delegates to GetInt64Value.

        #region Values
        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return base.GetInt64Value(localInstant) + offset;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, min, max);
            return base.SetValue(localInstant, value - offset);
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

        #endregion

        #region Ranges
        internal override long GetMinimumValue()
        {
            return min;
        }

        internal override long GetMaximumValue()
        {
            return max;
        }
        #endregion

        #region Rounding
        // No need to override RoundFloor again - it already just delegates.

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfFloor(localInstant);
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundHalfCeiling(localInstant);
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