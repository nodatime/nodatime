// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Testing;
using NUnit.Framework;

namespace NodaTime.Test.Testing
{
    /// <summary>
    /// Tests for the FakeClock class in the Testing assembly.
    /// </summary>
    [TestFixture]
    public class FakeClockTest
    {
        [Test]
        public void DirectConstruction()
        {
            Instant instant = Instant.FromTicksSinceUnixEpoch(100L);
            FakeClock clock = new FakeClock(instant);
            Assert.AreEqual(instant, clock.GetCurrentInstant());
        }

        [Test]
        public void ConstructionWithAdvance()
        {
            Instant instant = Instant.FromTicksSinceUnixEpoch(100L);
            Duration advance = Duration.FromTicks(5);
            FakeClock clock = new FakeClock(instant, advance);
            Assert.AreEqual(advance, clock.AutoAdvance);
            Assert.AreEqual(instant, clock.GetCurrentInstant());
            Assert.AreEqual(instant + advance, clock.GetCurrentInstant());
            Assert.AreEqual(instant + advance + advance, clock.GetCurrentInstant());
        }

        [Test]
        public void ConstructionThenAdvance()
        {
            Instant instant = Instant.FromTicksSinceUnixEpoch(100L);
            FakeClock clock = new FakeClock(instant);
            Assert.AreEqual(instant, clock.GetCurrentInstant());
            Assert.AreEqual(instant, clock.GetCurrentInstant());
            Duration advance = Duration.FromTicks(5);
            clock.AutoAdvance = advance;
            // Setting auto-advance doesn't actually change the clock...
            // but this call will.
            Assert.AreEqual(instant, clock.GetCurrentInstant());
            Assert.AreEqual(instant + advance, clock.GetCurrentInstant());
            Assert.AreEqual(instant + advance + advance, clock.GetCurrentInstant());
        }

        [Test]
        public void UtcDateConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 1, 1, 0, 0);
            FakeClock clock = FakeClock.FromUtc(2010, 1, 1);
            Assert.AreEqual(instant, clock.GetCurrentInstant());
        }

        [Test]
        public void UtcDateTimeConstruction()
        {
            Instant instant = Instant.FromUtc(2010, 1, 1, 10, 30, 25);
            FakeClock clock = FakeClock.FromUtc(2010, 1, 1, 10, 30, 25);
            Assert.AreEqual(instant, clock.GetCurrentInstant());
        }

        [Test]
        public void Advance()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            Duration d = Duration.FromTicks(25);
            clock.Advance(d);
            Assert.AreEqual(125, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceTicks()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceTicks(3);
            Assert.AreEqual(103, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceMilliseconds()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceMilliseconds(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerMillisecond, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceSeconds()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceSeconds(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerSecond, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceMinutes()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceMinutes(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerMinute, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceHours()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceHours(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerHour, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void AdvanceDays()
        {
            FakeClock clock = new FakeClock(Instant.FromTicksSinceUnixEpoch(100L));
            clock.AdvanceDays(3);
            Assert.AreEqual(100 + 3 * NodaConstants.TicksPerDay, clock.GetCurrentInstant().Ticks);
        }

        [Test]
        public void Reset()
        {
            Instant instant1 = Instant.FromTicksSinceUnixEpoch(100L);
            Instant instant2 = Instant.FromTicksSinceUnixEpoch(500L);
            FakeClock clock = new FakeClock(instant1);
            Assert.AreEqual(instant1, clock.GetCurrentInstant());
            clock.Reset(instant2);
            Assert.AreEqual(instant2, clock.GetCurrentInstant());
        }
    }
}
