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
    public partial class YearsTest
    {
        [Test]
        public void Parse_ReturnsZero_ForNullOrEmptyString()
        {
            var sut = Years.Parse(null);
            Assert.AreEqual(0, sut.Value);

            sut = Years.Parse(string.Empty);
            Assert.AreEqual(0, sut.Value);
        }

        [Test]
        public void Parse_ReturnsZero_ForZeroYearsString()
        {
            var sut = Years.Parse("P0Y");
            Assert.AreEqual(0, sut.Value);
        }

        [Test]
        public void Parse_Returns1_For1YearString()
        {
            var sut = Years.Parse("P1Y");
            Assert.AreEqual(1, sut.Value);
        }

        [Test]
        public void Parse_ReturnsMinus3_ForMinus3YearsString()
        {
            var sut = Years.Parse("P-3Y");
            Assert.AreEqual(-3, sut.Value);
        }

        [Test]
        public void Parse_Returns2_For2YearZeroMonthsString()
        {
            var sut = Years.Parse("P2Y0M");
            Assert.AreEqual(2, sut.Value);
        }

        [Test]
        public void Parse_Returns2_For2YearZeroTimeString()
        {
            var sut = Years.Parse("P2YT0H0M");
            Assert.AreEqual(2, sut.Value);
        }

        [Test]
        public void Parse_Throws_ForWrongString()
        {
            Assert.Throws<NotSupportedException>(() => Years.Parse("P1MT1H"));
        }
    }
}
