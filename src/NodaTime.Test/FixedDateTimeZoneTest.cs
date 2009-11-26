using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class FixedDateTimeZoneTest
    {
        [Test]
        public void SimpleProperties_ReturnValuesFromConstructor()
        {
            Offset offset = new Offset(1000);
            FixedDateTimeZone zone = new FixedDateTimeZone("test", offset);
            Assert.AreEqual("test", zone.Id);
            // TODO: Use a real LocalDateTime when we've implemented it!
            Assert.AreEqual(offset, zone.GetOffsetFromLocal(LocalInstant.LocalUnixEpoch));
            Assert.AreEqual(offset, zone.GetOffsetFromUtc(Instant.UnixEpoch));
        }

        [Test]
        public void IsFixed_ReturnsTrue()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Offset(1000));
            Assert.IsTrue(zone.IsFixed);
        }

        [Test]
        public void NextTransition_ReturnsNull()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Offset(1000));
            Assert.IsNull(zone.NextTransition(Instant.UnixEpoch));
        }

        [Test]
        public void PreviousTransition_ReturnsNull()
        {
            FixedDateTimeZone zone = new FixedDateTimeZone("test", new Offset(1000));
            Assert.IsNull(zone.PreviousTransition(Instant.UnixEpoch));
        }
    }
}
