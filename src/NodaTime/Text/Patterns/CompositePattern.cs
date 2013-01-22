using System.Collections.Generic;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Composite pattern which parses by trying several parse patterns in turn, and formats
    /// by calling a delegate (which may have come from another <see cref="IPattern{T}"/> to start with).
    /// </summary>
    internal sealed class CompositePattern<T> : IPattern<T>
    {
        private List<IPattern<T>> parsePatterns;
        private NodaFunc<T, string> formatter;

        internal CompositePattern(IEnumerable<IPattern<T>> parsePatterns, IPattern<T> formatPattern)
        {
            this.parsePatterns = new List<IPattern<T>>(parsePatterns);
            this.formatter = formatPattern.Format;
        }

        internal CompositePattern(IEnumerable<IPattern<T>> parsePatterns, NodaFunc<T, string> formatter)
        {
            this.parsePatterns = new List<IPattern<T>>(parsePatterns);
            this.formatter = formatter;
        }

        public ParseResult<T> Parse(string text)
        {
            foreach (IPattern<T> pattern in parsePatterns)
            {
                ParseResult<T> result = pattern.Parse(text);
                if (result.Success || !result.ContinueAfterErrorWithMultipleFormats)
                {
                    return result;
                }
            }
            return ParseResult<T>.NoMatchingFormat;
        }

        public string Format(T value)
        {
            return formatter(value);
        }
    }

}
