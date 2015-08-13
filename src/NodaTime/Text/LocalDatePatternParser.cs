// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Calendars;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="LocalDate"/> values.
    /// </summary>
    internal sealed class LocalDatePatternParser : IPatternParser<LocalDate>
    {
        private readonly LocalDate templateValue;

        /// <summary>
        /// Maximum two-digit-year in the template to treat as the current century.
        /// (One day we may want to make this configurable, but it feels very low
        /// priority.)
        /// </summary>
        private const int TwoDigitYearMax = 30;

        private static readonly Dictionary<char, CharacterHandler<LocalDate, LocalDateParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<LocalDate, LocalDateParseBucket>>
        {
            { '%', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandleBackslash },
            { '/', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<LocalDate>.DateSeparatorMismatch) },
            { 'y', DatePatternHelper.CreateYearOfEraHandler<LocalDate, LocalDateParseBucket>(value => value.YearOfEra, (bucket, value) => bucket.YearOfEra = value) },
            { 'u', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandlePaddedField
                       (4, PatternFields.Year, -9999, 9999, value => value.Year, (bucket, value) => bucket.Year = value) },
            { 'M', DatePatternHelper.CreateMonthOfYearHandler<LocalDate, LocalDateParseBucket>
                        (value => value.Month, (bucket, value) => bucket.MonthOfYearText = value, (bucket, value) => bucket.MonthOfYearNumeric = value) },
            { 'd', DatePatternHelper.CreateDayHandler<LocalDate, LocalDateParseBucket>
                        (value => value.Day, value => value.DayOfWeek, (bucket, value) => bucket.DayOfMonth = value, (bucket, value) => bucket.DayOfWeek = value) },
            { 'c', DatePatternHelper.CreateCalendarHandler<LocalDate, LocalDateParseBucket>(value => value.Calendar, (bucket, value) => bucket.Calendar = value) },
            { 'g', DatePatternHelper.CreateEraHandler<LocalDate, LocalDateParseBucket>(date => date.Era, bucket => bucket) },
        };

        internal LocalDatePatternParser(LocalDate templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<LocalDate> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in LocalDatePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternCharacter, typeof(LocalDate));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<LocalDate, LocalDateParseBucket>(formatInfo,
                () => new LocalDateParseBucket(templateValue));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            return patternBuilder.Build(templateValue);
        }

        private string ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            switch (patternCharacter)
            {
                case 'd':
                    return formatInfo.DateTimeFormat.ShortDatePattern;
                case 'D':
                    return formatInfo.DateTimeFormat.LongDatePattern;
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }
        
        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by LocalDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class LocalDateParseBucket : ParseBucket<LocalDate>
        {
            private readonly LocalDate templateValue;

            internal CalendarSystem Calendar;
            internal int Year;
            private Era Era;
            internal int YearOfEra;
            internal int MonthOfYearNumeric;
            internal int MonthOfYearText;
            internal int DayOfMonth;
            internal int DayOfWeek;

            internal LocalDateParseBucket(LocalDate templateValue)
            {
                this.templateValue = templateValue;
                // Only fetch this once.
                this.Calendar = templateValue.Calendar;
            }

            internal ParseResult<TResult> ParseEra<TResult>(NodaFormatInfo formatInfo, ValueCursor cursor)
            {
                var compareInfo = formatInfo.CompareInfo;
                foreach (var era in Calendar.Eras)
                {
                    foreach (string eraName in formatInfo.GetEraNames(era))
                    {
                        if (cursor.MatchCaseInsensitive(eraName, compareInfo, true))
                        {
                            Era = era;
                            return null;
                        }
                    }
                }
                return ParseResult<TResult>.MismatchedText(cursor, 'g');
            }

            internal override ParseResult<LocalDate> CalculateValue(PatternFields usedFields, string text)
            {
                // This will set Year if necessary
                ParseResult<LocalDate> failure = DetermineYear(usedFields, text);
                if (failure != null)
                {
                    return failure;
                }
                // This will set MonthOfYearNumeric if necessary
                failure = DetermineMonth(usedFields, text);
                if (failure != null)
                {
                    return failure;
                }

                int day = IsFieldUsed(usedFields, PatternFields.DayOfMonth) ? DayOfMonth : templateValue.Day;
                if (day > Calendar.GetDaysInMonth(Year, MonthOfYearNumeric))
                {
                    return ParseResult<LocalDate>.DayOfMonthOutOfRange(text, day, MonthOfYearNumeric, Year);
                }

                LocalDate value = new LocalDate(Year, MonthOfYearNumeric, day, Calendar);

                if (IsFieldUsed(usedFields, PatternFields.DayOfWeek) && DayOfWeek != value.DayOfWeek)
                {
                    return ParseResult<LocalDate>.InconsistentDayOfWeekTextValue(text);
                }

                // FIXME: If we got an era, check that the resulting date really lies within that era.
                return ParseResult<LocalDate>.ForValue(value);
            }

            /// <summary>
            /// Work out the year, based on fields of:
            /// - Year
            /// - YearOfEra
            /// - YearTwoDigits (implies YearOfEra)
            /// - Era
            /// 
            /// If the year is specified, that trumps everything else - any other fields
            /// are just used for checking.
            /// 
            /// If nothing is specified, the year of the template value is used.
            /// 
            /// If just the era is specified, the year of the template value is used,
            /// and the specified era is checked against it. (Hopefully no-one will
            /// expect to get useful information from a format string with era but no year...)
            /// 
            /// Otherwise, we have the year of era (possibly only two digits) and possibly the
            /// era. If the era isn't specified, take it from the template value.
            /// Finally, if we only have two digits, then use either the century of the template
            /// value or the previous century if the year-of-era is greater than TwoDigitYearMax...
            /// and if the template value isn't in the first century already.
            /// 
            /// Phew.
            /// </summary>
            private ParseResult<LocalDate> DetermineYear(PatternFields usedFields, string text)
            {
                if (IsFieldUsed(usedFields, PatternFields.Year))
                {
                    if (Year > Calendar.MaxYear || Year < Calendar.MinYear)
                    {
                        return ParseResult<LocalDate>.FieldValueOutOfRangePostParse(text, Year, 'u');
                    }

                    if (IsFieldUsed(usedFields, PatternFields.Era) && Era != Calendar.GetEra(Year))
                    {
                        return ParseResult<LocalDate>.InconsistentValues(text, 'g', 'u');
                    }

                    if (IsFieldUsed(usedFields, PatternFields.YearOfEra))
                    {
                        int yearOfEraFromYear = Calendar.GetYearOfEra(Year);
                        if (IsFieldUsed(usedFields, PatternFields.YearTwoDigits))
                        {
                            // We're only checking the last two digits
                            yearOfEraFromYear = yearOfEraFromYear % 100;
                        }
                        if (yearOfEraFromYear != YearOfEra)
                        {
                            return ParseResult<LocalDate>.InconsistentValues(text, 'y', 'u');
                        }
                    }
                    return null;
                }

                // Use the year from the template value, possibly checking the era.
                if (!IsFieldUsed(usedFields, PatternFields.YearOfEra))
                {
                    Year = templateValue.Year;
                    return IsFieldUsed(usedFields, PatternFields.Era) && Era != Calendar.GetEra(Year)
                        ? ParseResult<LocalDate>.InconsistentValues(text, 'g', 'u') : null;
                }

                if (!IsFieldUsed(usedFields, PatternFields.Era))
                {
                    Era = templateValue.Era;
                }

                if (IsFieldUsed(usedFields, PatternFields.YearTwoDigits))
                {
                    int century = templateValue.YearOfEra / 100;
                    if (YearOfEra > TwoDigitYearMax && century > 1)
                    {
                        century--;
                    }
                    YearOfEra += century * 100;
                }

                if (YearOfEra < Calendar.GetMinYearOfEra(Era) ||
                    YearOfEra > Calendar.GetMaxYearOfEra(Era))
                {
                    return ParseResult<LocalDate>.YearOfEraOutOfRange(text, YearOfEra, Era, Calendar);
                }
                Year = Calendar.GetAbsoluteYear(YearOfEra, Era);
                return null;
            }

            private ParseResult<LocalDate> DetermineMonth(PatternFields usedFields, string text)
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
                            return ParseResult<LocalDate>.InconsistentMonthValues(text);
                        }
                        // No need to change MonthOfYearNumeric - this was just a check
                        break;
                    case 0:
                        MonthOfYearNumeric = templateValue.Month;
                        break;
                }
                if (MonthOfYearNumeric > Calendar.GetMonthsInYear(Year))
                {
                    return ParseResult<LocalDate>.MonthOutOfRange(text, MonthOfYearNumeric, Year);
                }
                return null;
            }
        }
    }
}
