using System;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Still need to do AddWrapField.
    /// </summary>
    internal class IsoYearOfEraDateTimeField : DecoratedDateTimeField
    {
        internal static readonly IDateTimeField Instance = new IsoYearOfEraDateTimeField();

        private IsoYearOfEraDateTimeField()
            : base(GregorianCalendarSystem.Default.Fields.Year, DateTimeFieldType.YearOfEra)
        {
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return Math.Abs(WrappedField.GetValue(localInstant));
        }

        public override int GetDifference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetDifference(minuendInstant, subtrahendInstant);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return WrappedField.GetInt64Difference(minuendInstant, subtrahendInstant);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, 0, GetMaximumValue());
            if (WrappedField.GetValue(localInstant) < 0)
            {
                value = -value;
            }
            return base.SetValue(localInstant, value);
        }

        public override long GetMinimumValue()
        {
            return 0;
        }

        public override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue();
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
    }
}
