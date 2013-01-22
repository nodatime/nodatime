using System;
using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides the era component of any calendar with only a single era.
    /// This always returns a value of 0, as it's always the sole entry in the list of eras.
    /// </summary>
    internal sealed class BasicSingleEraDateTimeField : DateTimeField
    {
        private readonly Era era;

        internal BasicSingleEraDateTimeField(Era era)
            : base(DateTimeFieldType.Era, UnsupportedPeriodField.Eras)
        {
            this.era = era;
        }

        internal override string Name { get { return era.Name; } }

        internal override int GetValue(LocalInstant localInstant)
        {
            return 0;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return 0;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, 0, 0);
            return localInstant;
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MaxValue;
        }

        internal override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        internal override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            return LocalInstant.MinValue;
        }

        // TODO(Post-V1): Joda returns a null. Could return an unsupported field?
        internal override PeriodField RangePeriodField
        {
            get { throw new NotSupportedException(); }
        }

        internal override long GetMinimumValue()
        {
            return 0;
        }

        internal override long GetMaximumValue()
        {
            return 0;
        }
    }
}
