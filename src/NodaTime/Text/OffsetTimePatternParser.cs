// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Globalization;
using NodaTime.Text.Patterns;
using System.Collections.Generic;

namespace NodaTime.Text
{
    internal sealed class OffsetTimePatternParser : IPatternParser<OffsetTime>
    {
        private readonly OffsetTime templateValue;

        private static readonly Dictionary<char, CharacterHandler<OffsetTime, OffsetTimeParseBucket>> PatternCharacterHandlers =
            new Dictionary<char, CharacterHandler<OffsetTime, OffsetTimeParseBucket>>
        {
            { '%', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandlePercent },
            { '\'', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandleQuote },
            { '\"', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandleQuote },
            { '\\', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandleBackslash },
            { '.', TimePatternHelper.CreatePeriodHandler<OffsetTime, OffsetTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ';', TimePatternHelper.CreateCommaDotHandler<OffsetTime, OffsetTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { ':', (pattern, builder) => builder.AddLiteral(builder.FormatInfo.TimeSeparator, ParseResult<OffsetTime>.TimeSeparatorMismatch) },
            { 'h', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours12, 1, 12, value => value.ClockHourOfHalfDay, (bucket, value) => bucket.Time.Hours12 = value) },
            { 'H', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Hours24, 0, 24, value => value.Hour, (bucket, value) => bucket.Time.Hours24 = value) },
            { 'm', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Minutes, 0, 59, value => value.Minute, (bucket, value) => bucket.Time.Minutes = value) },
            { 's', SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>.HandlePaddedField
                       (2, PatternFields.Seconds, 0, 59, value => value.Second, (bucket, value) => bucket.Time.Seconds = value) },
            { 'f', TimePatternHelper.CreateFractionHandler<OffsetTime, OffsetTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 'F', TimePatternHelper.CreateFractionHandler<OffsetTime, OffsetTimeParseBucket>(9, value => value.NanosecondOfSecond, (bucket, value) => bucket.Time.FractionalSeconds = value) },
            { 't', TimePatternHelper.CreateAmPmHandler<OffsetTime, OffsetTimeParseBucket>(time => time.Hour, (bucket, value) => bucket.Time.AmPm = value) },
            { 'o', HandleOffset },
            { 'l', (cursor, builder) => builder.AddEmbeddedTimePattern(cursor.Current, cursor.GetEmbeddedPattern(), bucket => bucket.Time, value => value.TimeOfDay) },
        };

        internal OffsetTimePatternParser(OffsetTime templateValue)
        {
            this.templateValue = templateValue;
        }

        // Note: public to implement the interface. It does no harm, and it's simpler than using explicit
        // interface implementation.
        public IPattern<OffsetTime> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            // Nullity check is performed in OffsetTimePattern.
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(TextErrorMessages.FormatStringEmpty);
            }

            // Handle standard patterns
            if (patternText.Length == 1)
            {
                return patternText[0] switch
                {
                    'G' => OffsetTimePattern.Patterns.GeneralIsoPatternImpl,
                    'o' => OffsetTimePattern.Patterns.ExtendedIsoPatternImpl,
                    _ => throw new InvalidPatternException(TextErrorMessages.UnknownStandardFormat, patternText[0], typeof(OffsetTime))
                };
            }

            var patternBuilder = new SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket>(formatInfo, () => new OffsetTimeParseBucket(templateValue));
            patternBuilder.ParseCustomPattern(patternText, PatternCharacterHandlers);
            patternBuilder.ValidateUsedFields();
            // Need to reconstruct the template value from the bits...
            return patternBuilder.Build(templateValue);
        }
        
        private static void HandleOffset(PatternCursor pattern,
            SteppedPatternBuilder<OffsetTime, OffsetTimeParseBucket> builder)
        {
            builder.AddField(PatternFields.EmbeddedOffset, pattern.Current);
            string embeddedPattern = pattern.GetEmbeddedPattern();
            var offsetPattern = OffsetPattern.Create(embeddedPattern, builder.FormatInfo).UnderlyingPattern;
            builder.AddEmbeddedPattern(offsetPattern, (bucket, offset) => bucket.Offset = offset, zdt => zdt.Offset);
        }
        
        private sealed class OffsetTimeParseBucket : ParseBucket<OffsetTime>
        {
            internal readonly LocalTimePatternParser.LocalTimeParseBucket Time;
            internal Offset Offset;

            internal OffsetTimeParseBucket(OffsetTime templateValue)
            {
                Time = new LocalTimePatternParser.LocalTimeParseBucket(templateValue.TimeOfDay);
                Offset = templateValue.Offset;
            }

            internal override ParseResult<OffsetTime> CalculateValue(PatternFields usedFields, string text)
            {
                ParseResult<LocalTime> timeResult = Time.CalculateValue(usedFields & PatternFields.AllTimeFields, text);
                if (!timeResult.Success)
                {
                    return timeResult.ConvertError<OffsetTime>();
                }
                LocalTime date = timeResult.Value;
                return ParseResult<OffsetTime>.ForValue(date.WithOffset(Offset));
            }
        }
    }
}
