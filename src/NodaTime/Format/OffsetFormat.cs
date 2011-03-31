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
using System.Text;
using NodaTime.Globalization;

#endregion

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
            var parseInfo = new OffsetParseInfo(value, formatProvider, true, DateTimeParseStyles.None);
            if (string.IsNullOrEmpty(format))
            {
                format = "g";
            }
            if (format.Length == 1)
            {
                return FormatStandard(parseInfo, Char.ToLowerInvariant(format[0]));
            }
            return FormatPattern(parseInfo, format);
        }

        /// <summary>
        ///   Formats the given value based on the custom format pattern given.
        /// </summary>
        /// <param name = "parseInfo">The offset info to format.</param>
        /// <param name = "patternString">The custom foramt pattern.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatPattern(OffsetParseInfo parseInfo, string patternString)
        {
            var pattern = new Pattern(patternString);
            var outputBuffer = new StringBuilder();
            while (pattern.MoveNext())
            {
                int repeatLength;
                switch (pattern.Current)
                {
                    case '+':
                        FormatHelper.FormatSign(parseInfo, true, outputBuffer);
                        break;
                    case '-':
                        FormatHelper.FormatSign(parseInfo, false, outputBuffer);
                        break;
                    case ':':
                        outputBuffer.Append(parseInfo.FormatInfo.TimeSeparator);
                        break;
                    case '.':
                        outputBuffer.Append(parseInfo.FormatInfo.DecimalSeparator);
                        break;
                    case '%':
                        outputBuffer.Append(FormatPattern(parseInfo, pattern.GetNextCharacter().ToString()));
                        break;
                    case '\'':
                    case '"':
                        outputBuffer.Append(pattern.GetQuotedString(parseInfo));
                        break;
                    case '\\':
                        outputBuffer.Append(pattern.GetNextCharacter());
                        break;
                    case 'h':
                    case 'H':
                        repeatLength = pattern.GetRepeatCount(2, parseInfo);
                        FormatHelper.LeftPad(parseInfo.Hours.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 's':
                        repeatLength = pattern.GetRepeatCount(2, parseInfo);
                        FormatHelper.LeftPad(parseInfo.Seconds.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 'm':
                        repeatLength = pattern.GetRepeatCount(2, parseInfo);
                        FormatHelper.LeftPad(parseInfo.Minutes.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 'F':
                        repeatLength = pattern.GetRepeatCount(3, parseInfo);
                        FormatHelper.RightPadTruncate(parseInfo.FractionalSeconds.GetValueOrDefault(), repeatLength, 3, parseInfo.FormatInfo.DecimalSeparator,
                                                      outputBuffer);
                        break;
                    case 'f':
                        repeatLength = pattern.GetRepeatCount(3, parseInfo);
                        FormatHelper.RightPad(parseInfo.FractionalSeconds.GetValueOrDefault(), repeatLength, 3, outputBuffer);
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
        /// <param name = "parseInfo">The offset info to format.</param>
        /// <param name = "formatCharacter">The standard format character.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatStandard(OffsetParseInfo parseInfo, char formatCharacter)
        {
            var formatInfo = parseInfo.FormatInfo;
            string pattern;
            switch (formatCharacter)
            {
                case 'i':
                case 'g':
                    return FormatStandardGeneral(parseInfo, formatInfo);
                case 'n':
                    return parseInfo.Milliseconds.GetValueOrDefault().ToString("N0", formatInfo);
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
            return FormatPattern(parseInfo, pattern);
        }

        /// <summary>
        ///   Determines the format pattern for the "g" specifier based on the value to format.
        /// </summary>
        /// <param name = "parseInfo">The <see cref = "OffsetParseInfo" /> of the value to format.</param>
        /// <param name = "formatInfo">The <see cref = "NodaFormatInfo" /> to use.</param>
        /// <exception cref = "FormatException">if the value cannot be formatted.</exception>
        /// <returns>The formatted string.</returns>
        private static string FormatStandardGeneral(OffsetParseInfo parseInfo, NodaFormatInfo formatInfo)
        {
            string pattern;
            if (parseInfo.FractionalSeconds != 0)
            {
                pattern = formatInfo.OffsetPatternFull;
            }
            else if (parseInfo.Seconds != 0)
            {
                pattern = formatInfo.OffsetPatternLong;
            }
            else if (parseInfo.Minutes != 0)
            {
                pattern = formatInfo.OffsetPatternMedium;
            }
            else
            {
                pattern = formatInfo.OffsetPatternShort;
            }
            return FormatPattern(parseInfo, pattern);
        }
    }
}