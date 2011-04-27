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
using NodaTime.Properties;
using NodaTime.Format.Builder;
#endregion

namespace NodaTime.Format
{
    /// <summary>
    ///   Supports the formatting of <see cref="Offset"/> objects.
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
            var formatter = MakeFormatter(format, formatProvider);
            return formatter.Format(value, formatProvider);
        }

        internal static string FormatX(Offset value, string format, NodaFormatInfo formatProvider)
        {
            var parseInfo = new OffsetParseInfo(value, formatProvider);
            if (string.IsNullOrEmpty(format))
            {
                format = "g";
            }
            if (format.Length == 1)
            {
                return FormatStandard(parseInfo, format[0]);
            }
            return FormatPattern(parseInfo, format);
        }

        internal static INodaFormatter<Offset> MakeFormatter(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "g";
            }
            if (format.Length == 1)
            {
                return MakeFormatStandard(format[0], formatProvider);
            }
            return MakeFormatPattern(format, formatProvider);
        }

        internal static INodaFormatter<Offset> MakeFormatPattern(string format, IFormatProvider formatProvider)
        {
            var builder = new FormatterBuilder<Offset, OffsetParseInfo>();
            var errorInfo = new ParseErrorInfo(formatProvider, true);
            DoMakeFormatPattern(format, errorInfo, builder);
            return builder.Build((value, provider) => new OffsetParseInfo(value, provider));
        }

        internal static void DoMakeFormatPattern(string format, ParseErrorInfo errorInfo, FormatterBuilder<Offset, OffsetParseInfo> builder)
        {
            var pattern = new Pattern(format);
            while (pattern.MoveNext())
            {
                int repeatLength;
                switch (pattern.Current)
                {
                    case '+':
                        builder.AddSign(true, info => info);
                        break;
                    case '-':
                        builder.AddSign(false, info => info);
                        break;
                    case ':':
                        builder.AddTimeSeparator();
                        break;
                    case '.':
                        builder.AddDecimalSeparator();
                        break;
                    case '%':
                        if (pattern.HasMoreCharacters)
                        {
                            if (pattern.PeekNext() != '%')
                            {
                                DoMakeFormatPattern(pattern.GetNextCharacter().ToString(), errorInfo, builder);
                                pattern.MoveNext(); // Eat next character
                                break;
                            }
                            errorInfo.FailParsePercentDoubled();
                        }
                        errorInfo.FailParsePercentAtEndOfString();
                        break;
                    case '\'':
                    case '"':
                        builder.AddString(pattern.GetQuotedString());
                        break;
                    case '\\':
                        if (!pattern.HasMoreCharacters)
                        {
                            errorInfo.FailParseEscapeAtEndOfString();
                        }
                        builder.AddString(pattern.GetNextCharacter().ToString());
                        break;
                    case 'h':
                        errorInfo.FailParse12HourPatternNotSupported(typeof(Offset));
                        break; // Never gets here
                    case 'H':
                        repeatLength = pattern.GetRepeatCount(2);
                        builder.AddLeftPad(repeatLength, info => info.Hours.GetValueOrDefault());
                        break;
                    case 's':
                        repeatLength = pattern.GetRepeatCount(2);
                        builder.AddLeftPad(repeatLength, info => info.Seconds.GetValueOrDefault());
                        break;
                    case 'm':
                        repeatLength = pattern.GetRepeatCount(2);
                        builder.AddLeftPad(repeatLength, info => info.Minutes.GetValueOrDefault());
                        break;
                    case 'F':
                        repeatLength = pattern.GetRepeatCount(3);
                        builder.AddRightPadTruncate(repeatLength, 3, info => info.FractionalSeconds.GetValueOrDefault());
                        break;
                    case 'f':
                        repeatLength = pattern.GetRepeatCount(3);
                        builder.AddRightPad(repeatLength, 3, info => info.FractionalSeconds.GetValueOrDefault());
                        break;
                    default:
                        builder.AddString(pattern.Current.ToString());
                        break;
                }
            }
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
                        if (pattern.HasMoreCharacters)
                        {
                            if (pattern.PeekNext() != '%')
                            {
                                outputBuffer.Append(FormatPattern(parseInfo, pattern.GetNextCharacter().ToString()));
                                pattern.MoveNext(); // Eat next character
                                break;
                            }
                            throw FormatError.PercentDoubled();
                        }
                        throw FormatError.PercentAtEndOfString();
                    case '\'':
                    case '"':
                        outputBuffer.Append(pattern.GetQuotedString());
                        break;
                    case '\\':
                        if (!pattern.HasMoreCharacters)
                        {
                            throw FormatError.EscapeAtEndOfString();
                        }
                        outputBuffer.Append(pattern.GetNextCharacter());
                        break;
                    case 'h':
                        throw FormatError.Hour12PatternNotSupported(typeof(Offset));
                    case 'H':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(parseInfo.Hours.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 's':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(parseInfo.Seconds.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 'm':
                        repeatLength = pattern.GetRepeatCount(2);
                        FormatHelper.LeftPad(parseInfo.Minutes.GetValueOrDefault(), repeatLength, outputBuffer);
                        break;
                    case 'F':
                        repeatLength = pattern.GetRepeatCount(3);
                        FormatHelper.RightPadTruncate(parseInfo.FractionalSeconds.GetValueOrDefault(), repeatLength, 3, parseInfo.FormatInfo.DecimalSeparator,
                                                      outputBuffer);
                        break;
                    case 'f':
                        repeatLength = pattern.GetRepeatCount(3);
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
                    string message = string.Format(Resources.Parse_UnknownStandardFormat, formatCharacter, typeof(Offset).FullName);
                    throw new ParseException(ParseFailureKind.UnknownStandardFormat, message);
            }
            return FormatPattern(parseInfo, pattern);
        }

        private static INodaFormatter<Offset> MakeFormatStandard(char formatCharacter, IFormatProvider formatProvider)
        {
            var formatInfo = NodaFormatInfo.GetInstance(formatProvider);
            string pattern;
            switch (formatCharacter)
            {
                case 'g':
                    return new FormatterG();
                case 'n':
                    return new FormatterN();
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
                    string message = string.Format(Resources.Parse_UnknownStandardFormat, formatCharacter, typeof(Offset).FullName);
                    throw new ParseException(ParseFailureKind.UnknownStandardFormat, message);
            }
            return MakeFormatPattern(pattern, formatProvider);
        }

        private sealed class FormatterN : AbstractNodaFormatter<Offset>
        {
            public override string Format(Offset value, IFormatProvider formatProvider)
            {
                return value.Milliseconds.ToString("N0", formatProvider);
            }
        }

        private sealed class FormatterG : AbstractNodaFormatter<Offset>
        {
            public override string Format(Offset value, IFormatProvider formatProvider)
            {
                var formatInfo = NodaFormatInfo.GetInstance(formatProvider);
                string pattern;
                if (value.FractionalSeconds != 0)
                {
                    pattern = formatInfo.OffsetPatternFull;
                }
                else if (value.Seconds != 0)
                {
                    pattern = formatInfo.OffsetPatternLong;
                }
                else if (value.Minutes != 0)
                {
                    pattern = formatInfo.OffsetPatternMedium;
                }
                else
                {
                    pattern = formatInfo.OffsetPatternShort;
                }
                var formatter = MakeFormatter(pattern, formatProvider);
                return formatter.Format(value);
            }
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