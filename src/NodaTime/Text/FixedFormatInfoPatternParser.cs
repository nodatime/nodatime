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

using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// A pattern parser for a single format info, which caches patterns by text/style.
    /// </summary>
    // TODO: Consider making this an interface. It was a sealed class before we needed a non-caching implementation.
    internal abstract class FixedFormatInfoPatternParser<T>
    {
        private readonly IPatternParser<T> patternParser;
        private readonly NodaFormatInfo formatInfo;

        internal abstract PatternParseResult<T> ParsePattern(string pattern);

        private FixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
        {
            this.patternParser = patternParser;
            this.formatInfo = formatInfo;
        }

        internal static FixedFormatInfoPatternParser<T> CreateCachingParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
        {
            return new CachingFixedFormatInfoPatternParser(patternParser, formatInfo);
        }

        internal static FixedFormatInfoPatternParser<T> CreateNonCachingParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
        {
            return new NonCachingFixedFormatInfoPatternParser(patternParser, formatInfo);
        }

        private sealed class CachingFixedFormatInfoPatternParser: FixedFormatInfoPatternParser<T>
        {
            // TODO: Replace this with a real LRU cache or something similar.
            private readonly Dictionary<string, PatternParseResult<T>> cache;

            internal CachingFixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
                : base(patternParser, formatInfo)
            {                
                cache = new Dictionary<string, PatternParseResult<T>>();
            }

            internal override PatternParseResult<T> ParsePattern(string pattern)
            {
                // TODO: This currently only caches valid patterns. Is that reasonable?

                // I don't normally like locking on anything other than object, but I trust
                // Dictionary not to lock on itself.
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
                var result = patternParser.ParsePattern(pattern, formatInfo);
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

        private sealed class NonCachingFixedFormatInfoPatternParser : FixedFormatInfoPatternParser<T>
        {
            internal NonCachingFixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
                : base(patternParser, formatInfo)
            {                
            }

            internal override PatternParseResult<T>  ParsePattern(string pattern)
            {
                return patternParser.ParsePattern(pattern, formatInfo);
            }
        }
    }
}
