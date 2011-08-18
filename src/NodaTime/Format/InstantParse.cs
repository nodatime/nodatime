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
    ///   Provides the implementation for parsing strings into <see cref="Instant" /> values.
    /// </summary>
    /// <remarks>
    ///   The concept and general format for this class comes from the Microsoft system libraries and their
    ///   implementations of parsing of objects like <see cref="int" /> and <see cref="DateTime" />.
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
        /// <param name="value">The string value to parse.</param>
        /// <param name="formatInfo">The <see cref="IFormatProvider" /> to use.</param>
        /// <param name="styles">The <see cref="DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref="Instant" /> value.</returns>
        /// <exception cref="ArgumentNullException">value is <c>null</c>.</exception>
        /// <exception cref="FormatException">value is not a valid <see cref="Instant" /> string.</exception>
        internal static Instant Parse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, styles);
            DoParseMultiple(value, AllFormats, parseResult);
            return parseResult.Value;
        }

        /// <summary>
        ///   Implements the ParseExact methods.
        /// </summary>
        /// <remarks>
        ///   This attempts to parse the value in the given format.
        /// </remarks>
        /// <param name="value">The string value to parse.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="formatInfo">The <see cref="IFormatProvider" /> to use.</param>
        /// <param name="styles">The <see cref="DateTimeParseStyles" /> flags.</param>
        /// <returns>The parsed <see cref="Instant" /> value.</returns>
        /// <exception cref="ArgumentNullException">value or format is <c>null</c>.</exception>
        /// <exception cref="FormatException">value is not a valid <see cref="Instant" /> string.</exception>
        internal static Instant ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, styles);
            DoParse(value, format, parseResult);
            return parseResult.Value;
        }

        /// <summary>
        ///   Implements the ParseExact methods that take multiple formats.
        /// </summary>
        /// <remarks>
        ///   This attempts to parse the value in the given formats. The first one to match is used.
        /// </remarks>
        /// <param name="value"></param>
        /// <param name="formats"></param>
        /// <param name="formatInfo"></param>
        /// <param name="styles"></param>
        /// <returns></returns>
        internal static Instant ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new InstantParseInfo(formatInfo, styles);
            DoParseMultiple(value, formats, parseResult);
            return parseResult.Value;
        }

        internal static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, out result);
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            result = Instant.MinValue;
            var parseResult = new InstantParseInfo(formatInfo, styles);
            try
            {
                DoParse(value, format, parseResult);
                result = parseResult.Value;
                return true;
            }
            catch (FormatException)
            {
                // Do nothing
            }
            return false;
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Instant result)
        {
            result = Instant.MinValue;
            var parseResult = new InstantParseInfo(formatInfo, styles);
            try
            {
                DoParseMultiple(value, formats, parseResult);
                result = parseResult.Value;
                return true;
            }
            catch (FormatException)
            {
                // Do nothing
            }
            return false;
        }

        private static void DoStrictParse(string value, char format, InstantParseInfo parseInfo)
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
                    DoStrictParseGeneral(value, parseInfo);
                    break;
                case 'n':
                case 'd':
                    DoStrictParseNumber(value, parseInfo, format);
                    break;
                default:
                    throw FormatError.UnknownStandardFormat(format, typeof(Instant));
            }
        }

        private static void DoStrictParseGeneral(string value, InstantParseInfo parseInfo)
        {
            string label = value.ToUpperInvariant();
            if (label.Equals(Instant.BeginningOfTimeLabel))
            {
                parseInfo.Value = Instant.MinValue;
                return;
            }
            if (label.Equals(Instant.EndOfTimeLabel))
            {
                parseInfo.Value = Instant.MaxValue;
                return;
            }
            DateTime result;
            if (!DateTime.TryParseExact(value, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", parseInfo.FormatInfo.DateTimeFormat,
                                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out result))
            {
                throw FormatError.CannotParseValue(value, typeof(Instant), "g");
            }
            parseInfo.Value = Instant.FromDateTimeUtc(result);
        }

        private static void DoStrictParseNumber(string value, InstantParseInfo parseInfo, char format)
        {
            const NumberStyles parseStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
            long number;
            if (Int64.TryParse(value, parseStyles, parseInfo.FormatInfo.NumberFormat, out number))
            {
                parseInfo.Value = new Instant(number);
                return;
            }
            throw FormatError.CannotParseValue(value, typeof(Instant), format.ToString());
        }

        private static void DoParse(string value, string format, InstantParseInfo parseInfo)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            if (value.Length == 0)
            {
                throw FormatError.ValueStringEmpty();
            }
            if (format.Length == 0)
            {
                throw FormatError.FormatStringEmpty();
            }
            format = format.Trim();
            if (format.Length > 1)
            {
                throw FormatError.FormatInvalid(format);
            }
            char formatChar = format[0];
            DoStrictParse(value, formatChar, parseInfo);
        }

        private static void DoParseMultiple(string value, string[] formats, InstantParseInfo parseInfo)
        {
            if (formats == null)
            {
                throw new ArgumentNullException("formats");
            }
            if (formats.Length == 0)
            {
                throw FormatError.EmptyFormatsArray();
            }
            foreach (string format in formats)
            {
                if (string.IsNullOrEmpty(format))
                {
                    throw FormatError.FormatElementInvalid();
                }
                try
                {
                    DoParse(value, format, parseInfo);
                    return;
                }
                catch (FormatError.FormatValueException)
                {
                    // do nothing
                }
            }
            throw FormatError.NoMatchingFormat();
        }

        #region Nested type: InstantParseInfo
        internal class InstantParseInfo : ParseInfo
        {
            internal InstantParseInfo(IFormatProvider formatInfo, DateTimeParseStyles parseStyles)
                : base(formatInfo, parseStyles)
            {
            }

            internal Instant Value { get; set; }
        }
        #endregion
    }
}