// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class OffsetTest
    {
        [Test]
        public void IEquatableIComparable_Tests()
        {
            var value = Offset.FromMilliseconds(12345);
            var equalValue = Offset.FromMilliseconds(12345);
            var greaterValue = Offset.FromMilliseconds(5432199);

            TestHelper.TestEqualsStruct(value, equalValue, greaterValue);
            TestHelper.TestCompareToStruct(value, equalValue, greaterValue);
            TestHelper.TestNonGenericCompareTo(value, equalValue, greaterValue);
            TestHelper.TestOperatorComparisonEquality(value, equalValue, greaterValue);
        }

        [Test]
        public void UnaryPlusOperator()
        {
            Assert.AreEqual(Offset.Zero, +Offset.Zero, "+ 0");
            Assert.AreEqual(Offset.FromMilliseconds(1), + Offset.FromMilliseconds(1), "+ 1");
            Assert.AreEqual(Offset.FromMilliseconds(-7), + Offset.FromMilliseconds(-7), "+ (-7)");
        }

        [Test]
        public void NegateOperator()
        {
            Assert.AreEqual(Offset.Zero, -Offset.Zero, "-0");
            Assert.AreEqual(Offset.FromMilliseconds(-1), -Offset.FromMilliseconds(1), "-1");
            Assert.AreEqual(Offset.FromMilliseconds(7), -Offset.FromMilliseconds(-7), "- (-7)");
        }

        [Test]
        public void NegateMethod()
        {
            Assert.AreEqual(Offset.Zero, Offset.Negate(Offset.Zero), "-0");
            Assert.AreEqual(Offset.FromMilliseconds(-1), Offset.Negate(Offset.FromMilliseconds(1)), "-1");
            Assert.AreEqual(Offset.FromMilliseconds(7), Offset.Negate(Offset.FromMilliseconds(-7)), "- (-7)");
        }

        #region operator +
        [Test]
        public void OperatorPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0, (Offset.Zero + Offset.Zero).Milliseconds, "0 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), ThreeHours + Offset.Zero, "1 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), Offset.Zero + ThreeHours, "0 + 1");
        }

        [Test]
        public void OperatorPlus_NonZero()
        {
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), ThreeHours + ThreeHours, "3,000,000 + 1");
            Assert.AreEqual(Offset.Zero, ThreeHours + NegativeThreeHours, "1 + (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(9, 0, 0, 0), NegativeTwelveHours + ThreeHours, "-TwelveHours + threeHours");
        }

        // Static method equivalents
        [Test]
        public void MethodAdd_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0, Offset.Add(Offset.Zero, Offset.Zero).Milliseconds, "0 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), Offset.Add(ThreeHours, Offset.Zero), "1 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), Offset.Add(Offset.Zero, ThreeHours), "0 + 1");
        }

        [Test]
        public void MethodAdd_NonZero()
        {
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), Offset.Add(ThreeHours, ThreeHours), "3,000,000 + 1");
            Assert.AreEqual(Offset.Zero, Offset.Add(ThreeHours, NegativeThreeHours), "1 + (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(9, 0, 0, 0), Offset.Add(NegativeTwelveHours, ThreeHours), "-TwelveHours + threeHours");
        }

        // Instance method equivalents
        [Test]
        public void MethodPlus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(0, Offset.Zero.Plus(Offset.Zero).Milliseconds, "0 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), ThreeHours.Plus(Offset.Zero), "1 + 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), Offset.Zero.Plus(ThreeHours), "0 + 1");
        }

        [Test]
        public void MethodPlus_NonZero()
        {
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), ThreeHours.Plus(ThreeHours), "3,000,000 + 1");
            Assert.AreEqual(Offset.Zero, ThreeHours.Plus(NegativeThreeHours), "1 + (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(9, 0, 0, 0), NegativeTwelveHours.Plus(ThreeHours), "-TwelveHours + threeHours");
        }
        #endregion

        #region operator -
        [Test]
        public void OperatorMinus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Offset.Zero, Offset.Zero - Offset.Zero, "0 - 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), ThreeHours - Offset.Zero, "1 - 0");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(3, 0, 0, 0), Offset.Zero - ThreeHours, "0 - 1");
        }

        [Test]
        public void OperatorMinus_NonZero()
        {
            Assert.AreEqual(Offset.Zero, ThreeHours - ThreeHours, "3,000,000 - 1");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), ThreeHours - NegativeThreeHours, "1 - (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(15, 0, 0, 0), NegativeTwelveHours - ThreeHours, "-TwelveHours - threeHours");
        }

        // Static method equivalents
        [Test]
        public void Subtract_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Offset.Zero, Offset.Subtract(Offset.Zero, Offset.Zero), "0 - 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), Offset.Subtract(ThreeHours, Offset.Zero), "1 - 0");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(3, 0, 0, 0), Offset.Subtract(Offset.Zero, ThreeHours), "0 - 1");
        }

        [Test]
        public void Subtract_NonZero()
        {
            Assert.AreEqual(Offset.Zero, Offset.Subtract(ThreeHours, ThreeHours), "3,000,000 - 1");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), Offset.Subtract(ThreeHours, NegativeThreeHours), "1 - (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(15, 0, 0, 0), Offset.Subtract(NegativeTwelveHours, ThreeHours), "-TwelveHours - threeHours");
        }

        // Instance method equivalents
        [Test]
        public void Minus_Zero_IsNeutralElement()
        {
            Assert.AreEqual(Offset.Zero, Offset.Zero.Minus(Offset.Zero), "0 - 0");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(3, 0, 0, 0), ThreeHours.Minus(Offset.Zero), "1 - 0");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(3, 0, 0, 0), Offset.Zero.Minus(ThreeHours), "0 - 1");
        }

        [Test]
        public void Minus_NonZero()
        {
            Assert.AreEqual(Offset.Zero, ThreeHours.Minus(ThreeHours), "3,000,000 - 1");
            Assert.AreEqual(TestObjects.CreatePositiveOffset(6, 0, 0, 0), ThreeHours.Minus(NegativeThreeHours), "1 - (-1)");
            Assert.AreEqual(TestObjects.CreateNegativeOffset(15, 0, 0, 0), NegativeTwelveHours.Minus(ThreeHours), "-TwelveHours - threeHours");
        }
        #endregion
    }
}