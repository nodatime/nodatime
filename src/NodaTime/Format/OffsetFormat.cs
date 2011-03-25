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
using System.Text;
using NodaTime.Globalization;

namespace NodaTime.Format
{
    /// <summary>
    ///   Provides a <see cref = "FormatterBase{T}" /> factory for generating <see cref = "Offset" />
    ///   formatters base on the format string.
    /// </summary>
    internal static class OffsetFormat
    {
        /// <summary>
        ///   Formats the given <see cref = "Offset" /> value using the given format.
        /// </summary>
        /// <param name = "value">The value to format.</param>
        /// <param name = "format">The format string. If <c>null</c> or empty defaults to "g".</param>
        /// <param name = "formatProvider">The <see cref = "NodaFormatInfo" /> to use. Must not be null.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The value formatted as a string.</returns>
        internal static string Format(Offset value, string format, NodaFormatInfo formatProvider)
        {
            var offsetInfo = new OffsetInfo(value, formatProvider);
            if (string.IsNullOrEmpty(format))
            {
                format = "g";
            }
            if (format.Length == 1)
            {
                return FormatStandard(offsetInfo, Char.ToLowerInvariant(format[0]));
            }
            return FormatPattern(offsetInfo, format);
        }

        /// <summary>
        ///   Formats the given value based on the custom format pattern given.
        /// </summary>
        /// <param name = "offsetInfo">The offset info to format.</param>
        /// <param name = "patternString">The custom foramt pattern.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatPattern(OffsetInfo offsetInfo, string patternString)
        {
            var pattern = new Pattern(patternString);
            var outputBuffer = new StringBuilder();
            while (pattern.MoveNext())
            {
                int repeatLength;
                switch (pattern.Current)
                {
                    case '+':
                        FormatHelper.FormatSign(offsetInfo, true, outputBuffer);
                        break;
                    case '-':
                        FormatHelper.FormatSign(offsetInfo, false, outputBuffer);
                        break;
                    case ':':
                        outputBuffer.Append(offsetInfo.Dtfi.TimeSeparator);
                        break;
                    case '.':
                        outputBuffer.Append(offsetInfo.Nfi.NumberDecimalSeparator);
                        break;
                    case '%':
                        outputBuffer.Append(FormatPattern(offsetInfo, pattern.GetNextCharacter().ToString()));
                        break;
                    case '\'':
                    case '"':
                        outputBuffer.Append(pattern.GetQuotedString());
                        break;
                    case '\\':
                        outputBuffer.Append(pattern.GetNextCharacter());
                        break;
                    case 'h':
                    case 'H':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(offsetInfo.Hours, repeatLength, outputBuffer);
                        break;
                    case 's':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(offsetInfo.Seconds, repeatLength, outputBuffer);
                        break;
                    case 'm':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(offsetInfo.Minutes, repeatLength, outputBuffer);
                        break;
                    case 'F':
                        repeatLength = pattern.GetRepeatCount(3);
                        FormatHelper.RightPadTruncate(offsetInfo.FractionalSecond, repeatLength, 3, offsetInfo.Nfi.NumberDecimalSeparator, outputBuffer);
                        break;
                    case 'f':
                        repeatLength = pattern.GetRepeatCount(3);
                        FormatHelper.RightPad(offsetInfo.FractionalSecond, repeatLength, 3, outputBuffer);
                        break;
                    default:
                        outputBuffer.Append(pattern.Current);
                        break;
                }
            }
            return outputBuffer.ToString();
        }

        /// <summary>
        ///   Formats the value using one of the standard format patterns.
        /// </summary>
        /// <param name = "offsetInfo">The offset info to format.</param>
        /// <param name = "formatCharacter">The standard format character.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatStandard(OffsetInfo offsetInfo, char formatCharacter)
        {
            var formatInfo = offsetInfo.FormatProvider;
            string pattern;
            switch (formatCharacter)
            {
                case 'i':
                case 'g':
                    return FormatStandardGeneral(offsetInfo, formatInfo);
                case 'n':
                    return offsetInfo.Milliseconds.ToString("N0", offsetInfo.FormatProvider);
                case 's':
                    pattern = formatInfo.OffsetPatternShort;
                    break;
                case 'm':
                    pattern = formatInfo.OffsetPatternMedium;
                    break;
                case 'l':
                    pattern = formatInfo.OffsetPatternLong;
                    break;
                case 'f':
                    pattern = formatInfo.OffsetPatternFull;
                    break;
                default:
                    throw new FormatException("Invalid format string: unknown flag");
            }
            return FormatPattern(offsetInfo, pattern);
        }

