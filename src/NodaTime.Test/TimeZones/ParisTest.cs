using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for the Paris time zone. This exercises functionality within various classes.
    /// Paris varies between +1 (standard) and +2 (DST); transitions occur at 2am or 3am wall time,
    /// which is always 1am UTC.
    /// 2009 fall transition: October 25th
    /// 2010 spring transition: March 28th
    /// 2010 fall transition: October 31st
    /// 2011 spring transition: March 27th
    /// </summary>
    [TestFixture]
    public class ParisTest
    {
        // Make sure we deal with the uncached time zone
        private static readonly DateTimeZone Paris = DateTimeZoneProviders.Tzdb["Europe/Paris"].Uncached();

        // Until 1911, Paris was 9 minutes and 21 seconds off UTC.
        private static readonly Offset InitialOffset = TestObjects.CreatePositiveOffset(0, 9, 21, 0);

        [Test]
        public void FirstTransitions()
        {
            // Paris had a name change in 1891, and then moved from +0:09:21 to UTC in 1911
            var nameChangeInstant = Instant.FromUtc(1891, 3, 14, 23, 51, 39);
            var utcChangeInstant = Instant.FromUtc(1911, 3, 10, 23, 51, 39);

            var beforeNameChange = Paris.GetZoneInterval(nameChangeInstant - Duration.Epsilon);
            var afterNameChange = Paris.GetZoneInterval(nameChangeInstant);
            var afterSmallChange = Paris.GetZoneInterval(utcChangeInstant);

            Assert.AreEqual("LMT", beforeNameChange.Name);
            Assert.AreEqual(InitialOffset, beforeNameChange.WallOffset);

            Assert.AreEqual("PMT", afterNameChange.Name);
            Assert.AreEqual(InitialOffset, afterNameChange.WallOffset);

            Assert.AreEqual("WET", afterSmallChange.Name);
            Assert.AreEqual(Offset.Zero, afterSmallChange.WallOffset);
        }
    }
}
