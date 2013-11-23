// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;

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
            var actual = TestZone.GetZoneInterval(NodaConstants.UnixEpoch);
            Assert.AreEqual(FixedPeriod, actual);
        }

        [Test]
        public void SimpleProperties_ReturnValuesFromConstructor()
        {
            Assert.AreEqual("UTC-08", TestZone.Id, "TestZone.Id");
            Assert.AreEqual("UTC-08", TestZone.GetZoneInterval(NodaConstants.UnixEpoch).Name);
            Assert.AreEqual(ZoneOffset, TestZone.GetUtcOffset(NodaConstants.UnixEpoch));
            Assert.AreEqual(ZoneOffset, TestZone.MinOffset);
            Assert.AreEqual(ZoneOffset, TestZone.MaxOffset);
        }

        [Test]
        public void GetZoneIntervals_ReturnsSingleInterval()
        {
            var intervals = TestZone.GetZoneIntervalPair(new LocalDateTime(2001, 7, 1, 1, 0, 0).LocalInstant);
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