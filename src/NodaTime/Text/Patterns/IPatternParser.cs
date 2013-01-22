using NodaTime.Globalization;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Internal interface used by FixedFormatInfoPatternParser. Unfortunately
    /// even though this is internal, implementations must either use public methods
    /// or explicit interface implementation.
    /// </summary>
    internal interface IPatternParser<T>
    {
        IPattern<T> ParsePattern(string pattern, NodaFormatInfo formatInfo);
    }
}
