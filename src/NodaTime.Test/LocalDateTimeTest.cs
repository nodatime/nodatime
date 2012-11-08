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
using System.Globalization;
using NUnit.Framework;
using NodaTime.Calendars;
using NodaTime.TimeZones;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for <see cref="LocalDateTime" />.
    /// </summary>
    // TODO(Post-V1): We need more tests, desperately!
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
            LocalDateTime julianEpoch = isoEpoch.WithCalendar(CalendarSystem.GetJulianCalendar(4));
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

            Assert.AreEqual(new LocalDateTime(1970, 1, 1, 12, 5, 23), dateTime.TimeOfDay.LocalDateTime);
        }

        // Verifies that positive local instant ticks don't cause a problem with the date
        [Test]
        public void TimeOfDay_After1970()
        {
            LocalDateTime dateTime = new LocalDateTime(1975, 11, 8, 12, 5, 23);
            LocalTime expected = new LocalTime(12, 5, 23);
            Assert.AreEqual(expected, dateTime.TimeOfDay);

            Assert.AreEqual(new LocalDateTime(1970, 1, 1, 12, 5, 23), dateTime.TimeOfDay.LocalDateTime);
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
        public void CompareTo_DifferentCalendars_OnlyLocalInstantMatters()
        {
            CalendarSystem islamic = CalendarSystem.GetIslamicCalendar(IslamicLeapYearPattern.Base15, IslamicEpoch.Astronomical);
            LocalDateTime value1 = new LocalDateTime(2011, 1, 2, 10, 30);
            LocalDateTime value2 = new LocalDateTime(1500, 1, 1, 10, 30, islamic);
            LocalDateTime value3 = value1.WithCalendar(islamic);

            Assert.That(value1.CompareTo(value2), Is.LessThan(0));
            Assert.That(value2.CompareTo(value1), Is.GreaterThan(0));
            Assert.That(value1.CompareTo(value3), Is.EqualTo(0));
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
        /// so 2am became 1am. We'll return the later result, i.e. with the offset of -8
        /// </summary>
        [Test]
        public void InZoneLeniently_AmbiguousTime_ReturnsLaterMapping()
        {
            var local = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            var zoned = local.InZoneLeniently(Pacific);
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-8), zoned.Offset);
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2.30am doesn't exist on that day.
        /// We'll return 3am, the start of the second interval.
        /// </summary>
        [Test]
        public void InZoneLeniently_ReturnsStartOfSecondInterval()
        {
            var local = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            var zoned = local.InZoneLeniently(Pacific);
            Assert.AreEqual(new LocalDateTime(2009, 3, 8, 3, 0, 0), zoned.LocalDateTime);
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
    }
}
