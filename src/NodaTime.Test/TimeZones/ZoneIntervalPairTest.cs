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

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneIntervalPairTest
    {
        [Test]
        public void MatchingIntervals_SingleInterval()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.Unambiguous(new ZoneInterval("Foo", new Instant(0), new Instant(10), Offset.Zero, Offset.Zero));
            Assert.AreEqual(1, pair.MatchingIntervals);
        }

        [Test]
        public void MatchingIntervals_NoIntervals()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.NoMatch;
            Assert.AreEqual(0, pair.MatchingIntervals);
        }

        [Test]
        public void MatchingIntervals_TwoIntervals()
        {
            ZoneIntervalPair pair = ZoneIntervalPair.Ambiguous(
                new ZoneInterval("Foo", new Instant(0), new Instant(10), Offset.Zero, Offset.Zero),
                new ZoneInterval("Bar", new Instant(10), new Instant(20), Offset.Zero, Offset.Zero));
            Assert.AreEqual(2, pair.MatchingIntervals);
        }
    }
}
