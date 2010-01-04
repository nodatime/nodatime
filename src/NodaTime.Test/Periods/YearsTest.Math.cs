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
        #region Add

        [Test]
        public void Add()
        {
            Assert.AreEqual(5, Years.Two.Add(3).Value, "2 + 3");
            Assert.AreEqual(1, Years.One.Add(0).Value, "1 + 0");
            Assert.AreSame(Years.Three, Years.Three.Add(0));
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
            Assert.AreEqual(0, (Years.Add(Years.Zero,Years.Zero)).Value, "0 + 0");
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
            Assert.AreSame(Years.Three, Years.Three.Subtract(0));
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
            Assert.AreSame(Years.Zero, Years.Three.Multiply(0), "3 * 0");
        }

        [Test]
        public void MultiplyOperator()
        {
            Assert.AreEqual(1, (Years.One * Years.One).Value, "1 * 1");
            Assert.AreEqual(0, (Years.Two * Years.Zero).Value, "2 * 0");
            Assert.AreEqual(-3, (Years.Three * Years.From(-1)).Value, "3 * (-1)");
        }

        [Test]
        public void MultiplyStatic()
        {
            Assert.AreEqual(1, (Years.Multiply(Years.One, Years.One)).Value, "1 * 1");
            Assert.AreEqual(0, (Years.Multiply(Years.One, Years.Zero)).Value, "1 * 0");
            Assert.AreEqual(-9, (Years.Multiply(Years.From(9), Years.From(-1))).Value, "9 * (-1)");
        }

        #endregion

        #region Division

        [Test]
        public void Divide()
        {
            Assert.AreEqual(3, Years.From(6).Divide(2).Value, "6 / 2");
            Assert.AreEqual(2, Years.Two.Divide(1).Value, "2 / 1");
            Assert.AreSame(Years.Three, Years.Three.Divide(1), "3 / 1");
        }

        [Test]
        public void DivideOperator()
        {
            Assert.AreEqual(1, (Years.One / Years.One).Value, "1 / 1");
            Assert.AreEqual(2, (Years.From(4) / Years.Two).Value, "4 / 2");
            Assert.AreEqual(-3, (Years.Three / Years.From(-1)).Value, "3 / (-1)");
        }

        [Test]
        public void DivideStatic()
        {
            Assert.AreEqual(1, (Years.Divide(Years.One, Years.One)).Value, "1 / 1");
            Assert.AreEqual(1, (Years.Divide(Years.Three, Years.Two)).Value, "3 / 2");
            Assert.AreEqual(-3, (Years.Divide(Years.From(9), Years.From(-3))).Value, "9 / (-3)");
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
