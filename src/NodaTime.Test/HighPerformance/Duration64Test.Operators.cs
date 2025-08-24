// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.HighPerformance;
using NUnit.Framework;

namespace NodaTime.Test.HighPerformance;

partial class Duration64Test
{
    private readonly Duration64 threeMillion = Duration64.FromNanoseconds(3000000L);
    private readonly Duration64 negativeFiftyMillion = Duration64.FromNanoseconds(-50000000L);

    #region operator +
    [Test]
    public void OperatorPlus_Zero_IsNeutralElement()
    {
        Assert.AreEqual(0L, (Duration64.Zero + Duration64.Zero).Nanoseconds, "0 + 0");
        Assert.AreEqual(1L, (Duration64.Epsilon + Duration64.Zero).Nanoseconds, "1 + 0");
        Assert.AreEqual(1L, (Duration64.Zero + Duration64.Epsilon).Nanoseconds, "0 + 1");
    }

    [Test]
    public void OperatorPlus_NonZero()
    {
        Assert.AreEqual(3000001L, (threeMillion + Duration64.Epsilon).Nanoseconds, "3,000,000 + 1");
        Assert.AreEqual(0L, (Duration64.Epsilon + Duration64.FromNanoseconds(-1)).Nanoseconds, "1 + (-1)");
        Assert.AreEqual(-49999999L, (negativeFiftyMillion + Duration64.Epsilon).Nanoseconds, "-50,000,000 + 1");
    }

    [Test]
    public void OperatorPlus_MethodEquivalents()
    {
        Duration64 x = Duration64.FromNanoseconds(100);
        Duration64 y = Duration64.FromNanoseconds(200);
        Assert.AreEqual(x + y, Duration64.Add(x, y));
        Assert.AreEqual(x + y, x.Plus(y));
    }
    #endregion

    #region operator -
    [Test]
    public void OperatorMinus_Zero_IsNeutralElement()
    {
        Assert.AreEqual(0L, (Duration64.Zero - Duration64.Zero).Nanoseconds, "0 - 0");
        Assert.AreEqual(1L, (Duration64.Epsilon - Duration64.Zero).Nanoseconds, "1 - 0");
        Assert.AreEqual(-1L, (Duration64.Zero - Duration64.Epsilon).Nanoseconds, "0 - 1");
    }

    [Test]
    public void OperatorMinus_NonZero()
    {
        Duration64 negativeEpsilon = Duration64.FromNanoseconds(-1L);
        Assert.AreEqual(2999999L, (threeMillion - Duration64.Epsilon).Nanoseconds, "3,000,000 - 1");
        Assert.AreEqual(2L, (Duration64.Epsilon - negativeEpsilon).Nanoseconds, "1 - (-1)");
        Assert.AreEqual(-50000001L, (negativeFiftyMillion - Duration64.Epsilon).Nanoseconds, "-50,000,000 - 1");
    }

    [Test]
    public void OperatorMinus_MethodEquivalents()
    {
        Duration64 x = Duration64.FromNanoseconds(100);
        Duration64 y = Duration64.FromNanoseconds(200);
        Assert.AreEqual(x - y, Duration64.Subtract(x, y));
        Assert.AreEqual(x - y, x.Minus(y));
    }
    #endregion

    [Test]
    public void UnaryMinusAndNegate()
    {
        var start = Duration64.FromNanoseconds(5000);
        var expected = Duration64.FromNanoseconds(-5000);
        Assert.AreEqual(expected, -start);
        Assert.AreEqual(expected, Duration64.Negate(start));
    }

    [Test]
    public void UnaryAddition()
    {
        Duration64 duration = Duration64.FromNanoseconds(5000);
        Duration64 addition = +duration;
        Assert.AreEqual(duration, addition);
    }
}
