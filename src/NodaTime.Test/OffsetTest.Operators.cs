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

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        [Test]
        public void IEquatableIComparable_Tests()
        {
            var value = Offset.FromMilliseconds(12345);
            var equalValue = Offset.FromMilliseconds(12345);
            var greaterValue = Offset.FromMilliseconds(5432199);

            TestHelper.TestEqualsStruct(value, equalValue, greaterValue);
            TestHelper.TestCompareToStruct(value, equalValue, greaterValue);
            TestHelper.TestOperatorComparisonEquality(value, equalValue, greaterValue);
        }

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(Offset.Zero, +Offset.Zero, "+ 0");
            Assert.AreEqual(Offset.FromMilliseconds(1), + Offset.FromMilliseconds(1), "+ 1");
            Assert.AreEqual(Offset.FromMilliseconds(-7), + Offset.FromMilliseconds(-7), "+ (-7)");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(Offset.Zero, -Offset.Zero, "-0");
            Assert.AreEqual(Offset.FromMilliseconds(-1), -Offset.FromMilliseconds(1), "-1");
            Assert.AreEqual(Offset.FromMilliseconds(7), -Offset.FromMilliseconds(-7), "- (-7)");
        }

        #region operator +
        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0, (Offset.Zero + Offset.Zero).Milliseconds, "0 + 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), ThreeHours + Offset.Zero, "1 + 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), Offset.Zero + ThreeHours, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(MakeOffset(6, 0, 0, 0), ThreeHours + ThreeHours, "3,000,000 + 1");
            Assert.AreEqual(Offset.Zero, ThreeHours + NegativeThreeHours, "1 + (-1)");
            Assert.AreEqual(MakeOffset(-9, 0, 0, 0), NegativeTwelveHours + ThreeHours, "-TwelveHours + threeHours");
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Offset.Zero, Offset.Zero - Offset.Zero, "0 - 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), ThreeHours - Offset.Zero, "1 - 0");
            Assert.AreEqual(MakeOffset(-3, 0, 0, 0), Offset.Zero - ThreeHours, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(Offset.Zero, ThreeHours - ThreeHours, "3,000,000 - 1");
            Assert.AreEqual(MakeOffset(6, 0, 0, 0), ThreeHours - NegativeThreeHours, "1 - (-1)");
            Assert.AreEqual(MakeOffset(-15, 0, 0, 0), NegativeTwelveHours - ThreeHours, "-TwelveHours - threeHours");
        }
        #endregion
    }
}