using System;

namespace NodaTime.Format
{
    /// <summary>
    /// Formatter for ZonedDateTime.
    /// </summary>
    internal class ZonedDateTimeFormatter : FormatterBase<ZonedDateTime>
    {
        public ZonedDateTimeFormatter(FormatPattern pattern)
        {
            this.Pattern = pattern;
        }

        public FormatPattern Pattern { get; private set; }

        public ZonedDateTimeParser ToParser()
        {
            throw new NotImplementedException();
        }

        protected override string FormatValue(ZonedDateTime value, IFormatProvider formatProvider)
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
