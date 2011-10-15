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
    public partial class LocalTimePatternTest
    {
        // Used by tests via reflection - do not remove!
        private static readonly IEnumerable<CultureInfo> AllCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures).ToList();

        private static readonly DateTime SampleDateTime = new DateTime(2000, 1, 1, 21, 13, 34, 123, DateTimeKind.Unspecified).AddTicks(4567);
        private static readonly LocalTime SampleLocalTime = new LocalTime(21, 13, 34, 123, 4567);

        // Characters we expect to work the same in Noda Time as in the BCL.
        private static readonly string ExpectedCharacters = "hHms.:fFtT ";

        [Test]
        [TestCaseSource("AllCultures")]
        public void BclLongTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            AssertValidNodaPattern(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource("AllCultures")]
        public void BclShortTimePatternIsValidNodaPattern(CultureInfo culture)
        {
            AssertValidNodaPattern(culture, culture.DateTimeFormat.ShortTimePattern);
        }

        [Test]
        [TestCaseSource("AllCultures")]
        public void BclLongTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongTimePattern);
        }

        [Test]
        [TestCaseSource("AllCultures")]
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
        [TestCaseSource("DefaultPatternData")]
        public void FormatDefaultPattern(Data data)
        {
            Assert.AreEqual(data.Text, data.Value.ToString(data.Culture));
        }

        [Test]
        [TestCaseSource("DefaultPatternData")]
        public void ParseDefaultPattern(Data data)
        {
            Assert.AreEqual(data.Value, LocalTime.Parse(data.Text, data.Culture));
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            var pattern = LocalTimePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture));

            Assert.AreEqual(SampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalTime));
        }

        private static void AssertValidNodaPattern(CultureInfo culture, string pattern)
        {
            Assert.IsTrue(pattern.All(c => ExpectedCharacters.Contains(c)),
                "Culture {0} uses pattern '{1}' which contains unexpected characters",
                culture.Name, pattern);
            // Check that the pattern parses
            LocalTimePattern.Create(pattern, NodaFormatInfo.GetFormatInfo(culture));
        }
    }
}
