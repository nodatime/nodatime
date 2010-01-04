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
using NUnit.Framework;
using NodaTime.Periods;
using NodaTime.Fields;
using System;

namespace NodaTime.Test.Periods
{
    public partial class MonthsTest
    {
        [Test]
        public void Parse_ReturnsZero_ForNullOrEmptyString()
        {
            var sut = Months.Parse(null);
            Assert.AreEqual(0, sut.Value);

            sut = Months.Parse(string.Empty);
            Assert.AreEqual(0, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_ReturnsZero_ForZeroMonthsString()
        {
            var sut = Months.Parse("P0M");
            Assert.AreEqual(0, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_Returns1_For1MonthString()
        {
            var sut = Months.Parse("P1M");
            Assert.AreEqual(1, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_ReturnsMinus3_ForMinus3MonthsString()
        {
            var sut = Months.Parse("P-3M");
            Assert.AreEqual(-3, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_Returns2_ForZeroYears2MonthString()
        {
            var sut = Months.Parse("POY2M");
            Assert.AreEqual(2, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_Returns2_For2YearZeroTimeString()
        {
            var sut = Years.Parse("P2YT0H0M");
            Assert.AreEqual(2, sut.Value);
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Parse_Throws_ForWrongString()
        {
            Assert.Throws<ArgumentException>(() => Months.Parse("P1MT1H"));
        }
    }
}
