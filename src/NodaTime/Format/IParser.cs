using System;

namespace NodaTime.Format
{
    /// <summary>
    /// Generic parsing interface.
    /// TODO: Just have the final method? The rest can *all* be extension methods...
    /// If not, we probably want an AbstractParser[T] which delegates everything.
    /// </summary>
    /// <typeparam name="T">Type of value to parse</typeparam>
    public interface IParser<T>
    {
        /// <summary>
        /// Parses the text according to the 
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Parsed value</returns>
        /// <exception cref="ParseException">the text could not be parsed</exception>
        /// <exception cref="ArgumentNullException"><paramref name="text"/> was null</exception>
        T Parse(string text);
        T Parse(string text, IFormatProvider formatProvider);

        // For those who like the standard .NET pattern...
        bool TryParse(string text, out T value);
        bool TryParse(string text, IFormatProvider formatProvider, out T value);

        ParseResult<T> TryParse(string text);
        ParseResult<T> TryParse(string text, IFormatProvider formatProvider);
    }
}
