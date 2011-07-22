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
    public class TransitionResolverTest
    {
        // -8 standard time, -7 in summer
        private static readonly DateTimeZone LosAngeles = DateTimeZone.ForId("America/Los_Angeles");

        // Zone intervals corresponding to winter 2010/2011, summer 2011 and winter 2011/2012
        private static readonly ZoneInterval Early2011 = LosAngeles.GetZoneInterval(Instant.FromUtc(2011, 1, 5, 0, 0));
        private static readonly ZoneInterval Summer2011 = LosAngeles.GetZoneInterval(Instant.FromUtc(2011, 6, 1, 0, 0));
        private static readonly ZoneInterval Late2011 = LosAngeles.GetZoneInterval(Instant.FromUtc(2011, 12, 1, 0, 0));

        // Part way through the spring transition: 2.20am on March 13th 2011 (clocks go 2am to 3am)
        private static readonly LocalInstant SkippedSpring = new LocalDateTime(2011, 3, 13, 2, 20).LocalInstant;

        // Part way through the fall transition: 1.20am on March 3rd 2011 (clocks go 2am to 1am)
        private static readonly LocalInstant AmbiguousFall = new LocalDateTime(2011, 11, 6, 1, 20).LocalInstant;

        [Test]
        public void StrictSpring()
        {
            Assert.Throws<SkippedTimeException>(() => ResolveGap(TransitionResolver.GapStrategy.Strict));
        }

        [Test]
        public void EndOfEarlyIntervalSpring()
        {
            Instant expected = Instant.FromUtc(2011, 3, 13, 10, 0) - Duration.One;
            Instant actual = ResolveGap(TransitionResolver.GapStrategy.EndOfEarlyInterval);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StartOfLateIntervalSpring()
        {
            Instant expected = Instant.FromUtc(2011, 3, 13, 10, 0);
            Instant actual = ResolveGap(TransitionResolver.GapStrategy.StartOfLateInterval);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PushForwardSpring()
        {
            Instant expected = Instant.FromUtc(2011, 3, 13, 10, 20);
            Instant actual = ResolveGap(TransitionResolver.GapStrategy.PushForward);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PushBackwardSpring()
        {
            Instant expected = Instant.FromUtc(2011, 3, 13, 9, 20);
            Instant actual = ResolveGap(TransitionResolver.GapStrategy.PushBackward);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StrictFall()
        {
            Assert.Throws<AmbiguousTimeException>(() => ResolveAmbiguity(TransitionResolver.AmbiguityStrategy.Strict));
        }

        [Test]
        public void EarlierFall()
        {
            Instant expected = Instant.FromUtc(2011, 11, 6, 8, 20); // Offset of +7 from 1.20am
            Instant actual = ResolveAmbiguity(TransitionResolver.AmbiguityStrategy.Earlier);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LaterFall()
        {
            Instant expected = Instant.FromUtc(2011, 11, 6, 9, 20); // Offset of +8 from 1.20am
            Instant actual = ResolveAmbiguity(TransitionResolver.AmbiguityStrategy.Later);
            Assert.AreEqual(expected, actual);
        }

        private Instant ResolveGap(TransitionResolver.GapStrategy strategy)
        {
            var resolver = TransitionResolver.FromStrategies(TransitionResolver.AmbiguityStrategy.Strict, strategy);
            return resolver.ResolveGap(SkippedSpring, LosAngeles);
        }

        private Instant ResolveAmbiguity(TransitionResolver.AmbiguityStrategy strategy)
        {
            var resolver = TransitionResolver.FromStrategies(strategy, TransitionResolver.GapStrategy.Strict);
            var pair = ZoneIntervalPair.Ambiguous(Summer2011, Late2011);
            return resolver.ResolveAmbiguity(pair, AmbiguousFall, LosAngeles);
        }
    }
}
