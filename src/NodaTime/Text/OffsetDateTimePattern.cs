// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
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

        // TODO(V1.2): Use "G" instead when we've got standard patterns (and actually use this constant!)
        private const string DefaultFormatPattern = "yyyy-MM-dd'T'HH:mm:ss z"; // General (long time)

        // TODO(V1.2): Quite possibly change this..
        /// <summary>
        /// Returns an invariant local date/time pattern based on ISO-8601 but with the offset at the end. The calendar
        /// system is not parsed or formatted as part of this pattern.
        /// </summary>
        public static OffsetDateTimePattern RoundtripWithoutCalendarPattern { get { return Patterns.RoundtripWithoutCalendarPatternImpl; } }

        // TODO(V1.2): Quite possibly change this..
        /// <summary>
        /// Returns an invariant local date/time pattern based on ISO-8601 but with the offset and calendar ID at the end.
        /// </summary>
        public static OffsetDateTimePattern RoundtripWithCalendarPattern { get { return Patterns.RoundtripWithCalendarPatternImpl; } }

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        internal static class Patterns
        {
            internal static readonly OffsetDateTimePattern RoundtripWithoutCalendarPatternImpl = Create("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFF o<g>", NodaFormatInfo.InvariantInfo, DefaultTemplateValue);

            internal static readonly OffsetDateTimePattern RoundtripWithCalendarPatternImpl = Create("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFF o<g> c", NodaFormatInfo.InvariantInfo, DefaultTemplateValue);
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
            // TODO(V1.2): Work out the best place to do this test. Currently it's also done in OffsetDateTimePatternParser.
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
    }
}
