// Copyright 2018 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NodaTime.Test.Text
{
    public class AnnualDatePatternTest : PatternTestBase<AnnualDate>
    {
        internal static readonly Data[] InvalidPatternData = {
            new Data { Pattern = "", Message = TextErrorMessages.FormatStringEmpty },
            new Data { Pattern = "!", Message = TextErrorMessages.UnknownStandardFormat, Parameters = {'!', typeof(AnnualDate).FullName }},
            new Data { Pattern = "%", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '%', typeof(AnnualDate).FullName } },
            new Data { Pattern = "\\", Message = TextErrorMessages.UnknownStandardFormat, Parameters = { '\\', typeof(AnnualDate).FullName } },
            new Data { Pattern = "%%", Message = TextErrorMessages.PercentDoubled },
            new Data { Pattern = "%\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "MMMMM", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'M', 4 } },
            new Data { Pattern = "ddd", Message = TextErrorMessages.RepeatCountExceeded, Parameters = { 'd', 2 } },
            new Data { Pattern = "M%", Message = TextErrorMessages.PercentAtEndOfString },
            new Data { Pattern = "'qwe", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },
            new Data { Pattern = "'qwe\\", Message = TextErrorMessages.EscapeAtEndOfString },
            new Data { Pattern = "'qwe\\'", Message = TextErrorMessages.MissingEndQuote, Parameters = { '\'' } },

            // Common typo (m doesn't mean months)
            new Data { Pattern = "mm-dd", Message = TextErrorMessages.UnquotedLiteral, Parameters = { 'm' } },
            // T isn't valid in a date pattern
            new Data { Pattern = "MM-ddT00:00:00", Message = TextErrorMessages.UnquotedLiteral, Parameters = { 'T' } },
        };

        internal static Data[] ParseFailureData = {
            new Data { Pattern = "MM dd MMMM", Text = "10 09 January", Message = TextErrorMessages.InconsistentMonthTextValue },
            new Data { Pattern = "MM dd MMMM", Text = "10 09 FooBar", Message = TextErrorMessages.MismatchedText, Parameters = {'M'} },
            new Data { Pattern = "MM/dd", Text = "02-29", Message = TextErrorMessages.DateSeparatorMismatch },
            // Don't match a short name against a long pattern
            new Data { Pattern = "MMMM dd", Text = "Oct 09", Message = TextErrorMessages.MismatchedText, Parameters = {'M'} },
            // Or vice versa... although this time we match the "Oct" and then fail as we're expecting a space
            new Data { Pattern = "MMM dd", Text = "October 09", Message = TextErrorMessages.MismatchedCharacter, Parameters = {' '}},

            // Invalid month, day
            new Data { Pattern = "MM dd", Text = "15 29", Message = TextErrorMessages.IsoMonthOutOfRange, Parameters = { 15 } },
            new Data { Pattern = "MM dd", Text = "02 35", Message = TextErrorMessages.DayOfMonthOutOfRangeNoYear, Parameters = { 35, 2 } },
        };

        internal static Data[] ParseOnlyData = {
            // Month parsing should be case-insensitive
            new Data(10, 3) { Pattern = "MMM dd", Text = "OcT 03" },
            new Data(10, 3) { Pattern = "MMMM dd", Text = "OcToBeR 03" },

            // Genitive name is an extension of the non-genitive name; parse longer first.
            new Data(1, 10) { Pattern = "MMMM dd", Text = "MonthName-Genitive 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(1, 10) { Pattern = "MMMM dd", Text = "MonthName 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(1, 10) { Pattern = "MMM dd", Text = "MN-Gen 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
            new Data(1, 10) { Pattern = "MMM dd", Text = "MN 10", Culture = Cultures.GenitiveNameTestCultureWithLeadingNames },
        };

        internal static Data[] FormatOnlyData = { };

        internal static Data[] FormatAndParseData = {
            // Standard patterns
            new Data(10, 20) { Pattern = "G", Text = "10-20" },

            // Custom patterns
            new Data(10, 3) { Pattern = "MM/dd", Text = "10/03" },
            new Data(10, 3) { Pattern = "MM/dd", Text = "10-03", Culture = Cultures.FrCa },
            new Data(10, 3) { Pattern = "MMdd", Text = "1003" },
            new Data(7, 3) { Pattern = "M d", Text = "7 3" },

            // Template value provides the month when we only specify the day
            new Data(5, 10) { Pattern = "dd", Text = "10", Template = new AnnualDate(5, 20)},
            // Template value provides the day when we only specify the month
            new Data(10, 20) { Pattern = "MM", Text = "10", Template = new AnnualDate(5, 20)},

            // When we parse in all of the below tests, we'll use the month and day-of-month if it's provided;
            // the template value is specified to allow simple roundtripping.
            // Non-genitive month name when there's no "day of month"
            new Data(1, 3) { Pattern = "MMMM", Text = "FullNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new AnnualDate(5, 3)},
            new Data(1, 3) { Pattern = "MMM", Text = "AbbrNonGenName", Culture = Cultures.GenitiveNameTestCulture, Template = new AnnualDate(5, 3) },
            // Genitive month name when the pattern includes "day of month"
            new Data(1, 3) { Pattern = "MMMM dd", Text = "FullGenName 03", Culture = Cultures.GenitiveNameTestCulture, Template = new AnnualDate(5, 3) },
            // TODO: Check whether or not this is actually appropriate
            new Data(1, 3) { Pattern = "MMM dd", Text = "AbbrGenName 03", Culture = Cultures.GenitiveNameTestCulture, Template = new AnnualDate(5, 3) },

            // Month handling with both text and numeric
            new Data(10, 9) { Pattern = "MMMM dd MM", Text = "October 09 10" },
            new Data(10, 9) { Pattern = "MMM dd MM", Text = "Oct 09 10" },
        };

        internal static IEnumerable<Data> ParseData = ParseOnlyData.Concat(FormatAndParseData);
        internal static IEnumerable<Data> FormatData = FormatOnlyData.Concat(FormatAndParseData);
        
        [Test]
        public void CreateWithCurrentCulture()
        {
            var date = new AnnualDate(8, 23);
            using (CultureSaver.SetCultures(Cultures.FrFr))
            {
                var pattern = AnnualDatePattern.CreateWithCurrentCulture("MM/dd");
                Assert.AreEqual("08/23", pattern.Format(date));
            }
            using (CultureSaver.SetCultures(Cultures.FrCa))
            {
                var pattern = AnnualDatePattern.CreateWithCurrentCulture("MM/dd");
                Assert.AreEqual("08-23", pattern.Format(date));
            }
        }

        [TestCase("fr-FR", "08/23")]
        [TestCase("fr-CA", "08-23")]
        public void CreateWithCulture(string cultureId, string expected)
        {
            var date = new AnnualDate(8, 23);
            var culture = new CultureInfo(cultureId);
            var pattern = AnnualDatePattern.Create("MM/dd", culture);
            Assert.AreEqual(expected, pattern.Format(date));
        }

        [TestCase("fr-FR", "08/23")]
        [TestCase("fr-CA", "08-23")]
        public void CreateWithCultureAndTemplateValue(string cultureId, string expected)
        {
            var date = new AnnualDate(8, 23);
            var template = new AnnualDate(5, 3);
            var culture = new CultureInfo(cultureId);
            // Check the culture is still used
            var pattern1 = AnnualDatePattern.Create("MM/dd", culture, template);
            Assert.AreEqual(expected, pattern1.Format(date));
            // And the template value
            var pattern2 = AnnualDatePattern.Create("MM", culture, template);
            var parsed = pattern2.Parse("08").Value;
            Assert.AreEqual(new AnnualDate(8, 3), parsed);
        }

        [Test]
        public void ParseNull() => AssertParseNull(AnnualDatePattern.Iso);
        
        public sealed class Data : PatternTestData<AnnualDate>
        {
            // Default to January 1st
            protected override AnnualDate DefaultTemplate => AnnualDatePattern.DefaultTemplateValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="Data" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Data(AnnualDate value) : base(value)
            {
            }

            public Data(int month, int day) : this(new AnnualDate(month, day))
            {
            }            

            public Data() : this(AnnualDatePattern.DefaultTemplateValue)
            {
            }

            internal override IPattern<AnnualDate> CreatePattern() =>
                AnnualDatePattern.CreateWithInvariantCulture(Pattern)
                    .WithTemplateValue(Template)
                    .WithCulture(Culture);
        }
    }
}
