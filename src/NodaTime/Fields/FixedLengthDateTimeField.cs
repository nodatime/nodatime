// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

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