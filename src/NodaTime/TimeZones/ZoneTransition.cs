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
using NodaTime.Utility;

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
        : IEquatable<ZoneTransition>, IComparable<ZoneTransition>
    {
        internal Instant Instant { get { return this.instant; } }
        internal string Name { get { return this.name; } }
        internal Offset WallOffset { get { return this.wallOffset; } }
        internal Offset StandardOffset { get { return this.standardOffset; } }
        internal Offset Savings { get { return WallOffset - StandardOffset; } }

        private readonly Instant instant;
        private readonly string name;
        private readonly Offset wallOffset;
        private readonly Offset standardOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="tr">The tr.</param>
        internal ZoneTransition(Instant instant, ZoneTransition tr)
            : this(instant, tr.Name, tr.WallOffset, tr.StandardOffset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <param name="instant">The instant.</param>
        /// <param name="name">The name.</param>
        /// <param name="wallOffset">The wall offset.</param>
        /// <param name="standardOffset">The standard offset.</param>
        internal ZoneTransition(Instant instant, String name, Offset wallOffset, Offset standardOffset)
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
        /// To be a transition from another the instant at which the transition occurs must be
        /// greater than the given transition's and either the time offset or the name must be
        /// different.
        /// </remarks>
        /// <param name="other">The <see cref="ZoneTransition"/> to compare to.</param>
        /// <returns>
        /// <c>true</c> if this is a transition from the given transition; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsTransitionFrom(ZoneTransition other)
        {
            if (other == null)
            {
                return true;
            }
            return Instant > other.Instant && (WallOffset != other.WallOffset || Name != other.Name);
        }

        #region Object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is ZoneTransition)
            {
                return Equals((ZoneTransition)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Instant.GetHashCode();
        }

        #endregion // Object overrides

        #region IEquatable<ZoneTransition> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ZoneTransition other)
        {
            if (other == null)
            {
                return false;
            }
            return Instant == other.Instant;
        }

        #endregion

        #region IComparable<ZoneTransition> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. 
        /// The return value has the following meanings:
        /// Value
        /// Meaning
        /// Less than zero
        /// This object is less than the <paramref name="other"/> parameter.
        /// Zero
        /// This object is equal to <paramref name="other"/>.
        /// Greater than zero
        /// This object is greater than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(ZoneTransition other)
        {
            if (other == null)
            {
                return 1;
            }
            return Instant.CompareTo(other.Instant);
        }

        #endregion
    }

}
