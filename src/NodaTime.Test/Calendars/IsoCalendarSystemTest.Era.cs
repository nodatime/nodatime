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
    public partial class IsoCalendarSystemTest
    {
        [Test]
        public void GetMaxYearOfEra()
        {
            LocalDate date = new LocalDate(Iso.MaxYear, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMaxYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(Iso.MinYear, 1, 1);
            Assert.AreEqual(Iso.MinYear, date.Year);
            Assert.AreEqual(date.YearOfEra, Iso.GetMaxYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetMinYearOfEra()
        {
            LocalDate date = new LocalDate(1, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMinYearOfEra(Era.Common));
            Assert.AreEqual(Era.Common, date.Era);
            date = new LocalDate(0, 1, 1);
            Assert.AreEqual(date.YearOfEra, Iso.GetMinYearOfEra(Era.BeforeCommon));
            Assert.AreEqual(Era.BeforeCommon, date.Era);
        }

        [Test]
        public void GetAbsoluteYear()
        {
            Assert.AreEqual(1, Iso.GetAbsoluteYear(1, Era.Common));
            Assert.AreEqual(0, Iso.GetAbsoluteYear(1, Era.BeforeCommon));
            Assert.AreEqual(-1, Iso.GetAbsoluteYear(2, Era.BeforeCommon));
            Assert.AreEqual(Iso.MaxYear, Iso.GetAbsoluteYear(Iso.GetMaxYearOfEra(Era.Common), Era.Common));
            Assert.AreEqual(Iso.MinYear, Iso.GetAbsoluteYear(Iso.GetMaxYearOfEra(Era.BeforeCommon), Era.BeforeCommon));
        }
    }
}
