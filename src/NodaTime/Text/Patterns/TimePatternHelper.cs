// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;

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
            (int maxCount, Func<TResult, int> getter, Action<TBucket, int> setter)
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
                    pattern.MoveNext();
                    int count = pattern.GetRepeatCount(maxCount);
                    builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
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
                    builder.AddFormatFractionTruncate(count, maxCount, getter);
                }
                else
                {
                    builder.AddLiteral('.', ParseResult<TResult>.MismatchedCharacter);
                }
            };
        }

        /// <summary>
        /// Creates a character handler for a dot (period) or comma, which have the same meaning.
        /// Formatting always uses a dot, but parsing will allow a comma instead, to conform with
        /// ISO-8601. This is *not* culture sensitive.
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateCommaDotHandler<TResult, TBucket>
            (int maxCount, Func<TResult, int> getter, Action<TBucket, int> setter)
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
                    pattern.MoveNext();
                    int count = pattern.GetRepeatCount(maxCount);
                    builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
                    builder.AddParseAction((valueCursor, bucket) =>
                    {
                        // If the next token isn't a dot or comma, we assume
                        // it's part of the next token in the pattern
                        if (!valueCursor.Match('.') && !valueCursor.Match(','))
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
                        // No need to validate the value - we've got an appropriate number of digits, so the range is guaranteed.
                        setter(bucket, fractionalSeconds);
                        return null;
                    });
                    builder.AddFormatAction((localTime, sb) => sb.Append('.'));
                    builder.AddFormatFractionTruncate(count, maxCount, getter);
                }
                else
                {
                    builder.AddParseAction((str, bucket) => str.Match('.') || str.Match(',') 
                                                            ? null 
                                                            : ParseResult<TResult>.MismatchedCharacter(';'));
                    builder.AddFormatAction((value, sb) => sb.Append('.'));
                }
            };
        }

        /// <summary>
        /// Creates a character handler to handle the "fraction of a second" specifier (f or F).
        /// </summary>
        internal static CharacterHandler<TResult, TBucket> CreateFractionHandler<TResult, TBucket>
            (int maxCount, Func<TResult, int> getter, Action<TBucket, int> setter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                char patternCharacter = pattern.Current;
                int count = pattern.GetRepeatCount(maxCount);
                builder.AddField(PatternFields.FractionalSeconds, pattern.Current);
                
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
                if (patternCharacter == 'f')
                {
                    builder.AddFormatFraction(count, maxCount, getter);
                }
                else
                {
                    builder.AddFormatFractionTruncate(count, maxCount, getter);
                }
            };
        }

        internal static CharacterHandler<TResult, TBucket> CreateAmPmHandler<TResult, TBucket>
            (Func<TResult, int> hourOfDayGetter, Action<TBucket, int> amPmSetter)
            where TBucket : ParseBucket<TResult>
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(2);
                builder.AddField(PatternFields.AmPm, pattern.Current);

                string amDesignator = builder.FormatInfo.AMDesignator;
                string pmDesignator = builder.FormatInfo.PMDesignator;

                // If we don't have an AM or PM designator, we're nearly done. Set the AM/PM designator
                // to the special value of 2, meaning "take it from the template".
                if (amDesignator == "" && pmDesignator == "")
                {
                    builder.AddParseAction((str, bucket) =>
                    {
                        amPmSetter(bucket, 2);
                        return null;
                    });
                    return;
                }
                // Odd scenario (but present in af-ZA for .NET 2) - exactly one of the AM/PM designator is valid.
                // Delegate to a separate method to keep this clearer...
                if (amDesignator == "" || pmDesignator == "")
                {
                    int specifiedDesignatorValue = amDesignator == "" ? 1 : 0;
                    string specifiedDesignator = specifiedDesignatorValue == 1 ? pmDesignator : amDesignator;
                    HandleHalfAmPmDesignator(count, specifiedDesignator, specifiedDesignatorValue, hourOfDayGetter, amPmSetter, builder);
                    return;
                }
                CompareInfo compareInfo = builder.FormatInfo.CompareInfo;
                // Single character designator
                if (count == 1)
                {
                    // It's not entirely clear whether this is the right thing to do... there's no nice
                    // way of providing a single-character case-insensitive match.
                    string amFirst = amDesignator.Substring(0, 1);
                    string pmFirst = pmDesignator.Substring(0, 1);
                    builder.AddParseAction((str, bucket) =>
                    {
                        if (str.MatchCaseInsensitive(amFirst, compareInfo, true))
                        {
                            amPmSetter(bucket, 0);
                            return null;
                        }
                        if (str.MatchCaseInsensitive(pmFirst, compareInfo, true))
                        {
                            amPmSetter(bucket, 1);
                            return null;
                        }
                        return ParseResult<TResult>.MissingAmPmDesignator;
                    });
                    builder.AddFormatAction((value, sb) => sb.Append(hourOfDayGetter(value) > 11 ? pmDesignator[0] : amDesignator[0]));
                    return;
                }
                // Full designator
                builder.AddParseAction((str, bucket) =>
                {
                    // Could use the "match longest" approach, but with only two it feels a bit silly to build a list...
                    bool pmLongerThanAm = pmDesignator.Length > amDesignator.Length;
                    string longerDesignator = pmLongerThanAm ? pmDesignator : amDesignator;
                    string shorterDesignator = pmLongerThanAm ? amDesignator : pmDesignator;
                    int longerValue = pmLongerThanAm ? 1 : 0;
                    if (str.MatchCaseInsensitive(longerDesignator, compareInfo, true))
                    {
                        amPmSetter(bucket, longerValue);
                        return null;
                    }
                    if (str.MatchCaseInsensitive(shorterDesignator, compareInfo, true))
                    {
                        amPmSetter(bucket, 1 - longerValue);
                        return null;
                    }
                    return ParseResult<TResult>.MissingAmPmDesignator;
                });
                builder.AddFormatAction((value, sb) => sb.Append(hourOfDayGetter(value) > 11 ? pmDesignator : amDesignator));
            };
        }

        private static void HandleHalfAmPmDesignator<TResult, TBucket>
            (int count, string specifiedDesignator, int specifiedDesignatorValue, Func<TResult, int> hourOfDayGetter, Action<TBucket, int> amPmSetter,
             SteppedPatternBuilder<TResult, TBucket> builder)
            where TBucket : ParseBucket<TResult>
        {
            CompareInfo compareInfo = builder.FormatInfo.CompareInfo;
            if (count == 1)
            {
                string abbreviation = specifiedDesignator.Substring(0, 1);
                builder.AddParseAction((str, bucket) =>
                {
                    int value = str.MatchCaseInsensitive(abbreviation, compareInfo, true) ? specifiedDesignatorValue : 1 - specifiedDesignatorValue;
                    amPmSetter(bucket, value);
                    return null;
                });
                builder.AddFormatAction((value, sb) =>
                {
                    // Only append anything if it's the non-empty designator.
                    if (hourOfDayGetter(value) / 12 == specifiedDesignatorValue)
                    {
                        sb.Append(specifiedDesignator[0]);
                    }
                });
                return;
            }
            builder.AddParseAction((str, bucket) =>
            {
                int value = str.MatchCaseInsensitive(specifiedDesignator, compareInfo, true) ? specifiedDesignatorValue : 1 - specifiedDesignatorValue;
                amPmSetter(bucket, value);
                return null;
            });
            builder.AddFormatAction((value, sb) =>
            {
                // Only append anything if it's the non-empty designator.
                if (hourOfDayGetter(value) / 12 == specifiedDesignatorValue)
                {
                    sb.Append(specifiedDesignator);
                }
            });
        }
    }
}
