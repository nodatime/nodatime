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
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaDateTimeZoneConverterTest
    {
        private readonly JsonConverter converter = NodaConverters.DateTimeZoneConverter;

        [Test]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZone.ForId("America/Los_Angeles");
            var json = JsonConvert.SerializeObject(dateTimeZone, Formatting.None, converter);
            string expectedJson = "\"America/Los_Angeles\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"America/Los_Angeles\"";
            var dateTimeZone = JsonConvert.DeserializeObject<DateTimeZone>(json, converter);
            var expectedDateTimeZone = DateTimeZone.ForId("America/Los_Angeles");
            Assert.AreEqual(expectedDateTimeZone, dateTimeZone);
        }

        [Test]
        public void Deserialize_TimeZoneNotFound()
        {
            string json = "\"America/DOES_NOT_EXIST\"";
            Assert.Throws<TimeZoneNotFoundException>(() => JsonConvert.DeserializeObject<DateTimeZone>(json, converter));
        }
    }
}
