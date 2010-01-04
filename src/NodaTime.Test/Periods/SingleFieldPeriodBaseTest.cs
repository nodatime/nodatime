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
using System;
using NodaTime.Fields;
using NodaTime.Periods;

namespace NodaTime.Test.Periods
{
    [TestFixture]
    public partial class SingleFieldPeriodBaseTest
    {
        [Test]
        public void Constructor_SetsTheSpecifiedValue()
        {
            var sut = new Single(10);
            Assert.AreEqual(10, sut.Value);
        }

        [Test]
        public void Size_AlwaysReturns1()
        {
            var sut = new Single(10);
            Assert.AreEqual(1, sut.Size);
        }

        [Test]
        public void GetFieldType_ReturnsSingleFieldType_ForZeroIndex()
        {
            var sut = new Single(10);
            Assert.AreEqual(sut.FieldType, sut.GetFieldType(0));
        }

        [Test]
        public void GetFieldType_ThrowsArgumentOutOfRange_ForNonZeroIndex()
        {            
            var sut = new Single(10);
            Assert.Throws<ArgumentOutOfRangeException>(()=> sut.GetFieldType(1));
        }

        [Test]
        public void GetValue_ReturnsSingleValue_ForZeroIndex()
        {
            var sut = new Single(10);
            Assert.AreEqual(sut.Value, sut.GetValue(0));
        }

        [Test]
        public void GetValue_ThrowsArgumentOutOfRange_ForNonZeroIndex()
        {
            var sut = new Single(10);
            Assert.Throws<ArgumentOutOfRangeException>(() => sut.GetValue(1));
        }

        [Test]
        public void Get_ReturnsSingleValue_ForTheSupportedDurationFieldType()
        {
            var sut = new Single(10);
            Assert.AreEqual(sut.Value, sut.Get(DurationFieldType.Centuries));
        }

        [Test]
        public void Get_ReturnsZero_ForUnsupportedDurationFieldType()
        {
            var sut = new Single(10);
            Assert.AreEqual(0, sut.Get(DurationFieldType.Days));
        }

        [Test]
        public void IsSupported_ReturnsTrue_ForTheSupportedDurationFieldType()
        {
            var sut = new Single(10);
            Assert.True(sut.IsSupported(DurationFieldType.Centuries));
        }

        [Test]
        public void IsSupported_ReturnsFalse_ForUnsupportedDurationFieldType()
        {
            var sut = new Single(10);
            Assert.False(sut.IsSupported(DurationFieldType.Days));
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void ToPeriod_ReturnsStandardPeriodWithDays()
        {
            var sut = new Single(20);
            Assert.AreEqual(Period.FromDays(20), sut.ToPeriod());
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsClass<SingleFieldPeriodBase>(new Single(2),
                new Single(2), new Single(4));

            TestHelper.TestEqualsClass<SingleFieldPeriodBase>(new Single(2),
                new Single(2), new Single2(2));

            TestHelper.TestOperatorEquality<SingleFieldPeriodBase>(new Single(2),
                new Single(2), new Single(4));
        }

        [Test]
        public void Comparison()
        {
            TestHelper.TestCompareToClass<SingleFieldPeriodBase>(new Single(2),
                new Single(2), new Single(4));

            TestHelper.TestOperatorComparisonEquality<SingleFieldPeriodBase>(new Single(2),
                new Single(2), new Single(4));
        }
    }
}
