using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NodaTime.Demo
{
    [TestFixture]
    public class LocalInstantDemo
    {
        [Test]
        public void Construction()
        {
            // What does this actually mean?
            // 1000000 ticks after January 1970, midnight wherever it occurred for you
            // Not an instant in time.
            // Not specific to a calendar.
            // Not specific to a time zone.
            // Confusing - but fortunately internal, for the most part.
            // Better descriptions welcome.
            LocalInstant x = LocalInstant.FromTicks(1000000);
        }

        [Test]
        public void OffsetArithmetic()
        {
            // If local time is two hours ahead of UTC...
            Offset offset = Offset.ForHours(2);
            Instant instant = new Instant();
            LocalInstant local = instant + offset;
            Instant backAgain = local - offset;
            Assert.AreEqual(instant, backAgain);

            // Can't get it wrong way round:
            // LocalInstant invalid = instant - offset;
        }
    }
}
