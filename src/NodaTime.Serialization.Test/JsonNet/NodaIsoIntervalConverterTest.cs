// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Serialization.Test.JsonNet
{
    /// <summary>
    /// The same tests as NodaIntervalConverterTest, but using the ISO-based interval converter.
    /// </summary>
    [TestFixture]
    public class NodaIsoIntervalConverterTest
    {
        private readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = { NodaConverters.IsoIntervalConverter },
            DateParseHandling = DateParseHandling.None
        };

        [Test]
        public void Serialize()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromTicks(1234567);
            var interval = new Interval(startInstant, endInstant);

            var json = JsonConvert.SerializeObject(interval, Formatting.None, settings);

            string expectedJson = "\"2012-01-02T03:04:05.67Z/2013-06-07T08:09:10.1234567Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            // Comma is deliberate, to show that we can parse a comma decimal separator too.
            string json = "\"2012-01-02T03:04:05.670Z/2013-06-07T08:09:10,1234567Z\"";

            var interval = JsonConvert.DeserializeObject<Interval>(json, settings);

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5) + Duration.FromMilliseconds(670);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10) + Duration.FromTicks(1234567);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_MissingSlash()
        {
            string json = "\"2012-01-02T03:04:05Z2013-06-07T08:09:10Z\"";

            Assert.Throws<InvalidNodaDataException>(() => JsonConvert.DeserializeObject<Interval>(json, settings));
        }

        [Test]
        public void Serialize_InObject()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var testObject = new TestObject { Interval = interval };

            var json = JsonConvert.SerializeObject(testObject, Formatting.None, settings);

            string expectedJson = "{\"Interval\":\"2012-01-02T03:04:05Z/2013-06-07T08:09:10Z\"}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_InObject()
        {
            string json = "{\"Interval\":\"2012-01-02T03:04:05Z/2013-06-07T08:09:10Z\"}";

            var testObject = JsonConvert.DeserializeObject<TestObject>(json, settings);

            var interval = testObject.Interval;

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, interval);
        }

        public class TestObject
        {
            public Interval Interval { get; set; }
        }
    }
}
