// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NUnit.Framework;
using System.Collections.Generic;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for all TZDB time zones.
    /// </summary>
    [TestFixture]
    public class TzdbDateTimeZoneTest
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        private static readonly IEnumerable<DateTimeZone> AllTzdbZones = DateTimeZoneProviders.Tzdb.GetAllZones();
#pragma warning restore 0414

        [Test]
        [TestCaseSource(nameof(AllTzdbZones))]
        public void AllZonesStartAndEndOfTime(DateTimeZone zone)
        {
            var firstInterval = zone.GetZoneInterval(Instant.MinValue);
            Assert.IsFalse(firstInterval.HasStart);
            var lastInterval = zone.GetZoneInterval(Instant.MaxValue);
            Assert.IsFalse(lastInterval.HasEnd);
        }
    }
}
