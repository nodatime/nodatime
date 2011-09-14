#region Copyright and license information
// Copyright 281-289 Stephen Colebourne
// Copyright 289-2010 Jon Skeet
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
    public partial class LocalDateTimeTest
    {
        [Test]
        public void AddYear_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 6, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2016, 6, 26, 12, 15, 8);
            Assert.AreEqual(expected, start.AddYears(5));

            expected = new LocalDateTime(2006, 6, 26, 12, 15, 8);
            Assert.AreEqual(expected, start.AddYears(-5));
        }

        [Test]
        public void AddYear_LeapToNonLeap()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2013, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.AddYears(1));

            expected = new LocalDateTime(2011, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.AddYears(-1));
        }

        [Test]
        public void AddYear_LeapToLeap()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2016, 2, 29, 12, 15, 8);
            Assert.AreEqual(expected, start.AddYears(4));
        }

        [Test]
        public void AddMonth_Simple()
        {
            LocalDateTime start = new LocalDateTime(2012, 4, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 8, 15, 12, 15, 8);
            Assert.AreEqual(expected, start.AddMonths(4));
        }

        [Test]
        public void AddMonth_ChangingYear()
        {
            LocalDateTime start = new LocalDateTime(2012, 10, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2013, 2, 15, 12, 15, 8);
            Assert.AreEqual(expected, start.AddMonths(4));
        }

        [Test]
        public void AddMonth_WithTruncation()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 30, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 2, 28, 12, 15, 8);
            Assert.AreEqual(expected, start.AddMonths(1));
        }

        [Test]
        public void AddDays_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 15, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 1, 23, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(8));

            expected = new LocalDateTime(2011, 1, 7, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(-8));
        }

        [Test]
        public void AddDays_MonthBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 1, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 2, 3, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.AddDays(8).AddDays(-8));
        }

        [Test]
        public void AddDays_YearBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 12, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 1, 3, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.AddDays(8).AddDays(-8));
        }

        [Test]
        public void AddDays_EndOfFebruary_InLeapYear()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 3, 5, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(8));
            // Round-trip back across the boundary
            Assert.AreEqual(start, start.AddDays(8).AddDays(-8));
        }

        [Test]
        public void AddDays_EndOfFebruary_NotInLeapYear()
        {
            LocalDateTime start = new LocalDateTime(2011, 2, 26, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 3, 6, 12, 15, 8);
            Assert.AreEqual(expected, start.AddDays(8));

            // Round-trip back across the boundary
            Assert.AreEqual(start, start.AddDays(8).AddDays(-8));
        }

        [Test]
        public void AddWeeks_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 23, 12, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 3, 12, 12, 15, 8);
            Assert.AreEqual(expectedForward, start.AddWeeks(3));
            Assert.AreEqual(expectedBackward, start.AddWeeks(-3));
        }

        [Test]
        public void AddHours_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 14, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 10, 15, 8);
            Assert.AreEqual(expectedForward, start.AddHours(2));
            Assert.AreEqual(expectedBackward, start.AddHours(-2));
        }

        [Test]
        public void AddHours_CrossingDayBoundary()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2011, 4, 3, 8, 15, 8);
            Assert.AreEqual(expected, start.AddHours(20));
            Assert.AreEqual(start, start.AddHours(20).AddHours(-20));
        }

        [Test]
        public void AddHours_CrossingYearBoundary()
        {
            // Christmas day + 10 days and 1 hour
            LocalDateTime start = new LocalDateTime(2011, 12, 25, 12, 15, 8);
            LocalDateTime expected = new LocalDateTime(2012, 1, 4, 13, 15, 8);
            Assert.AreEqual(expected, start.AddHours(241));
            Assert.AreEqual(start, start.AddHours(241).AddHours(-241));
        }

        // Having tested that hours cross boundaries correctly, the other time unit
        // tests are straightforward
        [Test]
        public void AddMinutes_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 17, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 13, 8);
            Assert.AreEqual(expectedForward, start.AddMinutes(2));
            Assert.AreEqual(expectedBackward, start.AddMinutes(-2));
        }

        [Test]
        public void AddSeconds_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 15, 18);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 14, 58);
            Assert.AreEqual(expectedForward, start.AddSeconds(10));
            Assert.AreEqual(expectedBackward, start.AddSeconds(-10));
        }

        [Test]
        public void AddMilliseconds_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8, 300);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 15, 8, 700);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 15, 7, 900);
            Assert.AreEqual(expectedForward, start.AddMilliseconds(400));
            Assert.AreEqual(expectedBackward, start.AddMilliseconds(-400));
        }

        [Test]
        public void AddTicks_Simple()
        {
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8, 300, 7500);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 2, 12, 15, 8, 301, 1500);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 2, 12, 15, 8, 300, 3500);
            Assert.AreEqual(expectedForward, start.AddTicks(4000));
            Assert.AreEqual(expectedBackward, start.AddTicks(-4000));
        }

        [Test]
        public void AddTicks_Long()
        {
            Assert.IsTrue(NodaConstants.TicksPerStandardDay > int.MaxValue);
            LocalDateTime start = new LocalDateTime(2011, 4, 2, 12, 15, 8);
            LocalDateTime expectedForward = new LocalDateTime(2011, 4, 3, 12, 15, 8);
            LocalDateTime expectedBackward = new LocalDateTime(2011, 4, 1, 12, 15, 8);
            Assert.AreEqual(expectedForward, start.AddTicks(NodaConstants.TicksPerStandardDay));
            Assert.AreEqual(expectedBackward, start.AddTicks(-NodaConstants.TicksPerStandardDay));
        }
    }
}
