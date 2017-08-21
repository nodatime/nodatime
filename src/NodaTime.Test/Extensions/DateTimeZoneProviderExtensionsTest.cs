// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NodaTime.Testing.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.Extensions
{
    public class DayOfWeekExtensionsTest
    {
        [Test]
        public void GetAllZones()
        {
            // Note: in ID order.
            var zone1 = DateTimeZoneProviders.Tzdb["America/New_York"];
            var zone2 = DateTimeZoneProviders.Tzdb["Europe/London"];
            var zone3 = DateTimeZoneProviders.Tzdb["Europe/Paris"];

            var provider = new FakeDateTimeZoneSource.Builder { zone1, zone2, zone3 }.Build().ToProvider();
            CollectionAssert.AreEqual(new[] { zone1, zone2, zone3 }, provider.GetAllZones());
        }
    }
}
