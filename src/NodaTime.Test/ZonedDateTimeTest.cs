// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using NodaTime.Calendars;
using NodaTime.Testing.TimeZones;
using NodaTime.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for <see cref="ZonedDateTime"/>. Many of these are really testing
    /// calendar and time zone functionality, but the entry point to that
    /// functionality is usually through ZonedDateTime. This makes testing easier,
    /// as well as serving as more useful documentation.
    /// </summary>
    [TestFixture]
    public class ZonedDateTimeTest
    {
        /// <summary>
        /// Changes from UTC+3 to UTC+4 at 1am local time on June 13th 2011.
        /// </summary>
        private static readonly SingleTransitionDateTimeZone SampleZone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 3, 4);

        [Test]
        public void SimpleProperties()
        {
            var value = SampleZone.AtStrictly(new LocalDateTime(2012, 2, 10, 8, 9, 10, 11, 12));
            Assert.AreEqual(new LocalDate(2012, 2, 10), value.Date);
            Assert.AreEqual(new LocalTime(8, 9, 10, 11, 12), value.TimeOfDay);
            Assert.AreEqual(Era.Common, value.Era);
            Assert.AreEqual(2012, value.Year);
            Assert.AreEqual(2012, value.YearOfEra);
            Assert.AreEqual(2, value.Month);
            Assert.AreEqual(10, value.Day);
            Assert.AreEqual(6, value.WeekOfWeekYear);
            Assert.AreEqual(2012, value.WeekYear);
            Assert.AreEqual(IsoDayOfWeek.Friday, value.IsoDayOfWeek);
            Assert.AreEqual((int) IsoDayOfWeek.Friday, value.DayOfWeek);
            Assert.AreEqual(41, value.DayOfYear);
            Assert.AreEqual(8, value.ClockHourOfHalfDay);
            Assert.AreEqual(8, value.Hour);
            Assert.AreEqual(9, value.Minute);
            Assert.AreEqual(10, value.Second);
            Assert.AreEqual(11, value.Millisecond);
            Assert.AreEqual(11 * 10000 + 12, value.TickOfSecond);
            Assert.AreEqual(8 * NodaConstants.TicksPerHour +
                            9 * NodaConstants.TicksPerMinute +
                            10 * NodaConstants.TicksPerSecond +
                            11 * NodaConstants.TicksPerMillisecond +
                            12,
                            value.TickOfDay);
        }

        [Test]
        public void Add_AroundTimeZoneTransition()
        {
            // Before the transition at 3pm...
            ZonedDateTime before = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 12, 15, 0));
            // 24 hours elapsed, and it's 4pm
            ZonedDateTime afterExpected = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 13, 16, 0));
            ZonedDateTime afterAdd = ZonedDateTime.Add(before, Duration.OneDay);
            ZonedDateTime afterOperator = before + Duration.OneDay;

            Assert.AreEqual(afterExpected, afterAdd);
            Assert.AreEqual(afterExpected, afterOperator);
        }

        [Test]
        public void Add_MethodEquivalents()
        {
            ZonedDateTime before = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 12, 15, 0));
            Assert.AreEqual(before + Duration.OneDay, ZonedDateTime.Add(before, Duration.OneDay));
            Assert.AreEqual(before + Duration.OneDay, before.Plus(Duration.OneDay));
        }

        [Test]
        public void Subtract_AroundTimeZoneTransition()
        {
            // After the transition at 4pm...
            ZonedDateTime after = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 13, 16, 0));
            // 24 hours earlier, and it's 3pm
            ZonedDateTime beforeExpected = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 12, 15, 0));
            ZonedDateTime beforeSubtract = ZonedDateTime.Subtract(after, Duration.OneDay);
            ZonedDateTime beforeOperator = after - Duration.OneDay;

            Assert.AreEqual(beforeExpected, beforeSubtract);
            Assert.AreEqual(beforeExpected, beforeOperator);
        }

        [Test]
        public void SubtractDuration_MethodEquivalents()
        {
            ZonedDateTime after = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 13, 16, 0));
            Assert.AreEqual(after - Duration.OneDay, ZonedDateTime.Subtract(after, Duration.OneDay));
            Assert.AreEqual(after - Duration.OneDay, after.Minus(Duration.OneDay));
        }

        [Test]
        public void Subtraction_ZonedDateTime()
        {
            // Test all three approaches... not bothering to check a different calendar,
            // but we'll use two different time zones.
            ZonedDateTime start = new LocalDateTime(2014, 08, 14, 5, 51).InUtc();
            // Sample zone is UTC+4 at this point, so this is 14:00Z.
            ZonedDateTime end = SampleZone.AtStrictly(new LocalDateTime(2014, 08, 14, 18, 0));
            Duration expected = Duration.FromHours(8) + Duration.FromMinutes(9);
            Assert.AreEqual(expected, end - start);
            Assert.AreEqual(expected, end.Minus(start));
            Assert.AreEqual(expected, ZonedDateTime.Subtract(end, start));
        }

        [Test]
        public void WithZone()
        {
            Instant instant = Instant.FromUtc(2012, 2, 4, 12, 35);
            ZonedDateTime zoned = new ZonedDateTime(instant, SampleZone);
            Assert.AreEqual(new LocalDateTime(2012, 2, 4, 16, 35, 0), zoned.LocalDateTime);

            // Will be UTC-8 for our instant.
            DateTimeZone newZone = new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 1, 1, 0, 0), -7, -8);
            ZonedDateTime converted = zoned.WithZone(newZone);
            Assert.AreEqual(new LocalDateTime(2012, 2, 4, 4, 35, 0), converted.LocalDateTime);
            Assert.AreEqual(converted.ToInstant(), instant);
        }

        [Test]
        public void IsDaylightSavings()
        {
            // Use a real time zone rather than a single-transition zone, so that we can get
            // a savings offset.
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            var winterSummerTransition = Instant.FromUtc(2014, 3, 30, 1, 0);
            var winter = (winterSummerTransition - Duration.Epsilon).InZone(zone);
            var summer = winterSummerTransition.InZone(zone);
            Assert.IsFalse(winter.IsDaylightSavingTime());
            Assert.IsTrue(summer.IsDaylightSavingTime());
        }

        [Test]
        public void FromDateTimeOffset()
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(2011, 3, 5, 1, 0, 0, TimeSpan.FromHours(3));
            DateTimeZone fixedZone = new FixedDateTimeZone(Offset.FromHours(3));
            ZonedDateTime expected = fixedZone.AtStrictly(new LocalDateTime(2011, 3, 5, 1, 0, 0));
            ZonedDateTime actual = ZonedDateTime.FromDateTimeOffset(dateTimeOffset);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDateTimeOffset()
        {
            ZonedDateTime zoned = SampleZone.AtStrictly(new LocalDateTime(2011, 3, 5, 1, 0, 0));
            DateTimeOffset expected = new DateTimeOffset(2011, 3, 5, 1, 0, 0, TimeSpan.FromHours(3));
            DateTimeOffset actual = zoned.ToDateTimeOffset();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToDateTimeUtc()
        {
            ZonedDateTime zoned = SampleZone.AtStrictly(new LocalDateTime(2011, 3, 5, 1, 0, 0));
            // Note that this is 10pm the previous day, UTC - so 1am local time
            DateTime expected = new DateTime(2011, 3, 4, 22, 0, 0, DateTimeKind.Utc);
            DateTime actual = zoned.ToDateTimeUtc();
            Assert.AreEqual(expected, actual);
            // Kind isn't checked by Equals...
            Assert.AreEqual(DateTimeKind.Utc, actual.Kind);

        }

        [Test]
        public void ToDateTimeUnspecified()
        {
            ZonedDateTime zoned = SampleZone.AtStrictly(new LocalDateTime(2011, 3, 5, 1, 0, 0));
            DateTime expected = new DateTime(2011, 3, 5, 1, 0, 0, DateTimeKind.Unspecified);
            DateTime actual = zoned.ToDateTimeUnspecified();
            Assert.AreEqual(expected, actual);
            // Kind isn't checked by Equals...
            Assert.AreEqual(DateTimeKind.Unspecified, actual.Kind);
        }

        [Test]
        public void ToOffsetDateTime()
        {
            var local = new LocalDateTime(1911, 3, 5, 1, 0, 0); // Early interval
            var zoned = SampleZone.AtStrictly(local);
            var offsetDateTime = zoned.ToOffsetDateTime();
            Assert.AreEqual(local, offsetDateTime.LocalDateTime);
            Assert.AreEqual(SampleZone.EarlyInterval.WallOffset, offsetDateTime.Offset);
        }

        [Test]
        public void Equality()
        {
            // Goes back from 2am to 1am on June 13th
            SingleTransitionDateTimeZone zone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 4, 3);
            var sample = zone.MapLocal(new LocalDateTime(2011, 6, 13, 1, 30)).First();
            var fromUtc = Instant.FromUtc(2011, 6, 12, 21, 30).InZone(zone);

            // Checks all the overloads etc: first check is that the zone matters
            TestHelper.TestEqualsStruct(sample, fromUtc, Instant.FromUtc(2011, 6, 12, 21, 30).InUtc());
            TestHelper.TestOperatorEquality(sample, fromUtc, Instant.FromUtc(2011, 6, 12, 21, 30).InUtc());

            // Now just use a simple inequality check for other aspects...

            // Different offset
            var later = zone.MapLocal(new LocalDateTime(2011, 6, 13, 1, 30)).Last();
            Assert.AreEqual(sample.LocalDateTime, later.LocalDateTime);
            Assert.AreNotEqual(sample.Offset, later.Offset);
            Assert.AreNotEqual(sample, later);

            // Different local time
            Assert.AreNotEqual(sample, zone.MapLocal(new LocalDateTime(2011, 6, 13, 1, 29)).First());

            // Different calendar
            var withOtherCalendar = zone.MapLocal(new LocalDateTime(2011, 6, 13, 1, 30, CalendarSystem.Gregorian)).First();
            Assert.AreNotEqual(sample, withOtherCalendar);
        }

        [Test]
        public void Constructor_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(Instant.FromTicksSinceUnixEpoch(1000), null));
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(Instant.FromTicksSinceUnixEpoch(1000), null, CalendarSystem.Iso));
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(Instant.FromTicksSinceUnixEpoch(1000), SampleZone, null));
        }

        [Test]
        public void Construct_FromLocal_ValidUnambiguousOffset()
        {
            SingleTransitionDateTimeZone zone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 4, 3);

            LocalDateTime local = new LocalDateTime(2000, 1, 2, 3, 4, 5);
            ZonedDateTime zoned = new ZonedDateTime(local, zone, zone.EarlyInterval.WallOffset);
            Assert.AreEqual(zoned, local.InZoneStrictly(zone));
        }

        [Test]
        public void Construct_FromLocal_ValidEarlierOffset()
        {
            SingleTransitionDateTimeZone zone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 4, 3);

            LocalDateTime local = new LocalDateTime(2011, 6, 13, 1, 30);
            ZonedDateTime zoned = new ZonedDateTime(local, zone, zone.EarlyInterval.WallOffset);

            // Map the local time to the earlier of the offsets in a way which is tested elsewhere.
            var resolver = Resolvers.CreateMappingResolver(Resolvers.ReturnEarlier, Resolvers.ThrowWhenSkipped);
            Assert.AreEqual(zoned, local.InZone(zone, resolver));
        }

        [Test]
        public void Construct_FromLocal_ValidLaterOffset()
        {
            SingleTransitionDateTimeZone zone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 4, 3);

            LocalDateTime local = new LocalDateTime(2011, 6, 13, 1, 30);
            ZonedDateTime zoned = new ZonedDateTime(local, zone, zone.LateInterval.WallOffset);

            // Map the local time to the later of the offsets in a way which is tested elsewhere.
            var resolver = Resolvers.CreateMappingResolver(Resolvers.ReturnLater, Resolvers.ThrowWhenSkipped);
            Assert.AreEqual(zoned, local.InZone(zone, resolver));
        }

        [Test]
        public void Construct_FromLocal_InvalidOffset()
        {
            SingleTransitionDateTimeZone zone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 4, 3);

            // Attempt to ask for the later offset in the earlier interval
            LocalDateTime local = new LocalDateTime(2000, 1, 1, 0, 0, 0);
            Assert.Throws<ArgumentException>(() => new ZonedDateTime(local, zone, zone.LateInterval.WallOffset));
        }

        /// <summary>
        ///   Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new ZonedDateTime();
            Assert.AreEqual(new LocalDateTime(1, 1, 1, 0, 0), actual.LocalDateTime);
            Assert.AreEqual(Offset.Zero, actual.Offset);
            Assert.AreEqual(DateTimeZone.Utc, actual.Zone);
        }

        [Test]
        public void BinarySerialization_Iso()
        {
            DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Tzdb;
            var zone = DateTimeZoneProviders.Tzdb["America/New_York"];
            var value = new ZonedDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23).WithOffset(Offset.FromHours(-4)), zone);
            TestHelper.AssertBinaryRoundtrip(value);
        }

        [Test]
        public void XmlSerialization_Iso()
        {
            DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Tzdb;
            var zone = DateTimeZoneProviders.Tzdb["America/New_York"];
            var value = new ZonedDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23).WithOffset(Offset.FromHours(-4)), zone);
            TestHelper.AssertXmlRoundtrip(value, "<value zone=\"America/New_York\">2013-04-12T17:53:23-04</value>");
        }

