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
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Format
{
    internal class OffsetParser : AbstractNodaParser<Offset>
    {
        private static readonly string[] AllFormats = { "g", "n", "d" };

        internal OffsetParser() : base(AllFormats, Offset.Zero)
        {
        }

        protected override ParseResult<Offset> ParseSingle(string value, string format, NodaFormatInfo formatInfo, DateTimeParseStyles parseStyles)
        {
            if (value == null)
            {
                return ParseResult<Offset>.ArgumentNull("value");
            }
            if (format == null)
            {
                return ParseResult<Offset>.ArgumentNull("format");
            }
            if (value.Length == 0)
            {
                return ParseResult<Offset>.ValueStringEmpty;
            }
            if (format.Length == 0)
            {
                return ParseResult<Offset>.FormatStringEmpty;
            }
            if (format.Length == 1)
            {
                char patternCharacter = format[0];
                if (patternCharacter == 'n')
                {
                    return ParseNumber(value, formatInfo);
                }
                var formats = ExpandStandardFormatPattern(patternCharacter, formatInfo);
                if (formats == null)
                {
                    return ParseResult<Offset>.UnknownStandardFormat(patternCharacter, typeof(Offset));
                }
                if (formats.Length > 1)
                {
                    return ParseMultiple(value, formats, formatInfo, parseStyles);
                }
                format = formats[0];
            }

            var pattern = new NonThrowingPattern(format);
            var str = new ParseString(value);

            OffsetParseInfo parseInfo = new OffsetParseInfo(formatInfo, parseStyles);
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
                if ((parseStyles & DateTimeParseStyles.AllowInnerWhite) != 0)
                {
                    str.SkipWhiteSpaces();
                }
                ParseResult<Offset> possibleErrorResult = ParseByFormat(str, pattern, parseInfo);
                if (possibleErrorResult != null)
                {
                    return possibleErrorResult;
                }
            }
            if (str.Current != Parsable.Nul)
            {
                return ParseResult<Offset>.ExtraValueCharacters(str.Remainder);
            }

            parseInfo.CalculateValue();
            return ParseResult<Offset>.ForValue(parseInfo.Value);
        }

        private static ParseResult<Offset> ParseNumber(string value, NodaFormatInfo formatInfo)
        {
            int milliseconds;
            if (Int32.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, formatInfo.NumberFormat, out milliseconds))
            {
                if (milliseconds < -NodaConstants.MillisecondsPerStandardDay || NodaConstants.MillisecondsPerStandardDay < milliseconds)
                {
                    return ParseResult<Offset>.ValueOutOfRange(milliseconds, typeof(Offset));
                }
                return ParseResult<Offset>.ForValue(Offset.FromMilliseconds(milliseconds));
            }
            return ParseResult<Offset>.CannotParseValue(value, typeof(Offset), "n");
        }

        private static ParseResult<Offset> ParseByFormat(ParseString str, NonThrowingPattern pattern, OffsetParseInfo parseInfo)
        {
            // Used in various pattern calls to get an exception provider.
            ParseResult<Offset> failure = null;
            char patternCharacter = pattern.GetNextCharacter(ref failure);
            if (failure != null)
            {
                return failure;
            }

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
                        return ParseResult<Offset>.PercentDoubled;
                    }
                    return ParseResult<Offset>.PercentAtEndOfString;
                case '\'':
                case '"':
                    string quoted = pattern.GetQuotedString(patternCharacter, ref failure);
                    if (failure != null)
                    {
                        return failure;
                    }
                    foreach (char character in quoted)
                    {
                        if (character == ' ' && parseInfo.AllowInnerWhite)
                        {
                            str.SkipWhiteSpaces();
                        }
                        else if (!str.Match(character))
                        {
                            return ParseResult<Offset>.QuotedStringMismatch;
                        }
                    }
                    break;
                case '\\':
                    if (!pattern.HasMoreCharacters)
                    {
                        return ParseResult<Offset>.EscapeAtEndOfString;
                    }
                    if (str.Match(pattern.PeekNext()))
                    {
                        pattern.MoveNext();
                        break;
                    }
                    return ParseResult<Offset>.EscapedCharacterMismatch(pattern.PeekNext());
                case '.':
                    if (!str.Match(parseInfo.FormatInfo.DecimalSeparator))
                    {
                        if (!pattern.HasMoreCharacters || pattern.PeekNext() != 'F')
                        {
                            return ParseResult<Offset>.MissingDecimalSeparator;
                        }
                        pattern.MoveNext();
                        pattern.GetRepeatCount(3, ref failure); // Skip the F pattern characters
                        if (failure != null)
                        {
                            return failure;
                        }
                    }
                    break;
                case ':':
                    if (!str.Match(parseInfo.FormatInfo.TimeSeparator))
                    {
                        return ParseResult<Offset>.TimeSeparatorMismatch;
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
                        return ParseResult<Offset>.MissingSign;
                    }
                    break;
                case '-':
                    if (str.Match(parseInfo.FormatInfo.NegativeSign))
                    {
                        parseInfo.IsNegative = true;
                    }
                    else if (str.Match(parseInfo.FormatInfo.PositiveSign))
                    {
                        return ParseResult<Offset>.PositiveSignInvalid;
                    }
                    else
                    {
                        return ParseResult<Offset>.MissingSign;
                    }
                    break;
                case 'h':
                    return ParseResult<Offset>.Hour12PatternNotSupported(typeof(Offset));
                case 'H':
                    count = pattern.GetRepeatCount(2, ref failure);
                    if (failure != null)
                    {
                        return failure;
                    }
                    if (str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        if (!ParseInfo.TryAssignNewValue(ref parseInfo.Hours, value, patternCharacter))
                        {
                            return ParseResult<Offset>.DoubleAssigment(patternCharacter);
                        }
                        break;
                    }
                    return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                case 'm':
                    count = pattern.GetRepeatCount(2, ref failure);
                    if (failure != null)
                    {
                        return failure;
                    }
                    if (str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        if (!ParseInfo.TryAssignNewValue(ref parseInfo.Minutes, value, patternCharacter))
                        {
                            return ParseResult<Offset>.DoubleAssigment(patternCharacter);
                        }
                        break;
                    }
                    return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                case 's':
                    count = pattern.GetRepeatCount(2, ref failure);
                    if (failure != null)
                    {
                        return failure;
                    }
                    if (str.ParseDigits(count < 2 ? 1 : 2, 2, out value))
                    {
                        if (!ParseInfo.TryAssignNewValue(ref parseInfo.Seconds, value, patternCharacter))
                        {
                            return ParseResult<Offset>.DoubleAssigment(patternCharacter);
                        }
                        break;
                    }
                    return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                case 'F':
                case 'f':
                    // TODO: fix the scaling of the value
                    count = pattern.GetRepeatCount(3, ref failure);
                    if (failure != null)
                    {
                        return failure;
                    }
                    int fractionalSeconds;
                    if (!str.ParseFractionExact(count, 3, out fractionalSeconds) && patternCharacter == 'f')
                    {
                        return ParseResult<Offset>.MismatchedNumber(new string(patternCharacter, count));
                    }
                    if (!ParseInfo.TryAssignNewValue(ref parseInfo.FractionalSeconds, fractionalSeconds, patternCharacter))
                    {
                        return ParseResult<Offset>.DoubleAssigment(patternCharacter);
                    }
                    break;
                default:
                    if (patternCharacter == ' ')
                    {
                        if (!parseInfo.AllowInnerWhite && !str.Match(patternCharacter))
                        {
                            return ParseResult<Offset>.MismatchedSpace;
                        }
                    }
                    else if (!str.Match(patternCharacter))
                    {
                        return ParseResult<Offset>.MismatchedCharacter(patternCharacter);
                    }
                    break;
            }
            return null;
        }

        private static string[] ExpandStandardFormatPattern(char formatCharacter, NodaFormatInfo formatInfo)
        {
            // TODO: Cache these by culture.
            switch (formatCharacter)
            {
                case 'g':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternFull", formatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternLong", formatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternMedium", formatInfo.CultureInfo),
                               Resources.ResourceManager.GetString("OffsetPatternShort", formatInfo.CultureInfo),
                           };
                case 'f':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternFull", formatInfo.CultureInfo),
                           };
                case 'l':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternLong", formatInfo.CultureInfo),
                           };
                case 'm':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternMedium", formatInfo.CultureInfo),
                           };
                case 's':
                    return new[]
                           {
                               Resources.ResourceManager.GetString("OffsetPatternShort", formatInfo.CultureInfo),
                           };
                default:
                    // Will be turned into an exception.
                    return null;
            }
        }
    }
}
