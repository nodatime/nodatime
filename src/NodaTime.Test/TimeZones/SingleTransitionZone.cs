using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Time zone with a single transition between two offsets. This is
    /// similar to a precalculated zone with only two intervals and no
    /// tail zone, but it's simpler to construct and can be used to test
    /// PrecalculatedDateTimeZone.
    /// </summary>
    internal class SingleTransitionZone : DateTimeZone
    {
        private readonly ZoneInterval earlyInterval;
        internal ZoneInterval EarlyInterval { get { return earlyInterval; } }

        private readonly ZoneInterval lateInterval;
        internal ZoneInterval LateInterval { get { return lateInterval; } }

        internal SingleTransitionZone(Instant transitionPoint, int offsetBeforeHours, int offsetAfterHours)
            : this(transitionPoint, Offset.ForHours(offsetBeforeHours), Offset.ForHours(offsetAfterHours))
        {
        }

        internal SingleTransitionZone(Instant transitionPoint, Offset offsetBefore, Offset offsetAfter)
            : base("Single", false, Offset.Min(offsetBefore, offsetAfter), Offset.Max(offsetBefore, offsetAfter))
        {
            earlyInterval = new ZoneInterval("Single-Early", Instant.MinValue, transitionPoint,
                offsetBefore, Offset.Zero);
            lateInterval = new ZoneInterval("Single-Late", transitionPoint, Instant.MaxValue,
                offsetAfter, Offset.Zero);
        }

        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            return earlyInterval.Contains(instant) ? earlyInterval : lateInterval;
        }

        internal override ZoneInterval GetZoneInterval(LocalInstant localInstant)
        {
            // This method will be removed anyway, so let's not implemented it
            throw new NotImplementedException();
        }

        internal override ZoneIntervalPair GetZoneIntervals(LocalInstant localInstant)
        {
            if (earlyInterval.Contains(localInstant))
            {
                return new ZoneIntervalPair(earlyInterval, lateInterval.Contains(localInstant) ? lateInterval : null);
            }
            return new ZoneIntervalPair(lateInterval.Contains(localInstant) ? lateInterval : null, null);
        }

        internal override void Write(DateTimeZoneWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
