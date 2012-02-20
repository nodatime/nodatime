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
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneYearOffsetTest
    {
        private const long TicksPerStandardYear = NodaConstants.TicksPerStandardDay * 365;
        private const long TicksPerLeapYear = NodaConstants.TicksPerStandardDay * 366;

        private Offset oneHour = Offset.FromHours(1);
        private Offset twoHours = Offset.FromHours(2);
        // private Offset minusOneHour = Offset.FromHours(-1);

        [Test]
        public void Construct_InvalidMonth_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 0, 1, 1, true, Offset.Zero), "Month 0");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 34, 1, 1, true, Offset.Zero), "Month 34");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, -3, 1, 1, true, Offset.Zero), "Month -3");
        }

        [Test]
        public void Construct_InvalidDayOfMonth_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 0, 1, true, Offset.Zero), "Day of Month 0");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 32, 1, true, Offset.Zero), "Day of Month 32");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 475, 1, true, Offset.Zero),
                          "Day of Month 475");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, -32, 1, true, Offset.Zero),
                          "Day of Month -32");
        }

        [Test]
        public void Construct_InvalidDayOfWeek_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, -1, true, Offset.Zero), "Day of Week -1");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, 8, true, Offset.Zero), "Day of Week 8");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, 5756, true, Offset.Zero),
                          "Day of Week 5856");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, -347, true, Offset.Zero),
                          "Day of Week -347");
        }

        [Test]
        public void Construct_InvalidTickOfDay_Exception()
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, -1, true, Offset.MinValue),
                          "Tick of day MinValue");
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new ZoneYearOffset(TransitionMode.Standard, 2, 3, 8, true, Offset.FromMilliseconds(-1)),
                          "Tick of day MinValue -1");
        }

        [Test]
        public void Construct_ValidMonths()
        {
            for (int month = 1; month <= 12; month++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, month, 1, 1, true, Offset.Zero), "Month " + month);
            }
        }

        [Test]
        public void Construct_ValidDays()
        {
            for (int day = 1; day <= 31; day++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, day, 1, true, Offset.Zero), "Day " + day);
            }
            for (int day = -1; day >= -31; day--)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, day, 1, true, Offset.Zero), "Day " + day);
            }
        }

        [Test]
        public void Construct_ValidDaysOfWeek()
        {
            for (int dayOfWeek = 0; dayOfWeek <= 7; dayOfWeek++)
            {
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, 1, dayOfWeek, true, Offset.Zero), "Day of week " + dayOfWeek);
            }
        }

        [Test]
        public void Construct_ValidTickOfDay()
        {
            int delta = (Offset.MaxValue.TotalMilliseconds / 100);
            for (int millisecond = 0; millisecond < Offset.MaxValue.TotalMilliseconds; millisecond += delta)
            {
                var tickOfDay = Offset.FromMilliseconds(millisecond);
                Assert.NotNull(new ZoneYearOffset(TransitionMode.Standard, 1, 1, 0, true, tickOfDay), "Tick of Day " + tickOfDay);
            }
        }

        [Test]
        public void MakeInstant_Defaults_Epoch()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Year_1971()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1971, Offset.Zero, Offset.Zero);
            var expected = new Instant(365L * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingOffsetIgnored_Epoch()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, twoHours, oneHour);
            var expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingIgnored()
        {
            var offset = new ZoneYearOffset(TransitionMode.Standard, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, twoHours, oneHour);
            var expected = new Instant((1L - 1) * NodaConstants.TicksPerStandardDay - twoHours.TotalTicks);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingAndOffset()
        {
            var offset = new ZoneYearOffset(TransitionMode.Wall, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, twoHours, oneHour);
            var expected = new Instant((1L - 1) * NodaConstants.TicksPerStandardDay - (twoHours.TotalTicks + oneHour.TotalTicks));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Milliseconds()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Create(0, 0, 0, 1));
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((1L - 1) * NodaConstants.TicksPerStandardDay + NodaConstants.TicksPerMillisecond);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_WednesdayForward()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, (int)DayOfWeek.Wednesday, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((7L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_WednesdayBackward()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 15, (int)DayOfWeek.Wednesday, false, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((14L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((1L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanMinusTwo()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, -2, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((30L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanFive()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 5, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((5L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Feb()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 2, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            var expected = new Instant((32L - 1) * NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_JanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks + (1 * TicksPerStandardYear));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_OneSecondBeforeJanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero) - Duration.OneTick;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            var expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_JanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks + (2 * TicksPerStandardYear));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_Feb29_FourYears()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1972, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks + (3 * TicksPerStandardYear) + TicksPerLeapYear);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_Feb29_FourYears()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 2, 29, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1972, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            actual = offset.Next(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks + (2 * ((3 * TicksPerStandardYear) + TicksPerLeapYear)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_JanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Previous(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks - (1 * TicksPerStandardYear));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Previous_OneSecondAfterJanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero) + Duration.OneTick;
            actual = offset.Previous(actual, Offset.Zero, Offset.Zero);
            var expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PreviousTwice_JanOne()
        {
            var offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            var actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            long baseTicks = actual.Ticks;
            actual = offset.Previous(actual, Offset.Zero, Offset.Zero);
            actual = offset.Previous(actual, Offset.Zero, Offset.Zero);
            var expected = new Instant(baseTicks - (TicksPerStandardYear + TicksPerLeapYear));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Next_WednesdayForward()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var actual = offset.MakeInstant(2006, Offset.Zero, Offset.Zero); // Nov 1 2006
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero); // Oct 31 2007
            var expected = new Instant(baseTicks + (1 * TicksPerStandardYear) - NodaConstants.TicksPerStandardDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NextTwice_WednesdayForward()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var actual = offset.MakeInstant(2006, Offset.Zero, Offset.Zero); // Nov 1 2006
            long baseTicks = actual.Ticks;
            actual = offset.Next(actual, Offset.Zero, Offset.Zero); // Oct 31 2007
            actual = offset.Next(actual, Offset.Zero, Offset.Zero); // Nov 5 2008
            var expected = new Instant(baseTicks + TicksPerStandardYear + TicksPerLeapYear + (4 * NodaConstants.TicksPerStandardDay));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_LastSundayInOctober()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 10, -1, (int)IsoDayOfWeek.Sunday, false, Offset.Zero);
            var actual = offset.MakeInstant(1996, Offset.Zero, Offset.Zero);
            Assert.AreEqual(Instant.FromUtc(1996, 10, 27, 0, 0), actual);
        }

        [Test]
        public void Test()
        {
            var dio = new DtzIoHelper("ZoneYearOffset");
            var expected = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            dio.TestZoneYearOffset(expected);

            dio = new DtzIoHelper("ZoneYearOffset");
            expected = new ZoneYearOffset(TransitionMode.Utc, 10, -31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            dio.TestZoneYearOffset(expected);
        }

        [Test]
        public void IEquatable_Tests()
        {
            var value = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var equalValue = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var unequalValue = new ZoneYearOffset(TransitionMode.Utc, 9, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);

            TestHelper.TestEqualsClass(value, equalValue, unequalValue);
            TestHelper.TestOperatorEquality(value, equalValue, unequalValue);
        }
    }
}