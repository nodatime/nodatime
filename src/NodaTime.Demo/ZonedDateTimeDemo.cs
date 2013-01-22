using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    internal class ZonedDateTimeDemo
    {
        private static readonly DateTimeZone Dublin = DateTimeZoneProviders.Tzdb["Europe/Dublin"];

        [Test]
        public void Construction()
        {
            ZonedDateTime dt = Dublin.AtStrictly(new LocalDateTime(2010, 6, 9, 15, 15, 0));

            Assert.AreEqual(15, dt.Hour);
            Assert.AreEqual(2010, dt.Year);
            // Not 21... we're not in the Gregorian calendar!
            Assert.AreEqual(20, dt.CenturyOfEra);

            Instant instant = Instant.FromUtc(2010, 6, 9, 14, 15, 0);
            Assert.AreEqual(instant, dt.ToInstant());
        }

        [Test]
        public void Ambiguity()
        {
            Assert.Throws<AmbiguousTimeException>(() => Dublin.AtStrictly(new LocalDateTime(2010, 10, 31, 1, 15, 0)));
        }

        [Test]
        public void Impossibility()
        {
            Assert.Throws<SkippedTimeException>(() => Dublin.AtStrictly(new LocalDateTime(2010, 3, 28, 1, 15, 0)));
        }
    }
}