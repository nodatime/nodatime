// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class DateIntervalDemo
    {
        [Test]
        public void Construction()
        {
            var calendar = CalendarSystem.Gregorian;
            LocalDate start = new LocalDate(2017, 1, 1, calendar);
            LocalDate end = new LocalDate(2017, 12, 31, calendar);

            DateInterval interval = Snippet.For(new DateInterval(start, end));

            Assert.AreEqual(365, interval.Length);
            Assert.AreEqual("[2017-01-01, 2017-12-31]", interval.ToString());
            Assert.AreEqual(start, interval.Start);
            Assert.AreEqual(end, interval.End);
            Assert.AreEqual(calendar, interval.Calendar);
        }

        [Test]
        public void Intersection()
        {
            DateInterval januaryToAugust = new DateInterval(
                new LocalDate(2017, 1, 1),
                new LocalDate(2017, 8, 31));

            DateInterval juneToNovember = new DateInterval(
                new LocalDate(2017, 6, 1),
                new LocalDate(2017, 11, 30));

            DateInterval juneToAugust = new DateInterval(
                new LocalDate(2017, 6, 1),
                new LocalDate(2017, 8, 31));

            var result = Snippet.For(januaryToAugust.Intersection(juneToNovember));
            Assert.AreEqual(juneToAugust, result);
        }
        
        [Test]
        public void Contains_LocalDate()
        {
            LocalDate start = new LocalDate(2017, 1, 1);
            LocalDate end = new LocalDate(2017, 12, 31);

            DateInterval interval = new DateInterval(start, end);
            
            var result = Snippet.For(interval.Contains(new LocalDate(2017, 12, 5)));
            Assert.AreEqual(true, result);
        }

        [Test]
        public void Contains_Interval()
        {
            LocalDate start = new LocalDate(2017, 1, 1);
            LocalDate end = new LocalDate(2017, 12, 31);

            DateInterval interval = new DateInterval(start, end);
            DateInterval june = new DateInterval(
                new LocalDate(2017, 6, 1),
                new LocalDate(2017, 6, 30));
            
            var result = Snippet.For(interval.Contains(june));
            Assert.AreEqual(true, result);
        }
    }
}
