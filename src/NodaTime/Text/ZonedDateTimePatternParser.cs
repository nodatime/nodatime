#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using NodaTime.TimeZones;
using System.Collections.Generic;

namespace NodaTime.Text
{
    internal sealed class ZonedDateTimePatternParser
    {
        private static readonly CharacterHandler<ZonedDateTime, ZonedDateTimeParseBucket> DefaultCharacterHandler =
            SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandleDefaultCharacter;

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
            { 'y', DatePatternHelper.CreateYearHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.YearOfCentury, value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'Y', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<ZonedDateTime, ZonedDateTimeParseBucket>
                        (value => value.Month, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<ZonedDateTime, ZonedDateTimeParseBucket>
                        (value => value.Day, value => value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { '.', TimePatternHelper.CreatePeriodHandler<ZonedDateTime, ZonedDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<ZonedDateTime, ZonedDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<ZonedDateTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 24, value => value.Hour, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<ZonedDateTime, ZonedDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<ZonedDateTime, ZonedDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<ZonedDateTime, ZonedDateTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.Time.AmPm = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.LocalDateTime.Calendar, (bucket, value) => bucket.Date.Calendar = value) },
            { 'z', HandleZone }
        };

        internal ZonedDateTimePatternParser(ZonedDateTime templateValue, ZoneLocalMappingResolver resolver, IDateTimeZoneProvider zoneProvider)
        {
            templateValueDate = templateValue.LocalDateTime.Date;
            templateValueTime = templateValue.LocalDateTime.TimeOfDay;
            templateValueZone = templateValue.Zone;
            this.resolver = resolver;
            this.zoneProvider = zoneProvider;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<ZonedDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<ZonedDateTime>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<ZonedDateTime>.FormatStringEmpty;
            }

            // TODO: Standard patterns

            var patternBuilder = new SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>(formatInfo,
                () => new ZonedDateTimeParseBucket(templateValueDate, templateValueTime, templateValueZone, resolver, zoneProvider));
            var patternCursor = new PatternCursor(patternText);

            // Prime the pump...
            // TODO(Post-V1): Add this to the builder?
            patternBuilder.AddParseAction((str, bucket) =>
            {
                str.MoveNext();
                return null;
            });

            while (patternCursor.MoveNext())
            {
                CharacterHandler<ZonedDateTime, ZonedDateTimeParseBucket> handler;
                // The era parser needs access to the calendar, so we need a new handler each time.
                if (patternCursor.Current == 'g')
                {
                    handler = DatePatternHelper.CreateEraHandler<ZonedDateTime, ZonedDateTimeParseBucket>
                        (templateValueDate.Calendar, value => value.Era, (bucket, value) => bucket.Date.EraIndex = value);
                }
                else
                {
                    if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                    {
                        handler = DefaultCharacterHandler;
                    }
                }
                PatternParseResult<ZonedDateTime> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            return patternBuilder.ValidateFieldsBuildPatternParseResult();
        }

        private static PatternParseResult<ZonedDateTime> HandleZone(PatternCursor pattern,
            SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket> builder)
        {
            var failure = builder.AddField(PatternFields.Zone, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            builder.AddParseAction(ParseZone);
            builder.AddFormatAction((value, sb) => sb.Append(value.Zone.Id));
            return null;
        }

        private static ParseResult<ZonedDateTime> ParseZone(ValueCursor value, ZonedDateTimeParseBucket bucket)
        {
            return bucket.ParseZone(value);
        }

        internal sealed class ZonedDateTimeParseBucket : ParseBucket<ZonedDateTime>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;
            internal DateTimeZone Zone;
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
                // TODO: Make this *much* faster, match longest, and match fixed offsets.
                // (Binary search should help on the first two...)
                foreach (string id in zoneProvider.Ids)
                {
                    if (value.Match(id))
                    {
                        Zone = zoneProvider[id];
                    }
                }
                return ParseResult<ZonedDateTime>.NoMatchingZoneId;
            }

            internal override ParseResult<ZonedDateTime> CalculateValue(PatternFields usedFields)
            {
                var localResult = LocalDateTimePatternParser.LocalDateTimeParseBucket.CombineBuckets(usedFields, Date, Time);
                if (!localResult.Success)
                {
                    return localResult.ConvertError<ZonedDateTime>();
                }

                var localDateTime = localResult.Value;

                try
                {
                    return ParseResult<ZonedDateTime>.ForValue(Zone.ResolveLocal(localDateTime, resolver));
                }
                catch (SkippedTimeException)
                {
                    return ParseResult<ZonedDateTime>.SkippedLocalTime;
                }
                catch (AmbiguousTimeException)
                {
                    return ParseResult<ZonedDateTime>.AmbiguousLocalTime;
                }
            }
        }
    }
}
