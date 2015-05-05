// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using NodaTime.Calendars;
using NodaTime.Text;
using NodaTime.TimeZones;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for <see cref="LocalDateTime" />.
    /// </summary>
    [TestFixture]
    public partial class LocalDateTimeTest
    {
        private static readonly DateTimeZone Pacific = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];

        [Test]
        public void ToDateTimeUnspecified()
        {
            LocalDateTime zoned = new LocalDateTime(2011, 3, 5, 1, 0, 0);
            DateTime expected = new DateTime(2011, 3, 5, 1, 0, 0, DateTimeKind.Unspecified);
            DateTime actual = zoned.ToDateTimeUnspecified();
            Assert.AreEqual(expected, actual);
            // Kind isn't checked by Equals...
            Assert.AreEqual(DateTimeKind.Unspecified, actual.Kind);
        }

        [Test]
        public void FromDateTime()
        {
            LocalDateTime expected = new LocalDateTime(2011, 08, 18, 20, 53);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                LocalDateTime actual = LocalDateTime.FromDateTime(x);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void FromDateTime_WithCalendar()
        {
            // Julian calendar is 13 days behind Gregorian calendar in the 21st century
            LocalDateTime expected = new LocalDateTime(2011, 08, 05, 20, 53, CalendarSystem.Julian);
            foreach (DateTimeKind kind in Enum.GetValues(typeof(DateTimeKind)))
            {
                DateTime x = new DateTime(2011, 08, 18, 20, 53, 0, kind);
                LocalDateTime actual = LocalDateTime.FromDateTime(x, CalendarSystem.Julian);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TimeProperties_AfterEpoch()
        {
            // Use the largest valid year as part of validating against overflow
            LocalDateTime ldt = new LocalDateTime(GregorianYearMonthDayCalculator.MaxGregorianYear, 1, 2, 15, 48, 25, 456, 3456);
            Assert.AreEqual(15, ldt.Hour);
            Assert.AreEqual(3, ldt.ClockHourOfHalfDay);
            Assert.AreEqual(48, ldt.Minute);
            Assert.AreEqual(25, ldt.Second);
            Assert.AreEqual(456, ldt.Millisecond);
            Assert.AreEqual(4563456, ldt.TickOfSecond);
            Assert.AreEqual(15 * NodaConstants.TicksPerHour + 
                            48 * NodaConstants.TicksPerMinute +
                            25 * NodaConstants.TicksPerSecond +
                            4563456, ldt.TickOfDay);
        }

        [Test]
        public void TimeProperties_BeforeEpoch()
        {
            // Use the smallest valid year number as part of validating against overflow
            LocalDateTime ldt = new LocalDateTime(GregorianYearMonthDayCalculator.MinGregorianYear, 1, 2, 15, 48, 25, 456, 3456);
            Assert.AreEqual(15, ldt.Hour);
            Assert.AreEqual(3, ldt.ClockHourOfHalfDay);
            Assert.AreEqual(48, ldt.Minute);
            Assert.AreEqual(25, ldt.Second);
            Assert.AreEqual(456, ldt.Millisecond);
            Assert.AreEqual(4563456, ldt.TickOfSecond);
            Assert.AreEqual(15 * NodaConstants.TicksPerHour +
                            48 * NodaConstants.TicksPerMinute +
                            25 * NodaConstants.TicksPerSecond +
                            4563456, ldt.TickOfDay);
        }

        [Test]
        public void DateTime_Roundtrip_OtherCalendarInBcl()
        {
            DateTime original = new DateTime(1376, 6, 19, new HijriCalendar());
            LocalDateTime noda = LocalDateTime.FromDateTime(original);
            // The DateTime only knows about the ISO version...
            Assert.AreNotEqual(1376, noda.Year);
            Assert.AreEqual(CalendarSystem.Iso, noda.Calendar);
            DateTime final = noda.ToDateTimeUnspecified();
            Assert.AreEqual(original, final);
        }

        [Test]
        public void WithCalendar()
        {
            LocalDateTime isoEpoch = new LocalDateTime(1970, 1, 1, 0, 0, 0);
            LocalDateTime julianEpoch = isoEpoch.WithCalendar(CalendarSystem.Julian);
            Assert.AreEqual(1969, julianEpoch.Year);
            Assert.AreEqual(12, julianEpoch.Month);
            Assert.AreEqual(19, julianEpoch.Day);
            Assert.AreEqual(isoEpoch.TimeOfDay, julianEpoch.TimeOfDay);
        }

        // Verifies that negative local instant ticks don't cause a problem with the date
        [Test]
        public void TimeOfDay_Before1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1965, 11, 8, 12, 5, 23);
            LocalTime expected = new LocalTime(12, 5, 23);
            Assert.AreEqual(expected, dateTime.TimeOfDay);
        }

        // Verifies that positive local instant ticks don't cause a problem with the date
        [Test]
        public void TimeOfDay_After1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1975, 11, 8, 12, 5, 23);
            LocalTime expected = new LocalTime(12, 5, 23);
            Assert.AreEqual(expected, dateTime.TimeOfDay);
        }

        // Verifies that negative local instant ticks don't cause a problem with the date
        [Test]
        public void Date_Before1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1965, 11, 8, 12, 5, 23);
            LocalDate expected = new LocalDate(1965, 11, 8);
            Assert.AreEqual(expected, dateTime.Date);
        }

        // Verifies that positive local instant ticks don't cause a problem with the date
        [Test]
        public void Date_After1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1975, 11, 8, 12, 5, 23);
            LocalDate expected = new LocalDate(1975, 11, 8);
            Assert.AreEqual(expected, dateTime.Date);
        }

        [Test]
        public void IsoDayOfWeek_AroundEpoch()
        {
            // Test about couple of months around the Unix epoch. If that works, I'm confident the rest will.
            LocalDateTime dateTime = new LocalDateTime(1969, 12, 1, 0, 0);
            for (int i = 0; i < 60; i++)
            {
                // Check once per hour of the day, just in case something's messed up based on the time of day.
                for (int hour = 0; hour < 24; hour++)
                {
                    Assert.AreEqual(BclConversions.ToIsoDayOfWeek(dateTime.ToDateTimeUnspecified().DayOfWeek),
                        dateTime.IsoDayOfWeek);
                    dateTime = dateTime.PlusHours(1);
                }
            }
        }

        [Test]
        public void ClockHourOfHalfDay()
        {
            Assert.AreEqual(12, new LocalDateTime(1975, 11, 8, 0, 0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalDateTime(1975, 11, 8, 1, 0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(12, new LocalDateTime(1975, 11, 8, 12, 0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalDateTime(1975, 11, 8, 13, 0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(11, new LocalDateTime(1975, 11, 8, 23, 0, 0).ClockHourOfHalfDay);
        }

        [Test]
        public void ComparisonOperators_SameCalendar()
        {
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30, 0);
            LocalDateTime value2 = new LocalDateTime(2011, 1, 2, 10, 30, 0);
            LocalDateTime value3 = new LocalDateTime(2011, 1, 2, 10, 45, 0);

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
        public void ComparisonOperators_DifferentCalendars_Throws()
        {
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(2011, 1, 3, 10, 30, CalendarSystem.Julian);

            Assert.Throws<ArgumentException>(() => (value1 < value2).ToString());
            Assert.Throws<ArgumentException>(() => (value1 <= value2).ToString());
            Assert.Throws<ArgumentException>(() => (value1 > value2).ToString());
            Assert.Throws<ArgumentException>(() => (value1 >= value2).ToString());
        }

        [Test]
        public void CompareTo_SameCalendar()
        {
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value3 = new LocalDateTime(2011, 1, 2, 10, 45);

            Assert.That(value1.CompareTo(value2), Is.EqualTo(0));
            Assert.That(value1.CompareTo(value3), Is.LessThan(0));
            Assert.That(value3.CompareTo(value2), Is.GreaterThan(0));
        }

        [Test]
        public void CompareTo_DifferentCalendars_Throws()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(1500, 1, 1, 10, 30, islamic);

            Assert.Throws<ArgumentException>(() => value1.CompareTo(value2));
            Assert.Throws<ArgumentException>(() => ((IComparable)value1).CompareTo(value2));
        }

        /// <summary>
        /// IComparable.CompareTo works properly for LocalDateTime inputs with different calendars.
        /// </summary>
        [Test]
        public void IComparableCompareTo_SameCalendar()
        {
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value3 = new LocalDateTime(2011, 1, 2, 10, 45);

            IComparable i_value1 = (IComparable)value1;
            IComparable i_value3 = (IComparable)value3;

            Assert.That(i_value1.CompareTo(value2), Is.EqualTo(0));
            Assert.That(i_value1.CompareTo(value3), Is.LessThan(0));
            Assert.That(i_value3.CompareTo(value2), Is.GreaterThan(0));
        }

        /// <summary>
        /// IComparable.CompareTo returns a positive number for a null input.
        /// </summary>
        [Test]
        public void IComparableCompareTo_Null_Positive()
        {
            var instance = new LocalDateTime(2012, 3, 5, 10, 45);
            var i_instance = (IComparable)instance;
            object arg = null;
            var result = i_instance.CompareTo(arg);
            Assert.That(result, Is.GreaterThan(0));
        }

        /// <summary>
        /// IComparable.CompareTo throws an ArgumentException for non-null arguments 
        /// that are not a LocalDateTime.
        /// </summary>
        [Test]
        public void IComparableCompareTo_WrongType_ArgumentException()
        {
            var instance = new LocalDateTime(2012, 3, 5, 10, 45);
            var i_instance = (IComparable)instance;
            var arg = new LocalDate(2012, 3, 6);
            Assert.Throws<ArgumentException>(() => i_instance.CompareTo(arg));
        }

        [Test]
        public void WithOffset()
        {
            var offset = Offset.FromHoursAndMinutes(5, 10);
            var localDateTime = new LocalDateTime(2009, 12, 22, 21, 39, 30);
            var offsetDateTime = localDateTime.WithOffset(offset);
            Assert.AreEqual(localDateTime, offsetDateTime.LocalDateTime);
            Assert.AreEqual(offset, offsetDateTime.Offset);
        }

        [Test]
        public void InUtc()
        {
            var local = new LocalDateTime(2009, 12, 22, 21, 39, 30);
            var zoned = local.InUtc();
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.Zero, zoned.Offset);
            Assert.AreSame(DateTimeZone.Utc, zoned.Zone);
        }

        [Test]
        public void InZoneStrictly_InWinter()
        {
            var local = new LocalDateTime(2009, 12, 22, 21, 39, 30);
            var zoned = local.InZoneStrictly(Pacific);
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-8), zoned.Offset);
        }

        [Test]
        public void InZoneStrictly_InSummer()
        {
            var local = new LocalDateTime(2009, 6, 22, 21, 39, 30);
            var zoned = local.InZoneStrictly(Pacific);
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-7), zoned.Offset);
        }

        /// <summary>
        /// Pacific time changed from -7 to -8 at 2am wall time on November 2nd 2009,
        /// so 2am became 1am.
        /// </summary>
        [Test]
        public void InZoneStrictly_ThrowsWhenAmbiguous()
        {
            var local = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            Assert.Throws<AmbiguousTimeException>(() => local.InZoneStrictly(Pacific));
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2.30am doesn't exist on that day.
        /// </summary>
        [Test]
        public void InZoneStrictly_ThrowsWhenSkipped()
        {
            var local = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            Assert.Throws<SkippedTimeException>(() => local.InZoneStrictly(Pacific));
        }

        /// <summary>
        /// Pacific time changed from -7 to -8 at 2am wall time on November 2nd 2009,
        /// so 2am became 1am. We'll return the earlier result, i.e. with the offset of -7
        /// </summary>
        [Test]
        public void InZoneLeniently_AmbiguousTime_ReturnsEarlierMapping()
        {
            var local = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            var zoned = local.InZoneLeniently(Pacific);
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-7), zoned.Offset);
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2:30am doesn't exist on that day.
        /// We'll return 3:30am, the forward-shifted value.
        /// </summary>
        [Test]
        public void InZoneLeniently_ReturnsStartOfSecondInterval()
        {
            var local = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            var zoned = local.InZoneLeniently(Pacific);
            Assert.AreEqual(new LocalDateTime(2009, 3, 8, 3, 30, 0), zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-7), zoned.Offset);
        }

        [Test]
        public void InZone()
        {
            // Don't need much for this - it only delegates.
            var ambiguous = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            var skipped = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            Assert.AreEqual(Pacific.AtLeniently(ambiguous), ambiguous.InZone(Pacific, Resolvers.LenientResolver));
            Assert.AreEqual(Pacific.AtLeniently(skipped), skipped.InZone(Pacific, Resolvers.LenientResolver));
        }

        /// <summary>
        ///   Using the default constructor is equivalent to January 1st 1970, midnight, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalDateTime();
            Assert.AreEqual(new LocalDateTime(1, 1, 1, 0, 0), actual);
        }

        [Test]
        public void XmlSerialization_Iso()
        {
            var value = new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567);
            TestHelper.AssertXmlRoundtrip(value, "<value>2013-04-12T17:53:23.1234567</value>");
        }

        [Test]
        public void BinarySerialization()
        {
            TestHelper.AssertBinaryRoundtrip(new LocalDateTime(2013, 4, 12, 17, 53, 23, CalendarSystem.Julian));
            TestHelper.AssertBinaryRoundtrip(new LocalDateTime(2013, 4, 12, 17, 53, 23, 123, 4567));
        }

        [Test]
        public void XmlSerialization_NonIso()
        {
            var value = new LocalDateTime(2013, 4, 12, 17, 53, 23, CalendarSystem.Julian);
            TestHelper.AssertXmlRoundtrip(value, "<value calendar=\"Julian\">2013-04-12T17:53:23</value>");
        }

        [Test]
        [TestCase("<value calendar=\"Rubbish\">2013-06-12T17:53:23</value>", typeof(KeyNotFoundException), Description = "Unknown calendar system")]
        [TestCase("<value>2013-15-12T17:53:23</value>", typeof(UnparsableValueException), Description = "Invalid month")]
        public void XmlSerialization_Invalid(string xml, Type expectedExceptionType)
        {
            TestHelper.AssertXmlInvalid<LocalDateTime>(xml, expectedExceptionType);
        }
    }
}
