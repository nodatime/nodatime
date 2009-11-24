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
        private static readonly Instant start = new Instant(startTicks);
        private static readonly Instant end = new Instant(endTicks);

        [Test]
        public void Zero()
        {
            Duration test = Duration.Zero;
            Assert.AreEqual(0, test.Ticks);
        }

        [Test]
        public void Factory_StandardDays()
        {
            Duration test = Duration.StandardDays(1);
            Assert.AreEqual(1 * DateTimeConstants.TicksPerDay, test.Ticks);

            test = Duration.StandardDays(2);
            Assert.AreEqual(2 * DateTimeConstants.TicksPerDay, test.Ticks);

            test = Duration.StandardDays(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardHours()
        {
            Duration test = Duration.StandardHours(1);
            Assert.AreEqual(1* DateTimeConstants.TicksPerHour, test.Ticks);

            test = Duration.StandardHours(2);
            Assert.AreEqual(2 * DateTimeConstants.TicksPerHour, test.Ticks);

            test = Duration.StandardHours(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardMinutes()
        {
            Duration test = Duration.StandardMinutes(1);
            Assert.AreEqual(1 * DateTimeConstants.TicksPerMinute, test.Ticks);

            test = Duration.StandardMinutes(2);
            Assert.AreEqual(2 * DateTimeConstants.TicksPerMinute, test.Ticks);

            test = Duration.StandardMinutes(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_StandardSeconds()
        {
            Duration test = Duration.StandardSeconds(1);
            Assert.AreEqual(1 * DateTimeConstants.TicksPerSecond, test.Ticks);

            test = Duration.StandardSeconds(2);
            Assert.AreEqual(2 * DateTimeConstants.TicksPerSecond, test.Ticks);

            test = Duration.StandardSeconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test]
        public void Factory_Milliseconds()
        {
            Duration test = Duration.Milliseconds(1);
            Assert.AreEqual(1 * DateTimeConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.Milliseconds(2);
            Assert.AreEqual(2 * DateTimeConstants.TicksPerMillisecond, test.Ticks);

            test = Duration.Milliseconds(0);
            Assert.AreEqual(Duration.Zero, test);
        }

        [Test] 
        [Ignore("Not enough code")]
        public void Factory_Parse()
        {
            Duration test = Duration.Parse("PT72.3456S");
            Assert.Equals(72345600, test.Ticks);
        }

        [Test]
        public void ConstructFrom_Int64()
        {
            long length = 4 * DateTimeConstants.TicksPerDay +
                          5 * DateTimeConstants.TicksPerHour +
                          6 * DateTimeConstants.TicksPerMinute +
                          7 * DateTimeConstants.TicksPerSecond +
                          8 * DateTimeConstants.TicksPerMillisecond + 9;
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