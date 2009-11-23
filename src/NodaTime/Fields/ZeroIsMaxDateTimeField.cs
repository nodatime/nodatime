using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Fields
{
    /// <summary>
    /// Wraps another field such that zero values are replaced with one more than
    /// it's maximum. This is particularly useful for implementing an clockhourOfDay
    /// field, where the midnight value of 0 is replaced with 24.
    /// </summary>
    internal sealed class ZeroIsMaxDateTimeField : DecoratedDateTimeField
    {
        internal ZeroIsMaxDateTimeField(IDateTimeField wrappedField,
            DateTimeFieldType fieldType)
            : base(wrappedField, fieldType)
        {
            if (wrappedField.GetMinimumValue() != 0)
            {
                throw new ArgumentException("Wrapped field's minumum value must be zero");
            }
        }

        public override int GetValue(LocalInstant localInstant)
        {
            int value = WrappedField.GetValue(localInstant);
            return value == 0 ? (int) GetMaximumValue() : value;
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

        public override bool IsLeap(LocalInstant localInstant)
        {
            return WrappedField.IsLeap(localInstant);
        }

        public override int GetLeapAmount(LocalInstant localInstant)
        {
            return WrappedField.GetLeapAmount(localInstant);
        }

        public override DurationField LeapDurationField { get { return WrappedField.LeapDurationField; } }

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
    }
}
