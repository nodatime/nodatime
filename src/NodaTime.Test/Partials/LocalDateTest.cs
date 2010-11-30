using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.Partials;
using NUnit.Framework;

namespace NodaTime.Test.Partials
{
    [TestFixture]
    public class LocalDateTest
    {
        [Test]
        public void Addition_WithPeriod()
        {
            LocalDate start = new LocalDate(2010, 6, 19);
            Period2 period = Period2.FromMonths(3) + Period2.FromDays(10);
            LocalDate expected = new LocalDate(2010, 9, 29);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_TruncatesOnShortMonth()
        {
            LocalDate start = new LocalDate(2010, 1, 30);
            Period2 period = Period2.FromMonths(1);
            LocalDate expected = new LocalDate(2010, 2, 28);
            Assert.AreEqual(expected, start + period);
        }

        [Test]
        public void Addition_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date + (Period2)null).ToString());
        }

        [Test]
        public void Subtraction_WithPeriod()
        {
            LocalDate start = new LocalDate(2010, 9, 29);
            Period2 period = Period2.FromMonths(3) + Period2.FromDays(10);
            LocalDate expected = new LocalDate(2010, 6, 19);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_TruncatesOnShortMonth()
        {
            LocalDate start = new LocalDate(2010, 3, 30);
            Period2 period = Period2.FromMonths(1);
            LocalDate expected = new LocalDate(2010, 2, 28);
            Assert.AreEqual(expected, start - period);
        }

        [Test]
        public void Subtraction_WithNullPeriod_ThrowsArgumentNullException()
        {
            LocalDate date = new LocalDate(2010, 1, 1);
            // Call to ToString just to make it a valid statement
            Assert.Throws<ArgumentNullException>(() => (date - (Period2)null).ToString());
        }
    }
}
