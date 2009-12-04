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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a base class for <see cref="IDateTimeZone"/> implementations.
    /// </summary>
    /// <remarks>
    /// This base is immutable and thread safe. All sub-classes should be as well.
    /// </remarks>
    public abstract class DateTimeZoneBase
        : IDateTimeZone
    {
        private readonly string id;
        private readonly bool isFixed;
        protected readonly Offset standardOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneBase"/> class.
        /// </summary>
        /// <param name="id">The unique id of this time zone.</param>
        /// <param name="standardOffset">The standard offset from UTC.</param>
        /// <param name="isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        public DateTimeZoneBase(string id, Offset standardOffset, bool isFixed)
        {
            this.id = id;
            this.standardOffset = standardOffset;
            this.isFixed = isFixed;
        }

        #region IDateTimeZone Members

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant,
        /// or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <returns>
        /// The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        public abstract Instant? NextTransition(Instant instant);

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant,
        /// or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        public abstract Instant? PreviousTransition(Instant instant);

        public abstract void Write(DateTimeZoneWriter writer);

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public virtual Offset GetOffsetFromUtc(Instant instant)
        {
            return this.standardOffset;
        }

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC is earlier
        /// than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="localTime">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public abstract Offset GetOffsetFromLocal(LocalInstant instant);

        /// <summary>
        /// Returns the name associated with the given instant.
        /// </summary>
        /// <param name="instant">The instant to get the name for.</param>
        /// <returns>
        /// The name of this time. Never returns null.
        /// </returns>
        /// <remarks>
        /// For a fixed time zone this will always return the same value but for a time zone that
        /// honors daylight savings this will return a different name depending on the time of year
        /// it represents. For example in the Pacific Standard Time (UTC-8) it will return either
        /// PST or PDT depending on the time of year.
        /// </remarks>
        public virtual string Name(Instant instant)
        {
            return Id;
        }

        /// <summary>
        /// The database ID for the time zone.
        /// </summary>
        public string Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        public bool IsFixed
        {
            get { return this.isFixed; }
        }

        #endregion
    }
}
