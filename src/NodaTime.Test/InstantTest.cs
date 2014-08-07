// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Calendars;
using NodaTime.Text;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class InstantTest
    {
        private static readonly Instant one = new Instant(Duration.FromNanoseconds(1L));
        private static readonly Instant threeMillion = new Instant(Duration.FromNanoseconds(3000000L));
        private static readonly Instant negativeFiftyMillion = new Instant(Duration.FromNanoseconds(-50000000L));

        [Test]
        public void FromUtcNoSeconds()
        {
            Instant viaUtc = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 0)).ToInstant();
            Assert.AreEqual(viaUtc, Instant.FromUtc(2008, 4, 3, 10, 35));
        }

        [Test]
        public void FromUtcWithSeconds()
        {
            Instant viaUtc = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 23)).ToInstant();
            Assert.AreEqual(viaUtc, Instant.FromUtc(2008, 4, 3, 10, 35, 23));
        }

        [Test]
        public void InUtc()
        {
            ZonedDateTime viaInstant = Instant.FromUtc(2008, 4, 3, 10, 35, 23).InUtc();
            ZonedDateTime expected = DateTimeZone.Utc.AtStrictly(new LocalDateTime(2008, 4, 3, 10, 35, 23));
            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void InZone()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZonedDateTime viaInstant = Instant.FromUtc(2008, 6, 10, 13, 16, 17).InZone(london);

            // London is UTC+1 in the Summer, so the above is 14:16:17 local.
            LocalDateTime local = new LocalDateTime(2008, 6, 10, 14, 16, 17);
            ZonedDateTime expected = london.AtStrictly(local);

            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void WithOffset()
        {
            // Jon talks about Noda Time at Leetspeak in Sweden on October 12th 2013, at 13:15 UTC+2
            Instant instant = Instant.FromUtc(2013, 10, 12, 11, 15);
            Offset offset = Offset.FromHours(2);
            OffsetDateTime actual = instant.WithOffset(offset);
            OffsetDateTime expected = new OffsetDateTime(new LocalDateTime(2013, 10, 12, 13, 15), offset);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WithOffset_NonIsoCalendar()
        {
            // October 12th 2013 ISO is 1434-12-07 Islamic
            CalendarSystem calendar = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Civil);
            Instant instant = Instant.FromUtc(2013, 10, 12, 11, 15);
            Offset offset = Offset.FromHours(2);
            OffsetDateTime actual = instant.WithOffset(offset, calendar);
            OffsetDateTime expected = new OffsetDateTime(new LocalDateTime(1434, 12, 7, 13, 15, calendar), offset);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromTicksSinceUnixEpoch()
        {
            Instant instant = Instant.FromTicksSinceUnixEpoch(12345L);
            Assert.AreEqual(12345L, instant.Ticks);
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_Valid()
        {
            Instant actual = Instant.FromMillisecondsSinceUnixEpoch(12345L);
            Instant expected = Instant.FromTicksSinceUnixEpoch(12345L * NodaConstants.TicksPerMillisecond);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromMillisecondsSinceUnixEpoch(long.MaxValue / 100));
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_TooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromMillisecondsSinceUnixEpoch(long.MinValue / 100));
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_Valid()
        {
            Instant actual = Instant.FromSecondsSinceUnixEpoch(12345L);
            Instant expected = Instant.FromTicksSinceUnixEpoch(12345L * NodaConstants.TicksPerSecond);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_TooLarge()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromSecondsSinceUnixEpoch(long.MaxValue / 1000000));
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_TooSmall()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Instant.FromSecondsSinceUnixEpoch(long.MinValue / 1000000));
        }

        [Test]
        public void InZoneWithCalendar()
        {
            CalendarSystem copticCalendar = CommonCalendars.Coptic;
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZonedDateTime viaInstant = Instant.FromUtc(2004, 6, 9, 11, 10).InZone(london, copticCalendar);

            // Date taken from CopticCalendarSystemTest. Time will be 12:10 (London is UTC+1 in Summer)
            LocalDateTime local = new LocalDateTime(1720, 10, 2, 12, 10, 0, copticCalendar);
            ZonedDateTime expected = london.AtStrictly(local);
            Assert.AreEqual(expected, viaInstant);
        }

        [Test]
        public void Max()
        {
            Instant x = Instant.FromTicksSinceUnixEpoch(100);
            Instant y = Instant.FromTicksSinceUnixEpoch(200);
            Assert.AreEqual(y, Instant.Max(x, y));
            Assert.AreEqual(y, Instant.Max(y, x));
            Assert.AreEqual(x, Instant.Max(x, Instant.MinValue));
            Assert.AreEqual(x, Instant.Max(Instant.MinValue, x));
            Assert.AreEqual(Instant.MaxValue, Instant.Max(Instant.MaxValue, x));
            Assert.AreEqual(Instant.MaxValue, Instant.Max(x, Instant.MaxValue));
        }

        [Test]
        public void Min()
        {
            Instant x = Instant.FromTicksSinceUnixEpoch(100);
            Instant y = Instant.FromTicksSinceUnixEpoch(200);
            Assert.AreEqual(x, Instant.Min(x, y));
            Assert.AreEqual(x, Instant.Min(y, x));
            Assert.AreEqual(Instant.MinValue, Instant.Min(x, Instant.MinValue));
            Assert.AreEqual(Instant.MinValue, Instant.Min(Instant.MinValue, x));
            Assert.AreEqual(x, Instant.Min(Instant.MaxValue, x));
            Assert.AreEqual(x, Instant.Min(x, Instant.MaxValue));
        }

        [Test]
        public void ToDateTimeUtc()
        {
            Instant x = Instant.FromUtc(2011, 08, 18, 20, 53);
            DateTime expected = new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Utc);
            DateTime actual = x.ToDateTimeUtc();
            Assert.AreEqual(expected, actual);

            // Kind isn't checked by Equals...
            Assert.AreEqual(DateTimeKind.Utc, actual.Kind);
        }

        [Test]
        public void ToDateTimeOffset()
        {
            Instant x = Instant.FromUtc(2011, 08, 18, 20, 53);
            DateTimeOffset expected = new DateTimeOffset(2011, 08, 18, 20, 53, 0, TimeSpan.Zero);
            Assert.AreEqual(expected, x.ToDateTimeOffset());
        }

        [Test]
        public void FromDateTimeOffset()
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(2011, 08, 18, 20, 53, 0, TimeSpan.FromHours(5));
            Instant expected = Instant.FromUtc(2011, 08, 18, 15, 53);
            Assert.AreEqual(expected, Instant.FromDateTimeOffset(dateTimeOffset));
        }

        [Test]
        public void FromDateTimeUtc_Invalid()
        {
            Assert.Throws<ArgumentException>(() => Instant.FromDateTimeUtc(new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Local)));
            Assert.Throws<ArgumentException>(() => Instant.FromDateTimeUtc(new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Unspecified)));
        }

        [Test]
        public void FromDateTimeUtc_Valid()
        {
            DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, DateTimeKind.Utc);
            Instant expected = Instant.FromUtc(2011, 08, 18, 20, 53);
            Assert.AreEqual(expected, Instant.FromDateTimeUtc(x));
        }

        /// <summary>
        /// Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO Calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new Instant();
            Assert.AreEqual(NodaConstants.UnixEpoch, actual);
        }

        [Test]
        public void XmlSerialization()
        {
            var value = new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567).InUtc().ToInstant();
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04-12T17:53:23.1234567Z</value>");
        }

        [Test]
        [TestCase("<value>2013-15-12T17:53:23Z</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<Instant>(xml, expectedExceptionType);
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(Instant.FromTicksSinceUnixEpoch(12345L));
            TestHelper.AssertBinaryRoundtrip(Instant.MinValue);
            TestHelper.AssertBinaryRoundtrip(Instant.MaxValue);
        }

        [Test]
        [TestCase(-101L, -2L)]
        [TestCase(-100L, -1L)]
        [TestCase(-99L, -1L)]
        [TestCase(-1L, -1L)]
        [TestCase(0L, 0L)]
        [TestCase(99L, 0L)]
        [TestCase(100L, 1L)]
        [TestCase(101L, 1L)]
        public void TicksTruncatesDown(long nanoseconds, long expectedTicks)
        {
            Duration nanos = Duration.FromNanoseconds(nanoseconds);
            Instant instant = new Instant(nanos);
            Assert.AreEqual(expectedTicks, instant.Ticks);
        }

        [Test]
        public void IsValid()
        {
            Assert.IsFalse(Instant.BeforeMinValue.IsValid);
            Assert.IsTrue(Instant.MinValue.IsValid);
            Assert.IsTrue(Instant.MaxValue.IsValid);
            Assert.IsFalse(Instant.AfterMaxValue.IsValid);
        }

        [Test]
        public void InvalidValues()
        {
            Assert.Greater(Instant.AfterMaxValue, Instant.MaxValue);
            Assert.Less(Instant.BeforeMinValue, Instant.MinValue);
        }

        [Test]
        public void PlusDuration_Overflow()
        {
            TestHelper.AssertOverflow(Instant.MinValue.Plus, -Duration.Epsilon);
            TestHelper.AssertOverflow(Instant.MaxValue.Plus, Duration.Epsilon);
        }

        [Test]
        public void ExtremeArithmetic()
        {
            Duration hugeAndPositive = Instant.MaxValue - Instant.MinValue;
            Duration hugeAndNegative = Instant.MinValue - Instant.MaxValue;
            Assert.AreEqual(hugeAndNegative, -hugeAndPositive);
            Assert.AreEqual(Instant.MaxValue, Instant.MinValue - hugeAndNegative);
            Assert.AreEqual(Instant.MaxValue, Instant.MinValue + hugeAndPositive);
            Assert.AreEqual(Instant.MinValue, Instant.MaxValue + hugeAndNegative);
            Assert.AreEqual(Instant.MinValue, Instant.MaxValue - hugeAndPositive);
        }

        [Test]
        public void PlusOffset_Overflow()
        {
            TestHelper.AssertOverflow(Instant.MinValue.Plus, Offset.FromSeconds(-1));
            TestHelper.AssertOverflow(Instant.MaxValue.Plus, Offset.FromSeconds(1));
        }

        [Test]
        public void FromMillisecondsSinceUnixEpoch_Range()
        {
            long smallestValid = Instant.MinValue.Ticks / NodaConstants.TicksPerMillisecond;
            long largestValid = Instant.MaxValue.Ticks / NodaConstants.TicksPerMillisecond;
            TestHelper.AssertValid(Instant.FromMillisecondsSinceUnixEpoch, smallestValid);
            TestHelper.AssertOutOfRange(Instant.FromMillisecondsSinceUnixEpoch, smallestValid - 1);
            TestHelper.AssertValid(Instant.FromMillisecondsSinceUnixEpoch, largestValid);
            TestHelper.AssertOutOfRange(Instant.FromMillisecondsSinceUnixEpoch, largestValid + 1);
        }

        [Test]
        public void FromSecondsSinceUnixEpoch_Range()
        {
            long smallestValid = Instant.MinValue.Ticks / NodaConstants.TicksPerSecond;
            long largestValid = Instant.MaxValue.Ticks / NodaConstants.TicksPerSecond;
            TestHelper.AssertValid(Instant.FromSecondsSinceUnixEpoch, smallestValid);
            TestHelper.AssertOutOfRange(Instant.FromSecondsSinceUnixEpoch, smallestValid - 1);
            TestHelper.AssertValid(Instant.FromSecondsSinceUnixEpoch, largestValid);
            TestHelper.AssertOutOfRange(Instant.FromSecondsSinceUnixEpoch, largestValid + 1);
        }

        [Test]
        public void FromTicksSinceUnixEpoch_Range()
        {
            long smallestValid = Instant.MinValue.Ticks;
            long largestValid = Instant.MaxValue.Ticks;
            TestHelper.AssertValid(Instant.FromTicksSinceUnixEpoch, smallestValid);
            TestHelper.AssertOutOfRange(Instant.FromTicksSinceUnixEpoch, smallestValid - 1);
            TestHelper.AssertValid(Instant.FromTicksSinceUnixEpoch, largestValid);
            TestHelper.AssertOutOfRange(Instant.FromTicksSinceUnixEpoch, largestValid + 1);
        }

        // See issue 269.
        [Test]
        public void ToDateTimeUtc_WithOverflow()
        {
            TestHelper.AssertOverflow(() => Instant.MinValue.ToDateTimeUtc());
        }

        [Test]
        public void ToDateTimeOffset_WithOverflow()
        {
            TestHelper.AssertOverflow(() => Instant.MinValue.ToDateTimeOffset());
        }
    }
}
