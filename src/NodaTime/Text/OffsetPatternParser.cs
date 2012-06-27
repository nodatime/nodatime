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
using NodaTime.Properties;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    internal sealed class OffsetPatternParser : IPatternParser<Offset>
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        private static readonly CharacterHandler<Offset, OffsetParseBucket> DefaultCharacterHandler = SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleDefaultCharacter;

        private static readonly Dictionary<char, CharacterHandler<Offset, OffsetParseBucket>> PatternCharacterHandlers = 
            new Dictionary<char, CharacterHandler<Offset, OffsetParseBucket>>
        {
            { '%', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<Offset, OffsetParseBucket>.HandleBackslash },
            { '.', TimePatternHelper.CreatePeriodHandler<Offset, OffsetParseBucket>(3, value => value.FractionalSeconds, (bucket, value) => bucket.FractionalSeconds = value) },
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
            { 'f', TimePatternHelper.CreateFractionHandler<Offset, OffsetParseBucket>(3, value => value.FractionalSeconds, (bucket, value) => bucket.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<Offset, OffsetParseBucket>(3, value => value.FractionalSeconds, (bucket, value) => bucket.FractionalSeconds = value) },
            { 'Z', (ignored1, ignored2) => PatternParseResult<Offset>.ForInvalidFormat(Messages.Parse_ZPrefixNotAtStartOfPattern) }
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
            // This is the only way we'd normally end up in custom parsing land for Z on its own.
            if (patternText == "%Z")
            {
                return PatternParseResult<Offset>.ForInvalidFormat(Messages.Parse_EmptyZPrefixedOffsetPattern);
            }

            // Handle Z-prefix by stripping it, parsing the rest as a normal pattern, then building a special pattern
            // which decides whether or not to delegate.
            bool zPrefix = patternText.StartsWith("Z");

            var patternBuilder = new SteppedPatternBuilder<Offset, OffsetParseBucket>(formatInfo, () => new OffsetParseBucket());
            var patternCursor = new PatternCursor(zPrefix ? patternText.Substring(1) : patternText);

            // Prime the pump...
            // TODO(Post-V1): Add this to the builder?
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
                    handler = DefaultCharacterHandler;
                }
                PatternParseResult<Offset> possiblePatternParseFailure = handler(patternCursor, patternBuilder);
                if (possiblePatternParseFailure != null)
                {
                    return possiblePatternParseFailure;
                }
            }
            IPattern<Offset> pattern = patternBuilder.Build();
            return PatternParseResult<Offset>.ForValue(zPrefix ? new ZPrefixPattern(pattern) : pattern);
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
        /// Pattern which optionally delegates to another, but both parses and formats Offset.Zero as "Z".
        /// </summary>
        private class ZPrefixPattern : IPattern<Offset>
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
