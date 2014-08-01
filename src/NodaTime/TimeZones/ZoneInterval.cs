// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Diagnostics;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Represents a range of time for which a particular Offset applies.
    /// </summary>
    /// <threadsafety>This type is an immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class ZoneInterval : IEquatable<ZoneInterval>
    {
        private readonly Instant end;
        private readonly LocalInstant localEnd;
        private readonly LocalInstant localStart;
        private readonly string name;
        private readonly Offset wallOffset;
        private readonly Offset savings;
        private readonly Instant start;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneInterval" /> class.
        /// </summary>
        /// <param name="name">The name of this offset period (e.g. PST or PDT).</param>
        /// <param name="start">The first <see cref="Instant" /> that the <paramref name = "wallOffset" /> applies,
        /// or <c>null</c> to make the zone interval extend to the start of time.</param>
        /// <param name="end">The last <see cref="Instant" /> (exclusive) that the <paramref name = "wallOffset" /> applies,
        /// or <c>null</c> to make the zone interval extend to the end of time.</param>
        /// <param name="wallOffset">The <see cref="WallOffset" /> from UTC for this period including any daylight savings.</param>
        /// <param name="savings">The <see cref="WallOffset" /> daylight savings contribution to the offset.</param>
        /// <exception cref="ArgumentException">If <c><paramref name = "start" /> &gt;= <paramref name = "end" /></c>.</exception>
        public ZoneInterval([NotNull] string name, Instant? start, Instant? end, Offset wallOffset, Offset savings)
            : this(name, start ?? Instant.BeforeMinValue, end ?? Instant.AfterMaxValue, wallOffset, savings)
        {
        }
     
        /// <summary>
        /// Initializes a new instance of the <see cref="ZoneInterval" /> class.
        /// </summary>
        /// <param name="name">The name of this offset period (e.g. PST or PDT).</param>
        /// <param name="start">The first <see cref="Instant" /> that the <paramref name = "wallOffset" /> applies,
        /// or <see cref="Instant.BeforeMinValue"/> to make the zone interval extend to the start of time.</param>
        /// <param name="end">The last <see cref="Instant" /> (exclusive) that the <paramref name = "wallOffset" /> applies,
        /// or <see cref="Instant.AfterMaxValue"/> to make the zone interval extend to the end of time.</param>
        /// <param name="wallOffset">The <see cref="WallOffset" /> from UTC for this period including any daylight savings.</param>
        /// <param name="savings">The <see cref="WallOffset" /> daylight savings contribution to the offset.</param>
        /// <exception cref="ArgumentException">If <c><paramref name = "start" /> &gt;= <paramref name = "end" /></c>.</exception>
        internal ZoneInterval([NotNull] string name, Instant start, Instant end, Offset wallOffset, Offset savings)
        {
            Preconditions.CheckNotNull(name, "name");
            Preconditions.CheckArgument(start < end, "start", "The start Instant must be less than the end Instant");
            this.name = name;
            this.start = start;
            this.end = end;
            this.wallOffset = wallOffset;
            this.savings = savings;
            // FIXME(2.0): Work out what to do about these (and IsoLocalStart etc).
            // In particular, consider Instant.MinValue with a negative offset, etc.
            localStart = start == Instant.BeforeMinValue ? LocalInstant.BeforeMinValue : this.start.Plus(this.wallOffset);
            localEnd = end == Instant.AfterMaxValue ? LocalInstant.AfterMaxValue : this.end.Plus(this.wallOffset);
        }
        
        /// <summary>
        /// Returns a copy of this zone interval, but with the given start instant.
        /// </summary>
        internal ZoneInterval WithStart(Instant newStart)
        {
            return new ZoneInterval(name, newStart, end, wallOffset, savings);
        }

        /// <summary>
        /// Returns a copy of this zone interval, but with the given end instant.
        /// </summary>
        internal ZoneInterval WithEnd(Instant newEnd)
        {
            return new ZoneInterval(name, start, newEnd, wallOffset, savings);
        }

        #region Properties
        /// <summary>
        ///   Gets the standard offset for this period. This is the offset without any daylight savings
        ///   contributions.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>Offset - Savings</c>.
        /// </remarks>
        /// <value>The base Offset.</value>
        public Offset StandardOffset
        {
            [DebuggerStepThrough] get { return WallOffset - Savings; }
        }

        /// <summary>
        /// Gets the duration of this zone interval.
        /// </summary>
        /// <remarks>
        /// This is effectively <c>End - Start</c>.
        /// </remarks>
        /// <value>The Duration of this zone interval.</value>
        /// <exception cref="InvalidOperationException">This zone extends to the start or end of time.</exception>
        public Duration Duration
        {
            [DebuggerStepThrough] get { return End - Start; }
        }

        /// <summary>
        ///   Gets the last Instant (exclusive) that the Offset applies.
        /// </summary>
        /// <value>The last Instant (exclusive) that the Offset applies.</value>
        /// <exception cref="InvalidOperationException">The zone interval extends to the end of time</exception>
        public Instant End
        {
            [DebuggerStepThrough]
            get
            {
                Preconditions.CheckState(end.IsValid, "Zone interval extends to the end of time");
                return end;
            }
        }

        /// <summary>
        /// Returns the underlying end instant of this zone interval. If the zone interval extends to the
        /// end of time, the return value will be <see cref="Instant.AfterMaxValue"/>; this value
        /// should *not* be exposed publicly.
        /// </summary>
        internal Instant RawEnd { get { return end; } }

        /// <summary>
        /// Returns <c>true</c> if this zone interval has a fixed end point, or <c>false</c> if it
        /// extends to the end of time.
        /// </summary>
        /// <returns><c>true</c> if this interval has a fixed end point, or <c>false</c> if it
        /// extends to the end of time.</returns>
        public bool HasEnd { get { return end.IsValid; } }

        /// <summary>
        ///   Gets the end time as a LocalInstant.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>End + Offset</c>.
        /// </remarks>
        /// <value>The ending LocalInstant.</value>
        internal LocalInstant LocalEnd
        {
            [DebuggerStepThrough] get { return localEnd; }
        }

        /// <summary>
        ///   Gets the start time as a LocalInstant.
        /// </summary>
        /// <remarks>
        ///   This is effectively <c>Start + Offset</c>.
        /// </remarks>
        /// <value>The starting LocalInstant.</value>
        internal LocalInstant LocalStart
        {
            [DebuggerStepThrough] get { return localStart; }
        }

        /// <summary>
        /// Returns the local start time of the interval, as LocalDateTime
        /// in the ISO calendar.
        /// </summary>
        public LocalDateTime IsoLocalStart
        {
            [DebuggerStepThrough] get { return new LocalDateTime(localStart); }
        }

        /// <summary>
        /// Returns the local start time of the interval, as LocalDateTime
        /// in the ISO calendar.
        /// </summary>
        public LocalDateTime IsoLocalEnd
        {
            [DebuggerStepThrough]
            get { return new LocalDateTime(localEnd); }
        }
        /// <summary>
        ///   Gets the name of this offset period (e.g. PST or PDT).
        /// </summary>
        /// <value>The name of this offset period (e.g. PST or PDT).</value>
        public string Name
        {
            [DebuggerStepThrough] get { return name; }
        }

        /// <summary>
        ///   Gets the offset from UTC for this period. This includes any daylight savings value.
        /// </summary>
        /// <value>The offset from UTC for this period.</value>
        public Offset WallOffset
        {
            [DebuggerStepThrough] get { return wallOffset; }
        }

        /// <summary>
        ///   Gets the daylight savings value for this period.
        /// </summary>
        /// <value>The savings value.</value>
        public Offset Savings
        {
            [DebuggerStepThrough] get { return savings; }
        }

        /// <summary>
        ///   Gets the first Instant that the Offset applies.
        /// </summary>
        /// <value>The first Instant that the Offset applies.</value>
        public Instant Start
        {
            [DebuggerStepThrough]
            get
            {
                Preconditions.CheckState(start.IsValid, "Zone interval extends to the beginning of time");
                return start;
            }
        }

        /// <summary>
        /// Returns the underlying start instant of this zone interval. If the zone interval extends to the
        /// beginning of time, the return value will be <see cref="Instant.BeforeMinValue"/>; this value
        /// should *not* be exposed publicly.
        /// </summary>
        internal Instant RawStart { get { return start; } }

        /// <summary>
        /// Returns <c>true</c> if this zone interval has a fixed start point, or <c>false</c> if it
        /// extends to the beginning of time.
        /// </summary>
        /// <returns><c>true</c> if this interval has a fixed start point, or <c>false</c> if it
        /// extends to the beginning of time.</returns>
        public bool HasStart { get { return start.IsValid; } }
        #endregion // Properties

        #region Contains
        /// <summary>
        ///   Determines whether this period contains the given Instant in its range.
        /// </summary>
        /// <remarks>
        /// Usually this is half-open, i.e. the end is exclusive, but an interval with an end point of "the end of time" 
        /// is deemed to be inclusive at the end.
        /// </remarks>
        /// <param name="instant">The instant to test.</param>
        /// <returns>
        ///   <c>true</c> if this period contains the given Instant in its range; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        public bool Contains(Instant instant)
        {
            return start <= instant && instant < end;
        }

        /// <summary>
        ///   Determines whether this period contains the given LocalInstant in its range.
        /// </summary>
        /// <param name="localInstant">The local instant to test.</param>
        /// <returns>
        ///   <c>true</c> if this period contains the given LocalInstant in its range; otherwise, <c>false</c>.
        /// </returns>
        [DebuggerStepThrough]
        internal bool Contains(LocalInstant localInstant)
        {
            return localStart <= localInstant && localInstant < localEnd;
        }
        #endregion // Contains

        #region IEquatable<ZoneInterval> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        /// </param>
        [DebuggerStepThrough]
        public bool Equals(ZoneInterval other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return name == other.Name && RawStart == other.RawStart && RawEnd == other.RawEnd
                && WallOffset == other.WallOffset && Savings == other.Savings;
        }
        #endregion

        #region object Overrides
        /// <summary>
        ///   Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, <c>false</c>.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <filterpriority>2</filterpriority>
        [DebuggerStepThrough]
        public override bool Equals(object obj)
        {
            return Equals(obj as ZoneInterval);
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///   A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, Name);
            hash = HashCodeHelper.Hash(hash, RawStart);
            hash = HashCodeHelper.Hash(hash, RawEnd);
            hash = HashCodeHelper.Hash(hash, WallOffset);
            hash = HashCodeHelper.Hash(hash, Savings);
            return hash;
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: [{1}, {2}) {3} ({4})",
                Name,
                HasStart ? Start.ToString() : "StartOfTime",
                HasEnd ? End.ToString() : "EndOfTime",
                WallOffset, Savings);
        }
        #endregion // object Overrides
    }
}
