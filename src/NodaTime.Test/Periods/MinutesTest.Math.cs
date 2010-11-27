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
    public partial class MinutesTest
    {
        #region Negation
        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Minutes.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Minutes.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Minutes.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Minutes.One).Value, "-1");
            Assert.AreEqual(7, (-Minutes.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Minutes.Negate(Minutes.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Minutes.Negate(Minutes.Three)).Value, "-3");
            Assert.AreEqual(8, (Minutes.Negate(Minutes.From(-8))).Value, "- (-8)");
        }
        #endregion

        #region Unary Operators
        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Minutes.Zero).Value, "+0");
            Assert.AreEqual(1, (+Minutes.One).Value, "+1");
            Assert.AreEqual(7, (+Minutes.From(+7)).Value, "+ (+7)");
        }

        [Test]
        public void UnaryPlusAlternativeMethod()
        {
            Assert.AreEqual(0, Minutes.Plus(Minutes.Zero).Value, "+0");
            Assert.AreEqual(1, Minutes.Plus(Minutes.One).Value, "+1");
            Assert.AreEqual(-7, Minutes.Plus(Minutes.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoMinutes = Minutes.Two;
            ++twoMinutes;

            Assert.AreEqual(3, (twoMinutes).Value, "++2 = 3");
        }

        [Test]
        public void UnaryIncrementAlternativeMethod()
        {
            var twoMinutes = Minutes.Two;
            var threMinutes = Minutes.Increment(twoMinutes);

            Assert.AreEqual(3, (threMinutes).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoMinutes = Minutes.Two;
            --twoMinutes;

            Assert.AreEqual(1, (twoMinutes).Value, "--2 = 1");
        }

        [Test]
        public void UnaryDecrementAlternativeMethod()
        {
            var twoMinutes = Minutes.Two;
            var oneMinute = --twoMinutes;

            Assert.AreEqual(1, (oneMinute).Value, "--2 = 1");
        }
        #endregion

        #region Add
        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Minutes.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Minutes.One.Add(0).Value, "1 + 0");

            var tenMinutes = Minutes.From(10);
            Assert.AreSame(tenMinutes, tenMinutes.Add(0), "10 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Minutes.Zero + Minutes.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Minutes.One + Minutes.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Minutes.Zero + Minutes.One).Value, "0 + 1");
            Assert.AreEqual(5, (Minutes.Two + Minutes.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Minutes.From(9) + Minutes.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Minutes.Add(Minutes.Zero, Minutes.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Minutes.Add(Minutes.One, Minutes.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Minutes.Add(Minutes.Zero, Minutes.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Minutes.Add(Minutes.Two, Minutes.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Minutes.Add(Minutes.From(9), Minutes.From(-8))).Value, "9 + (-8)");
        }
        #endregion

        #region Subtract
        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Minutes.Three.Subtract(2).Value, "3 - 2");
            Assert.AreEqual(1, Minutes.One.Subtract(0).Value, "1 - 0");

            var ten = Minutes.From(10);
            Assert.AreSame(ten, ten.Subtract(0), "10 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Minutes.Zero - Minutes.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Minutes.One - Minutes.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Minutes.Zero - Minutes.One).Value, "0 - 1");
            Assert.AreEqual(1, (Minutes.Three - Minutes.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Minutes.From(9) - Minutes.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Minutes.Subtract(Minutes.Zero, Minutes.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Minutes.Subtract(Minutes.One, Minutes.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Minutes.Subtract(Minutes.Zero, Minutes.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Minutes.Subtract(Minutes.Three, Minutes.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Minutes.Subtract(Minutes.From(9), Minutes.From(-1))).Value, "9 - (-1)");
        }
        #endregion

        #region Multiplication
        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Minutes.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Minutes.Two.Multiply(1).Value, "2 * 1");
            Assert.AreSame(Minutes.Zero, Minutes.Three.Multiply(0), "3 * 0");

            var ten = Minutes.From(10);
            Assert.AreSame(ten, ten.Multiply(1), "10 * 1");
        }

        [Test]
        public void MultiplyOperator_Left()
        {
            Assert.AreEqual(1, (Minutes.One * 1).Value, "1 * 1");
            Assert.AreEqual(0, (Minutes.Two * 0).Value, "2 * 0");
            Assert.AreEqual(-3, (Minutes.Three * -1).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyOperator_Right()
        {
            Assert.AreEqual(1, (1 * Minutes.One).Value, "1 * 1");
            Assert.AreEqual(0, (0 * Minutes.Two).Value, "0 * 2");
            Assert.AreEqual(-3, (-1 * Minutes.Three).Value, "(-1) * 3");
        }

        [Test]
        public void MultiplyStatic()
        {
            Assert.AreEqual(1, (Minutes.Multiply(Minutes.One, 1)).Value, "1 * 1");
            Assert.AreEqual(0, (Minutes.Multiply(Minutes.One, 0)).Value, "1 * 0");
            Assert.AreEqual(-9, (Minutes.Multiply(Minutes.From(9), -1)).Value, "9 * (-1)");
        }
        #endregion

        #region Division
        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Minutes.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Minutes.Two.Divide(1).Value, "2 / 1");
            Assert.AreEqual(1, Minutes.From(4).Divide(3).Value, "4 / 3");

            var ten = Minutes.From(10);
            Assert.AreSame(ten, ten.Divide(1), "10 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Minutes.One / 1).Value, "1 / 1");
            Assert.AreEqual(2, (Minutes.From(4) / 2).Value, "4 / 2");
            Assert.AreEqual(-3, (Minutes.Three / -1).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Minutes.Divide(Minutes.One, 1)).Value, "1 / 1");
            Assert.AreEqual(1, (Minutes.Divide(Minutes.Three, 2)).Value, "3 / 2");
            Assert.AreEqual(-3, (Minutes.Divide(Minutes.From(9), -3)).Value, "9 / (-3)");
        }
        #endregion

        #region Conversions
        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Minutes two = Minutes.Two;
            int iwo = two;

            Assert.AreEqual(2, iwo);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Minutes @null = null;
            int zero = @null;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Minutes three = (Minutes)3;

            Assert.AreEqual(3, three.Value);
        }
        #endregion
    }
}