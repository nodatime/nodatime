// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

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
        /// TODO(Post-V1): Make this configurable, and define its meaning for negative absolute years.
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
            { 'y', DatePatternHelper.CreateYearHandler<LocalDate, LocalDateParseBucket>(value => value.YearOfCentury, value => value.Year, (bucket, value) => bucket.Year = value) },
            { 'Y', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.YearOfEra = value) },
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
            Preconditions.CheckNotNull(patternText, "patternText");
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
            return patternBuilder.Build();
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
            internal int EraIndex;
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
                var compareInfo = formatInfo.CultureInfo.CompareInfo;
                var eras = Calendar.Eras;
                for (int i = 0; i < eras.Count; i++)
                {
                    foreach (string eraName in formatInfo.GetEraNames(eras[i]))
                    {
                        if (cursor.MatchCaseInsensitive(eraName, compareInfo, true))
                        {
                            EraIndex = i;
                            return null;
                        }
                    }
                }
                return ParseResult<TResult>.MismatchedText('g');
            }

            internal override ParseResult<LocalDate> CalculateValue(PatternFields usedFields)
            {
                // This will set Year if necessary
                ParseResult<LocalDate> failure = DetermineYear(usedFields);
                if (failure != null)
                {
                    return failure;
                }
                // This will set MonthOfYearNumeric if necessary
                failure = DetermineMonth(usedFields);
                if (failure != null)
                {
                    return failure;
                }

                int day = IsFieldUsed(usedFields, PatternFields.DayOfMonth) ? DayOfMonth : templateValue.Day;
                if (day > Calendar.GetDaysInMonth(Year, MonthOfYearNumeric))
                {
                    return ParseResult<LocalDate>.DayOfMonthOutOfRange(day, MonthOfYearNumeric, Year);
                }

                LocalDate value = new LocalDate(Year, MonthOfYearNumeric, day, Calendar);

                if (IsFieldUsed(usedFields, PatternFields.DayOfWeek) && DayOfWeek != value.DayOfWeek)
                {
                    return ParseResult<LocalDate>.InconsistentDayOfWeekTextValue;
                }

                return ParseResult<LocalDate>.ForValue(value);
            }

            private ParseResult<LocalDate> DetermineYear(PatternFields usedFields)
            {
                int yearFromEra = 0;
                if (IsFieldUsed(usedFields, PatternFields.YearOfEra))
                {
                    // Odd to have a year-of-era without era, but it's valid...
                    if (!IsFieldUsed(usedFields, PatternFields.Era))
                    {
                        EraIndex = Calendar.Eras.IndexOf(templateValue.Era);
                    }
                    // Find the absolute year from the year-of-era and era
                    if (YearOfEra < Calendar.GetMinYearOfEra(EraIndex) ||
                        YearOfEra > Calendar.GetMaxYearOfEra(EraIndex))
                    {
                        return ParseResult<LocalDate>.YearOfEraOutOfRange(YearOfEra, EraIndex, Calendar);
                    }
                    yearFromEra = Calendar.GetAbsoluteYear(YearOfEra, EraIndex);
                }

                // Note: we can't have YearTwoDigits without Year, hence there are only 6 options here rather than 8.
                switch (usedFields & (PatternFields.Year | PatternFields.YearOfEra | PatternFields.YearTwoDigits))
                {
                    case PatternFields.Year:
                        // Fine, we'll just use the Year value we've been provided
                        break;
                    case PatternFields.Year | PatternFields.YearTwoDigits:
                        Year = GetAbsoluteYearFromTwoDigits(templateValue.Year, Year);
                        break;
                    case PatternFields.YearOfEra:
                        Year = yearFromEra;
                        break;
                    case PatternFields.YearOfEra | PatternFields.Year | PatternFields.YearTwoDigits:
                        // We've been given a year of era, but only a two digit year. The year of era
                        // takes precedence, so we just check that the two digits are correct.
                        // This is a pretty bizarre situation...
                        if ((Math.Abs(yearFromEra) % 100) != Year)
                        {
                            return ParseResult<LocalDate>.InconsistentValues('y', 'Y');
                        }
                        Year = yearFromEra;
                        break;
                    case PatternFields.YearOfEra | PatternFields.Year:
                        if (Year != yearFromEra)
                        {
                            return ParseResult<LocalDate>.InconsistentValues('y', 'Y');
                        }
                        Year = yearFromEra;
                        break;
                    case 0:
                        Year = templateValue.Year;
                        break;
                    // No default: it would be impossible.
                }
                if (Year > Calendar.MaxYear || Year < Calendar.MinYear)
                {
                    // The field can't be YearOfEra, as we've already validated that earlier.
                    return ParseResult<LocalDate>.FieldValueOutOfRange(Year, 'y');
                }
                return null;
            }

            private static int GetAbsoluteYearFromTwoDigits(int absoluteBase, int twoDigits)
            {
                // TODO(Post-V1): Sanity check this. It's one way of defining it...
                if (absoluteBase < 0)
                {
                    return -GetAbsoluteYearFromTwoDigits(Math.Abs(absoluteBase), twoDigits);
                }
                int absoluteBaseCentury = absoluteBase - absoluteBase % 100;
                if (twoDigits > TwoDigitYearMax)
                {
                    absoluteBaseCentury -= 100;
                }
                return absoluteBaseCentury + twoDigits;
            }

            private ParseResult<LocalDate> DetermineMonth(PatternFields usedFields)
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
                            return ParseResult<LocalDate>.InconsistentMonthValues;
                        }
                        // No need to change MonthOfYearNumeric - this was just a check
                        break;
                    case 0:
                        MonthOfYearNumeric = templateValue.Month;
                        break;
                }
                if (MonthOfYearNumeric > Calendar.GetMaxMonth(Year))
                {
                    return ParseResult<LocalDate>.MonthOutOfRange(MonthOfYearNumeric, Year);
                }
                return null;
            }
        }
    }
}
