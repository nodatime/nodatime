// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NodaTime.Utility;
using System;

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
            Preconditions.CheckNotNull(name, nameof(name));
            this.Instant = instant;
            this.Name = name;
            this.StandardOffset = standardOffset;
            this.Savings = savings;
        }

        /// <summary>
        /// The instant at which the transition occurs.
        /// </summary>
        internal Instant Instant { get; }

        /// <summary>
        /// The name of the zone interval after this transition.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// The standard offset after this transition.
        /// </summary>
        internal Offset StandardOffset { get; }

        /// <summary>
        /// The daylight savings after this transition.
        /// </summary>
        internal Offset Savings { get; }

        /// <summary>
        /// The wall offset (savings + standard) after this transition.
        /// </summary>
        internal Offset WallOffset => StandardOffset + Savings;

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
            bool later = Instant > other.Instant;
            bool different = Name != other.Name || StandardOffset != other.StandardOffset || Savings != other.Savings;
            return later && different;
        }

        /// <summary>
        /// Creates a new zone interval from this transition to the given end point.
        /// </summary>
        /// <param name="end">The end of the interval.</param>
        internal ZoneInterval ToZoneInterval(Instant end) => new ZoneInterval(Name, Instant, end, StandardOffset + Savings, Savings);

        #region Object overrides

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() => $"{Name} at {Instant} {StandardOffset} [{Savings}]";
        #endregion // Object overrides
    }
}
