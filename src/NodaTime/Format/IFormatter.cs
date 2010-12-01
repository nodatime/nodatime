using System;

namespace NodaTime.Format
{
    /// <summary>
    /// Generic formatting interface, implemented separately for specific types.
    /// TODO: We *could* include the format provider information within the formatter itself (and ditto for the parser)
    /// - if you want a formatter with a given IFormatProvider, you call WithFormatProvider(...). Not sure.
    /// </summary>
    public interface IFormatter<T>
    {
        /// <summary>
        /// Formats the given value with the current thread's culture info.
        /// TODO: Definitely use this rather than CultureInfo.InvariantThread? Hmm.
        /// TODO: Consider making this an extension method on the interface, given that it will always
        /// just call Format(value, CurrentThread.CurrentCulture)?
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <returns>The formatted string</returns>
        string Format(T value);

        /// <summary>
        /// Formats the given value using the specified format provider for cultural purposes, such as
        /// month names.
        /// </summary>
        /// <param name="value">Value to format</param>
        /// <param name="formatProvider">Format provider for culture-specific formatting</param>
        /// <returns>The formatted string</returns>
        string Format(T value, IFormatProvider formatProvider);
    }
}
