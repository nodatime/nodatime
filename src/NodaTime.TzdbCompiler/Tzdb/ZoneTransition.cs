// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Text;
using NodaTime.Utility;

namespace NodaTime.TzdbCompiler.Tzdb
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
    internal sealed class ZoneTransition
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

        /// <summary>
        /// Determines whether is a transition from the given transition.
        /// </summary>
        /// <remarks>
        /// To be a transition from another the instant at which the transition occurs must be
        /// greater than the given transition's and at least one aspect out of (name, standard
        /// offset, wall offset) must differ. If this is not true then this transition is considered
        /// to be redundant and should not be used. Note that there are a few transitions which
        /// keep the same wall offset and name, but differ in how that wall offset is divided into
        /// daylight saving and standard components. One notable example of this is October 27th 1968, when
        /// the UK went from "British Summer Time" (BST, standard=0, daylight=1) to "British Standard Time"
        /// (BST, standard=1, daylight=0).
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
            return Instant > other.Instant &&
                WallOffset != other.WallOffset || Name != other.Name || StandardOffset != other.StandardOffset;
        }

        #region Object overrides
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
