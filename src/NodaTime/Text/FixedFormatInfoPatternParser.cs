// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// A pattern parser for a single format info, which caches patterns by text/style.
    /// </summary>
    internal sealed class FixedFormatInfoPatternParser<T>
    {
        // It would be unusual to have more than 50 different patterns for a specific culture
        // within a real app. 
        private const int CacheSize = 50;
        private readonly Cache<string, IPattern<T>> cache;

        internal FixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
        {                
            cache = new Cache<string, IPattern<T>>(CacheSize, patternText => patternParser.ParsePattern(patternText, formatInfo),
                StringComparer.Ordinal);
        }

        internal IPattern<T> ParsePattern(string pattern) => cache.GetOrAdd(pattern);
    }
}
