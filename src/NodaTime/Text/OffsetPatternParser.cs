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
            {
                'h',
                (pattern, builder) =>
                PatternParseResult<Offset>.Hour12PatternNotSupported
                },
            { 'H', Handle24HourSpecifier },
            { 'm', HandleMinuteSpecifier },
            { 's', HandleSecondSpecifier },
            { 'f', HandleFractionSpecifier },
            { 'F', HandleFractionSpecifier },
        };

        public PatternParseResult<Offset> ParsePattern(string pattern, NodaFormatInfo formatInfo, ParseStyles parseStyles)
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
                return ExpandStandardFormatPattern(patternCharacter, formatInfo, parseStyles);
            }

            var patternBuilder = new SteppedPatternBuilder<Offset, OffsetParseBucket>(formatInfo, parseStyles, () => new OffsetParseBucket());
            var patternCursor = new PatternCursor(pattern);

            // FIXME: Still need to reproduce the whitespace in the pattern when formatting.
            // FIXME: Put all of this in one place... in the builder?
            // FIXME: Presumably we don't have tests for this...
            if (patternBuilder.AllowTrailingWhite)
            {
                patternCursor.TrimTrailingWhiteSpaces();
                patternCursor.TrimTrailingInQuoteSpaces();
                patternBuilder.AddParseAction((str, bucket) =>
                {
                    str.TrimTrailingWhiteSpaces();
                    return null;
                });
            }
            if (patternBuilder.AllowLeadingWhite)
            {
                patternCursor.TrimLeadingWhiteSpaces();
                patternCursor.TrimLeadingInQuoteSpaces();
                patternBuilder.AddParseAction((str, bucket) =>
                {
                    str.TrimLeadingWhiteSpaces();
                    return null;
                });
            }
            // Prime the pump...
            // TODO: Add this to the builder
            patternBuilder.AddParseAction((str, bucket) =>
                                          {
                                              str.MoveNext();
                                              return null;
                                          });

            bool allowInnerWhite = patternBuilder.AllowInnerWhite;

            while (patternCursor.MoveNext())
            {
                // Not sure about this... skipping here means we don't get to know whether there *was* whitespace
                // when we hit a space in the pattern itself.
                if (allowInnerWhite)
                {
                    patternBuilder.SkipWhiteSpace();
                }
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
        private PatternParseResult<Offset> ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo, ParseStyles parseStyles)
        {
            // TODO: Cache these by culture.
            string singlePatternResource = null;
            switch (patternCharacter)
            {
                case 'g':
                {
                    var parsePatterns = new List<IParsedPattern<Offset>>();
                    foreach (char c in "flms")
                    {
                        // Each of the parsers could fail
                        var individual = ExpandStandardFormatPattern(c, formatInfo, parseStyles);
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
                    return PatternParseResult<Offset>.UnknownStandardFormat(patternCharacter, typeof(Offset));
            }
            // TODO: Guard against recursion? Validate that the pattern expands to a longer pattern?
            string pattern = Resources.ResourceManager.GetString(singlePatternResource, formatInfo.CultureInfo);
            return ParsePattern(pattern, formatInfo, parseStyles);
        }

        private string FormatGeneral(Offset value, List<IParsedPattern<Offset>> parsedPatterns)
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
            builder.AddParseAction((str, bucket) =>
            {
                foreach (char character in quoted)
                {
                    if (character == ' ' && builder.AllowInnerWhite)
                    {
                        str.SkipWhiteSpaces();
                    }
                    else if (!str.Match(character))
                    {
                        return ParseResult<Offset>.QuotedStringMismatch;
                    }
                }
                return null;
            });
            builder.AddFormatAction((offset, sb) => sb.Append(quoted));
            return null;
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
                    if (!valueCursor.ParseFraction(count, 3, out fractionalSeconds, false)) // We don't need *all* the digits to be present
                    {
                        return ParseResult<Offset>.MismatchedNumber(new string('F', count));
                    }
                    if (!BucketHelper.TryAssignNewValue(ref bucket.FractionalSeconds, fractionalSeconds, 'F'))
                    {
                        return ParseResult<Offset>.DoubleAssigment('F');
                    }
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
            builder.AddRequiredSign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.Milliseconds >= 0);
            return null;
        }

        private static PatternParseResult<Offset> HandleMinus(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            builder.AddNegativeOnlySign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.Milliseconds >= 0);
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
            
            // TODO: Make this part of the *pattern* failure detection.
            builder.AddParseAction((str, bucket) =>
            {
                int value;
                if (str.ParseDigits(count, 2, out value))
                {
                    if (!BucketHelper.TryAssignNewValue(ref bucket.Hours, value, 'H'))
                    {
                        return ParseResult<Offset>.DoubleAssigment('H');
                    }
                    return null;
                }
                return ParseResult<Offset>.MismatchedNumber(new string('H', count));
            });
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
            // TODO: Make this part of the *pattern* failure detection.
            builder.AddParseAction((str, bucket) =>
            {
                int value;
                if (str.ParseDigits(count, 2, out value))
                {
                    if (!BucketHelper.TryAssignNewValue(ref bucket.Minutes, value, 'm'))
                    {
                        return ParseResult<Offset>.DoubleAssigment('m');
                    }
                    return null;
                }
                return ParseResult<Offset>.MismatchedNumber(new string('m', count));
            });
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
            // TODO: Make this part of the *pattern* failure detection.
            builder.AddParseAction((str, bucket) =>
            {
                int value;
                if (str.ParseDigits(count, 2, out value))
                {
                    if (!BucketHelper.TryAssignNewValue(ref bucket.Seconds, value, 's'))
                    {
                        return ParseResult<Offset>.DoubleAssigment('s');
                    }
                    return null;
                }
                return ParseResult<Offset>.MismatchedNumber(new string('s', count));
            });
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
            builder.AddParseAction((str, bucket) =>
            {
                int fractionalSeconds;
                // If the pattern is 'f', we need exactly "count" digits. Otherwise ('F') we need
                // "up to count" digits.
                if (!str.ParseFraction(count, 3, out fractionalSeconds, patternCharacter == 'f'))
                {
                    return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                }
                if (!BucketHelper.TryAssignNewValue(ref bucket.FractionalSeconds, fractionalSeconds, pattern.Current))
                {
                    // TODO: Make this part of the *pattern* failure detection.
                    return ParseResult<Offset>.DoubleAssigment(patternCharacter);
                }
                return null;
            });
            return patternCharacter == 'f' ? builder.AddFormatRightPad(count, 3, offset => offset.FractionalSeconds)
                                           : builder.AddFormatRightPadTruncate(count, 3, offset => offset.FractionalSeconds);
        }

        private static PatternParseResult<Offset> HandleDefaultCharacter(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            char current = pattern.Current;
            if (current == ' ')
            {
                builder.AddParseAction((str, bucket) => !builder.AllowInnerWhite && !str.Match(' ') ? ParseResult<Offset>.MismatchedSpace : null);
                builder.AddFormatAction((offset, sb) => sb.Append(current));
            }
            else
            {
                builder.AddLiteral(current, ParseResult<Offset>.MismatchedCharacter);
            }
            return null;
        }

        private static IParsedPattern<Offset> CreateNumberPattern(NodaFormatInfo formatInfo)
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
                        return ParseResult<Offset>.ValueOutOfRange(milliseconds, typeof(Offset));
                    }
                    return ParseResult<Offset>.ForValue(Offset.FromMilliseconds(milliseconds));
                }
                return ParseResult<Offset>.CannotParseValue(value, typeof(Offset), "n");
            };
            NodaFunc<Offset, string> formatter = value => value.Milliseconds.ToString("N0", formatInfo);
            return new SimpleParsedPattern<Offset>(parser, formatter);
        }

        #endregion
    }
}
