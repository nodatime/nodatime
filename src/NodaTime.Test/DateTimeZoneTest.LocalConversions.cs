// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Testing.TimeZones;
using NodaTime.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for aspects of DateTimeZone to do with converting from LocalDateTime and 
    /// LocalDate to ZonedDateTime.
    /// </summary>
    // TODO: Fix all tests to use SingleTransitionZone.
    public partial class DateTimeZoneTest
    {
        // Sample time zones for DateTimeZone.AtStartOfDay etc. I didn't want to only test midnight transitions.
        private static readonly DateTimeZone LosAngeles = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
        private static readonly DateTimeZone NewZealand = DateTimeZoneProviders.Tzdb["Pacific/Auckland"];
        private static readonly DateTimeZone Paris = DateTimeZoneProviders.Tzdb["Europe/Paris"];
        private static readonly DateTimeZone NewYork = DateTimeZoneProviders.Tzdb["America/New_York"];
        private static readonly DateTimeZone Pacific = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];

        /// <summary>
        /// Local midnight at the start of the transition (June 1st) becomes 1am.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardAtMidnightZone =
            new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 6, 1, 2, 0), Offset.FromHours(-2), Offset.FromHours(-1));

        /// <summary>
        /// Local 1am at the start of the transition (June 1st) becomes midnight.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardToMidnightZone =
            new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 6, 1, 3, 0), Offset.FromHours(-2), Offset.FromHours(-3));

        /// <summary>
        /// Local 11.20pm at the start of the transition (May 30th) becomes 12.20am of June 1st.
        /// </summary>
        private static readonly DateTimeZone TransitionForwardBeforeMidnightZone =
            new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 6, 1, 1, 20), Offset.FromHours(-2), Offset.FromHours(-1));

        /// <summary>
        /// Local 12.20am at the start of the transition (June 1st) becomes 11.20pm of the previous day.
        /// </summary>
        private static readonly DateTimeZone TransitionBackwardAfterMidnightZone =
            new SingleTransitionDateTimeZone(Instant.FromUtc(2000, 6, 1, 2, 20), Offset.FromHours(-2), Offset.FromHours(-3));

        private static readonly LocalDate TransitionDate = new LocalDate(2000, 6, 1);

        [Test]
        public void AmbiguousStartOfDay_TransitionAtMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 0).WithOffset(Offset.FromHours(-2)),
                TransitionBackwardToMidnightZone);
            var actual = TransitionBackwardToMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AmbiguousStartOfDay_TransitionAfterMidnight()
        {
            // Occurrence before transition
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 0).WithOffset(Offset.FromHours(-2)),
                TransitionBackwardAfterMidnightZone);
            var actual = TransitionBackwardAfterMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionAtMidnight()
        {
            // 1am because of the skip
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 1, 0).WithOffset(Offset.FromHours(-1)),
                TransitionForwardAtMidnightZone);
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SkippedStartOfDay_TransitionBeforeMidnight()
        {
            // 12.20am because of the skip
            var expected = new ZonedDateTime(new LocalDateTime(2000, 6, 1, 0, 20).WithOffset(Offset.FromHours(-1)),
                TransitionForwardBeforeMidnightZone);
            var actual = TransitionForwardBeforeMidnightZone.AtStartOfDay(TransitionDate);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnambiguousStartOfDay()
        {
            // Just a simple midnight in March.
            var expected = new ZonedDateTime(new LocalDateTime(2000, 3, 1, 0, 0).WithOffset(Offset.FromHours(-2)),
                TransitionForwardAtMidnightZone);
            var actual = TransitionForwardAtMidnightZone.AtStartOfDay(new LocalDate(2000, 3, 1));
            Assert.AreEqual(expected, actual);
        }

        private static void AssertImpossible(LocalDateTime localTime, DateTimeZone zone)
        {
            try
            {
                zone.MapLocal(localTime).Single();
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
            ZonedDateTime earlier = zone.MapLocal(localTime).First();
            ZonedDateTime later = zone.MapLocal(localTime).Last();
            Assert.AreEqual(localTime, earlier.LocalDateTime);
            Assert.AreEqual(localTime, later.LocalDateTime);
            Assert.That(earlier.ToInstant(), Is.LessThan(later.ToInstant()));

            try
            {
                zone.MapLocal(localTime).Single();
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
            var zoned = zone.MapLocal(localTime).Single();
            int actualHours = zoned.Offset.Milliseconds / NodaConstants.MillisecondsPerHour;
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
            var mapping = NewYork.MapLocal(unambigiousTime);
            Assert.AreEqual(1, mapping.Count);
        }

        [Test]
        public void MapLocalDateTime_AmbiguousDateReturnsAmbigousMapping()
        {
            //2011-11-06 01:30:00 - falls during DST - EST conversion in America/New York timezone
            var ambiguousTime = new LocalDateTime(2011, 11, 6, 1, 30);
            var mapping = NewYork.MapLocal(ambiguousTime);
            Assert.AreEqual(2, mapping.Count);
        }

        [Test]
        public void MapLocalDateTime_SkippedDateReturnsSkippedMapping()
        {
            //2011-03-13 02:30:00 - falls during EST - DST conversion in America/New York timezone
            var skippedTime = new LocalDateTime(2011, 3, 13, 2, 30);
            var mapping = NewYork.MapLocal(skippedTime);
            Assert.AreEqual(0, mapping.Count);
        }

        // Some zones skipped dates by changing from UTC-lots to UTC+lots. For example, Samoa (Pacific/Apia)
        // skipped December 30th 2011, going from  23:59:59 December 29th local time UTC-10
        // to 00:00:00 December 31st local time UTC+14
        [Test]
        [TestCase("Pacific/Apia", "2011-12-30")]
        [TestCase("Pacific/Enderbury", "1995-01-01")]
        [TestCase("Pacific/Kiritimati", "1995-01-01")]
        [TestCase("Pacific/Kwajalein", "1993-08-20")]
        public void AtStartOfDay_DayDoesntExist(string zoneId, string localDate)
        {
            LocalDate badDate = LocalDatePattern.IsoPattern.Parse(localDate).Value;
            DateTimeZone zone = DateTimeZoneProviders.Tzdb[zoneId];
            var exception = Assert.Throws<SkippedTimeException>(() => zone.AtStartOfDay(badDate));
            Assert.AreEqual(badDate + LocalTime.Midnight, exception.LocalDateTime);
        }

        [Test]
        public void AtStrictly_InWinter()
        {
            var when = Pacific.AtStrictly(new LocalDateTime(2009, 12, 22, 21, 39, 30));

            Assert.AreEqual(2009, when.Year);
            Assert.AreEqual(12, when.Month);
            Assert.AreEqual(22, when.Day);
            Assert.AreEqual(2, when.DayOfWeek);
            Assert.AreEqual(21, when.Hour);
            Assert.AreEqual(39, when.Minute);
            Assert.AreEqual(30, when.Second);
            Assert.AreEqual(Offset.FromHours(-8), when.Offset);
        }

        [Test]
        public void AtStrictly_InSummer()
        {
            var when = Pacific.AtStrictly(new LocalDateTime(2009, 6, 22, 21, 39, 30));

            Assert.AreEqual(2009, when.Year);
            Assert.AreEqual(6, when.Month);
            Assert.AreEqual(22, when.Day);
            Assert.AreEqual(21, when.Hour);
            Assert.AreEqual(39, when.Minute);
            Assert.AreEqual(30, when.Second);
            Assert.AreEqual(Offset.FromHours(-7), when.Offset);
        }

        /// <summary>
        /// Pacific time changed from -7 to -8 at 2am wall time on November 2nd 2009,
        /// so 2am became 1am.
        /// </summary>
        [Test]
        public void AtStrictly_ThrowsWhenAmbiguous()
        {
            Assert.Throws<AmbiguousTimeException>(() => Pacific.AtStrictly(new LocalDateTime(2009, 11, 1, 1, 30, 0)));
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2.30am doesn't exist on that day.
        /// </summary>
        [Test]
        public void AtStrictly_ThrowsWhenSkipped()
        {
            Assert.Throws<SkippedTimeException>(() => Pacific.AtStrictly(new LocalDateTime(2009, 3, 8, 2, 30, 0)));
        }

        /// <summary>
        /// Pacific time changed from -7 to -8 at 2am wall time on November 2nd 2009,
        /// so 2am became 1am. We'll return the earlier result, i.e. with the offset of -7
        /// </summary>
        [Test]
        public void AtLeniently_AmbiguousTime_ReturnsEarlierMapping()
        {
            var local = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            var zoned = Pacific.AtLeniently(local);
            Assert.AreEqual(local, zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-7), zoned.Offset);
        }

        /// <summary>
        /// Pacific time changed from -8 to -7 at 2am wall time on March 8th 2009,
        /// so 2am became 3am. This means that 2:30am doesn't exist on that day.
        /// We'll return 3:30am, the forward-shifted value.
        /// </summary>
        [Test]
        public void AtLeniently_ReturnsForwardShiftedValue()
        {
            var local = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            var zoned = Pacific.AtLeniently(local);
            Assert.AreEqual(new LocalDateTime(2009, 3, 8, 3, 30, 0), zoned.LocalDateTime);
            Assert.AreEqual(Offset.FromHours(-7), zoned.Offset);
        }

        [Test]
        public void ResolveLocal()
        {
            // Don't need much for this - it only delegates.
            var ambiguous = new LocalDateTime(2009, 11, 1, 1, 30, 0);
            var skipped = new LocalDateTime(2009, 3, 8, 2, 30, 0);
            Assert.AreEqual(Pacific.AtLeniently(ambiguous), Pacific.ResolveLocal(ambiguous, Resolvers.LenientResolver));
            Assert.AreEqual(Pacific.AtLeniently(skipped), Pacific.ResolveLocal(skipped, Resolvers.LenientResolver));
        }
    }
}
