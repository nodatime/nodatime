// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Annotations;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="LocalDateTime"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only cultures for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    [Immutable] // Well, assuming an immutable culture...
    public sealed class LocalDateTimePattern : IPattern<LocalDateTime>
    {
        internal static readonly LocalDateTime DefaultTemplateValue = new LocalDateTime(2000, 1, 1, 0, 0);

        private const string DefaultFormatPattern = "G"; // General (long time)

        internal static readonly PatternBclSupport<LocalDateTime> BclSupport =
            new PatternBclSupport<LocalDateTime>(DefaultFormatPattern, fi => fi.LocalDateTimePatternParser);

        /// <summary>
        /// Returns an invariant local date/time pattern which is ISO-8601 compatible, down to the second.
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss", and is also used as the "sortable"
        /// standard pattern.
        /// </summary>
        public static LocalDateTimePattern GeneralIsoPattern { get { return Patterns.GeneralIsoPatternImpl; } }

        /// <summary>
        /// Returns an invariant local date/time pattern which is ISO-8601 compatible, providing up to 7 decimal places
        /// of sub-second accuracy. (These digits are omitted when unnecessary.)
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFF".
        /// </summary>
        public static LocalDateTimePattern ExtendedIsoPattern { get { return Patterns.ExtendedIsoPatternImpl; } }

        /// <summary>
        /// Returns an invariant local date/time pattern which is ISO-8601 compatible, providing up to 7 decimal places
        /// of sub-second accuracy which are always present (including trailing zeroes). This is compatible with the
        /// BCL round-trip formatting of <see cref="DateTime"/> values with a kind of "unspecified".
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff".
        /// FIXME(2.0): What should we do with this? Maintain BCL compatibility, or go to nanos?
        /// </summary>
        public static LocalDateTimePattern BclRoundtripPattern { get { return Patterns.BclRoundtripPatternImpl; } }

        /// <summary>
        /// Returns an invariant local date/time pattern which round trips values including the calendar system.
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff '('c')'".
        /// </summary>
        public static LocalDateTimePattern FullRoundtripPattern { get { return Patterns.FullRoundtripPatternImpl; } }

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        internal static class Patterns
        {
            internal static readonly LocalDateTimePattern GeneralIsoPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            internal static readonly LocalDateTimePattern ExtendedIsoPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFF");
            internal static readonly LocalDateTimePattern BclRoundtripPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff");
            internal static readonly LocalDateTimePattern FullRoundtripPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff '('c')'");
        }

        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<LocalDateTime> pattern;
        private readonly LocalDateTime templateValue;

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
        public LocalDateTime TemplateValue { get { return templateValue; } }

        private LocalDateTimePattern(string patternText, NodaFormatInfo formatInfo, LocalDateTime templateValue,
            IPattern<LocalDateTime> pattern)
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
        public ParseResult<LocalDateTime> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given local date/time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The local date/time to format.</param>
        /// <returns>The local date/time formatted according to this pattern.</returns>
        public string Format(LocalDateTime value)
        {
            return pattern.Format(value);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, format info, and template value.
        /// </summary>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">The format info to use in the pattern</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        private static LocalDateTimePattern Create(string patternText, NodaFormatInfo formatInfo, LocalDateTime templateValue)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            Preconditions.CheckNotNull(formatInfo, "formatInfo");
            // Use the "fixed" parser for the common case of the default template value.
            var pattern = templateValue == DefaultTemplateValue
                ? formatInfo.LocalDateTimePatternParser.ParsePattern(patternText)
                : new LocalDateTimePatternParser(templateValue).ParsePattern(patternText, formatInfo);
            return new LocalDateTimePattern(patternText, formatInfo, templateValue, pattern);
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
        public static LocalDateTimePattern Create(string patternText, CultureInfo cultureInfo, LocalDateTime templateValue)
        {
            return Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), templateValue);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text and culture, with a template value of midnight on 2000-01-01.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDateTimePattern Create(string patternText, CultureInfo cultureInfo)
        {
            return Create(patternText, cultureInfo, DefaultTemplateValue);
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
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDateTimePattern CreateWithCurrentCulture(string patternText)
        {
            return Create(patternText, NodaFormatInfo.CurrentInfo, DefaultTemplateValue);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text in the invariant culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDateTimePattern CreateWithInvariantCulture(string patternText)
        {
            return Create(patternText, NodaFormatInfo.InvariantInfo, DefaultTemplateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private LocalDateTimePattern WithFormatInfo(NodaFormatInfo newFormatInfo)
        {
            return Create(patternText, newFormatInfo, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        public LocalDateTimePattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public LocalDateTimePattern WithTemplateValue(LocalDateTime newTemplateValue)
        {
            return Create(patternText, formatInfo, newTemplateValue);
        }
    }
}
