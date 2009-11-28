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

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        #region IEquatable.Equals

        [Test]
        public void IEquatableEquals_ToSelf_IsTrue()
        {
            Assert.True(Offset.One.Equals(Offset.One), "1 == 1 (same object)");
        }

        [Test]
        public void IEquatableEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(Offset.One.Equals(onePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void IEquatableEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(Offset.One.Equals(negativeOne), "1 == -1");
            Assert.False(Offset.One.Equals(threeMillion), "1 == 3,000,000");
            Assert.False(Offset.One.Equals(negativeFiftyMillion), "1 == -50,000,000");
            Assert.False(Offset.MinValue.Equals(Offset.MaxValue), "MinValue == MaxValue");
        }

        #endregion

        #region object.Equals

        [Test]
        public void ObjectEquals_ToNull_IsFalse()
        {
            object oOne = Offset.One;

            Assert.False(oOne.Equals(null), "1 == null");
        }

        [Test]
        public void ObjectEquals_ToSelf_IsTrue()
        {
            object oOne = Offset.One;

            Assert.True(oOne.Equals(oOne), "1 == 1 (same object)");
        }

        [Test]
        public void ObjectEquals_WithEqualTicks_IsTrue()
        {
            object oOne = Offset.One;
            object oOnePrime = onePrime;

            Assert.True(oOne.Equals(oOnePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void ObjectEquals_WithDifferentTicks_IsFalse()
        {
            object oOne = Offset.One;
            object oNegativeOne = negativeOne;
            object oThreeMillion = threeMillion;
            object oNegativeFiftyMillion = negativeFiftyMillion;
            object oMinimum = Offset.MinValue;
            object oMaximum = Offset.MaxValue;

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
            Offset test1 = new Offset(123L);
            Offset test2 = new Offset(123L);
            Assert.AreEqual(test1.GetHashCode(), test1.GetHashCode());
            Assert.AreEqual(test2.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_SameTicks_IsEqual()
        {
            Offset test1 = new Offset(123L);
            Offset test2 = new Offset(123L);
            Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentTicks_IsDifferent()
        {
            Offset test1 = new Offset(123L);
            Offset test2 = new Offset(123L);
            Offset test3 = new Offset(321L);

            Assert.AreNotEqual(test1.GetHashCode(), test3.GetHashCode());
            Assert.AreNotEqual(test2.GetHashCode(), test3.GetHashCode());
        }

        #endregion

        #region CompareTo

        [Test]
        public void CompareTo_Self_IsEqual()
        {
            Assert.AreEqual(0, Offset.One.CompareTo(Offset.One), "1 == 1 (same object)");
        }

        [Test]
        public void CompareTo_WithEqualTicks_IsEqual()
        {
            Assert.AreEqual(0, Offset.One.CompareTo(onePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void CompareTo_WithMoreTicks_IsGreater()
        {
            Assert.Greater(Offset.One.CompareTo(negativeFiftyMillion), 0, "1 > -50,000,000");
            Assert.Greater(threeMillion.CompareTo(Offset.One), 0, "3,000,000 > 1");
            Assert.Greater(negativeOne.CompareTo(negativeFiftyMillion), 0, "-1 > -50,000,000");
            Assert.Greater(Offset.MaxValue.CompareTo(Offset.MinValue), 0, "MaxValue > MinValue");
        }

        [Test]
        public void CompareTo_WithLessTicks_IsLess()
        {
            Assert.Less(negativeFiftyMillion.CompareTo(Offset.One), 0, "-50,000,000 < 1");
            Assert.Less(Offset.One.CompareTo(threeMillion), 0, "1 < 3,000,000");
            Assert.Less(negativeFiftyMillion.CompareTo(negativeOne), 0, "-50,000,000 > -1");
            Assert.Less(Offset.MinValue.CompareTo(Offset.MaxValue), 0, "MinValue < MaxValue");
        }

        #endregion

        #region operator ==

        [Test]
        public void OperatorEquals_ToSelf_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Offset.One == Offset.One, "1 == 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(Offset.One == onePrime, "1 == 1 (different objects)");
        }

        [Test]
        public void OperatorEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(Offset.One == negativeOne, "1 == -1");
            Assert.False(Offset.One == threeMillion, "1 == 3,000,000");
            Assert.False(Offset.One == negativeFiftyMillion, "1 == -50,000,000");
            Assert.False(Offset.MinValue == Offset.MaxValue, "MinValue == MaxValue");
        }

        #endregion

        #region operator !=

        [Test]
        public void OperatorNotEquals_ToSelf_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Offset.One != Offset.One, "1 != 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorNotEquals_WithEqualTicks_IsFalse()
        {
            Assert.False(Offset.One != onePrime, "1 != 1 (different objects)");
        }

        [Test]
        public void OperatorNotEquals_WithDifferentTicks_IsTrue()
        {
            Assert.True(Offset.One != negativeOne, "1 != -1");
            Assert.True(Offset.One != threeMillion, "1 != 3,000,000");
            Assert.True(Offset.One != negativeFiftyMillion, "1 != -50,000,000");
            Assert.True(Offset.MinValue != Offset.MaxValue, "MinValue != MaxValue");
        }

        #endregion

        #region operator <

        [Test]
        public void OperatorLessThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Offset.One < Offset.One, "1 < 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThan_EqualTicks_IsFalse()
        {
            Assert.False(Offset.One < onePrime, "1 < 1 (different objects)");
        }

        [Test]
        public void OperatorLessThan_MoreTicks_IsTrue()
        {
            Assert.True(Offset.One < threeMillion, "1 < 3,000,000");
            Assert.True(negativeFiftyMillion < negativeOne, "-50,000,000 < -1");
            Assert.True(Offset.MinValue < Offset.MaxValue, "MinValue < MaxValue");
        }

        [Test]
        public void OperatorLessThan_LessTicks_IsFalse()
        {
            Assert.False(threeMillion < Offset.One, "3,000,000 < 1");
            Assert.False(negativeOne < negativeFiftyMillion, "-1 < -50,000,000");
            Assert.False(Offset.MaxValue < Offset.MinValue, "MaxValue < MinValue");
        }

        #endregion

        #region operator <=

        [Test]
        public void OperatorLessThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Offset.One <= Offset.One, "1 <= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(Offset.One <= onePrime, "1 <= 1 (different objects)");
        }

        [Test]
        public void OperatorLessThanOrEqual_MoreTicks_IsTrue()
        {
            Assert.True(Offset.One <= threeMillion, "1 <= 3,000,000");
            Assert.True(negativeFiftyMillion <= negativeOne, "-50,000,000 <= -1");
            Assert.True(Offset.MinValue <= Offset.MaxValue, "MinValue <= MaxValue");
        }

        [Test]
        public void OperatorLessThanOrEqual_LessTicks_IsFalse()
        {
            Assert.False(threeMillion <= Offset.One, "3,000,000 <= 1");
            Assert.False(negativeOne <= negativeFiftyMillion, "-1 <= -50,000,000");
            Assert.False(Offset.MaxValue <= Offset.MinValue, "MaxValue <= MinValue");
        }

        #endregion

        #region operator >

        [Test]
        public void OperatorGreaterThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(Offset.One > Offset.One, "1 > 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThan_EqualTicks_IsFalse()
        {
            Assert.False(Offset.One > onePrime, "1 > 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThan_MoreTicks_IsFalse()
        {
            Assert.False(Offset.One > threeMillion, "1 > 3,000,000");
            Assert.False(negativeFiftyMillion > negativeOne, "-50,000,000 > -1");
            Assert.False(Offset.MinValue > Offset.MaxValue, "MinValue > MaxValue");
        }

        [Test]
        public void OperatorGreaterThan_LessTicks_IsTrue()
        {
            Assert.True(threeMillion > Offset.One, "3,000,000 > 1");
            Assert.True(negativeOne > negativeFiftyMillion, "-1 > -50,000,000");
            Assert.True(Offset.MaxValue > Offset.MinValue, "MaxValue > MinValue");
        }

        #endregion

        #region operator >=

        [Test]
        public void OperatorGreaterThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(Offset.One >= Offset.One, "1 >= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(Offset.One >= onePrime, "1 >= 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_MoreTicks_IsFalse()
        {
            Assert.False(Offset.One >= threeMillion, "1 >= 3,000,000");
            Assert.False(negativeFiftyMillion >= negativeOne, "-50,000,000 >= -1");
            Assert.False(Offset.MinValue >= Offset.MaxValue, "MinValue >= MaxValue");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_LessTicks_IsTrue()
        {
            Assert.True(threeMillion >= Offset.One, "3,000,000 >= 1");
            Assert.True(negativeOne >= negativeFiftyMillion, "-1 >= -50,000,000");
            Assert.True(Offset.MaxValue >= Offset.MinValue, "MaxValue >= MinValue");
        }

        #endregion

        #region operator +

        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Offset.Zero + Offset.Zero).Ticks, "0 + 0");
            Assert.AreEqual(1L, (Offset.One + Offset.Zero).Ticks, "1 + 0");
            Assert.AreEqual(1L, (Offset.Zero + Offset.One).Ticks, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(3000001L, (threeMillion + Offset.One).Ticks, "3,000,000 + 1");
            Assert.AreEqual(0L, (Offset.One + negativeOne).Ticks, "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Offset.One).Ticks, "-50,000,000 + 1");
        }

        #endregion

        #region operator -

        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Offset.Zero - Offset.Zero).Ticks, "0 - 0");
            Assert.AreEqual(1L, (Offset.One - Offset.Zero).Ticks, "1 - 0");
            Assert.AreEqual(-1L, (Offset.Zero - Offset.One).Ticks, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(2999999L, (threeMillion - Offset.One).Ticks, "3,000,000 - 1");
            Assert.AreEqual(2L, (Offset.One - negativeOne).Ticks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - Offset.One).Ticks, "-50,000,000 - 1");
        }

        #endregion
    }
}
