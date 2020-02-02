// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Text;
using NUnit.Framework;
using NodaTime.Test.Calendars;

namespace NodaTime.Test.Text
{
    public class YearMonthPatternTest : PatternTestBase<YearMonth>
    {
        private static readonly YearMonth SampleYearMonth = new YearMonth(1976, 6);

        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data { Pattern = "!", Message = TextErrorMessages.UnknownStandardFormat, Parameters = {'!', typeof(YearMonth).FullName! }},
            new Data { Pattern = "%", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '%', typeof(YearMonth).FullName! } },
            new Data { Pattern = "\\", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '\\', typeof(YearMonth).FullName! } },
            new Data { Pattern = "%%", Message = TextErrorMessages.PercentDoubled },
            new Data { Pattern = "%\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "MMMMM", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'M', 4 } },
            new Data { Pattern = "M%", Message = TextErrorMessages.PercentAtEndOfString },
            new Data { Pattern = "yyyyy", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'y', 4 } },
            new Data { Pattern = "uuuuu", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'u', 4 } },
            new Data { Pattern = "ggg", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'g', 2 } },
            new Data { Pattern = "'qwe", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            // Note incorrect use of "u" (year) instead of "y" (year of era)
            new Data { Pattern = "MM uuuu gg", Message = TextErrorMessages.EraWithoutYearOfEra },
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "MM yyyy gg c", Message = TextErrorMessages.CalendarAndEra },

            // Invalid patterns directly after the uuuu specifier. This will detect the issue early, but then
            // continue and reject it in the normal path.
            new Data { Pattern = "uuuu'", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "uuuu\\", Message = TextErrorMessages.EscapeAtEndOfString },

            // Common typo, which is caught in 2.0...
            new Data { Pattern = "uuuu-mm", Message = TextErrorMessages.UnquotedLiteral, Parameters = { 'm' } },
            // T isn't valid in a year/month pattern
            new Data { Pattern = "uuuu-MMT00:00:00", Message = TextErrorMessages.UnquotedLiteral, Parameters = { 'T' } },

            // These became invalid in date patterns in v2.0, when we decided that y and yyy weren't sensible.
            new Data { Pattern = "y M", Message = TextErrorMessages.InvalidRepeatCount, Parameters = { 'y', 1 } },
            new Data { Pattern = "yyy M", Message = TextErrorMessages.InvalidRepeatCount, Parameters = { 'y', 3 } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "yyyy gg", Text = "2011 NodaEra", Message = TextErrorMessages.MismatchedText, Parameters = {'g'} },
            new Data { Pattern = "yyyy uuuu gg", Text = "0010 0009 B.C.", Message = TextErrorMessages.InconsistentValues2, Parameters = {'g', 'u', typeof(YearMonth)} },
            new Data { Pattern = "uuuu MM MMMM", Text = "2011 10 January", Message = TextErrorMessages.InconsistentMonthTextValue },
            // Don't match a short name against a long pattern
            new Data { Pattern = "uuuu MMMM", Text = "2011 Oct", Message = TextErrorMessages.MismatchedText, Parameters = {'M'} },
            // Or vice versa... although this time we match the "Oct" and then fail as we're expecting a space
            new Data { Pattern = "MMM uuuu", Text = "October 2011", Message = TextErrorMessages.MismatchedCharacter, Parameters = {' '}},

            // Invalid year-of-era/year, month
            new Data { Pattern = "yyyy MM", Text = "0000 01", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = { 0, 'y', typeof(YearMonth) } },
            new Data { Pattern = "yyyy MM", Text = "2011 15", Message = TextErrorMessages.MonthOutOfRange, Parameters = { 15, 2011 } },
            new Data { Pattern = "uuuu MM", Text = "2011 15", Message = TextErrorMessages.MonthOutOfRange, Parameters = { 15, 2011 } },
            // Year of era can't be negative...
            new Data { Pattern = "yyyy MM", Text = "-15 01", Message = TextErrorMessages.UnexpectedNegative },

            // Year of era and two-digit year, but they don't match
            new Data { Pattern = "yyyy uu", Text = "2011 10", Message = TextErrorMessages.InconsistentValues2, Parameters = { 'y', 'u', typeof(YearMonth) } },

            // Invalid calendar name
            new Data { Pattern = "c uuuu MM", Text = "2015 01", Message = TextErrorMessages.NoMatchingCalendarSystem },

            // Invalid year
            new Data { Template = new YearMonth(1, 1, CalendarSystem.IslamicBcl), Pattern = "uuuu", Text = "9999", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = { 9999, 'u', typeof(YearMonth) } },
            new Data { Template = new YearMonth(1, 1, CalendarSystem.IslamicBcl), Pattern = "yyyy", Text = "9999", Message = TextErrorMessages.YearOfEraOutOfRange, Parameters = { 9999, "EH", "Hijri" } },

            // https://github.com/nodatime/nodatime/issues/414
            new Data { Pattern = "uuuu-MM", Text = "1984-00", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = { 0, 'M', typeof(YearMonth) } },
            new Data { Pattern = "M/uuuu", Text = "00/1984", Message = TextErrorMessages.FieldValueOutOfRange, Parameters = { 0, 'M', typeof(YearMonth) } },

            // Calendar ID parsing is now ordinal, case-sensitive
            new Data(2011, 10) { Pattern = "uuuu MM c", Text = "2011 10 iso", Message = TextErrorMessages.NoMatchingCalendarSystem },
        };

        internal static Data[] ParseOnlyData = {
            // Alternative era names
            new Data(0, 10) { Pattern = "yyyy MM gg", Text = "0001 10 BCE" },

            // Month parsing should be case-insensitive
            new Data(2011, 10) { Pattern = "uuuu MMM", Text = "2011 OcT" },
            new Data(2011, 10) { Pattern = "uuuu MMMM", Text = "2011 OcToBeR" },

            // Genitive name is an extension of the non-genitive name; parse longer first.
            new Data(2011, 1) { Pattern = "uuuu MMMM", Text = "2011 MonthName-Genitive", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1) { Pattern = "uuuu MMMM", Text = "2011 MonthName", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1) { Pattern = "uuuu MMM", Text = "2011 MN-Gen", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1) { Pattern = "uuuu MMM", Text = "2011 MN", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
        };

        internal static Data[] FormatOnlyData = {
            // Would parse back to 2011
            new Data(1811, 7) { Pattern = "yy M", Text = "11 7" },
            // Tests for the documented 2-digit formatting of BC years
            // (Less of an issue since yy became "year of era")
            new Data(-94, 7) { Pattern = "yy M", Text = "95 7" },
            new Data(-93, 7) { Pattern = "yy M", Text = "94 7" },
        };

        internal static Data[] FormatAndParseData = {
            // Standard patterns
            new Data(2011, 10) { Pattern = "g", Text = "2011-10" },

            // Custom patterns
            new Data(2011, 10) { Pattern = "uuuu/MM", Text = "2011/10" },
            new Data(2011, 10) { Pattern = "uuuu/MM", Text = "2011-10", Culture = Cultures.FrCa },
            new Data(2011, 10) { Pattern = "uuuuMM", Text = "201110" },
            new Data(2001, 7) { Pattern = "yy M", Text = "01 7" },
            new Data(2011, 7) { Pattern = "yy M", Text = "11 7" },
            new Data(2030, 7) { Pattern = "yy M", Text = "30 7" },
            // Cutoff defaults to 30 (at the moment...)
            new Data(1931, 7) { Pattern = "yy M", Text = "31 7" },
            new Data(1976, 7) { Pattern = "yy M", Text = "76 7" },

            // In the first century, we don't skip back a century for "high" two-digit year numbers.
            new Data(25, 7) { Pattern = "yy M", Text = "25 7", Template = new YearMonth(50, 1) },
            new Data(35, 7) { Pattern = "yy M", Text = "35 7", Template = new YearMonth(50, 1) },

            new Data(2000, 10) { Pattern = "MM", Text = "10"},
            new Data(1885, 10) { Pattern = "MM", Text = "10", Template = new YearMonth(1885, 10) },

            // When we parse in all of the below tests, we'll use the month and day-of-month if it's provided;
            // the template value is specified to allow simple roundtripping. (Day of week doesn't affect what value is parsed; it just validates.)
            // Non-genitive month name when there's no "day of month", even if there's a "day of week"
            new Data(2011, 1) { Pattern = "MMMM", Text = "FullNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new YearMonth(2011, 5)},
            new Data(2011, 1) { Pattern = "MMM", Text = "AbbrNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new YearMonth(2011, 5) },

            // Era handling
            new Data(2011, 1) { Pattern = "yyyy MM gg", Text = "2011 01 A.D." },
            new Data(2011, 1) { Pattern = "uuuu yyyy MM gg", Text = "2011 2011 01 A.D." },
            new Data(-1, 1) { Pattern = "yyyy MM gg", Text = "0002 01 B.C." },

            // Month handling
            new Data(2011, 10) { Pattern = "uuuu MMMM", Text = "2011 October" },
            new Data(2011, 10) { Pattern = "uuuu MMM", Text = "2011 Oct" },

            // Year and two-digit year-of-era in the same format. Note that the year
            // gives the full year information, so we're not stuck in the 20th/21st century
            new Data(1825, 10) { Pattern = "uuuu yy MM", Text = "1825 25 10" },

            // Negative years
            new Data(-43, 3) { Pattern = "uuuu MM", Text = "-0043 03"},

            // Calendar handling
            new Data(2011, 10) { Pattern = "c uuuu MM", Text = "ISO 2011 10" },
            new Data(2011, 10) { Pattern = "uuuu MM c", Text = "2011 10 ISO" },
            new Data(2011, 10, CalendarSystem.Coptic) { Pattern = "c uuuu MM", Text = "Coptic 2011 10" },
            new Data(2011, 10, CalendarSystem.Coptic) { Pattern = "uuuu MM c", Text = "2011 10 Coptic" },

            new Data(180, 15, CalendarSystem.Badi) { Pattern = "uuuu MM c", Text = "0180 15 Badi" },

            // 3 digit year patterns (odd, but valid)
            new Data(12, 1) { Pattern = "uuu MM", Text = "012 01" },
            new Data(-12, 1) { Pattern = "uuu MM", Text = "-012 01" },
            new Data(123, 1) { Pattern = "uuu MM", Text = "123 01" },
            new Data(-123, 1) { Pattern = "uuu MM", Text = "-123 01" },
            new Data(1234, 1) { Pattern = "uuu MM", Text = "1234 01" },
            new Data(-1234, 1) { Pattern = "uuu MM", Text = "-1234 01" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);
        
        [Test]
        public void CreateWithCurrentCulture()
        {
            var yearMonth = new YearMonth(2017, 8);
            using (CultureSaver.SetCultures(Cultures.EnUs))
            {
                var pattern = YearMonthPattern.CreateWithCurrentCulture("uuuu/MM");
                Assert.AreEqual("2017/08", pattern.Format(yearMonth));
            }
            using (CultureSaver.SetCultures(Cultures.FrCa))
            {
                var pattern = YearMonthPattern.CreateWithCurrentCulture("uuuu/MM");
                Assert.AreEqual("2017-08", pattern.Format(yearMonth));
            }
        }

        [Test]
        public void CreateWithSpecificCulture()
        {
            var pattern = YearMonthPattern.Create("uuuu/MM", Cultures.FrCa);
            Assert.AreEqual("2017-08", pattern.Format(new YearMonth(2017, 8)));
        }

        [Test]
        public void CreateWithTemplateValue()
        {
            // This is testing the Create call specifying the template directly, instead of the WithTemplate method
            var template = new YearMonth(2000, 6);
            var pattern = YearMonthPattern.Create("uuuu", CultureInfo.InvariantCulture, template);
            Assert.AreEqual(template, pattern.TemplateValue);
            var parseResult = pattern.Parse("1990");
            Assert.True(parseResult.Success);
            Assert.AreEqual(new YearMonth(1990, 6), parseResult.Value);
        }

        [Test]
        public void ParseNull() => AssertParseNull(YearMonthPattern.Iso);

        public sealed class Data : PatternTestData<YearMonth>
        {
            // Default to the start of the year 2000.
            protected override YearMonth DefaultTemplate => YearMonthPattern.DefaultTemplateValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(YearMonth value) : base(value)
            {
            }

            public Data(int year, int month) : this(new YearMonth(year, month))
            {
            }

            public Data(int year, int month, CalendarSystem calendar)
                : this(new YearMonth(year, month, calendar))
            {
            }

            public Data() : this(YearMonthPattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<YearMonth> CreatePattern() =>
                YearMonthPattern.CreateWithInvariantCulture(Pattern!)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
        }
    }
}