#if !PCL
        [Test]
        public void XmlSerialization_Bcl()
        {
            // Skip this on Mono, which will have different BCL time zones. We can't easily
            // guess which will be available :(
            if (!TestHelper.IsRunningOnMono)
            {
                DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Bcl;
                var zone = DateTimeZoneProviders.Bcl["Eastern Standard Time"];
                var value = new ZonedDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23).WithOffset(Offset.FromHours(-4)), zone);
                TestHelper.AssertXmlRoundtrip(value, "<value zone=\"Eastern Standard Time\">2013-04-12T17:53:23-04</value>");
            }
        }

        [Test]
        public void BinarySerialization_Bcl()
        {
            // Skip this on Mono, which will have different BCL time zones. We can't easily
            // guess which will be available :(
            if (!TestHelper.IsRunningOnMono)
            {
                DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Bcl;
                var zone = DateTimeZoneProviders.Bcl["Eastern Standard Time"];
                var value = new ZonedDateTime(new LocalDateTime(2013, 4, 12, 17, 53, 23).WithOffset(Offset.FromHours(-4)), zone);
                TestHelper.AssertBinaryRoundtrip(value);
            }
        }
#endif

        [Test]
        public void XmlSerialization_NonIso()
        {
            DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Tzdb;
            var zone = DateTimeZoneProviders.Tzdb["America/New_York"];
            var localDateTime = new LocalDateTime(2013, 6, 12, 17, 53, 23, CalendarSystem.Julian);
            var value = new ZonedDateTime(localDateTime.WithOffset(Offset.FromHours(-4)), zone);
            TestHelper.AssertXmlRoundtrip(value,
                "<value zone=\"America/New_York\" calendar=\"Julian\">2013-06-12T17:53:23-04</value>");
        }

        [Test]
        public void BinarySerialization_NonIso()
        {
            DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Tzdb;
            var zone = DateTimeZoneProviders.Tzdb["America/New_York"];
            var localDateTime = new LocalDateTime(2013, 6, 12, 17, 53, 23, CalendarSystem.Julian);
            var value = new ZonedDateTime(localDateTime.WithOffset(Offset.FromHours(-4)), zone);
            TestHelper.AssertBinaryRoundtrip(value);
        }

        [Test]
        [TestCase("<value zone=\"America/New_York\" calendar=\"Rubbish\">2013-06-12T17:53:23-04</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-04-12T17:53:23-04</value>", typeof(ArgumentException), Description = "No zone")]
        [TestCase("<value zone=\"Unknown\">2013-04-12T17:53:23-04</value>", typeof(DateTimeZoneNotFoundException), Description = "Unknown zone")]
        [TestCase("<value zone=\"Europe/London\">2013-04-12T17:53:23-04</value>", typeof(UnparsableValueException), Description = "Incorrect offset")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            DateTimeZoneProviders.Serialization = DateTimeZoneProviders.Tzdb;
            TestHelper.AssertXmlInvalid<ZonedDateTime>(xml, expectedExceptionType);
        }

        [Test]
        public void ZonedDateTime_ToString()
        {
            var local = new LocalDateTime(2013, 7, 23, 13, 05, 20);
            ZonedDateTime zoned = local.InZoneStrictly(SampleZone);
            Assert.AreEqual("2013-07-23T13:05:20 Single (+04)", zoned.ToString());
        }

        [Test]
        public void ZonedDateTime_ToString_WithFormat()
        {
            var local = new LocalDateTime(2013, 7, 23, 13, 05, 20);
            ZonedDateTime zoned = local.InZoneStrictly(SampleZone);
            Assert.AreEqual("2013/07/23 13:05:20 Single", zoned.ToString("yyyy/MM/dd HH:mm:ss z", CultureInfo.InvariantCulture));
        }

        [Test]
        public void LocalComparer()
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var losAngeles = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];

            // LA is 8 hours behind London. So the London evening occurs before the LA afternoon.
            var londonEvening = new LocalDateTime(2014, 7, 9, 20, 32).InZoneStrictly(london);
            var losAngelesAfternoon = new LocalDateTime(2014, 7, 9, 14, 0).InZoneStrictly(losAngeles);

            // Same local time as losAngelesAfternoon
            var londonAfternoon = losAngelesAfternoon.LocalDateTime.InZoneStrictly(london);

            var londonPersian = londonEvening.LocalDateTime
                                             .WithCalendar(CalendarSystem.PersianSimple)
                                             .InZoneStrictly(london);

            TestHelper.TestComparerStruct(ZonedDateTime.Comparer.Local, losAngelesAfternoon, londonAfternoon, londonEvening);
            Assert.Throws<ArgumentException>(() => ZonedDateTime.Comparer.Local.Compare(londonPersian, londonEvening));
            Assert.IsFalse(ZonedDateTime.Comparer.Local.Equals(londonPersian, londonEvening));
            Assert.IsFalse(ZonedDateTime.Comparer.Local.Equals(londonAfternoon, londonEvening));
            Assert.IsTrue(ZonedDateTime.Comparer.Local.Equals(londonAfternoon, losAngelesAfternoon));
        }

        [Test]
        public void InstantComparer()
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            var losAngeles = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];

            // LA is 8 hours behind London. So the London evening occurs before the LA afternoon.
            var londonEvening = new LocalDateTime(2014, 7, 9, 20, 32).InZoneStrictly(london);
            var losAngelesAfternoon = new LocalDateTime(2014, 7, 9, 14, 0).InZoneStrictly(losAngeles);

            // Same instant as londonEvening
            var losAngelesLunchtime = new LocalDateTime(2014, 7, 9, 12, 32).InZoneStrictly(losAngeles);

            var londonPersian = londonEvening.LocalDateTime
                                             .WithCalendar(CalendarSystem.PersianSimple)
                                             .InZoneStrictly(london);

            TestHelper.TestComparerStruct(ZonedDateTime.Comparer.Instant, londonEvening, losAngelesLunchtime, losAngelesAfternoon);
            Assert.AreEqual(0, ZonedDateTime.Comparer.Instant.Compare(londonPersian, londonEvening));
            Assert.IsTrue(ZonedDateTime.Comparer.Instant.Equals(londonPersian, londonEvening));
            Assert.IsTrue(ZonedDateTime.Comparer.Instant.Equals(losAngelesLunchtime, londonEvening));
            Assert.IsFalse(ZonedDateTime.Comparer.Instant.Equals(losAngelesAfternoon, londonEvening));
        }
    }
}
