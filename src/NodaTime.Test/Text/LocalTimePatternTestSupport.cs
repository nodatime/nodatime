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
using System.Collections.Generic;
using System.Linq;
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Defines the test data for the <see cref="LocalTime" /> type formatting and parsing tests.
    /// </summary>
    public class LocalTimePatternTestSupport : FormattingTestSupport
    {
        /// <summary>
        /// Test data that can only be used to test formatting.
        /// </summary>
        internal static readonly LocalTimeData[] FormatData = {
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "", P = "%F"  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "", P = "FF"  },
            new LocalTimeData(5, 0, 0, 0) { C = EnUs, S = "05:00:00" },
            new LocalTimeData(5, 12, 0, 0) { C = EnUs, S = "05:12:00" },
            new LocalTimeData(5, 12, 34, 0) { C = EnUs, S = "05:12:34" },
            new LocalTimeData(5, 12, 34, 567) { C = EnUs, S = "05:12:34.567" },
            new LocalTimeData(5, 12, 34, 567, 8901) { C = EnUs, S = "05:12:34.567" }
        };

        /// <summary>
        /// Test data that can only be used to test parsing.
        /// </summary>
        internal static readonly LocalTimeData[] ParseData = {
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly LocalTimeData[] CommonData = {
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = ".", P = "%.", Name = "decimal separator" },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = ":", P = "%:", Name = "date separator" },
            // TODO: Work out what this should actually do...
            new LocalTimeData(LocalTime.Midnight) { C = ItIt, S = ".", P = "%.", Name = "decimal separator" },
            new LocalTimeData(LocalTime.Midnight) { C = ItIt, S = ".", P = "%:", Name = "date separator" },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "H", P = "\\H" },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "HHss", P = "'HHss'" },
            new LocalTimeData(0, 0, 0, 100) { C = EnUs, S = "1", P = "%f" },
            new LocalTimeData(0, 0, 0, 100) { C = EnUs, S = "1", P = "%F" },
            new LocalTimeData(0, 0, 0, 100) { C = EnUs, S = "1", P = "FF" },
            new LocalTimeData(0, 0, 0, 100) { C = EnUs, S = "1", P = "FFF" },
            new LocalTimeData(0, 0, 0, 120) { C = EnUs, S = "12", P = "ff" },
            new LocalTimeData(0, 0, 0, 120) { C = EnUs, S = "12", P = "FF" },
            new LocalTimeData(0, 0, 0, 120) { C = EnUs, S = "12", P = "FFF" },
            new LocalTimeData(0, 0, 0, 123) { C = EnUs, S = "123", P = "fff" },
            new LocalTimeData(0, 0, 0, 123) { C = EnUs, S = "123", P = "FFF" },
            new LocalTimeData(0, 0, 0, 123, 4000) { C = EnUs, S = "1234", P = "ffff" },
            new LocalTimeData(0, 0, 0, 123, 4000) { C = EnUs, S = "1234", P = "FFFF" },
            new LocalTimeData(0, 0, 0, 123, 4500) { C = EnUs, S = "12345", P = "fffff" },
            new LocalTimeData(0, 0, 0, 123, 4500) { C = EnUs, S = "12345", P = "FFFFF" },
            new LocalTimeData(0, 0, 0, 123, 4560) { C = EnUs, S = "123456", P = "ffffff" },
            new LocalTimeData(0, 0, 0, 123, 4560) { C = EnUs, S = "123456", P = "FFFFFF" },
            new LocalTimeData(0, 0, 0, 123, 4567) { C = EnUs, S = "1234567", P = "fffffff" },
            new LocalTimeData(0, 0, 0, 123, 4567) { C = EnUs, S = "1234567", P = "FFFFFFF" },
            new LocalTimeData(0, 0, 0, 600) { C = EnUs, S = ".6", P = ".f" },
            new LocalTimeData(0, 0, 0, 600) { C = EnUs, S = ".6", P = ".F" },
            new LocalTimeData(0, 0, 0, 600) { C = EnUs, S = ".6", P = ".FFF", Name = "elided zeros" },
            new LocalTimeData(0, 0, 0, 678) { C = EnUs, S = ".678", P = ".fff" },
            new LocalTimeData(0, 0, 0, 678) { C = EnUs, S = ".678", P = ".FFF" },
            new LocalTimeData(0, 0, 12, 0) { C = EnUs, S = "12", P = "%s" },
            new LocalTimeData(0, 0, 12, 0) { C = EnUs, S = "12", P = "ss" },
            new LocalTimeData(0, 0, 2, 0) { C = EnUs, S = "2", P = "%s" },
            new LocalTimeData(0, 1, 23, 0) { C = EnUs, S = "1:23", P = "HH:mm\0m:ss"},
            new LocalTimeData(0, 12, 0, 0) { C = EnUs, S = "12", P = "%m" },
            new LocalTimeData(0, 12, 0, 0) { C = EnUs, S = "12", P = "mm" },
            new LocalTimeData(0, 2, 0, 0) { C = EnUs, S = "2", P = "%m" },
            new LocalTimeData(1, 0, 0, 0) { C = EnUs, S = "1", P = "H.FFF", Name = "missing fraction" },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4", P = "%f", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4", P = "%F", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4", P = "FF", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4", P = "FFF", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "40", P = "ff", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "400", P = "fff", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4000", P = "ffff", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "40000", P = "fffff", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "400000", P = "ffffff", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 400) { C = EnUs, S = "4000000", P = "fffffff", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "4", P = "%f", PV = new LocalTime(0, 0, 0, 400) },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "4", P = "%F", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "45", P = "ff", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "45", P = "FF", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "45", P = "FFF", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 450) { C = EnUs, S = "450", P = "fff", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "4", P = "%f", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "4", P = "%F", PV = new LocalTime(0, 0, 0, 400)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "45", P = "ff", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "45", P = "FF", PV = new LocalTime(0, 0, 0, 450)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "456", P = "fff", PV = new LocalTime(0, 0, 0, 456)  },
            new LocalTimeData(1, 1, 1, 456) { C = EnUs, S = "456", P = "FFF", PV = new LocalTime(0, 0, 0, 456)  },
            new LocalTimeData(12, 0, 0, 0) { C = EnUs, S = "12", P = "%H" },
            new LocalTimeData(12, 0, 0, 0) { C = EnUs, S = "12", P = "HH" },
            new LocalTimeData(2, 0, 0, 0) { C = EnUs, S = "2", P = "%H" },
            new LocalTimeData(2, 0, 0, 0) { C = EnUs, S = "2", P = "%H" },
            new LocalTimeData(0, 0, 12, 340) { C = EnUs, S = "12.34", P = "ss.FFF"  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "0", P = "%f", PV = new LocalTime(0, 0, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "00", P = "ff", PV = new LocalTime(0, 0, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "008", P = "fff", PV = new LocalTime(0, 0, 0, 8)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "008", P = "FFF", PV = new LocalTime(0, 0, 0, 8)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "05", P = "HH", PV = new LocalTime(5, 0, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "06", P = "mm", PV = new LocalTime(0, 6, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "07", P = "ss", PV = new LocalTime(0, 0, 7, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "5", P = "%H", PV = new LocalTime(5, 0, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "6", P = "%m", PV = new LocalTime(0, 6, 0, 0)  },
            new LocalTimeData(5, 6, 7, 8) { C = EnUs, S = "7", P = "%s", PV = new LocalTime(0, 0, 7, 0)  },
            new LocalTimeData(14, 15, 16) { C = EnUs, S = "14:15:16", P = "r" },
            new LocalTimeData(14, 15, 16, 700) { C = EnUs, S = "14:15:16.7", P = "r" },
            new LocalTimeData(14, 15, 16, 780) { C = EnUs, S = "14:15:16.78", P = "r" },
            new LocalTimeData(14, 15, 16, 789) { C = EnUs, S = "14:15:16.789", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1000) { C = EnUs, S = "14:15:16.7891", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1200) { C = EnUs, S = "14:15:16.78912", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1230) { C = EnUs, S = "14:15:16.789123", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1234) { C = EnUs, S = "14:15:16.7891234", P = "r" },
            new LocalTimeData(14, 15, 16, 700) { C = BnBd, S = "14.15.16.7", P = "r" },
            new LocalTimeData(14, 15, 16, 780) { C = BnBd, S = "14.15.16.78", P = "r" },
            new LocalTimeData(14, 15, 16, 789) { C = BnBd, S = "14.15.16.789", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1000) { C = BnBd, S = "14.15.16.7891", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1200) { C = BnBd, S = "14.15.16.78912", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1230) { C = BnBd, S = "14.15.16.789123", P = "r" },
            new LocalTimeData(14, 15, 16, 789, 1234) { C = BnBd, S = "14.15.16.7891234", P = "r" },
        };

        internal static readonly LocalTimeData[] InvalidPatterns = {
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "!", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(LocalTime).FullName}},
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(LocalTime).FullName } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(LocalTime).FullName } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "%%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_PercentDoubled },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "%\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_EscapeAtEndOfString },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "ffffffff", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'f', 7 } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "FFFFFFFF", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'F', 7 } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "H%", Exception=typeof(InvalidPatternException), Message = Messages.Parse_PercentAtEndOfString },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "HHH", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'H', 2 } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "mmm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "mmmmmmmmmmmmmmmmmmm", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "'qwe", Exception=typeof(InvalidPatternException), Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "'qwe\\", Exception=typeof(InvalidPatternException), Message = Messages.Parse_EscapeAtEndOfString },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "'qwe\\'", Exception=typeof(InvalidPatternException), Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new LocalTimeData(LocalTime.Midnight) { C = EnUs, S = "123", P = "sss", Exception=typeof(InvalidPatternException), Message = Messages.Parse_RepeatCountExceeded, Parameters = { 's', 2 } },
        };

        // TODO: Decide whether to test invalid patterns separately
        internal static readonly IEnumerable<LocalTimeData> AllParseData = ParseData.Concat(CommonData).Concat(InvalidPatterns);
        internal static readonly IEnumerable<LocalTimeData> AllFormatData = FormatData.Concat(CommonData).Concat(InvalidPatterns);
        internal static readonly IEnumerable<LocalTimeData> FormatWithoutFormat = AllFormatData.Where(data => data.P == null);

        #region Nested type: LocalTimeData
        /// <summary>
        /// A container for test data for formatting and parsing <see cref="Offset" /> objects.
        /// </summary>
        public sealed class LocalTimeData : AbstractFormattingData<LocalTime>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LocalTimeData" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public LocalTimeData(LocalTime value)
                : base(value)
            {
            }

            public LocalTimeData(int hours, int minutes, int seconds)
                : this(new LocalTime(hours, minutes, seconds))
            {
            }

            public LocalTimeData(int hours, int minutes, int seconds, int milliseconds)
                : this(new LocalTime(hours, minutes, seconds, milliseconds))
            {
            }

            public LocalTimeData(int hours, int minutes, int seconds, int milliseconds, int ticksWithinMillisecond)
                : this(new LocalTime(hours, minutes, seconds, milliseconds, ticksWithinMillisecond))
            {
            }

            /// <summary>
            /// Returns a string representation of the given value. This will usually not call the ToString()
            /// method as that is problably being tested. The returned string is only used in test code and
            /// labels so it doesn't have to be beautiful. Must handle <c>null</c> if the type is a reference
            /// type. This should not throw an exception.
            /// </summary>
            /// <param name="value">The value to format.</param>
            /// <returns>The string representation.</returns>
            protected override string ValueLabel(LocalTime value)
            {
                return string.Format("new LocalTime({0}, {1}, {2}, {3}, {4})",
                                     value.HourOfDay,
                                     value.MinuteOfHour,
                                     value.SecondOfMinute,
                                     value.MillisecondOfSecond,
                                     value.TickOfMillisecond);
            }
        }
        #endregion
    }
}
