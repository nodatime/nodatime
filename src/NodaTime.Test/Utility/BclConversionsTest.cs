using System;
using NUnit.Framework;
using NodaTime.Utility;

namespace NodaTime.Test.Utility
{
    [TestFixture]
    public class BclConversionsTest
    {
        // This tests both directions for all valid values.
        // Alternatively, we could have checked names...
        // it doesn't matter much.
        [TestCase(DayOfWeek.Sunday, IsoDayOfWeek.Sunday)]
        [TestCase(DayOfWeek.Monday, IsoDayOfWeek.Monday)]
        [TestCase(DayOfWeek.Tuesday, IsoDayOfWeek.Tuesday)]
        [TestCase(DayOfWeek.Wednesday, IsoDayOfWeek.Wednesday)]
        [TestCase(DayOfWeek.Thursday, IsoDayOfWeek.Thursday)]
        [TestCase(DayOfWeek.Friday, IsoDayOfWeek.Friday)]
        [TestCase(DayOfWeek.Saturday, IsoDayOfWeek.Saturday)]
        public void DayOfWeek_BothWaysValid(DayOfWeek bcl, IsoDayOfWeek noda)
        {
            Assert.AreEqual(bcl, BclConversions.ToDayOfWeek(noda));
            Assert.AreEqual(noda, BclConversions.ToIsoDayOfWeek(bcl));
        }

        [TestCase(0)]
        [TestCase(8)]
        public void ToDayOfWeek_InvalidValues(IsoDayOfWeek noda)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToDayOfWeek(noda));
        }

        [TestCase(-1)]
        [TestCase(7)]
        public void ToIsoDayOfWeek_InvalidValues(DayOfWeek bcl)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => BclConversions.ToIsoDayOfWeek(bcl));
        }
    }
}
