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

using NodaTime.Format;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Format
{
    [TestFixture]
    public class PeriodFormatsTest
    {
        private readonly Period standardPeriodEmpty = new Period(0, 0, 0, 0, 0, 0, 0, 0);

        private object[] DefaultFormatterTestData = {
            new TestCaseData(new Period(0, 0, 0, 1, 5, 6, 7, 8), "1 day, 5 hours, 6 minutes, 7 seconds and 8 milliseconds"),
            new TestCaseData(new Period(6, 3, 0, 2, 0, 0, 0, 0), "6 years, 3 months and 2 days"), new TestCaseData(Period.FromDays(2), "2 days"),
            new TestCaseData(Period.FromDays(2).WithHours(5), "2 days and 5 hours"),
        };

        [Test]
        [TestCaseSource("DefaultFormatterTestData")]
        public void DefaultFormatter_Prints(IPeriod period, string periodText)
        {
            Assert.That(PeriodFormats.Default.Print(period), Is.EqualTo(periodText));
        }

        [Test]
        [TestCaseSource("DefaultFormatterTestData")]
        public void DefaultFormatter_Parses(IPeriod period, string periodText)
        {
            Assert.That(PeriodFormats.Default.Parse(periodText), Is.EqualTo(standardPeriodEmpty.With(period)));
        }
    }
}