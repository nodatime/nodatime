// Copyright 2025 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.HighPerformance;
using NUnit.Framework;
using System;

namespace NodaTime.Test.HighPerformance;

partial class Instant64Test
{
    [Test]
    public void Equality()
    {
        Instant64 equal = Instant64.FromUnixTimeNanoseconds(1);
        Instant64 different1 = Instant64.FromUnixTimeNanoseconds(100);
        Instant64 different2 = Instant64.FromUnixTimeNanoseconds(200);

        TestHelper.TestEqualsStruct(equal, equal, different1);
        TestHelper.TestOperatorEquality(equal, equal, different1);

        TestHelper.TestEqualsStruct(equal, equal, different2);
        TestHelper.TestOperatorEquality(equal, equal, different2);
    }

    [Test]
    public void Comparison()
    {
        Instant64 equal = Instant64.FromUnixTimeNanoseconds(1);
        Instant64 greater1 = Instant64.FromUnixTimeNanoseconds(100);
        Instant64 greater2 = Instant64.FromUnixTimeNanoseconds(200);

        TestHelper.TestCompareToStruct(equal, equal, greater1);
        TestHelper.TestNonGenericCompareTo(equal, equal, greater1);
        TestHelper.TestOperatorComparisonEquality(equal, equal, greater1, greater2);
    }

    #region operator +
    [Test]
    public void PlusTicks()
    {
        Instant64 instant = Instant64.FromUnixTimeTicks(5);
        Assert.AreEqual(Instant64.FromUnixTimeTicks(8), instant.PlusTicks(3));
    }

    [Test]
    public void PlusNanoseconds()
    {
        Instant64 instant = Instant64.FromUnixTimeTicks(5);
        Assert.AreEqual(Instant64.FromUnixTimeTicks(8), instant.PlusNanoseconds(300));
    }

    [Test]
    public void OperatorPlusDuration_Zero_IsNeutralElement()
    {
        Assert.AreEqual(Instant64.UnixEpoch, Instant64.UnixEpoch + Duration64.Zero, "UnixEpoch + Duration.Zero");
        Assert.AreEqual(one, one + Duration64.Zero, "Instant(1) + Duration.Zero");
        Assert.AreEqual(one, Instant64.UnixEpoch + Duration64.Epsilon, "UnixEpoch + Duration.Epsilon");
    }

    [Test]
    public void OperatorPlusDuration_NonZero()
    {
        Assert.AreEqual(3000001L, (threeMillion + Duration64.Epsilon).TimeSinceEpoch.TotalNanoseconds, "3,000,000 + 1");
        Assert.AreEqual(0L, (one + -Duration64.Epsilon).TimeSinceEpoch.TotalNanoseconds, "1 + (-1)");
        Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration64.Epsilon).TimeSinceEpoch.TotalNanoseconds, "-50,000,000 + 1");
    }

    // Smoke tests for methods which simply delegate to the + operator.
    [Test]
    public void OperatorPlus_Equivalents()
    {
        Assert.AreEqual(threeMillion + Duration64.Epsilon, threeMillion.Plus(Duration64.Epsilon));
        Assert.AreEqual(threeMillion + Duration64.Epsilon, Instant64.Add(threeMillion, Duration64.Epsilon));
    }

    [Test]
    public void OperatorPlus_OutOfRange()
    {
        Assert.Throws<OverflowException>(() => (Instant64.MaxValue + Duration64.Epsilon).GetHashCode());
    }
    #endregion

    #region operator - (duration)
    [Test]
    public void OperatorMinusDuration()
    {
        Assert.AreEqual(threeMillion, threeMillion - Duration64.Zero);
        Assert.AreEqual(2999999L, (threeMillion - Duration64.Epsilon).TimeSinceEpoch.TotalNanoseconds, "3,000,000 - 1");
        Assert.AreEqual(2L, (one - Duration64.FromNanoseconds(-1)).TimeSinceEpoch.TotalNanoseconds, "1 - (-1)");
        Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration64.Epsilon).TimeSinceEpoch.TotalNanoseconds, "-50,000,000 - 1");
    }

    // Smoke tests for methods which simply delegate to the - operator.
    [Test]
    public void OperatorMinus_Duration_Equivalents()
    {
        Assert.AreEqual(threeMillion - Duration64.Epsilon, threeMillion.Minus(Duration64.Epsilon));
        Assert.AreEqual(threeMillion - Duration64.Epsilon, Instant64.Subtract(threeMillion, Duration64.Epsilon));
    }

    [Test]
    public void OperatorMinus_OutOfRange()
    {
        Assert.Throws<OverflowException>(() => (Instant64.MinValue - Duration64.Epsilon).GetHashCode());
    }
    #endregion

    #region operator - (instant)
    [Test]
    public void OperatorMinusInstant_NonZero()
    {
        Assert.AreEqual(2999999L, (threeMillion - one).TotalNanoseconds, "3,000,000 - 1");
        Assert.AreEqual(2L, (one - Instant64.FromUnixTimeNanoseconds(-1)).TotalNanoseconds, "1 - (-1)");
        Assert.AreEqual(-50000001L, (negativeFiftyMillion - one).TotalNanoseconds, "-50,000,000 - 1");
    }

    [Test]
    public void OperatorMinusInstant_UnixEpoch_IsNeutralElement()
    {
        Assert.AreEqual(0L, (Instant64.UnixEpoch - Instant64.UnixEpoch).TotalNanoseconds, "0 - 0");
        Assert.AreEqual(1L, (one - Instant64.UnixEpoch).TotalNanoseconds, "1 - 0");
        Assert.AreEqual(-1L, (Instant64.UnixEpoch - one).TotalNanoseconds, "0 - 1");
    }

    // Smoke tests for methods which simply delegate to the - operator.
    [Test]
    public void OperatorMinus_Instant_Equivalents()
    {
        Assert.AreEqual(threeMillion - one, threeMillion.Minus(one));
        Assert.AreEqual(threeMillion - one, Instant64.Subtract(threeMillion, one));
    }
    #endregion
}
