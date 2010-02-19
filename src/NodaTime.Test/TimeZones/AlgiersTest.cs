using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Algiers had DST until May 1st 1981, after which time it didn't have any - so
    /// we use that to test a time zone whose transitions run out. (When Algiers
    /// decided to stop using DST, it changed its standard offset to be what had previously
    /// been its DST offset, i.e. +1.)
    /// </summary>
    [TestFixture]
    public class AlgiersTest
    {
        private static readonly IDateTimeZone Algiers = DateTimeZones.ForId("Africa/Algiers");

        [Test]
        public void NextTransition_RunsOutOfTransitions()
        {
            Instant april1981 = new ZonedDateTime(1981, 4, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
            Transition? lastTransition = Algiers.NextTransition(april1981);
            Assert.IsNotNull(lastTransition);

            Transition expected = new Transition(new ZonedDateTime(1981, 5, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant(),
                Offset.Zero, Offset.ForHours(1));
            Assert.AreEqual(expected, lastTransition.Value);

            Assert.IsNull(Algiers.NextTransition(expected.Instant));
        }
    }
}
