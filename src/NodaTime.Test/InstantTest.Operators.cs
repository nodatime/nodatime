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
    partial class InstantTest
    {
        #region IEquatable.Equals
        [Test]
        public void IEquatableEquals_ToSelf_IsTrue()
        {
            Assert.True(Instant.UnixEpoch.Equals(Instant.UnixEpoch), "epoch == epoch (same object)");
        }

        [Test]
        public void IEquatableEquals_WithEqualTicks_IsTrue()
        {
            Instant first = new Instant(100L);
            Instant second = new Instant(100L);
            Assert.True(first.Equals(second), "100 == 100 (different objects)");
        }

        [Test]
        public void IEquatableEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(one.Equals(negativeOne), "1 == -1");
            Assert.False(one.Equals(threeMillion), "1 == 3,000,000");
            Assert.False(one.Equals(negativeFiftyMillion), "1 == -50,000,000");
            Assert.False(Instant.MinValue.Equals(Instant.MaxValue), "MinValue == MaxValue");
        }
        #endregion

        #region object.Equals
        [Test]
        public void ObjectEquals_ToNull_IsFalse()
        {
            object oOne = one;

            Assert.False(oOne.Equals(null), "1 == null");
        }

        [Test]
        public void ObjectEquals_ToSelf_IsTrue()
        {
            object oOne = one;

            Assert.True(oOne.Equals(oOne), "1 == 1 (same object)");
        }

        [Test]
        public void ObjectEquals_WithEqualTicks_IsTrue()
        {
            object oOne = one;
            object oOnePrime = onePrime;

            Assert.True(oOne.Equals(oOnePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void ObjectEquals_WithDifferentTicks_IsFalse()
        {
            object oOne = one;
            object oNegativeOne = negativeOne;
            object oThreeMillion = threeMillion;
            object oNegativeFiftyMillion = negativeFiftyMillion;
            object oMinimum = Instant.MinValue;
            object oMaximum = Instant.MaxValue;

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
            Instant test1 = new Instant(123L);
            Instant test2 = new Instant(123L);
            Assert.AreEqual(test1.GetHashCode(), test1.GetHashCode());
            Assert.AreEqual(test2.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_SameTicks_IsEqual()
        {
            Instant test1 = new Instant(123L);
            Instant test2 = new Instant(123L);
            Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentTicks_IsDifferent()
        {
            Instant test1 = new Instant(123L);
            Instant test2 = new Instant(123L);
            Instant test3 = new Instant(321L);

            Assert.AreNotEqual(test1.GetHashCode(), test3.GetHashCode());
            Assert.AreNotEqual(test2.GetHashCode(), test3.GetHashCode());
        }
        #endregion

        #region CompareTo
        [Test]
        public void CompareTo_Self_IsEqual()
        {
            Assert.AreEqual(0, one.CompareTo(one), "1 == 1 (same object)");
        }

        [Test]
        public void CompareTo_WithEqualTicks_IsEqual()
        {
            Assert.AreEqual(0, one.CompareTo(onePrime), "1 == 1 (different objects)");
        }

        [Test]
        public void CompareTo_WithMoreTicks_IsGreater()
        {
            Assert.Greater(one.CompareTo(negativeFiftyMillion), 0, "1 > -50,000,000");
            Assert.Greater(threeMillion.CompareTo(one), 0, "3,000,000 > 1");
            Assert.Greater(negativeOne.CompareTo(negativeFiftyMillion), 0, "-1 > -50,000,000");
            Assert.Greater(Instant.MaxValue.CompareTo(Instant.MinValue), 0, "MaxValue > MinValue");
        }

        [Test]
        public void CompareTo_WithLessTicks_IsLess()
        {
            Assert.Less(negativeFiftyMillion.CompareTo(one), 0, "-50,000,000 < 1");
            Assert.Less(one.CompareTo(threeMillion), 0, "1 < 3,000,000");
            Assert.Less(negativeFiftyMillion.CompareTo(negativeOne), 0, "-50,000,000 > -1");
            Assert.Less(Instant.MinValue.CompareTo(Instant.MaxValue), 0, "MinValue < MaxValue");
        }
        #endregion

        #region operator ==
        [Test]
        public void OperatorEquals_ToSelf_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one == one, "1 == 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(one == onePrime, "1 == 1 (different objects)");
        }

        [Test]
        public void OperatorEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(one == negativeOne, "1 == -1");
            Assert.False(one == threeMillion, "1 == 3,000,000");
            Assert.False(one == negativeFiftyMillion, "1 == -50,000,000");
            Assert.False(Instant.MinValue == Instant.MaxValue, "MinValue == MaxValue");
        }
        #endregion

        #region operator !=
        [Test]
        public void OperatorNotEquals_ToSelf_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one != one, "1 != 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorNotEquals_WithEqualTicks_IsFalse()
        {
            Assert.False(one != onePrime, "1 != 1 (different objects)");
        }

        [Test]
        public void OperatorNotEquals_WithDifferentTicks_IsTrue()
        {
            Assert.True(one != negativeOne, "1 != -1");
            Assert.True(one != threeMillion, "1 != 3,000,000");
            Assert.True(one != negativeFiftyMillion, "1 != -50,000,000");
            Assert.True(Instant.MinValue != Instant.MaxValue, "MinValue != MaxValue");
        }
        #endregion

        #region operator <
        [Test]
        public void OperatorLessThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one < one, "1 < 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThan_EqualTicks_IsFalse()
        {
            Assert.False(one < onePrime, "1 < 1 (different objects)");
        }

        [Test]
        public void OperatorLessThan_MoreTicks_IsTrue()
        {
            Assert.True(one < threeMillion, "1 < 3,000,000");
            Assert.True(negativeFiftyMillion < negativeOne, "-50,000,000 < -1");
            Assert.True(Instant.MinValue < Instant.MaxValue, "MinValue < MaxValue");
        }

        [Test]
        public void OperatorLessThan_LessTicks_IsFalse()
        {
            Assert.False(threeMillion < one, "3,000,000 < 1");
            Assert.False(negativeOne < negativeFiftyMillion, "-1 < -50,000,000");
            Assert.False(Instant.MaxValue < Instant.MinValue, "MaxValue < MinValue");
        }
        #endregion

        #region operator <=
        [Test]
        public void OperatorLessThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one <= one, "1 <= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(one <= onePrime, "1 <= 1 (different objects)");
        }

        [Test]
        public void OperatorLessThanOrEqual_MoreTicks_IsTrue()
        {
            Assert.True(one <= threeMillion, "1 <= 3,000,000");
            Assert.True(negativeFiftyMillion <= negativeOne, "-50,000,000 <= -1");
            Assert.True(Instant.MinValue <= Instant.MaxValue, "MinValue <= MaxValue");
        }

        [Test]
        public void OperatorLessThanOrEqual_LessTicks_IsFalse()
        {
            Assert.False(threeMillion <= one, "3,000,000 <= 1");
            Assert.False(negativeOne <= negativeFiftyMillion, "-1 <= -50,000,000");
            Assert.False(Instant.MaxValue <= Instant.MinValue, "MaxValue <= MinValue");
        }
        #endregion

        #region operator >
        [Test]
        public void OperatorGreaterThan_Self_IsFalse()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.False(one > one, "1 > 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThan_EqualTicks_IsFalse()
        {
            Assert.False(one > onePrime, "1 > 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThan_MoreTicks_IsFalse()
        {
            Assert.False(one > threeMillion, "1 > 3,000,000");
            Assert.False(negativeFiftyMillion > negativeOne, "-50,000,000 > -1");
            Assert.False(Instant.MinValue > Instant.MaxValue, "MinValue > MaxValue");
        }

        [Test]
        public void OperatorGreaterThan_LessTicks_IsTrue()
        {
            Assert.True(threeMillion > one, "3,000,000 > 1");
            Assert.True(negativeOne > negativeFiftyMillion, "-1 > -50,000,000");
            Assert.True(Instant.MaxValue > Instant.MinValue, "MaxValue > MinValue");
        }
        #endregion

        #region operator >=
        [Test]
        public void OperatorGreaterThanOrEqual_Self_IsTrue()
        {
            // Warning CS1718: Comparison made to same variable; did you mean to compare something else?
            // This is intentional for testing
#pragma warning disable 1718
            Assert.True(one >= one, "1 >= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(one >= onePrime, "1 >= 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_MoreTicks_IsFalse()
        {
            Assert.False(one >= threeMillion, "1 >= 3,000,000");
            Assert.False(negativeFiftyMillion >= negativeOne, "-50,000,000 >= -1");
            Assert.False(Instant.MinValue >= Instant.MaxValue, "MinValue >= MaxValue");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_LessTicks_IsTrue()
        {
            Assert.True(threeMillion >= one, "3,000,000 >= 1");
            Assert.True(negativeOne >= negativeFiftyMillion, "-1 >= -50,000,000");
            Assert.True(Instant.MaxValue >= Instant.MinValue, "MaxValue >= MinValue");
        }
        #endregion

        #region operator +
        [Test]
        public void OperatorPlusDuration_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Instant.UnixEpoch, Instant.UnixEpoch + Duration.Zero, "UnixEpoch + Duration.Zero");
            Assert.AreEqual(one, one + Duration.Zero, "Instant(1) + Duration.Zero");
            Assert.AreEqual(one, Instant.UnixEpoch + Duration.One, "UnixEpoch + Duration.One");
        }

        [Test]
        public void OperatorPlusDuration_NonZero()
        {
            Assert.AreEqual(3000001L, (threeMillion + Duration.One).Ticks, "3,000,000 + 1");
            Assert.AreEqual(0L, (one + Duration.NegativeOne).Ticks, "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration.One).Ticks, "-50,000,000 + 1");
        }

        [Test]
        public void OperatorPlusOffset_Zero_IsNeutralElement()
        {
            Assert.AreEqual(LocalInstant.LocalUnixEpoch, Instant.Add(Instant.UnixEpoch, Offset.Zero), "UnixEpoch + Offset.Zero");
            Assert.AreEqual(new LocalInstant(1L), Instant.Add(one, Offset.Zero), "Instant(1) + Offset.Zero");
            Assert.AreEqual(new LocalInstant(NodaConstants.TicksPerHour), Instant.Add(Instant.UnixEpoch, offsetOneHour), "UnixEpoch + offsetOneHour");
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinusDuration_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0L, (Instant.UnixEpoch - Instant.UnixEpoch).Ticks, "0 - 0");
            Assert.AreEqual(1L, (one - Instant.UnixEpoch).Ticks, "1 - 0");
            Assert.AreEqual(-1L, (Instant.UnixEpoch - one).Ticks, "0 - 1");
        }

        [Test]
        public void OperatorMinusDuration_NonZero()
        {
            Assert.AreEqual(2999999L, (threeMillion - one).Ticks, "3,000,000 - 1");
            Assert.AreEqual(2L, (one - negativeOne).Ticks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - one).Ticks, "-50,000,000 - 1");
        }
        #endregion
    }
}