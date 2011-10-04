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
using System.Globalization;
using NUnit.Framework;
using NodaTime.Globalization;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    [TestFixture]
    public partial class LocalDatePatternTest
    {
        private static readonly DateTime SampleDateTime = new DateTime(1976, 6, 19, 21, 13, 34, 123, DateTimeKind.Unspecified).AddTicks(4567);
        private static readonly LocalDate SampleLocalDate = new LocalDate(1976, 6, 19);

        [Test]
        [TestCaseSource("InvalidPatternData")]
        public void InvalidPatterns(Data data)
        {
            data.TestInvalidPattern();
            Console.WriteLine(data.Pattern);
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
        [TestCaseSource("AllCultures")]
        public void BclLongDatePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.LongDatePattern);
        }

        [Test]
        [TestCaseSource("AllCultures")]
        public void BclShortTimePatternGivesSameResultsInNoda(CultureInfo culture)
        {
            AssertBclNodaEquality(culture, culture.DateTimeFormat.ShortDatePattern);
        }

        private void AssertBclNodaEquality(CultureInfo culture, string patternText)
        {
            var pattern = LocalDatePattern.Create(patternText, NodaFormatInfo.GetFormatInfo(culture));

            Assert.AreEqual(SampleDateTime.ToString(patternText, culture), pattern.Format(SampleLocalDate));
        }
    }
}
