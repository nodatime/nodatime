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

using System;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void EpochProperties()
        {
            LocalDate date = new LocalDate(new LocalDateTime(new LocalInstant(0)));
            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(70, date.YearOfCentury);
            Assert.AreEqual(1970, date.YearOfEra);
            Assert.AreEqual(1, date.DayOfMonth);
            Assert.AreEqual((int) IsoDayOfWeek.Thursday, date.DayOfWeek);
            Assert.AreEqual(IsoDayOfWeek.Thursday, date.IsoDayOfWeek);
            Assert.AreEqual(1, date.DayOfYear);
            Assert.AreEqual(1, date.MonthOfYear);
            Assert.AreEqual(1970, date.WeekYear);
            Assert.AreEqual(1, date.WeekOfWeekYear);
        }

        [Test]
        public void ArbitraryDateProperties()
        {
            DateTime bclDate = new DateTime(2011, 3, 5, 0, 0, 0, DateTimeKind.Utc);
            DateTime bclEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            LocalDate date = new LocalDate(new LocalDateTime(new LocalInstant((bclDate - bclEpoch).Ticks)));
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(11, date.YearOfCentury);
            Assert.AreEqual(2011, date.YearOfEra);
            Assert.AreEqual(5, date.DayOfMonth);
            Assert.AreEqual((int)IsoDayOfWeek.Saturday, date.DayOfWeek);
            Assert.AreEqual(IsoDayOfWeek.Saturday, date.IsoDayOfWeek);
            Assert.AreEqual(64, date.DayOfYear);
            Assert.AreEqual(3, date.MonthOfYear);
            Assert.AreEqual(2011, date.WeekYear);
            Assert.AreEqual(9, date.WeekOfWeekYear);
        }
    }
}
