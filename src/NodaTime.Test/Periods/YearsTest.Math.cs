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

using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Periods
{
    public partial class YearsTest
    {
        #region Negation
        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Years.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Years.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Years.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Years.One).Value, "-1");
            Assert.AreEqual(7, (-Years.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Years.Negate(Years.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Years.Negate(Years.Three)).Value, "-3");
            Assert.AreEqual(8, (Years.Negate(Years.From(-8))).Value, "- (-8)");
        }
        #endregion

        #region Unary Operators
        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Years.Zero).Value, "+0");
            Assert.AreEqual(1, (+Years.One).Value, "+1");
            Assert.AreEqual(-7, (+Years.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryPlusAlternativeMethod()
        {
            Assert.AreEqual(0, Years.Plus(Years.Zero).Value, "+0");
            Assert.AreEqual(1, Years.Plus(Years.One).Value, "+1");
            Assert.AreEqual(-7, Years.Plus(Years.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoYears = Years.Two;
            ++twoYears;

            Assert.AreEqual(3, (twoYears).Value, "++2 = 3");
        }

        [Test]
        public void UnaryIncrementAlternativeMethod()
        {
            var twoYears = Years.Two;
            var threeYears = Years.Increment(twoYears);

            Assert.AreEqual(3, (threeYears).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoYears = Years.Two;
            --twoYears;

            Assert.AreEqual(1, (twoYears).Value, "--2 = 1");
        }

        [Test]
        public void UnaryDecrementAlternativeMethod()
        {
            var twoYears = Years.Two;
            var oneYear = Years.Decrement(twoYears);

            Assert.AreEqual(1, (oneYear).Value, "--2 = 1");
        }
        #endregion

        #region Add
        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Years.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Years.One.Add(0).Value, "1 + 0");

            var twentyYears = Years.From(20);
            Assert.AreSame(twentyYears, twentyYears.Add(0), "20 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Years.Zero + Years.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Years.One + Years.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Years.Zero + Years.One).Value, "0 + 1");
            Assert.AreEqual(5, (Years.Two + Years.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Years.From(9) + Years.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Years.Add(Years.Zero, Years.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Years.Add(Years.One, Years.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Years.Add(Years.Zero, Years.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Years.Add(Years.Two, Years.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Years.Add(Years.From(9), Years.From(-8))).Value, "9 + (-8)");
        }
        #endregion

        #region Subtract
        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Years.Three.Subtract(2).Value);
            Assert.AreEqual(1, Years.One.Subtract(0).Value);

            var tenYears = Years.From(10);
            Assert.AreSame(tenYears, tenYears.Subtract(0), "10 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Years.Zero - Years.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Years.One - Years.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Years.Zero - Years.One).Value, "0 - 1");
            Assert.AreEqual(1, (Years.Three - Years.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Years.From(9) - Years.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Years.Subtract(Years.Zero, Years.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Years.Subtract(Years.One, Years.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Years.Subtract(Years.Zero, Years.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Years.Subtract(Years.Three, Years.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Years.Subtract(Years.From(9), Years.From(-1))).Value, "9 - (-1)");
        }
        #endregion

        #region Multiplication
        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Years.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Years.Two.Multiply(1).Value, "2 * 1");

            var twentyFiveYears = Years.From(25);
            Assert.AreSame(twentyFiveYears, twentyFiveYears.Multiply(1), "25 * 1");
        }

        [Test]
        public void MultiplyOperatorLeft()
        {
            Assert.AreEqual(1, (Years.One * 1).Value, "1 * 1");
            Assert.AreEqual(0, (Years.Two * 0).Value, "2 * 0");
            Assert.AreEqual(-3, (Years.Three * -1).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyOperatorRight()
        {
            Assert.AreEqual(1, (1 * Years.One).Value, "1 * 1");
            Assert.AreEqual(0, (0 * Years.Two).Value, "0 * 2");
            Assert.AreEqual(-3, (-1 * Years.Three).Value, "(-1) * 3");
        }

        [Test]
        public void MultiplyStaticLeft()
        {
            Assert.AreEqual(1, (Years.Multiply(Years.One, 1)).Value, "1 * 1");
            Assert.AreEqual(0, (Years.Multiply(Years.One, 0)).Value, "1 * 0");
            Assert.AreEqual(-9, (Years.Multiply(Years.From(9), -1)).Value, "9 * (-1)");
        }

        [Test]
        public void MultiplyStaticRight()
        {
            Assert.AreEqual(1, (Years.Multiply(1, Years.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Years.Multiply(0, Years.One)).Value, "0 * 1");
            Assert.AreEqual(-9, (Years.Multiply(-1, Years.From(9))).Value, "(-1) * 9");
        }
        #endregion

        #region Division
        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Years.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Years.Two.Divide(1).Value, "2 / 1");

            var twentySixYears = Years.From(26);
            Assert.AreSame(twentySixYears, twentySixYears.Divide(1), "26 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Years.One / 1).Value, "1 / 1");
            Assert.AreEqual(2, (Years.From(4) / 2).Value, "4 / 2");
            Assert.AreEqual(-3, (Years.Three / -1).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Years.Divide(Years.One, 1)).Value, "1 / 1");
            Assert.AreEqual(1, (Years.Divide(Years.Three, 2)).Value, "3 / 2");
            Assert.AreEqual(-3, (Years.Divide(Years.From(9), -3)).Value, "9 / (-3)");
        }
        #endregion

        #region Conversions
        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Years twoYears = Years.Two;
            int two = twoYears;

            Assert.AreEqual(2, two);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Years nullYears = null;
            int zero = nullYears;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Years threeYears = (Years)3;

            Assert.AreEqual(3, threeYears.Value);
        }
        #endregion
    }
}