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

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class NullDateTimeZoneTest
    {
        private static readonly NullDateTimeZone TimeZone = NullDateTimeZone.Instance;

        [Test]
        public void IsFixed_ReturnsTrue()
        {
            Assert.IsTrue(TimeZone.IsFixed);
        }

        [Test]
        public void GetZoneIntervalInstant_Null()
        {
            var actual = TimeZone.GetZoneInterval(Instant.UnixEpoch);
            Assert.IsNull(actual);
        }

        [Test]
        public void GetZoneIntervalLocalInstant_Null()
        {
            var actual = TimeZone.GetZoneInterval(LocalInstant.LocalUnixEpoch);
            Assert.IsNull(actual);
        }

        [Test]
        public void WriteRead()
        {
            var dio = new DtzIoHelper();
            var actual = dio.WriteRead(TimeZone);
            // TODO: this should be AreSame when Serialization is setup
            Assert.AreEqual(TimeZone, actual);
        }
    }
}