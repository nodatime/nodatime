// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class CalendarSystemTest
    {
        internal static readonly List<CalendarSystem> AllCalendars = CalendarSystem.Ids.Select(id => CalendarSystem.ForId(id)).ToList();

        [Test]
        [TestCaseSource("AllCalendars")]
        public void MaxDate(CalendarSystem calendar)
        {
            // Construct the largest LocalDate we can, and validate that all the properties can be fetched without
            // issues.
            int year = calendar.MaxYear;
            int month = calendar.GetMonthsInYear(year);
            int day = calendar.GetDaysInMonth(year, month);
            ValidateProperties(year, month, day, calendar);
        }

        [Test]
        [TestCaseSource("AllCalendars")]
        public void MinDate(CalendarSystem calendar)
        {
            // Construct the smallest LocalDate we can, and validate that all the properties can be fetched without
            // issues.
            ValidateProperties(calendar.MinYear, 1, 1, calendar);
        }

        private static void ValidateProperties(int year, int month, int day, CalendarSystem calendar)
        {
            var localDate = new LocalDate(year, month, day, calendar);
            Assert.AreEqual(year, localDate.Year);
            Assert.AreEqual(month, localDate.Month);
            Assert.AreEqual(day, localDate.Day);

            foreach (var property in typeof(LocalDate).GetProperties())
            {
                property.GetValue(localDate, null);
            }
        }

    }
}
