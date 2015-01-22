// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Represents a local date and time without reference to a calendar system,
    /// as the number of ticks since the Unix epoch which would represent that time
    /// of the same date in UTC. This needs a better description, and possibly a better name
    /// at some point...
    /// </summary>
    internal struct LocalInstant : IEquatable<LocalInstant>, IComparable<LocalInstant>, IComparable
    {
        public static readonly LocalInstant BeforeMinValue = new LocalInstant(Instant.BeforeMinValue.DaysSinceEpoch, deliberatelyInvalid: true);
        public static readonly LocalInstant AfterMaxValue = new LocalInstant(Instant.AfterMaxValue.DaysSinceEpoch, deliberatelyInvalid: true);

        /// <summary>
        /// Elapsed time since the local 1970-01-01T00:00:00.
        /// </summary>
        [ReadWriteForEfficiency] private Duration duration;

        /// <summary>
        /// Constructor which should *only* be used to construct the invalid instances.
        /// </summary>
        private LocalInstant([Trusted] int days, bool deliberatelyInvalid)
        {
            this.duration = new Duration(days, 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalInstant"/> struct.
        /// </summary>
        internal LocalInstant(Duration nanoseconds)
        {
            int days = nanoseconds.FloorDays;
            if (days < Instant.MinDays || days > Instant.MaxDays)
            {
                throw new OverflowException("Operation would overflow bounds of local date/time");
            }
            this.duration = nanoseconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalInstant"/> struct.
        /// </summary>
        /// <param name="days">Number of days since 1970-01-01, in a time zone neutral fashion.</param>
        /// <param name="nanoOfDay">Nanosecond of the local day.</param>
        internal LocalInstant([Trusted] int days, [Trusted] long nanoOfDay)
        {
            this.duration = new Duration(days, nanoOfDay);
        }

        /// <summary>
        /// Returns whether or not this is a valid instant. Returns true for all but
        /// <see cref="BeforeMinValue"/> and <see cref="AfterMaxValue"/>.
        /// </summary>
        internal bool IsValid => DaysSinceEpoch >= Instant.MinDays && DaysSinceEpoch <= Instant.MaxDays;

        /// <summary>
        /// Number of nanoseconds since the local unix epoch.
        /// </summary>
        internal Duration TimeSinceLocalEpoch => duration;

        /// <summary>
        /// Number of days since the local unix epoch.
        /// </summary>
        internal int DaysSinceEpoch => duration.FloorDays;

        /// <summary>
        /// Nanosecond within the day.
        /// </summary>
        internal long NanosecondOfDay => duration.NanosecondOfFloorDay;

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this LocalInstant which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Unspecified"/> and represents the same local date and time as this value.
        /// </summary>
        /// <remarks>
        /// <see cref="DateTimeKind.Unspecified"/> is slightly odd - it can be treated as UTC if you use <see cref="DateTime.ToLocalTime"/>
        /// or as system local time if you use <see cref="DateTime.ToUniversalTime"/>, but it's the only kind which allows
        /// you to construct a <see cref="DateTimeOffset"/> with an arbitrary offset, which makes it as close to
        /// the Noda Time non-system-specific "local" concept as exists in .NET.
        /// </remarks>
        [Pure]
        public DateTime ToDateTimeUnspecified() => 
            new DateTime(NodaConstants.BclTicksAtUnixEpoch + duration.Ticks, DateTimeKind.Unspecified);

        #region Operators
        // <summary>
        // Returns an instant after adding the given duration
        // </summary>
        public static LocalInstant operator +(LocalInstant left, Duration right) => new LocalInstant(left.duration + right);

        /// <summary>
        /// Returns a new instant based on this local instant, as if we'd applied a zero offset.
        /// This is just a slight optimization over calling <c>localInstant.Minus(Offset.Zero)</c>.
        /// </summary>
        internal Instant MinusZeroOffset() => new Instant(duration);

        /// <summary>
        /// Subtracts the given time zone offset from this local instant, to give an <see cref="Instant" />.
        /// </summary>
        /// <remarks>
        /// This would normally be implemented as an operator, but as the corresponding "plus" operation
        /// on Instant cannot be written (as Instant is a public type and LocalInstant is an internal type)
        /// it makes sense to keep them both as methods for consistency.
        /// </remarks>
        /// <param name="offset">The offset between UTC and a time zone for this local instant</param>
        /// <returns>A new <see cref="Instant"/> representing the difference of the given values.</returns>
        public Instant Minus(Offset offset) => new Instant(duration.MinusSmallNanoseconds(offset.Nanoseconds));

        /// <summary>
        /// Returns an instant after subtracting the given duration
        /// </summary>
        public static LocalInstant operator -(LocalInstant left, Duration right) => new LocalInstant(left.duration - right);

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(LocalInstant left, LocalInstant right) => left.duration == right.duration;

        /// <summary>
        /// Equivalent to <see cref="Instant.SafePlus"/>, but in the opposite direction.
        /// </summary>
        internal Instant SafeMinus(Offset offset)
        {
            int days = duration.FloorDays;
            // If we can do the arithmetic safely, do so.
            if (days > Instant.MinDays && days < Instant.MaxDays)
            {
                return Minus(offset);
            }
            // Handle BeforeMinValue and BeforeMaxValue simply.
            if (days < Instant.MinDays)
            {
                return Instant.BeforeMinValue;
            }
            if (days > Instant.MaxDays)
            {
                return Instant.AfterMaxValue;
            }
            // Okay, do the arithmetic as a Duration, then check the result for overflow, effectively.
            var asDuration = duration.PlusSmallNanoseconds(offset.Nanoseconds);
            if (asDuration.FloorDays < Instant.MinDays)
            {
                return Instant.BeforeMinValue;
            }
            if (asDuration.FloorDays > Instant.MaxDays)
            {
                return Instant.AfterMaxValue;
            }
            return new Instant(asDuration);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(LocalInstant left, LocalInstant right) => !(left == right);

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(LocalInstant left, LocalInstant right) => left.duration < right.duration;

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(LocalInstant left, LocalInstant right) => left.duration <= right.duration;

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(LocalInstant left, LocalInstant right) => left.duration > right.duration;

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(LocalInstant left, LocalInstant right) => left.duration >= right.duration;
        #endregion // Operators

        #region IComparable<LocalInstant> Members
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
        public int CompareTo(LocalInstant other) => duration.CompareTo(other.duration);

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two local instants.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="LocalInstant"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.LocalInstant)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is LocalInstant, nameof(obj), "Object must be of type NodaTime.LocalInstant.");
            return CompareTo((LocalInstant)obj);
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
        public override bool Equals(object obj) => obj is LocalInstant && Equals((LocalInstant)obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => duration.GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var date = new LocalDate(duration.FloorDays);
            var pattern = LocalDateTimePattern.CreateWithInvariantCulture("yyyy-MM-ddTHH:mm:ss LOC");
            var utc = new LocalDateTime(date, LocalTime.FromNanosecondsSinceMidnight(duration.NanosecondOfFloorDay));
            return pattern.Format(utc);
        }
        #endregion  // Object overrides

        #region IEquatable<LocalInstant> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(LocalInstant other) => this == other;
        #endregion
    }
}
