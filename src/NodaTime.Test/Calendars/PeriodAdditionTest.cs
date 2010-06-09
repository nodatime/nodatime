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

using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    /// <summary>
    /// Tests for adding periods. These use LocalDateTime for simplicity; its + operator
    /// simply delegates to the relevant calendar. Likewise most tests use the ISO calendar.
    /// </summary>
    [TestFixture]
    public class PeriodAdditionTest
    {
        [Test]
        public void DayCrossingMonthBoundary()
        {
            LocalDateTime start = new LocalDateTime(2010, 2, 20, 10, 0);
            LocalDateTime result = start + Days.From(10);
            Assert.AreEqual(new LocalDateTime(2010, 3, 2, 10, 0), result);
        }

        [Test]
        public void AddOneYearOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Years.One;
            // Feb 29th becomes Feb 28th
            Assert.AreEqual(new LocalDateTime(2013, 2, 28, 10, 0), result);
        }

        [Test]
        public void AddFourYearsOnLeapDay()
        {
            LocalDateTime start = new LocalDateTime(2012, 2, 29, 10, 0);
            LocalDateTime result = start + Years.From(4);
            // Feb 29th is still valid in 2016
            Assert.AreEqual(new LocalDateTime(2016, 2, 29, 10, 0), result);
        }

        [Test]
        public void AddYearMonthDay()
        {
            // One year, one month, two days
            IPeriod period = new Period(1, 1, 0, 2, 0, 0, 0, 0);
            LocalDateTime start = new LocalDateTime(2007, 1, 30, 0, 0);
            // Periods are added in order, so this becomes...
            // Add one year: Jan 30th 2008
            // Add one month: Feb 29th 2008
            // Add two days: March 2nd 2008
            // If we added the days first, we'd end up with March 1st instead.
            LocalDateTime result = start + period;
            Assert.AreEqual(new LocalDateTime(2008, 3, 2, 0, 0), result);
        }
    }
}
