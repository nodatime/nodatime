namespace NodaTime.Format
{
    /// <summary>
    /// A parsed format pattern, which can be reused across multiple formatters and parsers
    /// where that makes sense. For example, the same pattern can be used for a LocalDateTime and
    /// a ZonedDateTime with a default time zone.
    /// </summary>
    /// <remarks>
    /// The parsing will simply break the pattern into sections. Each section could be:
    /// - a literal (including whitespace and explicitly quoted text)
    /// - a field (a character and the number of repetitions, e.g. DD ends up as {'D', 2}
    /// - a symbol which may or may not be treated as a literal by the parser/formatter, depending
    /// on the settings (so ":" is a time separator field in a LocalTimeParser, but a literal in a LocalDateParser)
    /// 
    /// I expect there to be another (internal) level of abstraction here, so that a FormatPattern can be converted
    /// into a parser/formatter-type-specific sequence of objects which can be used one after another.
    /// </remarks>
    public class FormatPattern
    {
    }
}
