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

namespace NodaTime.Testing.TimeZones
{
    /// <summary>
    /// Time zone with a single transition between two offsets. This is
    /// similar to a precalculated zone with only two intervals and no
    /// tail zone, but it's simpler to construct and can be used to test
    /// PrecalculatedDateTimeZone.
    /// </summary>
    public class SingleTransitionZone : DateTimeZone
    {
        private readonly ZoneInterval earlyInterval;
        public ZoneInterval EarlyInterval { get { return earlyInterval; } }

        private readonly ZoneInterval lateInterval;
        public ZoneInterval LateInterval { get { return lateInterval; } }

        public SingleTransitionZone(Instant transitionPoint, int offsetBeforeHours, int offsetAfterHours)
            : this(transitionPoint, Offset.ForHours(offsetBeforeHours), Offset.ForHours(offsetAfterHours))
        {
        }

        public SingleTransitionZone(Instant transitionPoint, Offset offsetBefore, Offset offsetAfter)
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

        internal override void Write(DateTimeZoneWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
