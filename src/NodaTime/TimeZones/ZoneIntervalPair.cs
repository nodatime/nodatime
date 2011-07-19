#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A pair of possibly null ZoneInterval values. This is the result of fetching a time zone
    /// interval by LocalInstant, as the result could be 0, 1 or 2 matching ZoneIntervals.
    /// TODO: Decide if this should be public or not. We may well want a way of getting at the
    /// offset for a particular LocalDateTime, rather than having to construct a ZonedDateTime
    /// and get it that way. On the other hand, this is quite an awkward type... it *feels* like
    /// an implementation detail somehow.
    /// </summary>
    public struct ZoneIntervalPair
    {
        internal static readonly ZoneIntervalPair NoMatch = new ZoneIntervalPair(null, null);
        
        private readonly ZoneInterval earlyInterval;
        private readonly ZoneInterval lateInterval;

        /// <summary>
        /// The earlier of the two zone intervals matching the original local instant, or null
        /// if there were no matches. If there is a single match (the most common case) this
        /// value will be non-null, and LateInterval will be null.
        /// </summary>
        public ZoneInterval EarlyInterval { get { return earlyInterval; } }

        /// <summary>
        /// The later of the two zone intervals matching the original local instant, or null
        /// if there were no matches. If there is a single match (the most common case) this
        /// value will be null, and EarlyInterval will be non-null.
        /// </summary>
        public ZoneInterval LateInterval { get { return lateInterval; } }

        internal ZoneIntervalPair(ZoneInterval early, ZoneInterval late)
        {
            // TODO: Validation, if we want it:
            // - If early is null, late must be null
            // - If both are specified, the end of early must equal the start of late
            this.earlyInterval = early;
            this.lateInterval = late;
        }

        public int MatchingIntervals { get { return earlyInterval == null ? 0 : lateInterval == null ? 1 : 2; } }
    }
}
