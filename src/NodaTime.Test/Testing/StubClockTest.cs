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
using NodaTime.Testing;

namespace NodaTime.Test.Testing
{
    /// <summary>
    /// Tests for the StubClock class in the Testing assembly.
    /// </summary>
    [TestFixture]
    public class StubClockTest
    {
        [Test]
        public void DirectConstruction()
        {
            Instant instant = new Instant(100L);
            StubClock clock = new StubClock(instant);
            Assert.AreEqual(instant, clock.Now);
        }

        [Test]
        public void UtcDateConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 1, 1, 0, 0);
            StubClock clock = StubClock.FromUtc(2010, 1, 1);
            Assert.AreEqual(instant, clock.Now);
        }

        [Test]
        public void UtcDateTimeConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 1, 1, 10, 30, 25);
            StubClock clock = StubClock.FromUtc(2010, 1, 1, 10, 30, 25);
            Assert.AreEqual(instant, clock.Now);
        }

        [Test]
        public void Advance()
        {
            StubClock clock = new StubClock(new Instant(100L));
            Duration d = new Duration(25);
            clock.Advance(d);
            Assert.AreEqual(125, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceTicks()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceTicks(3);
            Assert.AreEqual(103, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceMilliseconds()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceMilliseconds(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerMillisecond, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceSeconds()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceSeconds(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerSecond, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceMinutes()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceMinutes(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerMinute, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceHours()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceHours(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerHour, clock.Now.Ticks);
        }

        [Test]
        public void AdvanceDays()
        {
            StubClock clock = new StubClock(new Instant(100L));
            clock.AdvanceDays(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerStandardDay, clock.Now.Ticks);
        }
    }
}
