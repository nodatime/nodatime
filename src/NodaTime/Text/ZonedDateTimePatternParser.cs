// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.TimeZones;

namespace NodaTime.Text
{
    internal sealed class ZonedDateTimePatternParser : IPatternParser<ZonedDateTime>
    {
        // Split the template value once, to avoid doing it every time we parse.
        private readonly LocalDate templateValueDate;
        private readonly LocalTime templateValueTime;
        private readonly DateTimeZone templateValueZone;
        private readonly IDateTimeZoneProvider zoneProvider;
        private readonly ZoneLocalMappingResolver resolver;

        private static readonly Dictionary<char, CharacterHandler<ZonedDateTime, ZonedDateTimeParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<ZonedDateTime, ZonedDateTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<ZonedDateTime>.DateSeparatorMismatch) },
            { 'T', (pattern, builder) => builder.AddLiteral('T', ParseResult<ZonedDateTime>.MismatchedCharacter) },
            { 'y', DatePatternHelper.CreateYearHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'Y', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<ZonedDateTime, ZonedDateTimeParseBucket>
                        (value => value.Month, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<ZonedDateTime, ZonedDateTimeParseBucket>
                        (value => value.Day, value => value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { '.', TimePatternHelper.CreatePeriodHandler<ZonedDateTime, ZonedDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<ZonedDateTime, ZonedDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<ZonedDateTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 24, value => value.Hour, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<ZonedDateTime, ZonedDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<ZonedDateTime, ZonedDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<ZonedDateTime, ZonedDateTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.Time.AmPm = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.LocalDateTime.Calendar, (bucket, value) => bucket.Date.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.Era, bucket => bucket.Date) },
            { 'z', HandleZone },
            { 'x', HandleZoneAbbreviation },
            { 'o', HandleOffset },
        };

        internal ZonedDateTimePatternParser(ZonedDateTime templateValue, ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider)
        {
            templateValueDate = templateValue.Date;
            templateValueTime = templateValue.TimeOfDay;
            templateValueZone = templateValue.Zone;
            this.resolver = resolver;
            this.zoneProvider = zoneProvider;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<ZonedDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in ZonedDateTimePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            // Handle standard patterns
            if (patternText.Length == 1)
            {
                switch (patternText[0])
                {
                    case 'G':
                        return ZonedDateTimePattern.Patterns.GeneralFormatOnlyPatternImpl
                            .WithZoneProvider(zoneProvider)
                            .WithResolver(resolver);
                    case 'F':
                        return ZonedDateTimePattern.Patterns.ExtendedFormatOnlyPatternImpl
                            .WithZoneProvider(zoneProvider)
                            .WithResolver(resolver);
                    default:
                        throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternText[0], typeof(ZonedDateTime));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>(formatInfo,
                () => new ZonedDateTimeParseBucket(templateValueDate, templateValueTime, templateValueZone, resolver, zoneProvider));
            if (zoneProvider == null)
            {
                patternBuilder.SetFormatOnly();
            }
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            return patternBuilder.Build();
        }

        private static void HandleZone(PatternCursor pattern,
            SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket> builder)
        {
            builder.AddField(PatternFields.Zone, pattern.Current);
            builder.AddParseAction(ParseZone);
            builder.AddFormatAction((value, sb) => sb.Append(value.Zone.Id));
        }

        private static void HandleZoneAbbreviation(PatternCursor pattern,
            SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket> builder)
        {
            builder.AddField(PatternFields.ZoneAbbreviation, pattern.Current);
            builder.SetFormatOnly();
            builder.AddFormatAction((value, sb) => sb.Append(value.GetZoneInterval().Name));
        }

        private static void HandleOffset(PatternCursor pattern,
            SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket> builder)
        {
            builder.AddField(PatternFields.EmbeddedOffset, pattern.Current);
            string embeddedPattern = pattern.GetEmbeddedPattern('<', '>');
            var offsetPattern = OffsetPattern.Create(embeddedPattern, builder.FormatInfo).UnderlyingPattern;
            builder.AddParseAction((value, bucket) =>
                {
                    var result = offsetPattern.ParsePartial(value);
                    if (!result.Success)
                    {
                        return result.ConvertError<ZonedDateTime>();
                    }
                    bucket.Offset = result.Value;
                    return null;
                });
            builder.AddFormatAction((value, sb) => offsetPattern.FormatPartial(value.Offset, sb));
        }

        private static ParseResult<ZonedDateTime> ParseZone(ValueCursor value, ZonedDateTimeParseBucket bucket)
        {
            return bucket.ParseZone(value);
        }

        private sealed class ZonedDateTimeParseBucket : ParseBucket<ZonedDateTime>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;
            private DateTimeZone Zone;
            internal Offset Offset;
            private readonly ZoneLocalMappingResolver resolver;
            private readonly IDateTimeZoneProvider zoneProvider;

            internal ZonedDateTimeParseBucket(LocalDate templateDate, LocalTime templateTime,
                DateTimeZone templateZone, ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider)
            {
                Date = new LocalDatePatternParser.LocalDateParseBucket(templateDate);
                Time = new LocalTimePatternParser.LocalTimeParseBucket(templateTime);
                Zone = templateZone;
                this.resolver = resolver;
                this.zoneProvider = zoneProvider;
            }

            internal ParseResult<ZonedDateTime> ParseZone(ValueCursor value)
            {
                DateTimeZone zone = TryParseFixedZone(value) ?? TryParseProviderZone(value);

                if (zone == null)
                {
                    return ParseResult<ZonedDateTime>.NoMatchingZoneId(value);
                }
                Zone = zone;
                return null;
            }

            /// <summary>
            /// Attempts to parse a fixed time zone from "UTC" with an optional
            /// offset, expressed as +HH, +HH:mm, +HH:mm:ss or +HH:mm:ss.fff - i.e. the
            /// general format. If it manages, it will move the cursor and return the
            /// zone. Otherwise, it will return null and the cursor will remain where
            /// it was.
            /// </summary>
            private DateTimeZone TryParseFixedZone(ValueCursor value)
            {
                if (value.CompareOrdinal(DateTimeZone.UtcId) != 0)
                {
                    return null;
                }
                value.Move(value.Index + 3);
                var pattern = OffsetPattern.GeneralInvariantPattern.UnderlyingPattern;
                var parseResult = pattern.ParsePartial(value);
                return parseResult.Success ? DateTimeZone.ForOffset(parseResult.Value) : DateTimeZone.Utc;
            }
                
            /// <summary>
            /// Tries to parse a time zone ID from the provider. Returns the zone
            /// on success (after moving the cursor to the end of the ID) or null on failure
            /// (leaving the cursor where it was).
            /// </summary>
            private DateTimeZone TryParseProviderZone(ValueCursor value)
            {
                // The IDs from the provider are guaranteed to be in order (using ordinal comparisons).
                // Use a binary search to find a match, then make sure it's the longest possible match.
                var ids = zoneProvider.Ids;
                int lowerBound = 0;         // Inclusive
                int upperBound = ids.Count; // Exclusive
                while (lowerBound < upperBound)
                {
                    int guess = (lowerBound + upperBound) / 2;
                    int result = value.CompareOrdinal(ids[guess]);
                    if (result < 0)
                    {
                        // Guess is later than our text: lower the upper bound
                        upperBound = guess;
                    }
                    else if (result > 0)
                    {
                        // Guess is earlier than our text: raise the lower bound
                        lowerBound = guess + 1;
                    }
                    else
                    {
                        // We've found a match! But it may not be as long as it
                        // could be. Keep looking until we find a value which isn't a match...
                        while (guess + 1 < upperBound && value.CompareOrdinal(ids[guess + 1]) == 0)
                        {
                            guess++;
                        }
                        string id = ids[guess];
                        value.Move(value.Index + id.Length);
                        return zoneProvider[id];
                    }
                }
                return null;
            }

            internal override ParseResult<ZonedDateTime> CalculateValue(PatternFields usedFields, string text)
            {
                var localResult = LocalDateTimePatternParser.LocalDateTimeParseBucket.CombineBuckets(usedFields, Date, Time, text);
                if (!localResult.Success)
                {
                    return localResult.ConvertError<ZonedDateTime>();
                }

                var localDateTime = localResult.Value;

                // No offset - so just use the resolver
                if ((usedFields & PatternFields.EmbeddedOffset) == 0)
                {
                    try
                    {
                        return ParseResult<ZonedDateTime>.ForValue(Zone.ResolveLocal(localDateTime, resolver));
                    }
                    catch (SkippedTimeException)
                    {
                        return ParseResult<ZonedDateTime>.SkippedLocalTime(text);
                    }
                    catch (AmbiguousTimeException)
                    {
                        return ParseResult<ZonedDateTime>.AmbiguousLocalTime(text);
                    }
                }
                
                // We were given an offset, so we can resolve and validate using that
                var mapping = Zone.MapLocal(localDateTime);
                ZonedDateTime result;
                switch (mapping.Count)
                {
                    // If the local time was skipped, the offset has to be invalid.
                    case 0:
                        return ParseResult<ZonedDateTime>.InvalidOffset(text);
                    case 1:
                        result = mapping.First(); // We'll validate in a minute
                        break;
                    case 2:
                        result = mapping.First().Offset == Offset ? mapping.First() : mapping.Last();
                        break;
                    default:
                        throw new InvalidOperationException("Mapping has count outside range 0-2; should not happen.");
                }
                if (result.Offset != Offset)
                {
                    return ParseResult<ZonedDateTime>.InvalidOffset(text);
                }
                return ParseResult<ZonedDateTime>.ForValue(result);
            }
        }
    }
}
