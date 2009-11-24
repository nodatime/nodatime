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
    /// Extends <see cref="ZoneYearOffset"/> with a name and savings.
    /// </summary>
    /// <remarks>
    /// Immutable, thread safe.
    /// </remarks>
    internal class ZoneRecurrence
        : IEquatable<ZoneRecurrence>
    {
        internal string Name { get { return this.name; } }
        internal Duration Savings { get { return this.savings; } }

        private readonly ZoneYearOffset yearOffset;
        private readonly string name;
        private readonly Duration savings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneRecurrence"/> class.
        /// </summary>
        /// <param name="yearOffset">The year offset.</param>
        /// <param name="name">The name.</param>
        /// <param name="savings">The savings.</param>
        internal ZoneRecurrence(ZoneYearOffset yearOffset, String name, Duration savings)
        {
            this.yearOffset = yearOffset;
            this.name = name;
            this.savings = savings;
        }

        /// <summary>
        /// Returns the given instant adjusted one year forward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal LocalInstant Next(LocalInstant instant, Duration standardOffset, Duration savings)
        {
            return this.yearOffset.Next(instant, standardOffset, savings);
        }

        /// <summary>
        /// Returns the given instant adjusted one year backward taking into account leap years and other
        /// adjustments like day of week.
        /// </summary>
        /// <param name="instant">The instant to adjust.</param>
        /// <param name="standardOffset">The standard offset.</param>
        /// <param name="savings">The daylight savings adjustment.</param>
        /// <returns>The adjusted <see cref="LocalInstant"/>.</returns>
        internal LocalInstant Previous(LocalInstant instant, Duration standardOffset, Duration savings)
        {
            return this.yearOffset.Previous(instant, standardOffset, savings);
        }

        /// <summary>
        /// Returns a new <see cref="Recurrence"/> with the same settings and given suffix appended
        /// to the original name. Used to created "-Summer" versions of conflicting recurrences.
        /// </summary>
        /// <param name="appendNameKey">The append name key.</param>
        /// <returns></returns>
        internal ZoneRecurrence RenameAppend(String suffix)
        {
            return new ZoneRecurrence(this.yearOffset, Name + suffix, Savings);
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
            if (obj is ZoneRecurrence) {
                return Equals((ZoneRecurrence)obj);
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
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, this.savings);
            hash = HashCodeHelper.Hash(hash, this.name);
            hash = HashCodeHelper.Hash(hash, this.yearOffset);
            return hash;
        }

        #endregion // Object overrides

        #region IEquatable<ZoneRecurrence> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ZoneRecurrence other)
        {
            if (other == null) {
                return false;
            }
            return
                this.savings == other.savings &&
                this.name == other.name &&
                this.yearOffset == other.yearOffset;
        }

        #endregion
    }
}
