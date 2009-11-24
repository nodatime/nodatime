using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    /// <summary>
    /// TODO: Implement, and consider moving to Calendars namespace.
    /// (To match the chrono namespace in Joda.)
    /// </summary>
    internal class BasicYearDateTimeField : ImpreciseDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal BasicYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(DateTimeFieldType.Year, calendarSystem.AverageTicksPerYear)
        {
            this.calendarSystem = calendarSystem;
        }
    }
}
