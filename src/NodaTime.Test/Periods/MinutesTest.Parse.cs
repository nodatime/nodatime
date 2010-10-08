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

using System;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class MinutesTest
    {
        private object[] ParseCorrectTestData = {
            new TestCaseData(null, 0), new TestCaseData(String.Empty, 0), new TestCaseData("PT0M", 0),
            new TestCaseData("PT1M", 1), new TestCaseData("PT-3M", -3), new TestCaseData("P0Y0M0W0DT2M", 2), new TestCaseData("PT0H2M", 2),
        };

        [Test]
        [TestCaseSource("ParseCorrectTestData")]
        public void Parse_CorrectString(string minutesString, int expectedMinutesValue)
        {
            var sut = Minutes.Parse(minutesString);
            Assert.AreEqual(expectedMinutesValue, sut.Value);
        }

        private object[] ParseWrongTestData = { new TestCaseData("P1Y1D"), new TestCaseData("P1DT1M"), };

        [Test]
        [TestCaseSource("ParseWrongTestData")]
        public void Parse_ThrowsNotSupported_ForWrongString(string minutesString)
        {
            Assert.Throws<NotSupportedException>(() => Minutes.Parse(minutesString));
        }
    }
}