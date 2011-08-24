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
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

namespace NodaTime.Text
{
    /// <summary>
    /// Pattern parsing support for <see cref="Instant" />.
    /// </summary>
    internal sealed class InstantPatternParser : IPatternParser<Instant>
    {

        public PatternParseResult<Instant> ParsePattern(string pattern, Globalization.NodaFormatInfo formatInfo, ParseStyles parseStyles)
        {
            if (pattern == null)
            {
                return PatternParseResult<Instant>.ArgumentNull("format");
            }
            if (pattern.Length == 0)
            {
                return PatternParseResult<Instant>.FormatStringEmpty;
            }
            pattern = pattern.Trim();
            if (pattern.Length > 1)
            {
                return PatternParseResult<Instant>.FormatInvalid(pattern);
            }
            char patternChar = char.ToLowerInvariant(pattern[0]);
            switch (patternChar)
            {
                case 'g':
                    return PatternParseResult<Instant>.ForValue(new GeneralParsePattern(formatInfo, parseStyles));
                case 'n':
                    return PatternParseResult<Instant>.ForValue(new NumberParsePattern(pattern, formatInfo, parseStyles, "N0"));
                case 'd':
                    return PatternParseResult<Instant>.ForValue(new NumberParsePattern(pattern, formatInfo, parseStyles, "D"));
                default:
                    return PatternParseResult<Instant>.UnknownStandardFormat(patternChar);
            }
        }

        // TODO: Create an AbstractParsePattern which contains the ParseStyles and NodaFormatInfo, and performs the appropriate trimming.
        private sealed class GeneralParsePattern : IParsedPattern<Instant>
        {
            private readonly NodaFormatInfo formatInfo;
            private readonly ParseStyles parseStyles;

            internal GeneralParsePattern(NodaFormatInfo formatInfo, ParseStyles parseStyles)
            {
                this.formatInfo = formatInfo;
                this.parseStyles = parseStyles;
            }

            public ParseResult<Instant> Parse(string value)
            {
                if (value == null)
                {
                    return ParseResult<Instant>.ArgumentNull("value");
                }
                if (value.Length == 0)
                {
                    return ParseResult<Instant>.ValueStringEmpty;
                }

                if ((parseStyles & ParseStyles.AllowLeadingWhite) != 0)
                {
                    value = value.TrimStart();
                }
                if ((parseStyles & ParseStyles.AllowTrailingWhite) != 0)
                {
                    value = value.TrimEnd();
                }
                if (value.Equals(Instant.BeginningOfTimeLabel, StringComparison.OrdinalIgnoreCase))
                {
                    return ParseResult<Instant>.ForValue(Instant.MinValue);
                }
                if (value.Equals(Instant.EndOfTimeLabel, StringComparison.OrdinalIgnoreCase))
                {
                    return ParseResult<Instant>.ForValue(Instant.MaxValue);
                }

                DateTime result;
                // TODO: When we've got our own parsers fully working, parse this as a LocalDateTime.
                if (!DateTime.TryParseExact(value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatInfo.DateTimeFormat,
                                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
                {
                    return ParseResult<Instant>.CannotParseValue(value, "g");
                }
                return ParseResult<Instant>.ForValue(Instant.FromDateTimeUtc(result));
            }

            public string Format(Instant value)
            {
                if (value == Instant.MinValue)
                {
                    return Instant.BeginningOfTimeLabel;
                }
                if (value == Instant.MaxValue)
                {
                    return Instant.EndOfTimeLabel;
                }
                // TODO: Use LocalDateTime instead, in the ISO calendar? Benchmark...
                DateTime utc = value.ToDateTimeUtc();
                return string.Format(CultureInfo.InvariantCulture,
                        "{0}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}Z",
                        utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second);
            }
        }

        private sealed class NumberParsePattern : IParsedPattern<Instant>
        {
            private const NumberStyles ParsingNumberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            private readonly NodaFormatInfo formatInfo;
            private readonly string pattern;
            private readonly ParseStyles parseStyles;
            private readonly string systemFormatString;

            internal NumberParsePattern(string pattern, NodaFormatInfo formatInfo, ParseStyles parseStyles, string systemFormatString)
            {
                this.pattern = pattern;
                this.formatInfo = formatInfo;
                this.parseStyles = parseStyles;
                this.systemFormatString = systemFormatString;
            }

            public ParseResult<Instant> Parse(string value)
            {
                if (value == null)
                {
                    return ParseResult<Instant>.ArgumentNull("value");
                }
                if (value.Length == 0)
                {
                    return ParseResult<Instant>.ValueStringEmpty;
                }
                if ((parseStyles & ParseStyles.AllowLeadingWhite) != 0)
                {
                    value = value.TrimStart();
                }
                if ((parseStyles & ParseStyles.AllowTrailingWhite) != 0)
                {
                    value = value.TrimEnd();
                }
                
                long number;
                if (Int64.TryParse(value, ParsingNumberStyles, formatInfo.NumberFormat, out number))
                {
                    return ParseResult<Instant>.ForValue(new Instant(number));
                }
                return ParseResult<Instant>.CannotParseValue(value, pattern);
            }

            public string Format(Instant value)
            {
                return value.Ticks.ToString(systemFormatString, formatInfo);
            }
        }
    }
}
