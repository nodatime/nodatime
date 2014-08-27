// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Utility;

namespace NodaTime.Text.Patterns
{
    /// <summary>
    /// Builder for a pattern which implements parsing and formatting as a sequence of steps applied
    /// in turn.
    /// </summary>
    internal sealed class SteppedPatternBuilder<TResult, TBucket> where TBucket : ParseBucket<TResult>
    {
        internal delegate ParseResult<TResult> ParseAction(ValueCursor cursor, TBucket bucket);

        private readonly NodaFormatInfo formatInfo;

        private readonly List<Action<TResult, StringBuilder>> formatActions;
        private readonly List<ParseAction> parseActions;
        private readonly Func<TBucket> bucketProvider;
        private PatternFields usedFields;
        private bool formatOnly = false;

        internal SteppedPatternBuilder(NodaFormatInfo formatInfo, Func<TBucket> bucketProvider)
        {
            this.formatInfo = formatInfo;
            formatActions = new List<Action<TResult, StringBuilder>>();
            parseActions = new List<ParseAction>();
            this.bucketProvider = bucketProvider;
        }

        internal NodaFormatInfo FormatInfo { get { return formatInfo; } }

        internal PatternFields UsedFields { get { return usedFields; } }

        /// <summary>
        /// Sets this pattern to only be capable of formatting; any attempt to parse using the
        /// built pattern will fail immediately.
        /// </summary>
        internal void SetFormatOnly()
        {
            formatOnly = true;
        }

        /// <summary>
        /// Performs common parsing operations: start with a parse action to move the
        /// value cursor onto the first character, then call a character handler for each
        /// character in the pattern to build up the steps. If any handler fails,
        /// that failure is returned - otherwise the return value is null.
        /// </summary>
        internal void ParseCustomPattern(string patternText,
            Dictionary<char, CharacterHandler<TResult, TBucket>> characterHandlers)
        {
            var patternCursor = new PatternCursor(patternText);

            // Now iterate over the pattern.
            while (patternCursor.MoveNext())
            {
                CharacterHandler<TResult, TBucket> handler;
                if (characterHandlers.TryGetValue(patternCursor.Current, out handler))
                {
                    handler(patternCursor, this);
                }
                else
                {
                    char current = patternCursor.Current;
                    if ((current >= 'A' && current <= 'Z') || (current >= 'a' && current <= 'z'))
                    {
                        throw new InvalidPatternException(Messages.Parse_UnquotedLiteral, current);
                    }
                    AddLiteral(patternCursor.Current, ParseResult<TResult>.MismatchedCharacter);
                }
            }
        }

        /// <summary>
        /// Validates the combination of fields used.
        /// </summary>
        internal void ValidateUsedFields()
        {
            // We assume invalid combinations are global across all parsers. The way that
            // the patterns are parsed ensures we never end up with any invalid individual fields
            // (e.g. time fields within a date pattern).

            if ((usedFields & (PatternFields.Era | PatternFields.YearOfEra)) == PatternFields.Era)
            {
                throw new InvalidPatternException(Messages.Parse_EraWithoutYearOfEra);
            }
            const PatternFields calendarAndEra = PatternFields.Era | PatternFields.Calendar;
            if ((usedFields & calendarAndEra) == calendarAndEra)
            {
                throw new InvalidPatternException(Messages.Parse_CalendarAndEra);
            }
        }

        /// <summary>
        /// Returns a built pattern. This is mostly to keep the API for the builder separate from that of the pattern,
        /// and for thread safety (publishing a new object, thus leading to a memory barrier).
        /// Note that this builder *must not* be used after the result has been built.
        /// </summary>
        internal IPartialPattern<TResult> Build()
        {
            Action<TResult, StringBuilder> formatDelegate = null;
            foreach (Action<TResult, StringBuilder> formatAction in formatActions)
            {
                IPostPatternParseFormatAction postAction = formatAction.Target as IPostPatternParseFormatAction;
                formatDelegate += postAction == null ? formatAction : postAction.BuildFormatAction(usedFields);
            }
            return new SteppedPattern(formatDelegate, formatOnly ? null : parseActions.ToArray(), bucketProvider, usedFields);
        }

        /// <summary>
        /// Registers that a pattern field has been used in this pattern, and throws a suitable error
        /// result if it's already been used.
        /// </summary>
        internal void AddField(PatternFields field, char characterInPattern)
        {
            PatternFields newUsedFields = usedFields | field;
            if (newUsedFields == usedFields)
            {
                throw new InvalidPatternException(Messages.Parse_RepeatedFieldInPattern, characterInPattern);
            }
            usedFields = newUsedFields;
        }

