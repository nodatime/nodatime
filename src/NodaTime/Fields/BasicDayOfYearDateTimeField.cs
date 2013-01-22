using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// Provides time calculations for the day of the year component of time.
    /// </summary>
    // Porting status: Needs partial and max for set support.
    internal sealed class BasicDayOfYearDateTimeField : FixedLengthPeriodDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicDayOfYearDateTimeField(BasicCalendarSystem calendarSystem, PeriodField days) : base(DateTimeFieldType.DayOfYear, days)
        {
            this.calendarSystem = calendarSystem;
        }

        internal override int GetValue(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfYear(localInstant);
        }

        internal override long GetInt64Value(LocalInstant localInstant)
        {
            return calendarSystem.GetDayOfYear(localInstant);
        }

        internal override PeriodField RangePeriodField { get { return calendarSystem.Fields.Years; } }

        internal override long GetMaximumValue()
        {
            return calendarSystem.GetDaysInYearMax();
        }

        internal override long GetMaximumValue(LocalInstant localInstant)
        {
            int year = calendarSystem.GetYear(localInstant);
            return calendarSystem.GetDaysInYear(year);
        }

        internal override long GetMinimumValue()
        {
            return 1;
        }
    }
}