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
    /// <summary>
    /// Tests for the converters exposed in NodaConverters.
    /// </summary>
    [TestFixture]
    public class NodaConvertersTest
    {
        [Test]
        public void OffsetConverter_Serialize()
        {
            var offset = Offset.Create(5, 30, 0, 0);

            var json = JsonConvert.SerializeObject(offset, Formatting.None, NodaConverters.OffsetConverter);

            string expectedJson = "\"+05:30\"";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void OffsetConverter_Deserialize()
        {
            string json = "\"+05:30\"";

            var offset = JsonConvert.DeserializeObject<Offset>(json, NodaConverters.OffsetConverter);

            var expectedOffset = Offset.Create(5, 30, 0, 0);
            Assert.AreEqual(expectedOffset, offset);
        }
    }
}