        internal void AddParseAction(ParseAction parseAction)
        {
            parseActions.Add(parseAction);
        }

        internal void AddFormatAction(Action<TResult, StringBuilder> formatAction)
        {
            formatActions.Add(formatAction);
        }

        internal void AddParseValueAction(int minimumDigits, int maximumDigits, char patternChar,
                                          int minimumValue, int maximumValue,
                                          Action<TBucket, int> valueSetter)
        {
            AddParseAction((cursor, bucket) =>
            {
                int startingIndex = cursor.Index;
                int value;
                bool negative = cursor.Match('-');
                if (negative && minimumValue >= 0)
                {
                    cursor.Move(startingIndex);
                    return ParseResult<TResult>.UnexpectedNegative(cursor);
                }
                if (!cursor.ParseDigits(minimumDigits, maximumDigits, out value))
                {
                    cursor.Move(startingIndex);
                    return ParseResult<TResult>.MismatchedNumber(cursor, new string(patternChar, minimumDigits));
                }
                if (negative)
                {
                    value = -value;
                }
                if (value < minimumValue || value > maximumValue)
                {
                    cursor.Move(startingIndex);
                    return ParseResult<TResult>.FieldValueOutOfRange(cursor, value, patternChar);
                }

                valueSetter(bucket, value);
                return null;
            });
        }

        /// <summary>
        /// Adds text which must be matched exactly when parsing, and appended directly when formatting.
        /// This overload uses the same failure result for all text values.
        /// </summary>
        internal void AddLiteral(string expectedText, Func<ValueCursor, ParseResult<TResult>> failure)
        {
            // Common case - single character literal, often a date or time separator.
            if (expectedText.Length == 1)
            {
                char expectedChar = expectedText[0];
                AddParseAction((str, bucket) => str.Match(expectedChar) ? null : failure(str));
                AddFormatAction((value, builder) => builder.Append(expectedChar));
                return;
            }
            // TODO: These are ludicrously slow... see
            // http://msmvps.com/blogs/jon_skeet/archive/2011/08/23/optimization-and-generics-part-2-lambda-expressions-and-reference-types.aspx
            // for a description of the problem. I need to find a solution though...
            AddParseAction((str, bucket) => str.Match(expectedText) ? null : failure(str));
            AddFormatAction((value, builder) => builder.Append(expectedText));
        }

        internal static void HandleQuote(PatternCursor pattern, SteppedPatternBuilder<TResult, TBucket> builder)
        {
            string quoted = pattern.GetQuotedString(pattern.Current);
            builder.AddLiteral(quoted, ParseResult<TResult>.QuotedStringMismatch);
        }

        internal static void HandleBackslash(PatternCursor pattern, SteppedPatternBuilder<TResult, TBucket> builder)
        {
            if (!pattern.MoveNext())
            {
                throw new InvalidPatternException(Messages.Parse_EscapeAtEndOfString);
            }
            builder.AddLiteral(pattern.Current, ParseResult<TResult>.EscapedCharacterMismatch);
        }

        /// <summary>
        /// Handle a leading "%" which acts as a pseudo-escape - it's mostly used to allow format strings such as "%H" to mean
        /// "use a custom format string consisting of H instead of a standard pattern H".
        /// </summary>
        internal static void HandlePercent(PatternCursor pattern, SteppedPatternBuilder<TResult, TBucket> builder)
        {
            if (pattern.HasMoreCharacters)
            {
                if (pattern.PeekNext() != '%')
                {
                    // Handle the next character as normal
                    return;
                }
                throw new InvalidPatternException(Messages.Parse_PercentDoubled);
            }
            throw new InvalidPatternException(Messages.Parse_PercentAtEndOfString);
        }

