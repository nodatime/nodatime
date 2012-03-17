#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class LocalDateTimeIsoTests
    {
        // TODO: we need tests for other calendars.

        private readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings().ConfigureForNodaTime();

        [Test]
        public void JsonNet_Can_Serialize_LocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);

            /* Act */
            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"2012-01-02T03:04:05.0060007\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullableLocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            var localDateTime = new LocalDateTime?(new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso));

            /* Act */
            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "\"2012-01-02T03:04:05.0060007\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Serialize_NullLocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            var localDateTime = new LocalDateTime?();

            /* Act */
            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, jsonSettings);

            /* Assert */
            const string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void JsonNet_Can_Deserialize_LocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "\"2012-01-02T03:04:05.0060007\"";

            /* Act */
            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime>(json, jsonSettings);

            /* Assert */
            var expectedLocalDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullableLocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "\"2012-01-02T03:04:05.0060007\"";

            /* Act */
            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime?>(json, jsonSettings);

            /* Assert */
            var expectedLocalDateTime = new LocalDateTime?(new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso));
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void JsonNet_Can_Deserialize_NullLocalDateTime_UsingIsoCalendar()
        {
            /* Arrange */
            const string json = "null";

            /* Act */
            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime?>(json, jsonSettings);

            /* Assert */
            var expectedLocalDateTime = new LocalDateTime?();
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void JsonNet_Serializes_LocalDateTime_Like_DateTime_UsingIsoCalendar()
        {
            /* Arrange */
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, 6, DateTimeKind.Unspecified);
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, CalendarSystem.Iso);

            /* Act */
            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonLocalDateTime = JsonConvert.SerializeObject(localDateTime, Formatting.None, jsonSettings);

            /* Assert */
            Assert.AreEqual(jsonDateTime, jsonLocalDateTime);
        }
    }
}
