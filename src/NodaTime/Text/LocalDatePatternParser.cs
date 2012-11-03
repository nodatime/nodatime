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

using System;
using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// Parser for patterns of <see cref="LocalDate"/> values.
    /// </summary>
    internal sealed class LocalDatePatternParser : IPatternParser<LocalDate>
    {
        private static readonly CharacterHandler<LocalDate, LocalDateParseBucket> DefaultCharacterHandler = SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandleDefaultCharacter;

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
            { 'c', DatePatternHelper.CreateCalendarHandler<LocalDate, LocalDateParseBucket>(value => value.Calendar, (bucket, value) => bucket.Calendar = value) },        };

        internal LocalDatePatternParser(LocalDate templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<LocalDate> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<LocalDate>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<LocalDate>.FormatStringEmpty;
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    return PatternParseResult<LocalDate>.UnknownStandardFormat(patternCharacter);
                }
            }

            var patternBuilder = new SteppedPatternBuilder<LocalDate, LocalDateParseBucket>(formatInfo, () => new LocalDateParseBucket(templateValue));
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
                CharacterHandler<LocalDate, LocalDateParseBucket> handler;
                // The era parser needs access to the calendar, so we need a new handler each time.
                if (patternCursor.Current == 'g')
                {
                    handler = DatePatternHelper.CreateEraHandler<LocalDate, LocalDateParseBucket>(templateValue.Calendar, value => value.Era, (bucket, value) => bucket.EraIndex = value);
                }
                else
                {
                    if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                    {
                        handler = DefaultCharacterHandler;
                    }                    
                }
                PatternParseResult<LocalDate> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            if ((patternBuilder.UsedFields & (PatternFields.Era | PatternFields.YearOfEra)) == PatternFields.Era)
            {
                return PatternParseResult<LocalDate>.EraDesignatorWithoutYearOfEra;
            }
            return PatternParseResult<LocalDate>.ForValue(patternBuilder.Build());
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
                // TODO(Post-V1): Wrong field if we happen to have been given the year of era instead...
                // Pretty insignificant problem, mind you...
                // (The error is reported in the right circumstances - it's just that it will refer to 'y' regardless.)
                if (Year > Calendar.MaxYear || Year < Calendar.MinYear)
                {
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
