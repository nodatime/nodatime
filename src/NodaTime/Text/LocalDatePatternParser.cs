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
using System.Text;
using NodaTime.Globalization;
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
        /// TODO: Make this configurable, and define its meaning for negative absolute years.
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
            { 'y', HandleYearSpecifier },
            { 'Y', SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.HandlePaddedField
                       (5, PatternFields.YearOfEra, 0, 99999, value => value.YearOfEra, (bucket, value) => bucket.YearOfEra = value) },
            { 'M', HandleMonthOfYearSpecifier },
            { 'd', HandleDaySpecifier },
        };

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
            // TODO: Add this to the builder?
            patternBuilder.AddParseAction((str, bucket) =>
            {
                str.MoveNext();
                return null;
            });

            while (patternCursor.MoveNext())
            {
                CharacterHandler<LocalDate, LocalDateParseBucket> handler;
                // The era parser needs access to the calendar, so has to be an instance method.
                if (patternCursor.Current == 'g')
                {
                    handler = HandleEraSpecifier;
                }
                else
                {
                    if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                    {
                        handler = HandleDefaultCharacter;
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

        #region Character handlers
        private static PatternParseResult<LocalDate> HandleYearSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            // TODO: Handle parsing and formatting negative years.
            PatternParseResult<LocalDate> failure = null;
            int count = pattern.GetRepeatCount(5, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Year, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            switch (count)
            {
                case 1:
                case 2:
                    builder.AddParseValueAction(count, 2, 'y', 0, 99, (bucket, value) => bucket.Year = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.YearOfCentury, count, sb));
                    // Just remember that we've set this particular field. We can't set it twice as we've already got the Year flag set.
                    builder.AddField(PatternFields.YearTwoDigits, pattern.Current);
                    break;
                case 3:
                    // Maximum value will be determined later.
                    // Three or more digits (ick).
                    builder.AddParseValueAction(count, 5, 'y', 0, 99999, (bucket, value) => bucket.Year = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.Year, count, sb));
                    break;
                default:
                    // Maximum value will be determined later.
                    // Note that the *exact* number of digits are required; not just "at least count".
                    builder.AddParseValueAction(count, count, 'y', 0, 99999, (bucket, value) => bucket.Year = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.Year, count, sb));
                    break;
            }
            return null;
        }

        private static PatternParseResult<LocalDate> HandleMonthOfYearSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            PatternParseResult<LocalDate> failure = null;
            int count = pattern.GetRepeatCount(4, ref failure);
            if (failure != null)
            {
                return failure;
            }
            PatternFields field;
            switch (count)
            {
                case 1:
                case 2:
                    field = PatternFields.MonthOfYearNumeric;
                    // Handle real maximum value in the bucket
                    builder.AddParseValueAction(count, 2, pattern.Current, 0, 99, (bucket, value) => bucket.MonthOfYearNumeric = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.MonthOfYear, count, sb));
                    break;
                case 3:
                case 4:
                    field = PatternFields.MonthOfYearText;
                    var format = builder.FormatInfo;
                    IList<string> nonGenitiveTextValues = count == 3 ? format.ShortMonthNames : format.LongMonthNames;
                    IList<string> genitiveTextValues = count == 3 ? format.ShortMonthGenitiveNames : format.LongMonthGenitiveNames;
                    if (nonGenitiveTextValues == genitiveTextValues)
                    {
                        builder.AddParseTextAction(pattern.Current, (bucket, value) => bucket.MonthOfYearText = value,
                            format.CultureInfo.CompareInfo, nonGenitiveTextValues);
                    }
                    else
                    {
                        // TODO: Make this more robust. The genitive text values come first here because in some cultures they
                        // are longer than the non-genitive forms - we want to match the longest substring. (We're not doing any backtracking...)
                        // This will fail to do the right thing if we get into the opposite situation.
                        builder.AddParseTextAction(pattern.Current, (bucket, value) => bucket.MonthOfYearText = value,
                            format.CultureInfo.CompareInfo, genitiveTextValues, nonGenitiveTextValues);
                    }

                    // Hack: see below
                    builder.AddFormatAction(new MonthFormatActionHolder(format, count).DummyMethod);
                    break;
                default:
                    throw new InvalidOperationException("Invalid count!");
            }
            failure = builder.AddField(field, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            return null;
        }

        // Hacky way of building an action which depends on the final set of pattern fields to determine whether to format a month
        // using the genitive form or not.
        private class MonthFormatActionHolder : SteppedPatternBuilder<LocalDate, LocalDateParseBucket>.IPostPatternParseFormatAction
        {
            private readonly int count;
            private readonly NodaFormatInfo formatInfo;

            internal MonthFormatActionHolder(NodaFormatInfo formatInfo, int count)
            {
                this.count = count;
                this.formatInfo = formatInfo;
            }

            internal void DummyMethod(LocalDate value, StringBuilder builder)
            {
                throw new InvalidOperationException("This method should never be called");
            }

            public NodaAction<LocalDate, StringBuilder> BuildFormatAction(PatternFields finalFields)
            {
                bool genitive = (finalFields & PatternFields.DayOfMonth) != 0;
                IList<string> textValues = count == 3
                    ? (genitive ? formatInfo.ShortMonthGenitiveNames : formatInfo.ShortMonthNames)
                    : (genitive ? formatInfo.LongMonthGenitiveNames : formatInfo.LongMonthNames);
                return (value, sb) => sb.Append(textValues[value.MonthOfYear]);
            }
        }

        private static PatternParseResult<LocalDate> HandleDaySpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            PatternParseResult<LocalDate> failure = null;
            int count = pattern.GetRepeatCount(4, ref failure);
            if (failure != null)
            {
                return failure;
            }
            PatternFields field;
            switch (count)
            {
                case 1:
                case 2:
                    field = PatternFields.DayOfMonth;
                    // Handle real maximum value in the bucket
                    builder.AddParseValueAction(count, 2, pattern.Current, 1, 99, (bucket, value) => bucket.DayOfMonth = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.DayOfMonth, count, sb));
                    break;
                case 3:
                case 4:
                    field = PatternFields.DayOfWeek;
                    var format = builder.FormatInfo;
                    IList<string> textValues = count == 3 ? format.ShortDayNames : format.LongDayNames;
                    builder.AddParseTextAction(pattern.Current, (bucket, value) => bucket.DayOfWeek = value, format.CultureInfo.CompareInfo, textValues);
                    builder.AddFormatAction((value, sb) => sb.Append(textValues[value.DayOfWeek]));
                    break;
                default:
                    throw new InvalidOperationException("Invalid count!");
            }
            failure = builder.AddField(field, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            return null;
        }

        private PatternParseResult<LocalDate> HandleEraSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            PatternParseResult<LocalDate> failure = null;
            pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Era, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            var formatInfo = builder.FormatInfo;
            var calendar = templateValue.Calendar;
            // Note: currently the count is ignored. More work needed to determine whether abbreviated era names should be used for just "g".

            builder.AddParseAction((cursor, bucket) => {
                var compareInfo = formatInfo.CultureInfo.CompareInfo;
                var eras = calendar.Eras;
                for (int i = 0; i < eras.Count; i++)
                {
                    foreach (string eraName in formatInfo.GetEraNames(eras[i]))
                    {
                        if (cursor.MatchCaseInsensitive(eraName, compareInfo))
                        {
                            bucket.EraIndex = i;
                            return null;
                        }
                    }
                }
                return ParseResult<LocalDate>.MismatchedText('g');
            });
            builder.AddFormatAction((value, sb) => sb.Append(formatInfo.GetEraPrimaryName(value.Era)));
            return null;
        }

        private static PatternParseResult<LocalDate> HandleDefaultCharacter(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            return builder.AddLiteral(pattern.Current, ParseResult<LocalDate>.MismatchedCharacter);
        }
        #endregion
        
        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by LocalDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class LocalDateParseBucket : ParseBucket<LocalDate>
        {
            private readonly LocalDate templateValue;

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

                int day = IsFieldUsed(usedFields, PatternFields.DayOfMonth) ? DayOfMonth : templateValue.DayOfMonth;
                if (day > templateValue.Calendar.GetDaysInMonth(Year, MonthOfYearNumeric))
                {
                    return ParseResult<LocalDate>.DayOfMonthOutOfRange(day, MonthOfYearNumeric, Year);
                }

                LocalDate value = new LocalDate(Year, MonthOfYearNumeric, day, templateValue.Calendar);

                if (IsFieldUsed(usedFields, PatternFields.DayOfWeek) && DayOfWeek != value.DayOfWeek)
                {
                    return ParseResult<LocalDate>.InconsistentDayOfWeekTextValue;
                }

                return ParseResult<LocalDate>.ForValue(value);
            }

            private ParseResult<LocalDate> DetermineYear(PatternFields usedFields)
            {
                CalendarSystem calendar = templateValue.Calendar;
                int yearFromEra = 0;
                if (IsFieldUsed(usedFields, PatternFields.YearOfEra))
                {
                    // Odd to have a year-of-era without era, but it's valid...
                    if (!IsFieldUsed(usedFields, PatternFields.Era))
                    {
                        EraIndex = calendar.Eras.IndexOf(templateValue.Era);
                    }
                    // Find the absolute year from the year-of-era and era
                    if (YearOfEra < calendar.GetMinYearOfEra(EraIndex) ||
                        YearOfEra > calendar.GetMaxYearOfEra(EraIndex))
                    {
                        return ParseResult<LocalDate>.YearOfEraOutOfRange(YearOfEra, EraIndex, calendar);
                    }
                    yearFromEra = calendar.GetAbsoluteYear(YearOfEra, EraIndex);
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
                // TODO: Wrong field if we happen to have been given the year of era instead...
                // Pretty insignificant problem, mind you...
                if (Year > templateValue.Calendar.MaxYear || Year < templateValue.Calendar.MinYear)
                {
                    return ParseResult<LocalDate>.FieldValueOutOfRange(Year, 'y');
                }
                return null;
            }

            private static int GetAbsoluteYearFromTwoDigits(int absoluteBase, int twoDigits)
            {
                // TODO: Sanity check this. It's one way of defining it...
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
                        MonthOfYearNumeric = templateValue.MonthOfYear;
                        break;
                }
                if (MonthOfYearNumeric > templateValue.Calendar.GetMaxMonth(Year))
                {
                    return ParseResult<LocalDate>.MonthOutOfRange(MonthOfYearNumeric, Year);
                }
                return null;
            }
        }
    }
}
