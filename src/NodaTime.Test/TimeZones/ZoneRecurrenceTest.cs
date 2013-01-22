// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NUnit.Framework;
using NodaTime.Test.TimeZones.IO;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneRecurrenceTest
    {
        private const long TicksPerStandardYear = NodaConstants.TicksPerStandardDay * 365;
        // private const long TicksPerLeapYear = NodaConstants.TicksPerStandardDay * 366;

        [Test]
        public void Constructor_nullName_exception()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
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
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(Instant.MinValue, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(new Instant(NodaConstants.UnixEpoch.Ticks + (1 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            actual = recurrence.Next(actual.Value.Instant, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(new Instant(NodaConstants.UnixEpoch.Ticks + (2 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_BeyondLastYear_null()
        {
            var afterRecurrenceEnd = Instant.FromUtc(1980, 1, 1, 0, 0);
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(afterRecurrenceEnd, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_AfterLastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(Instant.MaxValue, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(new Instant(NodaConstants.UnixEpoch.Ticks + (2 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(new Instant(NodaConstants.UnixEpoch.Ticks + (1 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousTwice_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1973);
            Transition? actual = recurrence.Previous(new Instant(NodaConstants.UnixEpoch.Ticks + (2 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            actual = recurrence.Previous(actual.Value.Instant, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_OnFirstYear_null()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(NodaConstants.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_BeforeFirstYear_null()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(NodaConstants.UnixEpoch - Duration.Epsilon, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSerialization()
        {
            var dio = DtzIoHelper.CreateNoStringPool();
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var expected = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            dio.TestZoneRecurrence(expected);
        }

        [Test]
        public void IEquatable_Tests()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);

            var value = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var equalValue = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var unequalValue = new ZoneRecurrence("foo", Offset.Zero, yearOffset, 1971, 2009);

            TestHelper.TestEqualsClass(value, equalValue, unequalValue);
            TestHelper.TestOperatorEquality(value, equalValue, unequalValue);
        }
    }
}
