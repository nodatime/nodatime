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
using NodaTime.Fields;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    [TestFixture]
    public partial class DaysTest
    {
        [Test]
        public void FieldType_ReturnsDaysAlways()
        {
            Assert.AreEqual(DurationFieldType.Days, Days.From(3).FieldType);
            Assert.AreEqual(DurationFieldType.Days, Days.From(-3).FieldType);
        }

        [Test]
        public void PeriodType_ReturnsDaysAlways()
        {
            Assert.AreEqual(PeriodType.Days, Days.From(6).PeriodType);
            Assert.AreEqual(PeriodType.Days, Days.From(-6).PeriodType);
        }

        [Test]
        public void ToString_ReturnsCorrectStrings()
        {
            Assert.AreEqual("P20D", Days.From(20).ToString());
            Assert.AreEqual("P-20D", Days.From(-20).ToString());
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsClass(Days.From(42), Days.From(42), Days.From(24));
            TestHelper.TestOperatorEquality(Days.From(42), Days.From(42), Days.From(24));
        }

        [Test]
        public void Comparison()
        {
            TestHelper.TestCompareToClass(Days.From(55), Days.From(55), Days.From(66));
            TestHelper.TestOperatorComparisonEquality(Days.From(55), Days.From(55), Days.From(66));
        }
    }
}
