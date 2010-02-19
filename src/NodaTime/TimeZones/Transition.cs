using System;
using System.Collections.Generic;
using System.Text;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// A transition between two offsets, usually for daylight saving reasons.
    /// TODO(jon): Potentially rename this to ZoneTransition after doing something
    /// else with the current ZoneTransition class :)
    /// </summary>
    public struct Transition : IEquatable<Transition>
    {
        private readonly Instant instant;
        public Instant Instant { get { return instant; } }

        private readonly Offset oldOffset;
        /// <summary>
        /// The offset which applied from the previous transition until this
        /// one.
        /// </summary>
        public Offset OldOffset { get { return oldOffset; } }

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
            return instant == other.Instant &&
                oldOffset == other.OldOffset &&
                newOffset == other.NewOffset;
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
