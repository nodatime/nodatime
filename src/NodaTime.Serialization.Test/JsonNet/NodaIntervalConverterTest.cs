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

using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;

namespace NodaTime.Serialization.Test.JsonNet
{
    [TestFixture]
    public class NodaIntervalConverterTest
    {
        private readonly JsonConverter[] converters = { NodaConverters.IntervalConverter, NodaConverters.InstantConverter };

        [Test]
        public void Serialize()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var json = JsonConvert.SerializeObject(interval, Formatting.None, converters);

            string expectedJson = "{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}";

            var interval = JsonConvert.DeserializeObject<Interval>(json, converters);

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.AreEqual(expectedInterval, interval);
        }

        [Test]
        public void Deserialize_MissingEnd()
        {
            string json = "{\"Start\":\"2012-01-02T03:04:05Z\"}";

            Assert.Throws<InvalidDataException>(() => JsonConvert.DeserializeObject<Interval>(json, converters));
        }

        [Test]
        public void Deserialize_MissingStart()
        {
            string json = "{\"End\":\"2012-01-02T03:04:05Z\"}";

            Assert.Throws<InvalidDataException>(() => JsonConvert.DeserializeObject<Interval>(json, converters));
        }
    }
}
