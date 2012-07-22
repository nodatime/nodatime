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
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public partial class LocalTimePatternTest : PatternTestBase<LocalTime>
    {
        private static readonly DateTime SampleDateTime = new DateTime(2000, 1, 1, 21, 13, 34, 123, DateTimeKind.Unspecified).AddTicks(4567);
        private static readonly LocalTime SampleLocalTime = new LocalTime(21, 13, 34, 123, 4567);

        // Characters we expect to work the same in Noda Time as in the BCL.
        private const string ExpectedCharacters = "hHms.:fFtT ";

        private static readonly CultureInfo AmOnlyCulture = CreateCustomAmPmCulture("am", "");
        private static readonly CultureInfo PmOnlyCulture = CreateCustomAmPmCulture("", "pm");
        private static readonly CultureInfo NoAmOrPmCulture = CreateCustomAmPmCulture("", "");

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(LocalTime).FullName}},
            new Data { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(LocalTime).FullName } },
            new Data { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(LocalTime).FullName } },
            new Data { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "ffffffff", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'f', 7 } },
            new Data { Pattern = "FFFFFFFF", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'F', 7 } },
            new Data { Pattern = "H%", Message = Messages.Parse_PercentAtEndOfString },
            new Data { Pattern = "HHH", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'H', 2 } },
            new Data { Pattern = "mmm", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data { Pattern = "mmmmmmmmmmmmmmmmmmm", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'm', 2 } },
            new Data { Pattern = "'qwe", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "sss", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 's', 2 } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Text = "17 6", Pattern = "HH h", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 'h', typeof(LocalTime).FullName}},
            new Data { Text = "17 AM", Pattern = "HH tt", Message = Messages.Parse_InconsistentValues2, Parameters = {'H', 't', typeof(LocalTime).FullName}},
        };

        internal static Data[] ParseOnlyData = {
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%f", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%F", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "FF", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "FFF", },
            new Data(0, 0, 0, 400) { Text = "40", Pattern = "ff", },
            new Data(0, 0, 0, 400) { Text = "400", Pattern = "fff", },
            new Data(0, 0, 0, 400) { Text = "4000", Pattern = "ffff", },
            new Data(0, 0, 0, 400) { Text = "40000", Pattern = "fffff", },
            new Data(0, 0, 0, 400) { Text = "400000", Pattern = "ffffff", },
            new Data(0, 0, 0, 400) { Text = "4000000", Pattern = "fffffff", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%f", },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%F", },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "ff" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "FF" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "FFF" },
            new Data(0, 0, 0, 450) { Text = "450", Pattern = "fff" },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%f" },
            new Data(0, 0, 0, 400) { Text = "4", Pattern = "%F" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "ff" },
            new Data(0, 0, 0, 450) { Text = "45", Pattern = "FF" },
            new Data(0, 0, 0, 456) { Text = "456", Pattern = "fff" },
            new Data(0, 0, 0, 456) { Text = "456", Pattern = "FFF" },

            new Data(0, 0, 0, 0) { Text = "0", Pattern = "%f" },
            new Data(0, 0, 0, 0) { Text = "00", Pattern = "ff" },
            new Data(0, 0, 0, 8) { Text = "008", Pattern = "fff" },
            new Data(0, 0, 0, 8) { Text = "008", Pattern = "FFF" },
            new Data(5, 0, 0, 0) { Text = "05", Pattern = "HH" },
            new Data(0, 6, 0, 0) { Text = "06", Pattern = "mm" },
            new Data(0, 0, 7, 0) { Text = "07", Pattern = "ss" },
            new Data(5, 0, 0, 0) { Text = "5", Pattern = "%H" },
            new Data(0, 6, 0, 0) { Text = "6", Pattern = "%m" },
            new Data(0, 0, 7, 0) { Text = "7", Pattern = "%s" },

            // AM/PM designator is case-insensitive for both short and long forms
            new Data(17, 0, 0, 0) { Text = "5 p", Pattern = "h t" },
            new Data(17, 0, 0, 0) { Text = "5 pm", Pattern = "h tt" },
        };

        internal static Data[] FormatOnlyData = {
            new Data(5, 6, 7, 8) { Text = "", Pattern = "%F" },
            new Data(5, 6, 7, 8) { Text = "", Pattern = "FF" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "FF" },
            new Data(1, 1, 1, 400) { Text = "4", Pattern = "FFF" },
            new Data(1, 1, 1, 400) { Text = "40", Pattern = "ff" },
            new Data(1, 1, 1, 400) { Text = "400", Pattern = "fff" },
            new Data(1, 1, 1, 400) { Text = "4000", Pattern = "ffff" },
            new Data(1, 1, 1, 400) { Text = "40000", Pattern = "fffff" },
            new Data(1, 1, 1, 400) { Text = "400000", Pattern = "ffffff" },
            new Data(1, 1, 1, 400) { Text = "4000000", Pattern = "fffffff" },
            new Data(1, 1, 1, 450) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 450) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "ff" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "FF" },
            new Data(1, 1, 1, 450) { Text = "45", Pattern = "FFF" },
            new Data(1, 1, 1, 450) { Text = "450", Pattern = "fff" },
            new Data(1, 1, 1, 456) { Text = "4", Pattern = "%f" },
            new Data(1, 1, 1, 456) { Text = "4", Pattern = "%F" },
            new Data(1, 1, 1, 456) { Text = "45", Pattern = "ff" },
            new Data(1, 1, 1, 456) { Text = "45", Pattern = "FF" },
            new Data(1, 1, 1, 456) { Text = "456", Pattern = "fff" },
            new Data(1, 1, 1, 456) { Text = "456", Pattern = "FFF" },


            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "0", Pattern = "%f" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "00", Pattern = "ff" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "008", Pattern = "fff" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "008", Pattern = "FFF" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "05", Pattern = "HH" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "06", Pattern = "mm" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "07", Pattern = "ss" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "5", Pattern = "%H" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "6", Pattern = "%m" },
            new Data(5, 6, 7, 8) { Culture = Cultures.EnUs, Text = "7", Pattern = "%s" },
        };

        internal static Data[] DefaultPatternData = {                              
            // Invariant culture uses HH:mm:ss for the "long" pattern
            new Data(5, 0, 0, 0) { Text = "05:00:00" },
            new Data(5, 12, 0, 0) { Text = "05:12:00" },
            new Data(5, 12, 34, 0) { Text = "05:12:34" },

            // US uses hh:mm:ss tt for the "long" pattern
            new Data(17, 0, 0, 0) { Culture = Cultures.EnUs, Text = "5:00:00 PM" },
            new Data(5, 0, 0, 0) { Culture = Cultures.EnUs, Text = "5:00:00 AM" },
            new Data(5, 12, 0, 0) { Culture = Cultures.EnUs, Text = "5:12:00 AM" },
            new Data(5, 12, 34, 0) { Culture = Cultures.EnUs, Text = "5:12:34 AM" },
        };

        internal static readonly Data[] TemplateValueData = {
            // Pattern specifies nothing - template value is passed through
            new Data(new LocalTime(1, 2, 3, 4, 5)) { Culture = Cultures.EnUs, Text = "X", Pattern = "%X", Template = new LocalTime(1, 2, 3, 4, 5) },
            // Tests for each individual field being propagated
            new Data(new LocalTime(1, 6, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "mm:ss.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 2, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:ss.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 7, 3, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:mm.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 7, 8, 4, 5)) { Culture = Cultures.EnUs, Text = "06:07:08", Pattern = "HH:mm:ss", Template = new LocalTime(1, 2, 3, 4, 5) },

            // Hours are tricky because of the ways they can be specified
            new Data(new LocalTime(6, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(1, 2, 3) },
            new Data(new LocalTime(18, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(17, 2, 3)) { Culture = Cultures.EnUs, Text = "5 PM", Pattern = "h tt", Template = new LocalTime(1, 2, 3) },
        };

        /// <summary>
        /// Common test data for both formatting and parsing. A test should be placed here unless is truly
        /// cannot be run both ways. This ensures that as many round-trip type tests are performed as possible.
        /// </summary>
        internal static readonly Data[] FormatAndParseData = {
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = ".", Pattern = "%." },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = ":", Pattern = "%:" },
            new Data(LocalTime.Midnight) { Culture = Cultures.ItIt, Text = ".", Pattern = "%." },
            new Data(LocalTime.Midnight) { Culture = Cultures.ItIt, Text = ".", Pattern = "%:" },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = "H", Pattern = "\\H" },
            new Data(LocalTime.Midnight) { Culture = Cultures.EnUs, Text = "HHss", Pattern = "'HHss'" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "%f" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "%F" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "FF" },
            new Data(0, 0, 0, 100) { Culture = Cultures.EnUs, Text = "1", Pattern = "FFF" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "ff" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "FF" },
            new Data(0, 0, 0, 120) { Culture = Cultures.EnUs, Text = "12", Pattern = "FFF" },
            new Data(0, 0, 0, 123) { Culture = Cultures.EnUs, Text = "123", Pattern = "fff" },
            new Data(0, 0, 0, 123) { Culture = Cultures.EnUs, Text = "123", Pattern = "FFF" },
            new Data(0, 0, 0, 123, 4000) { Culture = Cultures.EnUs, Text = "1234", Pattern = "ffff" },
            new Data(0, 0, 0, 123, 4000) { Culture = Cultures.EnUs, Text = "1234", Pattern = "FFFF" },
            new Data(0, 0, 0, 123, 4500) { Culture = Cultures.EnUs, Text = "12345", Pattern = "fffff" },
            new Data(0, 0, 0, 123, 4500) { Culture = Cultures.EnUs, Text = "12345", Pattern = "FFFFF" },
            new Data(0, 0, 0, 123, 4560) { Culture = Cultures.EnUs, Text = "123456", Pattern = "ffffff" },
            new Data(0, 0, 0, 123, 4560) { Culture = Cultures.EnUs, Text = "123456", Pattern = "FFFFFF" },
            new Data(0, 0, 0, 123, 4567) { Culture = Cultures.EnUs, Text = "1234567", Pattern = "fffffff" },
            new Data(0, 0, 0, 123, 4567) { Culture = Cultures.EnUs, Text = "1234567", Pattern = "FFFFFFF" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".f" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".F" },
            new Data(0, 0, 0, 600) { Culture = Cultures.EnUs, Text = ".6", Pattern = ".FFF" }, // Elided fraction
            new Data(0, 0, 0, 678) { Culture = Cultures.EnUs, Text = ".678", Pattern = ".fff" },
            new Data(0, 0, 0, 678) { Culture = Cultures.EnUs, Text = ".678", Pattern = ".FFF" },
            new Data(0, 0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%s" },
            new Data(0, 0, 12, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "ss" },
            new Data(0, 0, 2, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%s" },
            new Data(0, 12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%m" },
            new Data(0, 12, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "mm" },
            new Data(0, 2, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%m" },
            new Data(1, 0, 0, 0) { Culture = Cultures.EnUs, Text = "1", Pattern = "H.FFF" }, // Missing fraction
            new Data(12, 0, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "%H" },
            new Data(12, 0, 0, 0) { Culture = Cultures.EnUs, Text = "12", Pattern = "HH" },
            new Data(2, 0, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%H" },
            new Data(2, 0, 0, 0) { Culture = Cultures.EnUs, Text = "2", Pattern = "%H" },
            new Data(0, 0, 12, 340) { Culture = Cultures.EnUs, Text = "12.34", Pattern = "ss.FFF"  },

            new Data(14, 15, 16) { Culture = Cultures.EnUs, Text = "14:15:16", Pattern = "r" },
            new Data(14, 15, 16, 700) { Culture = Cultures.EnUs, Text = "14:15:16.7", Pattern = "r" },
            new Data(14, 15, 16, 780) { Culture = Cultures.EnUs, Text = "14:15:16.78", Pattern = "r" },
            new Data(14, 15, 16, 789) { Culture = Cultures.EnUs, Text = "14:15:16.789", Pattern = "r" },
            new Data(14, 15, 16, 789, 1000) { Culture = Cultures.EnUs, Text = "14:15:16.7891", Pattern = "r" },
            new Data(14, 15, 16, 789, 1200) { Culture = Cultures.EnUs, Text = "14:15:16.78912", Pattern = "r" },
            new Data(14, 15, 16, 789, 1230) { Culture = Cultures.EnUs, Text = "14:15:16.789123", Pattern = "r" },
            new Data(14, 15, 16, 789, 1234) { Culture = Cultures.EnUs, Text = "14:15:16.7891234", Pattern = "r" },
            new Data(14, 15, 16, 700) { Culture = Cultures.ItIt, Text = "14.15.16.7", Pattern = "r" },
            new Data(14, 15, 16, 780) { Culture = Cultures.ItIt, Text = "14.15.16.78", Pattern = "r" },
            new Data(14, 15, 16, 789) { Culture = Cultures.ItIt, Text = "14.15.16.789", Pattern = "r" },
            new Data(14, 15, 16, 789, 1000) { Culture = Cultures.ItIt, Text = "14.15.16.7891", Pattern = "r" },
            new Data(14, 15, 16, 789, 1200) { Culture = Cultures.ItIt, Text = "14.15.16.78912", Pattern = "r" },
            new Data(14, 15, 16, 789, 1230) { Culture = Cultures.ItIt, Text = "14.15.16.789123", Pattern = "r" },
            new Data(14, 15, 16, 789, 1234) { Culture = Cultures.ItIt, Text = "14.15.16.7891234", Pattern = "r" },

            // ------------ Template value tests ----------
            // Mixtures of 12 and 24 hour times
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 6 PM", Pattern = "HH h tt" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 6", Pattern = "HH h" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "18 PM", Pattern = "HH tt" },
            new Data(18, 0, 0) { Culture = Cultures.EnUs, Text = "6 PM", Pattern = "h tt" },
            new Data(6, 0, 0) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h" },
            new Data(0, 0, 0) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt" },
            new Data(12, 0, 0) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt" },

            // Pattern specifies nothing - template value is passed through
            new Data(new LocalTime(1, 2, 3, 4, 5)) { Culture = Cultures.EnUs, Text = "X", Pattern = "%X", Template = new LocalTime(1, 2, 3, 4, 5) },
            // Tests for each individual field being propagated
            new Data(new LocalTime(1, 6, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "mm:ss.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 2, 7, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:ss.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 7, 3, 8, 9)) { Culture = Cultures.EnUs, Text = "06:07.0080009", Pattern = "HH:mm.FFFFFFF", Template = new LocalTime(1, 2, 3, 4, 5) },
            new Data(new LocalTime(6, 7, 8, 4, 5)) { Culture = Cultures.EnUs, Text = "06:07:08", Pattern = "HH:mm:ss", Template = new LocalTime(1, 2, 3, 4, 5) },

            // Hours are tricky because of the ways they can be specified
            new Data(new LocalTime(6, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(1, 2, 3) },
            new Data(new LocalTime(18, 2, 3)) { Culture = Cultures.EnUs, Text = "6", Pattern = "%h", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(14, 2, 3) },
            new Data(new LocalTime(2, 2, 3)) { Culture = Cultures.EnUs, Text = "AM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(14, 2, 3)) { Culture = Cultures.EnUs, Text = "PM", Pattern = "tt", Template = new LocalTime(2, 2, 3) },
            new Data(new LocalTime(17, 2, 3)) { Culture = Cultures.EnUs, Text = "5 PM", Pattern = "h tt", Template = new LocalTime(1, 2, 3) },
            // --------------- end of template value tests ----------------------

            // Only one of the AM/PM designator is present. We should still be able to work out what is meant, by the presence
            // or absense of the non-empty one.
            new Data(5, 0, 0) { Culture = AmOnlyCulture, Text = "5 am", Pattern = "h tt" },
            new Data(15, 0, 0) { Culture = AmOnlyCulture, Text = "3 ", Pattern = "h tt", Description = "Implicit PM" },
            new Data(5, 0, 0) { Culture = AmOnlyCulture, Text = "5 a", Pattern = "h t" },
            new Data(15, 0, 0) { Culture = AmOnlyCulture, Text = "3 ", Pattern = "h t", Description = "Implicit PM"},

            new Data(5, 0, 0) { Culture = PmOnlyCulture, Text = "5 ", Pattern = "h tt" },
            new Data(15, 0, 0) { Culture = PmOnlyCulture, Text = "3 pm", Pattern = "h tt" },
            new Data(5, 0, 0) { Culture = PmOnlyCulture, Text = "5 ", Pattern = "h t" },
            new Data(15, 0, 0) { Culture = PmOnlyCulture, Text = "3 p", Pattern = "h t" },

            // AM / PM designators are both empty strings. The parsing side relies on the AM/PM value being correct on the
            // template value. (The template value is for the wrong actual hour, but in the right side of noon.)
            new Data(5, 0, 0) { Culture = NoAmOrPmCulture, Text = "5 ", Pattern = "h tt", Template = new LocalTime(2, 0, 0) },
            new Data(15, 0, 0) { Culture = NoAmOrPmCulture, Text = "3 ", Pattern = "h tt", Template = new LocalTime(14, 0, 0) },
            new Data(5, 0, 0) { Culture = NoAmOrPmCulture, Text = "5 ", Pattern = "h t", Template = new LocalTime(2, 0, 0) },
            new Data(15, 0, 0) { Culture = NoAmOrPmCulture, Text = "3 ", Pattern = "h t", Template = new LocalTime(14, 0, 0) },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        private static CultureInfo CreateCustomAmPmCulture(string amDesignator, string pmDesignator)
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            clone.DateTimeFormat.AMDesignator = amDesignator;
            clone.DateTimeFormat.PMDesignator = pmDesignator;
            return CultureInfo.ReadOnly(clone);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCulturesOrEmptyOnMono")]
        public void BclLongTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            AssertValidNodaPattern(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCulturesOrEmptyOnMono")]
        public void BclShortTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            AssertValidNodaPattern(culture, culture.DateTimeFormat.ShortTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclLongTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclShortTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.ShortTimePattern);
        }

        [Test]
        public void CreateWithInvariantInfo_NullPatternText()
        {
            Assert.Throws<ArgumentNullException>(() => LocalTimePattern.CreateWithInvariantInfo(null));
        }

        [Test]
        public void Create_NullFormatInfo()
        {
            Assert.Throws<ArgumentNullException>(() => LocalTimePattern.Create("HH", null));
        }

        [Test]
        public void TemplateValue_DefaultsToMidnight()
        {
            var pattern = LocalTimePattern.CreateWithInvariantInfo("HH");
            Assert.AreEqual(LocalTime.Midnight, pattern.TemplateValue);
        }

        [Test]
        public void WithTemplateValue_PropertyFetch()
        {
            LocalTime newValue = new LocalTime(1, 23, 45);
            var pattern = LocalTimePattern.CreateWithInvariantInfo("HH").WithTemplateValue(newValue);
            Assert.AreEqual(newValue, pattern.TemplateValue);
        }
        
        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            // On Mono, some general patterns include an offset at the end. For the moment, ignore them.
            // TODO(Post-V1): Work out what to do in such cases...
            if (patternText.EndsWith("z"))
            {
                return;
            }
            var pattern = LocalTimePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture));

            Assert.AreEqual(SampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalTime));
        }

        private static void AssertValidNodaPattern(CultureInfo culture, string pattern)
        {
            PatternCursor cursor = new PatternCursor(pattern);
            while (cursor.MoveNext())
            {
                if (cursor.Current == '\'')
                {
                    PatternParseResult<LocalTime> parseResult = null;
                    cursor.GetQuotedString(ref parseResult);
                    Assert.IsTrue(parseResult == null, "Pattern '" + pattern + "' is misquoted");
                }
                else
                {
                    Assert.IsTrue(ExpectedCharacters.Contains(cursor.Current),
                        "Pattern '" + pattern + "' contains unquoted, unexpected characters");
                }
            }
            // Check that the pattern parses
            LocalTimePattern.Create(pattern, NodaFormatInfo.GetFormatInfo(culture));
        }

        /// <summary>
        /// A container for test data for formatting and parsing <see cref="LocalTime" /> objects.
        /// </summary>
        public sealed class Data : PatternTestData<LocalTime>
        {
            // Default to midnight
            protected override LocalTime DefaultTemplate
            {
                get { return LocalTime.Midnight; }
            }

            public Data(LocalTime value)
                : base(value)
            {
            }

            public Data(int hours, int minutes, int seconds)
                : this(new LocalTime(hours, minutes, seconds))
            {
            }

            public Data(int hours, int minutes, int seconds, int milliseconds)
                : this(new LocalTime(hours, minutes, seconds, milliseconds))
            {
            }

            public Data(int hours, int minutes, int seconds, int milliseconds, int ticksWithinMillisecond)
                : this(new LocalTime(hours, minutes, seconds, milliseconds, ticksWithinMillisecond))
            {
            }

            public Data()
                : this(LocalTime.Midnight)
            {
            }

            internal override IPattern<LocalTime> CreatePattern()
            {
                return LocalTimePattern.CreateWithInvariantInfo(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
            }
        }
    }
}
