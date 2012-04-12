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
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Represents a transition two different time references.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Normally this is between standard time and daylight savings time.
    /// </para>
    /// <para>
    /// Immutable, thread safe.
    /// </para>
    /// </remarks>
    internal class ZoneTransition : IEquatable<ZoneTransition>, IComparable<ZoneTransition>
    {
        private readonly Instant instant;
        private readonly string name;
        private readonly Offset savings;
        private readonly Offset standardOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="instant">The instant that this transistion occurs at.</param>
        /// <param name="name">The name for the time at this transition e.g. PDT or PST.</param>
        /// <param name="standardOffset">The standard offset at this transition.</param>
        /// <param name="savings">The actual offset at this transition.</param>
        internal ZoneTransition(Instant instant, String name, Offset standardOffset, Offset savings)
        {
            Preconditions.CheckNotNull(name, "name");
            this.instant = instant;
            this.name = name;
            this.standardOffset = standardOffset;
            this.savings = savings;
        }

        internal Instant Instant { get { return instant; } }

        internal string Name { get { return name; } }

        internal Offset StandardOffset { get { return standardOffset; } }

        internal Offset Savings { get { return savings; } }

        internal Offset WallOffset { get { return StandardOffset + Savings; } }

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
            return other == null ? 1 : Instant.CompareTo(other.Instant);
        }
        #endregion

        #region Operator overloads
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneTransition left, ZoneTransition right)
        {
            return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ZoneTransition left, ZoneTransition right)
        {
            return !(left == right);
        }
        #endregion

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
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Instant == other.Instant;
        }
        #endregion

        /// <summary>
        /// Determines whether is a transition from the given transition.
        /// </summary>
        /// <remarks>
        /// To be a transition from another the instant at which the transition occurs must be
        /// greater than the given transition's and either the wall offset or the name must be
        /// different. If this is not true then this transition is considered to be redundant
        /// and should not be used.
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
            return Equals(obj as ZoneTransition);
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(name);
            builder.Append(" at ").Append(Instant);
            builder.Append(" ").Append(StandardOffset);
            builder.Append(" [").Append(Savings).Append("]");
            return builder.ToString();
        }
        #endregion // Object overrides
    }
}