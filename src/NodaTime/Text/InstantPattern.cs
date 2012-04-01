#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
    /// Represents a pattern for parsing and formatting <see cref="Instant"/> values.
    /// </summary>
    public sealed class InstantPattern : IPattern<Instant>
    {
        /// <summary>
        /// Returns the general pattern, which always uses an invariant culture. The general pattern represents
        /// an instant as a UTC date/time in ISO-8601 style "yyyy-MM-ddTHH:mm:ssZ".
        /// </summary>
        public static InstantPattern GeneralPattern { get { return Patterns.GeneralPatternImpl; } }

        /// <summary>
        /// Returns an invariant instant pattern which is ISO-8601 compatible other than providing up to 7 decimal places
        /// of sub-second accuracy. (These digits are omitted when unnecessary.)
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'".
        /// </summary>
        public static InstantPattern ExtendedIsoPattern { get { return Patterns.ExtendedIsoPatternImpl; } }

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        private static class Patterns
        {
            internal static readonly InstantPattern ExtendedIsoPatternImpl = CreateWithInvariantInfo("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFF'Z'");
            internal static readonly InstantPattern GeneralPatternImpl = CreateWithInvariantInfo("g");
        }

        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<Instant> pattern;

        /// <summary>
        /// Returns the pattern text for this pattern, as supplied on creation.
        /// </summary>
        public string PatternText { get { return patternText; } }

        /// <summary>
        /// Returns the localization information used in this pattern.
        /// </summary>
        public NodaFormatInfo FormatInfo { get { return formatInfo; } }

        private InstantPattern(string patternText, NodaFormatInfo formatInfo, IPattern<Instant> pattern)
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
        public ParseResult<Instant> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given instant as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The offset to format.</param>
        /// <returns>The instant formatted according to this pattern.</returns>
        public string Format(Instant value)
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
        /// <returns>A pattern for parsing and formatting offsets.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static InstantPattern Create(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                throw new ArgumentNullException("patternText");
            }
            if (formatInfo == null)
            {
                throw new ArgumentNullException("formatInfo");
            }
            var patternParseResult = formatInfo.InstantPatternParser.ParsePattern(patternText);
            return new InstantPattern(patternText, formatInfo, patternParseResult.GetResultOrThrow());
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
        /// <returns>A pattern for parsing and formatting offsets.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static InstantPattern CreateWithCurrentCulture(string patternText)
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
        /// <returns>A pattern for parsing and formatting offsets.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static InstantPattern CreateWithInvariantInfo(string patternText)
        {
            return Create(patternText, NodaFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Creates a "numeric" pattern for the given format information. The numeric format gives the
        /// number of ticks in decimal format, with or without thousands separators.
        /// </summary>
        /// <param name="formatInfo">The culture-specific information to use when formatting or parsing.</param>
        /// <param name="includeThousandsSeparators">True to include thousands separators when parsing or formatting; false to omit them.</param>
        /// <returns>A numeric pattern for the configuration</returns>
        public static InstantPattern CreateNumericPattern(NodaFormatInfo formatInfo, bool includeThousandsSeparators)
        {
            return Create(includeThousandsSeparators ? "n" : "d", formatInfo);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="formatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        public InstantPattern WithFormatInfo(NodaFormatInfo formatInfo)
        {
            return Create(patternText, formatInfo);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture information.</returns>
        public InstantPattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }
    }
}
