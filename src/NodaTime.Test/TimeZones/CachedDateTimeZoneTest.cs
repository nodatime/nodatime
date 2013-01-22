// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    // Note that this tests CachingZoneIntervalMap as much as CachedDateTimeZone...
    [TestFixture]
    public class CachedDateTimeZoneTest
    {
        #region Setup/Teardown
        [SetUp]
        public void Setup()
        {
            timeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"] as CachedDateTimeZone;
            if (timeZone == null)
            {
                Assert.Fail("The America/Los_Angeles time zone does not contain a CachedDateTimeZone.");
            }
            summer = Instant.FromUtc(2010, 6, 1, 0, 0);
        }
        #endregion

        private CachedDateTimeZone timeZone;
        private Instant summer;

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
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromStandardWeeks(2000)));
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromStandardWeeks(3000)));
            Assert.IsNotNull(timeZone.GetZoneInterval(NodaConstants.UnixEpoch + Duration.FromStandardWeeks(4000)));
            var newPeriod = timeZone.GetZoneInterval(summer);
            Assert.AreSame(actual, newPeriod);
        }

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(timeZone.Uncached().MinOffset, timeZone.MinOffset);
            Assert.AreEqual(timeZone.Uncached().MaxOffset, timeZone.MaxOffset);
        }
    }
}