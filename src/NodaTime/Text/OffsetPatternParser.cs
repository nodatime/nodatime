// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using NodaTime.Globalization;
using NodaTime.Properties;
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
            { '.', TimePatternHelper.CreatePeriodHandler<Offset, OffsetParseBucket>(3, GetPositiveMilliseconds, (bucket, value) => bucket.Milliseconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<Offset>.TimeSeparatorMismatch) },
            { 'h', (pattern, builder) => { throw new InvalidPatternException(Messages.Parse_Hour12PatternNotSupported, typeof(Offset)); } },
            { 'H', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, GetPositiveHours, (bucket, value) => bucket.Hours = value) },
            { 'm', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, GetPositiveMinutes, (bucket, value) => bucket.Minutes = value) },
            { 's', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, GetPositiveSeconds, (bucket, value) => bucket.Seconds = value) },
            { '+', HandlePlus },
            { '-', HandleMinus },
            { 'f', TimePatternHelper.CreateFractionHandler<Offset, OffsetParseBucket>(3, GetPositiveMilliseconds, (bucket, value) => bucket.Milliseconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<Offset, OffsetParseBucket>(3, GetPositiveMilliseconds, (bucket, value) => bucket.Milliseconds = value) },
            { 'Z', (ignored1, ignored2) => { throw new InvalidPatternException(Messages.Parse_ZPrefixNotAtStartOfPattern); } }
        };

        // These are used to compute the individual (always-positive) components of an offset.
        // For example, an offset of "three and a half hours behind UTC" would have a "positive hours" value
        // of 3, and a "positive minutes" value of 30. The sign is computed elsewhere.
        private static int GetPositiveHours(Offset offset)
        {
            return Math.Abs(offset.Milliseconds) / NodaConstants.MillisecondsPerHour;
        }

        private static int GetPositiveMinutes(Offset offset)
        {
            return (Math.Abs(offset.Milliseconds) % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
        }

        private static int GetPositiveSeconds(Offset offset)
        {
            return (Math.Abs(offset.Milliseconds) % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
        }

        private static int GetPositiveMilliseconds(Offset offset)
        {
            return Math.Abs(offset.Milliseconds) % NodaConstants.MillisecondsPerSecond;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<Offset> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            return ParsePartialPattern(patternText, formatInfo);
        }

        private IPartialPattern<Offset> ParsePartialPattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in OffsetPattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                if (patternCharacter == 'n')
                {
                    return new NumberPattern(formatInfo);
                }
                if (patternCharacter == 'g')
                {
                    return CreateGeneralPattern(formatInfo);
                }
                if (patternCharacter == 'G')
                {
                    return new ZPrefixPattern(CreateGeneralPattern(formatInfo));
                }
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternCharacter, typeof(Offset));
                }
            }
            // This is the only way we'd normally end up in custom parsing land for Z on its own.
            if (patternText == "%Z")
            {
                throw new InvalidPatternException(Messages.Parse_EmptyZPrefixedOffsetPattern);
            }

            // Handle Z-prefix by stripping it, parsing the rest as a normal pattern, then building a special pattern
            // which decides whether or not to delegate.
            bool zPrefix = patternText.StartsWith("Z");

            var patternBuilder = new SteppedPatternBuilder<Offset, OffsetParseBucket>(formatInfo, () => new OffsetParseBucket());
            patternBuilder.ParseCustomPattern(zPrefix ? patternText.Substring(1) : patternText, PatternCharacterHandlers);
            // No need to validate field combinations here, but we do need to do something a bit special
            // for Z-handling.
            IPartialPattern<Offset> pattern = patternBuilder.Build();
            return zPrefix ? new ZPrefixPattern(pattern) : pattern;
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

        private IPartialPattern<Offset> CreateGeneralPattern(NodaFormatInfo formatInfo)
        {
            var patterns = new List<IPartialPattern<Offset>>();
            foreach (char c in "flms")
            {
                patterns.Add(ParsePartialPattern(c.ToString(), formatInfo));
            }
            Func<Offset, IPartialPattern<Offset>> formatter = value => PickGeneralFormatter(value, patterns);
            return new CompositePattern<Offset>(patterns, formatter);
        }

        private static IPartialPattern<Offset> PickGeneralFormatter(Offset value, List<IPartialPattern<Offset>> patterns)
        {
            // Note: this relies on the order in ExpandStandardFormatPattern
            int index;
            // Work out the least significant non-zero part.
            int absoluteMilliseconds = Math.Abs(value.Milliseconds);
            if (absoluteMilliseconds % NodaConstants.MillisecondsPerSecond != 0)
            {
                index = 0;
            }
            else if ((absoluteMilliseconds % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond != 0)
            {
                index = 1;
            }
            else if ((absoluteMilliseconds % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute != 0)
            {
                index = 2;
            }
            else
            {
                index = 3;
            }
            return patterns[index];
        }
        #endregion

        /// <summary>
        /// Pattern which optionally delegates to another, but both parses and formats Offset.Zero as "Z".
        /// </summary>
        private sealed class ZPrefixPattern : IPartialPattern<Offset>
        {
            private readonly IPartialPattern<Offset> fullPattern;

            internal ZPrefixPattern(IPartialPattern<Offset> fullPattern)
            {
                this.fullPattern = fullPattern;
            }

            public ParseResult<Offset> Parse(string text)
            {
                return text == "Z" ? ParseResult<Offset>.ForValue(Offset.Zero) : fullPattern.Parse(text);
            }

            public string Format(Offset value)
            {
                return value == Offset.Zero ? "Z" : fullPattern.Format(value);
            }

            public ParseResult<Offset> ParsePartial(ValueCursor cursor)
            {
                if (cursor.Current == 'Z')
                {
                    cursor.MoveNext();
                    return ParseResult<Offset>.ForValue(Offset.Zero);
                }
                return fullPattern.ParsePartial(cursor);
            }

            public void FormatPartial(Offset value, StringBuilder builder)
            {
                if (value == Offset.Zero)
                {
                    builder.Append("Z");
                }
                else
                {
                    fullPattern.FormatPartial(value, builder);
                }
            }
        }

        private sealed class NumberPattern : IPartialPattern<Offset>
        {
            private readonly NodaFormatInfo formatInfo;
            private readonly int maxLength;

            internal NumberPattern(NodaFormatInfo formatInfo)
            {
                this.formatInfo = formatInfo;
                this.maxLength = Offset.MinValue.Seconds.ToString("N0", formatInfo.NumberFormat).Length;
            }

            public ParseResult<Offset> ParsePartial(ValueCursor cursor)
            {
                int startIndex = cursor.Index;
                // TODO: Do better than this. It's horrible, and may well be invalid
                // for some cultures. Or just remove the NumberPattern from 2.0...
                int longestPossible = Math.Min(maxLength, cursor.Length - cursor.Index);
                for (int length = longestPossible; length >= 0; length--)
                {
                    string candidate = cursor.Value.Substring(cursor.Index, length);
                    int seconds;
                    if (Int32.TryParse(candidate, NumberStyles.Integer | NumberStyles.AllowThousands,
                                        formatInfo.NumberFormat, out seconds))
                    {
                        if (seconds < -NodaConstants.SecondsPerStandardDay ||
                            NodaConstants.SecondsPerStandardDay < seconds)
                        {
                            cursor.Move(startIndex);
                            return ParseResult<Offset>.ValueOutOfRange(cursor, seconds);
                        }
                        cursor.Move(cursor.Index + length);
                        return ParseResult<Offset>.ForValue(Offset.FromSeconds(seconds));
                    }
                }
                cursor.Move(startIndex);
                return ParseResult<Offset>.CannotParseValue(cursor, "n");
            }

            public void FormatPartial(Offset value, StringBuilder builder)
            {
                builder.Append(Format(value));
            }

            public ParseResult<Offset> Parse(string text)
            {
                int seconds;
                if (Int32.TryParse(text, NumberStyles.Integer | NumberStyles.AllowThousands,
                                    formatInfo.NumberFormat, out seconds))
                {
                    if (seconds < -NodaConstants.SecondsPerStandardDay ||
                        NodaConstants.SecondsPerStandardDay < seconds)
                    {
                        return ParseResult<Offset>.ValueOutOfRange(new ValueCursor(text), seconds);
                    }
                    return ParseResult<Offset>.ForValue(Offset.FromSeconds(seconds));
                }
                return ParseResult<Offset>.CannotParseValue(new ValueCursor(text), "n");
            }

            public string Format(Offset value)
            {
                return value.Seconds.ToString("N0", formatInfo.NumberFormat);
            }
        }

        #region Character handlers
        private static void HandlePlus(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddRequiredSign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.Milliseconds >= 0);
        }

        private static void HandleMinus(PatternCursor pattern, SteppedPatternBuilder<Offset, OffsetParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddNegativeOnlySign((bucket, positive) => bucket.IsNegative = !positive, offset => offset.Milliseconds >= 0);
        }
        #endregion

        /// <summary>
        /// Provides a container for the interim parsed pieces of an <see cref="Offset" /> value.
        /// </summary>
        [DebuggerStepThrough]
        private sealed class OffsetParseBucket : ParseBucket<Offset>
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
            /// The milliseconds in the range [0, 999].
            /// </summary>
            internal int Milliseconds;

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
            internal override ParseResult<Offset> CalculateValue(PatternFields usedFields, string text)
            {
                int milliseconds = Hours * NodaConstants.MillisecondsPerHour +
                    Minutes * NodaConstants.MillisecondsPerMinute +
                    Seconds * NodaConstants.MillisecondsPerSecond +
                    Milliseconds;
                if (IsNegative)
                {
                    milliseconds = -milliseconds;
                }
                return ParseResult<Offset>.ForValue(Offset.FromMilliseconds(milliseconds));
            }
        }
    }
}
