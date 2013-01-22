using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public partial class LocalTimeTest
    {
        [Test]
        public void ClockHourOfHalfDay()
        {
            Assert.AreEqual(12, new LocalTime(0, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(1, 0).ClockHourOfHalfDay);
            Assert.AreEqual(12, new LocalTime(12, 0).ClockHourOfHalfDay);
            Assert.AreEqual(1, new LocalTime(13, 0).ClockHourOfHalfDay);
            Assert.AreEqual(11, new LocalTime(23, 0).ClockHourOfHalfDay);
        }

        /// <summary>
        ///   Using the default constructor is equivalent to midnight
        /// </summary>
        [Test]
        public void DefaultConstructor()
        {
            var actual = new LocalTime();
            Assert.AreEqual(LocalTime.Midnight, actual);
        }
    }
}