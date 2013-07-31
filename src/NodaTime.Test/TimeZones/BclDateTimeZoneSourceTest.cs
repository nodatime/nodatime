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

        [Test]
        public void CanLookupLocalTimeZoneById()
        {
            // Note that this test can fail legitimately, as there are valid situations where TimeZoneInfo.Local returns
            // a time zone that is not one of the system time zones.  These are unlikely to be cases that we encounter
            // in practice, though.

            // However, there _are_ cases where we can't fetch the local timezone at all (sigh, Mono), so we'll have to
            // skip this test then.
            TimeZoneInfo local = null;
            try
            {
                local = TimeZoneInfo.Local;
            }
            catch (TimeZoneNotFoundException)
            {
                // See https://bugzilla.xamarin.com/show_bug.cgi?id=11817
                Assert.Ignore("Test requires ability to fetch BCL local time zone");
            }
            if (local == null)
            {
                // https://code.google.com/p/noda-time/issues/detail?id=235#c8
                Assert.Ignore("Test requires ability to fetch BCL local time zone (was null)");
            }

            // Now that we have our BCL local time zone, we should be able to look it up in the source.

            var source = new BclDateTimeZoneSource();
            string id = source.MapTimeZoneId(local);  // in this case, just returns the Id, but required in general.

            // These lines replicate how DateTimeZoneCache implements GetSystemDefault().
            Assert.Contains(id, source.GetIds().ToList(), "BCL local time zone ID should be included in the source ID list");
            var zone = source.ForId(id);

            Assert.IsNotNull(zone);  // though really we only need to test that the call above didn't throw.
        }
    }
}
#endif
