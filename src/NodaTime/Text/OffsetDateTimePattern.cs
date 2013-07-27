// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;
using System.Globalization;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="OffsetDateTime"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only cultures for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    public sealed class OffsetDateTimePattern : IPattern<OffsetDateTime>
    {
        internal static readonly OffsetDateTime DefaultTemplateValue = new LocalDateTime(2000, 1, 1, 0, 0).WithOffset(Offset.Zero);

        /// <summary>
        /// Returns an invariant offset date/time pattern based on ISO-8601 (down to the second), including offset from UTC.
        /// The calendar system is not parsed or formatted as part of this pattern. It corresponds to a custom pattern of
        /// "yyyy'-'MM'-'dd'T'HH':'mm':'sso&lt;G&gt;". This pattern is available as the "G"
        /// standard pattern (even though it is invariant).
        /// </summary>
        public static OffsetDateTimePattern GeneralIsoPattern { get { return Patterns.GeneralIsoPatternImpl; } }

        /// <summary>
        /// Returns an invariant offset date/time pattern based on ISO-8601 (down to the tick), including offset from UTC.
        /// The calendar system is not parsed or formatted as part of this pattern. It corresponds to a custom pattern of
        /// "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo&lt;G&gt;". This will round-trip any values
        /// in the ISO calendar, and is available as the "o" standard pattern.
        /// </summary>
        public static OffsetDateTimePattern ExtendedIsoPattern { get { return Patterns.ExtendedIsoPatternImpl; } }

        /// <summary>
        /// Returns an invariant offset date/time pattern based on ISO-8601 (down to the tick)
        /// including offset from UTC and calendar ID. It corresponds to a custom pattern of
        /// "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo&lt;G&gt; '('c')'". This will round-trip any value in any calendar,
        /// and is available as the "r" standard pattern.
        /// </summary>
        public static OffsetDateTimePattern FullRoundtripPattern { get { return Patterns.FullRoundtripPatternImpl; } }

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        internal static class Patterns
        {
            internal static readonly OffsetDateTimePattern GeneralIsoPatternImpl = Create("yyyy'-'MM'-'dd'T'HH':'mm':'sso<G>", NodaFormatInfo.InvariantInfo, DefaultTemplateValue);
            internal static readonly OffsetDateTimePattern ExtendedIsoPatternImpl = Create("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G>", NodaFormatInfo.InvariantInfo, DefaultTemplateValue);
            internal static readonly OffsetDateTimePattern FullRoundtripPatternImpl = Create("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G> '('c')'", NodaFormatInfo.InvariantInfo, DefaultTemplateValue);
            internal static readonly PatternBclSupport<OffsetDateTime> BclSupport = new PatternBclSupport<OffsetDateTime>("G", fi => fi.OffsetDateTimePatternParser);
        }

        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<OffsetDateTime> pattern;
        private readonly OffsetDateTime templateValue;

        /// <summary>
        /// Returns the pattern text for this pattern, as supplied on creation.
        /// </summary>
        public string PatternText { get { return patternText; } }

        /// <summary>
        /// Returns the localization information used in this pattern.
        /// </summary>
        internal NodaFormatInfo FormatInfo { get { return formatInfo; } }

        /// <summary>
        /// Returns the value used as a template for parsing: any field values unspecified
        /// in the pattern are taken from the template.
        /// </summary>
        public OffsetDateTime TemplateValue { get { return templateValue; } }

        private OffsetDateTimePattern(string patternText, NodaFormatInfo formatInfo, OffsetDateTime templateValue,
            IPattern<OffsetDateTime> pattern)
        {
            this.patternText = patternText;
            this.formatInfo = formatInfo;
            this.templateValue = templateValue;
            // TODO(V1.2): Consider exposing all of the above on OffsetDateTimeZonePatternParser instead, and using that.
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
        public ParseResult<OffsetDateTime> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given zoned date/time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The zoned date/time to format.</param>
        /// <returns>The zoned date/time formatted according to this pattern.</returns>
        public string Format(OffsetDateTime value)
        {
            return pattern.Format(value);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, format info, and template value.
        /// </summary>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">The format info to use in the pattern</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting zoned date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        internal static OffsetDateTimePattern Create(string patternText, NodaFormatInfo formatInfo, OffsetDateTime templateValue)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            Preconditions.CheckNotNull(formatInfo, "formatInfo");
            var pattern = new OffsetDateTimePatternParser(templateValue).ParsePattern(patternText, formatInfo);
            return new OffsetDateTimePattern(patternText, formatInfo, templateValue, pattern);
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
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static OffsetDateTimePattern Create(string patternText, CultureInfo cultureInfo, OffsetDateTime templateValue)
        {
            return Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original localization information as this pattern, but with the specified
        /// pattern text.
        /// </summary>
        /// <param name="newPatternText">The pattern text to use in the new pattern.</param>
        /// <returns>A new pattern with the given pattern text.</returns>
        public OffsetDateTimePattern WithPatternText(string newPatternText)
        {
            return Create(newPatternText, formatInfo, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private OffsetDateTimePattern WithFormatInfo(NodaFormatInfo newFormatInfo)
        {
            return Create(patternText, newFormatInfo, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        public OffsetDateTimePattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text and culture as this pattern, but with
        /// the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value to use in the new pattern.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public OffsetDateTimePattern WithTemplateValue(OffsetDateTime newTemplateValue)
        {
            return Create(patternText, formatInfo, newTemplateValue);
        }
    }
}
