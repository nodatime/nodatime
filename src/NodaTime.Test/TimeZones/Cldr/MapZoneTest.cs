// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Test.TimeZones.Cldr
{
    public class MapZoneTest
    {
        [Test]
        public void Equality()
        {
            var zone1 = new MapZone("windowsId", "territory", new[] { "id1", "id2", "id3" });
            var zone2 = new MapZone("windowsId", "territory", new[] { "id1", "id2", "id3" });
            var zone3 = new MapZone("otherWindowsId", "territory", new[] { "id1", "id2", "id3" });
            var zone4 = new MapZone("windowsId", "otherTerritory", new[] { "id1", "id2", "id3" });
            var zone5 = new MapZone("windowsId", "territory", new[] { "id1", "id2" });
            var zone6 = new MapZone("windowsId", "territory", new[] { "id1", "id2", "id4" });
            TestHelper.TestEqualsClass(zone1, zone2, zone3);
            TestHelper.TestEqualsClass(zone1, zone2, zone4);
            TestHelper.TestEqualsClass(zone1, zone2, zone5);
            TestHelper.TestEqualsClass(zone1, zone2, zone6);
        }

        [Test]
        public new void ToString()
        {
            var zone = new MapZone("windowsId", "territory", new[] { "id1", "id2", "id3" });
            var expected = "Windows ID: windowsId; Territory: territory; TzdbIds: id1 id2 id3";
            Assert.AreEqual(expected, zone.ToString());
        }

        [Test]
        public void ReadWrite()
        {
            var zone = new MapZone("windowsId", "territory", new[] { "id1", "id2", "id3" });
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            zone.Write(writer);
            stream.Position = 0;

            var reader = new DateTimeZoneReader(stream, null);
            var zone2 = MapZone.Read(reader);
            Assert.AreEqual(zone, zone2);
        }
    }
}
