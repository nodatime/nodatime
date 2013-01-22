// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for OffsetDateTime.
    /// </summary>
    [TestFixture]
    public class OffsetDateTimeTest
    {
        [Test]
        public void LocalDateTimeProperties()
        {
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, 4, 5, CalendarSystem.GetJulianCalendar(5));
            Offset offset = Offset.FromHours(5);

            OffsetDateTime odt = new OffsetDateTime(local, offset);

            var localDateTimePropertyNames = typeof(LocalDateTime).GetProperties()
                                                                  .Select(p => p.Name)
                                                                  .ToList();
            var commonProperties = typeof(OffsetDateTime).GetProperties()
                                                         .Where(p => localDateTimePropertyNames.Contains(p.Name));
            foreach (var property in commonProperties)
            {
                Assert.AreEqual(typeof(LocalDateTime).GetProperty(property.Name).GetValue(local, null),
                                property.GetValue(odt, null));
            }
        }

        [Test]
        public void OffsetProperty()
        {
            Offset offset = Offset.FromHours(5);

            OffsetDateTime odt = new OffsetDateTime(new LocalDateTime(2012, 1, 2, 3, 4), offset);
            Assert.AreEqual(offset, odt.Offset);
        }

        [Test]
        public void LocalDateTimeProperty()
        {
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, 4, 5, CalendarSystem.GetJulianCalendar(5));
            Offset offset = Offset.FromHours(5);

            OffsetDateTime odt = new OffsetDateTime(local, offset);
            Assert.AreEqual(local, odt.LocalDateTime);
        }

        [Test]
        public void ToInstant()
        {
            Instant instant = Instant.FromUtc(2012, 6, 25, 16, 5, 20);
            LocalDateTime local = new LocalDateTime(2012, 6, 25, 21, 35, 20);
            Offset offset = Offset.FromHoursAndMinutes(5, 30);

            OffsetDateTime odt = new OffsetDateTime(local, offset);
            Assert.AreEqual(instant, odt.ToInstant());
        }

        [Test]
        public void Equality()
        {
            LocalDateTime local1 = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            LocalDateTime local2 = new LocalDateTime(2012, 9, 5, 1, 2, 3);
            Offset offset1 = Offset.FromHours(1);
            Offset offset2 = Offset.FromHours(2);

            OffsetDateTime equal1 = new OffsetDateTime(local1, offset1);
            OffsetDateTime equal2 = new OffsetDateTime(local1, offset1);
            OffsetDateTime unequalByOffset = new OffsetDateTime(local1, offset2);
            OffsetDateTime unequalByLocal = new OffsetDateTime(local2, offset1);

            TestHelper.TestEqualsStruct(equal1, equal2, unequalByOffset);
            TestHelper.TestEqualsStruct(equal1, equal2, unequalByLocal);

            TestHelper.TestOperatorEquality(equal1, equal2, unequalByOffset);
            TestHelper.TestOperatorEquality(equal1, equal2, unequalByLocal);
        }

        [Test]
        public void ToDateTimeOffset()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            Offset offset = Offset.FromHours(1);
            OffsetDateTime odt = new OffsetDateTime(local, offset);

            DateTimeOffset expected = new DateTimeOffset(DateTime.SpecifyKind(new DateTime(2012, 10, 6, 1, 2, 3), DateTimeKind.Unspecified),
                TimeSpan.FromHours(1));
            DateTimeOffset actual = odt.ToDateTimeOffset();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromDateTimeOffset()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            Offset offset = Offset.FromHours(1);
            OffsetDateTime expected = new OffsetDateTime(local, offset);

            // We can build an OffsetDateTime regardless of kind... although if the kind is Local, the offset
            // has to be valid for the local time zone when building a DateTimeOffset, and if the kind is Utc, the offset has to be zero.
            DateTimeOffset bcl = new DateTimeOffset(DateTime.SpecifyKind(new DateTime(2012, 10, 6, 1, 2, 3), DateTimeKind.Unspecified),
                TimeSpan.FromHours(1));
            OffsetDateTime actual = OffsetDateTime.FromDateTimeOffset(bcl);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InFixedZone()
        {
            Offset offset = Offset.FromHours(5);
            LocalDateTime local = new LocalDateTime(2012, 1, 2, 3, 4);
            OffsetDateTime odt = new OffsetDateTime(local, offset);

            ZonedDateTime zoned = odt.InFixedZone();
            Assert.AreEqual(DateTimeZone.ForOffset(offset).AtStrictly(local), zoned);
        }

        [Test]
        public void ToString_WholeHourOffset()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            Offset offset = Offset.FromHours(1);
            OffsetDateTime odt = new OffsetDateTime(local, offset);
            Assert.AreEqual("2012-10-06T01:02:03+01", odt.ToString());
        }

        [Test]
        public void ToString_PartHourOffset()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            Offset offset = Offset.FromHoursAndMinutes(1, 30);
            OffsetDateTime odt = new OffsetDateTime(local, offset);
            Assert.AreEqual("2012-10-06T01:02:03+01:30", odt.ToString());
        }

        [Test]
        public void ToString_Utc()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            OffsetDateTime odt = new OffsetDateTime(local, Offset.Zero);
            Assert.AreEqual("2012-10-06T01:02:03Z", odt.ToString());
        }

        /// <summary>
        ///   Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new OffsetDateTime();
            Assert.AreEqual(NodaConstants.UnixEpoch.InUtc().LocalDateTime, actual.LocalDateTime);
            Assert.AreEqual(Offset.Zero, actual.Offset);
        }
    }
}
