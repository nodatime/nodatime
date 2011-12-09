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
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public partial class LocalDateTimePatternTest
    {
        private static readonly LocalDateTime SampleLocalDateTime = new LocalDateTime(1376, 6, 19, 21, 13, 34, 123, 4567);
        private static readonly LocalDateTime SampleLocalDateTimeNoTicks = new LocalDateTime(1376, 6, 19, 21, 13, 34, 123);
        private static readonly LocalDateTime SampleLocalDateTimeNoMillis = new LocalDateTime(1376, 6, 19, 21, 13, 34);
        private static readonly LocalDateTime SampleLocalDateTimeNoSeconds = new LocalDateTime(1376, 6, 19, 21, 13);
        private static readonly string[] AllStandardPatterns = { "f", "F", "g", "G", "o", "O", "s" };
        private static readonly IEnumerable<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();

#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        // TODO: Uncomment the code below when NCrunch supports names properly.
        private static readonly object[] AllCulturesStandardPatterns = (from culture in AllCultures
                                                                        from format in AllStandardPatterns
                                                                        select new TestCaseData(culture, format)/*.SetName(culture + ": " + format)*/).ToArray();
#pragma warning restore 0414

        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(Data data)
        {
            data.TestInvalidPattern();
        }

        [Test]
        [TestCaseSource("ParseFailureData")]
        public void ParseFailures(Data data)
        {
            data.TestParseFailure();
        }

        [Test]
        [TestCaseSource("ParseData")]
        public void Parse(Data data)
        {
            data.TestParse();
        }

        [Test]
        [TestCaseSource("FormatData")]
        public void Format(Data data)
        {
            data.TestFormat();
        }

        [Test]
        [TestCaseSource("AllCulturesStandardPatterns")]
        public void BclStandardPatternComparison(CultureInfo culture, string pattern)
        {
            AssertBclNodaEquality(culture, pattern);
        }

        [Test]
        [TestCaseSource("AllCulturesStandardPatterns")]
        public void ParseFormattedStandardPattern(CultureInfo culture, string patternText)
        {
            if (culture.ToString() == "af-ZA")
            {
                // Still need to handle Afrikaans having just a PM designator.
                return;
            }
            // Some cultures use two-digit years, so let's put them in the right century.
            var pattern = LocalDateTimePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture))
                .WithTemplateValue(new LocalDateTime(1400, 1, 1, 0, 0));
            string formatted = pattern.Format(SampleLocalDateTime);
            var parseResult = pattern.Parse(formatted);
            Assert.IsTrue(parseResult.Success);
            var parsed = parseResult.Value;
            Assert.IsTrue(parsed == SampleLocalDateTime ||
                          parsed == SampleLocalDateTimeNoTicks ||
                          parsed == SampleLocalDateTimeNoMillis ||
                          parsed == SampleLocalDateTimeNoSeconds);
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            var pattern = LocalDateTimePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture));
            // Create the BCL version in the culture's calendar, so that when formatted it really will have those
            // values, even though that may represent a completely different date/time to the Noda Time version...
            // we're only testing the formatting here.

            // Formatting a DateTime with an always-invariant pattern (round-trip, sortable) does the wrong thing.
            // Use the Gregorian calendar for those tests.
            // TODO: Check this actually makes sense... maybe we ought to be converting to the ISO calendar in that case.
            Calendar calendar = "Oos".Contains(patternText) ? CultureInfo.InvariantCulture.Calendar : culture.Calendar;
            // Note that we're using Jon's -600th birthday so as to be in the right year range for the Saudi calendar.
            DateTime sampleDateTime = new DateTime(1376, 6, 19, 21, 13, 34, 123, calendar,
                                                   DateTimeKind.Unspecified).AddTicks(4567);

            Assert.AreEqual(sampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalDateTime));
        }
    }
}
