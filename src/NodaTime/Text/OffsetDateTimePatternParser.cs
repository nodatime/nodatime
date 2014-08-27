// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    internal sealed class OffsetDateTimePatternParser : IPatternParser<OffsetDateTime>
    {
        // Split the template value once, to avoid doing it every time we parse.
        private readonly LocalDate templateValueDate;
        private readonly LocalTime templateValueTime;
        private readonly Offset templateValueOffset;

        private static readonly Dictionary<char, CharacterHandler<OffsetDateTime, OffsetDateTimeParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<OffsetDateTime, OffsetDateTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<OffsetDateTime>.DateSeparatorMismatch) },
            { 'T', (pattern, builder) => builder.AddLiteral('T', ParseResult<OffsetDateTime>.MismatchedCharacter) },
            { 'y', DatePatternHelper.CreateYearHandler<OffsetDateTime, OffsetDateTimeParseBucket>(value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'Y', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<OffsetDateTime, OffsetDateTimeParseBucket>
                        (value => value.Month, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<OffsetDateTime, OffsetDateTimeParseBucket>
                        (value => value.Day, value => value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { '.', TimePatternHelper.CreatePeriodHandler<OffsetDateTime, OffsetDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<OffsetDateTime, OffsetDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<OffsetDateTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 24, value => value.Hour, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<OffsetDateTime, OffsetDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<OffsetDateTime, OffsetDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<OffsetDateTime, OffsetDateTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.Time.AmPm = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<OffsetDateTime, OffsetDateTimeParseBucket>(value => value.LocalDateTime.Calendar, (bucket, value) => bucket.Date.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<OffsetDateTime, OffsetDateTimeParseBucket>(value => value.Era, bucket => bucket.Date) },
            { 'o', HandleOffset },
        };

        internal OffsetDateTimePatternParser(OffsetDateTime templateValue)
        {
            templateValueDate = templateValue.Date;
            templateValueTime = templateValue.TimeOfDay;
            templateValueOffset = templateValue.Offset;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<OffsetDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in OffsetDateTimePattern.
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
                        return OffsetDateTimePattern.Patterns.GeneralIsoPatternImpl;
                    case 'o':
                        return OffsetDateTimePattern.Patterns.ExtendedIsoPatternImpl;
                    case 'r':
                        return OffsetDateTimePattern.Patterns.FullRoundtripPatternImpl;
                    default:
                        throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternText[0], typeof(OffsetDateTime));
                }
            }


            var patternBuilder = new SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket>(formatInfo,
                () => new OffsetDateTimeParseBucket(templateValueDate, templateValueTime, templateValueOffset));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            return patternBuilder.Build();
        }
        
        private static void HandleOffset(PatternCursor pattern,
            SteppedPatternBuilder<OffsetDateTime, OffsetDateTimeParseBucket> builder)
        {
            builder.AddField(PatternFields.EmbeddedOffset, pattern.Current);
            string embeddedPattern = pattern.GetEmbeddedPattern('<', '>');
            var offsetPattern = OffsetPattern.Create(embeddedPattern, builder.FormatInfo).UnderlyingPattern;
            builder.AddParseAction((value, bucket) =>
                {
                    var result = offsetPattern.ParsePartial(value);
                    if (!result.Success)
                    {
                        return result.ConvertError<OffsetDateTime>();
                    }
                    bucket.Offset = result.Value;
                    return null;
                });
            builder.AddFormatAction((value, sb) => offsetPattern.AppendFormat(value.Offset, sb));
        }

        private sealed class OffsetDateTimeParseBucket : ParseBucket<OffsetDateTime>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;
            internal Offset Offset;

            internal OffsetDateTimeParseBucket(LocalDate templateDate, LocalTime templateTime, Offset templateOffset)
            {
                Date = new LocalDatePatternParser.LocalDateParseBucket(templateDate);
                Time = new LocalTimePatternParser.LocalTimeParseBucket(templateTime);
                Offset = templateOffset;
            }

            internal override ParseResult<OffsetDateTime> CalculateValue(PatternFields usedFields, string text)
            {
                var localResult = LocalDateTimePatternParser.LocalDateTimeParseBucket.CombineBuckets(usedFields, Date, Time, text);
                if (!localResult.Success)
                {
                    return localResult.ConvertError<OffsetDateTime>();
                }

                var localDateTime = localResult.Value;
                return ParseResult<OffsetDateTime>.ForValue(localDateTime.WithOffset(Offset));
            }
        }
    }
}
