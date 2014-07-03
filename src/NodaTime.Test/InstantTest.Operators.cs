// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class InstantTest
    {
        #region IEquatable.Equals
        [Test]
        public void IEquatableEquals_ToSelf_IsTrue()
        {
            Assert.True(NodaConstants.UnixEpoch.Equals(NodaConstants.UnixEpoch), "epoch == epoch (same object)");
        }

        [Test]
        public void IEquatableEquals_WithEqualTicks_IsTrue()
        {
            var first = Instant.FromTicksSinceUnixEpoch(100L);
            var second = Instant.FromTicksSinceUnixEpoch(100L);
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
        public void ObjectEquals_ToNonInstant_IsFalse()
        {
            object oOne = one;

            Assert.False(oOne.Equals("foo"));
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
            var test1 = Instant.FromTicksSinceUnixEpoch(123L);
            var test2 = Instant.FromTicksSinceUnixEpoch(123L);
            Assert.AreEqual(test1.GetHashCode(), test1.GetHashCode());
            Assert.AreEqual(test2.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_SameTicks_IsEqual()
        {
            var test1 = Instant.FromTicksSinceUnixEpoch(123L);
            var test2 = Instant.FromTicksSinceUnixEpoch(123L);
            Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_DifferentTicks_IsDifferent()
        {
            var test1 = Instant.FromTicksSinceUnixEpoch(123L);
            var test2 = Instant.FromTicksSinceUnixEpoch(123L);
            var test3 = Instant.FromTicksSinceUnixEpoch(321L);

            Assert.AreNotEqual(test1.GetHashCode(), test3.GetHashCode());
            Assert.AreNotEqual(test2.GetHashCode(), test3.GetHashCode());
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
        public void PlusTicks()
        {
            Instant instant = Instant.FromTicksSinceUnixEpoch(5);
            Assert.AreEqual(Instant.FromTicksSinceUnixEpoch(8), instant.PlusTicks(3));
        }

        [Test]
        public void OperatorPlusDuration_Zero_IsNeutralElement()
        {
            Assert.AreEqual(NodaConstants.UnixEpoch, NodaConstants.UnixEpoch + Duration.Zero, "UnixEpoch + Duration.Zero");
            Assert.AreEqual(one, one + Duration.Zero, "Instant(1) + Duration.Zero");
            Assert.AreEqual(one, NodaConstants.UnixEpoch + Duration.Epsilon, "UnixEpoch + Duration.Epsilon");
        }

        [Test]
        public void OperatorPlusDuration_NonZero()
        {
            Assert.AreEqual(3000001L, (threeMillion + Duration.Epsilon).Ticks, "3,000,000 + 1");
            Assert.AreEqual(0L, (one + durationNegativeEpsilon).Ticks, "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration.Epsilon).Ticks, "-50,000,000 + 1");
        }

        // Smoke tests for methods which simply delegate to the + operator.
        [Test]
        public void OperatorPlus_Equivalents()
        {
            Assert.AreEqual(threeMillion + Duration.Epsilon, threeMillion.Plus(Duration.Epsilon));
            Assert.AreEqual(threeMillion + Duration.Epsilon, Instant.Add(threeMillion, Duration.Epsilon));
        }

        // The Plus(Offset) method *would* be an operator, but can't be as LocalInstant is internal.
        [Test]
        public void OperatorPlusOffset_Zero_IsNeutralElement()
        {
            Assert.AreEqual(LocalInstant.LocalUnixEpoch, NodaConstants.UnixEpoch.Plus(Offset.Zero), "UnixEpoch + Offset.Zero");
            Assert.AreEqual(new LocalInstant(0, 1L), one.Plus(Offset.Zero), "Instant(1) + Offset.Zero");
            Assert.AreEqual(new LocalInstant(0, NodaConstants.TicksPerHour), NodaConstants.UnixEpoch.Plus(offsetOneHour), "UnixEpoch + offsetOneHour");
        }
        #endregion

        #region operator - (duration)
        [Test]
        public void OperatorMinusDuration()
        {
            Assert.AreEqual(threeMillion, threeMillion - Duration.Zero);
            Assert.AreEqual(2999999L, (threeMillion - Duration.Epsilon).Ticks, "3,000,000 - 1");
            Assert.AreEqual(2L, (one - Duration.FromTicks(-1)).Ticks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration.Epsilon).Ticks, "-50,000,000 - 1");
        }

        // Smoke tests for methods which simply delegate to the - operator.
        [Test]
        public void OperatorMinus_Duration_Equivalents()
        {
            Assert.AreEqual(threeMillion - Duration.Epsilon, threeMillion.Minus(Duration.Epsilon));
            Assert.AreEqual(threeMillion - Duration.Epsilon, Instant.Subtract(threeMillion, Duration.Epsilon));
        }
        #endregion

        #region operator - (instant)
        [Test]
        public void OperatorMinusInstant_NonZero()
        {
            Assert.AreEqual(2999999L, (threeMillion - one).Ticks, "3,000,000 - 1");
            Assert.AreEqual(2L, (one - negativeOne).Ticks, "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - one).Ticks, "-50,000,000 - 1");
        }

        [Test]
        public void OperatorMinusInstant_UnixEpoch_IsNeutralElement()
        {
            Assert.AreEqual(0L, (NodaConstants.UnixEpoch - NodaConstants.UnixEpoch).Ticks, "0 - 0");
            Assert.AreEqual(1L, (one - NodaConstants.UnixEpoch).Ticks, "1 - 0");
            Assert.AreEqual(-1L, (NodaConstants.UnixEpoch - one).Ticks, "0 - 1");
        }

        // Smoke tests for methods which simply delegate to the - operator.
        [Test]
        public void OperatorMinus_Instant_Equivalents()
        {
            Assert.AreEqual(threeMillion - one, threeMillion.Minus(one));
            Assert.AreEqual(threeMillion - one, Instant.Subtract(threeMillion, one));
        }
        #endregion

        #region IComparable and IComparable{T}
        [Test]
        public void CompareTo()
        {
            TestHelper.TestCompareToStruct(one, one, threeMillion);
            TestHelper.TestCompareToStruct(Instant.MinValue, Instant.MinValue, Instant.MaxValue);
            TestHelper.TestNonGenericCompareTo(one, one, threeMillion);
            TestHelper.TestNonGenericCompareTo(Instant.MinValue, Instant.MinValue, Instant.MaxValue);
        }
        #endregion
    }
}
