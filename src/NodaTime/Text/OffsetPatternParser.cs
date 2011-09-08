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
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    internal class OffsetPatternParser : IPatternParser<Offset>
    {
        private delegate PatternParseResult<Offset> CharacterHandler(PatternCursor patternCursor, SteppedPatternBuilder<Offset, OffsetParseBucket> patternBuilder);

        private static readonly Dictionary<char, CharacterHandler> PatternCharacterHandlers = new Dictionary<char, CharacterHandler>()
        {
            { '%', HandlePercent },
            { '\'', HandleQuote },
            { '\"', HandleQuote },
            { '\\', HandleBackslash },
            { '.', HandlePeriod },
            { ':', HandleColon }, //(pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<Offset>.TimeSeparatorMismatch) },
            { '+', HandlePlus },
            { '-', HandleMinus },
            { 'h', (pattern, builder) => PatternParseResult<Offset>.Hour12PatternNotSupported },
            { 'H', Handle24HourSpecifier },
            { 'm', HandleMinuteSpecifier },
            { 's', HandleSecondSpecifier },
            { 'f', HandleFractionSpecifier },
            { 'F', HandleFractionSpecifier },
        };

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<Offset> ParsePattern(string pattern, NodaFormatInfo formatInfo)
        {
            if (pattern == null)
            {
                return PatternParseResult<Offset>.ArgumentNull("format");
            }
            if (pattern.Length == 0)
            {
                return PatternParseResult<Offset>.FormatStringEmpty;
            }

            if (pattern.Length == 1)
            {
                char patternCharacter = pattern[0];
                if (patternCharacter == 'n')
                {
                    return PatternParseResult<Offset>.ForValue(CreateNumberPattern(formatInfo));
                }
                return ExpandStandardFormatPattern(patternCharacter, formatInfo);
            }

            var patternBuilder = new SteppedPatternBuilder<Offset, OffsetParseBucket>(formatInfo, () => new OffsetParseBucket());
            var patternCursor = new PatternCursor(pattern);

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
                PatternParseResult<Offset> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            return PatternParseResult<Offset>.ForValue(patternBuilder.Build());
        }

        #region Standard patterns
        private PatternParseResult<Offset> ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            string singlePatternResource = null;
            switch (patternCharacter)
            {
                case 'g':
                {
                    var parsePatterns = new List<IPattern<Offset>>();
                    foreach (char c in "flms")
                    {
                        // Each of the parsers could fail
                        var individual = ExpandStandardFormatPattern(c, formatInfo);
                        if (!individual.Success)
                        {
                            return individual;
                        }
                        // We know this is safe now.
                        parsePatterns.Add(individual.GetResultOrThrow());
                    }
                    NodaFunc<Offset, string> formatter = value => FormatGeneral(value, parsePatterns);
                    return PatternParseResult<Offset>.ForValue(new CompositePattern<Offset>(parsePatterns, formatter));
                }
                case 'f':
                    singlePatternResource = "OffsetPatternFull";
                    break;
                case 'l':
                    singlePatternResource = "OffsetPatternLong";
                    break;
                case 'm':
                    singlePatternResource = "OffsetPatternMedium";
                    break;
                case 's':
                    singlePatternResource = "OffsetPatternShort";
                    break;
                default:
                    // Will be turned into an exception.
                    return PatternParseResult<Offset>.UnknownStandardFormat(patternCharacter);
            }
            // TODO: Guard against recursion? Validate that the pattern expands to a longer pattern?
            string pattern = Resources.ResourceManager.GetString(singlePatternResource, formatInfo.CultureInfo);
            return ParsePattern(pattern, formatInfo);
        }

        private string FormatGeneral(Offset value, List<IPattern<Offset>> parsedPatterns)
        {
            // Note: this relies on the order in ExpandStandardFormatPattern
            int index;
            if (value.FractionalSeconds != 0)
            {
                index = 0;
            }
            else if (value.Seconds != 0)
            {
                index = 1;
            }
            else if (value.Minutes != 0)
            {
                index = 2;
            }
            else
            {
                index = 3;
            }
            return parsedPatterns[index].Format(value);
        }
        #endregion

        #region Character handlers
        // TODO: Move a bunch of these into SteppedPatternBuilder.

        /// <summary>
        /// Handle a leading "%" which acts as a pseudo-escape - it's mostly used to allow format strings such as "%H" to mean
        /// "use a custom format string consisting of H instead of a standard pattern H".
        /// </summary>
        private static PatternParseResult<Offset> HandlePercent(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            if (pattern.HasMoreCharacters)
            {
                if (pattern.PeekNext() != '%')
                {
                    // Handle the next character as normal
                    return null;
                }
                return PatternParseResult<Offset>.PercentDoubled;
            }
            return PatternParseResult<Offset>.PercentAtEndOfString;
        }

        private static PatternParseResult<Offset> HandleQuote(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
            string quoted = pattern.GetQuotedString(pattern.Current, ref failure);
            if (failure != null)
            {
                return failure;
            }
            return builder.AddLiteral(quoted, ParseResult<Offset>.QuotedStringMismatch);
        }

        private static PatternParseResult<Offset> HandleBackslash(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            if (!pattern.MoveNext())
            {
                return PatternParseResult<Offset>.EscapeAtEndOfString;
            }
            builder.AddLiteral(pattern.Current, ParseResult<Offset>.EscapedCharacterMismatch);
            return null;
        }
        
        private static PatternParseResult<Offset> HandleColon(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            string timeSeparator = builder.FormatInfo.TimeSeparator;
            builder.AddParseAction((str, bucket) => str.Match(timeSeparator) ? null : ParseResult<Offset>.TimeSeparatorMismatch);
            builder.AddFormatAction((offset, sb) => sb.Append(timeSeparator));
            return null;
        }
    
        private static PatternParseResult<Offset> HandlePeriod(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            string decimalSeparator = builder.FormatInfo.DecimalSeparator;
            // If the next part of the pattern is an F, then this decimal separator is effectively optional.
            // At parse time, we need to check whether we've matched the decimal separator. If we have, match the fractional
            // seconds part as normal. Otherwise, we continue on to the next parsing token.
            // At format time, we should always append the decimal separator, and then append using PadRightTruncate.
            if (pattern.PeekNext() == 'F')
            {
                PatternParseResult<Offset> failure = null;
                pattern.MoveNext();
                int count = pattern.GetRepeatCount(3, ref failure);
                if (failure != null)
                {
                    return failure;
                }
                builder.AddParseAction((valueCursor, bucket) =>
                {
                    // If the next token isn't the decimal separator, we assume it's part of the next token in the pattern
                    if (!valueCursor.Match(decimalSeparator))
                    {
                        return null;
                    }

                    // If there *was* a decimal separator, we should definitely have a number.
                    int fractionalSeconds;
                    // Last argument is false because we don't need *all* the digits to be present
                    if (!valueCursor.ParseFraction(count, 3, out fractionalSeconds, false))
                    {
                        return ParseResult<Offset>.MismatchedNumber(new string('F', count));
                    }
                    // No need to validate the value - we've got one to three digits, so the range 0-999 is guaranteed.
                    bucket.FractionalSeconds = fractionalSeconds;
                    return null;
                });
                builder.AddFormatAction((offset, sb) => sb.Append(decimalSeparator));
                builder.AddFormatRightPadTruncate(count, 3, offset => offset.FractionalSeconds);
                return null;
            }
            else
            {
                return builder.AddLiteral(decimalSeparator, ParseResult<Offset>.MissingDecimalSeparator);
            }
        }

        private static PatternParseResult<Offset> HandlePlus(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = builder.AddField(PatternFields.Sign, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddRequiredSign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.TotalMilliseconds >= 0);
            return null;
        }

        private static PatternParseResult<Offset> HandleMinus(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = builder.AddField(PatternFields.Sign, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddNegativeOnlySign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.TotalMilliseconds >= 0);
            return null;
        }

        private static PatternParseResult<Offset> Handle24HourSpecifier(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
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

            builder.AddParseValueAction(count, 2, pattern.Current, 0, 23, (bucket, value) => bucket.Hours = value);
            builder.AddFormatAction((offset, sb) => FormatHelper.LeftPad(offset.Hours, count, sb)); // builder.AddFormatLeftPad(count, offset => offset.Hours);
            return null;
        }

        private static PatternParseResult<Offset> HandleMinuteSpecifier(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
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
            builder.AddFormatAction((offset, sb) => FormatHelper.LeftPad(offset.Minutes, count, sb)); //builder.AddFormatLeftPad(count, offset => offset.Minutes);            
            return null;
        }

        private static PatternParseResult<Offset> HandleSecondSpecifier(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
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
            builder.AddFormatAction((offset, sb) => FormatHelper.LeftPad(offset.Seconds, count, sb)); //builder.AddFormatLeftPad(count, offset => offset.Seconds);
            return null;
        }

        private static PatternParseResult<Offset> HandleFractionSpecifier(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
            // TODO: fix the scaling of the value
            char patternCharacter = pattern.Current;
            int count = pattern.GetRepeatCount(3, ref failure);
            if (failure != null)
            {
                return failure;
            }
            failure = builder.AddField(PatternFields.Milliseconds, pattern.Current);
            if (failure != null)
            {
                return failure;
            }

            builder.AddParseAction((str, bucket) =>
            {
                int fractionalSeconds;
                // If the pattern is 'f', we need exactly "count" digits. Otherwise ('F') we need
                // "up to count" digits.
                if (!str.ParseFraction(count, 3, out fractionalSeconds, patternCharacter == 'f'))
                {
                    return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                }
                // No need to validate the value - we've got one to three digits, so the range 0-999 is guaranteed.
                bucket.FractionalSeconds = fractionalSeconds;
                return null;
            });
            return patternCharacter == 'f' ? builder.AddFormatRightPad(count, 3, offset => offset.FractionalSeconds)
                                           : builder.AddFormatRightPadTruncate(count, 3, offset => offset.FractionalSeconds);
        }

        private static PatternParseResult<Offset> HandleDefaultCharacter(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            return builder.AddLiteral(pattern.Current, ParseResult<Offset>.MismatchedCharacter);
        }

        private static IPattern<Offset> CreateNumberPattern(NodaFormatInfo formatInfo)
        {
            NodaFunc<string, ParseResult<Offset>> parser = value =>
            {
                int milliseconds;
                if (Int32.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands,
                                    formatInfo.NumberFormat, out milliseconds))
                {
                    if (milliseconds < -NodaConstants.MillisecondsPerStandardDay ||
                        NodaConstants.MillisecondsPerStandardDay < milliseconds)
                    {
                        return ParseResult<Offset>.ValueOutOfRange(milliseconds);
                    }
                    return ParseResult<Offset>.ForValue(Offset.FromMilliseconds(milliseconds));
                }
                return ParseResult<Offset>.CannotParseValue(value, "n");
            };
            NodaFunc<Offset, string> formatter = value => value.TotalMilliseconds.ToString("N0", formatInfo);
            return new SimplePattern<Offset>(parser, formatter);
        }

        #endregion
    }
}
