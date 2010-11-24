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

using NUnit.Framework;

namespace NodaTime.Test
{
    partial class DurationTest
    {
        private const long startTicks = 123456789L;
        private const long endTicks = 987654321L;
        private static readonly Instant start = Instant.FromTicks(startTicks);
        private static readonly Instant end = Instant.FromTicks(endTicks);

        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.Ticks);
        }

        [Test]
        public void Factory_StandardDays()
        {
            Duration test = Duration.FromStandardDays(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerDay, test.Ticks);

            test = Duration.FromStandardDays(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerDay, test.Ticks);

            test = Duration.FromStandardDays(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardHours()
        {
            Duration test = Duration.FromHours(1);
            Assert.AreEqual(1* NodaConstants.TicksPerHour, test.Ticks);

            test = Duration.FromHours(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerHour, test.Ticks);

            test = Duration.FromHours(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardMinutes()
        {
            Duration test = Duration.FromMinutes(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerMinute, test.Ticks);

            test = Duration.FromMinutes(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerMinute, test.Ticks);

            test = Duration.FromMinutes(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardSeconds()
        {
            Duration test = Duration.FromSeconds(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerSecond, test.Ticks);

            test = Duration.FromSeconds(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerSecond, test.Ticks);

            test = Duration.FromSeconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_Milliseconds()
        {
            Duration test = Duration.FromMilliseconds(1);
            Assert.AreEqual(1 * NodaConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.FromMilliseconds(2);
            Assert.AreEqual(2 * NodaConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.FromMilliseconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void ConstructFrom_Int64()
        {
            long length = 4 * NodaConstants.TicksPerDay +
                          5 * NodaConstants.TicksPerHour +
                          6 * NodaConstants.TicksPerMinute +
                          7 * NodaConstants.TicksPerSecond +
                          8 * NodaConstants.TicksPerMillisecond + 9;
            Duration test = new Duration(length);
            Assert.AreEqual(length, test.Ticks);
        }

        [Test]
        public void ConstructFrom_TickEndPoints()
        {
            Duration test = new Duration(startTicks, endTicks);
            Assert.AreEqual(endTicks - startTicks, test.Ticks);
        }

        [Test]
        public void ConstructFrom_InstantEndPoints()
        {
            Duration test = new Duration(start, end);
            Assert.AreEqual(end.Ticks - start.Ticks, test.Ticks);
        }

        [Test]
        public void ConstructFrom_Interval()
        {
            Interval interval = new Interval(start, end);
            Duration test = new Duration(interval);
            Assert.AreEqual(end.Ticks - start.Ticks, test.Ticks);
        }
    }
}