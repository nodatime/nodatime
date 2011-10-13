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
using System.Diagnostics;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    internal sealed class OffsetPatternParser : IPatternParser<Offset>
    {
        private static readonly Dictionary<char, CharacterHandler<Offset, OffsetParseBucket>> PatternCharacterHandlers = 
            new Dictionary<char, CharacterHandler<Offset, OffsetParseBucket>>
        {
            { '%', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleBackslash },
            { '.', HandlePeriod },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<Offset>.TimeSeparatorMismatch) },
            { 'h', (pattern, builder) => PatternParseResult<Offset>.Hour12PatternNotSupported },
            { 'H', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, value => value.Hours, (bucket, value) => bucket.Hours = value) },
            { 'm', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minutes, (bucket, value) => bucket.Minutes = value) },
            { 's', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Seconds, (bucket, value) => bucket.Seconds = value) },
            { '+', HandlePlus },
            { '-', HandleMinus },
            { 'f', HandleFractionSpecifier },
            { 'F', HandleFractionSpecifier },
        };

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public PatternParseResult<Offset> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<Offset>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<Offset>.FormatStringEmpty;
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                if (patternCharacter == 'n')
                {
                    return PatternParseResult<Offset>.ForValue(CreateNumberPattern(formatInfo));
                }
                if (patternCharacter == 'g')
                {
                    return CreateGeneralPattern(formatInfo);
                }
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    return PatternParseResult<Offset>.UnknownStandardFormat(patternCharacter);
                }
            }

            var patternBuilder = new SteppedPatternBuilder<Offset, OffsetParseBucket>(formatInfo, () => new OffsetParseBucket());
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
                CharacterHandler<Offset, OffsetParseBucket> handler;
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
        private string ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            switch (patternCharacter)
            {
                case 'f':
                    return formatInfo.OffsetPatternFull;
                case 'l':
                    return formatInfo.OffsetPatternLong;
                case 'm':
                    return formatInfo.OffsetPatternMedium;
                case 's':
                    return formatInfo.OffsetPatternShort;
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }

        private PatternParseResult<Offset> CreateGeneralPattern(NodaFormatInfo formatInfo)
        {
            var patterns = new List<IPattern<Offset>>();
            foreach (char c in "flms")
            {
                // Each of the parsers could fail
                var individual = ParsePattern(c.ToString(), formatInfo);
                if (!individual.Success)
                {
                    return individual;
                }
                // We know this is safe now.
                patterns.Add(individual.GetResultOrThrow());
            }
            NodaFunc<Offset, string> formatter = value => FormatGeneral(value, patterns);
            return PatternParseResult<Offset>.ForValue(new CompositePattern<Offset>(patterns, formatter));
        }

        private string FormatGeneral(Offset value, List<IPattern<Offset>> patterns)
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
            return patterns[index].Format(value);
        }
        #endregion

        #region Character handlers

        private static PatternParseResult<Offset> HandlePeriod(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            // Note: Deliberately *not* using the decimal separator of the culture - see issue 21.

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
                failure = builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
                if (failure != null)
                {
                    return failure;
                }
                builder.AddParseAction((valueCursor, bucket) =>
                {
                    // If the next token isn't a period, we assume it's part of the next token in the pattern
                    if (!valueCursor.Match("."))
                    {
                        return null;
                    }

                    // If there *was* a period, we should definitely have a number.
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
                builder.AddFormatAction((offset, sb) => sb.Append("."));
                builder.AddFormatRightPadTruncate(count, 3, offset => offset.FractionalSeconds);
                return null;
            }
            else
            {
                return builder.AddLiteral('.', ParseResult<Offset>.MismatchedCharacter);
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

        private static PatternParseResult<Offset> HandleFractionSpecifier(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            PatternParseResult<Offset> failure = null;
            char patternCharacter = pattern.Current;
            int count = pattern.GetRepeatCount(3, ref failure);
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

        /// <summary>
        /// Provides a container for the interim parsed pieces of an <see cref="Offset" /> value.
        /// </summary>
        [DebuggerStepThrough]
        internal sealed class OffsetParseBucket : ParseBucket<Offset>
        {
            /// <summary>
            /// The hours in the range [0, 23].
            /// </summary>
            internal int Hours;

            /// <summary>
            /// The minutes in the range [0, 59].
            /// </summary>
            internal int Minutes;

            /// <summary>
            /// The seconds in the range [0, 59].
            /// </summary>
            internal int Seconds;

            /// <summary>
            /// The fractions of a second in milliseconds.
            /// </summary>
            internal int FractionalSeconds;

            /// <summary>
            /// Gets a value indicating whether this instance is negative.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is negative; otherwise, <c>false</c>.
            /// </value>
            public bool IsNegative;

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>
            internal override ParseResult<Offset> CalculateValue(PatternFields usedFields)
            {
                Offset offset = Offset.Create(Hours, Minutes, Seconds, FractionalSeconds, IsNegative);
                return ParseResult<Offset>.ForValue(offset);
            }
        }
    }
}
