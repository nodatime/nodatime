// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class TzdbZoneLocationTest
    {
        [Test]
        public void Valid()
        {
            var location = new TzdbZoneLocation(
                60 * 3600 + 15 * 60 + 5,
                100 * 3600 + 30 * 60 + 10,
                "Country name",
                "CO",
                "Etc/MadeUpZone",
                "Comment");
            Assert.AreEqual(60.25 + 5.0 / 3600, location.Latitude, 0.000001);
            Assert.AreEqual(100.5 + 10.0 / 3600, location.Longitude, 0.000001);
            Assert.AreEqual("Country name", location.CountryName);
            Assert.AreEqual("CO", location.CountryCode);
            Assert.AreEqual("Etc/MadeUpZone", location.ZoneId);
            Assert.AreEqual("Comment", location.Comment);
        }

        [Test]
        public void LatitudeRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZoneLocation(90 * 3600 + 1, 0, "Name", "CO", "Zone", ""));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZoneLocation(-90 * 3600 - 1, 0, "Name", "CO", "Zone", ""));
            // We'll assume these are built correctly: we're just checking the constructor doesn't throw.
            new TzdbZoneLocation(90 * 3600, 0, "Name", "CO", "Zone", "");
            new TzdbZoneLocation(-90 * 3600, 0, "Name", "CO", "Zone", "");
        }

        [Test]
        public void LongitudeRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZoneLocation(0, 180 * 3600 + 1, "Name", "CO", "Zone", ""));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZoneLocation(0, -180 * 3600 - 1, "Name", "CO", "Zone", ""));
            // We'll assume these are built correctly: we're just checking the constructor doesn't throw.
            new TzdbZoneLocation(0, 180 * 3600, "Name", "CO", "Zone", "");
            new TzdbZoneLocation(0, -180 * 3600, "Name", "CO", "Zone", "");
        }

        [Test]
        public void NullStrings()
        {
            Assert.Throws<ArgumentNullException>(() => new TzdbZoneLocation(0, 0, null, "CO", "Zone", ""));
            Assert.Throws<ArgumentNullException>(() => new TzdbZoneLocation(0, 0, "Name", null, "Zone", ""));
            Assert.Throws<ArgumentNullException>(() => new TzdbZoneLocation(0, 0, "Name", "CO", null, ""));
            Assert.Throws<ArgumentNullException>(() => new TzdbZoneLocation(0, 0, "Name", "CO", "Zone", null));
        }
    }
}
