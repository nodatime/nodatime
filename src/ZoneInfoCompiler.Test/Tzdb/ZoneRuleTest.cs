using NodaTime.TimeZones;
using NodaTime.ZoneInfoCompiler.Tzdb;
using NUnit.Framework;

namespace NodaTime.ZoneInfoCompiler.Test.Tzdb
{
    [TestFixture]
    public class ZoneRuleTest
    {
        [Test]
        public void WriteRead()
        {
            var yearOffset = new ZoneYearOffset(TransitionMode.Utc, 10, 31, (int)IsoDayOfWeek.Wednesday, true, Offset.Zero);
            var recurrence = new ZoneRecurrence("bob", Offset.Zero, yearOffset, 1971, 2009);
            var actual = new ZoneRule(recurrence, "D");
            var expected = new ZoneRule(recurrence, "D");
            Assert.AreEqual(expected, actual);
        }
    }
}
