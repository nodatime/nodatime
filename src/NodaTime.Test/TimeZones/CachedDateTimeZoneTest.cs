using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class CachedDateTimeZoneTest
    {
        // PreviousTransition is tricky, as the Info for a period may be wrong for the first
        // tick (or for all other ones, if you're not careful)
        [Test]
        public void PreviousTransition_SucceedsOnTransitionPoint()
        {
            var cached = DateTimeZones.ForId("Europe/Paris");
            var uncached = cached.Uncached();
            Instant summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
            Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
            Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick),
                cached.PreviousTransition(nextTransitionTick));
        }

        [Test]
        public void PreviousTransition_SucceedsOffTransitionPoint()
        {
            // This fails with naive caching
            var cached = DateTimeZones.ForId("Europe/Paris");
            var uncached = cached.Uncached();
            Instant summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
            Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
            Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick + Duration.One),
                cached.PreviousTransition(nextTransitionTick + Duration.One));
        }
    }
}
