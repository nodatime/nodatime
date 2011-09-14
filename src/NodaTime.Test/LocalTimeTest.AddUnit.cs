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

namespace NodaTime.Test
{
    public partial class LocalTimeTest
    {
        [Test]
        public void AddHours_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(14, 15, 8);
            LocalTime expectedBackward = new LocalTime(10, 15, 8);
            Assert.AreEqual(expectedForward, start.AddHours(2));
            Assert.AreEqual(expectedBackward, start.AddHours(-2));
        }

        [Test]
        public void AddHours_CrossingDayBoundary()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(8, 15, 8);
            Assert.AreEqual(expected, start.AddHours(20));
            Assert.AreEqual(start, start.AddHours(20).AddHours(-20));
        }

        [Test]
        public void AddHours_CrossingSeveralDaysBoundary()
        {
            // Christmas day + 10 days and 1 hour
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(13, 15, 8);
            Assert.AreEqual(expected, start.AddHours(241));
            Assert.AreEqual(start, start.AddHours(241).AddHours(-241));
        }

        // Having tested that hours cross boundaries correctly, the other time unit
        // tests are straightforward
        [Test]
        public void AddMinutes_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 17, 8);
            LocalTime expectedBackward = new LocalTime(12, 13, 8);
            Assert.AreEqual(expectedForward, start.AddMinutes(2));
            Assert.AreEqual(expectedBackward, start.AddMinutes(-2));
        }

        [Test]
        public void AddSeconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 18);
            LocalTime expectedBackward = new LocalTime(12, 14, 58);
            Assert.AreEqual(expectedForward, start.AddSeconds(10));
            Assert.AreEqual(expectedBackward, start.AddSeconds(-10));
        }

        [Test]
        public void AddMilliseconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 700);
            LocalTime expectedBackward = new LocalTime(12, 15, 7, 900);
            Assert.AreEqual(expectedForward, start.AddMilliseconds(400));
            Assert.AreEqual(expectedBackward, start.AddMilliseconds(-400));
        }

        [Test]
        public void AddTicks_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300, 7500);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 301, 1500);
            LocalTime expectedBackward = new LocalTime(12, 15, 8, 300, 3500);
            Assert.AreEqual(expectedForward, start.AddTicks(4000));
            Assert.AreEqual(expectedBackward, start.AddTicks(-4000));
        }

        [Test]
        public void AddTicks_Long()
        {
            Assert.IsTrue(NodaConstants.TicksPerStandardDay > int.MaxValue);
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 9);
            LocalTime expectedBackward = new LocalTime(12, 15, 7);
            Assert.AreEqual(expectedForward, start.AddTicks(NodaConstants.TicksPerStandardDay + NodaConstants.TicksPerSecond));
            Assert.AreEqual(expectedBackward, start.AddTicks(-NodaConstants.TicksPerStandardDay - NodaConstants.TicksPerSecond));
        }

    }
}
