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
using NodaTime.Calendars;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class GJEraDateTimeFieldTest
    {
        [Test]
        public void YearOfEraPreserved()
        {
            var start = new LocalDate(Era.Common, 1980, 6, 19).LocalDateTime.LocalInstant;
            var bc = CalendarSystem.Iso.Fields.Era.SetValue(start, 0);
            Assert.AreEqual(1980, CalendarSystem.Iso.Fields.YearOfEra.GetValue(bc));
            var backAgain = CalendarSystem.Iso.Fields.Era.SetValue(bc, 1);
            Assert.AreEqual(start, backAgain);
        }
    }
}
