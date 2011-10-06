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

using NUnit.Framework;
using NodaTime.Calendars;

namespace NodaTime.Test.Calendars
{
    public partial class JulianCalendarSystemTest
    {
        [Test]
        public void GetMaxYearOfEra()
        {
            LocalDate date = new LocalDate(Julian.MaxYear, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMaxYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(Julian.MinYear, 1, 1, Julian);
            Assert.AreEqual(Julian.MinYear, date.Year);
            Assert.AreEqual(date.YearOfEra, Julian.GetMaxYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetMinYearOfEra()
        {
            LocalDate date = new LocalDate(1, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMinYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(0, 1, 1, Julian);
            Assert.AreEqual(date.YearOfEra, Julian.GetMinYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetAbsoluteYear()
        {
            Assert.AreEqual(1, Julian.GetAbsoluteYear(1, Era.Common));
            Assert.AreEqual(0, Julian.GetAbsoluteYear(1, Era.BeforeCommon));
            Assert.AreEqual(-1, Julian.GetAbsoluteYear(2, Era.BeforeCommon));
            Assert.AreEqual(Julian.MaxYear, Julian.GetAbsoluteYear(Julian.GetMaxYearOfEra(Era.Common), Era.Common));
            Assert.AreEqual(Julian.MinYear, Julian.GetAbsoluteYear(Julian.GetMaxYearOfEra(Era.BeforeCommon), Era.BeforeCommon));
        }
    }
}
