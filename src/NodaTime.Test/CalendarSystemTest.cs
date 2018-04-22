// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class CalendarSystemTest
    {
        private static readonly IEnumerable<string> SupportedIds = CalendarSystem.Ids.ToList();
        private static readonly List<CalendarSystem> SupportedCalendars = SupportedIds.Select(CalendarSystem.ForId).ToList();

        [Test]
        [TestCaseSource(nameof(SupportedCalendars))]
        public void MaxDate(CalendarSystem calendar)
        {
            // Construct the largest LocalDate we can, and validate that all the properties can be fetched without
            // issues.
            ValidateProperties(calendar, calendar.MaxDays, calendar.MaxYear);
        }

        [Test]
        [TestCaseSource(nameof(SupportedCalendars))]
        public void MinDate(CalendarSystem calendar)
        {
            // Construct the smallest LocalDate we can, and validate that all the properties can be fetched without
            // issues.
            ValidateProperties(calendar, calendar.MinDays, calendar.MinYear);
        }

        private static void ValidateProperties(CalendarSystem calendar, int daysSinceEpoch, int expectedYear)
        {
            var localDate = new LocalDate(daysSinceEpoch, calendar);
            Assert.AreEqual(expectedYear, localDate.Year);

            foreach (var property in typeof(LocalDate).GetTypeInfo().DeclaredProperties)
            {
                property.GetValue(localDate, null);
            }
        }
    }
}
