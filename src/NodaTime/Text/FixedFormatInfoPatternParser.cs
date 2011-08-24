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
    internal sealed class FixedFormatInfoPatternParser<T>
    {
        private const int StyleCombinations = 16;

        private readonly IPatternParser<T> patternParser;
        // TODO: Replace this with a real LRU cache or something similar.
        private readonly Dictionary<string, PatternParseResult<T>>[] caches;
        private readonly NodaFormatInfo formatInfo;

        internal FixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
        {
            this.patternParser = patternParser;
            // There aren't many valid style combinations, so we partition the caches by style.
            caches = new Dictionary<string, PatternParseResult<T>>[StyleCombinations];
            this.formatInfo = formatInfo;
        }

        internal PatternParseResult<T> ParsePattern(string pattern, ParseStyles styles)
        {
            // TODO: This currently only caches valid patterns. Is that reasonable?
            // TODO: Validate styles here or elsewhere?

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
