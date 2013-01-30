// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

#if !PCL

using System;
using System.Linq;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class BclDateTimeZoneSourceTest
    {
        [Test]
        public void AllZonesMapToTheirId()
        {
            BclDateTimeZoneSource source = new BclDateTimeZoneSource();
            foreach (var zone in TimeZoneInfo.GetSystemTimeZones())
            {
                Assert.AreEqual(zone.Id, source.MapTimeZoneId(zone));
            }
        }

        [Test]
        public void UtcDoesNotEqualBuiltIn()
        {
            var zone = new BclDateTimeZoneSource().ForId("UTC");
            Assert.AreNotEqual(DateTimeZone.Utc, zone);
        }

        [Test]
        public void FixedOffsetDoesNotEqualBuiltIn()
        {
            // Only a few fixed zones are advertised by Windows. We happen to know this one
            // is wherever we run tests :)
            // Unfortunately, it doesn't always exist on Mono (at least not on the Raspberry Pi...)
            string id = "UTC-02";
            var source = new BclDateTimeZoneSource();
            if (!source.GetIds().Contains(id))
            {
                Assert.Ignore("Test assumes existence of BCL zone with ID: " + id);
            }
            var zone = source.ForId(id);
            Assert.AreNotEqual(DateTimeZone.ForOffset(Offset.FromHours(-2)), zone);
            Assert.AreEqual(id, zone.Id);
            Assert.AreEqual(Offset.FromHours(-2), zone.GetZoneInterval(NodaConstants.UnixEpoch).WallOffset);
        }
    }
}
#endif
