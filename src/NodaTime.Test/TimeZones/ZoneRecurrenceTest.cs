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
using System;
using NodaTime.TimeZones;
using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public partial class ZoneRecurrenceTest
    {
        private const long TicksPerStandardYear = NodaConstants.TicksPerDay * 365;
        private const long TicksPerLeapYear = NodaConstants.TicksPerDay * 366;

        [Test]
        public void Constructor_nullName_exception()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)DaysOfWeek.Wednesday, true, Offset.Zero);
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneRecurrence(null, Offset.Zero, yearOffset, 1971, 2009), "Null name");
        }

        [Test]
        public void Constructor_nullYearOffset_exception()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ZoneRecurrence("bob", Offset.Zero, null, 1971, 2009), "Null yearOffset");
        }

        [Test]
        public void RenameAppend_nullSuffix()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)DaysOfWeek.Wednesday, true, Offset.Zero);
            var old = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            Assert.Throws(typeof(ArgumentNullException), () => old.RenameAppend(null), "Null suffix");
        }

        [Test]
        public void RenameAppend()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)DaysOfWeek.Wednesday, true, Offset.Zero);
            var old = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var actual = old.RenameAppend("-Summer");
            var expected = new ZoneRecurrence("bob-Summer", Offset.Zero, yearOffset, 1971, 2009);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_BeforeFirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(Instant.MinValue, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(new Instant(Instant.UnixEpoch.Ticks + (1 * TicksPerStandardYear)),
                Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_FirstYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Next(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            actual = recurrence.Next(actual.Value.Instant, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(new Instant(Instant.UnixEpoch.Ticks + (2 * TicksPerStandardYear)),
                Offset.Zero, Offset.Zero);
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
            Transition? expected = new Transition(new Instant(Instant.UnixEpoch.Ticks + (2 * TicksPerStandardYear)),
                Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(new Instant(Instant.UnixEpoch.Ticks + (1 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousTwice_LastYear()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1973);
            Transition? actual = recurrence.Previous(new Instant(Instant.UnixEpoch.Ticks + (2 * TicksPerStandardYear)), Offset.Zero, Offset.Zero);
            actual = recurrence.Previous(actual.Value.Instant, Offset.Zero, Offset.Zero);
            Transition? expected = new Transition(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_OnFirstYear_null()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(Instant.UnixEpoch, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_BeforeFirstYear_null()
        {
            var januaryFirstMidnight = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, januaryFirstMidnight, 1970, 1972);
            Transition? actual = recurrence.Previous(Instant.UnixEpoch - Duration.One, Offset.Zero, Offset.Zero);
            Transition? expected = null;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WriteRead()
        {
            var dio = new DtzIoHelper();
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)DaysOfWeek.Wednesday, true, Offset.Zero);
            var actual = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var expected = dio.WriteRead(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IEquatable_Tests()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)DaysOfWeek.Wednesday, true, Offset.Zero);

            var value = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var equalValue = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var unequalValue = new ZoneRecurrence("foo", Offset.Zero, yearOffset, 1971, 2009);

            TestHelper.TestEqualsClass(value, equalValue, unequalValue);
            TestHelper.TestOperatorEquality(value, equalValue, unequalValue);
        }
    }
}
