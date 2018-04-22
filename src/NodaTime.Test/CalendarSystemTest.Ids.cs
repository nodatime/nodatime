// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class CalendarSystemTest
    {
        [Test]
        [TestCaseSource(nameof(SupportedIds))]
        public void ValidId(string id)
        {
            Assert.IsInstanceOf<CalendarSystem>(CalendarSystem.ForId(id));
        }

        [Test]
        [TestCaseSource(nameof(SupportedIds))]
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
                    Assert.AreNotEqual(0, comparison.Compare(firstId, 0, firstId.Length, secondId, 0, firstId.Length, CompareOptions.IgnoreCase),
                        "{0} is a leading substring of {1}", firstId, secondId);
                }
            }
        }

        // Ordinals are similar enough to IDs to keep the tests in this file too...

        [Test, TestCaseSource(nameof(SupportedCalendars))]
        public void ForOrdinal_Roundtrip(CalendarSystem calendar)
        {
            Assert.AreSame(calendar, CalendarSystem.ForOrdinal(calendar.Ordinal));
        }

        [Test, TestCaseSource(nameof(SupportedCalendars))]
        public void ForOrdinalUncached_Roundtrip(CalendarSystem calendar)
        {
            Assert.AreSame(calendar, CalendarSystem.ForOrdinalUncached(calendar.Ordinal));
        }

        [Test]
        public void ForOrdinalUncached_Invalid()
        {
            Assert.Throws<InvalidOperationException>(() => CalendarSystem.ForOrdinalUncached((CalendarOrdinal)9999));
        }
    }
}
