#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
    public partial class DurationTest
    {
        [Test]
        public void PositiveRemainderProperties()
        {
            Duration duration = Duration.FromStandardDays(2) +
                Duration.FromHours(14) +
                Duration.FromMinutes(35) +
                Duration.FromSeconds(12) +
                Duration.FromMilliseconds(234) +
                Duration.FromTicks(9876);
            Assert.AreEqual(2, duration.StandardDays);
            Assert.AreEqual(14, duration.HoursRemainder);
            Assert.AreEqual(35, duration.MinutesRemainder);
            Assert.AreEqual(12, duration.SecondsRemainder);
            Assert.AreEqual(234, duration.MillisecondsRemainder);
            Assert.AreEqual(9876, duration.TicksRemainder);
        }

        [Test]
        public void PositiveTotalProperties()
        {
            Duration duration = Duration.FromStandardDays(2) +
                Duration.FromHours(14) +
                Duration.FromMinutes(35) +
                Duration.FromSeconds(12) +
                Duration.FromMilliseconds(234) +
                Duration.FromTicks(9876);
            Assert.AreEqual(14 + 2 * NodaConstants.HoursPerStandardDay, duration.TotalHours);
            Assert.AreEqual(35 +
                14 * NodaConstants.MinutesPerHour +
                2 * NodaConstants.MinutesPerStandardDay,
                duration.TotalMinutes);
            Assert.AreEqual(12 +
                35 * NodaConstants.SecondsPerMinute +
                14 * NodaConstants.SecondsPerHour +
                2 * NodaConstants.SecondsPerStandardDay,
                duration.TotalSeconds);
            Assert.AreEqual(234 +
                12 * NodaConstants.MillisecondsPerSecond +
                35 * NodaConstants.MillisecondsPerMinute +
                14 * NodaConstants.MillisecondsPerHour +
                2 * NodaConstants.MillisecondsPerStandardDay,
                duration.TotalMilliseconds);
            Assert.AreEqual(9876 + 
                234 * NodaConstants.TicksPerMillisecond + 
                12 * NodaConstants.TicksPerSecond +
                35 * NodaConstants.TicksPerMinute +
                14 * NodaConstants.TicksPerHour +
                2 * NodaConstants.TicksPerStandardDay,
                duration.Ticks);
        }

        [Test]
        public void NegativeRemainderProperties()
        {
            Duration duration = Duration.FromStandardDays(2) +
                Duration.FromHours(14) +
                Duration.FromMinutes(35) +
                Duration.FromSeconds(12) +
                Duration.FromMilliseconds(234) +
                Duration.FromTicks(9876);
            duration = -duration;
            Assert.AreEqual(-2, duration.StandardDays);
            Assert.AreEqual(-14, duration.HoursRemainder);
            Assert.AreEqual(-35, duration.MinutesRemainder);
            Assert.AreEqual(-12, duration.SecondsRemainder);
            Assert.AreEqual(-234, duration.MillisecondsRemainder);
            Assert.AreEqual(-9876, duration.TicksRemainder);
        }

        [Test]
        public void NegativeTotalProperties()
        {
            Duration duration = Duration.FromStandardDays(2) +
                Duration.FromHours(14) +
                Duration.FromMinutes(35) +
                Duration.FromSeconds(12) +
                Duration.FromMilliseconds(234) +
                Duration.FromTicks(9876);
            duration = -duration;
            Assert.AreEqual(-(14 + 2 * NodaConstants.HoursPerStandardDay), duration.TotalHours);
            Assert.AreEqual(-(35 +
                14 * NodaConstants.MinutesPerHour +
                2 * NodaConstants.MinutesPerStandardDay),
                duration.TotalMinutes);
            Assert.AreEqual(-(12 +
                35 * NodaConstants.SecondsPerMinute +
                14 * NodaConstants.SecondsPerHour +
                2 * NodaConstants.SecondsPerStandardDay),
                duration.TotalSeconds);
            Assert.AreEqual(-(234 +
                12 * NodaConstants.MillisecondsPerSecond +
                35 * NodaConstants.MillisecondsPerMinute +
                14 * NodaConstants.MillisecondsPerHour +
                2 * NodaConstants.MillisecondsPerStandardDay),
                duration.TotalMilliseconds);
            Assert.AreEqual(-(9876 +
                234 * NodaConstants.TicksPerMillisecond +
                12 * NodaConstants.TicksPerSecond +
                35 * NodaConstants.TicksPerMinute +
                14 * NodaConstants.TicksPerHour +
                2 * NodaConstants.TicksPerStandardDay),
                duration.Ticks);
        }
    }
}
