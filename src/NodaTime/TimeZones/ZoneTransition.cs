#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Represents a transition two different time references. Normally this is between standard
    /// timne and daylight savings time but it might be for other purposes like the discontinuity in
    /// the Gregorian calendar to account for leap time. 
    /// </summary>
    /// <remarks>
    /// Immutable, thread safe.
    /// </remarks>
    internal class ZoneTransition
    {
        internal LocalInstant Instant { get { return this.instant; } }
        internal string Name { get { return this.name; } }
        internal Duration WallOffset { get { return this.wallOffset; } }
        internal Duration StandardOffset { get { return this.standardOffset; } }
        internal Duration Savings { get { return WallOffset - StandardOffset; } }

        private readonly LocalInstant instant;
        private readonly string name;
        private readonly Duration wallOffset;
        private readonly Duration standardOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="tr">The tr.</param>
        internal ZoneTransition(LocalInstant instant, ZoneTransition tr)
            : this(instant, tr.Name, tr.WallOffset, tr.StandardOffset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="standardOffset">The standard offset.</param>
        internal ZoneTransition(LocalInstant instant, ZoneRule rule, Duration standardOffset)
            : this(instant, rule.Name, standardOffset + rule.Savings, standardOffset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="name">The name.</param>
        /// <param name="wallOffset">The wall offset.</param>
        /// <param name="standardOffset">The standard offset.</param>
        internal ZoneTransition(LocalInstant instant, String name, Duration wallOffset, Duration standardOffset)
        {
            this.instant = instant;
            this.name = name;
            this.wallOffset = wallOffset;
            this.standardOffset = standardOffset;
        }

        /// <summary>
        /// Determines whether is a transition from the given transition.
        /// </summary>
        /// <remarks>
        /// To be a transition at least one of the basic values must be different.
        /// </remarks>
        /// <param name="other">The other.</param>
        /// <returns>
        /// <c>true</c> if this is a transition from the given transition; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsTransitionFrom(ZoneTransition other)
        {
            if (other == null) {
                return true;
            }
            return Instant > other.Instant &&
                (WallOffset != other.WallOffset ||
                 Name != other.Name);
        }
    }

}
