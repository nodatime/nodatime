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
    public partial class SecondsTest
    {
        #region Negation

        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Seconds.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Seconds.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Seconds.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Seconds.One).Value, "-1");
            Assert.AreEqual(7, (-Seconds.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Seconds.Negate(Seconds.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Seconds.Negate(Seconds.Three)).Value, "-3");
            Assert.AreEqual(8, (Seconds.Negate(Seconds.From(-8))).Value, "- (-8)");
        }

        #endregion

        #region Unary Operators

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Seconds.Zero).Value, "+0");
            Assert.AreEqual(1, (+Seconds.One).Value, "+1");
            Assert.AreEqual(7, (+Seconds.From(+7)).Value, "+ (+7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var two = Seconds.Two;
            ++two;

            Assert.AreEqual(3, (two).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var two = Seconds.Two;
            --two;

            Assert.AreEqual(1, (two).Value, "--2 = 1");
        }

        #endregion

        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Seconds.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Seconds.One.Add(0).Value, "1 + 0");

            var ten = Seconds.From(10);
            Assert.AreSame(ten, ten.Add(0), "10 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Seconds.Zero + Seconds.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Seconds.One + Seconds.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Seconds.Zero + Seconds.One).Value, "0 + 1");
            Assert.AreEqual(5, (Seconds.Two + Seconds.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Seconds.From(9) + Seconds.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Seconds.Add(Seconds.Zero, Seconds.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Seconds.Add(Seconds.One, Seconds.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Seconds.Add(Seconds.Zero, Seconds.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Seconds.Add(Seconds.Two, Seconds.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Seconds.Add(Seconds.From(9), Seconds.From(-8))).Value, "9 + (-8)");
        }

        #endregion

        #region Subtract

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Seconds.Three.Subtract(2).Value, "3 - 2");
            Assert.AreEqual(1, Seconds.One.Subtract(0).Value, "1 - 0");

            var ten = Seconds.From(10);
            Assert.AreSame(ten, ten.Subtract(0), "10 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Seconds.Zero - Seconds.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Seconds.One - Seconds.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Seconds.Zero - Seconds.One).Value, "0 - 1");
            Assert.AreEqual(1, (Seconds.Three - Seconds.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Seconds.From(9) - Seconds.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Seconds.Subtract(Seconds.Zero, Seconds.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Seconds.Subtract(Seconds.One, Seconds.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Seconds.Subtract(Seconds.Zero, Seconds.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Seconds.Subtract(Seconds.Three, Seconds.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Seconds.Subtract(Seconds.From(9), Seconds.From(-1))).Value, "9 - (-1)");
        }

        #endregion

        #region Multiplication

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Seconds.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Seconds.Two.Multiply(1).Value, "2 * 1");
            Assert.AreSame(Seconds.Zero, Seconds.Three.Multiply(0), "3 * 0");

            var ten = Seconds.From(10);
            Assert.AreSame(ten, ten.Multiply(1), "10 * 1");
        }

        [Test]
        public void MultiplyOperator()
        {
            Assert.AreEqual(1, (Seconds.One * Seconds.One).Value, "1 * 1");
            Assert.AreEqual(0, (Seconds.Two * Seconds.Zero).Value, "2 * 0");
            Assert.AreEqual(-3, (Seconds.Three * Seconds.From(-1)).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyStatic()
        {
            Assert.AreEqual(1, (Seconds.Multiply(Seconds.One, Seconds.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Seconds.Multiply(Seconds.One, Seconds.Zero)).Value, "1 * 0");
            Assert.AreEqual(-9, (Seconds.Multiply(Seconds.From(9), Seconds.From(-1))).Value, "9 * (-1)");
        }

        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Seconds.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Seconds.Two.Divide(1).Value, "2 / 1");
            Assert.AreEqual(1, Seconds.From(4).Divide(3).Value, "4 / 3");

            var ten = Seconds.From(10);
            Assert.AreSame(ten, ten.Divide(1), "10 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Seconds.One / Seconds.One).Value, "1 / 1");
            Assert.AreEqual(2, (Seconds.From(4) / Seconds.Two).Value, "4 / 2");
            Assert.AreEqual(-3, (Seconds.Three / Seconds.From(-1)).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Seconds.Divide(Seconds.One, Seconds.One)).Value, "1 / 1");
            Assert.AreEqual(1, (Seconds.Divide(Seconds.Three, Seconds.Two)).Value, "3 / 2");
            Assert.AreEqual(-3, (Seconds.Divide(Seconds.From(9), Seconds.From(-3))).Value, "9 / (-3)");
        }

        #endregion

        #region Conversions

        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Seconds two = Seconds.Two;
            int iwo = two;

            Assert.AreEqual(2, iwo);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Seconds @null = null;
            int zero = @null;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Seconds three = (Seconds)3;

            Assert.AreEqual(3, three.Value);
        }

        #endregion
    }
}
