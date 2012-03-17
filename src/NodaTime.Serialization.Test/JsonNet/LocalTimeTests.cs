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
