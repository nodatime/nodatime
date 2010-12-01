using System;

namespace NodaTime.Format
{
    public class ZonedDateTimeParser : ParserBase<ZonedDateTime>
    {
        public override ParseResult<ZonedDateTime> TryParse(string text, IFormatProvider formatProvider)
        {
            throw new NotImplementedException();
        }

        public ZonedDateTimeParser WithDefaultTimeZone(DateTimeZone timeZone)
        {
            throw new NotImplementedException();
        }

        public ZonedDateTimeParser WithCalendar(CalendarSystem calendar)
        {
            throw new NotImplementedException();
        }
    }
}
