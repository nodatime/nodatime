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
        private readonly LocalDate templateValue;
        private delegate PatternParseResult<LocalDate> CharacterHandler(PatternCursor patternCursor, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> patternBuilder);

        /// <summary>
        /// Year to treat as the maximum two digit year to go to "the previous year" from the one specified in the template value.
        /// TBD: Define this more appropriately...
        /// </summary>
        private readonly int twoDigitPivotYear = 60;

        private static readonly Dictionary<char, CharacterHandler> PatternCharacterHandlers = new Dictionary<char, CharacterHandler>()
        {
            // TODO: Put these first four into SteppedPatternBuilder for sure...
            { '%', HandlePercent },
            { '\'', HandleQuote },
            { '\"', HandleQuote },
            { '\\', HandleBackslash },
            { '/', HandleSlash }, //(pattern, builder) => builder.AddLiteral(builder.FormatInfo.DateSeparator, ParseResult<LocalDate>.DateSeparatorMismatch) },
            { 'y', HandleYearSpecifier },
            { 'Y', HandleYearOfEraSpecifier },
            { 'M', HandleMonthOfYearSpecifier },
            { 'd', HandleDaySpecifier },
            { 'g', HandleEraSpecifier },
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
                CharacterHandler handler;
                if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                {
                    handler = HandleDefaultCharacter;
                }
                PatternParseResult<LocalDate> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
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
        // TODO: Move a bunch of these into SteppedPatternBuilder.

        /// <summary>
        /// Handle a leading "%" which acts as a pseudo-escape - it's mostly used to allow format strings such as "%H" to mean
        /// "use a custom format string consisting of H instead of a standard pattern H".
        /// </summary>
        private static PatternParseResult<LocalDate> HandlePercent(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            if (pattern.HasMoreCharacters)
            {
                if (pattern.PeekNext() != '%')
                {
                    // Handle the next character as normal
                    return null;
                }
                return PatternParseResult<LocalDate>.PercentDoubled;
            }
            return PatternParseResult<LocalDate>.PercentAtEndOfString;
        }

        private static PatternParseResult<LocalDate> HandleQuote(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            PatternParseResult<LocalDate> failure = null;
            string quoted = pattern.GetQuotedString(pattern.Current, ref failure);
            if (failure != null)
            {
                return failure;
            }
            return builder.AddLiteral(quoted, ParseResult<LocalDate>.QuotedStringMismatch);
        }

        private static PatternParseResult<LocalDate> HandleBackslash(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            if (!pattern.MoveNext())
            {
                return PatternParseResult<LocalDate>.EscapeAtEndOfString;
            }
            builder.AddLiteral(pattern.Current, ParseResult<LocalDate>.EscapedCharacterMismatch);
            return null;
        }

        private static PatternParseResult<LocalDate> HandleSlash(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            string timeSeparator = builder.FormatInfo.DateSeparator;
            builder.AddParseAction((str, bucket) => str.Match(timeSeparator) ? null : ParseResult<LocalDate>.TimeSeparatorMismatch);
            builder.AddFormatAction((localDate, sb) => sb.Append(timeSeparator));
            return null;
        }

        private static PatternParseResult<LocalDate> HandleYearSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            // TODO: Handle parsing negative years.
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
                    builder.AddParseValueAction(count, 2, 'y', 0, 99, (bucket, value) => bucket.TwoDigitYear = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.YearOfCentury, count, sb));
                    break;
                default:
                    // Maximum value will be determined later
                    builder.AddParseValueAction(count, 5, 'y', 0, 99999, (bucket, value) => bucket.Year = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.Year, count, sb));
                    break;
            }
            return null;
        }

        private static PatternParseResult<LocalDate> HandleYearOfEraSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            // TODO: Work out whether we need special handling for Y and YY
            PatternParseResult<LocalDate> failure = null;
            int count = pattern.GetRepeatCount(5, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.YearOfEra, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            // Maximum value will be determined later
            builder.AddParseValueAction(count, 5, 'Y', 0, 99999, (bucket, value) => bucket.YearOfEra = value);
            builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.Year, count, sb));
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
                    throw new NotImplementedException("Need to handle text versions!");
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
                    builder.AddParseValueAction(count, 2, pattern.Current, 0, 99, (bucket, value) => bucket.DayOfMonth = value);
                    builder.AddFormatAction((localDate, sb) => FormatHelper.LeftPad(localDate.DayOfMonth, count, sb));
                    break;
                case 3:
                case 4:
                    field = PatternFields.DayOfWeek;
                    throw new NotImplementedException("Need to handle text versions!");
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

        private static PatternParseResult<LocalDate> HandleEraSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            PatternParseResult<LocalDate> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Era, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            throw new NotImplementedException("Need to handle text versions!");
        }

        private static PatternParseResult<LocalDate> HandleDefaultCharacter(PatternCursor pattern, SteppedPatternBuilder<LocalDate, LocalDateParseBucket> builder)
        {
            return builder.AddLiteral(pattern.Current, ParseResult<LocalDate>.MismatchedCharacter);
        }
        #endregion
        private sealed class LocalDateParseBucket : ParseBucket<LocalDate>
        {
            private readonly LocalDate templateValue;

            internal int TwoDigitYear;
            internal int Year;
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
                throw new System.NotImplementedException();
            }
        }
    }
}
