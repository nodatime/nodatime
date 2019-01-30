// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Test.TimeZones.IO;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    public class FixedDateTimeZoneTest
    {
        private static readonly Offset ZoneOffset = Offset.FromHours(-8);
        private static readonly FixedDateTimeZone TestZone = new FixedDateTimeZone(ZoneOffset);
        private static readonly ZoneInterval FixedPeriod = new ZoneInterval(TestZone.Id, Instant.BeforeMinValue, Instant.AfterMaxValue, ZoneOffset, Offset.Zero);

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
            var mapping = TestZone.MapLocal(new LocalDateTime(2001, 7, 1, 1, 0, 0));
            Assert.AreEqual(FixedPeriod, mapping.EarlyInterval);
            Assert.AreEqual(FixedPeriod, mapping.LateInterval);
            Assert.AreEqual(1, mapping.Count);
        }

        [Test]
        public void For_Id_FixedOffset()
        {
            string id = "UTC+05:30";
            DateTimeZone zone = FixedDateTimeZone.GetFixedZoneOrNull(id)!;
            Assert.AreEqual(DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(5, 30)), zone);
            Assert.AreEqual(id, zone.Id);
        }

        [Test]
        public void For_Id_FixedOffset_NonCanonicalId()
        {
            string id = "UTC+05:00:00";
            DateTimeZone zone = FixedDateTimeZone.GetFixedZoneOrNull(id)!;
            Assert.AreEqual(zone, DateTimeZone.ForOffset(Offset.FromHours(5)));
            Assert.AreEqual("UTC+05", zone.Id);
        }

        [Test]
        public void For_Id_InvalidFixedOffset()
        {
            Assert.IsNull(FixedDateTimeZone.GetFixedZoneOrNull("UTC+5Months"));
        }

        [Test]
        public void ExplicitNameAppearsInZoneInterval()
        {
            var zone = new FixedDateTimeZone("id", Offset.FromHours(5), "name");
            var interval = zone.GetZoneInterval(NodaConstants.UnixEpoch);
            Assert.AreEqual("id", zone.Id); // Check we don't get this wrong...
            Assert.AreEqual("name", interval.Name);
            Assert.AreEqual("name", zone.Name);
        }

        [Test]
        public void ZoneIntervalNameDefaultsToZoneId()
        {
            var zone = new FixedDateTimeZone("id", Offset.FromHours(5));
            var interval = zone.GetZoneInterval(NodaConstants.UnixEpoch);
            Assert.AreEqual("id", interval.Name);
            Assert.AreEqual("id", zone.Name);
        }

        [Test]
        public void Read_NoNameInStream()
        {
            var ioHelper = DtzIoHelper.CreateNoStringPool();
            var offset = Offset.FromHours(5);
            ioHelper.Writer.WriteOffset(offset);
            var zone = (FixedDateTimeZone) FixedDateTimeZone.Read(ioHelper.Reader, "id");

            Assert.AreEqual("id", zone.Id);
            Assert.AreEqual(offset, zone.Offset);
            Assert.AreEqual("id", zone.Name);
        }

        [Test]
        public void Read_WithNameInStream()
        {
            var ioHelper = DtzIoHelper.CreateNoStringPool();
            var offset = Offset.FromHours(5);
            ioHelper.Writer.WriteOffset(offset);
            ioHelper.Writer.WriteString("name");
            var zone = (FixedDateTimeZone) FixedDateTimeZone.Read(ioHelper.Reader, "id");

            Assert.AreEqual("id", zone.Id);
            Assert.AreEqual(offset, zone.Offset);
            Assert.AreEqual("name", zone.Name);
        }

        [Test]
        public void Roundtrip()
        {
            var ioHelper = DtzIoHelper.CreateNoStringPool();
            var oldZone = new FixedDateTimeZone("id", Offset.FromHours(4), "name");
            oldZone.Write(ioHelper.Writer);
            var newZone = (FixedDateTimeZone) FixedDateTimeZone.Read(ioHelper.Reader, "id");

            Assert.AreEqual(oldZone.Id, newZone.Id);
            Assert.AreEqual(oldZone.Offset, newZone.Offset);
            Assert.AreEqual(oldZone.Name, newZone.Name);
        }

        [Test]
        public void Equals()
        {
            TestHelper.TestEqualsClass(new FixedDateTimeZone(Offset.FromSeconds(300)),
                new FixedDateTimeZone(Offset.FromSeconds(300)),
                new FixedDateTimeZone(Offset.FromSeconds(500)));

            TestHelper.TestEqualsClass(new FixedDateTimeZone("Foo", Offset.FromSeconds(300)),
                new FixedDateTimeZone("Foo", Offset.FromSeconds(300)),
                new FixedDateTimeZone("Bar", Offset.FromSeconds(300)));
        }
    }
}
