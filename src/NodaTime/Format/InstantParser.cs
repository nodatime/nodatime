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
using NodaTime.Utility;

namespace NodaTime.Format
{
    /// <summary>
    /// Provides the implementation for parsing strings into <see cref="Instant" /> values.
    /// </summary>
    internal class InstantParser : AbstractNodaParser<Instant>
    {
        // Don't bother with the "d" format here, as it behaves the same as "n".
        private static readonly string[] AllFormats = { "g", "n" };

        internal InstantParser() : base(AllFormats, Instant.MinValue)
        {
        }

        protected override ParseResult<Instant> ParseSingle(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles parseStyles)
        {
            if (value == null)
            {
                return ParseResult<Instant>.ArgumentNull("value");
            }
            if (format == null)
            {
                return ParseResult<Instant>.ArgumentNull("format");
            }
            if (value.Length == 0)
            {
                return ParseResult<Instant>.ValueStringEmpty;
            }
            if (format.Length == 0)
            {
                return ParseResult<Instant>.FormatStringEmpty;
            }
            format = format.Trim();
            if (format.Length > 1)
            {
                throw FormatError.FormatInvalid(format);
            }
            char formatChar = format[0];
            return DoStrictParse(value, formatChar, formatInfo, parseStyles);
        }

        private static ParseResult<Instant> DoStrictParse(string value, char format, NodaFormatInfo formatInfo, DateTimeParseStyles parseStyles)
        {
            if ((parseStyles & DateTimeParseStyles.AllowLeadingWhite) != 0)
            {
                value = value.TrimStart();
            }
            if ((parseStyles & DateTimeParseStyles.AllowTrailingWhite) != 0)
            {
                value = value.TrimEnd();
            }
            switch (format)
            {
                case 'g':
                    return DoStrictParseGeneral(value, formatInfo);
                case 'n':
                case 'd':
                    return DoStrictParseNumber(value, formatInfo, format);
                default:
                    return ParseResult<Instant>.UnknownStandardFormat(format, typeof(Instant));
            }
        }

        private static ParseResult<Instant> DoStrictParseGeneral(string value, NodaFormatInfo formatInfo)
        {
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
                return ParseResult<Instant>.CannotParseValue(value, typeof(Instant), "g");
            }
            return ParseResult<Instant>.ForValue(Instant.FromDateTimeUtc(result));
        }

        private static ParseResult<Instant> DoStrictParseNumber(string value, NodaFormatInfo formatInfo, char format)
        {
            const NumberStyles parseStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            long number;
            if (Int64.TryParse(value, parseStyles, formatInfo.NumberFormat, out number))
            {
                return ParseResult<Instant>.ForValue(new Instant(number));
            }
            return ParseResult<Instant>.CannotParseValue(value, typeof(Instant), format.ToString());
        }
    }
}
