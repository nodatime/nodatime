// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NUnit.Framework;
using System.IO;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class ExtensionsTest
    {
        [Test]
        public void Serializer_ConfigureForNodaTime_DefaultInterval()
        {
            var configuredSerializer = new JsonSerializer().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitSerializer = new JsonSerializer {
                Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter }
            };
            var interval = new Interval(Instant.FromTicksSinceUnixEpoch(1000L), Instant.FromTicksSinceUnixEpoch(20000L));
            Assert.AreEqual(Serialize(interval, explicitSerializer),
                Serialize(interval, configuredSerializer));
        }

        [Test]
        public void Serializer_ConfigureForNodaTime_WithIsoIntervalConverter()
        {
            var configuredSerializer = new JsonSerializer().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoIntervalConverter();
            var explicitSerializer = new JsonSerializer { Converters = { NodaConverters.IsoIntervalConverter } };
            var interval = new Interval(Instant.FromTicksSinceUnixEpoch(1000L), Instant.FromTicksSinceUnixEpoch(20000L));
            Assert.AreEqual(Serialize(interval, explicitSerializer),
                Serialize(interval, configuredSerializer));
        }

        [Test]
        public void Settings_ConfigureForNodaTime_DefaultInterval()
        {
            var configuredSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            var explicitSettings = new JsonSerializerSettings
            {
                Converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter }
            };
            var interval = new Interval(Instant.FromTicksSinceUnixEpoch(1000L), Instant.FromTicksSinceUnixEpoch(20000L));
            Assert.AreEqual(JsonConvert.SerializeObject(interval, explicitSettings),
                JsonConvert.SerializeObject(interval, configuredSettings));
        }

        [Test]
        public void Settings_ConfigureForNodaTime_WithIsoIntervalConverter()
        {
            var configuredSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb).WithIsoIntervalConverter();
            var explicitSettings = new JsonSerializerSettings { Converters = { NodaConverters.IsoIntervalConverter } };
            var interval = new Interval(Instant.FromTicksSinceUnixEpoch(1000L), Instant.FromTicksSinceUnixEpoch(20000L));
            Assert.AreEqual(JsonConvert.SerializeObject(interval, explicitSettings),
                JsonConvert.SerializeObject(interval, configuredSettings));
        }

        private static string Serialize<T>(T value, JsonSerializer serializer)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }
    }
}
