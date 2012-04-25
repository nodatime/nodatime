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
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// An interval between two instants in time.
    /// </summary>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    public struct Interval : IEquatable<Interval>
    {
        private static readonly int TypeInitializationChecking = NodaTime.Utility.TypeInitializationChecker.RecordInitializationStart();

        /// <summary>The start of the interval.</summary>
        private readonly Instant start;

        /// <summary>The end of the interval. This will never be earlier than the start.</summary>
        private readonly Instant end;

        /// <summary>
        /// Initializes a new <see cref="Interval"/> The interval includes the <paramref name="start"/> instant and excludes the
        /// <paramref name="end"/> instant. The end may equal the start (resulting in an empty interval), but must not be before the start.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/> is earlier than <paramref name="start"/>.</exception>
        /// <param name="start">The start <see cref="Instant"/>.</param>
        /// <param name="end">The end <see cref="Instant"/>.</param>
        public Interval(Instant start, Instant end)
        {
            if (end < start)
            {
                throw new ArgumentOutOfRangeException("end", "The end parameter must be equal to or later than the start parameter");
            }
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets the start instant.
        /// </summary>
        /// <value>The start <see cref="Instant"/>.</value>
        public Instant Start { get { return start; } }

        /// <summary>
        /// Gets the end instant.
        /// </summary>
        /// <value>The end <see cref="Instant"/>.</value>
        public Instant End { get { return end; } }

        /// <summary>
        /// Returns the duration of the interval, which will always be non-negative.
        /// </summary>
        /// <value>The duration of the interval.</value>
        public Duration Duration { get { return end - start; } }

        #region Implementation of IEquatable<Interval>
        /// <summary>
        /// Indicates whether the value of this interval is equal to the value of the specified interval.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Interval other)
        {
            return Start == other.Start && End == other.End;
        }
        #endregion

        #region object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
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
