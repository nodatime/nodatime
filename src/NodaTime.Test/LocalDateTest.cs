using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalDateTest
    {
        /// <summary>
        ///   Using the default constructor is equivalent to January 1st 1970, UTC, ISO calendar
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalDate();
            Assert.AreEqual(NodaConstants.UnixEpoch.InUtc().LocalDateTime.Date, actual);
        }

    }
}
