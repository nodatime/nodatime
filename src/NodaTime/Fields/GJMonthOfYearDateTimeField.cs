using NodaTime.Calendars;

namespace NodaTime.Fields
{
    internal class GJMonthOfYearDateTimeField : BasicMonthOfYearDateTimeField
    {
        internal GJMonthOfYearDateTimeField(BasicCalendarSystem calendarSystem)
            : base(calendarSystem, DateTimeConstants.February)
        {
        }

        // There will be more stuff here, honest :)
        // It's all i18n stuff though, which we don't support yet...
    }
}
