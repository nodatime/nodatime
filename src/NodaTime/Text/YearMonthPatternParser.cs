// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System.Collections.Generic;
using static NodaTime.Text.LocalDatePatternParser;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="YearMonth"/> values.
    /// </summary>
    internal sealed class YearMonthPatternParser : IPatternParser<YearMonth>
    {
        private readonly YearMonth templateValue;

        private static readonly Dictionary<char, CharacterHandler<YearMonth, YearMonthParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<YearMonth, YearMonthParseBucket>>
        {
            { '%', SteppedPatternBuilder<YearMonth, YearMonthParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<YearMonth, YearMonthParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<YearMonth, YearMonthParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<YearMonth, YearMonthParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<YearMonth>.DateSeparatorMismatch) },
            { 'y', DatePatternHelper.CreateYearOfEraHandler<YearMonth, YearMonthParseBucket>(value => value.YearOfEra, (bucket, value) => bucket.DateBucket.YearOfEra = value) },
            { 'u', SteppedPatternBuilder<YearMonth, YearMonthParseBucket>.HandlePaddedField
                       (4, PatternFields.Year, -9999, 9999, value => value.Year, (bucket, value) => bucket.DateBucket.Year = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<YearMonth, YearMonthParseBucket>
                        (value => value.Month, (bucket, value) => bucket.DateBucket.MonthOfYearText = value, (bucket, value) => bucket.DateBucket.MonthOfYearNumeric = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<YearMonth, YearMonthParseBucket>(value => value.Calendar, (bucket, value) => bucket.DateBucket.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<YearMonth, YearMonthParseBucket>(date => date.Era, bucket => bucket.DateBucket) },
        };

        internal YearMonthPatternParser(YearMonth templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<YearMonth> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in LocalDatePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                return patternText[0] switch
                {
                    // Invariant standard patterns return cached implementations.
                    'g' => YearMonthPattern.Patterns.IsoPatternImpl,
                    // Culture-specific default pattern
                    'G' => ParseNoStandardExpansion(formatInfo.DateTimeFormat.YearMonthPattern),
                    // Unknown standard patterns fail.
                    _ => throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText, typeof(YearMonth))
                };
            }

            return ParseNoStandardExpansion(patternText);

            IPattern<YearMonth> ParseNoStandardExpansion(string patternTextLocal)
            {
                var patternBuilder = new SteppedPatternBuilder<YearMonth, YearMonthParseBucket>(formatInfo,
                    () => new YearMonthParseBucket(templateValue));
                patternBuilder.ParseCustomPattern(patternTextLocal, PatternCharacterHandlers);
                patternBuilder.ValidateUsedFields();
                return patternBuilder.Build(templateValue);
            }
        }

        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation.
        /// Actually just delegates to a LocalDateParseBucket.
        /// </summary>
        internal sealed class YearMonthParseBucket : ParseBucket<YearMonth>
        {
            internal readonly LocalDateParseBucket DateBucket;

            internal YearMonthParseBucket(YearMonth templateValue)
            {
                DateBucket = new LocalDateParseBucket(templateValue.StartDate);
            }

            internal override ParseResult<YearMonth> CalculateValue(PatternFields usedFields, string text)
            {
                var result = DateBucket.CalculateValue(usedFields, text, typeof(YearMonth));
                if (!result.Success)
                {
                    return result.ConvertError<YearMonth>();
                }
                return ParseResult<YearMonth>.ForValue(result.Value.ToYearMonth());
            }
        }
    }
}
