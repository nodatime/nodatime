using NUnit.Framework;
using NodaTime;
using NodaTime.TimeZones;
using NodaTime.ZoneInfoCompiler.Tzdb;

namespace ZoneInfoCompiler.Test.Tzdb
{
    /// <summary>
    /// Tests for DateTimeZoneBuilder; currently only scant coverage based on bugs which have
    /// previously been found.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneBuilderTest
    {
        [Test]
        public void FixedZone_Western()
        {
            var offset = Offset.FromHours(-5);
            var builder = new DateTimeZoneBuilder();
            builder.SetStandardOffset(offset);
            builder.SetFixedSavings("GMT+5", Offset.Zero);
            var zone = builder.ToDateTimeZone("GMT+5");
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }

        [Test]
        public void FixedZone_Eastern()
        {
            var offset = Offset.FromHours(5);
            var builder = new DateTimeZoneBuilder();
            builder.SetStandardOffset(offset);
            builder.SetFixedSavings("GMT-5", Offset.Zero);
            var zone = builder.ToDateTimeZone("GMT-5");
            FixedDateTimeZone fixedZone = (FixedDateTimeZone)zone;
            Assert.AreEqual(offset, fixedZone.Offset);
        }
    }
}
