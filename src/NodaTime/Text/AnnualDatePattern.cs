// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using System.Text;
using NodaTime.Annotations;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;
using JetBrains.Annotations;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="AnnualDate"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only cultures for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    [Immutable] // Well, assuming an immutable culture...
    public sealed class AnnualDatePattern : IPattern<AnnualDate>
    {
        internal static readonly AnnualDate DefaultTemplateValue = new AnnualDate(1, 1);

        private const string DefaultFormatPattern = "G"; // General, ISO-like

        internal static readonly PatternBclSupport<AnnualDate> BclSupport =
            new PatternBclSupport<AnnualDate>(DefaultFormatPattern, fi => fi.AnnualDatePatternParser);

        /// <summary>
        /// Gets an invariant annual date pattern which is compatible with the month/day part of ISO-8601.
        /// This corresponds to the text pattern "MM'-'dd".
        /// </summary>
        /// <value>An invariant annual date pattern which is compatible with the month/day part of ISO-8601.</value>
        [NotNull] public static AnnualDatePattern Iso => Patterns.IsoPatternImpl;

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        private static class Patterns
        {
            internal static readonly AnnualDatePattern IsoPatternImpl = CreateWithInvariantCulture("MM'-'dd");
        }

        /// <summary>
        /// Returns the pattern that this object delegates to. Mostly useful to avoid this public class
        /// implementing an internal interface.
        /// </summary>
        internal IPartialPattern<AnnualDate> UnderlyingPattern { get; }

        /// <summary>
        /// Gets the pattern text for this pattern, as supplied on creation.
        /// </summary>
        /// <value>The pattern text for this pattern, as supplied on creation.</value>
        [NotNull] public string PatternText { get; }

        /// <summary>
        /// Returns the localization information used in this pattern.
        /// </summary>
        internal NodaFormatInfo FormatInfo { get; }

        /// <summary>
        /// Gets the value used as a template for parsing: any field values unspecified
        /// in the pattern are taken from the template.
        /// </summary>
        /// <value>The value used as a template for parsing.</value>
        public AnnualDate TemplateValue { get; }

        private AnnualDatePattern(string patternText, NodaFormatInfo formatInfo, AnnualDate templateValue,
            IPartialPattern<AnnualDate> pattern)
        {
            PatternText = patternText;
            FormatInfo = formatInfo;
            TemplateValue = templateValue;
            UnderlyingPattern = pattern;
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
        [NotNull] public ParseResult<AnnualDate> Parse([SpecialNullHandling] string text) => UnderlyingPattern.Parse(text);

        /// <summary>
        /// Formats the given annual date as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The annual date to format.</param>
        /// <returns>The annual date formatted according to this pattern.</returns>
        [NotNull] public string Format(AnnualDate value) => UnderlyingPattern.Format(value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        [NotNull] public StringBuilder AppendFormat(AnnualDate value, [NotNull] StringBuilder builder) => UnderlyingPattern.AppendFormat(value, builder);

        /// <summary>
        /// Creates a pattern for the given pattern text, format info, and template value.
        /// </summary>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">The format info to use in the pattern</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting annual dates.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        internal static AnnualDatePattern Create([NotNull] string patternText, [NotNull] NodaFormatInfo formatInfo,
            AnnualDate templateValue)
        {
            Preconditions.CheckNotNull(patternText, nameof(patternText));
            Preconditions.CheckNotNull(formatInfo, nameof(formatInfo));
            // Use the "fixed" parser for the common case of the default template value.
            var pattern = templateValue == DefaultTemplateValue
                ? formatInfo.AnnualDatePatternParser.ParsePattern(patternText)
                : new AnnualDatePatternParser(templateValue).ParsePattern(patternText, formatInfo);
            // If ParsePattern returns a standard pattern instance, we need to get the underlying partial pattern.
            pattern = (pattern as AnnualDatePattern)?.UnderlyingPattern ?? pattern;
            var partialPattern = (IPartialPattern<AnnualDate>) pattern;
            return new AnnualDatePattern(patternText, formatInfo, templateValue, partialPattern);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, culture, and template value.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting annual dates.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static AnnualDatePattern Create([NotNull] string patternText, [NotNull] CultureInfo cultureInfo, AnnualDate templateValue) =>
            Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), templateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text and culture, with a template value of 2000-01-01.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <returns>A pattern for parsing and formatting annual dates.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static AnnualDatePattern Create([NotNull] string patternText, [NotNull] CultureInfo cultureInfo) =>
            Create(patternText, cultureInfo, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text in the current thread's current culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options. Note that the current culture
        /// is captured at the time this method is called - it is not captured at the point of parsing
        /// or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting annual dates.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static AnnualDatePattern CreateWithCurrentCulture([NotNull] string patternText) =>
            Create(patternText, NodaFormatInfo.CurrentInfo, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text in the invariant culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options. Note that the current culture
        /// is captured at the time this method is called - it is not captured at the point of parsing
        /// or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting annual dates.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        [NotNull] public static AnnualDatePattern CreateWithInvariantCulture([NotNull] string patternText) =>
            Create(patternText, NodaFormatInfo.InvariantInfo, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="formatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private AnnualDatePattern WithFormatInfo([NotNull] NodaFormatInfo formatInfo) =>
            Create(PatternText, formatInfo, TemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        [NotNull] public AnnualDatePattern WithCulture([NotNull] CultureInfo cultureInfo) =>
            WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        [NotNull] public AnnualDatePattern WithTemplateValue(AnnualDate newTemplateValue) =>
            Create(PatternText, FormatInfo, newTemplateValue);
    }
}
