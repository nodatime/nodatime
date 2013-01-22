using System;
using NUnit.Framework;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NodaTime.TimeZones;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaDateTimeZoneConverterTest
    {
        private readonly JsonConverter converter = new NodaDateTimeZoneConverter(DateTimeZoneProviders.Tzdb);

        [Test]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = JsonConvert.SerializeObject(dateTimeZone, Formatting.None, converter);
            string expectedJson = "\"America/Los_Angeles\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"America/Los_Angeles\"";
            var dateTimeZone = JsonConvert.DeserializeObject<DateTimeZone>(json, converter);
            var expectedDateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            Assert.AreEqual(expectedDateTimeZone, dateTimeZone);
        }

        [Test]
        public void Deserialize_TimeZoneNotFound()
        {
            string json = "\"America/DOES_NOT_EXIST\"";
            Assert.Throws<DateTimeZoneNotFoundException>(() => JsonConvert.DeserializeObject<DateTimeZone>(json, converter));
        }
    }
}
