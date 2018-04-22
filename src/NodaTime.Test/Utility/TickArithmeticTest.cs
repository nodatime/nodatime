// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test.Utility
{
    public class TickArithmeticTest
    {
        [Test]
        [TestCase(long.MinValue)]
        [TestCase(-NodaConstants.TicksPerDay - 1)]
        [TestCase(-NodaConstants.TicksPerDay)]
        [TestCase(-NodaConstants.TicksPerDay + 1)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(NodaConstants.TicksPerDay - 1)]
        [TestCase(NodaConstants.TicksPerDay)]
        [TestCase(NodaConstants.TicksPerDay + 1)]
        [TestCase(long.MaxValue)]
        public void TicksToDaysAndTickOfDayAndBack(long ticks)
        {
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out long tickOfDay);

            Assert.AreEqual(ticks, TickArithmetic.DaysAndTickOfDayToTicks(days, tickOfDay));
        }

        [Test]
        public void DaysAndTickOfDayToTicksUncheckedBoundaries()
        {
            // Only a useful test under debug, but this proves that the arithmetic won't overflow when used from
            // LocalDateTime or Instant. (In debug mode, we have 
            TickArithmetic.BoundedDaysAndTickOfDayToTicks(CalendarSystem.Iso.MinDays, 0);
            TickArithmetic.BoundedDaysAndTickOfDayToTicks(CalendarSystem.Iso.MaxDays, NodaConstants.TicksPerDay - 1);
        }
    }
}
