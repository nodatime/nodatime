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
using NodaTime.Globalization;
using NodaTime.Properties;

#endregion

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
            var parseResult = new OffsetParseInfo(formatInfo, true, styles);
            TryParseExactMultiple(value, AllFormats, parseResult);
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseInfo(formatInfo, true, styles);
            TryParseExact(value, format, parseResult);
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseInfo(formatInfo, true, styles);
            TryParseExactMultiple(value, formats, parseResult);
            return parseResult.Value;
        }

        internal static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, out result);
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new OffsetParseInfo(formatInfo, false, styles);
            if (TryParseExactMultiple(value, formats, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new OffsetParseInfo(formatInfo, false, styles);
            if (TryParseExact(value, format, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        private static bool TryParseExactMultiple(string value, string[] formats, OffsetParseInfo parseInfo)
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

        internal static bool TryParseExact(string value, string format, OffsetParseInfo parseInfo)
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
            if (format.Length == 1)
            {
                var formats = ExpandStandardFormatPattern(format[0], parseInfo);
                if (formats == null)
                {
                    return false;
                }
                if (formats.Length > 1)
                {
                    return TryParseExactMultiple(value, formats, parseInfo);
                }
                format = formats[0];
            }

            var pattern = new Pattern(format);
            var str = new ParseString(value);
            if (parseInfo.AllowTrailingWhite)
            {
                pattern.TrimTrailingWhiteSpaces();
                pattern.TrimTrailingInQuoteSpaces();
                str.TrimTrailingWhiteSpaces();
            }
            if (parseInfo.AllowLeadingWhite)
            {
                pattern.TrimLeadingWhiteSpaces();
                pattern.TrimLeadingInQuoteSpaces();
                str.TrimLeadingWhiteSpaces();
            }
            str.MoveNext(); // Prime the pump
            while (pattern.HasMoreCharacters)
            {
                if (parseInfo.AllowInnerWhite)
                {
                    str.SkipWhiteSpaces();
                }
                if (!ParseByFormat(str, pattern, parseInfo))
                {
                    return false;
                }
            }
            if (str.Current != Parsable.Nul)
            {
                return parseInfo.SetFormatError(Resources.Parse_ExtraValueCharacters, str.Remainder);
            }

            parseInfo.CalculateValue();

            return true;
        }

        private static bool ParseByFormat(ParseString str, Pattern pattern, OffsetParseInfo parseInfo)
        {
            char patternCharacter = pattern.GetNextCharacter();
            int count;
            int value;
            switch (patternCharacter)
            {
                case '%':
                    if (pattern.HasMoreCharacters)
                    {
                        if (pattern.PeekNext() != '%')
                        {
                            return true;
                        }
                        return parseInfo.SetFormatError(Resources.Parse_PercentDoubled);
                    }
                    return parseInfo.SetFormatError(Resources.Parse_PercentAtEndOfString);
                case '\'':
                case '"':
                    string quoted = pattern.GetQuotedString(patternCharacter, parseInfo);
                    if (parseInfo.Failed)
                    {
                        return false;
                    }
                    for (int i = 0; i < quoted.Length; i++)
                    {
                        if (quoted[i] == ' ' && parseInfo.AllowInnerWhite)
                        {
                            str.SkipWhiteSpaces();
                        }
                        else if (!str.Match(quoted[i]))
                        {
                            return parseInfo.SetFormatError(Resources.Parse_QuotedStringMismatch);
                        }
                    }
                    return true;
                case '\\':
                    if (!pattern.HasMoreCharacters)
                    {
                        return parseInfo.SetFormatError(Resources.Parse_EscapeAtEndOfString);
                    }
                    if (str.Match(pattern.PeekNext()))
                    {
                        pattern.MoveNext();
                        return true;
                    }
                    return parseInfo.SetFormatError(Resources.Parse_EscapedCharacterMismatch, pattern.PeekNext());
                case '.':
                    if (!str.Match(parseInfo.FormatInfo.DecimalSeparator))
                    {
                        if (!pattern.HasMoreCharacters || pattern.PeekNext() != 'F')
                        {
                            return parseInfo.SetFormatError(Resources.Parse_MissingDecimalSeparator);
                        }
                        pattern.MoveNext();
                        pattern.GetRepeatCount(3, parseInfo); // Skip the F pattern characters
                    }
                    return true;
                case ':':
                    if (str.Match(parseInfo.FormatInfo.TimeSeparator))
                    {
                        return true;
                    }
                    return parseInfo.SetFormatError(Resources.Parse_TimeSeparatorMismatch);
                case 'h':
                    throw new FormatException(Resources.Offset_CustomPatternNotSupported);
                case 'H':
                    count = pattern.GetRepeatCount(2, parseInfo);
                    if (!parseInfo.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return parseInfo.AssignNewValue(ref parseInfo.Hours, value, patternCharacter);
                    }
                    return parseInfo.SetFormatError(Resources.Parse_MismatchedNumber, new string(patternCharacter, count));
                case 'm':
                    count = pattern.GetRepeatCount(2, parseInfo);
                    if (!parseInfo.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return parseInfo.AssignNewValue(ref parseInfo.Minutes, value, patternCharacter);
                    }
                    return parseInfo.SetFormatError(Resources.Parse_MismatchedNumber, new string(patternCharacter, count));
                case 's':
                    count = pattern.GetRepeatCount(2, parseInfo);
                    if (!parseInfo.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return parseInfo.AssignNewValue(ref parseInfo.Seconds, value, patternCharacter);
                    }
                    return parseInfo.SetFormatError(Resources.Parse_MismatchedNumber, new string(patternCharacter, count));
                case 'F':
                case 'f':
                    // TDOD: fix the scaling of the value
                    count = pattern.GetRepeatCount(3, parseInfo);
                    if (parseInfo.Failed)
                    {
                        return false;
                    }
                    int fractionalSeconds;
                    if (!str.ParseFractionExact(count, 3, out fractionalSeconds) && patternCharacter == 'f')
                    {
                        return parseInfo.SetFormatError(Resources.Parse_MismatchedNumber, new string(patternCharacter, count));
                    }
                    return parseInfo.AssignNewValue(ref parseInfo.FractionalSeconds, fractionalSeconds, patternCharacter);
                default:
                    if (patternCharacter == ' ')
                    {
                        if (!parseInfo.AllowInnerWhite && !str.Match(patternCharacter))
                        {
                            /*
                            if ((parseInfo.fAllowTrailingWhite && pattern.HasMoreCharacters) && ParseByFormat(str, pattern, formatInfo, false, parseResult))
                            {
                                return true;
                            }
                            */
                            parseInfo.SetFormatError(Resources.Parse_MismatchedSpace);
                            return false;
                        }
                    }
                    else if (!str.Match(patternCharacter))
                    {
                        parseInfo.SetFormatError(Resources.Parse_MismatchedCharacter, patternCharacter);
                        return false;
                    }
                    return true;
            }
        }

        private static string[] ExpandStandardFormatPattern(char formatCharacter, ParseInfo parseInfo)
        {
            switch (formatCharacter)
            {
                case 'g':
                    break;
            }
            parseInfo.SetFormatError(Resources.Parse_UnknownStandardFormat, formatCharacter, typeof(Instant).FullName);
            return null;
        }

        #region Nested type: OffsetParseInfo
        #endregion
    }
}