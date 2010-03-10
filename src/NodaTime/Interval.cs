#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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
namespace NodaTime
{
    /// <summary>
    /// An interval between two instants in time.
    /// </summary>
    /// <remarks>
    /// This type is immutable and thread-safe.
    /// </remarks>
    public struct Interval
        : IEquatable<Interval>
    {
        private readonly Instant end;
        private readonly Instant start;

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval"/> struct. The <see
        /// cref="Interval"/> includes the <paramref name="start"/> instant and excludes the
        /// <paramref name="end"/> instant.
        /// </summary>
        /// <param name="start">The start <see cref="Instant"/>.</param>
        /// <param name="end">The end <see cref="Instant"/>.</param>
        public Interval(Instant start, Instant end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the start instant.
        /// </summary>
        /// <value>The start <see cref="Instant"/>.</value>
        public Instant Start
        {
            get { return this.start; }
        }

        /// <summary>
        /// Gets the end instant.
        /// </summary>
        /// <value>The end <see cref="Instant"/>.</value>
        public Instant End
        {
            get { return this.end; }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public Duration Duration
        {
            get { return end - start; }
        }

        #region Implementation of IEquatable<Interval>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Interval other)
        {
            return Start == other.Start && End == other.End;
        }

        #endregion

        #region object overrides

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. 
        ///                 </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (obj is Interval)
            {
                return Equals((Interval)obj);
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Start);
            hash = HashCodeHelper.Hash(hash, End);
            return hash;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Interval left, Interval right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Interval left, Interval right)
        {
            return !(left == right);
        }

        #endregion
    }
}