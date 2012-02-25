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
    public partial class LocalDateTest
    {
        [Test]
        public void LocalDateTime()
        {
            LocalDate date = new LocalDate(2011, 6, 29);
            LocalDateTime expected = new LocalDateTime(2011, 6, 29, 0, 0, 0);
            Assert.AreEqual(expected, date.LocalDateTime);
        }

        [Test]
        public void WithCalendar()
        {
            LocalDate isoEpoch = new LocalDate(1970, 1, 1);
            LocalDate julianEpoch = isoEpoch.WithCalendar(CalendarSystem.GetJulianCalendar(4));
            Assert.AreEqual(1969, julianEpoch.Year);
            Assert.AreEqual(12, julianEpoch.Month);
            Assert.AreEqual(19, julianEpoch.Day);
        }
    }
}
