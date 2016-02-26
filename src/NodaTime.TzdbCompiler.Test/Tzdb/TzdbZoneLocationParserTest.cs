// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using NodaTime.TzdbCompiler.Tzdb;
using NodaTime.TimeZones;

namespace NodaTime.TzdbCompiler.Test.Tzdb
{
    public class TzdbZoneLocationParserTest
    {
        private static readonly Dictionary<string, string> SampleCountryMapping = new Dictionary<string, string>
        {
            { "CA", "Canada" },
            { "GB", "Britain (UK)" }
        };
        
        [Test]
        [TestCase("0\t1", Description = "Too few values")]
        [TestCase("0\t1\t2\t3\t4", Description = "Too many values")]
        public void ParseLocation_InvalidLength(string line)
        {
            Assert.Throws<ArgumentException>(() => TzdbZoneLocationParser.ParseLocation(line, SampleCountryMapping));
        }

        [Test]
        public void ParseLocation_InvalidCountryCode()
        {
            // Valid line, but not with our country code mapping...
            string line = "FK\t-5142-05751\tAtlantic/Stanley";
            Assert.Throws<KeyNotFoundException>(() => TzdbZoneLocationParser.ParseLocation(line, SampleCountryMapping));
        }

        [Test]
        public void ParseLocation_Valid_NoComment()
        {
            string line = "GB\t+4000+03000\tEurope/London";
            var location = TzdbZoneLocationParser.ParseLocation(line, SampleCountryMapping);
            Assert.AreEqual("GB", location.CountryCode);
            Assert.AreEqual("Britain (UK)", location.CountryName);
            Assert.AreEqual(40, location.Latitude);
            Assert.AreEqual(30, location.Longitude);
            Assert.AreEqual("Europe/London", location.ZoneId);
            Assert.AreEqual("", location.Comment);
        }

        // Most of the code is just from ParseLocation; this is a smoke test, really.
        [Test]
        public void ParseEnhancedLocation_Valid()
        {
            var countries = new Dictionary<string, TzdbZone1970Location.Country>
            {
                {  "CA", new TzdbZone1970Location.Country("Canada", "CA") },
                {  "GB", new TzdbZone1970Location.Country("Britain (UK)", "GB") },
                {  "US", new TzdbZone1970Location.Country("United States", "US") },
            };
            string line = "GB,CA\t+4000+03000\tEurope/London";
            var location = TzdbZoneLocationParser.ParseEnhancedLocation(line, countries);
            CollectionAssert.AreEqual(new[] { countries["GB"], countries["CA"] }, location.Countries);
            Assert.AreEqual(40, location.Latitude);
            Assert.AreEqual(30, location.Longitude);
            Assert.AreEqual("Europe/London", location.ZoneId);
            Assert.AreEqual("", location.Comment);
        }

        [Test]
        public void ParseLocation_Valid_WithComment()
        {
            string line = "GB\t+4000+03000\tEurope/London\tSome comment";
            var location = TzdbZoneLocationParser.ParseLocation(line, SampleCountryMapping);
            Assert.AreEqual("GB", location.CountryCode);
            Assert.AreEqual("Britain (UK)", location.CountryName);
            Assert.AreEqual(40, location.Latitude);
            Assert.AreEqual(30, location.Longitude);
            Assert.AreEqual("Europe/London", location.ZoneId);
            Assert.AreEqual("Some comment", location.Comment);
        }

        [Test]
        public void ParseCoordinates_InvalidLength()
        {
            Assert.Throws<ArgumentException>(() => TzdbZoneLocationParser.ParseCoordinates("-77+166"));
        }

        [Test]
        [TestCase("+4512+10034", 45 * 3600 + 12 * 60, 100 * 3600 + 34 * 60)]
        [TestCase("-0502+00134", -5 * 3600 + -2 * 60, 1 * 3600 + 34 * 60)]
        [TestCase("+0000-00001", 0, -1 * 60)]
        [TestCase("+451205+1003402", 45 * 3600 + 12 * 60 + 5, 100 * 3600 + 34 * 60 + 2)]
        [TestCase("-050205+0013402", -5 * 3600 + -2 * 60 - 5, 1 * 3600 + 34 * 60 + 2)]
        [TestCase("+000005-0000102", 5, -1 * 60 - 2)]
        public void ParseCoordinates_Valid(string text, int expectedLatitude, int expectedLongitude)
        {
            int[] actual = TzdbZoneLocationParser.ParseCoordinates(text);
            Assert.AreEqual(expectedLatitude, actual[0]);
            Assert.AreEqual(expectedLongitude, actual[1]);
        }
    }
}
