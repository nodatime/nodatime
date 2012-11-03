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
using System.Collections.Generic;
using System.Linq;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class CalendarSystemTest
    {
        [Test]
        [TestCaseSource(typeof(CalendarSystem), "Ids")]
        public void ValidId(string id)
        {
            CalendarSystem calendar = CalendarSystem.ForIdOrNull(id, false);
            Assert.AreEqual(id, calendar.Id);
            Assert.IsNotNull(calendar);

            Assert.AreSame(calendar, CalendarSystem.ForId(id));
            Assert.AreSame(calendar, CalendarSystem.ForId(id, false));
            Assert.AreSame(calendar, CalendarSystem.ForId(id, true));
            Assert.AreSame(calendar, CalendarSystem.ForIdOrNull(id, true));
            Assert.AreSame(calendar, CalendarSystem.ForIdOrNull(id.ToLowerInvariant(), true));
            Assert.AreSame(calendar, CalendarSystem.ForId(id.ToLowerInvariant(), true));
            Assert.IsNull(CalendarSystem.ForIdOrNull(id.ToLowerInvariant(), false));
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId(id.ToLowerInvariant(), false));
        }

        [Test]
        public void AllIdsGiveDifferentCalendars()
        {
            var allCalendars = CalendarSystem.Ids.Select(id => CalendarSystem.ForId(id));
            Assert.AreEqual(CalendarSystem.Ids.Count(), allCalendars.Distinct().Count());
        }

        [Test]
        public void BadId()
        {
            Assert.IsNull(CalendarSystem.ForIdOrNull("bad", false));
            Assert.IsNull(CalendarSystem.ForIdOrNull("bad", true));
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("bad", false));
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("bad", true));
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("bad"));
        }
    }
}
