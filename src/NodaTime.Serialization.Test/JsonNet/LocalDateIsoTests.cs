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

using NUnit.Framework;
using Newtonsoft.Json;
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
        public void Serialize_NonNullableType()
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
        public void Serialize_NullableType_NonNullValue()
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
        public void Serialize_NullableType_NullValue()
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
        public void Deserialize_ToNonNullableType()
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
        public void Deserialize_ToNullableType_NonNullValue()
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
        public void Deserialize_ToNullableType_NullValue()
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
