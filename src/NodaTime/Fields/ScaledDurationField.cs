using System;

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Decide whether this wouldn't be better as a DelegatedDurationField...
    /// </summary>
    internal sealed class ScaledDurationField : DecoratedDurationField
    {
        private readonly int scale;

        public ScaledDurationField(DurationField wrappedField, DurationFieldType fieldType, int scale)
            : base(wrappedField, fieldType)
        {
            if (scale == 0 || scale == 1) {
                throw new ArgumentOutOfRangeException("scale", "The scale must not be 0 or 1");
            }
            this.scale = scale;
        }

        public override int GetValue(Duration duration)
        {
            return WrappedField.GetValue(duration) / scale;
        }

        public override int GetValue(Duration duration, LocalInstant localInstant)
        {
            return WrappedField.GetValue(duration, localInstant) / scale;
        }

        public override long GetInt64Value(Duration duration)
        {
            return WrappedField.GetInt64Value(duration) / scale;
        }

        public override long GetInt64Value(Duration duration, LocalInstant localInstant)
        {
            return WrappedField.GetInt64Value(duration, localInstant) / scale;
        }

        public override Duration GetDuration(long value)
        {
            return WrappedField.GetDuration(value * scale);
        }

        public override Duration GetDuration(long value, LocalInstant localInstant)
        {
            return WrappedField.GetDuration(value * scale, localInstant);
        }

        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            return WrappedField.Add(localInstant, value * scale);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return WrappedField.Add(localInstant, value * scale);
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant) / scale;
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant) / scale;
        }

        public override long UnitTicks { get { return WrappedField.UnitTicks * scale; } }
    }
}
