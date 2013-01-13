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
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.TimeZones;
using System.Collections.Generic;
using NodaTime.Utility;

namespace NodaTime.Text
{
    internal sealed class ZonedDateTimePatternParser
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
            { 'g', DatePatternHelper.CreateEraHandler<ZonedDateTime, ZonedDateTimeParseBucket>(value => value.Era, bucket => bucket.Date) },
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
        public IPattern<ZonedDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            // TODO: Standard patterns

            var patternBuilder = new SteppedPatternBuilder<ZonedDateTime, ZonedDateTimeParseBucket>(formatInfo,
                () => new ZonedDateTimeParseBucket(templateValueDate, templateValueTime, templateValueZone, resolver, zoneProvider));
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

            // TODO: Find a better way of handling this than hard-coding... this avoids type initialization concerns though.
            private static readonly int FullPatternLength = "UTC+HH:mm:ss.fff".Length;
            private static readonly int LongPatternLength = "UTC+HH:mm:ss".Length;
            private static readonly int MediumPatternLength = "UTC+HH:mm".Length;
            private static readonly int ShortPatternLength = "UTC+HH".Length;
            private static readonly int NoOffsetLength = "UTC".Length;

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
                    return ParseResult<ZonedDateTime>.NoMatchingZoneId;
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
                if (value.CompareOrdinal("UTC") != 0)
                {
                    return null;
                }
                // This will never return null, given that we know it starts with UTC.
                return TryParseFixedZone(value, FullPatternLength)
                    ?? TryParseFixedZone(value, LongPatternLength)
                    ?? TryParseFixedZone(value, MediumPatternLength)
                    ?? TryParseFixedZone(value, ShortPatternLength)
                    ?? TryParseFixedZone(value, NoOffsetLength);
            }

            /// <summary>
            /// Attempts to parse the first "length" characters of value as a complete
            /// fixed time zone ID.
            /// </summary>
            private DateTimeZone TryParseFixedZone(ValueCursor value, int length)
            {
                if (value.Length - value.Index < length)
                {
                    return null;
                }
                string maybeFullText = value.Value.Substring(value.Index, length);
                DateTimeZone zone = FixedDateTimeZone.GetFixedZoneOrNull(maybeFullText);
                if (zone != null)
                {
                    value.Move(value.Index + length);
                }
                return zone;
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
