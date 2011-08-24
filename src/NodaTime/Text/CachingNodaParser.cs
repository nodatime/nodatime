using System;
using System.Collections.Generic;
using System.Text;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    internal class CachingNodaParser<T> : AbstractNodaParser<T>
    {
        // TODO: Consider having a cache per NodaFormatInfo instead...
        // TODO: Have a nice way of validating this
        private const int StyleCombinations = 16;

        private const string DefaultFormattingPattern = "g";
        private readonly IPatternParser<T> patternParser;
        // TODO: Replace this with a real LRU cache or something similar.
        private readonly Dictionary<CacheKey, PatternParseResult<T>>[] caches;

        internal CachingNodaParser(IPatternParser<T> patternParser, string[] allFormats, T failureValue) : base(allFormats, failureValue)
        {
            this.patternParser = patternParser;
            // There aren't many valid style combinations, so we partition the caches (and locks) by style.
            caches = new Dictionary<CacheKey, PatternParseResult<T>>[StyleCombinations];
            for (int i = 0; i < StyleCombinations; i++)
            {
                caches[i] = new Dictionary<CacheKey, PatternParseResult<T>>();
            }
        }

        protected override ParseResult<T> ParseSingle(string value, string pattern, NodaFormatInfo formatInfo, ParseStyles styles)
        {
            if (pattern == null)
            {
                return ParseResult<T>.ArgumentNull("format");
            }
            // TODO: Validate styles before we try to parse.
            PatternParseResult<T> patternParseResult = GetCachedPattern(pattern, formatInfo, styles);
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
            PatternParseResult<T> patternParseResult = GetCachedPattern(pattern, formatInfo, ParseStyles.None);
            IParsedPattern<T> parsedPattern = patternParseResult.GetResultOrThrow();
            return parsedPattern.Format(value);
        }

        private PatternParseResult<T> GetCachedPattern(string pattern, NodaFormatInfo formatInfo, ParseStyles styles)
        {
            // TODO: This currently only caches valid patterns. Is that reasonable?

            // This should be in range.
            var cache = caches[(int) styles];
            CacheKey newKey = new CacheKey(pattern, formatInfo);
            // I don't normally like locking on a non-private reference, but I trust Dictionary<,> not to lock,
            // and it means we don't have another array access for no reason.
            lock (cache)
            {
                PatternParseResult<T> cached;
                if (cache.TryGetValue(newKey, out cached))
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
                    cache[newKey] = result;
                }
            }
            return result;
        }

        private struct CacheKey : IEquatable<CacheKey>
        {
            private readonly string pattern;
            private readonly NodaFormatInfo formatInfo;

            internal CacheKey(string pattern, NodaFormatInfo formatInfo)
            {
                this.pattern = pattern;
                this.formatInfo = formatInfo;
            }
            
            public bool Equals(CacheKey other)
            {
                return other.pattern == pattern &&
                    other.formatInfo == formatInfo;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey && Equals((CacheKey)obj);
            }

            public override int GetHashCode()
            {
                return pattern.GetHashCode() ^ formatInfo.GetHashCode();
                /*
                int hash = HashCodeHelper.Initialize();
                hash = HashCodeHelper.Hash(hash, pattern);
                hash = HashCodeHelper.Hash(hash, formatInfo);
                hash = HashCodeHelper.Hash(hash, parseStyles);
                return hash;*/
            }
        }
    }
}
