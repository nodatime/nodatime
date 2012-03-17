using Newtonsoft.Json;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class LocalDateIsoTests
    {
        // TODO: we need tests for other calendars.

        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings().ConfigureForNodaTime();

        [Test]
        public void JsonNet_Can_Serialize_LocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            var localDate = new LocalDate(Era.Common, 2012, 1, 2, CalendarSystem.Iso);

            /* Act */
            var json = JsonConvert.SerializeObject(localDate, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"2012-01-02\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullableLocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            var localDate = new LocalDate?(new LocalDate(Era.Common, 2012, 1, 2, CalendarSystem.Iso));

            /* Act */
            var json = JsonConvert.SerializeObject(localDate, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"2012-01-02\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullLocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            var localDate = new LocalDate?();

            /* Act */
            var json = JsonConvert.SerializeObject(localDate, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Deserialize_LocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "\"2012-01-02\"";

            /* Act */
            var localDate = JsonConvert.DeserializeObject<LocalDate>(json, jsonSettings);

            /* Assert */
            var expectedLocalDate = new LocalDate(Era.Common, 2012, 1, 2, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDate, localDate);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullableLocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "\"2012-01-02\"";

            /* Act */
            var localDate = JsonConvert.DeserializeObject<LocalDate?>(json, jsonSettings);

            /* Assert */
            var expectedLocalDate = new LocalDate?(new LocalDate(Era.Common, 2012, 1, 2, CalendarSystem.Iso));
            Assert.AreEqual(expectedLocalDate, localDate);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullLocalDate_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "null";

            /* Act */
            var localDate = JsonConvert.DeserializeObject<LocalDate?>(json, jsonSettings);

            /* Assert */
            var expectedLocalDate = new LocalDate?();
            Assert.AreEqual(expectedLocalDate, localDate);
        }
    }
}
