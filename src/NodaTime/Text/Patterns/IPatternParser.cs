using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Globalization;

namespace NodaTime.Text.Patterns
{
    internal interface IPatternParser<T>
    {
        PatternParseResult<T> ParsePattern(string pattern, NodaFormatInfo formatInfo, ParseStyles parseStyles);
    }
}
