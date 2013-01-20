#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2013 Jon Skeet
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NodaTime.Testing.TimeZones;
using NodaTime.TimeZones;
using Options = NodaTime.TimeZones.ZoneEqualityComparer.Options;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class ZoneEqualityComparerTest
    {
        // Sample instants for use in tests. They're on January 1st 2000...2009, midnight UTC.
        private static IList<Instant> Instants = Enumerable.Range(2000, 10)
                                                           .Select(year => Instant.FromUtc(year, 1, 1, 0, 0))
                                                           .ToList()
                                                           .AsReadOnly();

        // Various tests using a pair of zones which can demonstrate a number of
        // different features.
        [Test]
        public void Various()
        {
            // Names, some offsets, and first transition are all different.
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[0], 1, 0, "xx" },
                { Instants[2], 3, 0, "1b" },
                { Instants[4], 2, 1, "1c" },
                { Instants[6], 4, 0, "1d" },
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[1], 1, 0, "xx" },
                { Instants[2], 3, 0, "2b" },
                { Instants[4], 1, 2, "2c" },
                { Instants[6], 5, 0, "2d"},
            }.Build();
            // Even though the first transition point is different, by default that's fine if
            // the start point is "inside" both.
            AssertEqual(zone1, zone2, Instants[1], Instants[5], Options.Default);
            // When we extend backwards a bit, we can see the difference between the two.
            AssertNotEqual(zone1, zone2, Instants[1] - Duration.Epsilon, Instants[5], Options.Default);
            // Or if we force the start and end transitions to be exact...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[5], Options.MatchStartAndEndTransitions);
            
            // The first two transitions have the same split between standard and saving...
            AssertEqual(zone1, zone2, Instants[1], Instants[4], Options.MatchOffsetComponents);
            // The third one (at Instants[4]) doesn't...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[5], Options.MatchOffsetComponents);

            // The first transition has the same name for the zone interval...
            AssertEqual(zone1, zone2, Instants[1], Instants[2], Options.MatchNames);
            // The second transition (at Instants[2]) doesn't...
            AssertNotEqual(zone1, zone2, Instants[1], Instants[3], Options.MatchNames);
        }

        [Test]
        public void ElidedTransitions()
        {
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 0, 0, "a" },
                { Instants[4], 1, 2, "b" },
                { Instants[5], 2, 1, "b" },
                { Instants[6], 1, 0, "d" },
                { Instants[7], 1, 0, "e" },
                { Instants[8], 0, 0, "x" },
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 0, 0, "a" },
                { Instants[4], 3, 0, "b" },
                // Instants[5] isn't included here: wall offset is the same; components change in zone1
                { Instants[6], 1, 0, "d" },
                // Instants[7] isn't included here: offset components are the same; names change in zone1
                { Instants[8], 0, 0, "x" },
            }.Build();

            AssertEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, Options.Default);
            // BOT-Instants[6] will elide transitions when ignoring components, even if we match names
            AssertEqual(zone1, zone2, Instant.MinValue, Instants[6], Options.MatchNames);
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instants[6], Options.MatchOffsetComponents);
            // Instants[6]-EOT will elide transitions when ignoring names, even if we match components
            AssertEqual(zone1, zone2, Instants[6], Instant.MaxValue, Options.MatchOffsetComponents);
            AssertNotEqual(zone1, zone2, Instants[6], Instant.MaxValue, Options.MatchNames);
            
            // But if we require the exact transitions, both fail
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instants[6], Options.MatchAllTransitions);
            AssertNotEqual(zone1, zone2, Instants[6], Instant.MaxValue, Options.MatchAllTransitions);
        }

        [Test]
        public void ElidedTransitions_Degenerate()
        {
            // Transitions with *nothing* that we care about. (Normally
            // these wouldn't even be generated, but we could imagine some
            // sort of zone interval in the future which had another property...)
            var zone1 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 1, 0, "a" },
                { Instants[4], 1, 0, "a" },
                { Instants[5], 1, 0, "a" },
                { Instants[6], 0 }
            }.Build();
            var zone2 = new MultiTransitionDateTimeZone.Builder
            {
                { Instants[3], 1, 0, "a" },
                { Instants[6], 0 }
            }.Build();

            // We can match *everything* except exact transitions...
            AssertEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, Options.MatchNames | Options.MatchOffsetComponents | Options.MatchStartAndEndTransitions);
            // But not the exact transitions...
            AssertNotEqual(zone1, zone2, Instant.MinValue, Instant.MaxValue, Options.MatchAllTransitions);
        }

        private void AssertEqual(DateTimeZone first, DateTimeZone second, 
            Instant start, Instant end, Options options)
        {
            var comparer = new ZoneEqualityComparer(start, end, options);
            Assert.IsTrue(comparer.Equals(first, second));
            Assert.AreEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
        }

        private void AssertNotEqual(DateTimeZone first, DateTimeZone second, 
            Instant start, Instant end, Options options)
        {
            var comparer = new ZoneEqualityComparer(start, end, options);
            Assert.IsFalse(comparer.Equals(first, second));
            // If this fails, the code *could* still be correct - but it's unlikely...
            Assert.AreNotEqual(comparer.GetHashCode(first), comparer.GetHashCode(second));
        }
    }
}
