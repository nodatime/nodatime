using System.Linq;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    /// <summary>
    /// Tests for code in DateTimeZone and code which will be moving out
    /// of DateTimeZones into DateTimeZone over time.
    /// </summary>
    [TestFixture]
    public class DateTimeZoneTest
    {
        [Test(Description = "Test for issue 7 in bug tracker")]
        public void IterateOverIds()
        {
            // According to bug, this would go bang
            int count = DateTimeZone.Ids.Count();

            Assert.IsTrue(count > 1);
            int utcCount = DateTimeZone.Ids.Count(id => id == DateTimeZone.UtcId);
            Assert.AreEqual(1, utcCount);
        }
    }
}
