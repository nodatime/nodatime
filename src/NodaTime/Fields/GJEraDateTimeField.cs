using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: needs text
    /// TODO: Rename to "GregulianEraDateTimeField" or something similar?
    /// </summary>
    internal sealed class GJEraDateTimeField : DateTimeFieldBase
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJEraDateTimeField(BasicCalendarSystem calendarSystem) 
            : base(DateTimeFieldType.Era)
        {
            this.calendarSystem = calendarSystem;
        }

        public override bool IsLenient { get { return false; } }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant) <= 0 ? NodaConstants.BeforeCommonEra : NodaConstants.CommonEra;
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, NodaConstants.BCE, NodaConstants.CE);

            int oldEra = GetValue(localInstant);
            if (oldEra != value)
            {
                int year = calendarSystem.GetYear(localInstant);
                return calendarSystem.SetYear(localInstant, -year);
            }
            else
            {
                return localInstant;
            }
        }

        public override DurationField DurationField { get { return UnsupportedDurationField.Eras; } }

        public override DurationField RangeDurationField { get { return null; } }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return GetValue(localInstant) == NodaConstants.CommonEra ? calendarSystem.SetYear(LocalInstant.LocalUnixEpoch, 1)
                : new LocalInstant(long.MinValue);
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return GetValue(localInstant) == NodaConstants.BeforeCommonEra ? calendarSystem.SetYear(LocalInstant.LocalUnixEpoch, 1)
                : new LocalInstant(long.MaxValue);
        }

        public override long GetMaximumValue()
        {
            return NodaConstants.CommonEra;
        }

        public override long GetMinimumValue()
        {
            return NodaConstants.BeforeCommonEra;
        }

        public override LocalInstant RoundHalfFloor(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }

        public override LocalInstant RoundHalfCeiling(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }

        public override LocalInstant RoundHalfEven(LocalInstant localInstant)
        {
            // In reality, the era is infinite, so there is no halfway point.
            return RoundFloor(localInstant);
        }
    }
}
