#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public partial class IsoPeriodFormatsTest
    {
        Period standardPeriodEmpty = new Period(0, 0, 0, 0, 0, 0, 0, 0);

        object[] StandardFormatterTestData =
        {
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 8), "P1Y2M3W4DT5H6M7.008S"),
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 0), "P1Y2M3W4DT5H6M7S"),
            new TestCaseData( new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime), "P1Y4DT5H6M7.008S"),
            new TestCaseData( new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime), "PT0S"),
            new TestCaseData( new Period(1, 2, 3, 4, 0, 0, 0, 0), "P1Y2M3W4D"),
            new TestCaseData( new Period(0, 0, 0, 0, 5, 6, 7, 8), "PT5H6M7.008S"),
        };

        [Test]
        [TestCaseSource("StandardFormatterTestData")]
        public void StandardFormatter_Prints(IPeriod period, string periodText)
        {
            Assert.That(IsoPeriodFormats.Standard.Print(period), Is.EqualTo(periodText));
        }

        [Test]
        [TestCaseSource("StandardFormatterTestData")]
        public void StandardFormatter_Parses(IPeriod period, string periodText)
        {
            Assert.That(IsoPeriodFormats.Standard.Parse(periodText), Is.EqualTo(standardPeriodEmpty.With(period)));
        }

        object[] AlternateFormatterTestData =
        {
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 8), "P00010204T050607.008"),
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 0), "P00010204T050607"),
            new TestCaseData( new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime), "P00010004T050607.008"),
            new TestCaseData( new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime), "P00000000T000000"),
            new TestCaseData( new Period(1, 2, 3, 4, 0, 0, 0, 0), "P00010204T000000"),
            new TestCaseData( new Period(0, 0, 0, 0, 5, 6, 7, 8), "P00000000T050607.008"),
        };


        [Test]
        [TestCaseSource("AlternateFormatterTestData")]
        public void AlternateFormatter_Prints(IPeriod period, string periodText)
        {
            Assert.That(IsoPeriodFormats.Alternate.Print(period), Is.EqualTo(periodText));
        }

        [Test]
        [TestCaseSource("AlternateFormatterTestData")]
        [Ignore("BUG")]
        public void AlternateFormatter_Parses(IPeriod period, string periodText)
        {
            Assert.That(IsoPeriodFormats.Alternate.Parse(periodText), Is.EqualTo(standardPeriodEmpty.With(period)));
        }


        object[] AlternateExtendedFormatterTestData =
        {
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 8)).Returns("P0001-02-04T05:06:07.008"),
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 0)).Returns("P0001-02-04T05:06:07"),
            new TestCaseData( new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime)).Returns("P0001-00-04T05:06:07.008"),
            new TestCaseData( new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime)).Returns("P0000-00-00T00:00:00"),
            new TestCaseData( new Period(1, 2, 3, 4, 0, 0, 0, 0)).Returns("P0001-02-04T00:00:00"),
            new TestCaseData( new Period(0, 0, 0, 0, 5, 6, 7, 8)).Returns("P0000-00-00T05:06:07.008"),
        };


        [Test]
        [TestCaseSource("AlternateExtendedFormatterTestData")]
        public string AlternateExtendedFormatter_Prints(IPeriod period)
        {
            return IsoPeriodFormats.AlternateExtended.Print(period);
        }

        object[] AlternateExtendedWithWeeksFormatterTestData =
        {
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 8)).Returns("P0001-W03-04T05:06:07.008"),
            new TestCaseData( new Period(1, 2, 3, 4, 5, 6, 7, 0)).Returns("P0001-W03-04T05:06:07"),
            new TestCaseData( new Period(1, 0, 0, 4, 5, 6, 7, 8, PeriodType.YearDayTime)).Returns("P0001-W00-04T05:06:07.008"),
            new TestCaseData( new Period(0, 0, 0, 0, 0, 0, 0, 0, PeriodType.YearDayTime)).Returns("P0000-W00-00T00:00:00"),
            new TestCaseData( new Period(1, 2, 3, 4, 0, 0, 0, 0)).Returns("P0001-W03-04T00:00:00"),
            new TestCaseData( new Period(0, 0, 0, 0, 5, 6, 7, 8)).Returns("P0000-W00-00T05:06:07.008"),
        };


        [Test]
        [TestCaseSource("AlternateExtendedWithWeeksFormatterTestData")]
        public string AlternateExtendedWithWeeksFormatter_Prints(IPeriod period)
        {
            return IsoPeriodFormats.AlternateExtendedWithWeeks.Print(period);
        }
    }
}
