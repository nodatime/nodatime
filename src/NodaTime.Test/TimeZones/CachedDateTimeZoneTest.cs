// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    // Note that this tests CachingZoneIntervalMap as much as CachedDateTimeZone...
    public class CachedDateTimeZoneTest
    {
        private static readonly CachedDateTimeZone timeZone = (CachedDateTimeZone) DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly Instant summer = Instant.FromUtc(2010, 6, 1, 0, 0);

        [Test]
        public void GetZoneIntervalInstant_NotNull()
        {
            var actual = timeZone.GetZoneInterval(summer);
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetZoneIntervalInstant_RepeatedCallsReturnSameObject()
        {
            var actual = timeZone.GetZoneInterval(summer);
            for (int i = 0; i < 100; i++)
            {
                var newPeriod = timeZone.GetZoneInterval(summer);
                Assert.AreSame(actual, newPeriod);
            }
        }

        [Test]
        public void GetZoneIntervalInstant_RepeatedCallsReturnSameObjectWithOthersInterspersed()
        {
            var actual = timeZone.GetZoneInterval(summer);
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch));
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromDays(2000 * 7)));
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromDays(3000 * 7)));
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromDays(4000 * 7)));
            var newPeriod = timeZone.GetZoneInterval(summer);
            Assert.AreSame(actual, newPeriod);
        }

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(timeZone.Uncached().MinOffset, timeZone.MinOffset);
            Assert.AreEqual(timeZone.Uncached().MaxOffset, timeZone.MaxOffset);
        }

        [Test]
        public void ForZone_Fixed()
        {
            var zone = DateTimeZone.ForOffset(Offset.FromHours(1));
            Assert.AreSame(zone, CachedDateTimeZone.ForZone(zone));
        }

        [Test]
        public void ForZone_AlreadyCached()
        {
            Assert.AreSame(timeZone, CachedDateTimeZone.ForZone(timeZone));
        }
    }
}