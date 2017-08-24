// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Test.TimeZones.Cldr
{
    public class WindowsZonesTest
    {
        private static readonly MapZone MapZone1 = new MapZone("windowsId1", "territory1", new[] { "id1.1", "id1.2", "id1.3" });
        private static readonly MapZone MapZone2 = new MapZone("windowsId2", MapZone.PrimaryTerritory, new[] { "primaryId2" });
        private static readonly MapZone MapZone3 = new MapZone("windowsId3", MapZone.PrimaryTerritory, new[] { "primaryId3" });

        [Test]
        public void Properties()
        {
            var zones = new WindowsZones("version", "tzdbVersion", "windowsVersion", new[] { MapZone1, MapZone2, MapZone3 });
            Assert.AreEqual("version", zones.Version);
            Assert.AreEqual("tzdbVersion", zones.TzdbVersion);
            Assert.AreEqual("windowsVersion", zones.WindowsVersion);
            Assert.AreEqual("primaryId2", zones.PrimaryMapping["windowsId2"]);
            Assert.AreEqual("primaryId3", zones.PrimaryMapping["windowsId3"]);
            Assert.AreEqual(new[] { MapZone1, MapZone2, MapZone3 }, zones.MapZones);
        }

        [Test]
        public void ReadWrite()
        {
            var zones = new WindowsZones("version", "tzdbVersion", "windowsVersion", new[] { MapZone1, MapZone2, MapZone3 });

            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            zones.Write(writer);
            stream.Position = 0;

            var reader = new DateTimeZoneReader(stream, null);
            var zones2 = WindowsZones.Read(reader);

            Assert.AreEqual("version", zones2.Version);
            Assert.AreEqual("tzdbVersion", zones2.TzdbVersion);
            Assert.AreEqual("windowsVersion", zones2.WindowsVersion);
            Assert.AreEqual("primaryId2", zones2.PrimaryMapping["windowsId2"]);
            Assert.AreEqual("primaryId3", zones2.PrimaryMapping["windowsId3"]);
            Assert.AreEqual(new[] { MapZone1, MapZone2, MapZone3 }, zones2.MapZones);
        }
    }
}
