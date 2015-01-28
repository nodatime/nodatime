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
using JetBrains.Annotations;

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
            { '.', TimePatternHelper.CreatePeriodHandler<Duration, DurationParseBucket>(9, GetPositiveNanosecondOfSecond, (bucket, value) => bucket.AddNanoseconds(value)) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<Duration>.TimeSeparatorMismatch) },
            { 'D', CreateDayHandler() },
            { 'H', CreateTotalHandler(PatternFields.Hours24, NodaConstants.NanosecondsPerHour, NodaConstants.HoursPerDay) },
            { 'h', CreatePartialHandler(PatternFields.Hours24, NodaConstants.NanosecondsPerHour, NodaConstants.HoursPerDay) },
            { 'M', CreateTotalHandler(PatternFields.Minutes, NodaConstants.NanosecondsPerMinute, NodaConstants.MinutesPerDay) },
            { 'm', CreatePartialHandler(PatternFields.Minutes, NodaConstants.NanosecondsPerMinute, NodaConstants.MinutesPerHour) },
            { 'S', CreateTotalHandler(PatternFields.Seconds, NodaConstants.NanosecondsPerSecond, NodaConstants.SecondsPerDay) },
            { 's', CreatePartialHandler(PatternFields.Seconds, NodaConstants.NanosecondsPerSecond, NodaConstants.SecondsPerMinute) },
            { 'f', TimePatternHelper.CreateFractionHandler<Duration, DurationParseBucket>(9, GetPositiveNanosecondOfSecond, (bucket, value) => bucket.AddNanoseconds(value)) },
            { 'F', TimePatternHelper.CreateFractionHandler<Duration, DurationParseBucket>(9, GetPositiveNanosecondOfSecond, (bucket, value) => bucket.AddNanoseconds(value)) },
            { '+', HandlePlus },
            { '-', HandleMinus },
        };

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<Duration> ParsePattern([NotNull] string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, nameof(patternText));
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
            // Somewhat random sample, admittedly...
            return patternBuilder.Build(Duration.FromHours(1) + Duration.FromMinutes(30) + Duration.FromSeconds(5) + Duration.FromMilliseconds(500));
        }

        private static int GetPositiveNanosecondOfSecond(Duration duration)
        {
            return (int) (Math.Abs(duration.NanosecondOfDay) % NodaConstants.NanosecondsPerSecond);
        }

        private static CharacterHandler<Duration, DurationParseBucket> CreateTotalHandler
            (PatternFields field, long nanosecondsPerUnit, int unitsPerDay)
        {
            return (pattern, builder) =>
            {
                // Needs to be big enough for 1449551462400 seconds
                int count = pattern.GetRepeatCount(13);
                // AddField would throw an inappropriate exception here, so handle it specially.
                if ((builder.UsedFields & PatternFields.TotalDuration) != 0)
                {
                    throw new InvalidPatternException(Messages.Parse_MultipleCapitalDurationFields);
                }
                builder.AddField(field, pattern.Current);
                builder.AddField(PatternFields.TotalDuration, pattern.Current);
                // FIXME: This will fail for large numbers of seconds. We probably need specific parse/format
                // code here as it needs to deal with long values :(
                builder.AddParseValueAction(count, 10, pattern.Current, 0, int.MaxValue, (bucket, value) => bucket.AddUnits(value, nanosecondsPerUnit));
                builder.AddFormatLeftPad(count, duration => (int) (GetPositiveNanosecondUnits(duration, nanosecondsPerUnit, unitsPerDay)),
                    assumeNonNegative: true,
                    assumeFitsInCount: false);
            };
        }

        private static CharacterHandler<Duration, DurationParseBucket> CreateDayHandler()
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(10);
                // AddField would throw an inappropriate exception here, so handle it specially.
                if ((builder.UsedFields & PatternFields.TotalDuration) != 0)
                {
                    throw new InvalidPatternException(Messages.Parse_MultipleCapitalDurationFields);
                }
                builder.AddField(PatternFields.DayOfMonth, pattern.Current);
                builder.AddField(PatternFields.TotalDuration, pattern.Current);
                builder.AddParseValueAction(count, 10, pattern.Current, 0, int.MaxValue, (bucket, value) => bucket.AddDays(value));
                builder.AddFormatLeftPad(count, duration => 
                {
                    int days = duration.FloorDays;
                    if (days >= 0)
                    {
                        return days;
                    }
                    // Round towards 0.
                    return duration.NanosecondOfFloorDay == 0 ? -days : -(days + 1);
                },
                assumeNonNegative: true,
                assumeFitsInCount: false);
            };
        }

        private static CharacterHandler<Duration, DurationParseBucket> CreatePartialHandler
            (PatternFields field, long nanosecondsPerUnit, int unitsPerContainer)
        {
            return (pattern, builder) =>
            {
                int count = pattern.GetRepeatCount(2);
                builder.AddField(field, pattern.Current);
                builder.AddParseValueAction(count, 2, pattern.Current, 0, unitsPerContainer - 1,
                    (bucket, value) => bucket.AddUnits(value, nanosecondsPerUnit));
                // This is never used for anything larger than a day, so the day part is irrelevant.
                builder.AddFormatLeftPad(count,
                    duration => (int) (((Math.Abs(duration.NanosecondOfDay) / nanosecondsPerUnit)) % unitsPerContainer),
                    assumeNonNegative: true,
                    assumeFitsInCount: count == 2);
            };
        }

        private static void HandlePlus(PatternCursor pattern, SteppedPatternBuilder<Duration, DurationParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddRequiredSign((bucket, positive) => bucket.IsNegative = !positive, duration => duration.FloorDays >= 0);
        }

        private static void HandleMinus(PatternCursor pattern, SteppedPatternBuilder<Duration, DurationParseBucket> builder)
        {
            builder.AddField(PatternFields.Sign, pattern.Current);
            builder.AddNegativeOnlySign((bucket, positive) => bucket.IsNegative = !positive, duration => duration.FloorDays >= 0);
        }

        private static long GetPositiveNanosecondUnits(Duration duration, long nanosecondsPerUnit, int unitsPerDay)
        {
            if (duration.FloorDays >= 0)
            {
                return duration.FloorDays * unitsPerDay + duration.NanosecondOfFloorDay / nanosecondsPerUnit;
            }
            else
            {
                long nanosecondOfDay = duration.NanosecondOfDay;
                // If it's not an exact number of days, FloorDays will overshoot (negatively) by 1.
                long negativeValue = nanosecondOfDay == 0
                    ? duration.FloorDays * unitsPerDay
                    : (duration.FloorDays + 1) * unitsPerDay + nanosecondOfDay / nanosecondsPerUnit;
                return -negativeValue;
            }
        }

        /// <summary>
        /// Provides a container for the interim parsed pieces of an <see cref="Offset" /> value.
        /// </summary>
        [DebuggerStepThrough]
        private sealed class DurationParseBucket : ParseBucket<Duration>
        {
            // FIXME: This model breaks for Duration.MinValue, which can't be represented as a positive duration :(
            internal bool IsNegative { get; set; }
            private Duration currentValue;

            internal void AddNanoseconds(long nanoseconds)
            {
                this.currentValue = this.currentValue.PlusSmallNanoseconds(nanoseconds);
            }

            internal void AddDays(int days)
            {
                // TODO(2.0): Add a PlusDays method to Duration?
                currentValue = new Duration(currentValue.FloorDays + days, currentValue.NanosecondOfFloorDay);
            }

            internal void AddUnits(int units, long nanosecondsPerUnit)
            {
                // TODO(2.0): Check whether there's a quicker way to do this,
                // possibly by adding something to Duration itself.
                if (units < long.MaxValue / nanosecondsPerUnit)
                {
                    currentValue += Duration.FromNanoseconds(units * nanosecondsPerUnit);
                }
                else
                {
                    currentValue += (Duration.FromNanoseconds(units)) * nanosecondsPerUnit;
                }
            }

            /// <summary>
            /// Calculates the value from the parsed pieces.
            /// </summary>
            internal override ParseResult<Duration> CalculateValue(PatternFields usedFields, string text)
            {
                return ParseResult<Duration>.ForValue(IsNegative ? -currentValue : currentValue);
            }
        }
    }
}
