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
    /// Supported patterns:
    /// <list type="bullet">
    ///   <item><description>g: general; the UTC ISO-8601 instant in the style yyyy-MM-ddTHH:mm:ssZ</description></item>
    ///   <item><description>n: numeric; the number of ticks since the epoch using thousands separators</description></item>
    ///   <item><description>d: numeric; the number of ticks since the epoch without thousands separators</description></item>
    /// </list>
    /// </remarks>
    internal sealed class InstantPatternParser : IPatternParser<Instant>
    {
        public IPattern<Instant> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            Preconditions.CheckNotNull(patternText, "patternText");
            if (patternText.Length == 0)
            {
                throw new InvalidPatternException(Messages.Parse_FormatStringEmpty);
            }
            patternText = patternText.Trim();
            if (patternText.Length > 1)
            {
                IPattern<LocalDateTime> localResult = formatInfo.LocalDateTimePatternParser.ParsePattern(patternText);
                return new LocalDateTimePatternAdapter(localResult);
            }
            char patternChar = char.ToLowerInvariant(patternText[0]);
            switch (patternChar)
            {
                case 'g':
                    return new GeneralPattern();
                case 'n':
                    return new NumberPattern(formatInfo, patternText, "N0");
                case 'd':
                    return new NumberPattern(formatInfo, patternText, "D");
                default:
                    throw new InvalidPatternException(Messages.Parse_UnknownStandardFormat, patternChar, typeof(Instant));
            }
        }

        private sealed class GeneralPattern : AbstractPattern<Instant>
        {
            private static readonly LocalDateTimePatternAdapter NonInfinitePattern =
                new LocalDateTimePatternAdapter(
                    LocalDateTimePattern.CreateWithInvariantCulture("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));

            internal GeneralPattern() : base(NodaFormatInfo.InvariantInfo)
            {
            }

            protected override ParseResult<Instant> ParseImpl(string value)
            {
                if (value.Equals(Instant.BeginningOfTimeLabel, StringComparison.OrdinalIgnoreCase))
                {
                    return ParseResult<Instant>.ForValue(Instant.MinValue);
                }
                if (value.Equals(Instant.EndOfTimeLabel, StringComparison.OrdinalIgnoreCase))
                {
                    return ParseResult<Instant>.ForValue(Instant.MaxValue);
                }

                return NonInfinitePattern.Parse(value);
            }

            public override string Format(Instant value)
            {
                if (value == Instant.MinValue)
                {
                    return Instant.BeginningOfTimeLabel;
                }
                if (value == Instant.MaxValue)
                {
                    return Instant.EndOfTimeLabel;
                }
                return NonInfinitePattern.Format(value);
            }
        }

        private sealed class NumberPattern : AbstractPattern<Instant>
        {
            private const NumberStyles ParsingNumberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            private readonly string patternText;
            private readonly string systemFormatString;

            internal NumberPattern(NodaFormatInfo formatInfo, string patternText, string systemFormatString)
                : base(formatInfo)
            {
                this.patternText = patternText;
                this.systemFormatString = systemFormatString;
            }

            protected override ParseResult<Instant> ParseImpl(string value)
            {                
                long number;
                if (Int64.TryParse(value, ParsingNumberStyles, FormatInfo.NumberFormat, out number))
                {
                    return ParseResult<Instant>.ForValue(new Instant(number));
                }
                return ParseResult<Instant>.CannotParseValue(value, patternText);
            }

            public override string Format(Instant value)
            {
                return value.Ticks.ToString(systemFormatString, FormatInfo);
            }
        }

        private sealed class LocalDateTimePatternAdapter : IPattern<Instant>
        {
            private readonly IPattern<LocalDateTime> pattern;

            internal LocalDateTimePatternAdapter(IPattern<LocalDateTime> pattern)
            {
                this.pattern = pattern;
            }

            public string Format(Instant value)
            {
                return pattern.Format(new LocalDateTime(new LocalInstant(value.Ticks)));
            }

            public ParseResult<Instant> Parse(string text)
            {
                return pattern.Parse(text).Convert(local => new Instant(local.LocalInstant.Ticks));
            }
        }
    }
}
