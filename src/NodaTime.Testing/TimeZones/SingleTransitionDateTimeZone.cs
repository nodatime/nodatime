using System;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// Time zone with a single transition between two offsets. This provides a simple way to test behaviour across a transition.
    /// </summary>
    public sealed class SingleTransitionDateTimeZone : DateTimeZone
    {
        private readonly ZoneInterval earlyInterval;
        /// <summary>
        /// The <see cref="ZoneInterval"/> for the period before the transition, starting at the beginning of time.
        /// </summary>
        public ZoneInterval EarlyInterval { get { return earlyInterval; } }

        private readonly ZoneInterval lateInterval;
        /// <summary>
        /// The <see cref="ZoneInterval"/> for the period after the transition, ending at the end of time.
        /// </summary>
        public ZoneInterval LateInterval { get { return lateInterval; } }

        /// <summary>
        /// The transition instant of the zone.
        /// </summary>
        public Instant Transition { get { return earlyInterval.End; } }

        /// <summary>
        /// Creates a zone with a single transition between two offsets.
        /// </summary>
        /// <param name="transitionPoint">The transition point as an <see cref="Instant"/>.</param>
        /// <param name="offsetBeforeHours">The offset of local time from UTC, in hours, before the transition.</param>
        /// <param name="offsetAfterHours">The offset of local time from UTC, in hours, before the transition.</param>
        public SingleTransitionDateTimeZone(Instant transitionPoint, int offsetBeforeHours, int offsetAfterHours)
            : this(transitionPoint, Offset.FromHours(offsetBeforeHours), Offset.FromHours(offsetAfterHours))
        {
        }

        /// <summary>
        /// Creates a zone with a single transition between two offsets.
        /// </summary>
        /// <param name="transitionPoint">The transition point as an <see cref="Instant"/>.</param>
        /// <param name="offsetBefore">The offset of local time from UTC before the transition.</param>
        /// <param name="offsetAfter">The offset of local time from UTC before the transition.</param>
        public SingleTransitionDateTimeZone(Instant transitionPoint, Offset offsetBefore, Offset offsetAfter)
            : this(transitionPoint, offsetBefore, offsetAfter, "Single")
        {
        }

        /// <summary>
        /// Creates a zone with a single transition between two offsets.
        /// </summary>
        /// <param name="transitionPoint">The transition point as an <see cref="Instant"/>.</param>
        /// <param name="offsetBefore">The offset of local time from UTC before the transition.</param>
        /// <param name="offsetAfter">The offset of local time from UTC before the transition.</param>
        /// <param name="id">ID for the newly created time zone.</param>
        public SingleTransitionDateTimeZone(Instant transitionPoint, Offset offsetBefore, Offset offsetAfter, string id)
            : base(id, false, Offset.Min(offsetBefore, offsetAfter), Offset.Max(offsetBefore, offsetAfter))
        {
            earlyInterval = new ZoneInterval(id + "-Early", Instant.MinValue, transitionPoint,
                offsetBefore, Offset.Zero);
            lateInterval = new ZoneInterval(id + "-Late", transitionPoint, Instant.MaxValue,
                offsetAfter, Offset.Zero);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This returns either the zone interval before or after the transition, based on the instant provided.
        /// </remarks>
        public override ZoneInterval GetZoneInterval(Instant instant)
        {
            return earlyInterval.Contains(instant) ? earlyInterval : lateInterval;
        }

        /// <inheritdoc />
        protected override bool EqualsImpl(DateTimeZone zone)
        {
            SingleTransitionDateTimeZone otherZone = (SingleTransitionDateTimeZone)zone;
            return Id == otherZone.Id && earlyInterval.Equals(otherZone.earlyInterval) && lateInterval.Equals(otherZone.lateInterval);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Id.GetHashCode();
            hash = hash * 31 + earlyInterval.GetHashCode();
            hash = hash * 31 + lateInterval.GetHashCode();
            return hash;
        }
    }
}
