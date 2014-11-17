// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Text;
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
        /// Gets an invariant local date/time pattern which is ISO-8601 compatible, down to the second.
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss", and is also used as the "sortable"
        /// standard pattern.
        /// </summary>
        /// <value>An invariant local date/time pattern which is ISO-8601 compatible, down to the second.</value>
        public static LocalDateTimePattern GeneralIsoPattern => Patterns.GeneralIsoPatternImpl;

        /// <summary>
        /// Gets an invariant local date/time pattern which is ISO-8601 compatible, providing up to 9 decimal places
        /// of sub-second accuracy. (These digits are omitted when unnecessary.)
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF".
        /// </summary>
        /// <value>An invariant local date/time pattern which is ISO-8601 compatible, providing up to 9 decimal places
        /// of sub-second accuracy.</value>
        public static LocalDateTimePattern ExtendedIsoPattern => Patterns.ExtendedIsoPatternImpl;

        /// <summary>
        /// Gets an invariant local date/time pattern which is ISO-8601 compatible, providing up to 7 decimal places
        /// of sub-second accuracy which are always present (including trailing zeroes). This is compatible with the
        /// BCL round-trip formatting of <see cref="DateTime"/> values with a kind of "unspecified".
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff".
        /// FIXME(2.0): What should we do with this? Maintain BCL compatibility, or go to nanos?
        /// </summary>
        /// <value>An invariant local date/time pattern which is ISO-8601 compatible, providing up to 7 decimal places
        /// of sub-second accuracy which are always present (including trailing zeroes).</value>
        public static LocalDateTimePattern BclRoundtripPattern => Patterns.BclRoundtripPatternImpl;

        /// <summary>
        /// Gets an invariant local date/time pattern which round trips values including the calendar system.
        /// This corresponds to the text pattern "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff '('c')'".
        /// </summary>
        /// <value>An invariant local date/time pattern which round trips values including the calendar system.</value>
        public static LocalDateTimePattern FullRoundtripPattern => Patterns.FullRoundtripPatternImpl;

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        internal static class Patterns
        {
            internal static readonly LocalDateTimePattern GeneralIsoPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss");
            internal static readonly LocalDateTimePattern ExtendedIsoPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF");
            internal static readonly LocalDateTimePattern BclRoundtripPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff");
            internal static readonly LocalDateTimePattern FullRoundtripPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffff '('c')'");
        }

        private readonly IPattern<LocalDateTime> pattern;

        /// <summary>
        /// Gets the pattern text for this pattern, as supplied on creation.
        /// </summary>
        /// <value>The pattern text for this pattern, as supplied on creation.</value>
        public string PatternText { get; }

        /// <summary>
        /// Gets the localization information used in this pattern.
        /// </summary>
        /// <value>The localization information used in this pattern.</value>
        internal NodaFormatInfo FormatInfo { get; }

        /// <summary>
        /// Get the value used as a template for parsing: any field values unspecified
        /// in the pattern are taken from the template.
        /// </summary>
        /// <value>The value used as a template for parsing.</value>
        public LocalDateTime TemplateValue { get; }

        private LocalDateTimePattern(string patternText, NodaFormatInfo formatInfo, LocalDateTime templateValue,
            IPattern<LocalDateTime> pattern)
        {
            this.PatternText = patternText;
            this.FormatInfo = formatInfo;
            this.pattern = pattern;
            this.TemplateValue = templateValue;
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
        public ParseResult<LocalDateTime> Parse(string text) => pattern.Parse(text);

        /// <summary>
        /// Formats the given local date/time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The local date/time to format.</param>
        /// <returns>The local date/time formatted according to this pattern.</returns>
        public string Format(LocalDateTime value) => pattern.Format(value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        public StringBuilder AppendFormat(LocalDateTime value, StringBuilder builder) => pattern.AppendFormat(value, builder);

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
            Preconditions.CheckNotNull(patternText, nameof(patternText));
            Preconditions.CheckNotNull(formatInfo, nameof(formatInfo));
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
        public static LocalDateTimePattern Create(string patternText, CultureInfo cultureInfo, LocalDateTime templateValue) =>
            Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), templateValue);

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
        public static LocalDateTimePattern Create(string patternText, CultureInfo cultureInfo) =>
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
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDateTimePattern CreateWithCurrentCulture(string patternText) =>
            Create(patternText, NodaFormatInfo.CurrentInfo, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text in the invariant culture.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static LocalDateTimePattern CreateWithInvariantCulture(string patternText) =>
            Create(patternText, NodaFormatInfo.InvariantInfo, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private LocalDateTimePattern WithFormatInfo(NodaFormatInfo newFormatInfo) =>
            Create(PatternText, newFormatInfo, TemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        public LocalDateTimePattern WithCulture(CultureInfo cultureInfo) =>
            WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public LocalDateTimePattern WithTemplateValue(LocalDateTime newTemplateValue) =>
            Create(PatternText, FormatInfo, newTemplateValue);
    }
}
