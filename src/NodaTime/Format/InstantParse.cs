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
    ///   Provides the implementation for parsing strings into <see cref = "Instant" /> values.
    /// </summary>
    /// <remarks>
    ///   The concept and general format for this class comes from the Microsoft system libraries and their
    ///   implementations of parsing of objects like <see cref = "int" /> and <see cref = "DateTime" />.
    /// </remarks>
    internal static class InstantParse
    {
        private static readonly string[] AllFormats = { "g", "n", "d" };

        /// <summary>
        ///   Implements the Parse methods.
        /// </summary>
        /// <remarks>
        ///   This attempts to parse the value in each of the valid formats until one matches.
        /// </remarks>
        /// <param name = "value">The string value to parse.</param>
        /// <param name = "formatProvider">The <see cref = "IFormatProvider" /> to use.</param>
        /// <param name = "styles">The <see cref = "DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref = "Instant" /> value.</returns>
        /// <exception cref = "ArgumentNullException">value is <c>null</c>.</exception>
        /// <exception cref = "FormatException">value is not a valid <see cref = "Instant" /> string.</exception>
        internal static Instant Parse(string value, NodaFormatInfo formatProvider, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Instant>();
            if (!TryParse(value, formatProvider, styles, ref parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        /// <summary>
        ///   Implements the ParseExact methods.
        /// </summary>
        /// <remarks>
        ///   This attempts to parse the value in the given format.
        /// </remarks>
        /// <param name = "value">The string value to parse.</param>
        /// <param name = "format">The format to use.</param>
        /// <param name = "formatProvider">The <see cref = "IFormatProvider" /> to use.</param>
        /// <param name = "styles">The <see cref = "DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref = "Instant" /> value.</returns>
        /// <exception cref = "ArgumentNullException">value or format is <c>null</c>.</exception>
        /// <exception cref = "FormatException">value is not a valid <see cref = "Instant" /> string.</exception>
        internal static Instant ParseExact(string value, string format, NodaFormatInfo formatProvider, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Instant>();
            if (!TryParseExact(value, format, formatProvider, styles, ref parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        /// <summary>
        ///   Implements the ParseExact methods that take multiple formats.
        /// </summary>
        /// <remarks>
        ///   This attempts to parse the value in the given formats. The first one to match is used.
        /// </remarks>
        /// <param name = "value"></param>
        /// <param name = "formats"></param>
        /// <param name = "formatProvider"></param>
        /// <param name = "styles"></param>
        /// <returns></returns>
        internal static Instant ParseExact(string value, string[] formats, NodaFormatInfo formatProvider, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Instant>();
            if (!TryParseExactMultiple(value, formats, formatProvider, styles, ref parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static bool TryParse(string value, NodaFormatInfo formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            return TryParseExactMultiple(value, AllFormats, formatProvider, styles, out result);
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            result = Instant.MinValue;
            var parseResult = new ParseResult<Instant>();
            if (TryParseExact(value, format, formatProvider, styles, ref parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            if (formatProvider == null)
            {
                throw new ArgumentNullException("formatProvider");
            }
            result = Instant.MinValue;
            var parseResult = new ParseResult<Instant>();
            if (TryParseExactMultiple(value, formats, formatProvider, styles, ref parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        private static bool DoStrictParse(string value, char format, NodaFormatInfo formatProvider, DateTimeParseStyles styles,
                                          ref ParseResult<Instant> parseResult)
        {
            if ((styles & DateTimeParseStyles.AllowLeadingWhite) != DateTimeParseStyles.None)
            {
                value = value.TrimStart();
            }
            if ((styles & DateTimeParseStyles.AllowTrailingWhite) != DateTimeParseStyles.None)
            {
                value = value.TrimEnd();
            }
            switch (format)
            {
                case 'g':
                    return DoStrictParseGeneral(value, formatProvider, ref parseResult);
                case 'n':
                case 'd':
                    return DoStrictParseNumber(value, formatProvider, ref parseResult);
            }
            return false;
        }

        private static bool DoStrictParseGeneral(string value, NodaFormatInfo formatProvider, ref ParseResult<Instant> parseResult)
        {
            string label = value.ToUpperInvariant();
            if (label.Equals(Instant.BeginningOfTimeLabel))
            {
                parseResult.Value = Instant.MinValue;
                return true;
            }
            if (label.Equals(Instant.EndOfTimeLabel))
            {
                parseResult.Value = Instant.MaxValue;
                return true;
            }
            DateTime result;
            if (DateTime.TryParseExact(value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatProvider,
                                       DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                parseResult.Value = SystemConversions.DateTimeToInstant(result);
                return true;
            }
            return false;
        }

        private static bool DoStrictParseNumber(string value, NodaFormatInfo formatProvider, ref ParseResult<Instant> parseResult)
        {
            const NumberStyles parseStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            long number;
            if (Int64.TryParse(value, parseStyles, formatProvider, out number))
            {
                parseResult.Value = new Instant(number);
                return true;
            }
            parseResult.SetFailure(ParseFailureKind.Format, "Parse_BadValue");
            return false;
        }

        private static bool TryParse(string value, NodaFormatInfo formatProvider, DateTimeParseStyles styles, ref ParseResult<Instant> parseResult)
        {
            return TryParseExactMultiple(value, AllFormats, formatProvider, styles, ref parseResult);
        }

        private static bool TryParseExact(string value, string format, NodaFormatInfo formatProvider, DateTimeParseStyles styles,
                                          ref ParseResult<Instant> parseResult)
        {
            if (formatProvider == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "formatProvider");
            }
            if (value == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "value");
            }
            if (format == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "format");
            }
            if (value.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Value_Empty");
            }
            if (format.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_Empty");
            }
            format = format.Trim();
            if (format.Length > 1)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_Invalid", format);
            }
            char formatChar = format[0];
            return DoStrictParse(value, formatChar, formatProvider, styles, ref parseResult);
        }

        private static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatProvider, DateTimeParseStyles styles,
                                                  ref ParseResult<Instant> parseResult)
        {
            if (formatProvider == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "formatProvider");
            }
            if (formats == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "format");
            }
            if (formats.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_List_Empty");
            }
            foreach (string format in formats)
            {
                if (string.IsNullOrEmpty(format))
                {
                    parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_BadFormatSpecifier", null);
                    return false;
                }
                if (TryParseExact(value, format, formatProvider, styles, ref parseResult))
                {
                    return true;
                }
            }
            return false;
        }
    }
}