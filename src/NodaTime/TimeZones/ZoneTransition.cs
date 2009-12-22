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
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Represents a transition two different time references.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Normally this is between standard timne and daylight savings time but it might be for other
    /// purposes like the discontinuity in the Gregorian calendar to account for leap time.
    /// </para>
    /// <para>
    /// Immutable, thread safe.
    /// </para>
    /// </remarks>
    internal class ZoneTransition
        : IEquatable<ZoneTransition>, IComparable<ZoneTransition>
    {
        internal Instant Instant { get { return this.instant; } }
        internal string Name { get { return this.name; } }
        internal Offset StandardOffset { get { return this.standardOffset; } }
        internal Offset Savings { get { return this.savings; } }
        internal Offset WallOffset { get { return StandardOffset + Savings; } }

        private readonly Instant instant;
        private readonly string name;
        private readonly Offset standardOffset;
        private readonly Offset savings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneTransition"/> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Assumption 1: Offset.MaxValue &lt;&lt; Instant.MaxValue
        /// </para>
        /// <para>
        /// Assumption 2: Offset.MinValue &gt;&gt; Instant.MinValue
        /// </para>
        /// <para>
        /// Therefore the sum of an Instant with an Offset of the opposite sign cannot overflow or
        /// underflow. We only have to worry about summing an Instant with an Offset of the same
        /// sign over/underflowing.
        /// </para>
        /// </remarks>
        /// <param name="instant">The instant that this transistion occurs at.</param>
        /// <param name="name">The name for the time at this transition e.g. PDT or PST.</param>
        /// <param name="standardOffset">The standard offset at this transition.</param>
        /// <param name="savings">The actual offset at this transition.</param>
        internal ZoneTransition(Instant instant, String name, Offset standardOffset, Offset savings)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name", "name cannot be null");
            }
            this.instant = instant;
            this.name = name;
            this.standardOffset = standardOffset;
            this.savings = savings;
            //
            // Make sure that the math will not overflow later.
            //
            if (instant.Ticks < 0 && WallOffset.Milliseconds < 0)
            {
                long distanceFromEndOfTime = instant.Ticks - Instant.MinValue.Ticks;
                if (distanceFromEndOfTime < Math.Abs(WallOffset.AsTicks()))
                {
                    this.standardOffset = Offset.FromTicks(-distanceFromEndOfTime);
                    this.savings = Offset.Zero;
                }
            }
            else if (instant.Ticks > 0 && savings.Milliseconds > 0)
            {
                long distanceFromEndOfTime = Instant.MaxValue.Ticks - instant.Ticks;
                if (distanceFromEndOfTime < WallOffset.AsTicks())
                {
                    this.standardOffset = Offset.FromTicks(distanceFromEndOfTime);
                    this.savings = Offset.Zero;
                }
            }
        }

        /// <summary>
        /// Determines whether is a transition from the given transition.
        /// </summary>
        /// <remarks>
        /// To be a transition from another the instant at which the transition occurs must be
        /// greater than the given transition's and either the savings offset or the name must be
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
            return Instant > other.Instant && (Savings != other.Savings|| Name != other.Name);
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
            ZoneTransition transition = obj as ZoneTransition;
            if (transition != null)
            {
                return Equals(transition);
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

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(name);
            builder.Append(" at ").Append(Instant);
            builder.Append(" ").Append(StandardOffset);
            builder.Append(" [").Append(Savings).Append("]");
            return builder.ToString();
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

        #region Operator overloads

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ZoneTransition left, ZoneTransition right)
        {
            if ((object)left == null || (object)right == null)
            {
                return (object)left == (object)right;
            }
            return left.Equals(right);
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
    }
}
