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
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class DaylightSavingTimeZoneTest
    {
        private static readonly ZoneRecurrence Winter = new ZoneRecurrence("Winter", Offset.Zero,
            new ZoneYearOffset(TransitionMode.Wall, 10, 5, 0, false, Offset.ForHours(2)), 2000, int.MaxValue);

        private static readonly ZoneRecurrence Summer = new ZoneRecurrence("Summer", Offset.ForHours(1),
            new ZoneYearOffset(TransitionMode.Wall, 3, 10, 0, false, Offset.ForHours(1)), 2000, int.MaxValue);

        /// <summary>
        /// Time zone with the following characteristics:
        /// - Only valid from March 10th 2000
        /// - Standard offset of +5 (so 4am UTC = 9am local)
        /// - Summer time (DST = 1 hour) always starts at 1am local time on March 10th (skips to 2am)
        /// - Winter time (DST = 0) always starts at 2am local time on October 5th (skips to 1am)
        /// </summary>
        private static readonly DaylightSavingsTimeZone TestZone = new DaylightSavingsTimeZone("Test",
            Offset.ForHours(5), Winter, Summer);

        [Test]
        public void MinMaxOffsets()
        {
            Assert.AreEqual(Offset.ForHours(6), TestZone.MaxOffset);
            Assert.AreEqual(Offset.ForHours(5), TestZone.MinOffset);
        }

        // TODO: Consider removing all of these tests, as they only use DateTimeZone now...
        [Test]
        public void GetZoneInterval_Instant_Summer()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 6, 1, 0, 0));
            Assert.AreEqual("Summer", interval.Name);
            Assert.AreEqual(Offset.ForHours(6), interval.Offset);
            Assert.AreEqual(Offset.ForHours(5), interval.BaseOffset);
            Assert.AreEqual(Offset.ForHours(1), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 3, 10, 2, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 2, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_Winter()
        {
            var interval = TestZone.GetZoneInterval(Instant.FromUtc(2010, 11, 1, 0, 0));
            Assert.AreEqual("Winter", interval.Name);
            Assert.AreEqual(Offset.ForHours(5), interval.Offset);
            Assert.AreEqual(Offset.ForHours(5), interval.BaseOffset);
            Assert.AreEqual(Offset.ForHours(0), interval.Savings);
            Assert.AreEqual(new LocalInstant(2010, 10, 5, 1, 0), interval.LocalStart);
            Assert.AreEqual(new LocalInstant(2011, 3, 10, 1, 0), interval.LocalEnd);
        }

        [Test]
        public void GetZoneInterval_Instant_StartOfFirstSummer()
        {
            // This is only just about valid
            var firstSummer = Instant.FromUtc(2000, 3, 9, 20, 0);
            var interval = TestZone.GetZoneInterval(firstSummer);
            Assert.AreEqual("Summer", interval.Name);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstSummer()
        {
            var early = new LocalInstant(2000, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(early);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstWinter()
        {
            var winter = new LocalInstant(2000, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtFirstGapStart()
        {
            var startOfFirstGap = new LocalInstant(2000, 3, 10, 1, 0);
            var actual = TestZone.GetZoneIntervals(startOfFirstGap);
            var expected = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetZoneIntervals_WithinFirstGap()
        {
            var middleOfFirstGap = new LocalInstant(2000, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervals(middleOfFirstGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfFirstGap()
        {
            var endOfFirstGap = new LocalInstant(2000, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervals(endOfFirstGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervals(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfFirstAmbiguity()
        {
            var firstAmbiguity = new LocalInstant(2000, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervals(firstAmbiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterFirstAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2000, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervals(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitrarySummer()
        {
            var summer = new LocalInstant(2010, 6, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(summer);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryWinter()
        {
            var winter = new LocalInstant(2010, 12, 1, 0, 0);
            var pair = TestZone.GetZoneIntervals(winter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_AtArbitraryGapStart()
        {
            var startOfGap = new LocalInstant(2010, 3, 10, 1, 0);
            var pair = TestZone.GetZoneIntervals(startOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_WithinArbitraryGap()
        {
            var middleOfGap = new LocalInstant(2010, 3, 10, 1, 30);
            var pair = TestZone.GetZoneIntervals(middleOfGap);
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void GetZoneIntervals_EndOfArbitraryGap()
        {
            var endOfGap = new LocalInstant(2010, 3, 10, 2, 0);
            var pair = TestZone.GetZoneIntervals(endOfGap);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }

        [Test]
        public void GetZoneIntervals_StartOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 0);
            var pair = TestZone.GetZoneIntervals(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_MiddleOfArbitraryAmbiguity()
        {
            var ambiguity = new LocalInstant(2010, 10, 5, 1, 30);
            var pair = TestZone.GetZoneIntervals(ambiguity);
            Assert.AreEqual("Summer", pair.EarlyInterval.Name);
            Assert.AreEqual("Winter", pair.LateInterval.Name);
        }

        [Test]
        public void GetZoneIntervals_AfterArbitraryAmbiguity()
        {
            var unambiguousWinter = new LocalInstant(2010, 10, 5, 2, 0);
            var pair = TestZone.GetZoneIntervals(unambiguousWinter);
            Assert.AreEqual("Winter", pair.EarlyInterval.Name);
            Assert.IsNull(pair.LateInterval);
        }
    }
}
