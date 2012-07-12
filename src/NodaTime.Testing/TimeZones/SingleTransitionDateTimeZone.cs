#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// Time zone with a single transition between two offsets. This is
    /// similar to a precalculated zone with only two intervals and no
    /// tail zone, but it's simpler to construct and can be used to test
    /// PrecalculatedDateTimeZone.
    /// </summary>
    public class SingleTransitionDateTimeZone : DateTimeZone
    {
        private readonly ZoneInterval earlyInterval;
        /// <summary>
        /// The ZoneInterval for the period before the transition.
        /// </summary>
        public ZoneInterval EarlyInterval { get { return earlyInterval; } }

        private readonly ZoneInterval lateInterval;
        /// <summary>
        /// The ZoneInterval for the period after the transition.
        /// </summary>
        public ZoneInterval LateInterval { get { return lateInterval; } }

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
            : base("Single", false, Offset.Min(offsetBefore, offsetAfter), Offset.Max(offsetBefore, offsetAfter))
        {
            earlyInterval = new ZoneInterval("Single-Early", Instant.MinValue, transitionPoint,
                offsetBefore, Offset.Zero);
            lateInterval = new ZoneInterval("Single-Late", transitionPoint, Instant.MaxValue,
                offsetAfter, Offset.Zero);
        }

        /// <summary>
        /// Returns the zone interval before or after the transition, based on the instant provided.
        /// </summary>
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
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Id);
            hash = HashCodeHelper.Hash(hash, earlyInterval);
            hash = HashCodeHelper.Hash(hash, lateInterval);
            return hash;
        }
    }
}
