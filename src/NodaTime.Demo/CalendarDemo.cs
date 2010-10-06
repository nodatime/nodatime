using NodaTime.Calendars;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class CalendarDemo
    {
        [Test]
        public void WorkingOutLocalInstantsFromFields()
        {
            ICalendarSystem calendar = IsoCalendarSystem.Instance;
            LocalInstant local = calendar.GetLocalInstant(2010, 6, 16, 16, 20);
            Assert.AreEqual("2010-06-16T16:20:00 LOC", local.ToString());
        }

        [Test]
        public void FieldAccess()
        {
            ICalendarSystem calendar = IsoCalendarSystem.Instance;
            LocalInstant local = calendar.GetLocalInstant(2010, 6, 16, 16, 20);

            int minute = calendar.Fields.MinuteOfHour.GetValue(local);
            Assert.AreEqual(20, minute);
        }
    }
}
