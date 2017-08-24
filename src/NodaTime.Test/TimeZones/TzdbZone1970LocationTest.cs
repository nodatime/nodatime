// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using NodaTime.TimeZones;
using NodaTime.TimeZones.IO;
using NUnit.Framework;

using NodaTime.Utility;
using static NodaTime.TimeZones.TzdbZone1970Location;

namespace NodaTime.Test.TimeZones
{
    public class TzdbZone1970LocationTest
    {
        private static readonly Country SampleCountry =
            new Country("Country name", "CO");

        [Test]
        public void Valid()
        {
            var location = new TzdbZone1970Location(
                60 * 3600 + 15 * 60 + 5,
                100 * 3600 + 30 * 60 + 10,
                new[] { SampleCountry },
                "Etc/MadeUpZone",
                "Comment");
            Assert.AreEqual(60.25 + 5.0 / 3600, location.Latitude, 0.000001);
            Assert.AreEqual(100.5 + 10.0 / 3600, location.Longitude, 0.000001);
            Assert.AreEqual("Country name", location.Countries[0].Name);
            Assert.AreEqual("CO", location.Countries[0].Code);
            Assert.AreEqual("Etc/MadeUpZone", location.ZoneId);
            Assert.AreEqual("Comment", location.Comment);
        }

        [Test]
        public void CountryToString() => Assert.AreEqual("CO (Country name)", SampleCountry.ToString());

        [Test]
        public void Serialization()
        {
            var location = new TzdbZone1970Location(
                60 * 3600 + 15 * 60 + 5,
                100 * 3600 + 30 * 60 + 10,
                new[] { SampleCountry },
                "Etc/MadeUpZone",
                "Comment");
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            location.Write(writer);
            stream.Position = 0;
            var reloaded = TzdbZone1970Location.Read(new DateTimeZoneReader(stream, null));
            Assert.AreEqual(location.Latitude, reloaded.Latitude);
            Assert.AreEqual(location.Longitude, reloaded.Longitude);
            CollectionAssert.AreEqual(location.Countries, reloaded.Countries);
            Assert.AreEqual(location.ZoneId, reloaded.ZoneId);
            Assert.AreEqual(location.Comment, reloaded.Comment);
        }

        [Test]
        public void ReadInvalid()
        {
            var stream = new MemoryStream();
            var writer = new DateTimeZoneWriter(stream, null);
            // Valid latitude/longitude
            writer.WriteSignedCount(0);
            writer.WriteSignedCount(0);
            // But no countries
            writer.WriteCount(0);
            writer.WriteString("Europe/Somewhere");
            writer.WriteString("");
            stream.Position = 0;
            var reader = new DateTimeZoneReader(stream, null);
            Assert.Throws<InvalidNodaDataException>(() => TzdbZone1970Location.Read(reader));
        }

        [Test]
        public void LatitudeRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZone1970Location(90 * 3600 + 1, 0, new[] { SampleCountry }, "Zone", ""));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZone1970Location(-90 * 3600 - 1, 0, new[] { SampleCountry }, "Zone", ""));
            // We'll assume these are built correctly: we're just checking the constructor doesn't throw.
            new TzdbZone1970Location(90 * 3600, 0, new[] { SampleCountry }, "Zone", "");
            new TzdbZone1970Location(-90 * 3600, 0, new[] { SampleCountry }, "Zone", "");
        }

        [Test]
        public void LongitudeRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZone1970Location(0, 180 * 3600 + 1, new[] { SampleCountry }, "Zone", ""));
            Assert.Throws<ArgumentOutOfRangeException>(() => new TzdbZone1970Location(0, -180 * 3600 - 1, new[] { SampleCountry }, "Zone", ""));
            // We'll assume these are built correctly: we're just checking the constructor doesn't throw.
            new TzdbZone1970Location(0, 180 * 3600, new[] { SampleCountry }, "Zone", "");
            new TzdbZone1970Location(0, -180 * 3600, new[] { SampleCountry }, "Zone", "");
        }

        [Test]
        public void Constructor_InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TzdbZone1970Location(0, 0, null, "Zone", ""));
            Assert.Throws<ArgumentException>(() => new TzdbZone1970Location(0, 0, new[] { SampleCountry, null }, "Zone", ""));
            Assert.Throws<ArgumentNullException>(() => new TzdbZone1970Location(0, 0, new[] { SampleCountry }, "Zone", null));
            Assert.Throws<ArgumentNullException>(() => new TzdbZone1970Location(0, 0, new[] { SampleCountry }, null, ""));
            Assert.Throws<ArgumentException>(() => new TzdbZone1970Location(0, 0, new Country[] { }, null, ""));
        }

        [Test]
        public void CountryConstructor_InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new Country(code: null, name: "Name"));
            Assert.Throws<ArgumentNullException>(() => new Country(code: "CO", name: null));
            Assert.Throws<ArgumentException>(() => new Country(code: "CO", name: ""));
            Assert.Throws<ArgumentException>(() => new Country(code: "Long code", name: "Name"));
            Assert.Throws<ArgumentException>(() => new Country(code: "S", name: "Name"));
        }

        [Test]
        public void CountryEquality()
        {
            TestHelper.TestEqualsClass(new Country("France", "FR"), new Country("France", "FR"), new Country("Germany", "DE"));
        }
    }
}
