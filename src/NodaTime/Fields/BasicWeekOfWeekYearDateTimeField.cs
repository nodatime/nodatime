using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Needs partial and max for set support.
    /// </summary>
    internal sealed class BasicWeekOfWeekYearDateTimeField : PreciseDurationDateTimeField
    {
        private static readonly Duration ThreeDays = Duration.StandardDays(3);
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicWeekOfWeekYearDateTimeField(BasicCalendarSystem calendarSystem, DurationField weeks)
            : base(DateTimeFieldType.WeekOfWeekYear, weeks)
        {
            this.calendarSystem = calendarSystem;
        }

        public override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        public override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetWeekOfWeekYear(localInstant);
        }

        public override DurationField RangeDurationField { get { return calendarSystem.Fields.WeekYears; } }

        public override LocalInstant RoundFloor(LocalInstant localInstant)
        {
            return base.RoundFloor(localInstant + ThreeDays) - ThreeDays;
        }

        public override LocalInstant RoundCeiling(LocalInstant localInstant)
        {
            return base.RoundCeiling(localInstant + ThreeDays) - ThreeDays;
        }

        public override Duration Remainder(LocalInstant localInstant)
        {
            return base.Remainder(localInstant + ThreeDays);
        }

        public override long GetMinimumValue()
        {
            return 1;
        }

        public override long GetMaximumValue()
        {
            return 53;
        }

        public override long GetMaximumValue(LocalInstant localInstant)
        {
            int weekyear = calendarSystem.GetWeekYear(localInstant);
            return calendarSystem.GetWeeksInYear(weekyear);
        }
    }
}
