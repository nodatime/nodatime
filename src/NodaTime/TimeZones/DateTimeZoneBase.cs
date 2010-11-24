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
    /// Provides a base class for <see cref="IDateTimeZone"/> implementations.
    /// </summary>
    /// <remarks>
    /// This base is immutable and thread safe. All sub-classes should be as well.
    /// </remarks>
    public abstract class DateTimeZoneBase : IDateTimeZone
    {
        private readonly string id;
        private readonly bool isFixed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneBase"/> class.
        /// </summary>
        /// <param name="id">The unique id of this time zone.</param>
        /// <param name="isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        protected DateTimeZoneBase(string id, bool isFixed)
        {
            this.id = id;
            this.isFixed = isFixed;
        }

        #region IDateTimeZone Members
        /// <summary>
        /// Gets the zone offset period for the given instant. Null is returned if no period is
        /// defined by the time zone for the given instant.
        /// </summary>
        /// <param name="instant">The Instant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public abstract ZoneInterval GetZoneInterval(Instant instant);

        /// <summary>
        /// Gets the zone offset period for the given local instant. Null is returned if no period
        /// is defined by the time zone for the given local instant.
        /// </summary>
        /// <param name="localInstant">The LocalInstant to test.</param>
        /// <returns>The defined ZoneOffsetPeriod or <c>null</c>.</returns>
        public abstract ZoneInterval GetZoneInterval(LocalInstant localInstant);

        public abstract void Write(IDateTimeZoneWriter writer);

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is
        /// later than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public virtual Offset GetOffsetFromUtc(Instant instant)
        {
            var period = GetZoneInterval(instant);
            return period.Offset;
        }

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC
        /// is earlier than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="localInstant">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public virtual Offset GetOffsetFromLocal(LocalInstant localInstant)
        {
            var period = GetZoneInterval(localInstant);
            return period.Offset;
        }

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
        public virtual string GetName(Instant instant)
        {
            var period = GetZoneInterval(instant);
            return period.Name;
        }

        /// <summary>
        /// The database ID for the time zone.
        /// </summary>
        public string Id { get { return id; } }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        public bool IsFixed { get { return isFixed; } }
        #endregion

        #region Object overrides
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }
        #endregion
    }
}