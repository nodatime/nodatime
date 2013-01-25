// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

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
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                if (patternCharacter == 'n')
                {
                    return CreateNumberPattern(formatInfo);
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
            IPattern<Offset> pattern = patternBuilder.Build();
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

        private IPattern<Offset> CreateGeneralPattern(NodaFormatInfo formatInfo)
        {
            var patterns = new List<IPattern<Offset>>();
            foreach (char c in "flms")
            {
                patterns.Add(ParsePattern(c.ToString(), formatInfo));
            }
            NodaFunc<Offset, string> formatter = value => FormatGeneral(value, patterns);
            return new CompositePattern<Offset>(patterns, formatter);
        }

        private string FormatGeneral(Offset value, List<IPattern<Offset>> patterns)
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
            return patterns[index].Format(value);
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
            NodaFunc<Offset, string> formatter = value => value.Milliseconds.ToString("N0", formatInfo);
            return new SimplePattern<Offset>(parser, formatter);
        }
        #endregion

        /// <summary>
        /// Pattern which optionally delegates to another, but both parses and formats Offset.Zero as "Z".
        /// </summary>
        private sealed class ZPrefixPattern : IPattern<Offset>
        {
            private readonly IPattern<Offset> fullPattern;

            internal ZPrefixPattern(IPattern<Offset> fullPattern)
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
            internal override ParseResult<Offset> CalculateValue(PatternFields usedFields)
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
