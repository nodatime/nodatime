// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        private readonly Duration threeMillion = Duration.FromNanoseconds(3000000L);
        private readonly Duration negativeFiftyMillion = Duration.FromNanoseconds(-50000000L);

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsStruct(threeMillion, Duration.FromNanoseconds(3000000), negativeFiftyMillion);
            TestHelper.TestOperatorEquality(threeMillion, Duration.FromNanoseconds(3000000), negativeFiftyMillion);
        }

        [Test]
        public void Comparison()
        {
            TestHelper.TestCompareToStruct(negativeFiftyMillion, Duration.FromNanoseconds(-50000000), threeMillion);
            TestHelper.TestNonGenericCompareTo(negativeFiftyMillion, Duration.FromNanoseconds(-50000000), threeMillion);
            TestHelper.TestOperatorComparisonEquality(negativeFiftyMillion, Duration.FromNanoseconds(-50000000), threeMillion);
        }

        #region operator +
        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (long) (Duration.Zero + Duration.Zero).Nanoseconds, "0 + 0");
            Assert.AreEqual(1L, (long) (Duration.Epsilon + Duration.Zero).Nanoseconds, "1 + 0");
            Assert.AreEqual(1L, (long) (Duration.Zero + Duration.Epsilon).Nanoseconds, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(3000001L, (long) (threeMillion + Duration.Epsilon).Nanoseconds, "3,000,000 + 1");
            Assert.AreEqual(0L, (long) (Duration.Epsilon + Duration.FromNanoseconds(-1)).Nanoseconds, "1 + (-1)");
            Assert.AreEqual(-49999999L, (long) (negativeFiftyMillion + Duration.Epsilon).Nanoseconds, "-50,000,000 + 1");
        }

        [Test]
        public void OperatorPlus_MethodEquivalents()
        {
            Duration x = Duration.FromNanoseconds(100);
            Duration y = Duration.FromNanoseconds(200);
            Assert.AreEqual(x + y, Duration.Add(x, y));
            Assert.AreEqual(x + y, x.Plus(y));
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (long) (Duration.Zero - Duration.Zero).Nanoseconds, "0 - 0");
            Assert.AreEqual(1L, (long) (Duration.Epsilon - Duration.Zero).Nanoseconds, "1 - 0");
            Assert.AreEqual(-1L, (long) (Duration.Zero - Duration.Epsilon).Nanoseconds, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Duration negativeEpsilon = Duration.FromNanoseconds(-1L);
            Assert.AreEqual(2999999L, (long) (threeMillion - Duration.Epsilon).Nanoseconds, "3,000,000 - 1");
            Assert.AreEqual(2L, (long) (Duration.Epsilon - negativeEpsilon).Nanoseconds, "1 - (-1)");
            Assert.AreEqual(-50000001L, (long) (negativeFiftyMillion - Duration.Epsilon).Nanoseconds, "-50,000,000 - 1");
        }

        [Test]
        public void OperatorMinus_MethodEquivalents()
        {
            Duration x = Duration.FromNanoseconds(100);
            Duration y = Duration.FromNanoseconds(200);
            Assert.AreEqual(x - y, Duration.Subtract(x, y));
            Assert.AreEqual(x - y, x.Minus(y));
        }
        #endregion

        #region operator /
        [Test]
        public void OperatorDivision_ByNonZero()
        {
            Assert.AreEqual(1000, (long) (threeMillion / 3000).Nanoseconds, "3000000 / 3000");
        }

        [Test]
        public void OperatorDivision_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => (threeMillion / 0).ToString(), "3000000 / 0");
        }

        [Test]
        public void OperatorDivision_Truncates()
        {
            Assert.AreEqual(1, (long) (threeMillion / 2000000).Nanoseconds, "3000000 / 2000000");
            Assert.AreEqual(-1, (long) (threeMillion / -2000000).Nanoseconds, "3000000 / -2000000");
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
            Assert.AreEqual(threeMillion, Duration.FromNanoseconds(3000) * 1000, "3000 * 1000");
            Assert.AreEqual(negativeFiftyMillion, Duration.FromNanoseconds(50000) * -1000, "50000 * -1000");
            Assert.AreEqual(negativeFiftyMillion, Duration.FromNanoseconds(-50000) * 1000, "-50000 * 1000");
            Assert.AreEqual(threeMillion, Duration.FromNanoseconds(-3000) * -1000, "-3000 * -1000");
        }

        [Test]
        public void OperatorMultiplication_Zero_IsAbsorbingElement()
        {
            Assert.AreEqual(Duration.Zero, Duration.Zero * 0, "0 * 0");
            Assert.AreEqual(Duration.Zero, Duration.Epsilon * 0, "1 * 0");
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
            Assert.AreEqual(Duration.FromNanoseconds(-50000) * 1000, Duration.Multiply(Duration.FromNanoseconds(-50000), 1000));
            Assert.AreEqual(1000 * Duration.FromNanoseconds(-50000), Duration.Multiply(1000, Duration.FromNanoseconds(-50000)));
        }
        #endregion

        #region Unary operator -
        public static void UnaryMinus()
        {
            Assert.AreEqual(Duration.FromNanoseconds(-5000), -Duration.FromNanoseconds(5000));
        }

        public static void Negate()
        {
            Assert.AreEqual(Duration.FromNanoseconds(-5000), Duration.Negate(Duration.FromNanoseconds(5000)));
        }
        #endregion
    }
}
