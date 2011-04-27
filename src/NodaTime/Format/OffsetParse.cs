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

        /// <summary>
        ///   Parses the specified value.
        /// </summary>
        /// <param name = "value">The value.</param>
        /// <param name = "formatInfo">The format info.</param>
        /// <param name = "styles">The styles.</param>
        /// <returns></returns>
        internal static Offset Parse(string value, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseInfo(formatInfo, true, styles);
            DoParseMultiple(value, AllFormats, parseResult);
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseInfo(formatInfo, true, styles);
            DoParse(value, format, parseResult);
            return parseResult.Value;
        }

        internal static Offset ParseExact(string value, string[] formats, NodaFormatInfo formatInfo, DateTimeParseStyles styles)
        {
            var parseResult = new OffsetParseInfo(formatInfo, false, styles);
            DoParseMultiple(value, formats, parseResult);
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
            try
            {
                DoParseMultiple(value, formats, parseResult);
                result = parseResult.Value;
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        internal static bool TryParseExact(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles styles, out Offset result)
        {
            result = Offset.MinValue;
            var parseResult = new OffsetParseInfo(formatInfo, false, styles);
            try
            {
                DoParse(value, format, parseResult);
                result = parseResult.Value;
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static void DoParseMultiple(string value, string[] formats, OffsetParseInfo parseInfo)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (formats == null)
            {
                throw new ArgumentNullException("formats");
            }
            if (value.Length == 0)
            {
                throw FormatError.ValueStringEmpty();
            }
            if (formats.Length == 0)
            {
                throw FormatError.EmptyFormatsArray();
            }
            foreach (string format in formats)
            {
                if (string.IsNullOrEmpty(format))
                {
                    throw FormatError.FormatStringEmpty();
                }
                try
                {
                    DoParse(value, format, parseInfo);
                    return;
                }
                catch (FormatError.FormatValueException)
                {
                    // Do nothing
                }
            }
            throw FormatError.NoMatchingFormat();
        }

        private static void DoParse(string value, string format, OffsetParseInfo parseInfo)
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
            if (format.Length == 1)
            {
                char patternCharacter = format[0];
                if (patternCharacter == 'n')
                {
                    ParseNumber(value, parseInfo);
                    return;
                }
                var formats = ExpandStandardFormatPattern(format[0], parseInfo);
                if (formats == null)
                {
                    return;
                }
                if (formats.Length > 1)
                {
                    DoParseMultiple(value, formats, parseInfo);
                    return;
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
                ParseByFormat(str, pattern, parseInfo);
            }
            if (str.Current != Parsable.Nul)
            {
                throw FormatError.ExtraValueCharacters(str.Remainder);
            }

            parseInfo.CalculateValue();
        }

        private static void ParseNumber(string value, OffsetParseInfo parseInfo)
        {
            int milliseconds;
            if (Int32.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, parseInfo.FormatInfo.NumberFormat, out milliseconds))
            {
                if (milliseconds < -NodaConstants.MillisecondsPerDay || NodaConstants.MillisecondsPerDay < milliseconds)
                {
                    throw FormatError.ValueOutOfRange(milliseconds, typeof(Offset));
                }
                parseInfo.Value = new Offset(milliseconds);
            }
        }

        private static void ParseByFormat(ParseString str, Pattern pattern, OffsetParseInfo parseInfo)
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
                            break;
                        }
                        throw FormatError.PercentDoubled();
                    }
                    throw FormatError.PercentAtEndOfString();
                case '\'':
                case '"':
                    string quoted = pattern.GetQuotedString(patternCharacter);
                    foreach (char character in quoted)
                    {
                        if (character == ' ' && parseInfo.AllowInnerWhite)
                        {
                            str.SkipWhiteSpaces();
                        }
                        else if (!str.Match(character))
                        {
                            throw FormatError.QuotedStringMismatch();
                        }
                    }
                    break;
                case '\\':
                    if (!pattern.HasMoreCharacters)
                    {
                        throw FormatError.EscapeAtEndOfString();
                    }
                    if (str.Match(pattern.PeekNext()))
                    {
                        pattern.MoveNext();
                        break;
                    }
                    throw FormatError.EscapedCharacterMismatch(pattern.PeekNext());
                case '.':
                    if (!str.Match(parseInfo.FormatInfo.DecimalSeparator))
                    {
                        if (!pattern.HasMoreCharacters || pattern.PeekNext() != 'F')
                        {
                            throw FormatError.MissingDecimalSeparator();
                        }
                        pattern.MoveNext();
                        pattern.GetRepeatCount(3); // Skip the F pattern characters
                    }
                    break;
                case ':':
                    if (!str.Match(parseInfo.FormatInfo.TimeSeparator))
                    {
                        throw FormatError.TimeSeparatorMismatch();
                    }
                    break;
                case '+':
                    if (str.Match(parseInfo.FormatInfo.NegativeSign))
                    {
                        parseInfo.IsNegative = true;
                    }
                    else if (str.Match(parseInfo.FormatInfo.PositiveSign))
                    {
                        parseInfo.IsNegative = false;
                    }
                    else
                    {
                        throw FormatError.MissingSign();
                    }
                    break;
                case '-':
                    if (str.Match(parseInfo.FormatInfo.NegativeSign))
                    {
                        parseInfo.IsNegative = true;
                    }
                    else if (str.Match(parseInfo.FormatInfo.PositiveSign))
                    {
                        throw FormatError.PositiveSignInvalid();
                    }
                    else
                    {
                        throw FormatError.MissingSign();
                    }
                    break;
                case 'h':
                    throw FormatError.Hour12PatternNotSupported(typeof(Offset));
                case 'H':
                    count = pattern.GetRepeatCount(2);
                    if (count > 0 && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        parseInfo.AssignNewValue(ref parseInfo.Hours, value, patternCharacter);
                        break;
                    }
                    throw FormatError.MismatchedNumber(new string(patternCharacter, Math.Abs(count)));
                case 'm':
                    count = pattern.GetRepeatCount(2);
                    if (count > 0 && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        parseInfo.AssignNewValue(ref parseInfo.Minutes, value, patternCharacter);
                        break;
                    }
                    throw FormatError.MismatchedNumber(new string(patternCharacter, Math.Abs(count)));
                case 's':
                    count = pattern.GetRepeatCount(2);
                    if (count > 0 && str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        parseInfo.AssignNewValue(ref parseInfo.Seconds, value, patternCharacter);
                        break;
                    }
                    throw FormatError.MismatchedNumber(new string(patternCharacter, Math.Abs(count)));
                case 'F':
                case 'f':
                    // TDOD: fix the scaling of the value
                    count = pattern.GetRepeatCount(3);
                    if (count <= 0)
                    {
                        throw FormatError.MismatchedNumber(new string(patternCharacter, Math.Abs(count)));
                    }
                    int fractionalSeconds;
                    if (!str.ParseFractionExact(count, 3, out fractionalSeconds) && patternCharacter == 'f')
                    {
                        throw FormatError.MismatchedNumber(new string(patternCharacter, count));
                    }
                    parseInfo.AssignNewValue(ref parseInfo.FractionalSeconds, fractionalSeconds, patternCharacter);
                    break;
                default:
                    if (patternCharacter == ' ')
                    {
                        if (!parseInfo.AllowInnerWhite && !str.Match(patternCharacter))
                        {
                            throw FormatError.MismatchedSpace();
                        }
                    }
                    else if (!str.Match(patternCharacter))
                    {
                        throw FormatError.MismatchedCharacter(patternCharacter);
                    }
                    break;
            }
        }

        private static string[] ExpandStandardFormatPattern(char formatCharacter, ParseInfo parseInfo)
        {
            switch (formatCharacter)
            {
                case 'g':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternFull", parseInfo.FormatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternLong", parseInfo.FormatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternMedium", parseInfo.FormatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternShort", parseInfo.FormatInfo.CultureInfo),
                           };
                case 'f':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternFull", parseInfo.FormatInfo.CultureInfo),
                           };
                case 'l':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternLong", parseInfo.FormatInfo.CultureInfo),
                           };
                case 'm':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternMedium", parseInfo.FormatInfo.CultureInfo),
                           };
                case 's':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternShort", parseInfo.FormatInfo.CultureInfo),
                           };
            }
            throw FormatError.UnknownStandardFormat(formatCharacter, typeof(Offset));
        }
    }
}