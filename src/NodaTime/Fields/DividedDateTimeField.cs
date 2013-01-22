// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Divides a DateTimeField such that the retrieved values are reduced by a
    /// fixed divisor. The field's unit duration is scaled accordingly, but the
    /// range duration is unchanged.
    /// </summary>
    internal sealed class DividedDateTimeField : DecoratedDateTimeField
    {
        private readonly int divisor;
        private readonly long min;
        private readonly long max;

        internal DividedDateTimeField(DateTimeField field, DateTimeFieldType fieldType, int divisor)
            : base(field, fieldType, new ScaledPeriodField(field.PeriodField, fieldType.PeriodFieldType, divisor))
        {
            if (divisor < 2)
            {
                throw new ArgumentOutOfRangeException("divisor", "The divisor must be at least 2");
            }

            this.divisor = divisor;

            long fieldMin = field.GetMinimumValue();
            min = fieldMin >= 0 ? fieldMin / divisor : ((fieldMin + 1) / divisor - 1);

            long fieldMax = field.GetMinimumValue();
            max = fieldMax >= 0 ? fieldMax / divisor : ((fieldMax + 1) / divisor - 1);
        }

        internal int Divisor { get { return divisor; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value / divisor : ((value + 1) / divisor) - 1;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            long value = WrappedField.GetValue(localInstant);
            return value >= 0 ? value / divisor : ((value + 1) / divisor) - 1;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, min, max);
            long wrappedValue = WrappedField.GetInt64Value(localInstant);
            long remainder = wrappedValue >= 0 ? wrappedValue % divisor : (divisor - 1) + ((wrappedValue + 1) % divisor);
            return WrappedField.SetValue(localInstant, value * divisor + remainder);
        }

        internal override long GetMinimumValue()
        {
            return min;
        }

        internal override long GetMaximumValue()
        {
            return max;
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            DateTimeField field = WrappedField;
            return field.RoundFloor(field.SetValue(localInstant, GetInt64Value(localInstant) * divisor));
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            // TODO(Post-V1): Check this - it looks very odd to me.
            Duration wrappedRemainder = WrappedField.Remainder(localInstant);
            return new Duration(SetValue(localInstant, GetValue(new LocalInstant(wrappedRemainder.Ticks))).Ticks);
        }
    }
}