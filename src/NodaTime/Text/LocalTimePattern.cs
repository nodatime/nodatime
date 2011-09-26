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
using System.Globalization;
using NodaTime.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="LocalTime"/> values.
    /// </summary>
    public sealed class LocalTimePattern : IPattern<LocalTime>
    {
        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<LocalTime> pattern;

        /// <summary>
        /// Returns the pattern text for this pattern, as supplied on creation.
        /// </summary>
        public string PatternText { get { return patternText; } }

        /// <summary>
        /// Returns the localization information used in this pattern.
        /// </summary>
        public NodaFormatInfo FormatInfo { get { return formatInfo; } }

        private LocalTimePattern(string patternText, NodaFormatInfo formatInfo, IPattern<LocalTime> pattern)
        {
            this.patternText = patternText;
            this.formatInfo = formatInfo;
            this.pattern = pattern;
        }

        /// <summary>
        /// Parses the given text value according to the rules of this pattern.
        /// </summary>
        /// <remarks>
        /// This method never throws an exception (barring a bug in Noda Time itself). Even errors such as
        /// the argument being null are wrapped in a parse result.
        /// </remarks>
        /// <param name="text">The text value to parse.</param>
        /// <returns>The result of parsing, which may be successful or unsuccessful.</returns>
        public ParseResult<LocalTime> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given local time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The local time to format.</param>
        /// <returns>The local time formatted according to this pattern.</returns>
        public string Format(LocalTime value)
        {
            return pattern.Format(value);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text and format info.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">Localization information</param>
        /// <returns>A pattern for parsing and formatting local times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalTimePattern Create(string patternText, NodaFormatInfo formatInfo)
        {
            // TODO: Work out the best place to do this test. Currently it's also done in LocalTimePatternParser.
            if (patternText == null)
            {
                throw new ArgumentNullException("patternText");
            }            
            if (formatInfo == null)
            {
                throw new ArgumentNullException("formatInfo");
            }
            var patternParseResult = formatInfo.LocalTimePatternParser.ParsePattern(patternText);
            return new LocalTimePattern(patternText, formatInfo, patternParseResult.GetResultOrThrow());
        }

        /// <summary>
        /// Creates a pattern for the given pattern text in the current thread's current culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options. Note that the current culture
        /// is captured at the time this method is called - it is not captured at the point of parsing
        /// or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting local times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalTimePattern CreateWithCurrentCulture(string patternText)
        {
            return Create(patternText, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text in the invariant culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options. Note that the current culture
        /// is captured at the time this method is called - it is not captured at the point of parsing
        /// or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting local times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalTimePattern CreateWithInvariantInfo(string patternText)
        {
            return Create(patternText, NodaFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="formatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        public LocalTimePattern WithFormatInfo(NodaFormatInfo formatInfo)
        {
            return Create(patternText, formatInfo);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture information.</returns>
        public LocalTimePattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }
    }
}
