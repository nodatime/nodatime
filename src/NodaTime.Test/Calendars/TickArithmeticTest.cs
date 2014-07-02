// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    [TestFixture]
    public class TickArithmeticTest
    {
        [Test]
        [TestCase(long.MinValue)]
        [TestCase(-NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(-NodaConstants.TicksPerStandardDay)]
        [TestCase(-NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(NodaConstants.TicksPerStandardDay - 1)]
        [TestCase(NodaConstants.TicksPerStandardDay)]
        [TestCase(NodaConstants.TicksPerStandardDay + 1)]
        [TestCase(long.MaxValue)]
        public void TicksToDaysAndTickOfDayAndBack(long ticks)
        {
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);

            Assert.AreEqual(ticks, TickArithmetic.DaysAndTickOfDayToTicks(days, tickOfDay));
        }
    }
}
