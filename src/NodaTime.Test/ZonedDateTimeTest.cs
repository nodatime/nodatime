#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
        private static readonly IDateTimeZone Pacific = DateTimeZones.ForId("America/Los_Angeles");

        [Test]
        public void Constructor_SpecifyingDateAndTimeToMinutesInWinter()
        {
            var when = new ZonedDateTime(2009, 12, 22, 21, 39, 30, Pacific);
            Instant instant = when.ToInstant();
            LocalInstant localInstant = when.LocalInstant;
            Assert.AreEqual(instant, localInstant - Offset.ForHours(-8));

            Assert.AreEqual(2009, when.Year);
            Assert.AreEqual(12, when.MonthOfYear);
            Assert.AreEqual(22, when.DayOfMonth);
            Assert.AreEqual(21, when.HourOfDay);
            Assert.AreEqual(39, when.MinuteOfHour);
            Assert.AreEqual(30, when.SecondOfMinute);
        }

        [Test]
        public void Constructor_SpecifyingDateAndTimeToMinutesInSummer()
        {
            var when = new ZonedDateTime(2009, 6, 22, 21, 39, 30, Pacific);
            Instant instant = when.ToInstant();
            LocalInstant localInstant = when.LocalInstant;
            Assert.AreEqual(instant, localInstant - Offset.ForHours(-7));

            Assert.AreEqual(2009, when.Year);
            Assert.AreEqual(6, when.MonthOfYear);
            Assert.AreEqual(22, when.DayOfMonth);
            Assert.AreEqual(21, when.HourOfDay);
            Assert.AreEqual(39, when.MinuteOfHour);
            Assert.AreEqual(30, when.SecondOfMinute);
        }

        /// <summary>
        /// Pacific time changed from -7 to -8 at 2am wall time on November 2nd 2009,
        /// so 2am became 1am. Using the constructor of ZonedDateTime, we should get
        /// the *later* version, i.e. when the offset is 8 hours. The instant should
        /// therefore represent 09:30 UTC.
        /// </summary>
        [Test]
        public void Constructor_WithAmbiguousTime_UsesLaterInstant()
        {
            var when = new ZonedDateTime(2009, 11, 2, 1, 30, 0, Pacific);
            Instant instant = when.ToInstant();
            LocalInstant localInstant = when.LocalInstant;
            Assert.AreEqual(localInstant - Offset.ForHours(-8), instant);

            Assert.AreEqual(2009, when.Year);
            Assert.AreEqual(11, when.MonthOfYear);
            Assert.AreEqual(2, when.DayOfMonth);
            Assert.AreEqual(1, when.HourOfDay);
            Assert.AreEqual(30, when.MinuteOfHour);
            Assert.AreEqual(0, when.SecondOfMinute);

            var utc = new LocalDateTime(new LocalInstant(instant.Ticks));
            Assert.AreEqual(2009, utc.Year);
            Assert.AreEqual(11, utc.MonthOfYear);
            Assert.AreEqual(2, utc.DayOfMonth);
            Assert.AreEqual(9, utc.HourOfDay);
            Assert.AreEqual(30, utc.MinuteOfHour);
            Assert.AreEqual(0, utc.SecondOfMinute);
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2.30am doesn't exist on that day.
        /// </summary>
        [Test]
        public void Constructor_WithImpossibleTime_ThrowsException()
        {
            Assert.Throws<SkippedTimeException>(() => new ZonedDateTime(2009, 3, 8, 2, 30, 0, Pacific));
        }
    }
}