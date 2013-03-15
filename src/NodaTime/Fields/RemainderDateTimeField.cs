// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    internal sealed class RemainderDateTimeField : DecoratedDateTimeField
    {
        private readonly int divisor;
        private readonly PeriodField remainderRangeField;

        internal RemainderDateTimeField(DateTimeField field, DateTimeFieldType fieldType, int divisor) : base(field, fieldType)
        {
            if (divisor < 2)
            {
                throw new ArgumentOutOfRangeException("divisor", "The divisor must be at least 2");
            }

            remainderRangeField = new ScaledPeriodField(field.PeriodField, fieldType.RangePeriodFieldType.Value, divisor);
            this.divisor = divisor;
        }

        internal RemainderDateTimeField(DividedDateTimeField dividedField) : this(dividedField, dividedField.FieldType)
        {
        }

        internal RemainderDateTimeField(DividedDateTimeField dividedField, DateTimeFieldType fieldType) : base(dividedField.WrappedField, fieldType)
        {
            divisor = dividedField.Divisor;
            remainderRangeField = dividedField.PeriodField;
        }

        internal PeriodField RemainderRangeField { get { return remainderRangeField; } }

        internal int Divisor { get { return divisor; } }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value % divisor : (divisor - 1) + ((value + 1) % divisor);
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, 0, divisor - 1);
            int wrappedValue = WrappedField.GetValue(localInstant);
            int divided = wrappedValue >= 0 ? wrappedValue / divisor : ((wrappedValue + 1) / divisor) - 1;
            return WrappedField.SetValue(localInstant, divided * divisor + value);
        }

        internal override long GetMinimumValue()
        {
            return 0;
        }

        internal override long GetMaximumValue()
        {
            return divisor - 1;
        }

        // No need to override RoundFloor - it already delegates
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
    }
}