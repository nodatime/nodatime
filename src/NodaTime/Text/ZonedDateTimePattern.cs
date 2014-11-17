// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Globalization;
using System.Text;
using NodaTime.Annotations;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Represents a pattern for parsing and formatting <see cref="ZonedDateTime"/> values.
    /// </summary>
    /// <threadsafety>
    /// When used with a read-only <see cref="CultureInfo" />, this type is immutable and instances
    /// may be shared freely between threads. We recommend only using read-only cultures for patterns, although this is
    /// not currently enforced.
    /// </threadsafety>
    [Immutable] // Well, assuming an immutable culture...
    public sealed class ZonedDateTimePattern : IPattern<ZonedDateTime>
    {
        internal static ZonedDateTime DefaultTemplateValue { get; } = new LocalDateTime(2000, 1, 1, 0, 0).InUtc();

        /// <summary>
        /// Gets an zoned local date/time pattern based on ISO-8601 (down to the second) including offset from UTC and zone ID.
        /// It corresponds to a custom pattern of "yyyy'-'MM'-'dd'T'HH':'mm':'ss z '('o&lt;g&gt;')'" and is available
        /// as the 'G' standard pattern.
        /// </summary>
        /// <remarks>
        /// The calendar system is not formatted as part of this pattern, and it cannot be used for parsing as no time zone
        /// provider is included. Call <see cref="WithZoneProvider"/> on the value of this property to obtain a
        /// pattern which can be used for parsing.
        /// </remarks>
        /// <value>An zoned local date/time pattern based on ISO-8601 (down to the second) including offset from UTC and zone ID.</value>
        public static ZonedDateTimePattern GeneralFormatOnlyIsoPattern => Patterns.GeneralFormatOnlyPatternImpl;

        // TODO(2.0): Add tests for this and other patterns from properties.
        /// <summary>
        /// Returns an invariant zoned date/time pattern based on ISO-8601 (down to the nanosecond) including offset from UTC and zone ID.
        /// It corresponds to a custom pattern of "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF z '('o&lt;g&gt;')'" and is available
        /// as the 'F' standard pattern.
        /// </summary>
        /// <remarks>
        /// The calendar system is not formatted as part of this pattern, and it cannot be used for parsing as no time zone
        /// provider is included. Call <see cref="WithZoneProvider"/> on the value of this property to obtain a
        /// pattern which can be used for parsing.
        /// </remarks>
        /// <value>An invariant zoned date/time pattern based on ISO-8601 (down to the nanosecond) including offset from UTC and zone ID.</value>
        public static ZonedDateTimePattern ExtendedFormatOnlyIsoPattern => Patterns.ExtendedFormatOnlyPatternImpl;

        private readonly IPattern<ZonedDateTime> pattern;

        /// <summary>
        /// Class whose existence is solely to avoid type initialization order issues, most of which stem
        /// from needing NodaFormatInfo.InvariantInfo...
        /// </summary>
        internal static class Patterns
        {
            internal static readonly ZonedDateTimePattern GeneralFormatOnlyPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss z '('o<g>')'", null);
            internal static readonly ZonedDateTimePattern ExtendedFormatOnlyPatternImpl = CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFFF z '('o<g>')'", null);
            internal static readonly PatternBclSupport<ZonedDateTime> BclSupport = new PatternBclSupport<ZonedDateTime>("G", fi => fi.ZonedDateTimePatternParser);
        }

        /// <summary>
        /// Gets the pattern text for this pattern, as supplied on creation.
        /// </summary>
        /// <value>The pattern text for this pattern, as supplied on creation.</value>
        public string PatternText { get; }

        /// <summary>
        /// Gets the localization information used in this pattern.
        /// </summary>
        internal NodaFormatInfo FormatInfo { get; }

        /// <summary>
        /// Gets the value used as a template for parsing: any field values unspecified
        /// in the pattern are taken from the template.
        /// </summary>
        /// <value>The value used as a template for parsing.</value>
        public ZonedDateTime TemplateValue { get; }

        /// <summary>
        /// Gets the resolver which is used to map local date/times to zoned date/times,
        /// handling skipped and ambiguous times appropriately (where the offset isn't specified in the pattern).
        /// </summary>
        /// <value>The resolver which is used to map local date/times to zoned date/times.</value>
        public ZoneLocalMappingResolver Resolver { get; }

        /// <summary>
        /// Gets the provider which is used to look up time zones when parsing a pattern
        /// which contains a time zone identifier. This may be null, in which case the pattern can
        /// only be used for formatting (not parsing).
        /// </summary>
        /// <value>The provider which is used to look up time zones when parsing a pattern
        /// which contains a time zone identifier.</value>
        public IDateTimeZoneProvider ZoneProvider { get; }

        private ZonedDateTimePattern(string patternText, NodaFormatInfo formatInfo, ZonedDateTime templateValue,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, IPattern<ZonedDateTime> pattern)
        {
            this.PatternText = patternText;
            this.FormatInfo = formatInfo;
            this.TemplateValue = templateValue;
            this.Resolver = resolver;
            this.ZoneProvider = zoneProvider;
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
        public ParseResult<ZonedDateTime> Parse(string text) => pattern.Parse(text);

        /// <summary>
        /// Formats the given zoned date/time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The zoned date/time to format.</param>
        /// <returns>The zoned date/time formatted according to this pattern.</returns>
        public string Format(ZonedDateTime value) => pattern.Format(value);

        /// <summary>
        /// Formats the given value as text according to the rules of this pattern,
        /// appending to the given <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="builder">The <c>StringBuilder</c> to append to.</param>
        /// <returns>The builder passed in as <paramref name="builder"/>.</returns>
        public StringBuilder AppendFormat(ZonedDateTime value, StringBuilder builder) => pattern.AppendFormat(value, builder);

        /// <summary>
        /// Creates a pattern for the given pattern text, format info, template value, mapping resolver and time zone provider.
        /// </summary>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="formatInfo">The format info to use in the pattern</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <param name="resolver">Resolver to apply when mapping local date/time values into the zone.</param>
        /// <param name="zoneProvider">Time zone provider, used when parsing text which contains a time zone identifier.</param>
        /// <returns>A pattern for parsing and formatting zoned date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        private static ZonedDateTimePattern Create(string patternText, NodaFormatInfo formatInfo,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, ZonedDateTime templateValue)
        {
            Preconditions.CheckNotNull(patternText, nameof(patternText));
            Preconditions.CheckNotNull(formatInfo, nameof(formatInfo));
            Preconditions.CheckNotNull(resolver, nameof(resolver));
            var pattern = new ZonedDateTimePatternParser(templateValue, resolver, zoneProvider).ParsePattern(patternText, formatInfo);
            return new ZonedDateTimePattern(patternText, formatInfo, templateValue, resolver, zoneProvider, pattern);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, culture, resolver, time zone provider, and template value.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// If <paramref name="zoneProvider"/> is null, the resulting pattern can be used for formatting
        /// but not parsing.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <param name="resolver">Resolver to apply when mapping local date/time values into the zone.</param>
        /// <param name="zoneProvider">Time zone provider, used when parsing text which contains a time zone identifier.</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting zoned date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static ZonedDateTimePattern Create(string patternText, CultureInfo cultureInfo,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, ZonedDateTime templateValue) =>
            Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), resolver, zoneProvider, templateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text and time zone provider, using a strict resolver, the invariant
        /// culture, and a default template value of midnight January 1st 2000 UTC.
        /// </summary>
        /// <remarks>
        /// The resolver is only used if the pattern text doesn't include an offset.
        /// If <paramref name="zoneProvider"/> is null, the resulting pattern can be used for formatting
        /// but not parsing.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="zoneProvider">Time zone provider, used when parsing text which contains a time zone identifier.</param>
        /// <returns>A pattern for parsing and formatting zoned date/times.</returns>
        public static ZonedDateTimePattern CreateWithInvariantCulture(string patternText, IDateTimeZoneProvider zoneProvider) =>
            Create(patternText, NodaFormatInfo.InvariantInfo, Resolvers.StrictResolver, zoneProvider, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the given pattern text and time zone provider, using a strict resolver, the current
        /// culture, and a default template value of midnight January 1st 2000 UTC.
        /// </summary>
        /// <remarks>
        /// The resolver is only used if the pattern text doesn't include an offset.
        /// If <paramref name="zoneProvider"/> is null, the resulting pattern can be used for formatting
        /// but not parsing. Note that the current culture is captured at the time this method is called
        /// - it is not captured at the point of parsing or formatting values.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="zoneProvider">Time zone provider, used when parsing text which contains a time zone identifier.</param>
        /// <returns>A pattern for parsing and formatting zoned date/times.</returns>
        public static ZonedDateTimePattern CreateWithCurrentCulture(string patternText, IDateTimeZoneProvider zoneProvider) =>
            Create(patternText, NodaFormatInfo.CurrentInfo, Resolvers.StrictResolver, zoneProvider, DefaultTemplateValue);

        /// <summary>
        /// Creates a pattern for the same original localization information as this pattern, but with the specified
        /// pattern text.
        /// </summary>
        /// <param name="newPatternText">The pattern text to use in the new pattern.</param>
        /// <returns>A new pattern with the given pattern text.</returns>
        public ZonedDateTimePattern WithPatternText(string newPatternText) =>
            Create(newPatternText, FormatInfo, Resolver, ZoneProvider, TemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private ZonedDateTimePattern WithFormatInfo(NodaFormatInfo newFormatInfo) =>
            Create(PatternText, newFormatInfo, Resolver, ZoneProvider, TemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        public ZonedDateTimePattern WithCulture(CultureInfo cultureInfo) =>
            WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// resolver.
        /// </summary>
        /// <param name="newResolver">The new local mapping resolver to use.</param>
        /// <returns>A new pattern with the given resolver.</returns>
        public ZonedDateTimePattern WithResolver(ZoneLocalMappingResolver newResolver) =>
            Resolver == newResolver ? this :  Create(PatternText, FormatInfo, newResolver, ZoneProvider, TemplateValue);

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// time zone provider.
        /// </summary>
        /// <remarks>
        /// If <paramref name="newZoneProvider"/> is null, the resulting pattern can be used for formatting
        /// but not parsing.
        /// </remarks>
        /// <param name="newZoneProvider">The new time zone provider to use.</param>
        /// <returns>A new pattern with the given time zone provider.</returns>
        public ZonedDateTimePattern WithZoneProvider(IDateTimeZoneProvider newZoneProvider) =>
            newZoneProvider == ZoneProvider ? this : Create(PatternText, FormatInfo, Resolver, newZoneProvider, TemplateValue);

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public ZonedDateTimePattern WithTemplateValue(ZonedDateTime newTemplateValue) =>
            newTemplateValue == TemplateValue ? this : Create(PatternText, FormatInfo, Resolver, ZoneProvider, newTemplateValue);
    }
}
