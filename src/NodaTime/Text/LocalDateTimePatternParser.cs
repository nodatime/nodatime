// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System.Collections.Generic;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="LocalDateTime"/> values.
    /// </summary>
    internal sealed class LocalDateTimePatternParser : IPatternParser<LocalDateTime>
    {
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
            { 'T', (pattern, builder) => builder.AddLiteral('T', ParseResult<LocalDateTime>.MismatchedCharacter) },
            { 'y', DatePatternHelper.CreateYearOfEraHandler<LocalDateTime, LocalDateTimeParseBucket>(value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'u', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (4, PatternFields.Year, -9999, 9999, value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<LocalDateTime, LocalDateTimeParseBucket>
                        (value => value.Month, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<LocalDateTime, LocalDateTimeParseBucket>
                        (value => value.Day, value => (int) value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { '.', TimePatternHelper.CreatePeriodHandler<LocalDateTime, LocalDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<LocalDateTime, LocalDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalDateTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 24, value => value.Hour, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<LocalDateTime, LocalDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<LocalDateTime, LocalDateTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<LocalDateTime, LocalDateTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.Time.AmPm = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<LocalDateTime, LocalDateTimeParseBucket>(value => value.Calendar, (bucket, value) => bucket.Date.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<LocalDateTime, LocalDateTimeParseBucket>(value => value.Era, bucket => bucket.Date) },
            { 'l', (cursor, builder) => builder.AddEmbeddedLocalPartial(cursor, bucket => bucket.Date, bucket => bucket.Time, value => value.Date, value => value.TimeOfDay, null) },
        };

        internal LocalDateTimePatternParser(LocalDateTime templateValue)
        {
            templateValueDate = templateValue.Date;
            templateValueTime = templateValue.TimeOfDay;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<LocalDateTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in LocalDateTimePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                return patternText[0] switch
                {
                    // Invariant standard patterns return cached implementations.
                    'o' => LocalDateTimePattern.Patterns.BclRoundtripPatternImpl,
                    'O' => LocalDateTimePattern.Patterns.BclRoundtripPatternImpl,
                    'r' => LocalDateTimePattern.Patterns.FullRoundtripPatternImpl,
                    'R' => LocalDateTimePattern.Patterns.FullRoundtripWithoutCalendarImpl,
                    's' => LocalDateTimePattern.Patterns.GeneralIsoPatternImpl,
                    'S' => LocalDateTimePattern.Patterns.ExtendedIsoPatternImpl,
                    // Other standard patterns expand the pattern text to the appropriate custom pattern.
                    // Note: we don't just recurse, as otherwise a FullDateTimePattern of 'F' would cause a stack overflow.
                    'f' => ParseNoStandardExpansion($"{formatInfo.DateTimeFormat.LongDatePattern} {formatInfo.DateTimeFormat.ShortTimePattern}"),
                    'F' => ParseNoStandardExpansion(formatInfo.DateTimeFormat.FullDateTimePattern),
                    'g' => ParseNoStandardExpansion($"{formatInfo.DateTimeFormat.ShortDatePattern} {formatInfo.DateTimeFormat.ShortTimePattern}"),
                    'G' => ParseNoStandardExpansion($"{formatInfo.DateTimeFormat.ShortDatePattern} {formatInfo.DateTimeFormat.LongTimePattern}"),
                    // Unknown standard patterns fail.
                    _ => throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText, typeof(LocalDateTime))
                };
            }
            return ParseNoStandardExpansion(patternText);

            IPattern<LocalDateTime> ParseNoStandardExpansion(string patternTextLocal)
            {
                var patternBuilder = new SteppedPatternBuilder<LocalDateTime, LocalDateTimeParseBucket>(formatInfo,
                    () => new LocalDateTimeParseBucket(templateValueDate, templateValueTime));
                patternBuilder.ParseCustomPattern(patternTextLocal, PatternCharacterHandlers);
                patternBuilder.ValidateUsedFields();
                return patternBuilder.Build(templateValueDate.At(templateValueTime));
            }
        }


        internal sealed class LocalDateTimeParseBucket : ParseBucket<LocalDateTime>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;

            internal LocalDateTimeParseBucket(LocalDate templateValueDate, LocalTime templateValueTime)
            {
                Date = new LocalDatePatternParser.LocalDateParseBucket(templateValueDate);
                Time = new LocalTimePatternParser.LocalTimeParseBucket(templateValueTime);
            }

            /// <summary>
            /// Combines the values in a date bucket with the values in a time bucket.
            /// </summary>
            /// <remarks>
            /// This would normally be the <see cref="CalculateValue"/> method, but we want
            /// to be able to use the same logic when parsing an <see cref="OffsetDateTime"/>
            /// and <see cref="ZonedDateTime"/>.
            /// </remarks>
            internal static ParseResult<LocalDateTime> CombineBuckets(
                PatternFields usedFields,
                LocalDatePatternParser.LocalDateParseBucket dateBucket,
                LocalTimePatternParser.LocalTimeParseBucket timeBucket,
                string text)
            {
                // Handle special case of hour = 24
                bool hour24 = false;
                if (timeBucket.Hours24 == 24)
                {
                    timeBucket.Hours24 = 0;
                    hour24 = true;
                }

                ParseResult<LocalDate> dateResult = dateBucket.CalculateValue(usedFields & PatternFields.AllDateFields, text, typeof(LocalDateTime));
                if (!dateResult.Success)
                {
                    return dateResult.ConvertError<LocalDateTime>();
                }
                ParseResult<LocalTime> timeResult = timeBucket.CalculateValue(usedFields & PatternFields.AllTimeFields, text, typeof(LocalDateTime));
                if (!timeResult.Success)
                {
                    return timeResult.ConvertError<LocalDateTime>();
                }

                LocalDate date = dateResult.Value;
                LocalTime time = timeResult.Value;

                if (hour24)
                {
                    if (time != LocalTime.Midnight)
                    {
                        return ParseResult<LocalDateTime>.InvalidHour24(text);
                    }
                    date = date.PlusDays(1);
                }
                return ParseResult<LocalDateTime>.ForValue(date + time);
            }

            internal override ParseResult<LocalDateTime> CalculateValue(PatternFields usedFields, string text) =>
                CombineBuckets(usedFields, Date, Time, text);
        }
    }
}
