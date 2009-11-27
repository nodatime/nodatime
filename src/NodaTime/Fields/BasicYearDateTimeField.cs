using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: need AddWrapField
    /// </summary>
    internal sealed class BasicYearDateTimeField : ImpreciseDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.Year, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }

        public override bool IsLenient { get { return false; } }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetYear(localInstant);
        }
        
        public override LocalInstant Add(LocalInstant localInstant, int value)
        {
            if (value == 0)
            {
                return localInstant;
            }
            int thisYear = GetValue(localInstant);
            return SetValue(localInstant, thisYear + value);
        }

        public override LocalInstant Add(LocalInstant localInstant, long value)
        {
            return Add(localInstant, (int)value);
        }

        public override LocalInstant SetValue(LocalInstant localInstant, long value)
        {
            FieldUtils.VerifyValueBounds(this, value, calendarSystem.MinYear, calendarSystem.MaxYear);
            return calendarSystem.SetYear(localInstant, (int) value);
        }

        public override long GetInt64Difference(LocalInstant minuendInstant, LocalInstant subtrahendInstant)
        {
            return minuendInstant < subtrahendInstant ? -calendarSystem.GetYearDifference(subtrahendInstant, minuendInstant)
                : calendarSystem.GetYearDifference(minuendInstant, subtrahendInstant);
        }

        public override DurationField RangeDurationField { get { return null; } }

        public override bool IsLeap(LocalInstant localInstant)
        {
            return calendarSystem.IsLeapYear(GetValue(localInstant));
        }

        public override int GetLeapAmount(LocalInstant localInstant)
        {
            return IsLeap(localInstant) ? 1 : 0;
        }

        public override DurationField LeapDurationField { get { return calendarSystem.Fields.Days; } }

        public override long GetMinimumValue()
        {
            return calendarSystem.MinYear;
        }

        public override long GetMaximumValue()
        {
            return calendarSystem.MaxYear;
        }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return new LocalInstant(calendarSystem.GetYearTicks(GetValue(localInstant)));
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            int year = GetValue(localInstant);
            long yearStartTicks = calendarSystem.GetYearTicks(year);
            return localInstant.Ticks == yearStartTicks ? localInstant
                : new LocalInstant(calendarSystem.GetYearTicks(year + 1));
        }

        // Removed override of Remainder as base implementation is fine
    }
}
