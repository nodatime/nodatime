// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NUnit.Framework;
using System;
using System.Globalization;

namespace NodaTime.Test.Calendars
{
    public class WeekYearRulesTest
    {
        [Test]
        public void UnsupportedCalendarWeekRule()
        {
            Assert.Throws<ArgumentException>(() => WeekYearRules.FromCalendarWeekRule(CalendarWeekRule.FirstDay + 1000, DayOfWeek.Monday));
        }
    }
}
