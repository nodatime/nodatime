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
            Assert.True(threeHours.Equals(threeHours), "threeHours == threeHours (same object)");
        }

        [Test]
        public void IEquatableEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(threeHours.Equals(threeHours), "threeHours == threeHours (different objects)");
        }

        [Test]
        public void IEquatableEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(threeHours.Equals(negativeThreeHours), "threeHours == -threeHours");
            Assert.False(threeHours.Equals(Offset.Zero), "threeHours == 0");
            Assert.False(Offset.Zero.Equals(negativeThreeHours), "0 == -threeHours");
            Assert.False(Offset.MinValue.Equals(Offset.MaxValue), "MinValue == MaxValue");
        }

        #endregion

        #region object.Equals

        [Test]
        public void ObjectEquals_ToNull_IsFalse()
        {
            object obj = threeHours;

            Assert.False(obj.Equals(null), "threeHours == null");
        }

        [Test]
        public void ObjectEquals_ToSelf_IsTrue()
        {
            object obj = threeHours;

            Assert.True(obj.Equals(obj), "threeHours == threeHours (same object)");
        }

        [Test]
        public void ObjectEquals_WithEqualTicks_IsTrue()
        {
            object obj = threeHours;
            object objPrime = threeHoursPrime;

            Assert.True(obj.Equals(objPrime), "threeHours == threeHours (different objects)");
        }

        [Test]
        public void ObjectEquals_WithDifferentTicks_IsFalse()
        {
            object oZero = Offset.Zero;
            object oThreehours = threeHours;
            object oNegativeThreeHours = negativeThreeHours;
            object oMinimum = Offset.MinValue;
            object oMaximum = Offset.MaxValue;

            Assert.False(oZero.Equals(oThreehours), "0 == threeHours");
            Assert.False(oZero.Equals(oNegativeThreeHours), "0 == -threeHours");
            Assert.False(oThreehours.Equals(oNegativeThreeHours), "threeHours == -threeHours");
            Assert.False(oMinimum.Equals(oMaximum), "MinValue == MaxValue");
        }

        #endregion

        #region object.GetHashCode

        [Test]
        public void GetHashCode_Twice_IsEqual()
        {
            Offset test1 = MakeOffset(1, 0, 0, 0);
            Offset test2 = MakeOffset(1, 0, 0, 0);
            Assert.AreEqual(test1.GetHashCode(), test1.GetHashCode());
            Assert.AreEqual(test2.GetHashCode(), test2.GetHashCode());
        }

        [Test]
        public void GetHashCode_SameTicks_IsEqual()
        {
            Offset test1 = MakeOffset(1, 0, 0, 0);
            Offset test2 = MakeOffset(1, 0, 0, 0);
            Assert.AreEqual(test1.GetHashCode(), test2.GetHashCode());
        }

        //// This is not a valid test for two reasons:
        ////
        ////   1. return 1; is a valid GetHashCode() method i.e it always returns the same value.
        ////   2. There are more long values than int values (in fact there are 2^32 longs values
        ////      for each int value so that means that the hash code function MUST be mapping 
        ////      multiple values to each int value. 
        //// 
        //// [Test]
        //// public void GetHashCode_DifferentTicks_IsDifferent()
        //// {
        ////     Offset test1 = new Offset(123L);
        ////     Offset test2 = new Offset(123L);
        ////     Offset test3 = new Offset(321L);
        //// 
        ////     Assert.AreNotEqual(test1.GetHashCode(), test3.GetHashCode());
        ////     Assert.AreNotEqual(test2.GetHashCode(), test3.GetHashCode());
        //// }

        #endregion

        #region CompareTo

        [Test]
        public void CompareTo_Self_IsEqual()
        {
            Assert.AreEqual(0, threeHours.CompareTo(threeHours), "threeHours == threeHours (same object)");
        }

        [Test]
        public void CompareTo_WithEqualTicks_IsEqual()
        {
            Assert.AreEqual(0, threeHours.CompareTo(threeHoursPrime), "threeHours == threeHours (different objects)");
        }

        [Test]
        public void CompareTo_WithMoreTicks_IsGreater()
        {
            Assert.Greater(Offset.Zero.CompareTo(negativeThreeHours), 0, "0 > -threeHours");
            Assert.Greater(threeHours.CompareTo(Offset.Zero), 0, "threeHours > 0");
            Assert.Greater(negativeThreeHours.CompareTo(negativeTwelveHours), 0, "-threeHours > -twelveHours");
            Assert.Greater(Offset.MaxValue.CompareTo(Offset.MinValue), 0, "MaxValue > MinValue");
        }

        [Test]
        public void CompareTo_WithLessTicks_IsLess()
        {
            Assert.Less(negativeThreeHours.CompareTo(Offset.Zero), 0, "-threeHours < 0");
            Assert.Less(Offset.Zero.CompareTo(threeHours), 0, "0 < threeHours");
            Assert.Less(negativeTwelveHours.CompareTo(negativeThreeHours), 0, "-twelveHours > -threeHours");
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
            Assert.True(threeHours == threeHours, "threeHours == threeHours (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorEquals_WithEqualTicks_IsTrue()
        {
            Assert.True(threeHours == threeHoursPrime, "threeHours == threeHours (different objects)");
        }

        [Test]
        public void OperatorEquals_WithDifferentTicks_IsFalse()
        {
            Assert.False(Offset.Zero == negativeThreeHours, "0 == -threeHours");
            Assert.False(Offset.Zero == threeHours, "0 == threeHours");
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
            Assert.False(threeHours != threeHours, "threeHours != threeHours (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorNotEquals_WithEqualTicks_IsFalse()
        {
            Assert.False(threeHours != threeHoursPrime, "threeHours != threeHours (different objects)");
        }

        [Test]
        public void OperatorNotEquals_WithDifferentTicks_IsTrue()
        {
            Assert.True(Offset.Zero != negativeThreeHours, "0 != -threeHours");
            Assert.True(Offset.Zero != threeHours, "0 != threeHours");
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
            Assert.False(threeHours < threeHours, "1 < 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThan_EqualTicks_IsFalse()
        {
            Assert.False(threeHours < threeHoursPrime, "1 < 1 (different objects)");
        }

        [Test]
        public void OperatorLessThan_MoreTicks_IsTrue()
        {
            Assert.True(Offset.Zero < threeHours, "1 < 3,000,000");
            Assert.True(negativeTwelveHours < negativeThreeHours, "-50,000,000 < -1");
            Assert.True(Offset.MinValue < Offset.MaxValue, "MinValue < MaxValue");
        }

        [Test]
        public void OperatorLessThan_LessTicks_IsFalse()
        {
            Assert.False(threeHours < Offset.Zero, "3,000,000 < 1");
            Assert.False(negativeThreeHours < negativeTwelveHours, "-1 < -50,000,000");
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
            Assert.True(threeHours <= threeHours, "1 <= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorLessThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(threeHours <= threeHoursPrime, "1 <= 1 (different objects)");
        }

        [Test]
        public void OperatorLessThanOrEqual_MoreTicks_IsTrue()
        {
            Assert.True(Offset.Zero <= threeHours, "1 <= 3,000,000");
            Assert.True(negativeTwelveHours <= negativeThreeHours, "-50,000,000 <= -1");
            Assert.True(Offset.MinValue <= Offset.MaxValue, "MinValue <= MaxValue");
        }

        [Test]
        public void OperatorLessThanOrEqual_LessTicks_IsFalse()
        {
            Assert.False(threeHours <= Offset.Zero, "3,000,000 <= 1");
            Assert.False(negativeThreeHours <= negativeTwelveHours, "-1 <= -50,000,000");
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
            Assert.False(threeHours > threeHours, "1 > 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThan_EqualTicks_IsFalse()
        {
            Assert.False(threeHours > threeHoursPrime, "1 > 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThan_MoreTicks_IsFalse()
        {
            Assert.False(Offset.Zero > threeHours, "1 > 3,000,000");
            Assert.False(negativeTwelveHours > negativeThreeHours, "-50,000,000 > -1");
            Assert.False(Offset.MinValue > Offset.MaxValue, "MinValue > MaxValue");
        }

        [Test]
        public void OperatorGreaterThan_LessTicks_IsTrue()
        {
            Assert.True(threeHours > Offset.Zero, "3,000,000 > 1");
            Assert.True(negativeThreeHours > negativeTwelveHours, "-1 > -50,000,000");
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
            Assert.True(threeHours >= threeHours, "1 >= 1 (same object)");
#pragma warning restore 1718
        }

        [Test]
        public void OperatorGreaterThanOrEqual_EqualTicks_IsTrue()
        {
            Assert.True(threeHours >= threeHoursPrime, "1 >= 1 (different objects)");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_MoreTicks_IsFalse()
        {
            Assert.False(Offset.Zero >= threeHours, "1 >= 3,000,000");
            Assert.False(negativeTwelveHours >= negativeThreeHours, "-50,000,000 >= -1");
            Assert.False(Offset.MinValue >= Offset.MaxValue, "MinValue >= MaxValue");
        }

        [Test]
        public void OperatorGreaterThanOrEqual_LessTicks_IsTrue()
        {
            Assert.True(threeHours >= Offset.Zero, "3,000,000 >= 1");
            Assert.True(negativeThreeHours >= negativeTwelveHours, "-1 >= -50,000,000");
            Assert.True(Offset.MaxValue >= Offset.MinValue, "MaxValue >= MinValue");
        }

        #endregion

        #region operator +

        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0, (Offset.Zero + Offset.Zero).Milliseconds, "0 + 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), threeHours + Offset.Zero, "1 + 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), Offset.Zero + threeHours, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(MakeOffset(6, 0, 0, 0), threeHours + threeHours, "3,000,000 + 1");
            Assert.AreEqual(Offset.Zero, threeHours + negativeThreeHours, "1 + (-1)");
            Assert.AreEqual(MakeOffset(-9, 0, 0, 0), negativeTwelveHours + threeHours, "-TwelveHours + threeHours");
        }

        #endregion

        #region operator -

        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Offset.Zero, Offset.Zero - Offset.Zero, "0 - 0");
            Assert.AreEqual(MakeOffset(3, 0, 0, 0), threeHours - Offset.Zero, "1 - 0");
            Assert.AreEqual(MakeOffset(-3, 0, 0, 0), Offset.Zero - threeHours, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(Offset.Zero, threeHours - threeHours, "3,000,000 - 1");
            Assert.AreEqual(MakeOffset(6, 0, 0, 0), threeHours - negativeThreeHours, "1 - (-1)");
            Assert.AreEqual(MakeOffset(-15, 0, 0, 0), negativeTwelveHours - threeHours, "-TwelveHours - threeHours");
        }

        #endregion
    }
}
