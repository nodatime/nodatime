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
