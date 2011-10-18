#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="LocalDateTime"/> values.
    /// </summary>
    internal sealed class LocalDateTimePatternParser : IPatternParser<LocalDateTime>
    {
        private static readonly CharacterHandler<LocalDateTime, LocalDateTimeParseBucket> DefaultCharacterHandler =
            SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandleDefaultCharacter;

        // Split the template value into date and time once, to avoid doing it every time we parse.
        private readonly LocalDate templateValueDate;
        private readonly LocalTime templateValueTime;       

        private static readonly Dictionary<char, CharacterHandler<LocalDateTime, LocalDateTimeParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<LocalDateTime, LocalDateTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<LocalDateTime>.DateSeparatorMismatch) },
            { 'y', DatePatternHelper.CreateYearHandler<LocalDateTime, LocalDateTimeParseBucket>(value => value.YearOfCentury, value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'Y', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<LocalDateTime, LocalDateTimeParseBucket>
                        (value => value.MonthOfYear, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<LocalDateTime, LocalDateTimeParseBucket>
                        (value => value.DayOfMonth, value => value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { '.', TimePatternHelper.CreatePeriodHandler<LocalDateTime, LocalDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalDateTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, value => value.HourOfDay, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.MinuteOfHour, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.SecondOfMinute, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<LocalDateTime, LocalDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<LocalDateTime, LocalDateTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<LocalDateTime, LocalDateTimeParseBucket>(time => time.HourOfDay, (bucket, value) => bucket.Time.AmPm = value) }
        };

        // These have to come *after* the above field initializers...
        private static readonly IPattern<LocalDateTime> RoundTripPattern =
            new LocalDateTimePatternParser(LocalDateTimePattern.DefaultTemplateValue).ParsePattern("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff", NodaFormatInfo.InvariantInfo).GetResultOrThrow();
        private static readonly IPattern<LocalDateTime> SortablePattern =
            new LocalDateTimePatternParser(LocalDateTimePattern.DefaultTemplateValue).ParsePattern("yyyy'-'MM'-'dd'T'HH':'mm':'ss", NodaFormatInfo.InvariantInfo).GetResultOrThrow();

        internal LocalDateTimePatternParser(LocalDateTime templateValue)
        {
            templateValueDate = templateValue.Date;
            templateValueTime = templateValue.TimeOfDay;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<LocalDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<LocalDateTime>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<LocalDateTime>.FormatStringEmpty;
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                if (patternCharacter == 'o' || patternCharacter == 'O')
                {
                    return PatternParseResult<LocalDateTime>.ForValue(RoundTripPattern);
                }
                if (patternCharacter == 's')
                {
                    return PatternParseResult<LocalDateTime>.ForValue(SortablePattern);
                }
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    return PatternParseResult<LocalDateTime>.UnknownStandardFormat(patternCharacter);
                }
            }

            var patternBuilder = new SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>(formatInfo, () => new LocalDateTimeParseBucket(templateValueDate, templateValueTime));
            var patternCursor = new PatternCursor(patternText);

            // Prime the pump...
            // TODO: Add this to the builder?
            patternBuilder.AddParseAction((str, bucket) =>
            {
                str.MoveNext();
                return null;
            });

            while (patternCursor.MoveNext())
            {
                CharacterHandler<LocalDateTime, LocalDateTimeParseBucket> handler;
                // The era parser needs access to the calendar, so we need a new handler each time.
                if (patternCursor.Current == 'g')
                {
                    handler = DatePatternHelper.CreateEraHandler<LocalDateTime, LocalDateTimeParseBucket>
                        (templateValueDate.Calendar, value => value.Era, (bucket, value) => bucket.Date.EraIndex = value);
                }
                else
                {
                    if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                    {
                        handler = DefaultCharacterHandler;
                    }                    
                }
                PatternParseResult<LocalDateTime> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            if ((patternBuilder.UsedFields & (PatternFields.Era | PatternFields.YearOfEra)) == PatternFields.Era)
            {
                return PatternParseResult<LocalDateTime>.EraDesignatorWithoutYearOfEra;
            }
            return PatternParseResult<LocalDateTime>.ForValue(patternBuilder.Build());
        }

        private string ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            switch (patternCharacter)
            {
                case 'f':
                    return formatInfo.DateTimeFormat.LongDatePattern + " " + formatInfo.DateTimeFormat.ShortTimePattern;
                case 'F':
                    return formatInfo.DateTimeFormat.FullDateTimePattern;
                case 'g':
                    return formatInfo.DateTimeFormat.ShortDatePattern + " " + formatInfo.DateTimeFormat.ShortTimePattern;
                case 'G':
                    return formatInfo.DateTimeFormat.ShortDatePattern + " " + formatInfo.DateTimeFormat.LongTimePattern;
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }
        
        private sealed class LocalDateTimeParseBucket : ParseBucket<LocalDateTime>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;

            internal LocalDateTimeParseBucket(LocalDate templateValueDate, LocalTime templateValueTime)
            {
                Date = new LocalDatePatternParser.LocalDateParseBucket(templateValueDate);
                Time = new LocalTimePatternParser.LocalTimeParseBucket(templateValueTime);
            }

            internal override ParseResult<LocalDateTime> CalculateValue(PatternFields usedFields)
            {
                ParseResult<LocalDate> dateResult = Date.CalculateValue(usedFields & PatternFields.AllDateFields);
                if (!dateResult.Success)
                {
                    return dateResult.WithResultType<LocalDateTime>();
                }
                ParseResult<LocalTime> timeResult = Time.CalculateValue(usedFields & PatternFields.AllTimeFields);
                if (!timeResult.Success)
                {
                    return timeResult.WithResultType<LocalDateTime>();
                }
                return ParseResult<LocalDateTime>.ForValue(dateResult.Value + timeResult.Value);
            }
        }
    }
}
