using NUnit.Framework;

namespace NodaTime.Test.Calendars
{
    public partial class IsoCalendarSystemTest
    {
        [Test]
        public void TimeFields_HalfDayOfDay()
        {
            var sut = isoFields.HalfDayOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("HalfDayOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_ClockHourOfHalfDay()
        {
            var sut = isoFields.ClockHourOfHalfDay;

            Assert.That(sut.ToString(), Is.EqualTo("ClockHourOfHalfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_HourOfHalfDay()
        {
            var sut = isoFields.HourOfHalfDay;

            Assert.That(sut.ToString(), Is.EqualTo("HourOfHalfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_ClockHourOfDay()
        {
            var sut = isoFields.ClockHourOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("ClockHourOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_HourOfDay()
        {
            var sut = isoFields.HourOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("HourOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_MinuteOfDay()
        {
            var sut = isoFields.MinuteOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("MinuteOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_SecondOfDay()
        {
            var sut = isoFields.SecondOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("SecondOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_SecondOfMinute()
        {
            var sut = isoFields.SecondOfMinute;

            Assert.That(sut.ToString(), Is.EqualTo("SecondOfMinute"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_MillisecondOfDay()
        {
            var sut = isoFields.MillisecondOfDay;

            Assert.That(sut.ToString(), Is.EqualTo("MillisecondOfDay"));
            Assert.That(sut.IsSupported, Is.True);
        }

        [Test]
        public void TimeFields_MillisecondOfSecond()
        {
            var sut = isoFields.MillisecondOfSecond;

            Assert.That(sut.ToString(), Is.EqualTo("MillisecondOfSecond"));
            Assert.That(sut.IsSupported, Is.True);
        }
    }
}