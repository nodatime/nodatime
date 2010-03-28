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

using System;

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        #region IEquatable.Equals

        [Test]
        public void IEquatableEquals_ToSelf_IsTrue()
        {
            Assert.True(Duration.One.Equals(Duration.One), "1 == 1 (same object)");
        }

        [Test]
        public void IEquatableEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(Duration.One.Equals(onePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void IEquatableEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(Duration.One.Equals(Duration.NegativeOne), "1 == -1");
            Assert.False(Duration.One.Equals(threeMillion), "1 == 3,000,000");
            Assert.False(Duration.One.Equals(negativeFiftyMillion), "1 == -50,000,000");
            Assert.False(Duration.MinValue.Equals(Duration.MaxValue), "MinValue == MaxValue");
        }

        #endregion

        #region object.Equals

        [Test]
        public void ObjectEquals_ToNull_IsFalse()
        {
            object oOne = Duration.One;

            Assert.False(oOne.Equals(null), "1 == null");
        }

        [Test]
        public void ObjectEquals_ToSelf_IsTrue()
        {
            object oOne = Duration.One;

            Assert.True(oOne.Equals(oOne), "1 == 1 (same object)");
        }

        [Test]
        public void ObjectEquals_WithEqualTicks_IsTrue()
        {
            object oOne = Duration.One;
            object oOnePrime = onePrime;

            Assert.True(oOne.Equals(oOnePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void ObjectEquals_WithDifferentTicks_IsFalse()
        {
            object oOne = Duration.One;
            object oNegativeOne = Duration.NegativeOne;
            object oThreeMillion = threeMillion;
            object oNegativeFiftyMillion = negativeFiftyMillion;
            object oMinimum = Duration.MinValue;
            object oMaximum = Duration.MaxValue;

            Assert.False(oOne.Equals(oNegativeOne), "1 == -1");
            Assert.False(oOne.Equals(oThreeMillion), "1 == 3,000,000");
            Assert.False(oOne.Equals(oNegativeFiftyMillion), "1 == -50,000,000");
            Assert.False(oMinimum.Equals(oMaximum), "MinValue == MaxValue");
        }

        #endregion

        #region object.GetHashCode

        [Test]
        public void GetHashCode_Twice_IsEqual()
        {
            Duration test1 = new Duration(123L);
            Duration test2 = new Duration(123L);
            Assert.AreEqual(test1.GetHashCode(), test1.GetHashCode());
            Assert.AreEqual(test2.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_SameTicks_IsEqual()
        {
            Duration test1 = new Duration(123L);
            Duration test2 = new Duration(123L);
            Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentTicks_IsDifferent()
        {
            Duration test1 = new Duration(123L);
            Duration test2 = new Duration(123L);
            Duration test3 = new Duration(321L);

            Assert.AreNotEqual(test1.GetHashCode(), test3.GetHashCode());
            Assert.AreNotEqual(test2.GetHashCode(), test3.GetHashCode());
        }

        #endregion

        #region CompareTo

        [Test]
        public void CompareTo_Self_IsEqual()
        {
            Assert.AreEqual(0, Duration.One.CompareTo(Duration.One), "1 == 1 (same object)");
        }

        [Test]
        public void CompareTo_WithEqualTicks_IsEqual()
        {
            Assert.AreEqual(0, Duration.One.CompareTo(onePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void CompareTo_WithMoreTicks_IsGreater()
        {
            Assert.Greater(Duration.One.CompareTo(negativeFiftyMillion), 0, "1 > -50,000,000");
            Assert.Greater(threeMillion.CompareTo(Duration.One), 0, "3,000,000 > 1");
            Assert.Greater(Duration.NegativeOne.CompareTo(negativeFiftyMillion), 0, "-1 > -50,000,000");
            Assert.Greater(Duration.MaxValue.CompareTo(Duration.MinValue), 0, "MaxValue > MinValue");
        }

        [Test]
        public void CompareTo_WithLessTicks_IsLess()
        {
            Assert.Less(negativeFiftyMillion.CompareTo(Duration.One), 0, "-50,000,000 < 1");
            Assert.Less(Duration.One.CompareTo(threeMillion), 0, "1 < 3,000,000");
            Assert.Less(negativeFiftyMillion.CompareTo(Duration.NegativeOne), 0, "-50,000,000 > -1");
            Assert.Less(Duration.MinValue.CompareTo(Duration.MaxValue), 0, "MinValue < MaxValue");
        }

        #endregion

        #region operator ==

        [Test]
        public void OperatorEquals_ToSelf_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Duration.One == Duration.One, "1 == 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(Duration.One == onePrime, "1 == 1 (different objects)");
        }

        [Test]
        public void OperatorEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(Duration.One == Duration.NegativeOne, "1 == -1");
            Assert.False(Duration.One == threeMillion, "1 == 3,000,000");
            Assert.False(Duration.One == negativeFiftyMillion, "1 == -50,000,000");
            Assert.False(Duration.MinValue == Duration.MaxValue, "MinValue == MaxValue");
        }

        #endregion

        #region operator !=

        [Test]
        public void OperatorNotEquals_ToSelf_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Duration.One != Duration.One, "1 != 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorNotEquals_WithEqualTicks_IsFalse()
        {
            Assert.False(Duration.One != onePrime, "1 != 1 (different objects)");
        }

        [Test]
        public void OperatorNotEquals_WithDifferentTicks_IsTrue()
        {
            Assert.True(Duration.One != Duration.NegativeOne, "1 != -1");
            Assert.True(Duration.One != threeMillion, "1 != 3,000,000");
            Assert.True(Duration.One != negativeFiftyMillion, "1 != -50,000,000");
            Assert.True(Duration.MinValue != Duration.MaxValue, "MinValue != MaxValue");
        }

        #endregion

        #region operator <

        [Test]
        public void OperatorLessThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Duration.One < Duration.One, "1 < 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThan_EqualTicks_IsFalse()
        {
            Assert.False(Duration.One < onePrime, "1 < 1 (different objects)");
        }

        [Test]
        public void OperatorLessThan_MoreTicks_IsTrue()
        {
            Assert.True(Duration.One < threeMillion, "1 < 3,000,000");
            Assert.True(negativeFiftyMillion < Duration.NegativeOne, "-50,000,000 < -1");
            Assert.True(Duration.MinValue < Duration.MaxValue, "MinValue < MaxValue");
        }

        [Test]
        public void OperatorLessThan_LessTicks_IsFalse()
        {
            Assert.False(threeMillion < Duration.One, "3,000,000 < 1");
            Assert.False(Duration.NegativeOne < negativeFiftyMillion, "-1 < -50,000,000");
            Assert.False(Duration.MaxValue < Duration.MinValue, "MaxValue < MinValue");
        }

        #endregion

        #region operator <=

        [Test]
        public void OperatorLessThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Duration.One <= Duration.One, "1 <= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(Duration.One <= onePrime, "1 <= 1 (different objects)");
        }

        [Test]
        public void OperatorLessThanOrEqual_MoreTicks_IsTrue()
        {
            Assert.True(Duration.One <= threeMillion, "1 <= 3,000,000");
            Assert.True(negativeFiftyMillion <= Duration.NegativeOne, "-50,000,000 <= -1");
            Assert.True(Duration.MinValue <= Duration.MaxValue, "MinValue <= MaxValue");
        }

        [Test]
        public void OperatorLessThanOrEqual_LessTicks_IsFalse()
        {
            Assert.False(threeMillion <= Duration.One, "3,000,000 <= 1");
            Assert.False(Duration.NegativeOne <= negativeFiftyMillion, "-1 <= -50,000,000");
            Assert.False(Duration.MaxValue <= Duration.MinValue, "MaxValue <= MinValue");
        }

        #endregion

        #region operator >

        [Test]
        public void OperatorGreaterThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Duration.One > Duration.One, "1 > 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThan_EqualTicks_IsFalse()
        {
            Assert.False(Duration.One > onePrime, "1 > 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThan_MoreTicks_IsFalse()
        {
            Assert.False(Duration.One > threeMillion, "1 > 3,000,000");
            Assert.False(negativeFiftyMillion > Duration.NegativeOne, "-50,000,000 > -1");
            Assert.False(Duration.MinValue > Duration.MaxValue, "MinValue > MaxValue");
        }

        [Test]
        public void OperatorGreaterThan_LessTicks_IsTrue()
        {
            Assert.True(threeMillion > Duration.One, "3,000,000 > 1");
            Assert.True(Duration.NegativeOne > negativeFiftyMillion, "-1 > -50,000,000");
            Assert.True(Duration.MaxValue > Duration.MinValue, "MaxValue > MinValue");
        }

        #endregion

        #region operator >=

        [Test]
        public void OperatorGreaterThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Duration.One >= Duration.One, "1 >= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(Duration.One >= onePrime, "1 >= 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_MoreTicks_IsFalse()
        {
            Assert.False(Duration.One >= threeMillion, "1 >= 3,000,000");
            Assert.False(negativeFiftyMillion >= Duration.NegativeOne, "-50,000,000 >= -1");
            Assert.False(Duration.MinValue >= Duration.MaxValue, "MinValue >= MaxValue");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_LessTicks_IsTrue()
        {
            Assert.True(threeMillion >= Duration.One, "3,000,000 >= 1");
            Assert.True(Duration.NegativeOne >= negativeFiftyMillion, "-1 >= -50,000,000");
            Assert.True(Duration.MaxValue >= Duration.MinValue, "MaxValue >= MinValue");
        }

        #endregion

        #region operator +

        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Duration.Zero + Duration.Zero).Ticks, "0 + 0");
            Assert.AreEqual(1L, (Duration.One + Duration.Zero).Ticks, "1 + 0");
            Assert.AreEqual(1L, (Duration.Zero + Duration.One).Ticks, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(3000001L, (threeMillion + Duration.One).Ticks, "3,000,000 + 1");
            Assert.AreEqual(0L, (Duration.One + Duration.NegativeOne).Ticks, "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration.One).Ticks, "-50,000,000 + 1");
        }

        #endregion

        #region operator -

        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Duration.Zero - Duration.Zero).Ticks, "0 - 0");
            Assert.AreEqual(1L, (Duration.One - Duration.Zero).Ticks, "1 - 0");
            Assert.AreEqual(-1L, (Duration.Zero - Duration.One).Ticks, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(2999999L, (threeMillion - Duration.One).Ticks, "3,000,000 - 1");
            Assert.AreEqual(2L, (Duration.One - Duration.NegativeOne).Ticks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration.One).Ticks, "-50,000,000 - 1");
        }

        #endregion

        #region operator /
        [Test]
        public void OperatorDivision_ByNonZero()
        {
            Assert.AreEqual(1000, (threeMillion / 3000).Ticks, "3000000 / 3000");
        }

        [Test]
        public void OperatorDivision_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() => { var unused = threeMillion / 0; }, "3000000 / 0");
        }

        [Test]
        public void OperatorDivision_Truncates()
        {
            Assert.AreEqual(1, (threeMillion / 2000000).Ticks, "3000000 / 2000000");
            Assert.AreEqual(-1, (threeMillion / -2000000).Ticks, "3000000 / -2000000");
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
        #endregion
    }
}
