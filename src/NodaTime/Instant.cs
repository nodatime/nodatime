// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Calendars;
using NodaTime.NodaConstants;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Represents an instant on the global timeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An instant is defined by an integral number of 'ticks' since the Unix epoch (typically described as January 1st
    /// 1970, midnight, UTC, ISO calendar), where a tick is equal to 100 nanoseconds. There are 10,000 ticks in a
    /// millisecond.
    /// </para>
    /// <para>
    /// An <see cref="Instant"/> has no concept of a particular time zone or calendar: it simply represents a point in
    /// time that can be globally agreed-upon.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct Instant : IEquatable<Instant>, IComparable<Instant>, IFormattable, IComparable, IXmlSerializable
#if !PCL
        , ISerializable
#endif
    {
        // These correspond to -9998-01-01 and 9999-12-31 respectively.
        internal const int MinDays = -4371222;
        internal const int MaxDays = 2932896;

        private const long MinTicks = MinDays * TicksPerDay;
        private const long MaxTicks = (MaxDays + 1) * TicksPerDay - 1;
        private const long MinMilliseconds = MinDays * (long) MillisecondsPerDay;
        private const long MaxMilliseconds = (MaxDays + 1) * (long) MillisecondsPerDay - 1;
        private const long MinSeconds = MinDays * (long) SecondsPerDay;
        private const long MaxSeconds = (MaxDays + 1) * (long) SecondsPerDay - 1;

        /// <summary>
        /// Represents the smallest possible <see cref="Instant"/>.
        /// </summary>
        /// <remarks>This value is equivalent to -9998-01-01T00:00:00Z</remarks>
        public static Instant MinValue { get; } = new Instant(MinDays, 0);
        /// <summary>
        /// Represents the largest possible <see cref="Instant"/>.
        /// </summary>
        /// <remarks>This value is equivalent to 9999-12-31T23:59:59.999999999Z</remarks>
        public static Instant MaxValue { get; } = new Instant(MaxDays, NanosecondsPerDay - 1);

        /// <summary>
        /// Instant which is invalid *except* for comparison purposes; it is earlier than any valid value.
        /// This must never be exposed.
        /// </summary>
        internal static readonly Instant BeforeMinValue = new Instant(Duration.MinDays, deliberatelyInvalid: true);
        /// <summary>
        /// Instant which is invalid *except* for comparison purposes; it is later than any valid value.
        /// This must never be exposed.
        /// </summary>
        internal static readonly Instant AfterMaxValue = new Instant(Duration.MaxDays, deliberatelyInvalid: true);

        /// <summary>
        /// Time elapsed since the Unix epoch.
        /// </summary>
        [ReadWriteForEfficiency] private Duration duration;

        /// <summary>
        /// Constructor which should *only* be used to construct the invalid instances.
        /// </summary>
        private Instant([Trusted] int days, bool deliberatelyInvalid)
        {
            this.duration = new Duration(days, 0);
        }

        internal Instant(Duration duration)
        {
            // TODO(2.0): Check callers, and handle ones which might not need validation.
            this.duration = duration;
            int days = duration.FloorDays;
            if (days < MinDays || days > MaxDays)
            {
                throw new OverflowException("Operation would overflow range of Instant");
            }
        }

        internal Instant([Trusted] int days, [Trusted] long nanoOfDay)
        {
            Preconditions.DebugCheckArgumentRange(nameof(days), days, MinDays, MaxDays);
            Preconditions.DebugCheckArgumentRange(nameof(nanoOfDay), nanoOfDay, 0, NanosecondsPerDay - 1);
            duration = new Duration(days, nanoOfDay);
        }

        /// <summary>
        /// Returns whether or not this is a valid instant. Returns true for all but
        /// <see cref="BeforeMinValue"/> and <see cref="AfterMaxValue"/>.
        /// </summary>
        internal bool IsValid => DaysSinceEpoch >= MinDays && DaysSinceEpoch <= MaxDays;

        /// <summary>
        /// Gets the number of ticks since the Unix epoch. Negative values represent instants before the Unix epoch.
        /// </summary>
        /// <remarks>
        /// A tick is equal to 100 nanoseconds. There are 10,000 ticks in a millisecond. If the number of nanoseconds
        /// in this instant is not an exact number of ticks, the value is truncated towards the start of time.
        /// </remarks>
        /// <value>The number of ticks since the Unix epoch.</value>
        public long Ticks =>
            // Can't use Duration.Ticks, as that truncates towards 0.
            TickArithmetic.DaysAndTickOfDayToTicks(duration.FloorDays, duration.NanosecondOfFloorDay / NanosecondsPerTick);

        /// <summary>
        /// Get the elapsed time since the Unix epoch, to nanosecond resolution.
        /// </summary>
        /// <returns>The elapsed time since the Unix epoch.</returns>
        internal Duration TimeSinceEpoch => duration;

        /// <summary>
        /// Number of days since the local unix epoch.
        /// </summary>
        internal int DaysSinceEpoch => duration.FloorDays;

        /// <summary>
        /// Nanosecond within the day.
        /// </summary>
        internal long NanosecondOfDay => duration.NanosecondOfFloorDay;

        #region IComparable<Instant> and IComparable Members
        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   A 32-bit signed integer that indicates the relative order of the objects being compared.
        ///   The return value has the following meanings:
        ///   <list type = "table">
        ///     <listheader>
        ///       <term>Value</term>
        ///       <description>Meaning</description>
        ///     </listheader>
        ///     <item>
        ///       <term>&lt; 0</term>
        ///       <description>This object is less than the <paramref name = "other" /> parameter.</description>
        ///     </item>
        ///     <item>
        ///       <term>0</term>
        ///       <description>This object is equal to <paramref name = "other" />.</description>
        ///     </item>
        ///     <item>
        ///       <term>&gt; 0</term>
        ///       <description>This object is greater than <paramref name = "other" />.</description>
        ///     </item>
        ///   </list>
        /// </returns>
        public int CompareTo(Instant other) => duration.CompareTo(other.duration);

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two instants.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Instant"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.Instant)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is Instant, nameof(obj), "Object must be of type NodaTime.Instant.");
            return CompareTo((Instant)obj);
        }
        #endregion

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is Instant && Equals((Instant)obj);

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => duration.GetHashCode();
        #endregion  // Object overrides

        /// <summary>
        /// Returns a new value of this instant with the given number of ticks added to it.
        /// </summary>
        /// <param name="ticksToAdd">The ticks to add to this instant to create the return value.</param>
        /// <returns>The result of adding the given number of ticks to this instant.</returns>
        [Pure]
        public Instant PlusTicks(long ticksToAdd) => new Instant(duration + Duration.FromTicks(ticksToAdd));
        #region Operators
        /// <summary>
        /// Implements the operator + (addition) for <see cref="Instant" /> + <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant operator +(Instant left, Duration right) => new Instant(left.duration + right);

        /// <summary>
        /// Adds the given offset to this instant, to return a <see cref="LocalInstant" />.
        /// </summary>
        /// <remarks>
        /// This was previously an operator+ implementation, but operators can't be internal.
        /// </remarks>
        /// <param name="offset">The right hand side of the operator.</param>
        /// <returns>A new <see cref="LocalInstant" /> representing the sum of the given values.</returns>
        [Pure]
        internal LocalInstant Plus(Offset offset) => new LocalInstant(duration.PlusSmallNanoseconds(offset.Nanoseconds));

        /// <summary>
        /// Adds the given offset to this instant, either returning a normal LocalInstant,
        /// or <see cref="LocalInstant.BeforeMinValue"/> or <see cref="LocalInstant.AfterMaxValue"/>
        /// if the value would overflow.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        internal LocalInstant SafePlus(Offset offset)
        {
            int days = duration.FloorDays;
            // If we can do the arithmetic safely, do so.
            if (days > MinDays && days < MaxDays)
            {
                return Plus(offset);
            }
            // Handle BeforeMinValue and BeforeMaxValue simply.
            if (days < MinDays)
            {
                return LocalInstant.BeforeMinValue;
            }
            if (days > MaxDays)
            {
                return LocalInstant.AfterMaxValue;
            }
            // Okay, do the arithmetic as a Duration, then check the result for overflow, effectively.
            var asDuration = duration.PlusSmallNanoseconds(offset.Nanoseconds);
            if (asDuration.FloorDays < Instant.MinDays)
            {
                return LocalInstant.BeforeMinValue;
            }
            if (asDuration.FloorDays > Instant.MaxDays)
            {
                return LocalInstant.AfterMaxValue;
            }
            return new LocalInstant(asDuration);
        }

        /// <summary>
        /// Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant Add(Instant left, Duration right) => left + right;

        /// <summary>
        /// Returns the result of adding a duration to this instant, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="duration">The duration to add</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the addition.</returns>
        [Pure]
        public Instant Plus(Duration duration) => this + duration;

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Instant" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration" /> representing the difference of the given values.</returns>
        public static Duration operator -(Instant left, Instant right) => left.duration - right.duration;

        /// <summary>
        /// Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        public static Instant operator -(Instant left, Duration right) => new Instant(left.duration - right);

        /// <summary>
        ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration" /> representing the difference of the given values.</returns>
        public static Duration Subtract(Instant left, Instant right) => left - right;

        /// <summary>
        /// Returns the result of subtracting another instant from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The other instant to subtract</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the subtraction.</returns>
        [Pure]
        public Duration Minus(Instant other) => this - other;

        /// <summary>
        /// Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        [Pure]
        public static Instant Subtract(Instant left, Duration right) => left - right;

        /// <summary>
        /// Returns the result of subtracting a duration from this instant, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="duration">The duration to subtract</param>
        /// <returns>A new <see cref="Instant" /> representing the result of the subtraction.</returns>
        [Pure]
        public Instant Minus(Duration duration) => this - duration;

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Instant left, Instant right) => left.duration == right.duration;

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Instant left, Instant right) => !(left == right);

        /// <summary>
        ///   Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Instant left, Instant right) => left.duration < right.duration;

        /// <summary>
        ///   Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Instant left, Instant right) => left.duration <= right.duration;

        /// <summary>
        ///   Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Instant left, Instant right) => left.duration > right.duration;

        /// <summary>
        ///   Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Instant left, Instant right) => left.duration >= right.duration;
        #endregion // Operators

        #region Convenience methods
        /// <summary>
        /// Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
        /// In most cases applications should use <see cref="ZonedDateTime" /> to represent a date
        /// and time, but this method is useful in some situations where an <see cref="Instant" /> is
        /// required, such as time zone testing.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="hourOfDay">The hour.</param>
        /// <param name="minuteOfHour">The minute.</param>
        /// <returns>An <see cref="Instant"/> value representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            var days = new LocalDate(year, monthOfYear, dayOfMonth).DaysSinceEpoch;
            var nanoOfDay = new LocalTime(hourOfDay, minuteOfHour).NanosecondOfDay;
            return new Instant(days, nanoOfDay);
        }

        /// <summary>
        /// Returns a new instant corresponding to the given UTC date and
        /// time in the ISO calendar. In most cases applications should 
        /// use <see cref="ZonedDateTime" />
        /// to represent a date and time, but this method is useful in some 
        /// situations where an Instant is required, such as time zone testing.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="hourOfDay">The hour.</param>
        /// <param name="minuteOfHour">The minute.</param>
        /// <param name="secondOfMinute">The second.</param>
        /// <returns>An <see cref="Instant"/> value representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            var days = new LocalDate(year, monthOfYear, dayOfMonth).DaysSinceEpoch;
            var nanoOfDay = new LocalTime(hourOfDay, minuteOfHour, secondOfMinute).NanosecondOfDay;
            return new Instant(days, nanoOfDay);
        }

        /// <summary>
        /// Returns the later instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The later instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Max(Instant x, Instant y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the earlier instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The earlier instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Min(Instant x, Instant y) => x < y ? x : y;
        #endregion

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("g"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() => InstantPattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("g").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider) =>
            InstantPattern.BclSupport.Format(this, patternText, formatProvider);
        #endregion Formatting

        #region IEquatable<Instant> Members
        /// <summary>
        /// Indicates whether the value of this instant is equal to the value of the specified instant.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Instant other) => this == other;
        #endregion

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this Instant which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as this value.
        /// </summary>
        /// <returns>A <see cref="DateTime"/> representing the same instant in time as this value, with a kind of "universal".</returns>
        [Pure]
        public DateTime ToDateTimeUtc() => new DateTime(BclTicksAtUnixEpoch + Ticks, DateTimeKind.Utc);

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> from this Instant which has an offset of zero.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> representing the same instant in time as this value.</returns>
        [Pure]
        public DateTimeOffset ToDateTimeOffset() => new DateTimeOffset(BclTicksAtUnixEpoch + Ticks, TimeSpan.Zero);

        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> into a new Instant representing the same instant in time. Note that
        /// the offset information is not preserved in the returned Instant.
        /// </summary>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as the given <see cref="DateTimeOffset"/>.</returns>
        /// <param name="dateTimeOffset">Date and time value with an offset.</param>
        public static Instant FromDateTimeOffset(DateTimeOffset dateTimeOffset) =>
            BclEpoch.PlusTicks(dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks);

        /// <summary>
        /// Converts a <see cref="DateTime"/> into a new Instant representing the same instant in time.
        /// </summary>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as the given universal <see cref="DateTime"/>.</returns>
        /// <param name="dateTime">Date and time value which must have a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/></param>
        /// <exception cref="ArgumentException"><paramref name="dateTime"/> is not of <see cref="DateTime.Kind"/>
        /// <see cref="DateTimeKind.Utc"/>.</exception>
        public static Instant FromDateTimeUtc(DateTime dateTime)
        {
            Preconditions.CheckArgument(dateTime.Kind == DateTimeKind.Utc, nameof(dateTime), "Invalid DateTime.Kind for Instant.FromDateTimeUtc");
            return BclEpoch.PlusTicks(dateTime.Ticks);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant" /> struct based
        /// on a number of seconds since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
        /// </summary>
        /// <param name="seconds">Number of seconds since the Unix epoch. May be negative (for instants before the epoch).</param>
        /// <returns>An <see cref="Instant"/> at exactly the given number of seconds since the Unix epoch.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The constructed instant would be out of the range representable in Noda Time.</exception>
        public static Instant FromSecondsSinceUnixEpoch(long seconds)
        {
            Preconditions.CheckArgumentRange(nameof(seconds), seconds, MinSeconds, MaxSeconds);
            return new Instant(Duration.FromSeconds(seconds));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant" /> struct based
        /// on a number of milliseconds since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
        /// </summary>
        /// <param name="milliseconds">Number of milliseconds since the Unix epoch. May be negative (for instants before the epoch).</param>
        /// <returns>An <see cref="Instant"/> at exactly the given number of milliseconds since the Unix epoch.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The constructed instant would be out of the range representable in Noda Time.</exception>
        public static Instant FromMillisecondsSinceUnixEpoch(long milliseconds)
        {
            Preconditions.CheckArgumentRange(nameof(milliseconds), milliseconds, MinMilliseconds, MaxMilliseconds);
            return new Instant(Duration.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant" /> struct based
        /// on a number of ticks since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
        /// </summary>
        /// <remarks>This is equivalent to calling the constructor directly, but indicates
        /// intent more explicitly.</remarks>
        /// <returns>An <see cref="Instant"/> at exactly the given number of ticks since the Unix epoch.</returns>
        /// <param name="ticks">Number of ticks since the Unix epoch. May be negative (for instants before the epoch).</param>
        public static Instant FromTicksSinceUnixEpoch(long ticks)
        {
            Preconditions.CheckArgumentRange(nameof(ticks), ticks, MinTicks, MaxTicks);
            return new Instant(Duration.FromTicks(ticks));
        }

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the UTC time
        /// zone and ISO-8601 calendar. This is a shortcut for calling <see cref="InZone(DateTimeZone)" /> with an
        /// argument of <see cref="DateTimeZone.Utc"/>.
        /// </summary>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the UTC time zone
        /// and the ISO-8601 calendar</returns>
        [Pure]
        public ZonedDateTime InUtc()
        {
            // Bypass any determination of offset and arithmetic, as we know the offset is zero.
            var ymdc = GregorianYearMonthDayCalculator.GetGregorianYearMonthDayCalendarFromDaysSinceEpoch(duration.FloorDays);
            var offsetDateTime = new OffsetDateTime(ymdc, duration.NanosecondOfFloorDay);
            return new ZonedDateTime(offsetDateTime, DateTimeZone.Utc);
        }

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the
        /// specified time zone and ISO-8601 calendar.
        /// </summary>
        /// <param name="zone">The time zone in which to represent this instant.</param>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the given time zone
        /// and the ISO-8601 calendar</returns>
        [Pure]
        public ZonedDateTime InZone([NotNull] DateTimeZone zone) =>
            // zone is checked for nullity by the constructor.
            new ZonedDateTime(this, zone);

        /// <summary>
        /// Returns the <see cref="ZonedDateTime"/> representing the same point in time as this instant, in the
        /// specified time zone and calendar system.
        /// </summary>
        /// <param name="zone">The time zone in which to represent this instant.</param>
        /// <param name="calendar">The calendar system in which to represent this instant.</param>
        /// <returns>A <see cref="ZonedDateTime"/> for the same instant, in the given time zone
        /// and calendar</returns>
        [Pure]
        public ZonedDateTime InZone([NotNull] DateTimeZone zone, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(zone, nameof(zone));
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            return new ZonedDateTime(this, zone, calendar);
        }

        /// <summary>
        /// Returns the <see cref="OffsetDateTime"/> representing the same point in time as this instant, with
        /// the specified UTC offset in the ISO calendar system.
        /// </summary>
        /// <param name="offset">The offset from UTC with which to represent this instant.</param>
        /// <returns>An <see cref="OffsetDateTime"/> for the same instant, with the given offset
        /// in the ISO calendar system</returns>
        [Pure]
        public OffsetDateTime WithOffset(Offset offset) => new OffsetDateTime(this, offset);

        /// <summary>
        /// Returns the <see cref="OffsetDateTime"/> representing the same point in time as this instant, with
        /// the specified UTC offset and calendar system.
        /// </summary>
        /// <param name="offset">The offset from UTC with which to represent this instant.</param>
        /// <param name="calendar">The calendar system in which to represent this instant.</param>
        /// <returns>An <see cref="OffsetDateTime"/> for the same instant, with the given offset
        /// and calendar</returns>
        [Pure]
        public OffsetDateTime WithOffset(Offset offset, [NotNull] CalendarSystem calendar)
        {
            Preconditions.CheckNotNull(calendar, nameof(calendar));
            return new OffsetDateTime(this, offset, calendar);
        }

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = InstantPattern.ExtendedIsoPattern;
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteString(InstantPattern.ExtendedIsoPattern.Format(this));
        }
        #endregion

#if !PCL
        #region Binary serialization
        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Instant(SerializationInfo info, StreamingContext context)
            // FIXME:SERIALIZATION COMPATIBILITY
            : this(new Duration(info))
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            duration.Serialize(info);
        }
        #endregion
#endif
    }
}
