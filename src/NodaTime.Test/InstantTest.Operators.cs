// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class InstantTest
    {
        [Test]
        public void Equality()
        {
            Instant equal = new Instant(1, 100L);
            Instant different1 = new Instant(1, 200L);
            Instant different2 = new Instant(2, 100L);

            TestHelper.TestEqualsStruct(equal, equal, different1);
            TestHelper.TestOperatorEquality(equal, equal, different1);

            TestHelper.TestEqualsStruct(equal, equal, different2);
            TestHelper.TestOperatorEquality(equal, equal, different2);
        }

        [Test]
        public void Comparison()
        {
            Instant equal = new Instant(1, 100L);
            Instant greater1 = new Instant(1, 101L);
            Instant greater2 = new Instant(2, 0L);

            TestHelper.TestCompareToStruct(equal, equal, greater1);
            TestHelper.TestNonGenericCompareTo(equal, equal, greater1);
            TestHelper.TestOperatorComparisonEquality(equal, equal, greater1);

            TestHelper.TestCompareToStruct(equal, equal, greater2);
            TestHelper.TestNonGenericCompareTo(equal, equal, greater2);
            TestHelper.TestOperatorComparisonEquality(equal, equal, greater2);
        }

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
            Assert.AreEqual(3000001L, (threeMillion + Duration.Epsilon).TimeSinceEpoch.ToInt64Nanoseconds(), "3,000,000 + 1");
            Assert.AreEqual(0L, (one + -Duration.Epsilon).TimeSinceEpoch.ToInt64Nanoseconds(), "1 + (-1)");
            Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration.Epsilon).TimeSinceEpoch.ToInt64Nanoseconds(), "-50,000,000 + 1");
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
            Assert.AreEqual(new LocalInstant(0, NodaConstants.NanosecondsPerHour), NodaConstants.UnixEpoch.Plus(Offset.FromHours(1)), "UnixEpoch + offsetOneHour");
        }
        #endregion

        #region operator - (duration)
        [Test]
        public void OperatorMinusDuration()
        {
            Assert.AreEqual(threeMillion, threeMillion - Duration.Zero);
            Assert.AreEqual(2999999L, (threeMillion - Duration.Epsilon).TimeSinceEpoch.ToInt64Nanoseconds(), "3,000,000 - 1");
            Assert.AreEqual(2L, (one - Duration.FromNanoseconds(-1)).TimeSinceEpoch.ToInt64Nanoseconds(), "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration.Epsilon).TimeSinceEpoch.ToInt64Nanoseconds(), "-50,000,000 - 1");
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
            Assert.AreEqual(2999999L, (threeMillion - one).ToInt64Nanoseconds(), "3,000,000 - 1");
            Assert.AreEqual(2L, (one - new Instant(Duration.FromNanoseconds(-1L))).ToInt64Nanoseconds(), "1 - (-1)");
            Assert.AreEqual(-50000001L, (negativeFiftyMillion - one).ToInt64Nanoseconds(), "-50,000,000 - 1");
        }

        [Test]
        public void OperatorMinusInstant_UnixEpoch_IsNeutralElement()
        {
            Assert.AreEqual(0L, (NodaConstants.UnixEpoch - NodaConstants.UnixEpoch).ToInt64Nanoseconds(), "0 - 0");
            Assert.AreEqual(1L, (one - NodaConstants.UnixEpoch).ToInt64Nanoseconds(), "1 - 0");
            Assert.AreEqual(-1L, (NodaConstants.UnixEpoch - one).ToInt64Nanoseconds(), "0 - 1");
        }

        // Smoke tests for methods which simply delegate to the - operator.
        [Test]
        public void OperatorMinus_Instant_Equivalents()
        {
            Assert.AreEqual(threeMillion - one, threeMillion.Minus(one));
            Assert.AreEqual(threeMillion - one, Instant.Subtract(threeMillion, one));
        }
        #endregion
    }
}
