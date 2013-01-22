// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
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
    internal sealed class ZonedDateTimePattern : IPattern<ZonedDateTime>
    {
        internal static readonly ZonedDateTime DefaultTemplateValue = new LocalDateTime(2000, 1, 1, 0, 0).InUtc();

        // TODO: Use "G" instead when we've got standard patterns
        private const string DefaultFormatPattern = "yyyy-MM-dd'T'HH:mm:ss z"; // General (long time)

        private readonly string patternText;
        private readonly NodaFormatInfo formatInfo;
        private readonly IPattern<ZonedDateTime> pattern;
        private readonly ZonedDateTime templateValue;
        private readonly ZoneLocalMappingResolver resolver;
        private readonly IDateTimeZoneProvider zoneProvider;

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
        public ZonedDateTime TemplateValue { get { return templateValue; } }

        /// <summary>
        /// Returns the resolver which is used to map local date/times to zoned date/times,
        /// handling skipped and ambiguous times appropriately.
        /// </summary>
        public ZoneLocalMappingResolver Resolver { get { return resolver; } }

        /// <summary>
        /// Returns the provider which is used to look up time zones when parsing a pattern
        /// which contains a time zone identifier.
        /// </summary>
        public IDateTimeZoneProvider ZoneProvider { get { return zoneProvider; } }

        private ZonedDateTimePattern(string patternText, NodaFormatInfo formatInfo, ZonedDateTime templateValue,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, IPattern<ZonedDateTime> pattern)
        {
            this.patternText = patternText;
            this.formatInfo = formatInfo;
            this.templateValue = templateValue;
            this.resolver = resolver;
            this.zoneProvider = zoneProvider;
            // TODO: Consider exposing all of the above on ZonedDateTimeZonePatternParser instead, and using that.
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
        public ParseResult<ZonedDateTime> Parse(string text)
        {
            return pattern.Parse(text);
        }

        /// <summary>
        /// Formats the given zoned date/time as text according to the rules of this pattern.
        /// </summary>
        /// <param name="value">The zoned date/time to format.</param>
        /// <returns>The zoned date/time formatted according to this pattern.</returns>
        public string Format(ZonedDateTime value)
        {
            return pattern.Format(value);
        }

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
        internal static ZonedDateTimePattern Create(string patternText, NodaFormatInfo formatInfo,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, ZonedDateTime templateValue)
        {
            // TODO(Post-V1): Work out the best place to do this test. Currently it's also done in LocalDateTimePatternParser.
            Preconditions.CheckNotNull(patternText, "patternText");
            Preconditions.CheckNotNull(formatInfo, "formatInfo");
            Preconditions.CheckNotNull(resolver, "resolver");
            Preconditions.CheckNotNull(zoneProvider, "zoneProvider");
            // TODO: Work out whether there should be a "fixed" pattern parser at all. We only need to format from the
            // BCL support, so that doesn't need a provider.
            var pattern = new ZonedDateTimePatternParser(templateValue, resolver, zoneProvider).ParsePattern(patternText, formatInfo);
            return new ZonedDateTimePattern(patternText, formatInfo, templateValue, resolver, zoneProvider, pattern);
        }

        /// <summary>
        /// Creates a pattern for the given pattern text, culture, resolver, time zone provider, and template value.
        /// </summary>
        /// <remarks>
        /// See the user guide for the available pattern text options.
        /// </remarks>
        /// <param name="patternText">Pattern text to create the pattern for</param>
        /// <param name="cultureInfo">The culture to use in the pattern</param>
        /// <param name="resolver">Resolver to apply when mapping local date/time values into the zone.</param>
        /// <param name="zoneProvider">Time zone provider, used when parsing text which contains a time zone identifier.</param>
        /// <param name="templateValue">Template value to use for unspecified fields</param>
        /// <returns>A pattern for parsing and formatting local date/times.</returns>
        /// <exception cref="InvalidPatternException">The pattern text was invalid.</exception>
        public static ZonedDateTimePattern Create(string patternText, CultureInfo cultureInfo,
            ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider, ZonedDateTime templateValue)
        {
            return Create(patternText, NodaFormatInfo.GetFormatInfo(cultureInfo), resolver, zoneProvider, templateValue);
        }

        public ZonedDateTimePattern WithPatternText(string newPatternText)
        {
            return Create(newPatternText, formatInfo, resolver, zoneProvider, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// localization information.
        /// </summary>
        /// <param name="newFormatInfo">The localization information to use in the new pattern.</param>
        /// <returns>A new pattern with the given localization information.</returns>
        private ZonedDateTimePattern WithFormatInfo(NodaFormatInfo newFormatInfo)
        {
            return Create(patternText, newFormatInfo, resolver, zoneProvider, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// culture.
        /// </summary>
        /// <param name="cultureInfo">The culture to use in the new pattern.</param>
        /// <returns>A new pattern with the given culture.</returns>
        public ZonedDateTimePattern WithCulture(CultureInfo cultureInfo)
        {
            return WithFormatInfo(NodaFormatInfo.GetFormatInfo(cultureInfo));
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// resolver.
        /// </summary>
        /// <param name="newResolver">The new local mapping resolver to use.</param>
        /// <returns>A new pattern with the given resolver.</returns>
        public ZonedDateTimePattern WithResolver(ZoneLocalMappingResolver newResolver)
        {
            return Create(patternText, formatInfo, newResolver, zoneProvider, templateValue);
        }

        /// <summary>
        /// Creates a pattern for the same original pattern text as this pattern, but with the specified
        /// time zone provider.
        /// </summary>
        /// <param name="newZoneProvider">The new time zone provider to use.</param>
        /// <returns>A new pattern with the given time zone provider.</returns>
        public ZonedDateTimePattern WithZoneProvider(IDateTimeZoneProvider newZoneProvider)
        {
            return Create(patternText, formatInfo, resolver, newZoneProvider, templateValue);
        }

        /// <summary>
        /// Creates a pattern like this one, but with the specified template value.
        /// </summary>
        /// <param name="newTemplateValue">The template value for the new pattern, used to fill in unspecified fields.</param>
        /// <returns>A new pattern with the given template value.</returns>
        public ZonedDateTimePattern WithTemplateValue(ZonedDateTime newTemplateValue)
        {
            return Create(patternText, formatInfo, resolver, zoneProvider, newTemplateValue);
        }
    }
}
