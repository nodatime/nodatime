// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

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
            { '.', TimePatternHelper.CreatePeriodHandler<LocalTime, LocalTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<LocalTime, LocalTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<LocalTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Hours12 = value) },
            { 'H', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 23, value => value.Hour, (bucket, value) => bucket.Hours24 = value) },
            { 'm', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Minutes = value) },
            { 's', SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<LocalTime, LocalTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<LocalTime, LocalTimeParseBucket>(7, value => value.TickOfSecond, (bucket, value) => bucket.FractionalSeconds = value) },
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
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            if (patternText.Length == 1)
            {
                char patternCharacter = patternText[0];
                patternText = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (patternText == null)
                {
                    throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternCharacter, typeof(LocalTime));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<LocalTime, LocalTimeParseBucket>(formatInfo,
                () => new LocalTimeParseBucket(templateValue));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            return patternBuilder.Build();
        }

        private string ExpandStandardFormatPattern(char patternCharacter, NodaFormatInfo formatInfo)
        {
            switch (patternCharacter)
            {
                case 't':
                    return formatInfo.DateTimeFormat.ShortTimePattern;
                case 'T':
                    return formatInfo.DateTimeFormat.LongTimePattern;
                case 'r':
                    return "HH:mm:ss.FFFFFFF";
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }

        /// <summary>
        /// Bucket to put parsed values in, ready for later result calculation. This type is also used
        /// by LocalDateTimePattern to store and calculate values.
        /// </summary>
        internal sealed class LocalTimeParseBucket : ParseBucket<LocalTime>
        {
            private readonly LocalTime templateValue;

            /// <summary>
            /// The fractions of a second in ticks, in the range [0, 9999999]
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
                this.templateValue = templateValue;
            }

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>            
            internal override ParseResult<LocalTime> CalculateValue(PatternFields usedFields)
            {
                if (AmPm == 2)
                {
                    AmPm = templateValue.Hour / 12;
                }
                int hour;
                ParseResult<LocalTime> failure = DetermineHour(usedFields, out hour);
                if (failure != null)
                {
                    return failure;
                }
                int minutes = IsFieldUsed(usedFields, PatternFields.Minutes) ? Minutes : templateValue.Minute;
                int seconds = IsFieldUsed(usedFields, PatternFields.Seconds) ? Seconds : templateValue.Second;
                int fraction = IsFieldUsed(usedFields, PatternFields.FractionalSeconds) ? FractionalSeconds : templateValue.TickOfSecond;
                return ParseResult<LocalTime>.ForValue(LocalTime.FromHourMinuteSecondTick(hour, minutes, seconds, fraction));
            }

            private ParseResult<LocalTime> DetermineHour(PatternFields usedFields, out int hour)
            {
                hour = 0;
                if (IsFieldUsed(usedFields, PatternFields.Hours24))
                {
                    if (AreAllFieldsUsed(usedFields, PatternFields.Hours12 | PatternFields.Hours24))
                    {
                        if (Hours12 % 12 != Hours24 % 12)
                        {
                            return ParseResult<LocalTime>.InconsistentValues('H', 'h');
                        }
                    }
                    if (IsFieldUsed(usedFields, PatternFields.AmPm))
                    {
                        if (Hours24 / 12 != AmPm)
                        {
                            return ParseResult<LocalTime>.InconsistentValues('H', 't');
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
                        hour = (Hours12 % 12) + (templateValue.Hour / 12) * 12;
                        break;
                    case PatternFields.AmPm:
                        // Preserve 12-hour hour of day from template value, use specified AM/PM
                        hour = (templateValue.Hour % 12) + AmPm * 12;
                        break;
                    case 0:
                        hour = templateValue.Hour;
                        break;
                }
                return null;
            }
        }
    }
}
