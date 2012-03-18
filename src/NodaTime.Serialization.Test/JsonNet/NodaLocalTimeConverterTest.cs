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
    public class NodaLocalTimeConverterTest
    {
        private readonly JsonSerializerSettings converter = new JsonSerializerSettings().ConfigureForNodaTime();

        [Test]
        public void Serialize_NonNullableType()
        {
            var localTime = new LocalTime(1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, converter);
            string expectedJson = "\"01:02:03.0040005\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            LocalTime? localTime = new LocalTime(1, 2, 3, 4, 5);
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, converter);
            string expectedJson = "\"01:02:03.0040005\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            LocalTime? localTime = null;
            var json = JsonConvert.SerializeObject(localTime, Formatting.None, converter);
            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"01:02:03.0040005\"";
            var localTime = JsonConvert.DeserializeObject<LocalTime>(json, converter);
            var expectedLocalTime = new LocalTime(1, 2, 3, 4, 5);
            Assert.AreEqual(expectedLocalTime, localTime);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"01:02:03.0040005\"";
            var localTime = JsonConvert.DeserializeObject<LocalTime?>(json, converter);
            LocalTime? expectedLocalTime = new LocalTime(1, 2, 3, 4, 5);
            Assert.AreEqual(expectedLocalTime, localTime);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";
            var localTime = JsonConvert.DeserializeObject<LocalTime?>(json, converter);
            Assert.IsNull(localTime);
        }
    }
}
