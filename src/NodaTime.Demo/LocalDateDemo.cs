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

using System.Globalization;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalDateDemo
    {
        [Test]
        public void SimpleConstruction()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            Assert.AreEqual("2010-06-16", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        [Test]
        public void ExplicitCalendar()
        {
            LocalDate date = new LocalDate(2010, 6, 16, CalendarSystem.Iso);
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void CombineWithTime()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20);
            LocalDateTime dateTime = date + time;
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 16, 20, 0), dateTime);
        }
    }
}