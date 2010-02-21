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
    public partial class SecondsTest
    {
        [Test]
        public void FieldType_ReturnsMinutesAlways()
        {
            Assert.AreEqual(DurationFieldType.Seconds, Seconds.From(3).FieldType);
            Assert.AreEqual(DurationFieldType.Seconds, Seconds.From(-3).FieldType);
        }

        [Test]
        public void PeriodType_ReturnsSecondsAlways()
        {
            Assert.AreEqual(PeriodType.Seconds, Seconds.From(6).PeriodType);
            Assert.AreEqual(PeriodType.Seconds, Seconds.From(-6).PeriodType);
        }

        [Test]
        public void ToString_ReturnsCorrectStrings()
        {
            Assert.AreEqual("PT20S", Seconds.From(20).ToString());
            Assert.AreEqual("PT-20S", Seconds.From(-20).ToString());
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsClass(Seconds.From(42), Seconds.From(42), Seconds.From(24));
            TestHelper.TestOperatorEquality(Seconds.From(42), Seconds.From(42), Seconds.From(24));
        }

        [Test]
        public void Comparison()
        {
            TestHelper.TestCompareToClass(Seconds.From(55), Seconds.From(55), Seconds.From(66));
            TestHelper.TestOperatorComparisonEquality(Seconds.From(55), Seconds.From(55), Seconds.From(66));
        }
    }
}
