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
using System.Globalization;

namespace NodaTime
{
    /// <summary>
    /// Represents an instant on the timeline, measured in ticks from the Unix epoch,
    /// which is typically described as January 1st 1970, midnight, UTC (ISO calendar).
    /// (There are 10,000 ticks in a millisecond.)
    /// </summary>
    /// <remarks>
    /// The default value of this struct is the Unix epoch.
    /// This type is immutable and thread-safe.
    /// </remarks>
    public struct Instant
        : IEquatable<Instant>, IComparable<Instant>
    {
        public static readonly Instant UnixEpoch = new Instant(0);
        public static readonly Instant MinValue = new Instant(Int64.MinValue);
        public static readonly Instant MaxValue = new Instant(Int64.MaxValue);

        private readonly long ticks;

        /// <summary>
        /// Ticks since the Unix epoch.
        /// </summary>
        public long Ticks { get { return ticks; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant"/> struct.
        /// </summary>
        /// <param name="ticks">The ticks from the unix epoch.</param>
        public Instant(long ticks)
        {
            this.ticks = ticks;
        }

        #region Operators

        /// <summary>
        /// Returns an instant after adding the given duration
        /// </summary>
        public static Instant operator +(Instant instant, Duration duration)
        {
            return new Instant(instant.Ticks + duration.Ticks);
        }

        /// <summary>
        /// Implements the operator + (addition) for <see cref="Instant"/> + <see cref="Offset"/>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="LocalInstant"/> representing the sum of the given values.</returns>
        public static LocalInstant operator +(Instant instant, Offset offset)
        {
            return new LocalInstant(instant.Ticks + offset.AsTicks());
        }

        /// <summary>
        /// Adds a duration to an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant"/> representing the sum of the given values.</returns>
        public static Instant Add(Instant left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds an offset to an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="LocalInstant"/> representing the sum of the given values.</returns>
        public static LocalInstant Add(Instant left, Offset right)
        {
            return left + right;
        }

        /// <summary>
        /// Returns the difference between two instants as a duration.
        /// TODO: It *could* return an interval... but I think this is better.
        /// </summary>
        public static Duration operator -(Instant first, Instant second)
        {
            return new Duration(first.Ticks - second.Ticks);
        }

        /// <summary>
        /// Returns an instant after subtracting the given duration
        /// </summary>
        public static Instant operator -(Instant instant, Duration duration)
        {
            return new Instant(instant.Ticks - duration.Ticks);
        }

        /// <summary>
        /// Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the difference of the given values.</returns>
        public static Duration Subtract(Instant left, Instant right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant"/> representing the difference of the given values.</returns>
        public static Instant Subtract(Instant left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Instant left, Instant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Instant left, Instant right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Instant left, Instant right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Instant left, Instant right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Instant left, Instant right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Instant left, Instant right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion // Operators

        #region IEquatable<Instant> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Instant other)
        {
            return this.Ticks == other.Ticks;
        }

        #endregion

        #region IComparable<Instant> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings:
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <description>Meaning</description>
        /// </listheader>
        /// <item>
        /// <term>&lt; 0</term>
        /// <description>This object is less than the <paramref name="other"/> parameter.</description>
        /// </item>
        /// <item>
        /// <term>0</term>
        /// <description>This object is equal to <paramref name="other"/>.</description>
        /// </item>
        /// <item>
        /// <term>&gt; 0</term>
        /// <description>This object is greater than <paramref name="other"/>.</description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(Instant other)
        {
            return Ticks.CompareTo(other.Ticks);
        }

        #endregion

        #region Object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Instant)
            {
                return Equals((Instant)obj);
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
            return Ticks.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Ticks.ToString("N0", CultureInfo.CurrentCulture);
        }

        #endregion  // Object overrides
    }
}
