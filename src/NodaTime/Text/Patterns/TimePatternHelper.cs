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

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Common methods used when parsing dates - these are used from LocalDateTimePatternParser,
    /// OffsetPatternParser and LocalTimePatternParser.
    /// </summary>
    internal static class TimePatternHelper
    {
        /// <summary>
        /// Creates a character handler for a dot (period). This is *not* culture sensitive - it is
        /// always treated as a literal, but with the additional behaviour that if it's followed by an 'F' pattern,
        /// that makes the period optional.
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreatePeriodHandler<TResult, TBucket>
            (int maxCount, NodaFunc<TResult, int> getter, NodaAction<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                // Note: Deliberately *not* using the decimal separator of the culture - see issue 21.

                // If the next part of the pattern is an F, then this decimal separator is effectively optional.
                // At parse time, we need to check whether we've matched the decimal separator. If we have, match the fractional
                // seconds part as normal. Otherwise, we continue on to the next parsing token.
                // At format time, we should always append the decimal separator, and then append using PadRightTruncate.
                if (pattern.PeekNext() == 'F')
                {
                    PatternParseResult<TResult> failure = null;
                    pattern.MoveNext();
                    int count = pattern.GetRepeatCount(maxCount, ref failure);
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
                        if (!valueCursor.ParseFraction(count, maxCount, out fractionalSeconds, false))
                        {
                            return ParseResult<TResult>.MismatchedNumber(new string('F', count));
                        }
                        // No need to validate the value - we've got one to three digits, so the range 0-999 is guaranteed.
                        setter(bucket, fractionalSeconds);
                        return null;
                    });
                    builder.AddFormatAction((localTime, sb) => sb.Append('.'));
                    builder.AddFormatRightPadTruncate(count, maxCount, getter);
                    return null;
                }
                else
                {
                    return builder.AddLiteral('.', ParseResult<TResult>.MismatchedCharacter);
                }
            };
        }

        /// <summary>
        /// Creates a character handler to handle the "fraction of a second" specifier (f or F).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateFractionHandler<TResult, TBucket>
            (int maxCount, NodaFunc<TResult, int> getter, NodaAction<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                PatternParseResult<TResult> failure = null;
                char patternCharacter = pattern.Current;
                int count = pattern.GetRepeatCount(maxCount, ref failure);
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
                    if (!str.ParseFraction(count, maxCount, out fractionalSeconds, patternCharacter == 'f'))
                    {
                        return ParseResult<TResult>.MismatchedNumber(new string(patternCharacter, count));
                    }
                    // No need to validate the value - we've got an appropriate number of digits, so the range is guaranteed.
                    setter(bucket, fractionalSeconds);
                    return null;
                });
                return patternCharacter == 'f'
                            ? builder.AddFormatRightPad(count, maxCount, getter)
                            : builder.AddFormatRightPadTruncate(count, maxCount, getter);
            };
        }

        internal static CharacterHandler<TResult, TBucket> CreateAmPmHandler<TResult, TBucket>
            (NodaFunc<TResult, int> hourOfDayGetter, NodaAction<TBucket, int> amPmSetter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                PatternParseResult<TResult> failure = null;
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
                // TODO: Handle empty designators
                // Single character designator
                if (count == 1)
                {
                    builder.AddParseAction((str, bucket) =>
                    {
                        if (str.Match(amDesignator[0]))
                        {
                            amPmSetter(bucket, 0);
                            return null;
                        }
                        if (str.Match(pmDesignator[0]))
                        {
                            amPmSetter(bucket, 1);
                            return null;
                        }
                        return ParseResult<TResult>.MissingAmPmDesignator;
                    });
                    builder.AddFormatAction((value, sb) => sb.Append(hourOfDayGetter(value) > 11 ? pmDesignator[0] : amDesignator[0]));
                    return null;
                }
                // Full designator
                builder.AddParseAction((str, bucket) =>
                {
                    if (str.Match(amDesignator))
                    {
                        amPmSetter(bucket, 0);
                        return null;
                    }
                    if (str.Match(pmDesignator))
                    {
                        amPmSetter(bucket, 1);
                        return null;
                    }
                    return ParseResult<TResult>.MissingAmPmDesignator;
                });
                builder.AddFormatAction((value, sb) => sb.Append(hourOfDayGetter(value) > 11 ? pmDesignator : amDesignator));
                return null;
            };
        }
    }
}
