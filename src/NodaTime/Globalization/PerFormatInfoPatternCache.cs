#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Globalization
{
    /// <summary>
    /// Cache of patterns for a single NodaFormatInfo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class PerFormatInfoPatternCache<T> : AbstractNodaParser<T>
    {
        private const int StyleCombinations = 16;

        private const string DefaultFormattingPattern = "g";
        private readonly IPatternParser<T> patternParser;
        // TODO: Replace this with a real LRU cache or something similar.
        private readonly Dictionary<string, PatternParseResult<T>>[] caches;
        private readonly NodaFormatInfo formatInfo;

        internal PerFormatInfoPatternCache(IPatternParser<T> patternParser, string[] allFormats, T failureValue, NodaFormatInfo formatInfo)
            : base(allFormats, failureValue)
        {
            this.patternParser = patternParser;
            // There aren't many valid style combinations, so we partition the caches by style.
            caches = new Dictionary<string, PatternParseResult<T>>[StyleCombinations];
            this.formatInfo = formatInfo;
        }

        protected override ParseResult<T> ParseSingle(string value, string pattern, NodaFormatInfo formatInfo, ParseStyles styles)
        {
            // Assume the right formatInfo... suggests the design smells a little.

            if (pattern == null)
            {
                return ParseResult<T>.ArgumentNull("format");
            }
            // TODO: Validate styles before we try to parse.
            PatternParseResult<T> patternParseResult = GetCachedPattern(pattern, styles);
            if (!patternParseResult.Success)
            {
                return patternParseResult.ToParseResult();
            }
            return patternParseResult.GetResultOrThrow().Parse(value);
        }

        public override string Format(T value, string pattern, NodaFormatInfo formatInfo)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                pattern = DefaultFormattingPattern;
            }
            PatternParseResult<T> patternParseResult = GetCachedPattern(pattern, ParseStyles.None);
            IParsedPattern<T> parsedPattern = patternParseResult.GetResultOrThrow();
            return parsedPattern.Format(value);
        }

        private PatternParseResult<T> GetCachedPattern(string pattern, ParseStyles styles)
        {
            // TODO: This currently only caches valid patterns. Is that reasonable?

            // This should be in range.
            var cache = caches[(int) styles];

            if (cache == null)
            {
                // It's possible that another thread won't notice the addition of this value. That's okay - we may
                // do more parsing than absolutely necessary. The important thing is that the dictionary will be usable
                // by the end of its constructor, and all of that is published by the constructor. We actually *use*
                // the caches in a thread-safe way.
                cache = new Dictionary<string, PatternParseResult<T>>();
                caches[(int)styles] = cache;
            }

            lock (cache)
            {
                PatternParseResult<T> cached;
                if (cache.TryGetValue(pattern, out cached))
                {
                    return cached;
                }
            }

            // Unlock, create the parser and then update the cache if necessary. We don't want to lock
            // for longer than we need to.
            var result = patternParser.ParsePattern(pattern, formatInfo, styles);
            if (result.Success)
            {
                lock (cache)
                {
                    cache[pattern] = result;
                }
            }
            return result;
        }
    }
}
