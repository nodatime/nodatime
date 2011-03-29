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
using System.Threading;
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
            var parseResult = new OffsetParseResult();
            if (!TryParse(value, formatInfo, styles, parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseResult();
            if (!TryParseExact(value, format, formatInfo, styles, parseResult))
            {
                throw parseResult.GetParseException();
            }
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseResult();
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

        private static bool TryParse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles, OffsetParseResult parseResult)
        {
            return TryParseExactMultiple(value, AllFormats, formatInfo, styles, parseResult);
        }

        internal static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new OffsetParseResult();
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
            var parseResult = new OffsetParseResult();
            if (TryParseExact(value, format, formatInfo, styles, parseResult))
            {
                result = parseResult.Value;
                return true;
            }
            return false;
        }

        private static bool TryParseExactMultiple(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles,
                                                  OffsetParseResult parseResult)
        {
            if (formats == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "format"); // TODO: Use correct message key
            }
            if (formats.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_List_Empty"); // TODO: Use correct message key
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

        private static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, OffsetParseResult parseResult)
        {
            if (value == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "value"); // TODO: Use correct message key
            }
            if (format == null)
            {
                return parseResult.SetFailure(ParseFailureKind.ArgumentNull, "Argument_Null", null, "format"); // TODO: Use correct message key
            }
            if (value.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Value_Empty"); // TODO: Use correct message key
            }
            if (format.Length == 0)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "TryParse_Format_Empty"); // TODO: Use correct message key
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
                pattern.TrimTrailingWhiteSpaces();
                pattern.TrimTrailingInQuoteSpaces();
                str.TrimTrailingWhiteSpaces();
            }
            if (allowLeadingWhite)
            {
                pattern.TrimLeadingWhiteSpaces();
                pattern.TrimLeadingInQuoteSpaces();
                str.TrimLeadingWhiteSpaces();
            }
            while (pattern.MoveNext())
            {
                if (allowInnerWhite)
                {
                    str.SkipWhiteSpaces();
                }
                if (!ParseByFormat(str, pattern, formatInfo, allowInnerWhite, parseResult))
                {
                    return false;
                }
            }
            if (str.HasMoreCharacters)
            {
                return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadOffset"); // TODO: Use correct message key
            }

            return true;
            /*
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
             */
        }

        private static bool ParseByFormat(ParseString str, Pattern pattern, NodaFormatInfo formatInfo, bool allowInnerWhite, OffsetParseResult parseResult)
        {
            char patternCharacter = pattern.GetNextCharacter();
            int count;
            int value;
            switch (patternCharacter)
            {
                case '\'':
                case '"':
                    string quoted = pattern.GetQuotedString(patternCharacter, parseResult);
                    if (parseResult.Failed)
                    {
                        return false;
                    }
                    for (int i = 0; i < quoted.Length; i++)
                    {
                        if (quoted[i] == ' ' && allowInnerWhite)
                        {
                            str.SkipWhiteSpaces();
                        }
                        else if (!str.Match(quoted[i]))
                        {
                            return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadOffset"); // TODO: Use correct message key
                        }
                    }
                    return true;
                case '\\':
                    if (!pattern.HasMoreCharacters)
                    {
                        return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadFormatSpecifier", null); // TODO: Use correct message key
                    }
                    if (str.Match(pattern.Current))
                    {
                        pattern.MoveNext();
                        return true;
                    }
                    return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                case '.':
                    NumberFormatInfo numberFormatInfo = GetNumberFormatInfo(formatInfo);
                    if (!str.Match(numberFormatInfo.NumberDecimalSeparator))
                    {
                        if (!pattern.HasMoreCharacters || pattern.Current != 'F')
                        {
                            return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadOffset"); // TODO: Use correct message key
                        }
                        pattern.GetRepeatCount(2, parseResult); // Skip the F pattern characters
                    }
                    return true;
                case ':':
                    DateTimeFormatInfo dateTimeFormatInfo = GetDateTimeFormatInfo(formatInfo);
                    if (str.Match(dateTimeFormatInfo.TimeSeparator))
                    {
                        return true;
                    }
                    return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadOffset"); // TODO: Use correct message key
                case 'H':
                case 'h':
                    count = pattern.GetRepeatCount(2, parseResult);
                    if (!parseResult.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return CheckNewValue(ref parseResult.Hours, value, patternCharacter, parseResult);
                    }
                    return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                case 'm':
                    count = pattern.GetRepeatCount(2, parseResult);
                    if (!parseResult.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return CheckNewValue(ref parseResult.Minutes, value, patternCharacter, parseResult);
                    }
                    return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                case 's':
                    count = pattern.GetRepeatCount(2, parseResult);
                    if (!parseResult.Failed && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        return CheckNewValue(ref parseResult.Seconds, value, patternCharacter, parseResult);
                    }
                    return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                case 'F':
                case 'f':
                    count = pattern.GetRepeatCount(3, parseResult);
                    if (parseResult.Failed)
                    {
                        return false;
                    }
                    double fraction;
                    if (!str.ParseFractionExact(count, out fraction) && patternCharacter == 'f')
                    {
                        return parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime"); // TODO: Use correct message key
                    }
                    return CheckNewValue(ref parseResult.Fraction, fraction, patternCharacter, parseResult);
                default:
                    if (patternCharacter == ' ')
                    {
                        if (!allowInnerWhite && !str.Match(patternCharacter))
                        {
                            /*
                            if ((parseInfo.fAllowTrailingWhite && pattern.HasMoreCharacters) && ParseByFormat(str, pattern, formatInfo, false, parseResult))
                            {
                                return true;
                            }
                            */
                            parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                            return false;
                        }
                    }
                    else if (!str.Match(patternCharacter))
                    {
                        parseResult.SetFailure(ParseFailureKind.Format, "Format_BadDateTime", null); // TODO: Use correct message key
                        return false;
                    }
                    return true;
            }
        }

        internal class OffsetParseResult : ParseResult
        {
            internal Offset Value { get; set; }
            internal int? Hours;
            public int? Minutes;
            public int? Seconds;
            public double? Fraction;
        }

        private static bool CheckNewValue(ref int? currentValue, int newValue, char patternCharacter, ParseResult result)
        {
            if (currentValue == null)
            {
                currentValue = newValue;
                return true;
            }
            if (newValue != currentValue)
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, "Format_RepeatDateTimePattern", patternCharacter); // TODO: Use correct message key
                return false;
            }
            return true;
        }

        private static bool CheckNewValue(ref double? currentValue, double newValue, char patternCharacter, ParseResult result)
        {
            if (currentValue == null)
            {
                currentValue = newValue;
                return true;
            }
            if (newValue != currentValue)
            {
                result.SetFailure(ParseFailureKind.FormatWithParameter, "Format_RepeatDateTimePattern", patternCharacter); // TODO: Use correct message key
                return false;
            }
            return true;
        }

        private static NumberFormatInfo GetNumberFormatInfo(IFormatProvider provider)
        {
            var numberFormatInfo = provider.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo;
            return numberFormatInfo ?? Thread.CurrentThread.CurrentUICulture.NumberFormat;
        }

        private static DateTimeFormatInfo GetDateTimeFormatInfo(IFormatProvider provider)
        {
            var dateTimeFormatInfo = provider.GetFormat(typeof(DateTimeFormatInfo)) as DateTimeFormatInfo;
            return dateTimeFormatInfo ?? Thread.CurrentThread.CurrentUICulture.DateTimeFormat;
        }

        private static string[] ExpandStandardFormatPattern(char formatCharacter, NodaFormatInfo formatInfo, ParseResult parseResult)
        {
            switch (formatCharacter)
            {
                case 'g':
                    break;
            }
            parseResult.SetFailure(ParseFailureKind.Format, "Format_InvalidString"); // TODO: Use correct message key
            return null;
        }
    }
}