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
    public partial class MonthsTest
    {

        #region Negation

        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Months.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Months.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Months.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Months.One).Value, "-1");
            Assert.AreEqual(7, (-Months.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Months.Negate(Months.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Months.Negate(Months.Three)).Value, "-3");
            Assert.AreEqual(8, (Months.Negate(Months.From(-8))).Value, "- (-8)");
        }

        #endregion

        #region Unary Operators

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Months.Zero).Value, "+0");
            Assert.AreEqual(1, (+Months.One).Value, "+1");
            Assert.AreEqual(7, (+Months.From(+7)).Value, "+ (+7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoMonths = Months.Two;
            ++twoMonths;

            Assert.AreEqual(3, (twoMonths).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoMonths = Months.Two;
            --twoMonths;

            Assert.AreEqual(1, (twoMonths).Value, "--2 = 1");
        }

        #endregion

        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Months.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Months.One.Add(0).Value, "1 + 0");
            
            var twentyMonths = Months.From(20);
            Assert.AreSame(twentyMonths, twentyMonths.Add(0), "20 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Months.Zero + Months.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Months.One + Months.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Months.Zero + Months.One).Value, "0 + 1");
            Assert.AreEqual(5, (Months.Two + Months.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Months.From(9) + Months.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Months.Add(Months.Zero, Months.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Months.Add(Months.One, Months.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Months.Add(Months.Zero, Months.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Months.Add(Months.Two, Months.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Months.Add(Months.From(9), Months.From(-8))).Value, "9 + (-8)");
        }

        #endregion

        #region Subtract

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Months.Three.Subtract(2).Value);
            Assert.AreEqual(1, Months.One.Subtract(0).Value);
            
            var tenMonths = Months.From(10);
            Assert.AreSame(tenMonths, tenMonths.Subtract(0), "10 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Months.Zero - Months.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Months.One - Months.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Months.Zero - Months.One).Value, "0 - 1");
            Assert.AreEqual(1, (Months.Three - Months.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Months.From(9) - Months.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Months.Subtract(Months.Zero, Months.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Months.Subtract(Months.One, Months.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Months.Subtract(Months.Zero, Months.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Months.Subtract(Months.Three, Months.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Months.Subtract(Months.From(9), Months.From(-1))).Value, "9 - (-1)");
        }

        #endregion

        #region Multiplication

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Months.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Months.Two.Multiply(1).Value, "2 * 1");
            
            var twentyFiveMonths = Months.From(25);
            Assert.AreSame(twentyFiveMonths, twentyFiveMonths.Multiply(1), "25 * 1");
        }

        [Test]
        public void MultiplyOperator()
        {
            Assert.AreEqual(1, (Months.One * Months.One).Value, "1 * 1");
            Assert.AreEqual(0, (Months.Two * Months.Zero).Value, "2 * 0");
            Assert.AreEqual(-3, (Months.Three * Months.From(-1)).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyStatic()
        {
            Assert.AreEqual(1, (Months.Multiply(Months.One, Months.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Months.Multiply(Months.One, Months.Zero)).Value, "1 * 0");
            Assert.AreEqual(-9, (Months.Multiply(Months.From(9), Months.From(-1))).Value, "9 * (-1)");
        }

        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Months.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Months.Two.Divide(1).Value, "2 / 1");
            
            var twentySixMonths = Months.From(26);
            Assert.AreSame(twentySixMonths, twentySixMonths.Divide(1), "26 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Months.One / Months.One).Value, "1 / 1");
            Assert.AreEqual(2, (Months.From(4) / Months.Two).Value, "4 / 2");
            Assert.AreEqual(-3, (Months.Three / Months.From(-1)).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Months.Divide(Months.One, Months.One)).Value, "1 / 1");
            Assert.AreEqual(1, (Months.Divide(Months.Three, Months.Two)).Value, "3 / 2");
            Assert.AreEqual(-3, (Months.Divide(Months.From(9), Months.From(-3))).Value, "9 / (-3)");
        }

        #endregion

        #region Conversions

        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Months twoMonths = Months.Two;
            int two = twoMonths;

            Assert.AreEqual(2, two);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Months nullMonths = null;
            int zero = nullMonths;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Months threeMonths = (Months)3;

            Assert.AreEqual(3, threeMonths.Value);
        }

        #endregion
    }
}
