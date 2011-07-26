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
    public class FixedDateTimeZoneTest
    {
        private static readonly Offset ZoneOffset = Offset.ForHours(-8);
        private static readonly FixedDateTimeZone TestZone = new FixedDateTimeZone(ZoneOffset);
        // private static readonly FixedDateTimeZone PstTimeZone = new FixedDateTimeZone("test", OneHour);
        private static readonly ZoneInterval FixedPeriod = new ZoneInterval(TestZone.Id, Instant.MinValue, Instant.MaxValue, ZoneOffset, Offset.Zero);

        [Test]
        public void IsFixed_ReturnsTrue()
        {
            Assert.IsTrue(TestZone.IsFixed);
        }

        [Test]
        public void GetZoneIntervalInstant_ZoneInterval()
        {
            var actual = TestZone.GetZoneInterval(Instant.UnixEpoch);
            Assert.AreEqual(FixedPeriod, actual);
        }

        [Test]
        public void SimpleProperties_ReturnValuesFromConstructor()
        {
            Assert.AreEqual("UTC-8", TestZone.Id);
            Assert.AreEqual("UTC-8", TestZone.GetZoneInterval(Instant.UnixEpoch).Name);
            Assert.AreEqual(ZoneOffset, TestZone.GetOffsetFromUtc(Instant.UnixEpoch));
            Assert.AreEqual(ZoneOffset, TestZone.MinOffset);
            Assert.AreEqual(ZoneOffset, TestZone.MaxOffset);
        }

        [Test]
        public void TestReadWrite()
        {
            var dio = new DtzIoHelper("FixedDateTimeZone");
            dio.TestTimeZone(TestZone);
        }

        [Test]
        public void GetZoneIntervals_ReturnsSingleInterval()
        {
            var intervals = TestZone.GetZoneIntervals(new LocalDateTime(2001, 7, 1, 1, 0, 0).LocalInstant);
            Assert.AreEqual(FixedPeriod, intervals.EarlyInterval);
            Assert.IsNull(intervals.LateInterval);
        }
    }
}