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
    public partial class DaysTest
    {
        #region Negation

        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Days.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Days.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Days.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Days.One).Value, "-1");
            Assert.AreEqual(7, (-Days.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Days.Negate(Days.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Days.Negate(Days.Three)).Value, "-3");
            Assert.AreEqual(8, (Days.Negate(Days.From(-8))).Value, "- (-8)");
        }

        #endregion

        #region Unary Operators

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Days.Zero).Value, "+0");
            Assert.AreEqual(1, (+Days.One).Value, "+1");
            Assert.AreEqual(-7, (+Days.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryPlusAlternativeMethod()
        {
            Assert.AreEqual(0, Days.Plus(Days.Zero).Value, "+0");
            Assert.AreEqual(1, Days.Plus(Days.One).Value, "+1");
            Assert.AreEqual(-7, Days.Plus(Days.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoDays = Days.Two;
            ++twoDays;

            Assert.AreEqual(3, (twoDays).Value, "++2 = 3");
        }

        [Test]
        public void UnaryIncrementAlternativeMethod()
        {
            var twoDays = Days.Two;
            var threeDays = Days.Increment(twoDays);

            Assert.AreEqual(3, (threeDays).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoDays = Days.Two;
            --twoDays;

            Assert.AreEqual(1, (twoDays).Value, "--2 = 1");
        }

        [Test]
        public void UnaryDecrementAlternativeMehod()
        {
            var twoDays = Days.Two;
            var oneDay = Days.Decrement(twoDays);

            Assert.AreEqual(1, (oneDay).Value, "--2 = 1");
        }

        #endregion

        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Days.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Days.One.Add(0).Value, "1 + 0");

            var sevenWeeks = Days.From(7);
            Assert.AreSame(sevenWeeks, sevenWeeks.Add(0), "7 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Days.Zero + Days.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Days.One + Days.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Days.Zero + Days.One).Value, "0 + 1");
            Assert.AreEqual(5, (Days.Two + Days.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Days.From(9) + Days.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Days.Add(Days.Zero, Days.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Days.Add(Days.One, Days.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Days.Add(Days.Zero, Days.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Days.Add(Days.Two, Days.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Days.Add(Days.From(9), Days.From(-8))).Value, "9 + (-8)");
        }

        #endregion

        #region Subtract

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Days.Three.Subtract(2).Value, "3 - 2");
            Assert.AreEqual(1, Days.One.Subtract(0).Value, "1 - 0");

            var eightWeeks = Days.From(8);
            Assert.AreSame(eightWeeks, eightWeeks.Subtract(0), "8 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Days.Zero - Days.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Days.One - Days.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Days.Zero - Days.One).Value, "0 - 1");
            Assert.AreEqual(1, (Days.Three - Days.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Days.From(9) - Days.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Days.Subtract(Days.Zero, Days.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Days.Subtract(Days.One, Days.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Days.Subtract(Days.Zero, Days.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Days.Subtract(Days.Three, Days.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Days.Subtract(Days.From(9), Days.From(-1))).Value, "9 - (-1)");
        }

        #endregion

        #region Multiplication

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Days.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Days.Two.Multiply(1).Value, "2 * 1");
            Assert.AreSame(Days.Zero, Days.Three.Multiply(0), "3 * 0");

            var fiveWeeks = Days.From(5);
            Assert.AreSame(fiveWeeks, fiveWeeks.Multiply(1), "5 * 1");
        }

        [Test]
        public void MultiplyOperatorLeft()
        {
            Assert.AreEqual(1, (Days.One * 1).Value, "1 * 1");
            Assert.AreEqual(0, (Days.Two * 0).Value, "2 * 0");
            Assert.AreEqual(-3, (Days.Three * -1).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyOperatorRight()
        {
            Assert.AreEqual(1, (1 * Days.One).Value, "1 * 1");
            Assert.AreEqual(0, (0 * Days.Two).Value, "0 * 2");
            Assert.AreEqual(-3, (-1 * Days.Three).Value, "(-1) * 3");
        }

        [Test]
        public void MultiplyStaticLeft()
        {
            Assert.AreEqual(1, (Days.Multiply(Days.One, 1)).Value, "1 * 1");
            Assert.AreEqual(0, (Days.Multiply(Days.One, 0)).Value, "1 * 0");
            Assert.AreEqual(-9, (Days.Multiply(Days.From(9), -1)).Value, "9 * (-1)");
        }

        [Test]
        public void MultiplyStaticRight()
        {
            Assert.AreEqual(1, (Days.Multiply(1, Days.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Days.Multiply(0, Days.One)).Value, "0 * 1");
            Assert.AreEqual(-9, (Days.Multiply(-1, Days.From(9))).Value, "(-1) * 9");
        }

        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Days.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Days.Two.Divide(1).Value, "2 / 1");
            Assert.AreEqual(1, Days.From(4).Divide(3).Value, "4 / 3");

            var sixWeeks = Days.From(6);
            Assert.AreSame(sixWeeks, sixWeeks.Divide(1), "6 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Days.One / Days.One).Value, "1 / 1");
            Assert.AreEqual(2, (Days.From(4) / Days.Two).Value, "4 / 2");
            Assert.AreEqual(-3, (Days.Three / Days.From(-1)).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Days.Divide(Days.One, Days.One)).Value, "1 / 1");
            Assert.AreEqual(1, (Days.Divide(Days.Three, Days.Two)).Value, "3 / 2");
            Assert.AreEqual(-3, (Days.Divide(Days.From(9), Days.From(-3))).Value, "9 / (-3)");
        }

        #endregion

        #region Conversions

        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Days twoDays = Days.Two;
            int two = twoDays;

            Assert.AreEqual(2, two);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Days nullDays = null;
            int zero = nullDays;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Days threeDays = (Days)3;

            Assert.AreEqual(3, threeDays.Value);
        }

        #endregion
    }
}
