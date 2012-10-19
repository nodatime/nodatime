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
using System.Linq;
using NUnit.Framework;
using NodaTime.Globalization;
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class LocalDatePatternTest : PatternTestBase<LocalDate>
    {
        private static readonly LocalDate SampleLocalDate = new LocalDate(1376, 6, 19);
        internal static readonly CultureInfo GenitiveNameTestCulture = CreateGenitiveTestCulture();
        internal static readonly CultureInfo GenitiveNameTestCultureWithLeadingNames = CreateGenitiveTestCultureWithLeadingNames();

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "!", Message = Messages.Parse_UnknownStandardFormat, Parameters = {'!', typeof(LocalDate).FullName }},
            new Data { Pattern = "%", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '%', typeof(LocalDate).FullName } },
            new Data { Pattern = "\\", Message = Messages.Parse_UnknownStandardFormat, Parameters = { '\\', typeof(LocalDate).FullName } },
            new Data { Pattern = "%%", Message = Messages.Parse_PercentDoubled },
            new Data { Pattern = "%\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "MMMMM", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'M', 4 } },
            new Data { Pattern = "ddddd", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'd', 4 } },
            new Data { Pattern = "H%", Message = Messages.Parse_PercentAtEndOfString },
            new Data { Pattern = "yyyyyy", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'y', 5 } },
            new Data { Pattern = "YYYYYY", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'Y', 5 } },
            new Data { Pattern = "ggg", Message = Messages.Parse_RepeatCountExceeded, Parameters = { 'g', 2 } },
            new Data { Pattern = "'qwe", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = Messages.Parse_EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            // Note incorrect use of "y" (year) instead of "Y" (year of era)
            new Data { Pattern = "dd MM yyyy gg", Message = Messages.Parse_EraWithoutYearOfEra },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "YYYY gg", Text = "2011 NodaEra", Message = Messages.Parse_MismatchedText, Parameters = {'g'} },
            new Data { Pattern = "YYYY yyyy gg", Text = "0010 0009 B.C.", Message = Messages.Parse_InconsistentValues2, Parameters = {'y', 'Y', typeof(LocalDate)} },
            new Data { Pattern = "yyyy MM dd dddd", Text = "2011 10 09 Saturday", Message = Messages.Parse_InconsistentDayOfWeekTextValue },
            new Data { Pattern = "yyyy MM dd ddd", Text = "2011 10 09 Sat", Message = Messages.Parse_InconsistentDayOfWeekTextValue },
            new Data { Pattern = "yyyy MM dd ddd", Text = "2011 10 09 FooBar", Message = Messages.Parse_MismatchedText, Parameters = {'d'} },
            new Data { Pattern = "yyyy MM dd dddd", Text = "2011 10 09 FooBar", Message = Messages.Parse_MismatchedText, Parameters = {'d'} },
            new Data { Pattern = "yyyy/MM/dd", Text = "2011/02-29", Message = Messages.Parse_DateSeparatorMismatch },
            // Don't match a short name against a long pattern
            new Data { Pattern = "yyyy MMMM dd", Text = "2011 Oct 09", Message = Messages.Parse_MismatchedText, Parameters = {'M'} },
            // Or vice versa... although this time we match the "Oct" and then fail as we're expecting a space
            new Data { Pattern = "yyyy MMM dd", Text = "2011 October 09", Message = Messages.Parse_MismatchedCharacter, Parameters = {' '}},

            // Invalid year, year-of-era, month, day
            new Data { Pattern = "yyyyy MM dd", Text = "99999 01 01", Message = Messages.Parse_FieldValueOutOfRange, Parameters = { 99999, 'y', typeof(LocalDate) } },
            new Data { Pattern = "YYYYY MM dd", Text = "99999 01 01", Message = Messages.Parse_YearOfEraOutOfRange, Parameters = { 99999, "CE", "ISO" } },
            new Data { Pattern = "YYYY MM dd", Text = "0000 01 01", Message = Messages.Parse_YearOfEraOutOfRange, Parameters = { 0, "CE", "ISO" } },
            new Data { Pattern = "yyyy MM dd", Text = "2011 15 29", Message = Messages.Parse_MonthOutOfRange, Parameters = { 15, 2011 } },
            new Data { Pattern = "yyyy MM dd", Text = "2011 02 35", Message = Messages.Parse_DayOfMonthOutOfRange, Parameters = { 35, 2, 2011 } },
            // Year of era can't be negative...
            new Data { Pattern = "YYYY MM dd", Text = "-15 01 01", Message = Messages.Parse_UnexpectedNegative },

            // Invalid leap years
            new Data { Pattern = "yyyy MM dd", Text = "2011 02 29", Message = Messages.Parse_DayOfMonthOutOfRange, Parameters = { 29, 2, 2011 } },
            new Data { Pattern = "yyyy MM dd", Text = "1900 02 29", Message = Messages.Parse_DayOfMonthOutOfRange, Parameters = { 29, 2, 1900 } },

            // Year of era and two-digit year, but they don't match
            new Data { Pattern = "YYYY yy", Text = "2011 10", Message = Messages.Parse_InconsistentValues2, Parameters = { 'y', 'Y', typeof(LocalDate) } },
        };

        internal static Data[] ParseOnlyData = {
            // Alternative era names
            new Data(0, 10, 3) { Pattern = "YYYY MM dd gg", Text = "0001 10 03 BCE" },

            // Valid leap years
            new Data(2000, 2, 29) { Pattern = "yyyy MM dd", Text = "2000 02 29" },
            new Data(2004, 2, 29) { Pattern = "yyyy MM dd", Text = "2004 02 29" },

            // Month parsing should be case-insensitive
            new Data(2011, 10, 3) { Pattern = "yyyy MMM dd", Text = "2011 OcT 03" },
            new Data(2011, 10, 3) { Pattern = "yyyy MMMM dd", Text = "2011 OcToBeR 03" },
            // Day-of-week parsing should be case-insensitive
            new Data(2011, 10, 9) { Pattern = "yyyy MM dd ddd", Text = "2011 10 09 sUN" },
            new Data(2011, 10, 9) { Pattern = "yyyy MM dd dddd", Text = "2011 10 09 SuNDaY" },

            // Genitive name is an extension of the non-genitive name; parse longer first.
            new Data(2011, 1, 10) { Pattern = "yyyy MMMM dd", Text = "2011 MonthName-Genitive 10", Culture = GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMMM dd", Text = "2011 MonthName 10", Culture = GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMM dd", Text = "2011 MN-Gen 10", Culture = GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMM dd", Text = "2011 MN 10", Culture = GenitiveNameTestCultureWithLeadingNames },
        };

        internal static Data[] FormatOnlyData = {
            // Would parse back to 2011
            new Data(1811, 7, 3) { Pattern = "yy M d", Text = "11 7 3" },
        };

        internal static Data[] FormatAndParseData = {
            // Standard patterns
            // Invariant culture uses the crazy MM/dd/yyyy format. Blech.
            new Data(2011, 10, 20) { Pattern = "d", Text = "10/20/2011" },
            new Data(2011, 10, 20) { Pattern = "D", Text = "Thursday, 20 October 2011" },

            // Custom patterns
            new Data(2011, 10, 3) { Pattern = "yyyy/MM/dd", Text = "2011/10/03" },
            new Data(2011, 10, 3) { Pattern = "yyyy/MM/dd", Text = "2011-10-03", Culture = Cultures.FrCa },
            new Data(2011, 10, 3) { Pattern = "yyyyMMdd", Text = "20111003" },
            new Data(2011, 7, 3) { Pattern = "yyy M d", Text = "2011 7 3" },
            new Data(2001, 7, 3) { Pattern = "yy M d", Text = "01 7 3" },
            new Data(2011, 7, 3) { Pattern = "yy M d", Text = "11 7 3" },
            new Data(2001, 7, 3) { Pattern = "y M d", Text = "1 7 3" },
            new Data(2011, 7, 3) { Pattern = "y M d", Text = "11 7 3"},
            // Cutoff defaults to 30 (at the moment...)
            new Data(1976, 7, 3) { Pattern = "y M d", Text = "76 7 3" },

            new Data(2000, 10, 3) { Pattern = "MM/dd", Text = "10/03"},
            new Data(1885, 10, 3) { Pattern = "MM/dd", Text = "10/03", Template = new LocalDate(1885, 10, 3) },

            // When we parse in all of the below tests, we'll use the month and day-of-month if it's provided;
            // the template value is specified to allow simple roundtripping. (Day of week doesn't affect what value is parsed; it just validates.)
            // Non-genitive month name when there's no "day of month", even if there's a "day of week"
            new Data(2011, 1, 3) { Pattern = "MMMM", Text = "FullNonGenName", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3)},
            new Data(2011, 1, 3) { Pattern = "MMMM dddd", Text = "FullNonGenName Monday", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            new Data(2011, 1, 3) { Pattern = "MMM", Text = "AbbrNonGenName", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            new Data(2011, 1, 3) { Pattern = "MMM ddd", Text = "AbbrNonGenName Mon", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            // Genitive month name when the pattern includes "day of month"
            new Data(2011, 1, 3) { Pattern = "MMMM dd", Text = "FullGenName 03", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            // TODO(Post-V1): Check whether or not this is actually appropriate
            new Data(2011, 1, 3) { Pattern = "MMM dd", Text = "AbbrGenName 03", Culture = GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },

            // Era handling
            new Data(2011, 1, 3) { Pattern = "YYYY MM dd gg", Text = "2011 01 03 A.D." },
            new Data(2011, 1, 3) { Pattern = "yyyy YYYY MM dd gg", Text = "2011 2011 01 03 A.D." },
            new Data(-1, 1, 3) { Pattern = "YYYY MM dd gg", Text = "0002 01 03 B.C." },

            // Day of week handling
            new Data(2011, 10, 9) { Pattern = "yyyy MM dd dddd", Text = "2011 10 09 Sunday" },
            new Data(2011, 10, 9) { Pattern = "yyyy MM dd ddd", Text = "2011 10 09 Sun" },

            // Month handling
            new Data(2011, 10, 9) { Pattern = "yyyy MMMM dd", Text = "2011 October 09" },
            new Data(2011, 10, 9) { Pattern = "yyyy MMM dd", Text = "2011 Oct 09" },

            // Year of era and two-digit year in the same format. Note that the year of era
            // gives the full year information, so we're not stuck in the 20th/21st century
            new Data(1825, 10, 9) { Pattern = "YYYY yy MM/dd", Text = "1825 25 10/09" },

            // Negative years
            new Data(-43, 3, 15) { Pattern = "yyyy MM dd", Text = "-0043 03 15"},
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclLongDatePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongDatePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclShortTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.ShortDatePattern);
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            var pattern = LocalDatePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture));
            // Create the BCL version in the culture's calendar, so that when formatted it really will have those
            // values, even though that may represent a completely different date/time to the Noda Time version...
            // we're only testing the formatting here.
            // Note that we're using Jon's -600th birthday so as to be in the right year range for the Saudi calendar.
            DateTime sampleDateTime = new DateTime(1376, 6, 19, 21, 13, 34, 123, culture.Calendar,
                                                   DateTimeKind.Unspecified).AddTicks(4567);

            Assert.AreEqual(sampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalDate));
        }

        /// <summary>
        /// .NET 3.5 doesn't contain any cultures where the abbreviated month names differ
        /// from the non-abbreviated month names. As we're testing under .NET 3.5, we'll need to create
        /// our own. This is just a clone of the invarant culture, with month 1 changed.
        /// </summary>
        private static CultureInfo CreateGenitiveTestCulture()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            format.MonthNames = ReplaceFirstElement(format.MonthNames, "FullNonGenName");
            format.MonthGenitiveNames = ReplaceFirstElement(format.MonthGenitiveNames, "FullGenName");
            format.AbbreviatedMonthNames = ReplaceFirstElement(format.AbbreviatedMonthNames, "AbbrNonGenName");
            format.AbbreviatedMonthGenitiveNames = ReplaceFirstElement(format.AbbreviatedMonthGenitiveNames, "AbbrGenName");
            // Note: Do *not* use CultureInfo.ReadOnly here, as it's broken in .NET 3.5 and lower; the month names
            // get reset to the original values, making the customization pointless :(
            return clone;
        }

        /// <summary>
        /// Some cultures may have genitive month names which are longer than th
        /// On Windows, we could use dsb-DE for this - but that doesn't exist in Mono.
        /// </summary>
        private static CultureInfo CreateGenitiveTestCultureWithLeadingNames()
        {
            CultureInfo clone = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            DateTimeFormatInfo format = clone.DateTimeFormat;
            format.MonthNames = ReplaceFirstElement(format.MonthNames, "MonthName");
            format.MonthGenitiveNames = ReplaceFirstElement(format.MonthGenitiveNames, "MonthName-Genitive");
            format.AbbreviatedMonthNames = ReplaceFirstElement(format.AbbreviatedMonthNames, "MN");
            format.AbbreviatedMonthGenitiveNames = ReplaceFirstElement(format.AbbreviatedMonthGenitiveNames, "MN-Gen");
            // Note: Do *not* use CultureInfo.ReadOnly here, as it's broken in .NET 3.5 and lower; the month names
            // get reset to the original values, making the customization pointless :(
            return clone;
        }

        private static string[] ReplaceFirstElement(string[] input, string newElement)
        {
            // Cloning so we don't accidentally *really* change any read-only cultures, to work around a bug in Mono.
            string[] clone = (string[])input.Clone();
            clone[0] = newElement;
            return clone;
        }

        public sealed class Data : PatternTestData<LocalDate>
        {
            // Default to the start of the year 2000.
            protected override LocalDate DefaultTemplate
            {
                get { return LocalDatePattern.DefaultTemplateValue; }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(LocalDate value)
                : base(value)
            {
            }

            public Data(int year, int month, int day)
                : this(new LocalDate(year, month, day))
            {
            }

            public Data()
                : this(LocalDatePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<LocalDate> CreatePattern()
            {
                return LocalDatePattern.CreateWithInvariantInfo(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
            }
        }
    }
}
