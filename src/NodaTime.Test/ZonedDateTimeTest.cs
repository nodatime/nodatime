// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

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
        private static SingleTransitionDateTimeZone SampleZone = new SingleTransitionDateTimeZone(Instant.FromUtc(2011, 6, 12, 22, 0), 3, 4);

        [Test]
        public void SimpleProperties()
        {
            var value = SampleZone.AtStrictly(new LocalDateTime(2012, 2, 10, 8, 9, 10, 11, 12));
            Assert.AreEqual(Era.Common, value.Era);
            Assert.AreEqual(20, value.CenturyOfEra);
            Assert.AreEqual(12, value.YearOfCentury);
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
            ZonedDateTime afterAdd = ZonedDateTime.Add(before, Duration.OneStandardDay);
            ZonedDateTime afterOperator = before + Duration.OneStandardDay;

            Assert.AreEqual(afterExpected, afterAdd);
            Assert.AreEqual(afterExpected, afterOperator);
        }

        [Test]
        public void Add_MethodEquivalents()
        {
            ZonedDateTime before = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 12, 15, 0));
            Assert.AreEqual(before + Duration.OneStandardDay, ZonedDateTime.Add(before, Duration.OneStandardDay));
            Assert.AreEqual(before + Duration.OneStandardDay, before.Plus(Duration.OneStandardDay));
        }

        [Test]
        public void Subtract_AroundTimeZoneTransition()
        {
            // After the transition at 4pm...
            ZonedDateTime after = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 13, 16, 0));
            // 24 hours earlier, and it's 3pm
            ZonedDateTime beforeExpected = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 12, 15, 0));
            ZonedDateTime beforeSubtract = ZonedDateTime.Subtract(after, Duration.OneStandardDay);
            ZonedDateTime beforeOperator = after - Duration.OneStandardDay;

            Assert.AreEqual(beforeExpected, beforeSubtract);
            Assert.AreEqual(beforeExpected, beforeOperator);
        }

        [Test]
        public void Subtract_MethodEquivalents()
        {
            ZonedDateTime after = SampleZone.AtStrictly(new LocalDateTime(2011, 6, 13, 16, 0));
            Assert.AreEqual(after - Duration.OneStandardDay, ZonedDateTime.Subtract(after, Duration.OneStandardDay));
            Assert.AreEqual(after - Duration.OneStandardDay, after.Minus(Duration.OneStandardDay));
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
            var withOtherCalendar = zone.MapLocal(new LocalDateTime(2011, 6, 13, 1, 30, CalendarSystem.GetGregorianCalendar(4))).First();
            Assert.AreNotEqual(sample, withOtherCalendar);
        }

        [Test]
        public void ComparisonOperators_SameCalendarAndZone()
        {
            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value2 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value3 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 45, 0));

            Assert.IsFalse(value1 < value2);
            Assert.IsTrue(value1 < value3);
            Assert.IsFalse(value2 < value1);
            Assert.IsFalse(value3 < value1);

            Assert.IsTrue(value1 <= value2);
            Assert.IsTrue(value1 <= value3);
            Assert.IsTrue(value2 <= value1);
            Assert.IsFalse(value3 <= value1);

            Assert.IsFalse(value1 > value2);
            Assert.IsFalse(value1 > value3);
            Assert.IsFalse(value2 > value1);
            Assert.IsTrue(value3 > value1);

            Assert.IsTrue(value1 >= value2);
            Assert.IsFalse(value1 >= value3);
            Assert.IsTrue(value2 >= value1);
            Assert.IsTrue(value3 >= value1);
        }

        [Test]
        public void ComparisonOperators_DifferentCalendars_AlwaysReturnsFalse()
        {
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(2011, 1, 3, 10, 30, CalendarSystem.GetJulianCalendar(4));

            // All inequality comparisons return false
            Assert.IsFalse(value1 < value2);
            Assert.IsFalse(value1 <= value2);
            Assert.IsFalse(value1 > value2);
            Assert.IsFalse(value1 >= value2);
        }

        [Test]
        public void ComparisonOperators_DifferentZones_AlwaysReturnsFalse()
        {
            // Note that the offsets will be the same as for SampleZone in the values we're using
            var otherZone = new FixedDateTimeZone(SampleZone.EarlyInterval.WallOffset);

            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            ZonedDateTime value2 = otherZone.AtStrictly(new LocalDateTime(2011, 1, 3, 10, 30));

            // All inequality comparisons return false
            Assert.IsFalse(value1 < value2);
            Assert.IsFalse(value1 <= value2);
            Assert.IsFalse(value1 > value2);
            Assert.IsFalse(value1 >= value2);
        }

        [Test]
        public void CompareTo_SameCalendarAndZone()
        {
            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value2 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value3 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 45, 0));

            Assert.That(value1.CompareTo(value2), Is.EqualTo(0));
            Assert.That(value1.CompareTo(value3), Is.LessThan(0));
            Assert.That(value3.CompareTo(value2), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_DifferentCalendars_OnlyInstantMatters()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            ZonedDateTime value2 = SampleZone.AtStrictly(new LocalDateTime(1500, 1, 1, 10, 30, islamic));
            ZonedDateTime value3 = SampleZone.AtStrictly(value1.LocalDateTime.WithCalendar(islamic));

            Assert.That(value1.CompareTo(value2), Is.LessThan(0));
            Assert.That(value2.CompareTo(value1), Is.GreaterThan(0));
            Assert.That(value1.CompareTo(value3), Is.EqualTo(0));
        }

        [Test]
        public void CompareTo_DifferentZones_OnlyInstantMatters()
        {
            var otherZone = new FixedDateTimeZone(Offset.FromHours(-20));

            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            // Earlier local time, but later instant
            ZonedDateTime value2 = otherZone.AtStrictly(new LocalDateTime(2011, 1, 2, 5, 30));
            ZonedDateTime value3 = value1.WithZone(otherZone);

            Assert.That(value1.CompareTo(value2), Is.LessThan(0));
            Assert.That(value2.CompareTo(value1), Is.GreaterThan(0));
            Assert.That(value1.CompareTo(value3), Is.EqualTo(0));
        }

        [Test]
        public void IComparableCompareTo_SameCalendarAndZone()
        {
            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value2 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30, 0));
            ZonedDateTime value3 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 45, 0));

            IComparable i_value1 = (IComparable)value1;
            IComparable i_value3 = (IComparable)value3;

            Assert.That(i_value1.CompareTo(value2), Is.EqualTo(0));
            Assert.That(i_value1.CompareTo(value3), Is.LessThan(0));
            Assert.That(i_value3.CompareTo(value2), Is.GreaterThan(0));
        }

        [Test]
        public void IComparableCompareTo_DifferentCalendars_OnlyInstantMatters()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            ZonedDateTime value2 = SampleZone.AtStrictly(new LocalDateTime(1500, 1, 1, 10, 30, islamic));
            ZonedDateTime value3 = SampleZone.AtStrictly(value1.LocalDateTime.WithCalendar(islamic));

            IComparable i_value1 = (IComparable)value1;
            IComparable i_value2 = (IComparable)value2;

            Assert.That(i_value1.CompareTo(value2), Is.LessThan(0));
            Assert.That(i_value2.CompareTo(value1), Is.GreaterThan(0));
            Assert.That(i_value1.CompareTo(value3), Is.EqualTo(0));
        }

        [Test]
        public void IComparableCompareTo_DifferentZones_OnlyInstantMatters()
        {
            var otherZone = new FixedDateTimeZone(Offset.FromHours(-20));

            ZonedDateTime value1 = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            // Earlier local time, but later instant
            ZonedDateTime value2 = otherZone.AtStrictly(new LocalDateTime(2011, 1, 2, 5, 30));
            ZonedDateTime value3 = value1.WithZone(otherZone);

            IComparable i_value1 = (IComparable)value1;
            IComparable i_value2 = (IComparable)value2;

            Assert.That(i_value1.CompareTo(value2), Is.LessThan(0));
            Assert.That(i_value2.CompareTo(value1), Is.GreaterThan(0));
            Assert.That(i_value1.CompareTo(value3), Is.EqualTo(0));
        }

        /// <summary>
        /// IComparable.CompareTo returns a positive number for a null input.
        /// </summary>
        [Test]
        public void IComparableCompareTo_Null_Positive()
        {
            var instance = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            var i_instance = (IComparable)instance;
            object arg = null;
            var result = i_instance.CompareTo(arg);
            Assert.That(result, Is.GreaterThan(0));
        }

        /// <summary>
        /// IComparable.CompareTo throws an ArgumentException for non-null arguments 
        /// that are not a ZonedDateTime.
        /// </summary>
        [Test]
        public void IComparableCompareTo_WrongType_ArgumentException()
        {
            var instance = SampleZone.AtStrictly(new LocalDateTime(2011, 1, 2, 10, 30));
            var i_instance = (IComparable)instance;
            var arg = new LocalDate(2012, 3, 6);
            Assert.Throws<ArgumentException>(() =>
            {
                var result = i_instance.CompareTo(arg);
            });
        }

        [Test]
        public void Constructor_ArgumentValidation()
        {
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(new Instant(1000), null));
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(new Instant(1000), null, CalendarSystem.Iso));
            Assert.Throws<ArgumentNullException>(() => new ZonedDateTime(new Instant(1000), SampleZone, null));
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
            // Leniently will return the later instant
            Assert.AreEqual(zoned, local.InZoneLeniently(zone));
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
            Assert.AreEqual(NodaConstants.UnixEpoch.InUtc().LocalDateTime, actual.LocalDateTime);
            Assert.AreEqual(Offset.Zero, actual.Offset);
            Assert.AreEqual(DateTimeZone.Utc, actual.Zone);
        }

    }
}
