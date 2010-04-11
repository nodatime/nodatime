#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Format
{
    /// <summary>
    /// Original name: DateTimeFormat.
    /// We could go back to the original name, or alternatives include
    /// DateTimeFormatterFactory or DateTimeFormatters. Or just put these
    /// methods in DateTimeFormatter to start with.
    /// </summary>
    public static partial class DateTimeFormats
    {
        private static readonly Dictionary<string, DateTimeFormatter> formattersCache = new Dictionary<string, DateTimeFormatter>(7);

        /// <summary>
        /// Factory to create a formatter from a pattern string.
        /// </summary>
        /// <param name="pattern">Pattern specification</param>
        /// <returns>The formatter</returns>
        /// <exception cref="ArgumentException">If the pattern is invalid</exception>
        public static DateTimeFormatter ForPattern(string pattern)
        {
            return CreateFormatterForPattern(pattern);
        }

        private static DateTimeFormatter CreateFormatterForPattern(string pattern)
        {
            if (String.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException("Invalid pattern specification");
            }

            lock (formattersCache)
            {
                if (!formattersCache.ContainsKey(pattern))
                {
                    var builder = new DateTimeFormatterBuilder();
                    ParsePatternTo(builder, pattern);
                    formattersCache[pattern] = builder.ToFormatter();
                }
                return formattersCache[pattern];
            }
        }

        private static void ParsePatternTo(DateTimeFormatterBuilder builder, string pattern)
        {
            var length = pattern.Length;
            int[] indexRef = new int[1];

            for (var i = 0; i < length; i++)
            {
                indexRef[0] = i;
                var token = ParseToken(pattern, indexRef);
                i = indexRef[0];

                var tokenLen = token.Length;
                if (tokenLen == 0)
                {
                    break;
                }
                char c = token[0];

                switch (c)
                {
                    case 'G': // era designator (text)
                        builder.AppendEraText();
                        break;
                    case 'C': // century of era (number)
                        builder.AppendCenturyOfEra(tokenLen, tokenLen);
                        break;
                    case 'x': // weekyear (number)
                    case 'y': // year (number)
                    case 'Y': // year of era (number)
                        if (tokenLen == 2)
                        {
                            bool lenientParse = true;

                            // Peek ahead to next token.
                            if (i + 1 < length)
                            {
                                indexRef[0]++;
                                if (IsNumericToken(ParseToken(pattern, indexRef)))
                                {
                                    // If next token is a number, cannot support
                                    // lenient parse, because it will consume digits
                                    // that it should not.
                                    lenientParse = false;
                                }
                                indexRef[0]--;
                            }

                            // Use pivots which are compatible with Java SimpleDateFormat.
                            switch (c)
                            {
                                case 'x':
                                    builder.AppendTwoDigitWeekYear
                                        (new ZonedDateTime().WeekYear - 30, lenientParse);
                                    break;
                                case 'y':
                                case 'Y':
                                default:
                                    builder.AppendTwoDigitYear(new ZonedDateTime().Year - 30, lenientParse);
                                    break;
                            }
                        }
                        else
                        {
                            // Try to support long year values.
                            int maxDigits = 9;

                            // Peek ahead to next token.
                            if (i + 1 < length)
                            {
                                indexRef[0]++;
                                if (IsNumericToken(ParseToken(pattern, indexRef)))
                                {
                                    // If next token is a number, cannot support long years.
                                    maxDigits = tokenLen;
                                }
                                indexRef[0]--;
                            }

                            switch (c)
                            {
                                case 'x':
                                    builder.AppendWeekYear(tokenLen, maxDigits);
                                    break;
                                case 'y':
                                    builder.AppendYear(tokenLen, maxDigits);
                                    break;
                                case 'Y':
                                    builder.AppendYearOfEra(tokenLen, maxDigits);
                                    break;
                            }
                        }
                        break;
                    case 'M': // month of year (text and number)
                        if (tokenLen >= 3)
                        {
                            if (tokenLen >= 4)
                            {
                                builder.AppendMonthOfYearText();
                            }
                            else
                            {
                                builder.AppendMonthOfYearShortText();
                            }
                        }
                        else
                        {
                            builder.AppendMonthOfYear(tokenLen);
                        }
                        break;
                    case 'd': // day of month (number)
                        builder.AppendDayOfMonth(tokenLen);
                        break;
                    case 'a': // am/pm marker (text)
                        builder.AppendHalfDayOfDayText();
                        break;
                    case 'h': // clockhour of halfday (number, 1..12)
                        builder.AppendClockHourOfHalfDay(tokenLen);
                        break;
                    case 'H': // hour of day (number, 0..23)
                        builder.AppendHourOfDay(tokenLen);
                        break;
                    case 'k': // clockhour of day (1..24)
                        builder.AppendClockHourOfDay(tokenLen);
                        break;
                    case 'K': // hour of halfday (0..11)
                        builder.AppendHourOfHalfDay(tokenLen);
                        break;
                    case 'm': // minute of hour (number)
                        builder.AppendMinuteOfHour(tokenLen);
                        break;
                    case 's': // second of minute (number)
                        builder.AppendSecondOfMinute(tokenLen);
                        break;
                    case 'S': // fraction of second (number)
                        builder.AppendFractionOfSecond(tokenLen, tokenLen);
                        break;
                    case 'e': // day of week (number)
                        builder.AppendDayOfWeek(tokenLen);
                        break;
                    case 'E': // dayOfWeek (text)
                        if (tokenLen >= 4)
                        {
                            builder.AppendDayOfWeekText();
                        }
                        else
                        {
                            builder.AppendDayOfWeekShortText();
                        }
                        break;
                    case 'D': // day of year (number)
                        builder.AppendDayOfYear(tokenLen);
                        break;
                    case 'w': // week of weekyear (number)
                        builder.AppendWeekOfWeekYear(tokenLen);
                        break;
                    case 'z': // time zone (text)
                        if (tokenLen >= 4)
                        {
                            builder.AppendTimeZoneName();
                        }
                        else
                        {
                            builder.AppendTimeZoneShortName();
                        }
                        break;
                    case 'Z': // time zone offset
                        if (tokenLen == 1)
                        {
                            builder.AppendTimeZoneOffset(null, false, 2, 2);
                        }
                        else if (tokenLen == 2)
                        {
                            builder.AppendTimeZoneOffset(null, true, 2, 2);
                        }
                        else
                        {
                            builder.AppendTimeZoneId();
                        }
                        break;
                    case '\'': // literal text
                        builder.AppendLiteral(token.Substring(1));
                        break;
                    default:
                        throw new ArgumentException("Illegal pattern component: " + token);
                }
            }
        }

        private static string ParseToken(string pattern, int[] indexRef)
        {
            var sb = new StringBuilder();

            int i = indexRef[0];
            int length = pattern.Length;

            char c = pattern[i];
            if (Char.IsLetter(c))
            {
                sb.Append(c);

                while (i + 1 < length)
                {
                    char peek = pattern[i + 1];
                    if (peek == c)
                    {
                        sb.Append(c);
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                // This will identify token as text.
                sb.Append('\'');

                bool inLiteral = false;

                for (; i < length; i++)
                {
                    c = pattern[i];

                    if (c == '\'')
                    {
                        if (i + 1 < length && pattern[i + 1] == '\'')
                        {
                            // '' is treated as escaped '
                            i++;
                            sb.Append(c);
                        }
                        else
                        {
                            inLiteral = !inLiteral;
                        }
                    }
                    else if (!inLiteral && Char.IsLetter(c))
                    {
                        i--;
                        break;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            indexRef[0] = i;
            return sb.ToString();
        }

        private static bool IsNumericToken(string token)
        {
            int tokenLen = token.Length;
            if (tokenLen > 0)
            {
                char c = token[0];
                switch (c)
                {
                    case 'c': // century (number)
                    case 'C': // century of era (number)
                    case 'x': // weekyear (number)
                    case 'y': // year (number)
                    case 'Y': // year of era (number)
                    case 'd': // day of month (number)
                    case 'h': // hour of day (number, 1..12)
                    case 'H': // hour of day (number, 0..23)
                    case 'm': // minute of hour (number)
                    case 's': // second of minute (number)
                    case 'S': // fraction of second (number)
                    case 'e': // day of week (number)
                    case 'D': // day of year (number)
                    case 'F': // day of week in month (number)
                    case 'w': // week of year (number)
                    case 'W': // week of month (number)
                    case 'k': // hour of day (1..24)
                    case 'K': // hour of day (0..11)
                        return true;
                    case 'M': // month of year (text and number)
                        if (tokenLen <= 2)
                        {
                            return true;
                        }
                        return false;
                }
            }
            return false;
        }
    }
}