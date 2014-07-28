// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text.Patterns;
using NodaTime.Utility;

namespace NodaTime.Text
{
    /// <summary>
    /// Pattern parsing support for <see cref="Instant" />.
    /// </summary>
    /// <remarks>
    /// Supported standard patterns:
    /// <list type="bullet">
    ///   <item><description>g: general; the UTC ISO-8601 instant in the style yyyy-MM-ddTHH:mm:ssZ</description></item>
    /// </list>
    /// </remarks>
    internal sealed class InstantPatternParser : IPatternParser<Instant>
    {
        private readonly string minLabel;
        private readonly string maxLabel;
        private const string GeneralPatternText = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";

        internal InstantPatternParser(string minLabel, string maxLabel)
        {
            this.minLabel = minLabel;
            this.maxLabel = maxLabel;
        }

        public IPattern<Instant> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }
            if (patternText.Length == 1)
            {
                switch (patternText)
                {
                    case "g": // Simplest way of handling the general pattern...
                        patternText = GeneralPatternText;
                        break;
                    default:
                        throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternText, typeof(Instant));
                }
            }

            IPattern<LocalDateTime> localResult = formatInfo.LocalDateTimePatternParser.ParsePattern(patternText);
            return new LocalDateTimePatternAdapter(localResult, minLabel, maxLabel);
        }

        // This not only converts between LocalDateTime and Instant; it also handles infinity.
        private sealed class LocalDateTimePatternAdapter : IPattern<Instant>
        {
            private readonly IPattern<LocalDateTime> pattern;
            private readonly string minLabel;
            private readonly string maxLabel;

            internal LocalDateTimePatternAdapter(IPattern<LocalDateTime> pattern, string minLabel, string maxLabel)
            {
                this.pattern = pattern;
                this.minLabel = minLabel;
                this.maxLabel = maxLabel;
            }

            public string Format(Instant value)
            {
                if (value == Instant.MinValue)
                {
                    return minLabel;
                }
                if (value == Instant.MaxValue)
                {
                    return maxLabel;
                }
                // FIXME(2.0): Stop using ticks!
                if (value.Ticks < CalendarSystem.Iso.MinTicks)
                {
                    return string.Format("{0} {1} ticks is earlier than the earliest supported ISO calendar value.",
                        InstantPattern.OutOfRangeLabel, value.Ticks);
                }
                if (value.Ticks > CalendarSystem.Iso.MaxTicks)
                {
                    return string.Format("{0} {1} ticks is later than the latest supported ISO calendar value.",
                        InstantPattern.OutOfRangeLabel, value.Ticks);
                }
                return pattern.Format(value.InUtc().LocalDateTime);
            }

            public ParseResult<Instant> Parse(string text)
            {
                if (text == minLabel)
                {
                    return ParseResult<Instant>.ForValue(Instant.MinValue);
                }
                if (text == maxLabel)
                {
                    return ParseResult<Instant>.ForValue(Instant.MaxValue);
                }
                return pattern.Parse(text).Convert(local => local.ToLocalInstant().MinusZeroOffset());
            }
        }
    }
}
