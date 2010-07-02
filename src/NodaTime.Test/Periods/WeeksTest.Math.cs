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
    public partial class WeeksTest
    {
        #region Negation

        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Weeks.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Weeks.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Weeks.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Weeks.One).Value, "-1");
            Assert.AreEqual(7, (-Weeks.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Weeks.Negate(Weeks.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Weeks.Negate(Weeks.Three)).Value, "-3");
            Assert.AreEqual(8, (Weeks.Negate(Weeks.From(-8))).Value, "- (-8)");
        }

        #endregion

        #region Unary Operators

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Weeks.Zero).Value, "+0");
            Assert.AreEqual(1, (+Weeks.One).Value, "+1");
            Assert.AreEqual(-7, (+Weeks.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryPlusAlternativeMethod()
        {
            Assert.AreEqual(0, Weeks.Plus(Weeks.Zero).Value, "+0");
            Assert.AreEqual(1, Weeks.Plus(Weeks.One).Value, "+1");
            Assert.AreEqual(-7, Weeks.Plus(Weeks.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoWeeks = Weeks.Two;
            ++twoWeeks;

            Assert.AreEqual(3, (twoWeeks).Value, "++2 = 3");
        }

        [Test]
        public void UnaryIncrementAlternativeMethod()
        {
            var twoWeeks = Weeks.Two;
            var threeWeeks = Weeks.Increment(twoWeeks);

            Assert.AreEqual(3, (threeWeeks).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoWeeks = Weeks.Two;
            --twoWeeks;

            Assert.AreEqual(1, (twoWeeks).Value, "--2 = 1");
        }

        [Test]
        public void UnaryDecrementAlternativeMethod()
        {
            var twoWeeks = Weeks.Two;
            var oneWeek = Weeks.Decrement(twoWeeks);

            Assert.AreEqual(1, (oneWeek).Value, "--2 = 1");
        }

        #endregion

        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Weeks.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Weeks.One.Add(0).Value, "1 + 0");

            var twentyWeeks = Weeks.From(20);
            Assert.AreSame(twentyWeeks, twentyWeeks.Add(0), "20 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Weeks.Zero + Weeks.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Weeks.One + Weeks.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Weeks.Zero + Weeks.One).Value, "0 + 1");
            Assert.AreEqual(5, (Weeks.Two + Weeks.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Weeks.From(9) + Weeks.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Weeks.Add(Weeks.Zero, Weeks.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Weeks.Add(Weeks.One, Weeks.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Weeks.Add(Weeks.Zero, Weeks.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Weeks.Add(Weeks.Two, Weeks.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Weeks.Add(Weeks.From(9), Weeks.From(-8))).Value, "9 + (-8)");
        }

        #endregion

        #region Subtract

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Weeks.Three.Subtract(2).Value, "3 - 2");
            Assert.AreEqual(1, Weeks.One.Subtract(0).Value, "1 - 0");

            var twentyOneWeeks = Weeks.From(21);
            Assert.AreSame(twentyOneWeeks, twentyOneWeeks.Subtract(0), "21 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Weeks.Zero - Weeks.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Weeks.One - Weeks.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Weeks.Zero - Weeks.One).Value, "0 - 1");
            Assert.AreEqual(1, (Weeks.Three - Weeks.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Weeks.From(9) - Weeks.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Weeks.Subtract(Weeks.Zero, Weeks.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Weeks.Subtract(Weeks.One, Weeks.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Weeks.Subtract(Weeks.Zero, Weeks.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Weeks.Subtract(Weeks.Three, Weeks.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Weeks.Subtract(Weeks.From(9), Weeks.From(-1))).Value, "9 - (-1)");
        }

        #endregion

        #region Multiplication

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Weeks.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Weeks.Two.Multiply(1).Value, "2 * 1");
            Assert.AreSame(Weeks.Zero, Weeks.Three.Multiply(0), "3 * 0");

            var twentyFiveWeeks = Weeks.From(25);
            Assert.AreSame(twentyFiveWeeks, twentyFiveWeeks.Multiply(1), "25 * 1");
        }

        [Test]
        public void MultiplyOperatorLeft()
        {
            Assert.AreEqual(1, (Weeks.One * 1).Value, "1 * 1");
            Assert.AreEqual(0, (Weeks.Two * 0).Value, "2 * 0");
            Assert.AreEqual(-3, (Weeks.Three * -1).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyOperatorRight()
        {
            Assert.AreEqual(1, (1 * Weeks.One).Value, "1 * 1");
            Assert.AreEqual(0, (0 * Weeks.Two).Value, "0 * 2");
            Assert.AreEqual(-3, (-1 * Weeks.Three).Value, "(-1) * 3");
        }

        [Test]
        public void MultiplyStaticLeft()
        {
            Assert.AreEqual(1, (Weeks.Multiply(Weeks.One, 1)).Value, "1 * 1");
            Assert.AreEqual(0, (Weeks.Multiply(Weeks.One, 0)).Value, "1 * 0");
            Assert.AreEqual(-9, (Weeks.Multiply(Weeks.From(9), -1)).Value, "9 * (-1)");
        }

        [Test]
        public void MultiplyStaticRight()
        {
            Assert.AreEqual(1, (Weeks.Multiply(1, Weeks.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Weeks.Multiply(0, Weeks.One)).Value, "0 * 1");
            Assert.AreEqual(-9, (Weeks.Multiply(-1, Weeks.From(9))).Value, "(-1) * 9");
        }

        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Weeks.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Weeks.Two.Divide(1).Value, "2 / 1");
            Assert.AreEqual(1, Weeks.From(4).Divide(3).Value, "4 / 3");

            var twentySixWeeks = Weeks.From(26);
            Assert.AreSame(twentySixWeeks, twentySixWeeks.Divide(1), "26 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Weeks.One / 1).Value, "1 / 1");
            Assert.AreEqual(2, (Weeks.From(4) / 2).Value, "4 / 2");
            Assert.AreEqual(-3, (Weeks.Three / -1).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Weeks.Divide(Weeks.One, 1)).Value, "1 / 1");
            Assert.AreEqual(1, (Weeks.Divide(Weeks.Three, 2)).Value, "3 / 2");
            Assert.AreEqual(-3, (Weeks.Divide(Weeks.From(9), -3)).Value, "9 / (-3)");
        }

        #endregion

        #region Conversions

        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Weeks twoWeeks = Weeks.Two;
            int two = twoWeeks;

            Assert.AreEqual(2, two);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Weeks nullWeeks = null;
            int zero = nullWeeks;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Weeks threeWeeks = (Weeks)3;

            Assert.AreEqual(3, threeWeeks.Value);
        }

        #endregion
    }
}
