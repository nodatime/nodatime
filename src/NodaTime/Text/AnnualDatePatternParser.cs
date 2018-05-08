// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System;
using System.Collections.Generic;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="AnnualDate"/> values.
    /// </summary>
    internal sealed class AnnualDatePatternParser : IPatternParser<AnnualDate>
    {
        private readonly AnnualDate templateValue;

        private static readonly Dictionary<char, CharacterHandler<AnnualDate, AnnualDateParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<AnnualDate, AnnualDateParseBucket>>
        {
            { '%', SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<AnnualDate>.DateSeparatorMismatch) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<AnnualDate, AnnualDateParseBucket>
                        (value => value.Month, (bucket, value) => bucket.MonthOfYearText = value, (bucket, value) => bucket.MonthOfYearNumeric = value) },
            { 'd', HandleDayOfMonth },
        };

        internal AnnualDatePatternParser(AnnualDate templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<AnnualDate> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in AnnualDatePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                switch (patternText[0])
                {
                    case 'G':
                        return AnnualDatePattern.Iso;
                    default:
                        throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText[0], typeof(AnnualDate));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket>(formatInfo,
                () => new AnnualDateParseBucket(templateValue));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            return patternBuilder.Build(templateValue);
        }

        private static void HandleDayOfMonth(PatternCursor pattern, SteppedPatternBuilder<AnnualDate, AnnualDateParseBucket> builder)
        {
            int count = pattern.GetRepeatCount(2);
            PatternFields field;
            switch (count)
            {
                case 1:
                case 2:
                    field = PatternFields.DayOfMonth;
                    // Handle real maximum value in the bucket
                    builder.AddParseValueAction(count, 2, pattern.Current, 1, 99, (bucket, value) => bucket.DayOfMonth = value);
                    builder.AddFormatLeftPad(count, value => value.Day, assumeNonNegative: true, assumeFitsInCount: count == 2);
                    break;
                default:
                    throw new InvalidOperationException("Invalid count!");
            }
            builder.AddField(field, pattern.Current);
        }

        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by AnnualDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class AnnualDateParseBucket : ParseBucket<AnnualDate>
        {
            internal readonly AnnualDate TemplateValue;
            internal int MonthOfYearNumeric;
            internal int MonthOfYearText;
            internal int DayOfMonth;

            internal AnnualDateParseBucket(AnnualDate templateValue)
            {
                this.TemplateValue = templateValue;
            }

            internal override ParseResult<AnnualDate> CalculateValue(PatternFields usedFields, string text)
            {
                // This will set MonthOfYearNumeric if necessary
                var failure = DetermineMonth(usedFields, text);
                if (failure != null)
                {
                    return failure;
                }

                int day = usedFields.HasAny(PatternFields.DayOfMonth) ? DayOfMonth : TemplateValue.Day;
                // Validate for the year 2000, just like the AnnualDate constructor does.
                if (day > CalendarSystem.Iso.GetDaysInMonth(2000, MonthOfYearNumeric))
                {
                    return ParseResult<AnnualDate>.DayOfMonthOutOfRangeNoYear(text, day, MonthOfYearNumeric);
                }

                return ParseResult<AnnualDate>.ForValue(new AnnualDate(MonthOfYearNumeric, day));
            }

            private ParseResult<AnnualDate> DetermineMonth(PatternFields usedFields, string text)
            {
                switch (usedFields & (PatternFields.MonthOfYearNumeric | PatternFields.MonthOfYearText))
                {
                    case PatternFields.MonthOfYearNumeric:
                        // No-op
                        break;
                    case PatternFields.MonthOfYearText:
                        MonthOfYearNumeric = MonthOfYearText;
                        break;
                    case PatternFields.MonthOfYearNumeric | PatternFields.MonthOfYearText:
                        if (MonthOfYearNumeric != MonthOfYearText)
                        {
                            return ParseResult<AnnualDate>.InconsistentMonthValues(text);
                        }
                        // No need to change MonthOfYearNumeric - this was just a check
                        break;
                    case 0:
                        MonthOfYearNumeric = TemplateValue.Month;
                        break;
                }
                if (MonthOfYearNumeric > CalendarSystem.Iso.GetMonthsInYear(2000))
                {
                    return ParseResult<AnnualDate>.IsoMonthOutOfRange(text, MonthOfYearNumeric);
                }
                return null;
            }
        }
    }
}
