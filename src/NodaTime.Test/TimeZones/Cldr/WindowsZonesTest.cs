// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones.Cldr;
using NodaTime.TimeZones.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace NodaTime.Test.TimeZones.Cldr
{
    public class WindowsZonesTest
    {
        private static readonly MapZone MapZone1 = new MapZone("windowsId1", "territory1", new[] { "id1.1", "id1.2", "id1.3" });
        private static readonly MapZone MapZone2a = new MapZone("windowsId2", MapZone.PrimaryTerritory, new[] { "primaryId2" });
        private static readonly MapZone MapZone2b = new MapZone("windowsId2", "territory2", new[] { "primaryId2", "id2.1", "id2.2" });
        private static readonly MapZone MapZone3 = new MapZone("windowsId3", MapZone.PrimaryTerritory, new[] { "primaryId3" });

        private static readonly WindowsZones SampleZones = new WindowsZones("version", "tzdbVersion", "windowsVersion", new[] { MapZone1, MapZone2a, MapZone2b, MapZone3 });

        [Test]
        public void Properties()
        {
            Assert.AreEqual("version", SampleZones.Version);
            Assert.AreEqual("tzdbVersion", SampleZones.TzdbVersion);
            Assert.AreEqual("windowsVersion", SampleZones.WindowsVersion);
            Assert.AreEqual("primaryId2", SampleZones.PrimaryMapping["windowsId2"]);
            Assert.AreEqual("primaryId3", SampleZones.PrimaryMapping["windowsId3"]);
            Assert.AreEqual(new[] { MapZone1, MapZone2a, MapZone2b, MapZone3 }, SampleZones.MapZones);
        }

        [Test]
        public void ReadWrite()
        {
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            SampleZones.Write(writer);
            stream.Position = 0;

            var reader = new DateTimeZoneReader(stream, null);
            var zones2 = WindowsZones.Read(reader);

            Assert.AreEqual("version", zones2.Version);
            Assert.AreEqual("tzdbVersion", zones2.TzdbVersion);
            Assert.AreEqual("windowsVersion", zones2.WindowsVersion);
            Assert.AreEqual("primaryId2", zones2.PrimaryMapping["windowsId2"]);
            Assert.AreEqual("primaryId3", zones2.PrimaryMapping["windowsId3"]);
            Assert.AreEqual(new[] { MapZone1, MapZone2a, MapZone2b, MapZone3 }, zones2.MapZones);
        }

        [Test]
        [TestCase("windowsId2", "primaryId2")]
        [TestCase("windowsId3", "primaryId3")]
        public void MapWindowsToTzdb_Valid(string windowsId, string expectedTzdbId)
        {
            Assert.AreEqual(expectedTzdbId, SampleZones.MapWindowsToTzdb(windowsId));
            Assert.AreEqual(expectedTzdbId, SampleZones.MapWindowsToTzdbOrNull(windowsId));
        }

        [Test]
        [TestCase("windowsId1")]
        [TestCase("windowsIdUnknown")]
        public void MapWindowsToTzdb_Invalid(string windowsId)
        {
            Assert.Throws<KeyNotFoundException>(() => SampleZones.MapWindowsToTzdb(windowsId));
            Assert.Null(SampleZones.MapWindowsToTzdbOrNull(windowsId)); ;
        }

        [Test]
        [TestCase("id1.1", "windowsId1")]
        [TestCase("id1.2", "windowsId1")]
        [TestCase("id1.3", "windowsId1")]
        [TestCase("primaryId2", "windowsId2")]
        [TestCase("id2.1", "windowsId2")]
        [TestCase("id2.2", "windowsId2")]
        [TestCase("primaryId3", "windowsId3")]
        public void MapTzdbToWindows_Valid(string tzdbId, string expectedWindowsId)
        {
            Assert.AreEqual(expectedWindowsId, SampleZones.MapTzdbToWindows(tzdbId));
            Assert.AreEqual(expectedWindowsId, SampleZones.MapTzdbToWindowsOrNull(tzdbId));
        }

        [Test]
        public void MapTzdbToWindows_Invalid()
        {
            Assert.Throws<KeyNotFoundException>(() => SampleZones.MapTzdbToWindows("unknown"));
            Assert.Null(SampleZones.MapTzdbToWindowsOrNull("unknown"));
        }

        [Test]
        public void MapMethods_NullInput()
        {
            Assert.Throws<ArgumentNullException>(() => SampleZones.MapWindowsToTzdb(null!));
            Assert.Throws<ArgumentNullException>(() => SampleZones.MapWindowsToTzdbOrNull(null!));
            Assert.Throws<ArgumentNullException>(() => SampleZones.MapTzdbToWindows(null!));
            Assert.Throws<ArgumentNullException>(() => SampleZones.MapTzdbToWindowsOrNull(null!));
        }
    }
}
