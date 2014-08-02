// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Test.TimeZones.IO;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneRecurrenceTest
    {
        [Test]
        public void Constructor_nullName_exception()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneRecurrence(null, Offset.Zero, yearOffset, 1971, 2009), "Null name");
        }

        [Test]
        public void Constructor_nullYearOffset_exception()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneRecurrence("bob", Offset.Zero, null, 1971, 2009), "Null yearOffset");
        }

        [Test]
        public void Next_BeforeFirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(Instant.MinValue, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.FromUtc(1971, 1, 1, 0, 0), Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            actual = recurrence.Next(actual.Value.Instant, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.FromUtc(1972, 1, 1, 0, 0), Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_BeyondLastYear_null()
        {
            var afterRecurrenceEnd = Instant.FromUtc(1980, 1, 1, 0, 0);
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(afterRecurrenceEnd, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousOrSame_AfterLastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.PreviousOrSame(Instant.MaxValue, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.FromUtc(1972, 1, 1, 0, 0), Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousOrSame_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.PreviousOrSame(Instant.FromUtc(1971, 1, 1, 0, 0) - Duration.Epsilon, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousOrSameTwice_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1973);
            Transition? actual = recurrence.PreviousOrSame(Instant.FromUtc(1972, 1, 1, 0, 0) - Duration.Epsilon, Offset.Zero, Offset.Zero);
            actual = recurrence.PreviousOrSame(actual.Value.Instant - Duration.Epsilon, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousOrSame_OnFirstYear_null()
        {
            // Transition is on January 2nd, but we're asking for January 1st.
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 2, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.PreviousOrSame(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousOrSame_BeforeFirstYear_null()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.PreviousOrSame(NodaConstants.UnixEpoch - Duration.Epsilon, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_ExcludesGivenInstant()
        {
            var january10thMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 10, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("x", Offset.Zero, january10thMidnight, 2000, 3000);
            var transition = Instant.FromUtc(2500, 1, 10, 0, 0);
            var next = recurrence.Next(transition, Offset.Zero, Offset.Zero);
            Assert.AreEqual(2501, next.Value.Instant.InUtc().Year);
        }

        [Test]
        public void PreviousOrSame_IncludesGivenInstant()
        {
            var january10thMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 10, 0, true, LocalTime.Midnight);
            var recurrence = new ZoneRecurrence("x", Offset.Zero, january10thMidnight, 2000, 3000);
            var transition = Instant.FromUtc(2500, 1, 10, 0, 0);
            var next = recurrence.PreviousOrSame(transition, Offset.Zero, Offset.Zero);
            Assert.AreEqual(transition, next.Value.Instant);
        }

        [Test]
        public void TestSerialization()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);
            var expected = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            dio.TestZoneRecurrence(expected);
        }

        [Test]
        public void IEquatable_Tests()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, LocalTime.Midnight);

            var value = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var equalValue = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var unequalValue = new ZoneRecurrence("foo", Offset.Zero, yearOffset, 1971, 2009);

            TestHelper.TestEqualsClass(value, equalValue, unequalValue);
        }
    }
}
