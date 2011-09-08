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

using NodaTime.Globalization;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Class providing simple support for the various Parse/TryParse/ParseExact/TryParseExact/Format overloads 
    /// provided by individual types.
    /// </summary>
    internal sealed class PatternBclSupport<T>
    {
        private readonly string[] allFormats;
        private readonly T failureValue;
        private readonly NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser;
        private readonly string defaultFormatPattern;

        internal PatternBclSupport(string[] allFormats, string defaultFormatPattern, T failureValue,
            NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser)
        {
            this.allFormats = allFormats;
            this.failureValue = failureValue;
            this.patternParser = patternParser;
            this.defaultFormatPattern = defaultFormatPattern;
        }

        internal T Parse(string value, NodaFormatInfo formatInfo)
        {
            return ParseExact(value, allFormats, formatInfo);
        }

        internal T ParseExact(string value, string format, NodaFormatInfo formatInfo)
        {
            return ParseSingle(value, format, formatInfo).GetValueOrThrow();
        }

        internal T ParseExact(string value, string[] formats, NodaFormatInfo formatInfo)
        {
            return ParseMultiple(value, formats, formatInfo).GetValueOrThrow();
        }

        internal bool TryParse(string value, NodaFormatInfo formatInfo, out T result)
        {
            return TryParseExact(value, allFormats, formatInfo, out result);
        }

        internal bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, out T result)
        {
            return ParseSingle(value, format, formatInfo).TryGetValue(failureValue, out result);
        }

        internal bool TryParseExact(string value, string[] formats, NodaFormatInfo formatInfo, out T result)
        {
            return ParseMultiple(value, formats, formatInfo).TryGetValue(failureValue, out result);
        }

        internal ParseResult<T> ParseMultiple(string value, string[] formats, NodaFormatInfo formatInfo)
        {
            if (formats == null)
            {
                return ParseResult<T>.ArgumentNull("formats");
            }
            if (formats.Length == 0)
            {
                return PatternParseResult<T>.EmptyFormatsArray.ToParseResult();
            }

            foreach (string format in formats)
            {
                ParseResult<T> result = ParseSingle(value, format, formatInfo);
                if (result.Success || !result.ContinueAfterErrorWithMultipleFormats)
                {
                    return result;
                }
            }
            return ParseResult<T>.NoMatchingFormat;
        }

        internal ParseResult<T> ParseSingle(string value, string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return ParseResult<T>.ArgumentNull("patternText");
            }
            PatternParseResult<T> patternResult = patternParser(formatInfo).ParsePattern(patternText);
            if (!patternResult.Success)
            {
                return patternResult.ToParseResult();
            }
            IPattern<T> pattern = patternResult.GetResultOrThrow();
            return pattern.Parse(value);
        }

        internal string Format(T value, string patternText, NodaFormatInfo formatInfo)
        {
            if (string.IsNullOrEmpty(patternText))
            {
                patternText = defaultFormatPattern;
            }
            PatternParseResult<T> patternResult = patternParser(formatInfo).ParsePattern(patternText);
            IPattern<T> pattern = patternResult.GetResultOrThrow();
            return pattern.Format(value);
        }
    }
}
