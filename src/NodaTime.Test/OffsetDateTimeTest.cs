// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Text;
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
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, 4, 5, CommonCalendars.Julian);
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
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, 4, 5, CommonCalendars.Julian);
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

        [Test]
        public void ToString_WithFormat()
        {
            LocalDateTime local = new LocalDateTime(2012, 10, 6, 1, 2, 3);
            Offset offset = Offset.FromHours(1);
            OffsetDateTime odt = new OffsetDateTime(local, offset);
            Assert.AreEqual("2012/10/06 01:02:03 01", odt.ToString("yyyy/MM/dd HH:mm:ss o<-HH>", CultureInfo.InvariantCulture));
        }
        
        [Test]
        public void LocalComparer()
        {
            var localControl = new LocalDateTime(2013, 4, 2, 19, 54);
            var control = new OffsetDateTime(localControl, Offset.Zero);
            var negativeOffset = control.LocalDateTime.WithOffset(Offset.FromHours(-1));
            var positiveOffset = control.LocalDateTime.WithOffset(Offset.FromHours(1));
            var differentCalendar = control.LocalDateTime.WithCalendar(CommonCalendars.Coptic).WithOffset(Offset.FromHours(5));
            // Later instant, earlier local
            var earlierLocal = control.LocalDateTime.PlusHours(-2).WithOffset(Offset.FromHours(-10));
            // Earlier instant, later local
            var laterLocal = control.LocalDateTime.PlusHours(2).WithOffset(Offset.FromHours(10));

            var comparer = OffsetDateTime.Comparer.Local;

            Assert.AreEqual(0, comparer.Compare(control, negativeOffset));
            Assert.AreEqual(0, comparer.Compare(control, positiveOffset));
            Assert.Throws<ArgumentException>(() => comparer.Compare(control, differentCalendar));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(control, earlierLocal)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(earlierLocal, control)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(control, laterLocal)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(laterLocal, control)));

            Assert.IsFalse(comparer.Equals(control, differentCalendar));
            Assert.IsFalse(comparer.Equals(control, earlierLocal));
            Assert.IsTrue(comparer.Equals(control, control));
        }

        [Test]
        public void InstantComparer()
        {
            var localControl = new LocalDateTime(2013, 4, 2, 19, 54);
            var control = new OffsetDateTime(localControl, Offset.Zero);
            var equalAndOppositeChanges = control.LocalDateTime.PlusHours(1).WithOffset(Offset.FromHours(1));
            var differentCalendar = control.LocalDateTime.WithCalendar(CommonCalendars.Coptic).WithOffset(Offset.Zero);

            // Negative offset means later instant
            var negativeOffset = control.LocalDateTime.WithOffset(Offset.FromHours(-1));
            // Positive offset means earlier instant
            var positiveOffset = control.LocalDateTime.WithOffset(Offset.FromHours(1));

            // Later instant, earlier local
            var earlierLocal = control.LocalDateTime.PlusHours(-2).WithOffset(Offset.FromHours(-10));
            // Earlier instant, later local
            var laterLocal = control.LocalDateTime.PlusHours(2).WithOffset(Offset.FromHours(10));

            var comparer = OffsetDateTime.Comparer.Instant;

            Assert.AreEqual(0, comparer.Compare(control, differentCalendar));
            Assert.AreEqual(0, comparer.Compare(control, equalAndOppositeChanges));

            Assert.AreEqual(-1, Math.Sign(comparer.Compare(control, negativeOffset)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(negativeOffset, control)));
            Assert.AreEqual(1, comparer.Compare(control, positiveOffset));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(positiveOffset, control)));

            Assert.AreEqual(-1, Math.Sign(comparer.Compare(control, earlierLocal)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(earlierLocal, control)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(control, laterLocal)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(laterLocal, control)));

            Assert.IsTrue(comparer.Equals(control, differentCalendar));
            Assert.IsFalse(comparer.Equals(control, earlierLocal));
            Assert.IsTrue(comparer.Equals(control, equalAndOppositeChanges));
        }

        /// <summary>
        /// Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new OffsetDateTime();
            Assert.AreEqual(new LocalDateTime(1, 1, 1, 0, 0), actual.LocalDateTime);
            Assert.AreEqual(Offset.Zero, actual.Offset);
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(
                new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23), Offset.FromHours(1)));
            TestHelper.AssertBinaryRoundtrip(
                new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23, CommonCalendars.Julian), Offset.FromHours(1)));
        }

        [Test]
        public void XmlSerialization_Iso()
        {
            var value = new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23), Offset.FromHours(1));
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04-12T17:53:23+01:00</value>");
        }

        [Test]
        public void XmlSerialization_ZeroOffset()
        {
            var value = new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23), Offset.Zero);
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04-12T17:53:23Z</value>");
        }

        [Test]
        public void XmlSerialization_NonIso()
        {
            var value = new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23, CommonCalendars.Julian),
                Offset.FromHours(1));
            TestHelper.AssertXmlRoundtrip(value, "<value calendar=\"Julian\">2013-04-12T17:53:23+01:00</value>");
        }

        [Test]
        [TestCase("<value calendar=\"Rubbish\">2013-06-12T17:53:23-04</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-15-12T17:53:23-04</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<OffsetDateTime>(xml, expectedExceptionType);
        }

        [Test]
        public void WithOffset()
        {
            LocalDateTime morning = new LocalDateTime(2014, 1, 31, 9, 30);
            OffsetDateTime original = new OffsetDateTime(morning, Offset.FromHours(-8));
            LocalDateTime evening = new LocalDateTime(2014, 1, 31, 19, 30);
            Offset newOffset = Offset.FromHours(2);
            OffsetDateTime expected = new OffsetDateTime(evening, newOffset);
            Assert.AreEqual(expected, original.WithOffset(newOffset));
        }

        [Test]
        public void WithCalendar()
        {
            CalendarSystem julianCalendar = CommonCalendars.Julian;
            OffsetDateTime gregorianEpoch = NodaConstants.UnixEpoch.WithOffset(Offset.Zero);

            OffsetDateTime expected = new LocalDate(1969, 12, 19, julianCalendar).AtMidnight().WithOffset(Offset.FromHours(0));
            OffsetDateTime actual = gregorianEpoch.WithCalendar(CommonCalendars.Julian);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void With_TimeAdjuster()
        {
            Offset offset = Offset.FromHoursAndMinutes(2, 30);
            OffsetDateTime start = new LocalDateTime(2014, 6, 27, 12, 15, 8, 100, 1234).WithOffset(offset);
            OffsetDateTime expected = new LocalDateTime(2014, 6, 27, 12, 15, 8).WithOffset(offset);
            Assert.AreEqual(expected, start.With(TimeAdjusters.TruncateToSecond));
        }

        [Test]
        public void With_DateAdjuster()
        {
            Offset offset = Offset.FromHoursAndMinutes(2, 30);
            OffsetDateTime start = new LocalDateTime(2014, 6, 27, 12, 5, 8, 100, 1234).WithOffset(offset);
            OffsetDateTime expected = new LocalDateTime(2014, 6, 30, 12, 5, 8, 100, 1234).WithOffset(offset);
            Assert.AreEqual(expected, start.With(DateAdjusters.EndOfMonth));
        }
    }
}
