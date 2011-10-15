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
    /// Pattern parser for <see cref="LocalTime"/> values.
    /// </summary>
    internal sealed class LocalTimePatternParser : IPatternParser<LocalTime>
    {
        private readonly LocalTime templateValue;

        internal const int FractionOfSecondLength = 7;

        private static readonly Dictionary<char, CharacterHandler<LocalTime, LocalTimeParseBucket>> PatternCharacterHandlers = 
            new Dictionary<char, CharacterHandler<LocalTime, LocalTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleBackslash },
            { '.', HandlePeriod },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Hours12 = value) },
            { 'H', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, value => value.HourOfDay, (bucket, value) => bucket.Hours24 = value) },
            { 'm', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.MinuteOfHour, (bucket, value) => bucket.Minutes = value) },
            { 's', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.SecondOfMinute, (bucket, value) => bucket.Seconds = value) },
            { 'f', HandleFractionSpecifier },
            { 'F', HandleFractionSpecifier },
            { 't', HandleAmPmDesignator }
        };

        public LocalTimePatternParser(LocalTime templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<LocalTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<LocalTime>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<LocalTime>.FormatStringEmpty;
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    return PatternParseResult<LocalTime>.UnknownStandardFormat(patternCharacter);
                }
            }

            var patternBuilder = new SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>(formatInfo, () => new LocalTimeParseBucket(templateValue));
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
                CharacterHandler<LocalTime, LocalTimeParseBucket> handler;
                if (!PatternCharacterHandlers.TryGetValue(patternCursor.Current, out handler))
                {
                    handler = HandleDefaultCharacter;
                }
                PatternParseResult<LocalTime> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            return PatternParseResult<LocalTime>.ForValue(patternBuilder.Build());
        }

        private string ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            switch (patternCharacter)
            {
                case 't':
                    return formatInfo.DateTimeFormat.ShortTimePattern;
                case 'T':
                    return formatInfo.DateTimeFormat.LongTimePattern;
                case 'r':
                    return "HH:mm:ss.FFFFFFF";
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }

        #region Character handlers
        private static PatternParseResult<LocalTime> HandlePeriod(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            // Note: Deliberately *not* using the decimal separator of the culture - see issue 21.

            // If the next part of the pattern is an F, then this decimal separator is effectively optional.
            // At parse time, we need to check whether we've matched the decimal separator. If we have, match the fractional
            // seconds part as normal. Otherwise, we continue on to the next parsing token.
            // At format time, we should always append the decimal separator, and then append using PadRightTruncate.
            if (pattern.PeekNext() == 'F')
            {
                PatternParseResult<LocalTime> failure = null;
                pattern.MoveNext();
                int count = pattern.GetRepeatCount(FractionOfSecondLength, ref failure);
                if (failure != null)
                {
                    return failure;
                }
                failure = builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
                builder.AddParseAction((valueCursor, bucket) =>
                {
                    // If the next token isn't the decimal separator, we assume it's part of the next token in the pattern
                    if (!valueCursor.Match('.'))
                    {
                        return null;
                    }

                    // If there *was* a decimal separator, we should definitely have a number.
                    int fractionalSeconds;
                    // Last argument is false because we don't need *all* the digits to be present
                    if (!valueCursor.ParseFraction(count, FractionOfSecondLength, out fractionalSeconds, false))
                    {
                        return ParseResult<LocalTime>.MismatchedNumber(new string('F', count));
                    }
                    // No need to validate the value - we've got one to three digits, so the range 0-999 is guaranteed.
                    bucket.FractionalSeconds = fractionalSeconds;
                    return null;
                });
                builder.AddFormatAction((localTime, sb) => sb.Append('.'));
                builder.AddFormatRightPadTruncate(count, FractionOfSecondLength, localTime => localTime.TickOfSecond);
                return null;
            }
            else
            {
                return builder.AddLiteral('.', ParseResult<LocalTime>.MismatchedCharacter);
            }
        }

        private static PatternParseResult<LocalTime> HandleFractionSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            char patternCharacter = pattern.Current;
            int count = pattern.GetRepeatCount(FractionOfSecondLength, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddParseAction((str, bucket) =>
            {
                int fractionalSeconds;
                // If the pattern is 'f', we need exactly "count" digits. Otherwise ('F') we need
                // "up to count" digits.
                if (!str.ParseFraction(count, FractionOfSecondLength, out fractionalSeconds, patternCharacter == 'f'))
                {
                    return ParseResult<LocalTime>.MismatchedNumber(new string(patternCharacter, count));
                }
                // No need to validate the value - we've got one to three digits, so the range 0-999 is guaranteed.
                bucket.FractionalSeconds = fractionalSeconds;
                return null;
            });
            return patternCharacter == 'f' ? builder.AddFormatRightPad(count, FractionOfSecondLength, localTime => localTime.TickOfSecond)
                                           : builder.AddFormatRightPadTruncate(count, FractionOfSecondLength, localTime => localTime.TickOfSecond);
        }

        private static PatternParseResult<LocalTime> HandleAmPmDesignator(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.AmPm, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            string amDesignator = builder.FormatInfo.AMDesignator;
            string pmDesignator = builder.FormatInfo.PMDesignator;
            // TODO: Work out if the single character designator should also consume the full designator if it's present.
            // Single character designator
            if (count == 1)
            {
                builder.AddParseAction((str, bucket) =>
                {
                    if (str.Match(amDesignator[0]))
                    {
                        bucket.AmPm = 0;
                        return null;
                    }
                    if (str.Match(pmDesignator[0]))
                    {
                        bucket.AmPm = 1;
                        return null;
                    }
                    return ParseResult<LocalTime>.MissingAmPmDesignator;
                });
                builder.AddFormatAction((localTime, sb) => sb.Append(localTime.HourOfDay > 11 ? pmDesignator[0] : amDesignator[0]));
                return null;
            }
            // Full designator
            builder.AddParseAction((str, bucket) =>
            {
                if (str.Match(amDesignator))
                {
                    bucket.AmPm = 0;
                    return null;
                }
                if (str.Match(pmDesignator))
                {
                    bucket.AmPm = 1;
                    return null;
                }
                return ParseResult<LocalTime>.MissingAmPmDesignator;
            });
            builder.AddFormatAction((localTime, sb) => sb.Append(localTime.HourOfDay > 11 ? pmDesignator : amDesignator));
            return null;
        }

        private static PatternParseResult<LocalTime> HandleDefaultCharacter(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            return builder.AddLiteral(pattern.Current, ParseResult<LocalTime>.MismatchedCharacter);
        }
        #endregion

        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by LocalDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class LocalTimeParseBucket : ParseBucket<LocalTime>
        {
            private readonly LocalTime templateValue;

            /// <summary>
            /// The fractions of a second in ticks, in the range [0, 9999999]
            /// </summary>
            internal int FractionalSeconds;

            /// <summary>
            /// The hours in the range [0, 23].
            /// </summary>
            internal int Hours24;

            /// <summary>
            /// The hours in the range [1, 12].
            /// </summary>
            internal int Hours12;

            /// <summary>
            /// The minutes in the range [0, 59].
            /// </summary>
            internal int Minutes;

            /// <summary>
            /// The seconds in the range [0, 59].
            /// </summary>
            internal int Seconds;

            /// <summary>
            /// AM (0) or PM (1).
            /// </summary>
            internal int AmPm;

            internal LocalTimeParseBucket(LocalTime templateValue)
            {
                this.templateValue = templateValue;
            }

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>            
            internal override ParseResult<LocalTime> CalculateValue(PatternFields usedFields)
            {
                int hour;
                ParseResult<LocalTime> failure = DetermineHour(usedFields, out hour);
                if (failure != null)
                {
                    return failure;
                }
                int minutes = IsFieldUsed(usedFields, PatternFields.Minutes) ? Minutes : templateValue.MinuteOfHour;
                int seconds = IsFieldUsed(usedFields, PatternFields.Seconds) ? Seconds : templateValue.SecondOfMinute;
                int fraction = IsFieldUsed(usedFields, PatternFields.FractionalSeconds) ? FractionalSeconds : templateValue.TickOfSecond;

                return ParseResult<LocalTime>.ForValue(LocalTime.FromHourMinuteSecondTick(hour, minutes, seconds, fraction));
            }

            private ParseResult<LocalTime> DetermineHour(PatternFields usedFields, out int hour)
            {
                hour = 0;
                if (IsFieldUsed(usedFields, PatternFields.Hours24))
                {
                    if (AreAllFieldsUsed(usedFields, PatternFields.Hours12 | PatternFields.Hours24))
                    {
                        if (Hours12 % 12 != Hours24 % 12)
                        {
                            return ParseResult<LocalTime>.InconsistentValues('H', 'h');
                        }
                    }
                    if (IsFieldUsed(usedFields, PatternFields.AmPm))
                    {
                        if (Hours24 / 12 != AmPm)
                        {
                            return ParseResult<LocalTime>.InconsistentValues('H', 't');
                        }
                    }
                    hour = Hours24;
                    return null;
                }
                // Okay, it's definitely valid - but we've still got 8 possibilities for what's been specified.
                switch (usedFields & (PatternFields.Hours12 | PatternFields.AmPm))
                {
                    case PatternFields.Hours12 | PatternFields.AmPm:
                        hour = (Hours12 % 12) + AmPm * 12;
                        break;
                    case PatternFields.Hours12:
                        // Preserve AM/PM from template value
                        hour = (Hours12 % 12) + (templateValue.HourOfDay / 12) * 12;
                        break;
                    case PatternFields.AmPm:
                        // Preserve 12-hour hour of day from template value, use specified AM/PM
                        hour = (templateValue.HourOfDay % 12) + AmPm * 12;
                        break;
                    case 0:
                        hour = templateValue.HourOfDay;
                        break;
                }
                return null;
            }
        }
    }
}
