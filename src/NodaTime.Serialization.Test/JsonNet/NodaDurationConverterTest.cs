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
        private readonly JsonConverter converter = NodaConverters.DurationConverter;

        [Test]
        public void Serialize()
        {
            var duration = Duration.FromHours(48);
            var json = JsonConvert.SerializeObject(duration, Formatting.None, converter);
            string expectedJson = "\"48:00:00\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"48:00:00\"";
            var duration = JsonConvert.DeserializeObject<Duration>(json, converter);
            var expectedDuration = Duration.FromHours(48);
            Assert.AreEqual(expectedDuration, duration);
        }
    }
}
