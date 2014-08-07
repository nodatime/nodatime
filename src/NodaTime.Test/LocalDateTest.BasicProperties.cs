// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Utility;
using NUnit.Framework;

namespace NodaTime.Test
{
    public partial class LocalDateTest
    {
        [Test]
        public void EpochProperties()
        {
            LocalDate date = NodaConstants.UnixEpoch.InUtc().Date;
            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(1970, date.YearOfEra);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual((int) IsoDayOfWeek.Thursday, date.DayOfWeek);
            Assert.AreEqual(IsoDayOfWeek.Thursday, date.IsoDayOfWeek);
            Assert.AreEqual(1, date.DayOfYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1970, date.WeekYear);
            Assert.AreEqual(1, date.WeekOfWeekYear);
        }

        [Test]
        public void ArbitraryDateProperties()
        {
            DateTime bclDate = new DateTime(2011, 3, 5, 0, 0, 0, DateTimeKind.Utc);
            DateTime bclEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long bclTicks = bclDate.Ticks - bclEpoch.Ticks;
            int bclDays = (int) (bclTicks / NodaConstants.TicksPerStandardDay);
            LocalDate date = new LocalDate(bclDays, CalendarSystem.Iso);
            Assert.AreEqual(2011, date.Year);
            Assert.AreEqual(2011, date.YearOfEra);
            Assert.AreEqual(5, date.Day);
            Assert.AreEqual((int)IsoDayOfWeek.Saturday, date.DayOfWeek);
            Assert.AreEqual(IsoDayOfWeek.Saturday, date.IsoDayOfWeek);
            Assert.AreEqual(64, date.DayOfYear);
            Assert.AreEqual(3, date.Month);
            Assert.AreEqual(2011, date.WeekYear);
            Assert.AreEqual(9, date.WeekOfWeekYear);
        }

        // See http://stackoverflow.com/questions/8010125
        [Test]
        public void WeekOfWeekYear_ComparisonWithOracle()
        {
            Assert.AreEqual(1, new LocalDate(2007, 12, 31).WeekOfWeekYear);
            Assert.AreEqual(1, new LocalDate(2008, 1, 6).WeekOfWeekYear);
            Assert.AreEqual(2, new LocalDate(2008, 1, 7).WeekOfWeekYear);

            Assert.AreEqual(52, new LocalDate(2008, 12, 28).WeekOfWeekYear);
            Assert.AreEqual(1, new LocalDate(2008, 12, 29).WeekOfWeekYear);
            Assert.AreEqual(1, new LocalDate(2009, 1, 4).WeekOfWeekYear);
            Assert.AreEqual(2, new LocalDate(2009, 1, 5).WeekOfWeekYear);

            Assert.AreEqual(52, new LocalDate(2009, 12, 27).WeekOfWeekYear);
            Assert.AreEqual(53, new LocalDate(2009, 12, 28).WeekOfWeekYear);
            Assert.AreEqual(53, new LocalDate(2010, 1, 3).WeekOfWeekYear);
            Assert.AreEqual(1, new LocalDate(2010, 1, 4).WeekOfWeekYear);
        }

        [Test]
        public void IsoDayOfWeek_AroundEpoch()
        {
            // Test about couple of months around the Unix epoch. If that works, I'm confident the rest will.
            LocalDate date = new LocalDate(1969, 12, 1);
            for (int i = 0; i < 60; i++)
            {
                Assert.AreEqual(
                    BclConversions.ToIsoDayOfWeek(date.AtMidnight().ToDateTimeUnspecified().DayOfWeek),
                    date.IsoDayOfWeek);
                date = date.PlusDays(1);
            }
        }
    }
}
