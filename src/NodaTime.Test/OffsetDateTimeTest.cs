// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Text;
using NUnit.Framework;
using System.Reflection;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for OffsetDateTime.
    /// </summary>
    public class OffsetDateTimeTest
    {
        [Test]
        public void LocalDateTimeProperties()
        {
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, CalendarSystem.Julian).PlusNanoseconds(123456789);
            Offset offset = Offset.FromHours(5);

            OffsetDateTime odt = new OffsetDateTime(local, offset);

            var localDateTimePropertyNames = typeof(LocalDateTime).GetTypeInfo()
                                                                  .DeclaredProperties
                                                                  .Select(p => p.Name)
                                                                  .ToList();
            var commonProperties = typeof(OffsetDateTime).GetTypeInfo()
                                                         .DeclaredProperties
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
            LocalDateTime local = new LocalDateTime(2012, 6, 19, 1, 2, 3, CalendarSystem.Julian).PlusNanoseconds(123456789);
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
        [TestCase(0, 30, 20)]
        [TestCase(-1, -30, -20)]
        [TestCase(0, 30, 55)]
        [TestCase(-1, -30, -55)]
        public void ToDateTimeOffset_TruncatedOffset(int hours, int minutes, int seconds)
        {
            var ldt = new LocalDateTime(2017, 1, 9, 21, 45, 20);
            var offset = Offset.FromHoursAndMinutes(hours, minutes).Plus(Offset.FromSeconds(seconds));
            var odt = ldt.WithOffset(offset);
            var dto = odt.ToDateTimeOffset();
            // We preserve the local date/time, so the instant will move forward as the offset
            // is truncated.
            Assert.AreEqual(new DateTime(2017, 1, 9, 21, 45, 20, DateTimeKind.Unspecified), dto.DateTime);
            Assert.AreEqual(TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes), dto.Offset);
        }

        [Test]
        [TestCase(-15)]
        [TestCase(15)]
        public void ToDateTimeOffset_OffsetOutOfRange(int hours)
        {
            var ldt = new LocalDateTime(2017, 1, 9, 21, 45, 20);
            var offset = Offset.FromHours(hours);
            var odt = ldt.WithOffset(offset);
            Assert.Throws<InvalidOperationException>(() => odt.ToDateTimeOffset());
        }

        [Test]
        [TestCase(-14)]
        [TestCase(14)]
        public void ToDateTimeOffset_OffsetEdgeOfRange(int hours)
        {
            var ldt = new LocalDateTime(2017, 1, 9, 21, 45, 20);
            var offset = Offset.FromHours(hours);
            var odt = ldt.WithOffset(offset);
            Assert.AreEqual(hours, odt.ToDateTimeOffset().Offset.TotalHours);
        }

        [Test]
        public void ToDateTimeOffset_DateOutOfRange()
        {
            // One day before 1st January, 1AD (which is DateTime.MinValue)
            var odt = new LocalDate(1, 1, 1).PlusDays(-1).AtMidnight().WithOffset(Offset.FromHours(1));
            Assert.Throws<InvalidOperationException>(() => odt.ToDateTimeOffset());
        }

        [Test]
        [TestCase(100)]
        [TestCase(1900)]
        [TestCase(2900)]
        public void ToDateTimeOffset_TruncateNanosTowardStartOfTime(int year)
        {
            var odt = new LocalDateTime(year, 1, 1, 13, 15, 55).PlusNanoseconds(NodaConstants.NanosecondsPerSecond - 1)
                .WithOffset(Offset.FromHours(1));
            var expected = new DateTimeOffset(year, 1, 1, 13, 15, 55, TimeSpan.FromHours(1))
                .AddTicks(NodaConstants.TicksPerSecond - 1);
            var actual = odt.ToDateTimeOffset();
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
            var differentCalendar = control.LocalDateTime.WithCalendar(CalendarSystem.Coptic).WithOffset(Offset.FromHours(5));
            // Later instant, earlier local
            var earlierLocal = control.LocalDateTime.PlusHours(-2).WithOffset(Offset.FromHours(-10));
            // Same offset, previous day
            var muchEarlierLocal = control.PlusHours(-24);
            // Earlier instant, later local
            var laterLocal = control.LocalDateTime.PlusHours(2).WithOffset(Offset.FromHours(10));
            // Same offset, next day
            var muchLaterLocal = control.PlusHours(24);

            var comparer = OffsetDateTime.Comparer.Local;

            Assert.AreEqual(0, comparer.Compare(control, negativeOffset));
            Assert.AreEqual(0, comparer.Compare(control, positiveOffset));
            Assert.Throws<ArgumentException>(() => comparer.Compare(control, differentCalendar));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(control, earlierLocal)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(control, muchEarlierLocal)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(earlierLocal, control)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(muchEarlierLocal, control)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(control, laterLocal)));
            Assert.AreEqual(-1, Math.Sign(comparer.Compare(control, muchLaterLocal)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(laterLocal, control)));
            Assert.AreEqual(1, Math.Sign(comparer.Compare(muchLaterLocal, control)));

            Assert.IsFalse(comparer.Equals(control, differentCalendar));
            Assert.IsFalse(comparer.Equals(control, earlierLocal));
            Assert.IsFalse(comparer.Equals(control, muchEarlierLocal));
            Assert.IsFalse(comparer.Equals(control, laterLocal));
            Assert.IsFalse(comparer.Equals(control, muchLaterLocal));
            Assert.IsTrue(comparer.Equals(control, control));

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(negativeOffset));
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(positiveOffset));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(earlierLocal));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(muchEarlierLocal));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(laterLocal));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(muchLaterLocal));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(differentCalendar));
        }

        [Test]
        public void InstantComparer()
        {
            var localControl = new LocalDateTime(2013, 4, 2, 19, 54);
            var control = new OffsetDateTime(localControl, Offset.Zero);
            var equalAndOppositeChanges = control.LocalDateTime.PlusHours(1).WithOffset(Offset.FromHours(1));
            var differentCalendar = control.LocalDateTime.WithCalendar(CalendarSystem.Coptic).WithOffset(Offset.Zero);

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

            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(differentCalendar));
            Assert.AreEqual(comparer.GetHashCode(control), comparer.GetHashCode(equalAndOppositeChanges));
            Assert.AreNotEqual(comparer.GetHashCode(control), comparer.GetHashCode(earlierLocal));
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
        public void Subtraction_Duration()
        {
            // Test all three approaches... not bothering to check a different calendar,
            // but we'll use two different offsets.
            OffsetDateTime end = new LocalDateTime(2014, 08, 14, 15, 0).WithOffset(Offset.FromHours(1));
            Duration duration = Duration.FromHours(8) + Duration.FromMinutes(9);
            OffsetDateTime expected = new LocalDateTime(2014, 08, 14, 6, 51).WithOffset(Offset.FromHours(1));
            Assert.AreEqual(expected, end - duration);
            Assert.AreEqual(expected, end.Minus(duration));
            Assert.AreEqual(expected, OffsetDateTime.Subtract(end, duration));
        }

        [Test]
        public void Addition_Duration()
        {
            const int minutes = 23;
            const int hours = 3;
            const int milliseconds = 40000;
            const long seconds = 321;
            const long nanoseconds = 12345;
            const long ticks = 5432112345;

            // Test all three approaches... not bothering to check a different calendar,
            // but we'll use two different offsets.
            OffsetDateTime start = new LocalDateTime(2014, 08, 14, 6, 51).WithOffset(Offset.FromHours(1));
            Duration duration = Duration.FromHours(8) + Duration.FromMinutes(9);
            OffsetDateTime expected = new LocalDateTime(2014, 08, 14, 15, 0).WithOffset(Offset.FromHours(1));
            Assert.AreEqual(expected, start + duration);
            Assert.AreEqual(expected, start.Plus(duration));
            Assert.AreEqual(expected, OffsetDateTime.Add(start, duration));

            Assert.AreEqual(start + Duration.FromHours(hours), start.PlusHours(hours));
            Assert.AreEqual(start + Duration.FromHours(-hours), start.PlusHours(-hours));

            Assert.AreEqual(start + Duration.FromMinutes(minutes), start.PlusMinutes(minutes));
            Assert.AreEqual(start + Duration.FromMinutes(-minutes), start.PlusMinutes(-minutes));

            Assert.AreEqual(start + Duration.FromSeconds(seconds), start.PlusSeconds(seconds));
            Assert.AreEqual(start + Duration.FromSeconds(-seconds), start.PlusSeconds(-seconds));

            Assert.AreEqual(start + Duration.FromMilliseconds(milliseconds), start.PlusMilliseconds(milliseconds));
            Assert.AreEqual(start + Duration.FromMilliseconds(-milliseconds), start.PlusMilliseconds(-milliseconds));

            Assert.AreEqual(start + Duration.FromTicks(ticks), start.PlusTicks(ticks));
            Assert.AreEqual(start + Duration.FromTicks(-ticks), start.PlusTicks(-ticks));

            Assert.AreEqual(start + Duration.FromNanoseconds(nanoseconds), start.PlusNanoseconds(nanoseconds));
            Assert.AreEqual(start + Duration.FromNanoseconds(-nanoseconds), start.PlusNanoseconds(-nanoseconds));
        }

        [Test]
        public void Subtraction_OffsetDateTime()
        {
            // Test all three approaches... not bothering to check a different calendar,
            // but we'll use two different offsets.
            OffsetDateTime start = new LocalDateTime(2014, 08, 14, 6, 51).WithOffset(Offset.FromHours(1));
            OffsetDateTime end = new LocalDateTime(2014, 08, 14, 18, 0).WithOffset(Offset.FromHours(4));
            Duration expected = Duration.FromHours(8) + Duration.FromMinutes(9);
            Assert.AreEqual(expected, end - start);
            Assert.AreEqual(expected, end.Minus(start));
            Assert.AreEqual(expected, OffsetDateTime.Subtract(end, start));
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(
                new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23).PlusNanoseconds(1), Offset.FromHours(1)));
            TestHelper.AssertBinaryRoundtrip(
                new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23, CalendarSystem.Julian).PlusNanoseconds(1), Offset.FromHours(1)));
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
            var value = new OffsetDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23, CalendarSystem.Julian),
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
        public void WithOffset_CrossDates()
        {
            OffsetDateTime noon = new OffsetDateTime(new LocalDateTime(2017, 8, 22, 12, 0, 0), Offset.FromHours(0));
            OffsetDateTime previousNight = noon.WithOffset(Offset.FromHours(-14));
            OffsetDateTime nextMorning = noon.WithOffset(Offset.FromHours(14));
            Assert.AreEqual(new LocalDateTime(2017, 8, 21, 22, 0, 0), previousNight.LocalDateTime);
            Assert.AreEqual(new LocalDateTime(2017, 8, 23, 2, 0, 0), nextMorning.LocalDateTime);
        }

        [Test]
        public void WithOffset_TwoDaysForwardAndBack()
        {
            // Go from UTC-18 to UTC+18
            OffsetDateTime night = new OffsetDateTime(new LocalDateTime(2017, 8, 21, 18, 0, 0), Offset.FromHours(-18));
            OffsetDateTime morning = night.WithOffset(Offset.FromHours(18));
            Assert.AreEqual(new LocalDateTime(2017, 8, 23, 6, 0, 0), morning.LocalDateTime);
            OffsetDateTime backAgain = morning.WithOffset(Offset.FromHours(-18));
            Assert.AreEqual(night, backAgain);
        }

        [Test]
        public void WithCalendar()
        {
            CalendarSystem julianCalendar = CalendarSystem.Julian;
            OffsetDateTime gregorianEpoch = NodaConstants.UnixEpoch.WithOffset(Offset.Zero);

            OffsetDateTime expected = new LocalDate(1969, 12, 19, julianCalendar).AtMidnight().WithOffset(Offset.FromHours(0));
            OffsetDateTime actual = gregorianEpoch.WithCalendar(CalendarSystem.Julian);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void With_TimeAdjuster()
        {
            Offset offset = Offset.FromHoursAndMinutes(2, 30);
            OffsetDateTime start = new LocalDateTime(2014, 6, 27, 12, 15, 8).PlusNanoseconds(123456789).WithOffset(offset);
            OffsetDateTime expected = new LocalDateTime(2014, 6, 27, 12, 15, 8).WithOffset(offset);
            Assert.AreEqual(expected, start.With(TimeAdjusters.TruncateToSecond));
        }

        [Test]
        public void With_DateAdjuster()
        {
            Offset offset = Offset.FromHoursAndMinutes(2, 30);
            OffsetDateTime start = new LocalDateTime(2014, 6, 27, 12, 5, 8).PlusNanoseconds(123456789).WithOffset(offset);
            OffsetDateTime expected = new LocalDateTime(2014, 6, 30, 12, 5, 8).PlusNanoseconds(123456789).WithOffset(offset);
            Assert.AreEqual(expected, start.With(DateAdjusters.EndOfMonth));
        }

        [Test]
        public void InZone()
        {
            Offset offset = Offset.FromHours(-7);
            OffsetDateTime start = new LocalDateTime(2017, 10, 31, 18, 12, 0).WithOffset(offset);
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            var zoned = start.InZone(zone);

            // On October 31st, the UK had already gone back, so the offset is 0.
            // Importantly, it's not the offset of the original OffsetDateTime: we're testing
            // that InZone *doesn't* require that.
            var expected = new ZonedDateTime(new LocalDateTime(2017, 11, 1, 1, 12, 0).WithOffset(Offset.Zero), zone);
            Assert.AreEqual(expected, zoned);
        }

        [Test]
        public void Deconstruction()
        {
            var dateTime = new LocalDateTime(2017, 10, 15, 21, 30);
            var offset = Offset.FromHours(-2);
            var value = new OffsetDateTime(dateTime, offset);

            var (actualDateTime, actualOffset) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(dateTime, actualDateTime);
                Assert.AreEqual(offset, actualOffset);
            });
        }

        [Test]
        public void MoreGranularDeconstruction()
        {
            var date = new LocalDate(2017, 10, 15);
            var time = new LocalTime(21, 30, 15);
            var offset = Offset.FromHours(-2);

            var value = new OffsetDateTime(date.At(time), offset);

            var (actualDate, actualTime, actualOffset) = value;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(date, actualDate);
                Assert.AreEqual(time, actualTime);
                Assert.AreEqual(offset, actualOffset);
            });
        }

        [Test]
        public void ToOffsetDate()
        {
            var offset = Offset.FromHoursAndMinutes(2, 30);
            var odt = new LocalDateTime(2014, 6, 27, 12, 15, 8).PlusNanoseconds(123456789).WithOffset(offset);
            var expected = new OffsetDate(new LocalDate(2014, 6, 27), offset);
            Assert.AreEqual(expected, odt.ToOffsetDate());
        }

        [Test]
        public void ToOffsetTime()
        {
            var offset = Offset.FromHoursAndMinutes(2, 30);
            var odt = new LocalDateTime(2014, 6, 27, 12, 15, 8).PlusNanoseconds(123456789).WithOffset(offset);
            var expected = new OffsetTime(new LocalTime(12, 15, 8).PlusNanoseconds(123456789), offset);
            Assert.AreEqual(expected, odt.ToOffsetTime());
        }
    }
}
