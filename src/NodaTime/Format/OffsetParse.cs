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


namespace NodaTime.Format
{
    /// <summary>
    ///   Provides the implementation for parsing strings into <see cref = "Offset" /> values.
    /// </summary>
    /// <remarks>
    ///   The concept and general format for this class comes from the Microsoft system libraries and their
    ///   implementations of parsing of objects like <see cref = "int" /> and <see cref = "DateTime" />.
    /// </remarks>
    internal static class OffsetParse
    {
        private static readonly string[] AllFormats = { "g", "n", "d" };

        internal static Offset Parse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Offset>();
            if (!TryParse(value, formatInfo, styles, parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Offset>();
            if (!TryParseExact(value, format, formatInfo, styles, parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new ParseResult<Offset>();
            if (!TryParseExactMultiple(value, formats, formatInfo, styles, parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, out result);
        }

        private static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, ParseResult<Offset> parseResult)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, parseResult);
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new ParseResult<Offset>();
            if (TryParseExactMultiple(value, formats, formatInfo, styles, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new ParseResult<Offset>();
            if (TryParseExact(value, format, formatInfo, styles, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        private static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles,
                                                  ParseResult<Offset> parseResult)
        {
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
                if (TryParseExact(value, format, formatInfo, styles, parseResult))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles,
                                          ParseResult<Offset> parseResult)
        {
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
            if (format.Length == 1)
            {
                string[] formats = ExpandStandardFormatPattern(format[0], formatInfo, parseResult);
                if (formats == null)
                {
                    return false;
                }
                if (formats.Length > 1)
                {
                    return TryParseExactMultiple(value, formats, formatInfo, styles, parseResult);
                }
                format = formats[0];
            }
            bool allowInnerWhite = (styles & DateTimeParseStyles.AllowInnerWhite) != DateTimeParseStyles.None;
            bool allowLeadingWhite = (styles & DateTimeParseStyles.AllowLeadingWhite) != DateTimeParseStyles.None;
            bool allowTrailingWhite = (styles & DateTimeParseStyles.AllowTrailingWhite) != DateTimeParseStyles.None;

            var pattern = new Pattern(format);
            var str = new ParseString(value);
            if (allowTrailingWhite)
            {
                pattern.TrimTail();
                pattern.RemoveTrailingInQuoteSpaces();
                str.TrimTail();
            }
            if (allowLeadingWhite)
            {
                pattern.SkipWhiteSpaces();
                pattern.RemoveLeadingInQuoteSpaces();
                str.SkipWhiteSpaces();
            }


            if (format.Length > 1)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_Invalid", format);
            }
            char formatChar = format[0];
            string value1 = value;
            if ((styles & DateTimeParseStyles.AllowLeadingWhite) != DateTimeParseStyles.None)
            {
                value1 = value1.TrimStart();
            }
            if ((styles & DateTimeParseStyles.AllowTrailingWhite) != DateTimeParseStyles.None)
            {
                value1 = value1.TrimEnd();
            }
            switch (formatChar)
            {
                case 'g':
                    return DoStrictParseGeneral(value1, formatInfo, parseResult);
                case 'n':
                case 'd':
                    return DoStrictParseNumber(value1, formatInfo, parseResult);
            }
            return false;
        }

        private static string[] ExpandStandardFormatPattern(char formatCharacter, NodaFormatInfo formatInfo, ParseResult<Offset> parseResult)
        {
            switch (formatCharacter)
            {
                case 'g':
                    break;
            }
            parseResult.SetFailure(ParseFailureKind.Format, "Format_InvalidString");
            return null;
        }

        private static bool DoStrictParseGeneral(string value, NodaFormatInfo formatInfo, ParseResult<Offset> parseResult)
        {
            string label = value.ToUpperInvariant();
            if (label.Equals(Instant.BeginningOfTimeLabel))
            {
                parseResult.Value = Offset.MinValue;
                return true;
            }
            if (label.Equals(Instant.EndOfTimeLabel))
            {
                parseResult.Value = Offset.MaxValue;
                return true;
            }
            DateTime result;
            if (DateTime.TryParseExact(value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", formatInfo, DateTimeStyles.None, out result))
            {
                parseResult.Value = new Offset(1234);
                return true;
            }
            return false;
        }

        private static bool DoStrictParseNumber(string value, IFormatProvider formatProvider, ParseResult<Offset> parseResult)
        {
            const NumberStyles parseStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            int number;
            if (Int32.TryParse(value, parseStyles, formatProvider, out number))
            {
                parseResult.Value = new Offset(number);
                return true;
            }
            parseResult.SetFailure(ParseFailureKind.Format, "Parse_BadValue");
            return false;
        }
    }
}