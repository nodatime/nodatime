using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NodaTime.Format
{
    public abstract class ParserBase<T> : IParser<T>
    {
        public T Parse(string text)
        {
            return Parse(text, CultureInfo.InvariantCulture);
        }

        public T Parse(string text, IFormatProvider formatProvider)
        {
            ParseResult<T> result = TryParse(text, formatProvider);
            if (result.Success)
            {
                return result.Value;
            }
            throw result.Exception;
        }

        public bool TryParse(string text, out T value)
        {
            return TryParse(text, CultureInfo.CurrentCulture, out value);
        }

        public bool TryParse(string text, IFormatProvider formatProvider, out T value)
        {
            ParseResult<T> result = TryParse(text, formatProvider);
            value = result.Success ? result.Value : default(T);
            return result.Success;
        }

        public ParseResult<T> TryParse(string text)
        {
            return TryParse(text, CultureInfo.InvariantCulture);
        }

        public abstract ParseResult<T> TryParse(string text, IFormatProvider formatProvider);
    }
}
