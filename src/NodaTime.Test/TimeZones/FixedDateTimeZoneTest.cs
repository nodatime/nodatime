#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class FixedDateTimeZoneTest
    {
        private static readonly Offset ZoneOffset = Offset.FromHours(-8);
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
            Assert.AreEqual("UTC-08", TestZone.Id, "TestZone.Id");
            Assert.AreEqual("UTC-08", TestZone.GetZoneInterval(Instant.UnixEpoch).Name);
            Assert.AreEqual(ZoneOffset, TestZone.GetOffsetFromUtc(Instant.UnixEpoch), "TestZone.GetOffsetFromUtc()");
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

        [Test]
        public void For_Id_FixedOffset()
        {
            string id = "UTC+05:30";
            DateTimeZone zone = FixedDateTimeZone.GetFixedZoneOrNull(id);
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
        }

        [Test]
        public void For_Id_FixedOffset_NonCanonicalId()
        {
            string id = "UTC+05:00:00";
            DateTimeZone zone = FixedDateTimeZone.GetFixedZoneOrNull(id);
            Assert.AreEqual(zone, DateTimeZone.ForOffset(Offset.FromHours(5)));
            Assert.AreEqual("UTC+05", zone.Id);
        }

        [Test]
        public void For_Id_InvalidFixedOffset()
        {
            Assert.IsNull(FixedDateTimeZone.GetFixedZoneOrNull("UTC+5Months"));
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsClass<DateTimeZone>(new FixedDateTimeZone(Offset.FromMilliseconds(300)),
                new FixedDateTimeZone(Offset.FromMilliseconds(300)),
                new FixedDateTimeZone(Offset.FromMilliseconds(500)));

            TestHelper.TestEqualsClass<DateTimeZone>(new FixedDateTimeZone("Foo", Offset.FromMilliseconds(300)),
                new FixedDateTimeZone("Foo", Offset.FromMilliseconds(300)),
                new FixedDateTimeZone("Bar", Offset.FromMilliseconds(300)));
        }
    }
}