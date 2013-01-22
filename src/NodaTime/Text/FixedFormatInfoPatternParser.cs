using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// A pattern parser for a single format info, which caches patterns by text/style.
    /// </summary>
    // TODO(Post-V1): Consider making this an interface. It was a sealed class before we needed a non-caching implementation.
    internal abstract class FixedFormatInfoPatternParser<T>
    {
        private readonly IPatternParser<T> patternParser;
        private readonly NodaFormatInfo formatInfo;

        internal abstract IPattern<T> ParsePattern(string pattern);

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
            // TODO(Post-V1): Replace this with a real LRU cache or something similar.
            private readonly Dictionary<string, IPattern<T>> cache;

            internal CachingFixedFormatInfoPatternParser(IPatternParser<T> patternParser, NodaFormatInfo formatInfo)
                : base(patternParser, formatInfo)
            {                
                cache = new Dictionary<string, IPattern<T>>();
            }

            internal override IPattern<T> ParsePattern(string pattern)
            {
                // I don't normally like locking on anything other than object, but I trust
                // Dictionary not to lock on itself.
                lock (cache)
                {
                    IPattern<T> cached;
                    if (cache.TryGetValue(pattern, out cached))
                    {
                        return cached;
                    }
                }

                // Unlock, create the parser and then update the cache if necessary. We don't want to lock
                // for longer than we need to.
                var result = patternParser.ParsePattern(pattern, formatInfo);
                lock (cache)
                {
                    cache[pattern] = result;
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

            internal override IPattern<T>  ParsePattern(string pattern)
            {
                return patternParser.ParsePattern(pattern, formatInfo);
            }
        }
    }
}
