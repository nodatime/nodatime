// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaInstantConverterTest
    {
        private readonly JsonConverter converter = NodaConverters.InstantConverter;

        [Test]
        public void Serialize_NonNullableType()
        {
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(instant, Formatting.None, converter);
            string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            Instant? instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(instant, Formatting.None, converter);
            string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            Instant? instant = null;
            var json = JsonConvert.SerializeObject(instant, Formatting.None, converter);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonConvert.DeserializeObject<Instant>(json, converter);
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"2012-01-02T03:04:05Z\"";
            var instant = JsonConvert.DeserializeObject<Instant?>(json, converter);
            Instant? expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var instant = JsonConvert.DeserializeObject<Instant?>(json, converter);
            Assert.IsNull(instant);
        }
        
        [Test]
        public void Serialize_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);
            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonInstant = JsonConvert.SerializeObject(instant, Formatting.None, converter);
            Assert.AreEqual(jsonDateTime, jsonInstant);
        }
    }
}
