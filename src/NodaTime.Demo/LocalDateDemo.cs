using System;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalDateDemo
    {
        [Test]
        public void SimpleConstruction()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            Assert.AreEqual("2010-06-16", date.ToString());
        }

        [Test]
        public void ExplicitCalendar()
        {
            LocalDate date = new LocalDate(2010, 6, 16, CalendarSystem.Iso);
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void Validation()
        {
            LocalDate date = new LocalDate(2008, 2, 29);
            Assert.Throws<ArgumentOutOfRangeException>(() => date.WithYear(2010));
        }

        [Test]
        public void CombineWithTime()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20, 0);
            LocalDateTime dateTime = date + time;
            Assert.AreEqual("ISO: 2010-06-16T16:20:00 LOC", dateTime.ToString());
        }
    }
}