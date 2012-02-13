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

using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for aspects of DateTimeZone to do with converting from LocalDateTime and 
    /// LocalDate to ZonedDateTime.
    /// </summary>
    // TODO(Post-V1): Fix all tests to use SingleTransitionZone.
    public partial class DateTimeZoneTest
    {
        // Sample time zones for DateTimeZone.AtStartOfDay etc. I didn't want to only test midnight transitions.
        private static readonly DateTimeZone LosAngeles = DateTimeZone.ForId("America/Los_Angeles");
        private static readonly DateTimeZone NewZealand = DateTimeZone.ForId("Pacific/Auckland");
        private static readonly DateTimeZone Paris = DateTimeZone.ForId("Europe/Paris");
        private static readonly DateTimeZone NewYork = DateTimeZone.ForId("America/New_York");

        /// <summary>
        /// Local midnight at the start of the transition (June 1st) becomes 1am.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardAtMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 2, 0), Offset.FromHours(-2), Offset.FromHours(-1));

        /// <summary>
        /// Local 1am at the start of the transition (June 1st) becomes midnight.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardToMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 3, 0), Offset.FromHours(-2), Offset.FromHours(-3));

        /// <summary>
        /// Local 11.20pm at the start of the transition (May 30th) becomes 12.20am of June 1st.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardBeforeMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 1, 20), Offset.FromHours(-2), Offset.FromHours(-1));

        /// <summary>
        /// Local 12.20am at the start of the transition (June 1st) becomes 11.20pm of the previous day.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardAfterMidnightZone =
            new SingleTransitionZone(Instant.FromUtc(2000, 6, 1, 2, 20), Offset.FromHours(-2), Offset.FromHours(-3));

        private static readonly LocalDate TransitionDate = new LocalDate(2000, 6, 1);

        [Test]
        public void AmbiguousStartOfDay_TransitionAtMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 0), Offset.FromHours(-2),
                TransitionBackwardToMidnightZone);
            var actual = TransitionBackwardToMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AmbiguousStartOfDay_TransitionAfterMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 0), Offset.FromHours(-2),
                TransitionBackwardAfterMidnightZone);
            var actual = TransitionBackwardAfterMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionAtMidnight()
        {
            // 1am because of the skip
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 1, 0), Offset.FromHours(-1),
                TransitionForwardAtMidnightZone);
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionBeforeMidnight()
        {
            // 12.20am because of the skip
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 20), Offset.FromHours(-1),
                TransitionForwardBeforeMidnightZone);
            var actual = TransitionForwardBeforeMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnambiguousStartOfDay()
        {
            // Just a simple midnight in March.
            var expected = new ZonedDateTime(new LocalDateTime(2000, 3, 1, 0, 0), Offset.FromHours(-2),
                TransitionForwardAtMidnightZone);
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(new LocalDate(2000, 3, 1));
            Assert.AreEqual(expected, actual);
        }

        private static void AssertImpossible(LocalDateTime localTime, DateTimeZone zone)
        {
            try
            {
                zone.AtExactly(localTime);
                Assert.Fail("Expected exception");
            }
            catch (SkippedTimeException e)
            {
                Assert.AreEqual(localTime, e.LocalDateTime);
                Assert.AreEqual(zone, e.Zone);
            }
        }

        private static void AssertAmbiguous(LocalDateTime localTime, DateTimeZone zone)
        {
            ZonedDateTime earlier = zone.AtEarlier(localTime);
            ZonedDateTime later = zone.AtLater(localTime);
            Assert.AreEqual(localTime, earlier.LocalDateTime);
            Assert.AreEqual(localTime, later.LocalDateTime);
            Assert.That(earlier.ToInstant(), Is.LessThan(later.ToInstant()));

            try
            {
                zone.AtExactly(localTime);
                Assert.Fail("Expected exception");
            }
            catch (AmbiguousTimeException e)
            {
                Assert.AreEqual(localTime, e.LocalDateTime);
                Assert.AreEqual(zone, e.Zone);
                Assert.AreEqual(earlier, e.EarlierMapping);
                Assert.AreEqual(later, e.LaterMapping);
            }
        }

        private static void AssertOffset(int expectedHours, LocalDateTime localTime, DateTimeZone zone)
        {
            var zoned = zone.AtExactly(localTime);
            int actualHours = zoned.Offset.TotalMilliseconds / NodaConstants.MillisecondsPerHour;
            Assert.AreEqual(expectedHours, actualHours);
        }

        // Los Angeles goes from -7 to -8 on November 7th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_LosAngelesFallTransition()
        {
            var before = new LocalDateTime(2010, 11, 7, 0, 30);
            var atTransition = new LocalDateTime(2010, 11, 7, 1, 0);
            var ambiguous = new LocalDateTime(2010, 11, 7, 1, 30);
            var after = new LocalDateTime(2010, 11, 7, 2, 30);
            AssertOffset(-7, before, LosAngeles);
            AssertAmbiguous(atTransition, LosAngeles);
            AssertAmbiguous(ambiguous, LosAngeles);
            AssertOffset(-8, after, LosAngeles);
        }

        [Test]
        public void GetOffsetFromLocal_LosAngelesSpringTransition()
        {
            var before = new LocalDateTime(2010, 3, 14, 1, 30);
            var impossible = new LocalDateTime(2010, 3, 14, 2, 30);
            var atTransition = new LocalDateTime(2010, 3, 14, 3, 0);
            var after = new LocalDateTime(2010, 3, 14, 3, 30);
            AssertOffset(-8, before, LosAngeles);
            AssertImpossible(impossible, LosAngeles);
            AssertOffset(-7, atTransition, LosAngeles);
            AssertOffset(-7, after, LosAngeles);
        }

        // New Zealand goes from +13 to +12 on April 4th 2010 at 3am wall time
        [Test]
        public void GetOffsetFromLocal_NewZealandFallTransition()
        {
            var before = new LocalDateTime(2010, 4, 4, 1, 30);
            var atTransition = new LocalDateTime(2010, 4, 4, 2, 0);
            var ambiguous = new LocalDateTime(2010, 4, 4, 2, 30);
            var after = new LocalDateTime(2010, 4, 4, 3, 30);
            AssertOffset(+13, before, NewZealand);
            AssertAmbiguous(atTransition, NewZealand);
            AssertAmbiguous(ambiguous, NewZealand);
            AssertOffset(+12, after, NewZealand);
        }

        // New Zealand goes from +12 to +13 on September 26th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_NewZealandSpringTransition()
        {
            var before = new LocalDateTime(2010, 9, 26, 1, 30);
            var impossible = new LocalDateTime(2010, 9, 26, 2, 30);
            var atTransition = new LocalDateTime(2010, 9, 26, 3, 0);
            var after = new LocalDateTime(2010, 9, 26, 3, 30);
            AssertOffset(+12, before, NewZealand);
            AssertImpossible(impossible, NewZealand);
            AssertOffset(+13, atTransition, NewZealand);
            AssertOffset(+13, after, NewZealand);
        }

        // Paris goes from +1 to +2 on March 28th 2010 at 2am wall time
        [Test]
        public void GetOffsetFromLocal_ParisFallTransition()
        {
            var before = new LocalDateTime(2010, 10, 31, 1, 30);
            var atTransition = new LocalDateTime(2010, 10, 31, 2, 0);
            var ambiguous = new LocalDateTime(2010, 10, 31, 2, 30);
            var after = new LocalDateTime(2010, 10, 31, 3, 30);
            AssertOffset(2, before, Paris);
            AssertAmbiguous(ambiguous, Paris);
            AssertAmbiguous(atTransition, Paris);
            AssertOffset(1, after, Paris);
        }

        [Test]
        public void GetOffsetFromLocal_ParisSpringTransition()
        {
            var before = new LocalDateTime(2010, 3, 28, 1, 30);
            var impossible = new LocalDateTime(2010, 3, 28, 2, 30);
            var atTransition = new LocalDateTime(2010, 3, 28, 3, 0);
            var after = new LocalDateTime(2010, 3, 28, 3, 30);
            AssertOffset(1, before, Paris);
            AssertImpossible(impossible, Paris);
            AssertOffset(2, atTransition, Paris);
            AssertOffset(2, after, Paris);
        }

        [Test]
        public void MapLocalDateTime_UnambiguousDateReturnsUnambiguousMapping()
        {
            //2011-11-09 01:30:00 - not ambiguous in America/New York timezone
            var unambigiousTime = new LocalDateTime(2011, 11, 9, 1, 30); 
            var actual = NewYork.MapLocalDateTime(unambigiousTime).Type;

            Assert.AreEqual(ZoneLocalMapping.ResultType.Unambiguous, actual);
        }

        [Test]
        public void MapLocalDateTime_AmbiguousDateReturnsAmbigousMapping()
        {
            //2011-11-06 01:30:00 - falls during DST - EST conversion in America/New York timezone
            var ambiguousTime = new LocalDateTime(2011, 11, 6, 1, 30); 
            var actual = NewYork.MapLocalDateTime(ambiguousTime).Type;

            Assert.AreEqual(ZoneLocalMapping.ResultType.Ambiguous, actual);
        }

        [Test]
        public void MapLocalDateTime_SkippedDateReturnsSkippedMapping()
        {
            //2011-03-13 02:30:00 - falls during EST - DST conversion in America/New York timezone
            var skippedTime = new LocalDateTime(2011, 3, 13, 2, 30); 
            var actual = NewYork.MapLocalDateTime(skippedTime).Type;

            Assert.AreEqual(ZoneLocalMapping.ResultType.Skipped, actual);
        }

        // Samoa (Pacific/Apia) skipped December 30th 2011, going from
        // 23:59:59 December 29th local time UTC-10
        // 00:00:00 December 31st local time UTC+14
        [Test]
        public void AtStartOfDay_DayDoesntExist()
        {
            LocalDate badDate = new LocalDate(2011, 12, 30);
            DateTimeZone samoa = DateTimeZone.ForId("Pacific/Apia");
            var exception = Assert.Throws<SkippedTimeException>(() => samoa.AtStartOfDay(badDate));
            Assert.AreEqual(new LocalDateTime(2011, 12, 30, 0, 0), exception.LocalDateTime);
        }
    }
}
