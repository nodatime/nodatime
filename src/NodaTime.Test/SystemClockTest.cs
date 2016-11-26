// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public class SystemClockTest
    {
        public void InstanceNow()
        {
            long frameworkNowTicks = NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks).ToUnixTimeTicks();
            long nodaTicks = SystemClock.Instance.GetCurrentInstant().ToUnixTimeTicks();
            Assert.Less(Math.Abs(nodaTicks - frameworkNowTicks), Duration.FromSeconds(1).BclCompatibleTicks);
        }

        [Test]
        public void Sanity()
        {
            // Previously all the conversions missed the SystemConversions.DateTimeEpochTicks,
            // so they were self-consistent but not consistent with sanity.
            Instant minimumExpected = Instant.FromUtc(2011, 8, 1, 0, 0);
            Instant maximumExpected = Instant.FromUtc(2020, 1, 1, 0, 0);
            Instant now = SystemClock.Instance.GetCurrentInstant();
            Assert.Less(minimumExpected.ToUnixTimeTicks(), now.ToUnixTimeTicks());
            Assert.Less(now.ToUnixTimeTicks(), maximumExpected.ToUnixTimeTicks());
        }
    }
}
