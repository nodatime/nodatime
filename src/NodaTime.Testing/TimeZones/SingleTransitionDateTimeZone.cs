// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// Time zone with a single transition between two offsets. This provides a simple way to test behaviour across a transition.
    /// </summary>
    public sealed class SingleTransitionDateTimeZone : DateTimeZone
    {
        /// <summary>
        /// Gets the <see cref="ZoneInterval"/> for the period before the transition, starting at the beginning of time.
        /// </summary>
        /// <value>The zone interval for the period before the transition, starting at the beginning of time.</value>
        public ZoneInterval EarlyInterval { get; }

        /// <summary>
        /// Gets the <see cref="ZoneInterval"/> for the period after the transition, ending at the end of time.
        /// </summary>
        /// <value>The zone interval for the period after the transition, ending at the end of time.</value>
        public ZoneInterval LateInterval { get; }

        /// <summary>
        /// Gets the transition instant of the zone.
        /// </summary>
        /// <value>The transition instant of the zone.</value>
        public Instant Transition => EarlyInterval.End;

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
            EarlyInterval = new ZoneInterval(id + "-Early", null, transitionPoint,
                offsetBefore, Offset.Zero);
            LateInterval = new ZoneInterval(id + "-Late", transitionPoint, null,
                offsetAfter, Offset.Zero);
        }

        /// <inheritdoc />
        /// <remarks>
        /// This returns either the zone interval before or after the transition, based on the instant provided.
        /// </remarks>
        public override ZoneInterval GetZoneInterval(Instant instant) => EarlyInterval.Contains(instant) ? EarlyInterval : LateInterval;
    }
}