        /// <summary>
        /// Returns a handler for a zero-padded purely-numeric field specifier, such as "seconds", "minutes", "24-hour", "12-hour" etc.
        /// </summary>
        /// <param name="maxCount">Maximum permissable count (usually two)</param>
        /// <param name="field">Field to remember that we've seen</param>
        /// <param name="minValue">Minimum valid value for the field (inclusive)</param>
        /// <param name="maxValue">Maximum value value for the field (inclusive)</param>
        /// <param name="getter">Delegate to retrieve the field value when formatting</param>
        /// <param name="setter">Delegate to set the field value into a bucket when parsing</param>
        /// <returns>The pattern parsing failure, or null on success.</returns>
        internal static CharacterHandler<TResult, TBucket> HandlePaddedField(int maxCount, PatternFields field, int minValue, int maxValue, Func<TResult, int> getter,
            Action<TBucket, int> setter)
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(maxCount);
                builder.AddField(field, pattern.Current);
                builder.AddParseValueAction(count, maxCount, pattern.Current, minValue, maxValue, setter);
                builder.AddFormatLeftPad(count, getter);
            };
        }

        /// <summary>
        /// Adds a character which must be matched exactly when parsing, and appended directly when formatting.
        /// </summary>
        internal void AddLiteral(char expectedChar, Func<ValueCursor, char, ParseResult<TResult>> failureSelector)
        {
            AddParseAction((str, bucket) => str.Match(expectedChar) ? null : failureSelector(str, expectedChar));
            AddFormatAction((value, builder) => builder.Append(expectedChar));
        }

        /// <summary>
        /// Adds parse actions for a list of strings, such as days of the week or month names.
        /// The parsing is performed case-insensitively. All candidates are tested, and only the longest
        /// match is used.
        /// TODO: Make this much more efficient in terms of capture...
        /// </summary>
        internal void AddParseLongestTextAction(char field, Action<TBucket, int> setter, CompareInfo compareInfo, IList<string> textValues)
        {
            AddParseAction((str, bucket) => {
                int bestIndex = -1;
                int longestMatch = 0;
                FindLongestMatch(compareInfo, str, textValues, ref bestIndex, ref longestMatch);
                if (bestIndex != -1)
                {
                    setter(bucket, bestIndex);
                    str.Move(str.Index + longestMatch);
                    return null;
                }
                return ParseResult<TResult>.MismatchedText(str, field);
            });
        }

        /// <summary>
        /// Adds parse actions for two list of strings, such as non-genitive and genitive month names.
        /// The parsing is performed case-insensitively. All candidates are tested, and only the longest
        /// match is used.
        /// TODO: Make this much more efficient in terms of capture...
        /// </summary>
        internal void AddParseLongestTextAction(char field, Action<TBucket, int> setter, CompareInfo compareInfo, IList<string> textValues1, IList<string> textValues2)
        {
            AddParseAction((str, bucket) =>
            {
                int bestIndex = -1;
                int longestMatch = 0;
                FindLongestMatch(compareInfo, str, textValues1, ref bestIndex, ref longestMatch);
                FindLongestMatch(compareInfo, str, textValues2, ref bestIndex, ref longestMatch);
                if (bestIndex != -1)
                {
                    setter(bucket, bestIndex);
                    str.Move(str.Index + longestMatch);
                    return null;
                }
                return ParseResult<TResult>.MismatchedText(str, field);
            });
        }

        /// <summary>
        /// Find the longest match from a given set of candidate strings, updating the index/length of the best value
        /// accordingly.
        /// </summary>
        private static void FindLongestMatch(CompareInfo compareInfo, ValueCursor cursor, IList<string> values, ref int bestIndex, ref int longestMatch)
        {
            for (int i = 0; i < values.Count; i++)
            {
                string candidate = values[i];
                if (candidate == null || candidate.Length <= longestMatch)
                {
                    continue;
                }
                if (cursor.MatchCaseInsensitive(candidate, compareInfo, false))
                {
                    bestIndex = i;
                    longestMatch = candidate.Length;
                }
            }
        }

        /// <summary>
        /// Adds parse and format actions for a mandatory positive/negative sign.
        /// </summary>
        /// <param name="signSetter">Action to take when to set the given sign within the bucket</param>
        /// <param name="nonNegativePredicate">Predicate to detect whether the value being formatted is non-negative</param>
        public void AddRequiredSign(Action<TBucket, bool> signSetter, Func<TResult, bool> nonNegativePredicate)
        {
            string negativeSign = formatInfo.NegativeSign;
            string positiveSign = formatInfo.PositiveSign;
            AddParseAction((str, bucket) =>
            {
                if (str.Match(negativeSign))
                {
                    signSetter(bucket, false);
                    return null;
                }
                if (str.Match(positiveSign))
                {
                    signSetter(bucket, true);
                    return null;
                }
                return ParseResult<TResult>.MissingSign(str);
            });
            AddFormatAction((value, sb) => sb.Append(nonNegativePredicate(value) ? positiveSign : negativeSign));
        }

        /// <summary>
        /// Adds parse and format actions for an "negative only" sign.
        /// </summary>
        /// <param name="signSetter">Action to take when to set the given sign within the bucket</param>
        /// <param name="nonNegativePredicate">Predicate to detect whether the value being formatted is non-negative</param>
        public void AddNegativeOnlySign(Action<TBucket, bool> signSetter, Func<TResult, bool> nonNegativePredicate)
        {
            string negativeSign = formatInfo.NegativeSign;
            string positiveSign = formatInfo.PositiveSign;
            AddParseAction((str, bucket) =>
            {
                if (str.Match(negativeSign))
                {
                    signSetter(bucket, false);
                    return null;
                }
                if (str.Match(positiveSign))
                {
                    return ParseResult<TResult>.PositiveSignInvalid(str);
                }
                signSetter(bucket, true);
                return null;
            });
            AddFormatAction((value, builder) =>
            {
                if (!nonNegativePredicate(value))
                {
                    builder.Append(negativeSign);
                }
            });
        }

        internal void AddFormatLeftPad(int count, Func<TResult, int> selector)
        {
            AddFormatAction((value, sb) => FormatHelper.LeftPad(selector(value), count, sb));
        }

        internal void AddFormatFraction(int width, int scale, Func<TResult, int> selector)
        {
            AddFormatAction((value, sb) => FormatHelper.AppendFraction(selector(value), width, scale, sb));
        }

        internal void AddFormatFractionTruncate(int width, int scale, Func<TResult, int> selector)
        {
            AddFormatAction((value, sb) => FormatHelper.AppendFractionTruncate(selector(value), width, scale, sb));
        }

        /// <summary>
        /// Hack to handle genitive month names - we only know what we need to do *after* we've parsed the whole pattern.
        /// </summary>
        internal interface IPostPatternParseFormatAction
        {
            Action<TResult, StringBuilder> BuildFormatAction(PatternFields finalFields);
        }

        private sealed class SteppedPattern : IPartialPattern<TResult>
        {
            private readonly Action<TResult, StringBuilder> formatActions;
            // This will be null if the pattern is only capable of formatting.
            private readonly ParseAction[] parseActions;
            private readonly Func<TBucket> bucketProvider;
            private readonly PatternFields usedFields;

            public SteppedPattern(Action<TResult, StringBuilder> formatActions,
                ParseAction[] parseActions,
                Func<TBucket> bucketProvider, PatternFields usedFields)
            {
                this.formatActions = formatActions;
                this.parseActions = parseActions;
                this.bucketProvider = bucketProvider;
                this.usedFields = usedFields;
            }

            public ParseResult<TResult> Parse(string text)
            {
                if (parseActions == null)
                {
                    return ParseResult<TResult>.FormatOnlyPattern;
                }
                if (text == null)
                {
                    return ParseResult<TResult>.ArgumentNull("text");
                }
                if (text.Length == 0)
                {
                    return ParseResult<TResult>.ValueStringEmpty;
                }

                var valueCursor = new ValueCursor(text);
                // Prime the pump... the value cursor ends up *before* the first character, but
                // our steps always assume it's *on* the right character.
                valueCursor.MoveNext();
                var result = ParsePartial(valueCursor);
                if (!result.Success)
                {
                    return result;
                }
                // Check that we've used up all the text
                if (valueCursor.Current != TextCursor.Nul)
                {
                    return ParseResult<TResult>.ExtraValueCharacters(valueCursor, valueCursor.Remainder);
                }
                return result;
            }

            public string Format(TResult value)
            {
                StringBuilder builder = new StringBuilder();
                // This will call all the actions in the multicast delegate.
                formatActions(value, builder);
                return builder.ToString();
            }

            public ParseResult<TResult> ParsePartial(ValueCursor cursor)
            {
                TBucket bucket = bucketProvider();

                foreach (var action in parseActions)
                {
                    ParseResult<TResult> failure = action(cursor, bucket);
                    if (failure != null)
                    {
                        return failure;
                    }
                }
                return bucket.CalculateValue(usedFields, cursor.Value);
            }

            public StringBuilder AppendFormat(TResult value, StringBuilder builder)
            {
                Preconditions.CheckNotNull(builder, "builder");
                formatActions(value, builder);
                return builder;
            }
        }
    }
}
