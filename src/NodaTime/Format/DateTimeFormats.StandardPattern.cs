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
                get { return 40; //guess
                }
            }

            public void PrintTo(TextWriter writer, LocalInstant instant, CalendarSystem calendarSystem, Offset timezoneOffset, DateTimeZone dateTimeZone,
                                IFormatProvider provider)
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
                get { return 40; //guess
                }
            }

            public int ParseInto(DateTimeParserBucket bucket, string text, int position)
            {
                var formatter = GetFormatter(bucket.Provider);
                return formatter.Parser.ParseInto(bucket, text, position);
            }

            private DateTimeFormatter GetFormatter(IFormatProvider provider)
            {
                var pattern = PatternForStandardPattern(standardFormatString, provider);
                lock (patternToFormatterMap)
                {
                    if (!patternToFormatterMap.ContainsKey(pattern))
                    {
                        patternToFormatterMap[pattern] = ForPattern(pattern);
                    }
                    return patternToFormatterMap[pattern];
                }
            }
        }

        private static readonly Dictionary<char, DateTimeFormatter> mapStandardPatternToDateTimeFormatter = new Dictionary<char, DateTimeFormatter>(19);

        /// <summary>
        /// Gets a format that outputs a short date format.
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>
        public static DateTimeFormatter ShortDate { get { return CreateFormatterForStandardPattern('d'); } }

        /// <summary>
        /// Gets a format that outputs a long date format.
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter LongDate { get { return CreateFormatterForStandardPattern('D'); } }

        /// <summary>
        /// Gets a format that outputs a full date/time pattern (short time)
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter FullShortTime { get { return CreateFormatterForStandardPattern('f'); } }

        /// <summary>
        /// Gets a format that outputs a full date/time pattern (long time)
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter FullLongTime { get { return CreateFormatterForStandardPattern('F'); } }

        /// <summary>
        /// Gets a format that outputs a general date/time pattern (short time)
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter GeneralShortTime { get { return CreateFormatterForStandardPattern('g'); } }

        /// <summary>
        /// Gets a format that outputs a general date/time pattern (long time)
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter GeneralLongTime { get { return CreateFormatterForStandardPattern('G'); } }

        /// <summary>
        /// Gets a format that outputs a month day pattern
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter MonthDay { get { return CreateFormatterForStandardPattern('m'); } }

        /// <summary>
        /// Gets a format that outputs a short time pattern
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter ShortTime { get { return CreateFormatterForStandardPattern('t'); } }

        /// <summary>
        /// Gets a format that outputs a long time pattern
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter LongTime { get { return CreateFormatterForStandardPattern('T'); } }

        /// <summary>
        /// Gets a format that outputs a year month pattern
        /// </summary>
        /// <remarks>
        /// The format will change as you change the format provider of the formatter.
        /// Call <see cref="DateTimeFormatter.WithProvider"/> to switch the format provider.
        /// </remarks>        
        public static DateTimeFormatter YearMonth { get { return CreateFormatterForStandardPattern('Y'); } }

        /// <summary>
        /// Factory to create a format from a standard date and time pattern.
        /// </summary>
        /// <param name="standardPattern">Standard date and time pattern</param>
        /// <returns>The formatter</returns>
        public static DateTimeFormatter ForStandardPattern(char standardPattern)
        {
            return CreateFormatterForStandardPattern(standardPattern);
        }

        /// <summary>
        /// Returns the pattern used by a particular standard pattern and format provider.
        /// </summary>
        /// <param name="standardPattern">Standard date and time format string</param>
        /// <param name="provider">The provider to use, null means default</param>
        /// <returns>Date and time pattern</returns>
        public static string PatternForStandardPattern(char standardPattern, IFormatProvider provider)
        {
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.GetInstance(provider);

            switch (standardPattern)
            {
                case 'D':
                    return dtfi.LongDatePattern;

                case 'd':
                    return dtfi.ShortDatePattern;

                case 'F':
                    return dtfi.FullDateTimePattern;

                case 'f':
                    return (dtfi.LongDatePattern + " " + dtfi.ShortTimePattern);

                case 'G':
                    return dtfi.ShortDatePattern + " " + dtfi.LongTimePattern;

                case 'g':
                    return dtfi.ShortDatePattern + " " + dtfi.ShortTimePattern;

                case 'M':
                case 'm':
                    return dtfi.MonthDayPattern;

                case 'T':
                    return dtfi.LongTimePattern;

                case 't':
                    return dtfi.ShortTimePattern;

                case 'Y':
                case 'y':
                    return dtfi.YearMonthPattern;

                case 's':
                    return dtfi.SortableDateTimePattern;

                case 'R':
                case 'r':
                    return dtfi.RFC1123Pattern;

                case 'U':
                    return dtfi.FullDateTimePattern;

                case 'u':
                    return dtfi.UniversalSortableDateTimePattern;

                case 'O':
                case 'o':
                    return "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK";

                default:
                    throw new FormatException("Invalid standard date and time pattern");
            }
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