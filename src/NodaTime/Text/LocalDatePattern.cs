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

using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="LocalDate"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with read-only <see cref="NodaFormatInfo" /> using a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only formatting information for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    public sealed class LocalDatePattern : IPattern<LocalDate>
    {
        private static readonly int TypeInitializationChecking = TypeInitializationChecker.RecordInitializationStart();

        internal static readonly LocalDate DefaultTemplateValue = new LocalDate(2000, 1, 1);

        private const string DefaultFormatPattern = "D"; // Long

        internal static readonly PatternBclSupport<LocalDate> BclSupport =
            new PatternBclSupport<LocalDate>(DefaultFormatPattern, fi => fi.LocalDatePatternParser);

        /// <summary>
        /// Returns an invariant local date pattern which is ISO-8601 compatible.
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd".
        /// </summary>
        public static LocalDatePattern IsoPattern { get { return Patterns.IsoPatternImpl; } }
        
        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        private static class Patterns
        {
            private static readonly int TypeInitializationChecking = TypeInitializationChecker.RecordInitializationStart();
            internal static readonly LocalDatePattern IsoPatternImpl = CreateWithInvariantInfo("yyyy'-'MM'-'dd");
        }

        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<LocalDate> pattern;
        private readonly LocalDate templateValue;

        /// <summary>
        /// Returns the pattern text for this pattern, as supplied on creation.
        /// </summary>
        public string PatternText { get { return patternText; } }

        /// <summary>
        /// Returns the localization information used in this pattern.
        /// </summary>
        public NodaFormatInfo FormatInfo { get { return formatInfo; } }

        /// <summary>
        /// Returns the value used as a template for parsing: any field values unspecified
        /// in the pattern are taken from the template.
        /// </summary>
        public LocalDate TemplateValue { get { return templateValue; } }

        private LocalDatePattern(string patternText, NodaFormatInfo formatInfo, LocalDate templateValue,
            IPattern<LocalDate> pattern)
        {
            this.patternText = patternText;
            this.formatInfo = formatInfo;
            this.pattern = pattern;
            this.templateValue = templateValue;
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
        public ParseResult<LocalDate> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given local time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The local time to format.</param>
        /// <returns>The local time formatted according to this pattern.</returns>
        public string Format(LocalDate value)
        {
            return pattern.Format(value);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, format info, and template value.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">Localization information</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting local times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDatePattern Create(string patternText, NodaFormatInfo formatInfo, LocalDate templateValue)
        {
            // TODO(Post-V1): Work out the best place to do this test. Currently it's also done in LocalDatePatternParser.
            Preconditions.CheckNotNull(patternText, "patternText");
            Preconditions.CheckNotNull(formatInfo, "formatInfo");
            // Use the "fixed" parser for the common case of the default template value.
            var patternParseResult = templateValue == DefaultTemplateValue
                ? formatInfo.LocalDatePatternParser.ParsePattern(patternText)
                : new LocalDatePatternParser(templateValue).ParsePattern(patternText, formatInfo);
            return new LocalDatePattern(patternText, formatInfo, templateValue, patternParseResult.GetResultOrThrow());
        }

        /// <summary>
        /// Creates a pattern for the given pattern text and format info, with a template value of midnight.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">Localization information</param>
        /// <returns>A pattern for parsing and formatting local times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDatePattern Create(string patternText, NodaFormatInfo formatInfo)
        {
            return Create(patternText, formatInfo, DefaultTemplateValue);
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
        public static LocalDatePattern CreateWithCurrentCulture(string patternText)
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
        public static LocalDatePattern CreateWithInvariantInfo(string patternText)
        {
            return Create(patternText, NodaFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        public LocalDatePattern WithFormatInfo(NodaFormatInfo newFormatInfo)
        {
            return Create(patternText, newFormatInfo, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture information.</returns>
        public LocalDatePattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public LocalDatePattern WithTemplateValue(LocalDate newTemplateValue)
        {
            return Create(patternText, formatInfo, newTemplateValue);
        }
    }
}
