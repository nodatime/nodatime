using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Partials;
using NodaTime.Periods;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalTimeDemo
    {
        [Test]
        public void Construction()
        {
            LocalTime time = new LocalTime(16, 20, 0);
            Assert.AreEqual("16:20:00.000", time.ToString());
        }

        [Test]
        public void Arithmetic()
        {
            LocalTime start = new LocalTime(16, 20, 0);
            LocalTime end = new LocalTime(17, 20, 0);
            Assert.AreEqual(end, start + Hours.One);
        }

        [Test]
        public void Wrapping()
        {
            LocalTime start = new LocalTime(16, 20, 0);
            LocalTime end = new LocalTime(15, 20, 0);
            Assert.AreEqual(end, start + Hours.From(23));
        }
    }
}
