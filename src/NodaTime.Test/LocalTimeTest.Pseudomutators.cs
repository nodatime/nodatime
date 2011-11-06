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
        public void PlusHours_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(14, 15, 8);
            LocalTime expectedBackward = new LocalTime(10, 15, 8);
            Assert.AreEqual(expectedForward, start.PlusHours(2));
            Assert.AreEqual(expectedBackward, start.PlusHours(-2));
        }

        [Test]
        public void PlusHours_CrossingDayBoundary()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(8, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(20));
            Assert.AreEqual(start, start.PlusHours(20).PlusHours(-20));
        }

        [Test]
        public void PlusHours_CrossingSeveralDaysBoundary()
        {
            // Christmas day + 10 days and 1 hour
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expected = new LocalTime(13, 15, 8);
            Assert.AreEqual(expected, start.PlusHours(241));
            Assert.AreEqual(start, start.PlusHours(241).PlusHours(-241));
        }

        // Having tested that hours cross boundaries correctly, the other time unit
        // tests are straightforward
        [Test]
        public void PlusMinutes_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 17, 8);
            LocalTime expectedBackward = new LocalTime(12, 13, 8);
            Assert.AreEqual(expectedForward, start.PlusMinutes(2));
            Assert.AreEqual(expectedBackward, start.PlusMinutes(-2));
        }

        [Test]
        public void PlusSeconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 18);
            LocalTime expectedBackward = new LocalTime(12, 14, 58);
            Assert.AreEqual(expectedForward, start.PlusSeconds(10));
            Assert.AreEqual(expectedBackward, start.PlusSeconds(-10));
        }

        [Test]
        public void PlusMilliseconds_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 700);
            LocalTime expectedBackward = new LocalTime(12, 15, 7, 900);
            Assert.AreEqual(expectedForward, start.PlusMilliseconds(400));
            Assert.AreEqual(expectedBackward, start.PlusMilliseconds(-400));
        }

        [Test]
        public void PlusTicks_Simple()
        {
            LocalTime start = new LocalTime(12, 15, 8, 300, 7500);
            LocalTime expectedForward = new LocalTime(12, 15, 8, 301, 1500);
            LocalTime expectedBackward = new LocalTime(12, 15, 8, 300, 3500);
            Assert.AreEqual(expectedForward, start.PlusTicks(4000));
            Assert.AreEqual(expectedBackward, start.PlusTicks(-4000));
        }

        [Test]
        public void PlusTicks_Long()
        {
            Assert.IsTrue(NodaConstants.TicksPerStandardDay > int.MaxValue);
            LocalTime start = new LocalTime(12, 15, 8);
            LocalTime expectedForward = new LocalTime(12, 15, 9);
            LocalTime expectedBackward = new LocalTime(12, 15, 7);
            Assert.AreEqual(expectedForward, start.PlusTicks(NodaConstants.TicksPerStandardDay + NodaConstants.TicksPerSecond));
            Assert.AreEqual(expectedBackward, start.PlusTicks(-NodaConstants.TicksPerStandardDay - NodaConstants.TicksPerSecond));
        }

    }
}
