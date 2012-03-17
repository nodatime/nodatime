using System;
using Newtonsoft.Json;
using NUnit.Framework;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class InstantTests
    {
        [Test]
        public void JsonNet_Can_Serialize_Instant()
        {
            /* Arrange */
            var instant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);

            /* Act */
            var json = JsonConvert.SerializeObject(instant, new NodaInstantConverter());

            /* Assert */
            const string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullableInstant()
        {
            /* Arrange */
            var instant = new Instant?(Instant.FromUtc(2012, 1, 2, 3, 4, 5));

            /* Act */
            var json = JsonConvert.SerializeObject(instant, new NodaInstantConverter());

            /* Assert */
            const string expectedJson = "\"2012-01-02T03:04:05Z\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullInstant()
        {
            /* Arrange */
            var instant = new Instant?();

            /* Act */
            var json = JsonConvert.SerializeObject(instant, new NodaInstantConverter());

            /* Assert */
            const string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Deserialize_Instant()
        {
            /* Arrange */
            const string json = "\"2012-01-02T03:04:05Z\"";

            /* Act */
            var instant = JsonConvert.DeserializeObject<Instant>(json, new NodaInstantConverter());

            /* Assert */
            var expectedInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullableInstant()
        {
            /* Arrange */
            const string json = "\"2012-01-02T03:04:05Z\"";

            /* Act */
            var instant = JsonConvert.DeserializeObject<Instant?>(json, new NodaInstantConverter());

            /* Assert */
            var expectedInstant = new Instant?(Instant.FromUtc(2012, 1, 2, 3, 4, 5));
            Assert.AreEqual(expectedInstant, instant);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullInstant()
        {
            /* Arrange */
            const string json = "null";

            /* Act */
            var instant = JsonConvert.DeserializeObject<Instant?>(json, new NodaInstantConverter());

            /* Assert */
            var expectedInstant = new Instant?();
            Assert.AreEqual(expectedInstant, instant);
        }
        
        [Test]
        public void JsonNet_Serializes_Instant_Like_UTCDateTime()
        {
            /* Arrange */
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var instant = Instant.FromDateTimeUtc(dateTime);

            /* Act */
            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonInstant = JsonConvert.SerializeObject(instant, new NodaInstantConverter());

            /* Assert */
            Assert.AreEqual(jsonDateTime, jsonInstant);
        }
    }
}
