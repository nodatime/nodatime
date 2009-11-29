#region Copyright and license information

// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public partial class ZoneYearOffsetTest
    {
        private Offset oneHour = new Offset(1L * NodaConstants.TicksPerHour);
        private Offset twoHours = new Offset(2L * NodaConstants.TicksPerHour);
        private Offset minusOneHour = new Offset(-1L * NodaConstants.TicksPerHour);

        [Test]
        public void MakeInstant_Defaults_Epoch()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Year_1971()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1971, Offset.Zero, Offset.Zero);
            Instant expected = new Instant(365L * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingOffsetIgnored_Epoch()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, twoHours, oneHour);
            Instant expected = Instant.UnixEpoch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingIgnored()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Standard, 1, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, twoHours, oneHour);
            Instant expected = new Instant((1L - 1) * NodaConstants.TicksPerDay - twoHours.Ticks);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_SavingAndOffset()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Wall, 1, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, twoHours, oneHour);
            Instant expected = new Instant((1L - 1) * NodaConstants.TicksPerDay - (twoHours.Ticks + oneHour.Ticks));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Milliseconds()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 0, 0, true, new Offset(1000L));
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((1L - 1) * NodaConstants.TicksPerDay + 1000L);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_WednesdayForward()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 0, (int)DayOfWeek.Wednesday, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((7L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_WednesdayBackward()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 15, (int)DayOfWeek.Wednesday, false, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((14L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanOne()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 1, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((1L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanMinusTwo()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, -2, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((30L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_JanFive()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 1, 5, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((5L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeInstant_Feb()
        {
            ZoneYearOffset offset = new ZoneYearOffset(TransitionMode.Utc, 2, 0, 0, true, Offset.Zero);
            Instant actual = offset.MakeInstant(1970, Offset.Zero, Offset.Zero);
            Instant expected = new Instant((32L - 1) * NodaConstants.TicksPerDay);
            Assert.AreEqual(expected, actual);
        }
    }
}
