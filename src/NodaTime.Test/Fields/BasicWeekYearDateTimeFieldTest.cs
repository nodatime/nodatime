// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Calendars;
using NodaTime.Fields;
using NUnit.Framework;

namespace NodaTime.Test.Fields
{
    [TestFixture]
    public class BasicWeekYearDateTimeFieldTest
    {
        private static readonly BasicWeekYearDateTimeField Field = (BasicWeekYearDateTimeField)GregorianCalendarSystem.IsoHelper.Instance.Fields.WeekYear;

        [Test]
        public void SetValue_Simple()
        {
            // Week-year 2013, week 11, Saturday
            LocalDateTime start = new LocalDateTime(2013, 3, 16, 20, 35, 30);
            LocalInstant adjusted = Field.SetValue(start.LocalInstant, 2010);
            LocalDateTime actual = new LocalDateTime(adjusted);
            Assert.AreEqual(11, actual.WeekOfWeekYear);
            LocalDateTime expected = new LocalDateTime(2010, 3, 20, 20, 35, 30);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SetValue_TruncationFromWeek53()
        {
            // Week-year 2009, week 53, Monday
            LocalDateTime start = new LocalDateTime(2009, 12, 28, 20, 35, 30);
            LocalInstant adjusted = Field.SetValue(start.LocalInstant, 2010);
            LocalDateTime actual = new LocalDateTime(adjusted);
            // Week-of-week-year has been adjusted
            Assert.AreEqual(52, actual.WeekOfWeekYear);
            LocalDateTime expected = new LocalDateTime(2010, 12, 27, 20, 35, 30);
            Assert.AreEqual(expected, actual);
        }
    }
}
