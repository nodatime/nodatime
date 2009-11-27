using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: Needs partial and max for set support.
    /// </summary>
    internal sealed class BasicDayOfMonthDateTimeField : PreciseDurationDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicDayOfMonthDateTimeField(BasicCalendarSystem calendarSystem, DurationField days)
            : base(DateTimeFieldType.DayOfMonth, days)
        {
            this.calendarSystem = calendarSystem;
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfMonth(localInstant);
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfMonth(localInstant);
        }

        public override DurationField RangeDurationField { get { return calendarSystem.Fields.Months; } }

        public override long GetMaximumValue()
        {
            return calendarSystem.GetDaysInMonthMax();
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDaysInMonthMax(localInstant);
        }

        public override long GetMinimumValue()
        {
            return 1;
        }
    }
}
