// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System.Collections.Generic;

namespace NodaTime.Text
{
    internal sealed class OffsetDatePatternParser : IPatternParser<OffsetDate>
    {
        private readonly OffsetDate templateValue;

        private static readonly Dictionary<char, CharacterHandler<OffsetDate, OffsetDateParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<OffsetDate, OffsetDateParseBucket>>
        {
            { '%', SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<OffsetDate>.DateSeparatorMismatch) },
            { 'y', DatePatternHelper.CreateYearOfEraHandler<OffsetDate, OffsetDateParseBucket>(value => value.YearOfEra, (bucket, value) => bucket.Date.YearOfEra = value) },
            { 'u', SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>.HandlePaddedField
                       (4, PatternFields.Year, -9999, 9999, value => value.Year, (bucket, value) => bucket.Date.Year = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<OffsetDate, OffsetDateParseBucket>
                        (value => value.Month, (bucket, value) => bucket.Date.MonthOfYearText = value, (bucket, value) => bucket.Date.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<OffsetDate, OffsetDateParseBucket>
                        (value => value.Day, value => (int) value.DayOfWeek, (bucket, value) => bucket.Date.DayOfMonth = value, (bucket, value) => bucket.Date.DayOfWeek = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<OffsetDate, OffsetDateParseBucket>(value => value.Date.Calendar, (bucket, value) => bucket.Date.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<OffsetDate, OffsetDateParseBucket>(value => value.Era, bucket => bucket.Date) },
            { 'o', HandleOffset },
            { 'l', (cursor, builder) => builder.AddEmbeddedDatePattern(cursor.Current, cursor.GetEmbeddedPattern(), bucket => bucket.Date, value => value.Date) },
        };

        internal OffsetDatePatternParser(OffsetDate templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<OffsetDate> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in OffsetDatePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            // Handle standard patterns
            if (patternText.Length == 1)
            {
                switch (patternText[0])
                {
                    case 'G':
                        return OffsetDatePattern.Patterns.GeneralIsoPatternImpl;
                    case 'r':
                        return OffsetDatePattern.Patterns.FullRoundtripPatternImpl;
                    default:
                        throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText[0], typeof(OffsetDate));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket>(formatInfo, () => new OffsetDateParseBucket(templateValue));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            // Need to reconstruct the template value from the bits...
            return patternBuilder.Build(templateValue);
        }
        
        private static void HandleOffset(PatternCursor pattern,
            SteppedPatternBuilder<OffsetDate, OffsetDateParseBucket> builder)
        {
            builder.AddField(PatternFields.EmbeddedOffset, pattern.Current);
            string embeddedPattern = pattern.GetEmbeddedPattern();
            var offsetPattern = OffsetPattern.Create(embeddedPattern, builder.FormatInfo).UnderlyingPattern;
            builder.AddEmbeddedPattern(offsetPattern, (bucket, offset) => bucket.Offset = offset, zdt => zdt.Offset);
        }

        private sealed class OffsetDateParseBucket : ParseBucket<OffsetDate>
        {
            internal readonly LocalDatePatternParser.LocalDateParseBucket Date;
            internal Offset Offset;

            internal OffsetDateParseBucket(OffsetDate templateValue)
            {
                Date = new LocalDatePatternParser.LocalDateParseBucket(templateValue.Date);
                Offset = templateValue.Offset;
            }

            internal override ParseResult<OffsetDate> CalculateValue(PatternFields usedFields, string text)
            {
                ParseResult<LocalDate> dateResult = Date.CalculateValue(usedFields & PatternFields.AllDateFields, text);
                if (!dateResult.Success)
                {
                    return dateResult.ConvertError<OffsetDate>();
                }
                LocalDate date = dateResult.Value;
                return ParseResult<OffsetDate>.ForValue(date.WithOffset(Offset));
            }
        }
    }
}
