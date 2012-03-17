using Newtonsoft.Json;
using NUnit.Framework;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class LocalTimeTests
    {
        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings().ConfigureForNodaTime();

        [Test]
        public void JsonNet_Can_Serialize_LocalTime()
        {
            /* Arrange */
            var localTime = new LocalTime(1, 2, 3, 4, 5);

            /* Act */
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"01:02:03.0040005\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullableLocalTime()
        {
            /* Arrange */
            var localTime = new LocalTime?(new LocalTime(1, 2, 3, 4, 5));

            /* Act */
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"01:02:03.0040005\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullLocalTime()
        {
            /* Arrange */
            var localTime = new LocalTime?();

            /* Act */
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Deserialize_LocalTime()
        {
            /* Arrange */
            const string json = "\"01:02:03.0040005\"";

            /* Act */
            var localTime = JsonConvert.DeserializeObject<LocalTime>(json, jsonSettings);

            /* Assert */
            var expectedLocalTime = new LocalTime(1, 2, 3, 4, 5);
            Assert.AreEqual(expectedLocalTime, localTime);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullableLocalTime()
        {
            /* Arrange */
            const string json = "\"01:02:03.0040005\"";

            /* Act */
            var localTime = JsonConvert.DeserializeObject<LocalTime?>(json, jsonSettings);

            /* Assert */
            var expectedLocalTime = new LocalTime?(new LocalTime(1, 2, 3, 4, 5));
            Assert.AreEqual(expectedLocalTime, localTime);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullLocalTime()
        {
            /* Arrange */
            const string json = "null";

            /* Act */
            var localTime = JsonConvert.DeserializeObject<LocalTime?>(json, jsonSettings);

            /* Assert */
            var expectedLocalTime = new LocalTime?();
            Assert.AreEqual(expectedLocalTime, localTime);
        }
    }
}
