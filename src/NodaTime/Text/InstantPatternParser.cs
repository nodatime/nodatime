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
using System.Globalization;
using NodaTime.Fields;
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

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
        public PatternParseResult<Instant> ParsePattern(string patternText, NodaFormatInfo formatInfo)
        {
            if (patternText == null)
            {
                return PatternParseResult<Instant>.ArgumentNull("patternText");
            }
            if (patternText.Length == 0)
            {
                return PatternParseResult<Instant>.FormatStringEmpty;
            }
            patternText = patternText.Trim();
            if (patternText.Length > 1)
            {
                PatternParseResult<LocalDateTime> localResult =
                    formatInfo.LocalDateTimePatternParser.ParsePattern(patternText);
                if (localResult.Success)
                {
                    var parser = new LocalDateTimePatternAdapter(localResult.GetResultOrThrow());
                    return PatternParseResult<Instant>.ForValue(parser);
                }
                return localResult.WithResultType<Instant>();
            }
            char patternChar = char.ToLowerInvariant(patternText[0]);
            switch (patternChar)
            {
                case 'g':
                    return PatternParseResult<Instant>.ForValue(new GeneralPattern());
                case 'n':
                    return PatternParseResult<Instant>.ForValue(new NumberPattern(formatInfo, patternText, "N0"));
                case 'd':
                    return PatternParseResult<Instant>.ForValue(new NumberPattern(formatInfo, patternText, "D"));
                default:
                    return PatternParseResult<Instant>.UnknownStandardFormat(patternChar);
            }
        }

        private sealed class GeneralPattern : AbstractPattern<Instant>
        {
            private static readonly LocalDateTimePattern LocalParsePattern = LocalDateTimePattern.CreateWithInvariantInfo("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");

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

                var result = LocalParsePattern.Parse(value);
                return result.Success ? ParseResult<Instant>.ForValue(new Instant(result.Value.LocalInstant.Ticks))
                    : result.WithResultType<Instant>();
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
                return LocalParsePattern.Format(new LocalDateTime(new LocalInstant(value.Ticks)));
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
                var result = pattern.Parse(text);
                return result.Success ? ParseResult<Instant>.ForValue(new Instant(result.Value.LocalInstant.Ticks))
                    : result.WithResultType<Instant>();
            }
        }
    }
}
