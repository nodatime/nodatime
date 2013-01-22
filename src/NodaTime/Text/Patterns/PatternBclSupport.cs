using NodaTime.Globalization;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Class providing simple support for the various Parse/TryParse/ParseExact/TryParseExact/Format overloads 
    /// provided by individual types.
    /// </summary>
    internal sealed class PatternBclSupport<T>
    {
        private readonly NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser;
        private readonly string defaultFormatPattern;

        internal PatternBclSupport(string defaultFormatPattern, NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser)
        {
            this.patternParser = patternParser;
            this.defaultFormatPattern = defaultFormatPattern;
        }

        internal string Format(T value, string patternText, NodaFormatInfo formatInfo)
        {
            if (string.IsNullOrEmpty(patternText))
            {
                patternText = defaultFormatPattern;
            }
            IPattern<T> pattern = patternParser(formatInfo).ParsePattern(patternText);
            return pattern.Format(value);
        }
    }
}
