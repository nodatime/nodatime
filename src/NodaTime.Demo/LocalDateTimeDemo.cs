using System.Globalization;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalDateTimeDemo
    {
        [Test]
        public void SimpleConstruction()
        {
            CalendarSystem calendar = CalendarSystem.Iso;
            LocalDateTime dt = new LocalDateTime(2010, 6, 16, 16, 20, calendar);
            Assert.AreEqual(20, dt.Minute);
        }

        [Test]
        public void ImplicitIsoCalendar()
        {
            LocalDateTime dt = new LocalDateTime(2010, 6, 16, 16, 20);
            Assert.AreEqual(20, dt.Minute);
        }

        [Test]
        public void TestToString()
        {
            LocalDateTime dt = new LocalDateTime(2010, 6, 16, 16, 20);
            Assert.AreEqual("2010-06-16T16:20:00", dt.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}