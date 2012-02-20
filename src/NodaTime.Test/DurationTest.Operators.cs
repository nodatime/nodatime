#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsStruct(threeMillion, new Duration(3000000), negativeFiftyMillion);
            TestHelper.TestOperatorEquality(threeMillion, new Duration(3000000), negativeFiftyMillion);
        }

        [Test]
        public void Comparison()
        {
            TestHelper.TestCompareToStruct(negativeFiftyMillion, new Duration(-50000000), threeMillion);
            TestHelper.TestNonGenericCompareTo(negativeFiftyMillion, new Duration(-50000000), threeMillion);
            TestHelper.TestOperatorComparisonEquality(negativeFiftyMillion, new Duration(-50000000), threeMillion);
        }

        #region operator +
        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Duration.Zero + Duration.Zero).TotalTicks, "0 + 0");
            Assert.AreEqual(1L, (Duration.One + Duration.Zero).TotalTicks, "1 + 0");
            Assert.AreEqual(1L, (Duration.Zero + Duration.One).TotalTicks, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(3000001L, (threeMillion + Duration.One).TotalTicks, "3,000,000 + 1");
            Assert.AreEqual(0L, (Duration.One + Duration.NegativeOne).TotalTicks, "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration.One).TotalTicks, "-50,000,000 + 1");
        }

        [Test]
        public void OperatorPlus_MethodEquivalents()
        {
            Duration x = new Duration(100);
            Duration y = new Duration(200);
            Assert.AreEqual(x + y, Duration.Add(x, y));
            Assert.AreEqual(x + y, x.Plus(y));
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Duration.Zero - Duration.Zero).TotalTicks, "0 - 0");
            Assert.AreEqual(1L, (Duration.One - Duration.Zero).TotalTicks, "1 - 0");
            Assert.AreEqual(-1L, (Duration.Zero - Duration.One).TotalTicks, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(2999999L, (threeMillion - Duration.One).TotalTicks, "3,000,000 - 1");
            Assert.AreEqual(2L, (Duration.One - Duration.NegativeOne).TotalTicks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration.One).TotalTicks, "-50,000,000 - 1");
        }

        [Test]
        public void OperatorMinus_MethodEquivalents()
        {
            Duration x = new Duration(100);
            Duration y = new Duration(200);
            Assert.AreEqual(x - y, Duration.Subtract(x, y));
            Assert.AreEqual(x - y, x.Minus(y));
        }
        #endregion

        #region operator /
        [Test]
        public void OperatorDivision_ByNonZero()
        {
            Assert.AreEqual(1000, (threeMillion / 3000).TotalTicks, "3000000 / 3000");
        }

        [Test]
        public void OperatorDivision_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => (threeMillion / 0).ToString(), "3000000 / 0");
        }

        [Test]
        public void OperatorDivision_Truncates()
        {
            Assert.AreEqual(1, (threeMillion / 2000000).TotalTicks, "3000000 / 2000000");
            Assert.AreEqual(-1, (threeMillion / -2000000).TotalTicks, "3000000 / -2000000");
        }

        [Test]
        public void OperatorDivision_MethodEquivalent()
        {
            Assert.AreEqual(threeMillion / 2000000, Duration.Divide(threeMillion, 2000000));
        }
        #endregion

        #region operator *
        [Test]
        public void OperatorMultiplication_NonZeroNonOne()
        {
            Assert.AreEqual(threeMillion, new Duration(3000) * 1000, "3000 * 1000");
            Assert.AreEqual(negativeFiftyMillion, new Duration(50000) * -1000, "50000 * -1000");
            Assert.AreEqual(negativeFiftyMillion, new Duration(-50000) * 1000, "-50000 * 1000");
            Assert.AreEqual(threeMillion, new Duration(-3000) * -1000, "-3000 * -1000");
        }

        [Test]
        public void OperatorMultiplication_Zero_IsAbsorbingElement()
        {
            Assert.AreEqual(Duration.Zero, Duration.Zero * 0, "0 * 0");
            Assert.AreEqual(Duration.Zero, Duration.One * 0, "1 * 0");
            Assert.AreEqual(Duration.Zero, threeMillion * 0, "3000000 * 0");
            Assert.AreEqual(Duration.Zero, negativeFiftyMillion * 0, "-50000000 * 0");
        }

        [Test]
        public void OperatorMultiplication_One_IsNeutralElement()
        {
            Assert.AreEqual(threeMillion, threeMillion * 1, "3000000 * 1");
            Assert.AreEqual(Duration.Zero, Duration.Zero * 1, "0 * 1");
            Assert.AreEqual(negativeFiftyMillion, negativeFiftyMillion * 1, "-50000000 * 1");
        }

        [Test]
        public void Commutation()
        {
            Assert.AreEqual(threeMillion * 5, 5 * threeMillion);
        }

        [Test]
        public void OperatorMultiplication_MethodEquivalents()
        {
            Assert.AreEqual(new Duration(-50000) * 1000, Duration.Multiply(new Duration(-50000), 1000));
            Assert.AreEqual(1000 * new Duration(-50000), Duration.Multiply(1000, new Duration(-50000)));
        }
        #endregion

        #region Unary operator -
        public static void UnaryMinus()
        {
            Assert.AreEqual(Duration.FromTicks(-5000), -Duration.FromTicks(5000));
        }

        public static void Negate()
        {
            Assert.AreEqual(Duration.FromTicks(-5000), Duration.Negate(Duration.FromTicks(5000)));
        }
        #endregion
    }
}