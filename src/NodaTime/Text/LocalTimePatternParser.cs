// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System;
using System.Collections.Generic;

namespace NodaTime.Text
{
    /// <summary>
    /// Pattern parser for <see cref="LocalTime"/> values.
    /// </summary>
    internal sealed class LocalTimePatternParser : IPatternParser<LocalTime>
    {
        private readonly LocalTime templateValue;

        private static readonly Dictionary<char, CharacterHandler<LocalTime, LocalTimeParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<LocalTime, LocalTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandleBackslash },
            { '.', TimePatternHelper.CreatePeriodHandler<LocalTime, LocalTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<LocalTime, LocalTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Hours12 = value) },
            { 'H', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, value => value.Hour, (bucket, value) => bucket.Hours24 = value) },
            { 'm', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Minutes = value) },
            { 's', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<LocalTime, LocalTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<LocalTime, LocalTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<LocalTime, LocalTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.AmPm = value) }
        };

        public LocalTimePatternParser(LocalTime templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<LocalTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in LocalTimePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                return patternText[0] switch
                {
                    // Invariant standard patterns return cached implementations.
                    'o' => LocalTimePattern.Patterns.ExtendedIsoPatternImpl,
                    'O' => LocalTimePattern.Patterns.LongExtendedIsoPatternImpl,
                    // Other standard patterns expand the pattern text to the appropriate custom pattern.
                    // Note: we don't just recurse, as otherwise a ShortTimePattern of 't' (for example) would cause a stack overflow.
                    't' => ParseNoStandardExpansion(formatInfo.DateTimeFormat.ShortTimePattern),
                    'T' => ParseNoStandardExpansion(formatInfo.DateTimeFormat.LongTimePattern),
                    'r' => ParseNoStandardExpansion("HH:mm:ss.FFFFFFFFF"),
                    // Unknown standard patterns fail.
                    _ => throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText, typeof(LocalTime))
                };
            }
            return ParseNoStandardExpansion(patternText);

            IPattern<LocalTime> ParseNoStandardExpansion(string patternTextLocal)
            {
                var patternBuilder = new SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>(formatInfo,
                    () => new LocalTimeParseBucket(templateValue));
                patternBuilder.ParseCustomPattern(patternTextLocal, PatternCharacterHandlers);
                patternBuilder.ValidateUsedFields();
                return patternBuilder.Build(templateValue);
            }
        }

        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by LocalDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class LocalTimeParseBucket : ParseBucket<LocalTime>
        {
            internal readonly LocalTime TemplateValue;

            /// <summary>
            /// The fractions of a second in nanoseconds, in the range [0, 999999999]
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
            /// AM (0) or PM (1) - or "take from the template" (2). The latter is used in situations
            /// where we're parsing but there is no AM or PM designator.
            /// </summary>
            internal int AmPm;

            internal LocalTimeParseBucket(LocalTime templateValue)
            {
                this.TemplateValue = templateValue;
                // By copying these out of the template value now, we don't have to use any conditional
                // logic later on.
                Minutes = templateValue.Minute;
                Seconds = templateValue.Second;
                FractionalSeconds = templateValue.NanosecondOfSecond;
            }

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>            
            internal override ParseResult<LocalTime> CalculateValue(PatternFields usedFields, string text) =>
                CalculateValue(usedFields, text, typeof(LocalTime));

            private const PatternFields Hour24MinuteSecond = PatternFields.Hours24 | PatternFields.Minutes | PatternFields.Seconds;
            private const PatternFields AllTimeFieldsExceptFractionalSeconds = PatternFields.AllTimeFields ^ PatternFields.FractionalSeconds;

            internal ParseResult<LocalTime> CalculateValue(PatternFields usedFields, string text, Type eventualResultType)
            {
                // Optimize common situation for ISO values.
                if ((usedFields & AllTimeFieldsExceptFractionalSeconds) == Hour24MinuteSecond)
                {
                    return ParseResult<LocalTime>.ForValue(LocalTime.FromHourMinuteSecondNanosecondTrusted(Hours24, Minutes, Seconds, FractionalSeconds));
                }

                // If this bucket was created from an embedded pattern, it's already been computed.
                if (usedFields.HasAny(PatternFields.EmbeddedTime))
                {
                    return ParseResult<LocalTime>.ForValue(LocalTime.FromHourMinuteSecondNanosecondTrusted(Hours24, Minutes, Seconds, FractionalSeconds));
                }
                
                if (AmPm == 2)
                {
                    AmPm = TemplateValue.Hour / 12;
                }
                ParseResult<LocalTime>? failure = DetermineHour(usedFields, text, out int hour, eventualResultType);
                if (failure != null)
                {
                    return failure;
                }
                return ParseResult<LocalTime>.ForValue(LocalTime.FromHourMinuteSecondNanosecondTrusted(hour, Minutes, Seconds, FractionalSeconds));
            }

            private ParseResult<LocalTime>? DetermineHour(PatternFields usedFields, string text, out int hour, Type eventualResultType)
            {
                hour = 0;
                if (usedFields.HasAny(PatternFields.Hours24))
                {
                    if (usedFields.HasAll(PatternFields.Hours12 | PatternFields.Hours24))
                    {
                        if (Hours12 % 12 != Hours24 % 12)
                        {
                            return ParseResult<LocalTime>.InconsistentValues(text, 'H', 'h', eventualResultType);
                        }
                    }
                    if (usedFields.HasAny(PatternFields.AmPm))
                    {
                        if (Hours24 / 12 != AmPm)
                        {
                            return ParseResult<LocalTime>.InconsistentValues(text, 'H', 't', eventualResultType);
                        }
                    }
                    hour = Hours24;
                    return null;
                }
                // Okay, it's definitely valid - but we've still got 8 possibilities for what's been specified.
                switch (usedFields & (PatternFields.Hours12 | PatternFields.AmPm))
                {
                    case PatternFields.Hours12 | PatternFields.AmPm:
                        hour = (Hours12 % 12) + AmPm * 12;
                        break;
                    case PatternFields.Hours12:
                        // Preserve AM/PM from template value
                        hour = (Hours12 % 12) + (TemplateValue.Hour / 12) * 12;
                        break;
                    case PatternFields.AmPm:
                        // Preserve 12-hour hour of day from template value, use specified AM/PM
                        hour = (TemplateValue.Hour % 12) + AmPm * 12;
                        break;
                    case 0:
                        hour = TemplateValue.Hour;
                        break;
                }
                return null;
            }
        }
    }
}
