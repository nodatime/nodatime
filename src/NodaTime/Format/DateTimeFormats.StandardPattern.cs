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
using System.Globalization;
using System.IO;

using NodaTime.Calendars;

namespace NodaTime.Format
{
    public static partial class DateTimeFormats
    {
        internal class StandardPatternFormatter : IDateTimePrinter, IDateTimeParser
        {
            private readonly char standardFormatString;
            private readonly Dictionary<string, DateTimeFormatter> patternToFormatterMap = new Dictionary<string, DateTimeFormatter>();

            public StandardPatternFormatter(char standardFormatString)
            {
                this.standardFormatString = standardFormatString;
            }

            public int EstimatedPrintedLength 
            { 
                get 
                { 
                    return 40; //guess
                } 
            }

            public void PrintTo(TextWriter writer, LocalInstant instant, ICalendarSystem calendarSystem, Offset timezoneOffset, IDateTimeZone dateTimeZone, IFormatProvider provider)
            {
                var formatter = GetFormatter(provider);
                formatter.Printer.PrintTo(writer, instant, calendarSystem, timezoneOffset, dateTimeZone, provider);
            }

            public void PrintTo(TextWriter writer, IPartial partial, IFormatProvider provider)
            {
                var formatter = GetFormatter(provider);
                formatter.Printer.PrintTo(writer, partial, provider);
            }

            public int EstimatedParsedLength
            {
                get
                {
                    return 40; //guess
                }
            }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                var formatter = GetFormatter(bucket.Provider);
                return formatter.Parser.ParseInto(bucket, text, position);
            }

            private DateTimeFormatter GetFormatter(IFormatProvider provider) 
            {
                var pattern = GetPattern(provider);
                lock (patternToFormatterMap)
                {
                    if (!patternToFormatterMap.ContainsKey(pattern))
                    {
                        patternToFormatterMap[pattern] = DateTimeFormats.ForPattern(pattern);
                    }
                    return patternToFormatterMap[pattern];
                }
            }

            private string GetPattern(IFormatProvider provider) 
            {
                DateTimeFormatInfo dtfi = DateTimeFormatInfo.GetInstance(provider);

                switch (standardFormatString)
                {
                    case 'D':
                        return dtfi.LongDatePattern;

                    case 'F':
                        return dtfi.FullDateTimePattern;

                    case 'G':
                        return dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;

                    case 'M':
                    case 'm':
                        return dtfi.MonthDayPattern;

                    case 'O':
                    case 'o':
                        return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

                    case 'R':
                    case 'r':
                        return dtfi.RFC1123Pattern;

                    case 'T':
                        return dtfi.LongTimePattern;

                    case 'U':
                        return dtfi.FullDateTimePattern;

                    case 'd':
                        return dtfi.ShortDatePattern;

                    case 'f':
                        return (dtfi.LongDatePattern + " " + dtfi.ShortTimePattern);

                    case 'g':
                        return dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;

                    case 'Y':
                    case 'y':
                        return dtfi.YearMonthPattern;

                    case 's':
                        return dtfi.SortableDateTimePattern;

                    case 't':
                        return dtfi.ShortTimePattern;

                    case 'u':
                        return dtfi.UniversalSortableDateTimePattern;

                    default:
                        throw new ArgumentException("Invalid standard date and time format string");
                }
            }
        }

        private static readonly Dictionary<char, DateTimeFormatter> mapStandardPatternToDateTimeFormatter = new Dictionary<char, DateTimeFormatter>(19);

        /// <summary>
        /// Factory to create a format from a standard date and time format string.
        /// </summary>
        /// <param name="standardPattern">Standard date and time format string</param>
        /// <returns>The formatter</returns>
        public static DateTimeFormatter ForStandardPattern(char standardPattern)
        {
            return CreateFormatterForStandardPattern(standardPattern);
        }

        private static DateTimeFormatter CreateFormatterForStandardPattern(char standardPattern)
        {
            lock (mapStandardPatternToDateTimeFormatter)
            {
                if (!mapStandardPatternToDateTimeFormatter.ContainsKey(standardPattern))
                {
                    var standardPatternFormatter = new StandardPatternFormatter(standardPattern);
                    mapStandardPatternToDateTimeFormatter[standardPattern] = new DateTimeFormatter(standardPatternFormatter, standardPatternFormatter);
                }
                return mapStandardPatternToDateTimeFormatter[standardPattern];

            }
        }

    }
}
