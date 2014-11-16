// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A transition between two offsets, usually for daylight saving reasons. This type only knows about
    /// the new offset, and the transition point.
    /// </summary>
    /// 
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
    internal struct Transition : IEquatable<Transition>
    {
        internal Instant Instant { get; }

        /// <summary>
        /// The offset from the time when this transition occurs until the next transition.
        /// </summary>
        internal Offset NewOffset { get; }

        internal Transition(Instant instant, Offset newOffset) : this()
        {
            this.Instant = instant;
            this.NewOffset = newOffset;
        }

        public bool Equals(Transition other) => Instant == other.Instant && NewOffset == other.NewOffset;

        #region Operators
        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Transition left, Transition right) => left.Equals(right);

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Transition left, Transition right) => !(left == right);
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
        public override bool Equals(object obj) => obj is Transition && Equals((Transition)obj);

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
            hash = hash * 31 + Instant.GetHashCode();
            hash = hash * 31 + NewOffset.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => "Transition to " + NewOffset + " at " + Instant;
        #endregion  // Object overrides
    }
}