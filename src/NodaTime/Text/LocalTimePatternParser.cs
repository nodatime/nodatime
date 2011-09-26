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
    internal sealed class LocalTimePatternParser : IPatternParser<LocalTime>
    {
        private const int FractionOfSecondLength = 7;
        private delegate PatternParseResult<LocalTime> CharacterHandler(PatternCursor patternCursor, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> patternBuilder);

        private static readonly Dictionary<char, CharacterHandler> PatternCharacterHandlers = new Dictionary<char, CharacterHandler>()
        {
            // TODO: Put these first four into SteppedPatternBuilder for sure...
            { '%', HandlePercent },
            { '\'', HandleQuote },
            { '\"', HandleQuote },
            { '\\', HandleBackslash },
            { '.', HandlePeriod },
            { ':', HandleColon }, //(pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalTime>.TimeSeparatorMismatch) },
            { 'h', Handle12HourSpecifier },
            { 'H', Handle24HourSpecifier },
            { 'm', HandleMinuteSpecifier },
            { 's', HandleSecondSpecifier },
            { 'f', HandleFractionSpecifier },
            { 'F', HandleFractionSpecifier },
            { 't', HandleAmPmDesignator }
        };

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

            var patternBuilder = new SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>(formatInfo, () => new LocalTimeParseBucket());
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
        // TODO: Move a bunch of these into SteppedPatternBuilder.

        /// <summary>
        /// Handle a leading "%" which acts as a pseudo-escape - it's mostly used to allow format strings such as "%H" to mean
        /// "use a custom format string consisting of H instead of a standard pattern H".
        /// </summary>
        private static PatternParseResult<LocalTime> HandlePercent(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            if (pattern.HasMoreCharacters)
            {
                if (pattern.PeekNext() != '%')
                {
                    // Handle the next character as normal
                    return null;
                }
                return PatternParseResult<LocalTime>.PercentDoubled;
            }
            return PatternParseResult<LocalTime>.PercentAtEndOfString;
        }

        private static PatternParseResult<LocalTime> HandleQuote(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            string quoted = pattern.GetQuotedString(pattern.Current, ref failure);
            if (failure != null)
            {
                return failure;
            }
            return builder.AddLiteral(quoted, ParseResult<LocalTime>.QuotedStringMismatch);
        }

        private static PatternParseResult<LocalTime> HandleBackslash(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            if (!pattern.MoveNext())
            {
                return PatternParseResult<LocalTime>.EscapeAtEndOfString;
            }
            builder.AddLiteral(pattern.Current, ParseResult<LocalTime>.EscapedCharacterMismatch);
            return null;
        }
        
        private static PatternParseResult<LocalTime> HandleColon(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            string timeSeparator = builder.FormatInfo.TimeSeparator;
            builder.AddParseAction((str, bucket) => str.Match(timeSeparator) ? null : ParseResult<LocalTime>.TimeSeparatorMismatch);
            builder.AddFormatAction((localTime, sb) => sb.Append(timeSeparator));
            return null;
        }
    
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

        private static PatternParseResult<LocalTime> Handle12HourSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Hours12, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddParseValueAction(count, 2, pattern.Current, 1, 12, (bucket, value) => bucket.Hours12 = value);
            builder.AddFormatAction((localTime, sb) => FormatHelper.LeftPad(localTime.ClockHourOfHalfDay, count, sb)); // builder.AddFormatLeftPad(count, localTime => localTime.ClockHourOfHalfDay);
            return null;
        }

        private static PatternParseResult<LocalTime> Handle24HourSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Hours24, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddParseValueAction(count, 2, pattern.Current, 0, 23, (bucket, value) => bucket.Hours24 = value);
            builder.AddFormatAction((localTime, sb) => FormatHelper.LeftPad(localTime.HourOfDay, count, sb)); // builder.AddFormatLeftPad(count, localTime => localTime.Hours);
            return null;
        }

        private static PatternParseResult<LocalTime> HandleMinuteSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Minutes, pattern.Current);
            if (failure != null)
            {
                return failure;
            }
            builder.AddParseValueAction(count, 2, pattern.Current, 0, 59, (bucket, value) => bucket.Minutes = value);
            builder.AddFormatAction((localTime, sb) => FormatHelper.LeftPad(localTime.MinuteOfHour, count, sb)); //builder.AddFormatLeftPad(count, localTime => localTime.Minutes);            
            return null;
        }

        private static PatternParseResult<LocalTime> HandleSecondSpecifier(PatternCursor pattern, SteppedPatternBuilder<LocalTime, LocalTimeParseBucket> builder)
        {
            PatternParseResult<LocalTime> failure = null;
            int count = pattern.GetRepeatCount(2, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Seconds, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddParseValueAction(count, 2, pattern.Current, 0, 59, (bucket, value) => bucket.Seconds = value);
            builder.AddFormatAction((localTime, sb) => FormatHelper.LeftPad(localTime.SecondOfMinute, count, sb)); //builder.AddFormatLeftPad(count, localTime => localTime.Seconds);
            return null;
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
                        bucket.AmPm = false;
                        return null;
                    }
                    if (str.Match(pmDesignator[0]))
                    {
                        bucket.AmPm = true;
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
                    bucket.AmPm = false;
                    return null;
                }
                if (str.Match(pmDesignator))
                {
                    bucket.AmPm = true;
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

        private sealed class LocalTimeParseBucket : ParseBucket<LocalTime>
        {
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
            /// AM (false) or PM (true).
            /// </summary>
            internal bool AmPm = false;

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>            
            internal override ParseResult<LocalTime> CalculateValue(PatternFields usedFields)
            {
                if ((usedFields & PatternFields.Hours12) != 0 &&
                    (usedFields & PatternFields.Hours24) != 0)
                {
                    if (Hours12 % 12 != Hours24 % 12)
                    {
                        return ParseResult<LocalTime>.InconsistentValues('h', 'H');
                    }
                }
                throw new NotImplementedException();
            }
        }
    }
}
