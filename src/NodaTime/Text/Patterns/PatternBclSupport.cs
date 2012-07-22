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
        private readonly NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser;
        private readonly string defaultFormatPattern;

        internal PatternBclSupport(string defaultFormatPattern, NodaFunc<NodaFormatInfo, FixedFormatInfoPatternParser<T>> patternParser)
        {
            this.patternParser = patternParser;
            this.defaultFormatPattern = defaultFormatPattern;
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
