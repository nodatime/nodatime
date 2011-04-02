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
#region usings
using System;
using System.Globalization;
using NodaTime.Globalization;
using NodaTime.Utility;
using NodaTime.Properties;
#endregion

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
        /// <param name = "formatInfo">The <see cref = "IFormatProvider" /> to use.</param>
        /// <param name = "styles">The <see cref = "DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref = "Instant" /> value.</returns>
        /// <exception cref = "ArgumentNullException">value is <c>null</c>.</exception>
        /// <exception cref = "FormatException">value is not a valid <see cref = "Instant" /> string.</exception>
        internal static Instant Parse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, true, styles);
            TryParse(value, parseResult);
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
        /// <param name = "formatInfo">The <see cref = "IFormatProvider" /> to use.</param>
        /// <param name = "styles">The <see cref = "DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref = "Instant" /> value.</returns>
        /// <exception cref = "ArgumentNullException">value or format is <c>null</c>.</exception>
        /// <exception cref = "FormatException">value is not a valid <see cref = "Instant" /> string.</exception>
        internal static Instant ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, true, styles);
            TryParseExact(value, format, parseResult);
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
        /// <param name = "formatInfo"></param>
        /// <param name = "styles"></param>
        /// <returns></returns>
        internal static Instant ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, true, styles);
            TryParseExactMultiple(value, formats, parseResult);
            return parseResult.Value;
        }

        internal static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, out result);
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            result = Instant.MinValue;
            var parseResult = new InstantParseInfo(formatInfo, false, styles);
            if (TryParseExact(value, format, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            result = Instant.MinValue;
            var parseResult = new InstantParseInfo(formatInfo, false, styles);
            if (TryParseExactMultiple(value, formats, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        private static bool DoStrictParse(string value, char format, InstantParseInfo parseInfo)
        {
            if (parseInfo.AllowLeadingWhite)
            {
                value = value.TrimStart();
            }
            if (parseInfo.AllowTrailingWhite)
            {
                value = value.TrimEnd();
            }
            switch (format)
            {
                case 'g':
                    return DoStrictParseGeneral(value, parseInfo);
                case 'n':
                case 'd':
                    return DoStrictParseNumber(value, parseInfo, format);
            }
            return false;
        }

        private static bool DoStrictParseGeneral(string value, InstantParseInfo parseInfo)
        {
            string label = value.ToUpperInvariant();
            if (label.Equals(Instant.BeginningOfTimeLabel))
            {
                parseInfo.Value = Instant.MinValue;
                return true;
            }
            if (label.Equals(Instant.EndOfTimeLabel))
            {
                parseInfo.Value = Instant.MaxValue;
                return true;
            }
            DateTime result;
            if (DateTime.TryParseExact(value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", parseInfo.FormatInfo.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                parseInfo.Value = SystemConversions.DateTimeToInstant(result);
                return true;
            }
            return false;
        }

        private static bool DoStrictParseNumber(string value, InstantParseInfo parseInfo, char format)
        {
            const NumberStyles parseStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            long number;
            if (Int64.TryParse(value, parseStyles, parseInfo.FormatInfo.NumberFormat, out number))
            {
                parseInfo.Value = new Instant(number);
                return true;
            }
            return parseInfo.SetFormatError(Resources.Parse_CannotParseValue, value, typeof(Instant).FullName, format);
        }

        private static bool TryParse(string value, InstantParseInfo parseInfo)
        {
            return TryParseExactMultiple(value, AllFormats, parseInfo);
        }

        private static bool TryParseExact(string value, string format, InstantParseInfo parseInfo)
        {
            if (value == null)
            {
                return parseInfo.SetArgumentNull("value");
            }
            if (format == null)
            {
                return parseInfo.SetArgumentNull("format");
            }
            if (value.Length == 0)
            {
                return parseInfo.SetFormatError(Resources.Parse_ValueStringEmpty);
            }
            if (format.Length == 0)
            {
                return parseInfo.SetFormatError(Resources.Parse_FormatStringEmpty);
            }
            format = format.Trim();
            if (format.Length > 1)
            {
                return parseInfo.SetFormatError(Resources.Parse_FormatInvalid, format);
            }
            char formatChar = format[0];
            return DoStrictParse(value, formatChar, parseInfo);
        }

        private static bool TryParseExactMultiple(string value, string[] formats, InstantParseInfo parseInfo)
        {
            if (formats == null)
            {
                return parseInfo.SetArgumentNull("formats");
            }
            if (formats.Length == 0)
            {
                return parseInfo.SetFormatError(Resources.Parse_EmptyFormatsArray);
            }
            foreach (string format in formats)
            {
                if (string.IsNullOrEmpty(format))
                {
                    return parseInfo.SetFormatError(Resources.Parse_FormatElementInvalid);
                }
                if (TryParseExact(value, format, parseInfo))
                {
                    return true;
                }
            }
            return false;
        }

        #region Nested type: InstantParseInfo
        internal class InstantParseInfo : ParseInfo
        {
            internal InstantParseInfo(NodaFormatInfo formatInfo, bool throwImmediate, DateTimeParseStyles parseStyles)
                : base(formatInfo, throwImmediate, parseStyles)
            {
            }

            internal Instant Value { get; set; }
        }
        #endregion
    }
}