// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using NodaTime.Properties;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public class LocalDatePatternTest : PatternTestBase<LocalDate>
    {
        private static readonly LocalDate SampleLocalDate = new LocalDate(1976, 6, 19);

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
            // Era specifier and calendar specifier in the same pattern.
            new Data { Pattern = "dd MM YYYY gg c", Message = Messages.Parse_CalendarAndEra },

            // Invalid patterns directly after the yyyy specifier. This will detect the issue early, but then
            // continue and reject it in the normal path.
            new Data { Pattern = "yyyy'", Message = Messages.Parse_MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "yyyy\\", Message = Messages.Parse_EscapeAtEndOfString },
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

            // Invalid calendar name
            new Data { Pattern = "c yyyy MM dd", Text = "2015 01 01", Message = Messages.Parse_NoMatchingCalendarSystem },

            // Invald year
            new Data { Pattern = "yyyyy", Text = "55555", Message = Messages.Parse_FieldValueOutOfRange, Parameters = { 55555, 'y', typeof(LocalDate) } },
            new Data { Pattern = "YYYYY", Text = "55555", Message = Messages.Parse_YearOfEraOutOfRange, Parameters = { 55555, "CE", "ISO" } },

            // Conservative yyyy parsing.
            new Data(12345, 1, 1) { Pattern = "yyyyMMdd", Text = "123450101", Message = Messages.Parse_MonthOutOfRange, Parameters = { 50, 1234 }},
            new Data(-12345, 1, 1) { Pattern = "yyyyMMdd", Text = "-123450101", Message = Messages.Parse_MonthOutOfRange, Parameters = { 50, -1234 }},
            new Data(19999, 1, 1) { Pattern = "yyyyVMM dd", Text = "19999V01 01", Message = Messages.Parse_MismatchedCharacter, Parameters = { 'V' }},
            new Data(19999, 1, 1) { Pattern = "yyyy%VMM dd", Text = "19999V01 01", Message = Messages.Parse_MismatchedCharacter, Parameters = { 'V' } },
            new Data(19999, 1, 1) { Pattern = "yyyy'0'MM dd", Text = "19999001 01", Message = Messages.Parse_QuotedStringMismatch, Parameters = { '0' }},
            new Data(19999, 1, 1) { Pattern = "yyyy''0MM dd", Text = "19999001 01", Message = Messages.Parse_MismatchedCharacter, Parameters = { '0' }},
            new Data(19999, 1, 1) { Pattern = @"yyyy\0MM dd", Text = "19999001 01", Message = Messages.Parse_EscapedCharacterMismatch, Parameters = { '0' }},
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
            new Data(2011, 1, 10) { Pattern = "yyyy MMMM dd", Text = "2011 MonthName-Genitive 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMMM dd", Text = "2011 MonthName 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMM dd", Text = "2011 MN-Gen 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(2011, 1, 10) { Pattern = "yyyy MMM dd", Text = "2011 MN 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
        };

        internal static Data[] FormatOnlyData = {
            // Would parse back to 2011
            new Data(1811, 7, 3) { Pattern = "yy M d", Text = "11 7 3" },

            // We can't round-trip this data, but at least we don't break when formatting.
            new Data(12345, 1, 1) { Pattern = "yyyyMMdd", Text = "123450101" },
            new Data(-12345, 1, 1) { Pattern = "yyyyMMdd", Text = "-123450101" },
            new Data(19999, 1, 1) { Pattern = "yyyyVMM dd", Text = "19999V01 01" },
            new Data(19999, 1, 1) { Pattern = "yyyy%VMM dd", Text = "19999V01 01" },
            new Data(19999, 1, 1) { Pattern = "yyyy'0'MM dd", Text = "19999001 01" },
            new Data(19999, 1, 1) { Pattern = "yyyy''0MM dd", Text = "19999001 01" },
            new Data(19999, 1, 1) { Pattern = @"yyyy\0MM dd", Text = "19999001 01" },
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
            new Data(2011, 1, 3) { Pattern = "MMMM", Text = "FullNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3)},
            new Data(2011, 1, 3) { Pattern = "MMMM dddd", Text = "FullNonGenName Monday", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            new Data(2011, 1, 3) { Pattern = "MMM", Text = "AbbrNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            new Data(2011, 1, 3) { Pattern = "MMM ddd", Text = "AbbrNonGenName Mon", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            // Genitive month name when the pattern includes "day of month"
            new Data(2011, 1, 3) { Pattern = "MMMM dd", Text = "FullGenName 03", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },
            // TODO(V1.2): Check whether or not this is actually appropriate
            new Data(2011, 1, 3) { Pattern = "MMM dd", Text = "AbbrGenName 03", Culture = Cultures.GenitiveNameTestCulture, Template = new LocalDate(2011, 5, 3) },

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

            // Calendar handling
            new Data(2011, 10, 9) { Pattern = "c yyyy MM dd", Text = "ISO 2011 10 09" },
            new Data(2011, 10, 9) { Pattern = "yyyy MM dd c", Text = "2011 10 09 ISO" },
            new Data(2011, 10, 9, CalendarSystem.GetCopticCalendar(4)) { Pattern = "c yyyy MM dd", Text = "Coptic 4 2011 10 09" },
            new Data(2011, 10, 9, CalendarSystem.GetCopticCalendar(4)) { Pattern = "yyyy MM dd c", Text = "2011 10 09 Coptic 4" },

            // Awkward day-of-week handling
            // December 14th 2012 was a Friday. Friday is "Foo" or "FooBar" in AwkwardDayOfWeekCulture.
            new Data(2012, 12, 14) { Pattern = "ddd yyyy MM dd", Text = "Foo 2012 12 14", Culture = Cultures.AwkwardDayOfWeekCulture },
            new Data(2012, 12, 14) { Pattern = "dddd yyyy MM dd", Text = "FooBar 2012 12 14", Culture = Cultures.AwkwardDayOfWeekCulture },
            // December 13th 2012 was a Thursday. Friday is "FooBaz" or "FooBa" in AwkwardDayOfWeekCulture.
            new Data(2012, 12, 13) { Pattern = "ddd yyyy MM dd", Text = "FooBaz 2012 12 13", Culture = Cultures.AwkwardDayOfWeekCulture },
            new Data(2012, 12, 13) { Pattern = "dddd yyyy MM dd", Text = "FooBa 2012 12 13", Culture = Cultures.AwkwardDayOfWeekCulture },

            // 5 digit year handling
            new Data(19999, 1, 1) { Pattern = "yyyyy MM dd", Text = "19999 01 01" },
            new Data(-19999, 1, 1) { Pattern = "yyyyy MM dd", Text = "-19999 01 01" },
            new Data(9999, 1, 1) { Pattern = "yyyyy MM dd", Text = "09999 01 01" },
            new Data(-9999, 1, 1) { Pattern = "yyyyy MM dd", Text = "-09999 01 01" },

            // 4-or-5 digit year handling. The number of digits to parse years up to is
            // set to 5 because the following character is a space.
            new Data(19999, 1, 1) { Pattern = "yyyy MM dd", Text = "19999 01 01" },
            new Data(-19999, 1, 1) { Pattern = "yyyy MM dd", Text = "-19999 01 01" },
            new Data(9999, 1, 1) { Pattern = "yyyy MM dd", Text = "9999 01 01" },
            new Data(-9999, 1, 1) { Pattern = "yyyy MM dd", Text = "-9999 01 01" },
            new Data(999, 1, 1) { Pattern = "yyyy MM dd", Text = "0999 01 01" },
            new Data(-999, 1, 1) { Pattern = "yyyy MM dd", Text = "-0999 01 01" },

            // Year at end of pattern: can't be ambiguous.
            new Data(19999, 1, 1) { Pattern = "ddMMyyyy", Text = "010119999" },

            // Quoted non-digit after year: can't be ambiguous.
            new Data(19999, 1, 1) { Pattern = "yyyy'Y'MM dd", Text = "19999Y01 01" },
            new Data(19999, 1, 1) { Pattern = "yyyy'V'MM dd", Text = "19999V01 01" },
            new Data(19999, 1, 1) { Pattern = "yyyy'V0'MM dd", Text = "19999V001 01" },
            new Data(19999, 1, 1) { Pattern = @"yyyy\VMM dd", Text = "19999V01 01" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);
        
        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclLongDatePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            // See https://bugzilla.xamarin.com/show_bug.cgi?id=11363
            if (TestHelper.IsRunningOnMono && culture.IetfLanguageTag == "mt-MT")
            {
                return;
            }
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongDatePattern);
        }

        [Test]
        [TestCaseSource(typeof(Cultures), "AllCultures")]
        public void BclShortDatePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.ShortDatePattern);
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            var pattern = LocalDatePattern.Create(patternText, culture);
            var calendarSystem = CalendarSystemForCalendar(culture.Calendar);
            if (calendarSystem == null)
            {
                // We can't map this calendar system correctly yet; the test would be invalid.
                return;
            }

            var sampleDateInCalendar = SampleLocalDate.WithCalendar(calendarSystem);
            // To construct a DateTime, we need a time... let's give a non-midnight one to catch
            // any unexpected uses of time within the date patterns.
            DateTime sampleDateTime = (SampleLocalDate + new LocalTime(2, 3, 5)).ToDateTimeUnspecified();
            Assert.AreEqual(sampleDateTime.ToString(patternText, culture), pattern.Format(sampleDateInCalendar));
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

            public Data(int year, int month, int day, CalendarSystem calendar)
                : this(new LocalDate(year, month, day, calendar))
            {
            }

            public Data()
                : this(LocalDatePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<LocalDate> CreatePattern()
            {
                return LocalDatePattern.CreateWithInvariantCulture(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
            }
        }
    }
}
