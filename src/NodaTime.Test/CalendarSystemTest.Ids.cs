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
using System.Globalization;
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
            Assert.IsInstanceOf<CalendarSystem>(CalendarSystem.ForId(id));
        }

        [Test]
        [TestCaseSource(typeof(CalendarSystem), "Ids")]
        public void IdsAreCaseSensitive(string id)
        {
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId(id.ToLowerInvariant()));
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
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("bad"));
        }

        [Test]
        public void NoSubstrings()
        {
            CompareInfo comparison = CultureInfo.InvariantCulture.CompareInfo;
            foreach (var firstId in CalendarSystem.Ids)
            {
                foreach (var secondId in CalendarSystem.Ids)
                {
                    // We're looking for firstId being a substring of secondId, which can only
                    // happen if firstId is shorter...
                    if (firstId.Length >= secondId.Length)
                    {
                        continue;
                    }
                    Assert.AreNotEqual(0, comparison.Compare(firstId, 0, firstId.Length, secondId, 0, firstId.Length, CompareOptions.IgnoreCase));
                }
            }
        }
    }
}
