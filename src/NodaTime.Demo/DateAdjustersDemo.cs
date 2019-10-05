using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.Demo
{
    public class DateAdjustersDemo
    {
        [Test]
        public void AddPeriod()
        {
            LocalDateTime localDateTime = new LocalDateTime(1985, 10, 26, 1, 18);
            Offset offset = Offset.FromHours(-5);
            OffsetDateTime original = new OffsetDateTime(localDateTime, offset);

            var dateAdjuster = Snippet.For(DateAdjusters.AddPeriod(Period.FromYears(30)));
            OffsetDateTime updated = original.With(dateAdjuster);

            Assert.AreEqual(
                new LocalDateTime(2015, 10, 26, 1, 18),
                updated.LocalDateTime);
            Assert.AreEqual(original.Offset, updated.Offset);
        }

        [Test]
        public void DayOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 19);

            var adjuster = Snippet.For(DateAdjusters.DayOfMonth(19));

            Assert.AreEqual(end, adjuster(start));
        }

        [Test]
        public void Month()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 2, 27);

            var adjuster = Snippet.For(DateAdjusters.Month(2));

            Assert.AreEqual(end, adjuster(start));
        }

        [Test]
        public void Next()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 7, 3);

            var adjuster = Snippet.For(DateAdjusters.Next(IsoDayOfWeek.Thursday));

            Assert.AreEqual(end, adjuster(start));
        }

        [Test]
        public void NextOrSame_SameDay()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 27);

            var adjuster = Snippet.For(DateAdjusters.NextOrSame(IsoDayOfWeek.Friday));

            Assert.AreEqual(end, adjuster(start));
        }
    }
}