        /// <summary>
        ///   Determines the format pattern for the "g" specifier based on the value to format.
        /// </summary>
        /// <param name = "offsetInfo">The <see cref = "OffsetInfo" /> of the value to format.</param>
        /// <param name = "formatInfo">The <see cref = "NodaFormatInfo" /> to use.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatStandardGeneral(OffsetInfo offsetInfo, NodaFormatInfo formatInfo)
        {
            string pattern;
            if (offsetInfo.FractionalSecond != 0)
            {
                pattern = formatInfo.OffsetPatternFull;
            }
            else if (offsetInfo.Seconds != 0)
            {
                pattern = formatInfo.OffsetPatternLong;
            }
            else if (offsetInfo.Minutes != 0)
            {
                pattern = formatInfo.OffsetPatternMedium;
            }
            else
            {
                pattern = formatInfo.OffsetPatternShort;
            }
            return FormatPattern(offsetInfo, pattern);
        }

        #region Nested type: OffsetInfo
        /// <summary>
        ///   Provides a holder for the parts of an <see cref = "Offset" /> value. This makes formatting simpler.
        /// </summary>
        private struct OffsetInfo : ISignedValue
        {
            /// <summary>
            ///   The <see cref = "DateTimeFormatInfo" /> for the culture being used to format this value.
            /// </summary>
            public readonly DateTimeFormatInfo Dtfi;

            /// <summary>
            ///   The <see cref = "NodaFormatInfo" /> for the culture being used to format this value.
            /// </summary>
            public readonly NodaFormatInfo FormatProvider;

            /// <summary>
            ///   The fractions of a seconds in milliseconds.
            /// </summary>
            public readonly int FractionalSecond;

            /// <summary>
            ///   The hours in the range [0, 23].
            /// </summary>
            public readonly int Hours;

            /// <summary>
            ///   The total millisconds. This is the only value that can be negative.
            /// </summary>
            public readonly int Milliseconds;

            /// <summary>
            ///   The minutes in the range [0, 59].
            /// </summary>
            public readonly int Minutes;

            /// <summary>
            ///   The <see cref = "NumberFormatInfo" /> for the culture being used to format this value.
            /// </summary>
            public readonly NumberFormatInfo Nfi;

            /// <summary>
            ///   The seconds in the range [0, 59].
            /// </summary>
            public readonly int Seconds;

            /// <summary>
            ///   True if this value is negative.
            /// </summary>
            private readonly bool isNegative;

            /// <summary>
            ///   The sign string.
            /// </summary>
            private readonly string sign;

            //public CultureInfo CultureInfo { get { return FormatProvider.CultureInfo; } }

            /// <summary>
            ///   Initializes a new instance of the <see cref = "OffsetInfo" /> struct.
            /// </summary>
            /// <param name = "value">The <see cref = "Offset" /> value.</param>
            /// <param name = "formatProvider">The <see cref = "NodaFormatInfo" /> format provider for the culture
            ///   this value is being formatted in.</param>
            public OffsetInfo(Offset value, NodaFormatInfo formatProvider)
            {
                FormatProvider = formatProvider;
                Dtfi = DateTimeFormatInfo.GetInstance(FormatProvider);
                Nfi = NumberFormatInfo.GetInstance(FormatProvider);
                Milliseconds = value.Milliseconds;
                isNegative = value.IsNegative;
                sign = isNegative ? Nfi.NegativeSign : Nfi.PositiveSign;
                int milliseconds = Math.Abs(value.Milliseconds);
                Hours = value.Hours;
                Minutes = value.Minutes;
                Seconds = value.Seconds;
                FractionalSecond = value.FractionalSeconds;
            }

            #region ISignedValue Members
            /// <summary>
            ///   Gets a value indicating whether this instance is negative.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is negative; otherwise, <c>false</c>.
            /// </value>
            public bool IsNegative { get { return isNegative; } }

            /// <summary>
            ///   Gets the sign.
            /// </summary>
            public string Sign { get { return sign; } }
            #endregion
        }
        #endregion
    }
}