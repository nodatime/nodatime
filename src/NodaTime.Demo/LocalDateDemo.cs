using System.Globalization;
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
            Assert.AreEqual("2010-06-16", date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        [Test]
        public void ExplicitCalendar()
        {
            LocalDate date = new LocalDate(2010, 6, 16, CalendarSystem.Iso);
            Assert.AreEqual(new LocalDate(2010, 6, 16), date);
        }

        [Test]
        public void CombineWithTime()
        {
            LocalDate date = new LocalDate(2010, 6, 16);
            LocalTime time = new LocalTime(16, 20);
            LocalDateTime dateTime = date + time;
            Assert.AreEqual(new LocalDateTime(2010, 6, 16, 16, 20, 0), dateTime);
        }
    }
}