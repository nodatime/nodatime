using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: Needs partial and max for set support.
    /// </summary>
    internal sealed class BasicDayOfYearDateTimeField : PreciseDurationDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicDayOfYearDateTimeField(BasicCalendarSystem calendarSystem, DurationField days)
            : base(DateTimeFieldType.DayOfMonth, days)
        {
            this.calendarSystem = calendarSystem;
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfYear(localInstant);
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfYear(localInstant);
        }

        public override DurationField RangeDurationField { get { return calendarSystem.Fields.Days; } }

        public override long GetMaximumValue()
        {
            return calendarSystem.GetDaysInYearMax();
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            int year = calendarSystem.GetYear(localInstant);
            return calendarSystem.GetDaysInYearMax(year);
        }

        public override long GetMinimumValue()
        {
            return 1;
        }
    }
}
