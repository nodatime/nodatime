using NodaTime.Calendars;
using NodaTime.Utility;

namespace NodaTime.Fields
{
    internal sealed class GJYearOfEraDateTimeField : DecoratedDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJYearOfEraDateTimeField(DateTimeField yearField, BasicCalendarSystem calendarSystem) : base(yearField, DateTimeFieldType.YearOfEra)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            int year = WrappedField.GetValue(localInstant);
            return year <= 0 ? 1 - year : year;
        }

        internal override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            Preconditions.CheckArgumentRange("value", value, 1, GetMaximumValue());
            if (calendarSystem.GetYear(localInstant) <= 0)
            {
                value = 1 - value;
            }
            return base.SetValue(localInstant, value);
        }

        internal override long GetMinimumValue()
        {
            return 1;
        }

        internal override long GetMaximumValue()
        {
            return WrappedField.GetMaximumValue();
        }

        internal override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return WrappedField.RoundFloor(localInstant);
        }

        internal override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return WrappedField.RoundCeiling(localInstant);
        }

        internal override Duration Remainder(LocalInstant localInstant)
        {
            return WrappedField.Remainder(localInstant);
        }
    }
}