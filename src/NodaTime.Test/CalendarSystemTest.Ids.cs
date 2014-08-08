// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class CalendarSystemTest
    {
        [Test]
        [TestCaseSource("SupportedIds")]
        public void ValidId(string id)
        {
            Assert.IsInstanceOf<CalendarSystem>(CalendarSystem.ForId(id));
        }

        [Test]
        [TestCaseSource("SupportedIds")]
        public void IdsAreCaseSensitive(string id)
        {
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId(id.ToLowerInvariant()));
        }

        [Test]
        public void AllIdsGiveDifferentCalendars()
        {
            var allCalendars = SupportedIds.Select(CalendarSystem.ForId);
            Assert.AreEqual(SupportedIds.Count(), allCalendars.Distinct().Count());
        }

        [Test]
        public void BadId()
        {
            Assert.Throws<KeyNotFoundException>(() => CalendarSystem.ForId("bad"));
        }

        [Test]
        public void GetUmAlQuraCalendar_ThrowsOnUnsupportedPlatform()
        {
            if (!UmAlQuraYearMonthDayCalculator.IsSupported)
            {
                Assert.Throws<NotSupportedException>(() => CalendarSystem.UmAlQura.ToString());
            }
        }

        [Test]
        public void GetUmAlQuraCalendar_WorksOnsupportedPlatform()
        {
            if (UmAlQuraYearMonthDayCalculator.IsSupported)
            {
                Assert.IsNotNull(CalendarSystem.UmAlQura);
            }
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
