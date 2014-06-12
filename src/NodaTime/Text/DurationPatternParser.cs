// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    internal sealed class DurationPatternParser : IPatternParser<Duration>
    {
        private static readonly Dictionary<char, CharacterHandler<Duration, DurationParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<Duration, DurationParseBucket>>
        {
            { '%', SteppedPatternBuilder<Duration, DurationParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<Duration, DurationParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<Duration, DurationParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<Duration, DurationParseBucket>.HandleBackslash },
            { '.', TimePatternHelper.CreatePeriodHandler<Duration, DurationParseBucket>(7, GetPositiveTickOfSecond, (bucket, value) => bucket.NegativeTicks -= value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<Duration>.TimeSeparatorMismatch) },
            { 'D', CreateTotalHandler(PatternFields.DayOfMonth, NodaConstants.TicksPerStandardDay) },
            { 'H', CreateTotalHandler(PatternFields.Hours24, NodaConstants.TicksPerHour) },
            { 'h', CreatePartialHandler(PatternFields.Hours24, NodaConstants.TicksPerHour, NodaConstants.HoursPerStandardDay) },
            { 'M', CreateTotalHandler(PatternFields.Minutes, NodaConstants.TicksPerMinute) },
            { 'm', CreatePartialHandler(PatternFields.Minutes, NodaConstants.TicksPerMinute, NodaConstants.MinutesPerHour) },
            { 'S', CreateTotalHandler(PatternFields.Seconds, NodaConstants.TicksPerSecond) },
            { 's', CreatePartialHandler(PatternFields.Seconds, NodaConstants.TicksPerSecond, NodaConstants.SecondsPerMinute) },
            { 'f', TimePatternHelper.CreateFractionHandler<Duration, DurationParseBucket>(7, GetPositiveTickOfSecond, (bucket, value) => bucket.NegativeTicks -= value) },
            { 'F', TimePatternHelper.CreateFractionHandler<Duration, DurationParseBucket>(7, GetPositiveTickOfSecond, (bucket, value) => bucket.NegativeTicks -= value) },
            { '+', HandlePlus },
            { '-', HandleMinus },
        };

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<Duration> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }

            // The sole standard pattern...
            if (patternText.Length == 1)
            {
                switch (patternText[0])
                {
                    case 'o':
                        return DurationPattern.Patterns.RoundtripPatternImpl;
                    default:
                        throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternText[0], typeof(Duration));
                }
            }

            var patternBuilder = new SteppedPatternBuilder<Duration, DurationParseBucket>(formatInfo,
                () => new DurationParseBucket());
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            return patternBuilder.Build();
        }

        private static int GetPositiveTickOfSecond(Duration duration)
        {
            return (int) (GetPositiveTicks(duration) % NodaConstants.TicksPerSecond);
        }

        private static CharacterHandler<Duration, DurationParseBucket> CreateTotalHandler
            (PatternFields field, long ticksPerUnit)
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(10);
                // AddField would throw an inappropriate exception here, so handle it specially.
                if ((builder.UsedFields & PatternFields.TotalDuration) != 0)
                {
                    throw new InvalidPatternException(Messages.Parse_MultipleCapitalDurationFields);
                }
                builder.AddField(field, pattern.Current);
                builder.AddField(PatternFields.TotalDuration, pattern.Current);
                builder.AddParseValueAction(count, 10, pattern.Current, 0, int.MaxValue, (bucket, value) => bucket.NegativeTicks -= value * ticksPerUnit);
                builder.AddFormatLeftPad(count, duration => (int) (GetPositiveTicks(duration) / (ulong)ticksPerUnit) );
            };
        }

        private static CharacterHandler<Duration, DurationParseBucket> CreatePartialHandler
            (PatternFields field, long ticksPerUnit, int unitsPerContainer)
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(2);
                builder.AddField(field, pattern.Current);
                builder.AddParseValueAction(count, 2, pattern.Current, 0, unitsPerContainer - 1,
                    (bucket, value) => bucket.NegativeTicks -= value * ticksPerUnit);
                builder.AddFormatLeftPad(count, duration => (int)((GetPositiveTicks(duration) / (ulong)ticksPerUnit) % (uint)unitsPerContainer));
            };
        }

        private static void HandlePlus(PatternCursor pattern, SteppedPatternBuilder<Duration, DurationParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddRequiredSign((bucket, positive) => bucket.IsNegative = !positive, duration => duration.Ticks >= 0);
        }

        private static void HandleMinus(PatternCursor pattern, SteppedPatternBuilder<Duration, DurationParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddNegativeOnlySign((bucket, positive) => bucket.IsNegative = !positive, duration => duration.Ticks >= 0);
        }

        /// <summary>
        /// Returns the absolute number of ticks in a duration, as a ulong in order to handle long.MinValue sensibly.
        /// </summary>
        private static ulong GetPositiveTicks(Duration duration)
        {
            long ticks = duration.Ticks;
            return ticks == long.MinValue ? long.MaxValue + 1UL : (ulong)Math.Abs(ticks);
        }

        /// <summary>
        /// Provides a container for the interim parsed pieces of an <see cref="Offset" /> value.
        /// </summary>
        [DebuggerStepThrough]
        private sealed class DurationParseBucket : ParseBucket<Duration>
        {
            // This is the negated number of "positive" ticks, so that we can cope with long.MinValue ticks
            // in the original duration. This value will always be negative (or 0).
            internal long NegativeTicks;
            public bool IsNegative;

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>
            internal override ParseResult<Duration> CalculateValue(PatternFields usedFields, string text)
            {
                return ParseResult<Duration>.ForValue(Duration.FromTicks(IsNegative ? NegativeTicks : -NegativeTicks));
            }
        }
    }
}
