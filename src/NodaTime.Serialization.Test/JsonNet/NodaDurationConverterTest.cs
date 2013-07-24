// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaDurationConverterTest
    {
        private static readonly JsonConverter Converter = NodaConverters.DurationConverter;

        private static void AssertRoundTrip(Duration value, string expectedJson)
        {
            var json = JsonConvert.SerializeObject(value, Formatting.None, Converter);
            Assert.AreEqual(expectedJson, json);
            var parsed = JsonConvert.DeserializeObject<Duration>(json, Converter);
            Assert.AreEqual(value, parsed);
        }

        [Test]
        public void WholeSeconds()
        {
            AssertRoundTrip(Duration.FromHours(48), "\"48:00:00\"");
        }

        [Test]
        public void FractionalSeconds()
        {
            AssertRoundTrip(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234567), "\"48:00:03.1234567\"");
            AssertRoundTrip(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1230000), "\"48:00:03.123\"");
            AssertRoundTrip(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234000), "\"48:00:03.1234000\"");
            AssertRoundTrip(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(12345), "\"48:00:03.0012345\"");
        }

        [Test]
        public void ParsePartialFractionalSeconds()
        {
            var parsed = JsonConvert.DeserializeObject<Duration>("\"25:10:00.1234\"", Converter);
            Assert.AreEqual(Duration.FromHours(25) + Duration.FromMinutes(10) + Duration.FromTicks(1234000), parsed);
        }
    }
}
