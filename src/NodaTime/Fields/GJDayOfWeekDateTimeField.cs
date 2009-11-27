using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Porting status: need text.
    /// </summary>
    internal sealed class GJDayOfWeekDateTimeField : PreciseDurationDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJDayOfWeekDateTimeField(BasicCalendarSystem calendarSystem, DurationField days)
            : base(DateTimeFieldType.DayOfWeek, days)
        {
            this.calendarSystem = calendarSystem;
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfWeek(localInstant);
        }

        public override DurationField RangeDurationField { get { return calendarSystem.Fields.Weeks; } }

        public override long GetMaximumValue()
        {
            return NodaConstants.Sunday;
        }

        public override long GetMinimumValue()
        {
            return NodaConstants.Monday;
        }
    }
}
