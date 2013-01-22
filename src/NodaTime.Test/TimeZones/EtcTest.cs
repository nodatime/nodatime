using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Tests for fixed "Etc/GMT+x" zones. These just test that the time zones are built
    /// appropriately; FixedDateTimeZoneTest takes care of the rest.
    /// </summary>
    [TestFixture]
    public class EtcTest
    {
        [Test]
        public void FixedEasternZone()
        {
            string id = "Etc/GMT+5";
            var zone = DateTimeZoneProviders.Tzdb[id];
            Assert.AreEqual(id, zone.Id);
            Assert.IsInstanceOf<FixedDateTimeZone>(zone);
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(Offset.FromHours(-5), fixedZone.Offset);
        }

        [Test]
        public void FixedWesternZone()
        {
            string id = "Etc/GMT-4";
            var zone = DateTimeZoneProviders.Tzdb[id];
            Assert.AreEqual(id, zone.Id);
            Assert.IsInstanceOf<FixedDateTimeZone>(zone);
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(Offset.FromHours(4), fixedZone.Offset);
        }
    }
}
