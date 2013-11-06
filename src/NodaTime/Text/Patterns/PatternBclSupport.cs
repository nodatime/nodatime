// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
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

        internal string Format(T value, string patternText, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(patternText))
            {
                patternText = defaultFormatPattern;
            }
            NodaFormatInfo formatInfo = NodaFormatInfo.GetInstance(formatProvider);
            IPattern<T> pattern = patternParser(formatInfo).ParsePattern(patternText);
            return pattern.Format(value);
        }
    }
}
