// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A transition between two offsets, usually for daylight saving reasons. This type only knows about
    /// the old offset, the new offset, and the transition point.
    /// </summary>
    /// 
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    internal struct Transition : IEquatable<Transition>
    {
        private readonly Instant instant;
        public Instant Instant { get { return instant; } }

        private readonly Offset oldOffset;

        private readonly Offset newOffset;

        /// <summary>
        /// The offset from the time when this transition occurs until the next transition.
        /// </summary>
        public Offset NewOffset { get { return newOffset; } }

        public Transition(Instant instant, Offset oldOffset, Offset newOffset)
        {
            this.instant = instant;
            this.oldOffset = oldOffset;
            this.newOffset = newOffset;
        }

        public bool Equals(Transition other)
        {
            return instant == other.Instant && oldOffset == other.oldOffset && newOffset == other.NewOffset;
        }

        /// <summary>
        /// Returns the transition which occurs later of the two provided. (If they occur at the same instant, the second argument is returned.)
        /// </summary>
        internal static Transition Later(Transition left, Transition right)
        {
            return left.Instant > right.Instant ? left : right;
        }

        /// <summary>
        /// Returns the transition which occurs earlier of the two provided. (If they occur at the same instant, the first argument is returned.)
        /// </summary>
        internal static Transition Earlier(Transition left, Transition right)
        {
            return left.Instant > right.Instant ? right : left;
        }

        #region Operators
        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Transition left, Transition right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Transition left, Transition right)
        {
            return !(left == right);
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
            if (obj is Transition)
            {
                return Equals((Transition)obj);
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
            int hash = 23;
            hash = hash * 31 + instant.GetHashCode();
            hash = hash * 31 + oldOffset.GetHashCode();
            hash = hash * 31 + newOffset.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Transition from " + oldOffset + " to " + newOffset + " at " + instant;
        }
        #endregion  // Object overrides
    }
}