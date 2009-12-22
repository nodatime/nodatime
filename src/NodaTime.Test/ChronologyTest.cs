using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class ChronologyTest
    {
        private static readonly IDateTimeZone testZone = new FixedDateTimeZone("tmp", Offset.Zero);

        [Test]
        public void Construction_WithNullArguments_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Chronology(null, IsoCalendarSystem.Instance));
            Assert.Throws<ArgumentNullException>(() => new Chronology(DateTimeZones.Utc, null));
        }

        [Test]
        public void Properties_Return_ConstructorArguments()
        {
            Chronology subject = new Chronology(testZone, GregorianCalendarSystem.Default);
            Assert.AreSame(testZone, subject.Zone);
            Assert.AreSame(GregorianCalendarSystem.Default, subject.Calendar);
        }

        [Test]
        public void IsoForZone()
        {
            Chronology subject = Chronology.IsoForZone(testZone);
            Assert.AreSame(testZone, subject.Zone);
            Assert.AreSame(IsoCalendarSystem.Instance, subject.Calendar);
        }

        [Test]
        public void IsoForZone_WithNullZone_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Chronology.IsoForZone(null));
        }

        [Test]
        public void IsoUtc()
        {
            Assert.AreSame(DateTimeZones.Utc, Chronology.IsoUtc.Zone);
            Assert.AreSame(IsoCalendarSystem.Instance, Chronology.IsoUtc.Calendar);
        }
    }
}
