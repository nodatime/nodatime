// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using System;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Class providing simple support for the various Parse/TryParse/ParseExact/TryParseExact/Format overloads 
    /// provided by individual types.
    /// </summary>
    internal sealed class PatternBclSupport<T>
    {
        private readonly Func<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser;
        private readonly string defaultFormatPattern;

        internal PatternBclSupport(string defaultFormatPattern, Func<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser)
        {
            this.patternParser = patternParser;
            this.defaultFormatPattern = defaultFormatPattern;
        }

        internal string Format(T value, string? patternText, IFormatProvider? formatProvider)
        {
            if (string.IsNullOrEmpty(patternText))
            {
                patternText = defaultFormatPattern;
            }
            NodaFormatInfo formatInfo = NodaFormatInfo.GetInstance(formatProvider);
            // Note: string.IsNullOrEmpty isn't annotated in netstandard2.0, hence the use of the
            // null-forgiving operator here. *We* know patternText isn't null.
            IPattern<T> pattern = patternParser(formatInfo).ParsePattern(patternText!);
            return pattern.Format(value);
        }
    }
}
