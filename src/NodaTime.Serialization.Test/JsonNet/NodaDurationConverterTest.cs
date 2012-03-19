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
    public class NodaDurationConverterTest
    {
        private readonly JsonConverter converter = NodaConverters.DurationConverter;

        [Test]
        public void Serialize()
        {
            var duration = Duration.FromHours(48);
            var json = JsonConvert.SerializeObject(duration, Formatting.None, converter);
            string expectedJson = "\"48:00:00\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void Deserialize()
        {
            string json = "\"48:00:00\"";
            var duration = JsonConvert.DeserializeObject<Duration>(json, converter);
            var expectedDuration = Duration.FromHours(48);
            Assert.AreEqual(expectedDuration, duration);
        }
    }
}
