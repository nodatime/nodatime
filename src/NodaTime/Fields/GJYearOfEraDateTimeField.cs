using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Calendars;

namespace NodaTime.Fields
{
    internal sealed class GJYearOfEraDateTimeField : DecoratedDateTimeField
    {
        private readonly BasicCalendarSystem calendarSystem;

        internal GJYearOfEraDateTimeField(IDateTimeField yearField, BasicCalendarSystem calendarSystem)
            : base (yearField, DateTimeFieldType.YearOfEra)
        {
            this.calendarSystem = calendarSystem;
        }
    }
}
