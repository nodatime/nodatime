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
    public partial class HoursTest
    {
        #region Negation

        [Test]
        public void Negated()
        {
            Assert.AreEqual(-2, Hours.Two.Negated().Value, "- 2");
            Assert.AreEqual(9, Hours.From(-9).Negated().Value, "- -9");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(0, (-Hours.Zero).Value, "-0");
            Assert.AreEqual(-1, (-Hours.One).Value, "-1");
            Assert.AreEqual(7, (-Hours.From(-7)).Value, "- (-7)");
        }

        [Test]
        public void NegateStatic()
        {
            Assert.AreEqual(0, (Hours.Negate(Hours.Zero)).Value, "-0");
            Assert.AreEqual(-3, (Hours.Negate(Hours.Three)).Value, "-3");
            Assert.AreEqual(8, (Hours.Negate(Hours.From(-8))).Value, "- (-8)");
        }

        #endregion

        #region Unary Operators

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(0, (+Hours.Zero).Value, "+0");
            Assert.AreEqual(1, (+Hours.One).Value, "+1");
            Assert.AreEqual(-7, (+Hours.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryPlusAlternativeMethod()
        {
            Assert.AreEqual(0, Hours.Plus(Hours.Zero).Value, "+0");
            Assert.AreEqual(1, Hours.Plus(Hours.One).Value, "+1");
            Assert.AreEqual(-7, Hours.Plus(Hours.From(-7)).Value, "+ (-7)");
        }

        [Test]
        public void UnaryIncrementOperator()
        {
            var twoHours = Hours.Two;
            ++twoHours;

            Assert.AreEqual(3, (twoHours).Value, "++2 = 3");
        }

        [Test]
        public void UnaryIncrementAlternativeMethod()
        {
            var twoHours = Hours.Two;
            var threeHours = Hours.Increment(twoHours);

            Assert.AreEqual(3, (threeHours).Value, "++2 = 3");
        }

        [Test]
        public void UnaryDecrementOperator()
        {
            var twoHours = Hours.Two;
            --twoHours;

            Assert.AreEqual(1, (twoHours).Value, "--2 = 1");
        }

        [Test]
        public void UnaryDecrementAlternativeMethod()
        {
            var twoHours = Hours.Two;
            var oneHour = Hours.Decrement(twoHours);

            Assert.AreEqual(1, (oneHour).Value, "--2 = 1");
        }

        #endregion

        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Hours.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Hours.One.Add(0).Value, "1 + 0");

            var tenWeeks = Hours.From(10);
            Assert.AreSame(tenWeeks, tenWeeks.Add(0), "10 + 0");
        }

        [Test]
        public void AddOperator()
        {
            Assert.AreEqual(0, (Hours.Zero + Hours.Zero).Value, "0 + 0");
            Assert.AreEqual(1, (Hours.One + Hours.Zero).Value, "1 + 0");
            Assert.AreEqual(1, (Hours.Zero + Hours.One).Value, "0 + 1");
            Assert.AreEqual(5, (Hours.Two + Hours.Three).Value, "2 + 3");
            Assert.AreEqual(1, (Hours.From(9) + Hours.From(-8)).Value, "9 + (-8)");
        }

        [Test]
        public void AddStatic()
        {
            Assert.AreEqual(0, (Hours.Add(Hours.Zero, Hours.Zero)).Value, "0 + 0");
            Assert.AreEqual(1, (Hours.Add(Hours.One, Hours.Zero)).Value, "1 + 0");
            Assert.AreEqual(1, (Hours.Add(Hours.Zero, Hours.One)).Value, "0 + 1");
            Assert.AreEqual(5, (Hours.Add(Hours.Two, Hours.Three)).Value, "2 + 3");
            Assert.AreEqual(1, (Hours.Add(Hours.From(9), Hours.From(-8))).Value, "9 + (-8)");
        }

        #endregion

        #region Subtract

        [Test]
        public void Subtract()
        {
            Assert.AreEqual(1, Hours.Three.Subtract(2).Value, "3 - 2");
            Assert.AreEqual(1, Hours.One.Subtract(0).Value, "1 - 0");

            var tenHours = Hours.From(10);
            Assert.AreSame(tenHours, tenHours.Subtract(0), "10 - 0");
        }

        [Test]
        public void SubtractOperator()
        {
            Assert.AreEqual(0, (Hours.Zero - Hours.Zero).Value, "0 - 0");
            Assert.AreEqual(1, (Hours.One - Hours.Zero).Value, "1 - 0");
            Assert.AreEqual(-1, (Hours.Zero - Hours.One).Value, "0 - 1");
            Assert.AreEqual(1, (Hours.Three - Hours.Two).Value, "3 - 2");
            Assert.AreEqual(10, (Hours.From(9) - Hours.From(-1)).Value, "9 - (-1)");
        }

        [Test]
        public void SubtractStatic()
        {
            Assert.AreEqual(0, (Hours.Subtract(Hours.Zero, Hours.Zero)).Value, "0 - 0");
            Assert.AreEqual(1, (Hours.Subtract(Hours.One, Hours.Zero)).Value, "1 - 0");
            Assert.AreEqual(-1, (Hours.Subtract(Hours.Zero, Hours.One)).Value, "0 - 1");
            Assert.AreEqual(1, (Hours.Subtract(Hours.Three, Hours.Two)).Value, "3 - 2");
            Assert.AreEqual(10, (Hours.Subtract(Hours.From(9), Hours.From(-1))).Value, "9 - (-1)");
        }

        #endregion

        #region Multiplication

        [Test]
        public void Multiply()
        {
            Assert.AreEqual(6, Hours.Three.Multiply(2).Value, "3 * 2");
            Assert.AreEqual(2, Hours.Two.Multiply(1).Value, "2 * 1");
            Assert.AreSame(Hours.Zero, Hours.Three.Multiply(0), "3 * 0");

            var tenHours = Hours.From(10);
            Assert.AreSame(tenHours, tenHours.Multiply(1), "10 * 1");
        }

        [Test]
        public void MultiplyOperatorLeft()
        {
            Assert.AreEqual(1, (Hours.One * 1).Value, "1 * 1");
            Assert.AreEqual(0, (Hours.Two * 0).Value, "2 * 0");
            Assert.AreEqual(-3, (Hours.Three * -1).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyOperatorRight()
        {
            Assert.AreEqual(1, (1 * Hours.One).Value, "1 * 1");
            Assert.AreEqual(0, (0 * Hours.Two).Value, "0 * 2");
            Assert.AreEqual(-3, (-1 * Hours.Three).Value, "(-1) * 3");
        }

        [Test]
        public void MultiplyStaticLeft()
        {
            Assert.AreEqual(1, (Hours.Multiply(Hours.One, 1)).Value, "1 * 1");
            Assert.AreEqual(0, (Hours.Multiply(Hours.One, 0)).Value, "1 * 0");
            Assert.AreEqual(-9, (Hours.Multiply(Hours.From(9), -1)).Value, "9 * (-1)");
        }

        [Test]
        public void MultiplyStaticRight()
        {
            Assert.AreEqual(1, (Hours.Multiply(1, Hours.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Hours.Multiply(0, Hours.One)).Value, "0 * 1");
            Assert.AreEqual(-9, (Hours.Multiply(-1, Hours.From(9))).Value, "(-1) * 9");
        }
        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Hours.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Hours.Two.Divide(1).Value, "2 / 1");
            Assert.AreEqual(1, Hours.From(4).Divide(3).Value, "4 / 3");

            var tenHours = Hours.From(10);
            Assert.AreSame(tenHours, tenHours.Divide(1), "10 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Hours.One / Hours.One).Value, "1 / 1");
            Assert.AreEqual(2, (Hours.From(4) / Hours.Two).Value, "4 / 2");
            Assert.AreEqual(-3, (Hours.Three / Hours.From(-1)).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Hours.Divide(Hours.One, Hours.One)).Value, "1 / 1");
            Assert.AreEqual(1, (Hours.Divide(Hours.Three, Hours.Two)).Value, "3 / 2");
            Assert.AreEqual(-3, (Hours.Divide(Hours.From(9), Hours.From(-3))).Value, "9 / (-3)");
        }

        #endregion

        #region Conversions

        [Test]
        public void ImplicitConversionToInt32_FromNotNullInstance()
        {
            Hours twoHours = Hours.Two;
            int two = twoHours;

            Assert.AreEqual(2, two);
        }

        [Test]
        public void ImplicitConversionToInt32_FromNullInstance()
        {
            Hours nullHours = null;
            int zero = nullHours;

            Assert.AreEqual(0, zero);
        }

        [Test]
        public void ExplicitConversionToYears_FromInt32()
        {
            Hours threeHours = (Hours)3;

            Assert.AreEqual(3, threeHours.Value);
        }

        #endregion
    }
}
