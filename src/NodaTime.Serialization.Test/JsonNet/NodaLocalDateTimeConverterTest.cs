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
    public class NodaLocalDateTimeConverterTest
    {
        // TODO: we need tests for other calendars.

        private readonly NodaLocalDateTimeConverter converter = new NodaLocalDateTimeConverter();

        [Test]
        public void Serialize_NonNullableType()
        {
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);

            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, converter);

            string expectedJson = "\"2012-01-02T03:04:05.0060007\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NonNullValue()
        {
            LocalDateTime? localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);

            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, converter);

            string expectedJson = "\"2012-01-02T03:04:05.0060007\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Serialize_NullableType_NullValue()
        {
            LocalDateTime? localDateTime = null;

            var json = JsonConvert.SerializeObject(localDateTime, Formatting.None, converter);

            string expectedJson = "null";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize_ToNonNullableType()
        {
            string json = "\"2012-01-02T03:04:05.0060007\"";

            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime>(json, converter);

            var expectedLocalDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void Deserialize_ToNullableType_NonNullValue()
        {
            string json = "\"2012-01-02T03:04:05.0060007\"";

            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime?>(json, converter);

            LocalDateTime? expectedLocalDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, 7, CalendarSystem.Iso);
            Assert.AreEqual(expectedLocalDateTime, localDateTime);
        }

        [Test]
        public void Deserialize_ToNullableType_NullValue()
        {
            string json = "null";

            var localDateTime = JsonConvert.DeserializeObject<LocalDateTime?>(json, converter);

            Assert.IsNull(localDateTime);
        }

        [Test]
        public void Serialize_EquivalentToIsoDateTimeConverter()
        {
            var dateTime = new DateTime(2012, 1, 2, 3, 4, 5, 6, DateTimeKind.Unspecified);
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, CalendarSystem.Iso);

            var jsonDateTime = JsonConvert.SerializeObject(dateTime, new IsoDateTimeConverter());
            var jsonLocalDateTime = JsonConvert.SerializeObject(localDateTime, Formatting.None, converter);

            Assert.AreEqual(jsonDateTime, jsonLocalDateTime);
        }
    }
}
