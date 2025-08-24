// Copyright 2025 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;
using System;
using System.Numerics;
using static NodaTime.NodaConstants;

namespace NodaTime.HighPerformance;

/// <summary>
/// Represents an instant on the global timeline, with nanosecond resolution.
/// This type is a equivalent to <see cref="Instant"/>, but with a more more limited range (a few hundred years either
/// side of the Unix epoch) and more compact and high-performance representation.
/// It is expected to be used in conjunction with <see cref="Duration64"/>,
/// typically in scenarios where performance and/or memory usage are important.
/// </summary>
/// <remarks>
/// <para>
/// An <see cref="Instant64"/> has no concept of a particular time zone or calendar: it simply represents a point in
/// time that can be globally agreed-upon.
/// </para>
/// <para>
/// Equality and ordering comparisons are defined in the natural way, with earlier points on the timeline
/// being considered "less than" later points.
/// </para>
/// <para>The default value of this type is <see cref="UnixEpoch"/>, i.e. the instant
/// which can be represented as 1970-01-01T00:00:00Z in the ISO calendar.</para>
/// </remarks>
/// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
public readonly struct Instant64 : IEquatable<Instant64>, IComparable<Instant64>, IFormattable, IComparable
#if NET8_0_OR_GREATER
    , IAdditionOperators<Instant64, Duration64, Instant64>
    , ISubtractionOperators<Instant64, Duration64, Instant64>
    , ISubtractionOperators<Instant64, Instant64, Duration64>
    , IComparisonOperators<Instant64, Instant64, bool>
    , IMinMaxValue<Instant64>
#endif
{
    /// <summary>
    /// The instant at the Unix epoch of midnight 1st January 1970 UTC.
    /// </summary>
    public static Instant64 UnixEpoch { get; } = new Instant64(0);

    /// <summary>
    /// Represents the smallest possible <see cref="Instant64"/>.
    /// </summary>
    /// <remarks>This value is equivalent to 1677-09-21T00:12:43.145224192Z.</remarks>
    public static Instant64 MinValue { get; } = new Instant64(long.MinValue);
    /// <summary>
    /// Represents the largest possible <see cref="Instant64"/>.
    /// </summary>
    /// <remarks>This value is equivalent to 2262-04-11T23:47:16.854775807Z.</remarks>
    public static Instant64 MaxValue { get; } = new Instant64(long.MaxValue);

    private readonly long nanoseconds;

    /// <summary>
    /// Constructor which constructs a new instance with the given number of nanoseconds since the Unix epoch.
    /// </summary>
    private Instant64(long nanoseconds)
    {
        this.nanoseconds = nanoseconds;
    }

    /// <summary>
    /// Get the elapsed time since the Unix epoch, to nanosecond resolution.
    /// </summary>
    /// <returns>The elapsed time since the Unix epoch.</returns>
    internal Duration64 TimeSinceEpoch => new(nanoseconds);

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// See the type documentation for a description of ordering semantics.
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
    public int CompareTo(Instant64 other) => nanoseconds.CompareTo(other.nanoseconds);

    /// <summary>
    /// Implementation of <see cref="IComparable.CompareTo"/> to compare two instants.
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <remarks>
    /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Instant64"/>.</exception>
    /// <param name="obj">The object to compare this value with.</param>
    /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.HighPerformance.Instant64)"/> for general details.
    /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
    /// </returns>
    int IComparable.CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }
        Preconditions.CheckArgument(obj is Instant64, nameof(obj), "Object must be of type NodaTime.HighPerformance.Instant64.");
        return CompareTo((Instant64) obj);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => obj is Instant64 other && Equals(other);

    /// <summary>
    /// Returns a hash code for this instance.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data
    /// structures like a hash table.
    /// </returns>
    public override int GetHashCode() => nanoseconds.GetHashCode();

    /// <summary>
    /// Returns a new value of this instant with the given number of ticks added to it.
    /// </summary>
    /// <param name="ticks">The ticks to add to this instant to create the return value.</param>
    /// <returns>The result of adding the given number of ticks to this instant.</returns>
    [Pure]
    public Instant64 PlusTicks(long ticks) => new(nanoseconds + ticks * NanosecondsPerTick);

    /// <summary>
    /// Returns a new value of this instant with the given number of nanoseconds added to it.
    /// </summary>
    /// <param name="nanoseconds">The nanoseconds to add to this instant to create the return value.</param>
    /// <returns>The result of adding the given number of ticks to this instant.</returns>
    [Pure]
    public Instant64 PlusNanoseconds(long nanoseconds) => new(this.nanoseconds + nanoseconds);

    /// <summary>
    /// Implements the operator + (addition) for <see cref="Instant64" /> + <see cref="Duration64" />.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Instant64" /> representing the sum of the given values.</returns>
    public static Instant64 operator +(Instant64 left, Duration64 right) => new(left.nanoseconds + right.Nanoseconds);

    /// <summary>
    /// Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Instant64" /> representing the sum of the given values.</returns>
    public static Instant64 Add(Instant64 left, Duration64 right) => left + right;

    /// <summary>
    /// Returns the result of adding a duration to this instant, for a fluent alternative to <c>operator+()</c>.
    /// </summary>
    /// <param name="duration">The duration to add</param>
    /// <returns>A new <see cref="Instant64" /> representing the result of the addition.</returns>
    [Pure]
    public Instant64 Plus(Duration64 duration) => this + duration;

    /// <summary>
    ///   Implements the operator - (subtraction) for <see cref="Instant64" /> - <see cref="Instant64" />.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64" /> representing the difference of the given values.</returns>
    public static Duration64 operator -(Instant64 left, Instant64 right) => new(left.nanoseconds - right.nanoseconds);

    /// <summary>
    /// Implements the operator - (subtraction) for <see cref="Instant64" /> - <see cref="Duration64" />.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Instant64" /> representing the difference of the given values.</returns>
    public static Instant64 operator -(Instant64 left, Duration64 right) => new(left.nanoseconds - right.Nanoseconds);

    /// <summary>
    ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64" /> representing the difference of the given values.</returns>
    public static Duration64 Subtract(Instant64 left, Instant64 right) => left - right;

    /// <summary>
    /// Returns the result of subtracting another instant from this one, for a fluent alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="other">The other instant to subtract</param>
    /// <returns>A new <see cref="Instant64" /> representing the result of the subtraction.</returns>
    [Pure]
    public Duration64 Minus(Instant64 other) => this - other;

    /// <summary>
    /// Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Instant64" /> representing the difference of the given values.</returns>
    [Pure]
    public static Instant64 Subtract(Instant64 left, Duration64 right) => left - right;

    /// <summary>
    /// Returns the result of subtracting a duration from this instant, for a fluent alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="duration">The duration to subtract</param>
    /// <returns>A new <see cref="Instant64" /> representing the result of the subtraction.</returns>
    [Pure]
    public Instant64 Minus(Duration64 duration) => this - duration;

    /// <summary>
    /// Implements the operator == (equality).
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
    public static bool operator ==(Instant64 left, Instant64 right) => left.nanoseconds == right.nanoseconds;

    /// <summary>
    /// Implements the operator != (inequality).
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
    public static bool operator !=(Instant64 left, Instant64 right) => !(left == right);

    /// <summary>
    /// Implements the operator &lt; (less than).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
    public static bool operator <(Instant64 left, Instant64 right) => left.nanoseconds < right.nanoseconds;

    /// <summary>
    /// Implements the operator &lt;= (less than or equal).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
    public static bool operator <=(Instant64 left, Instant64 right) => left.nanoseconds <= right.nanoseconds;

    /// <summary>
    /// Implements the operator &gt; (greater than).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
    public static bool operator >(Instant64 left, Instant64 right) => left.nanoseconds > right.nanoseconds;

    /// <summary>
    /// Implements the operator &gt;= (greater than or equal).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
    public static bool operator >=(Instant64 left, Instant64 right) => left.nanoseconds >= right.nanoseconds;

    /// <summary>
    /// Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
    /// </summary>
    /// <param name="year">The year. This is the "absolute year",
    /// so a value of 0 means 1 BC, for example.</param>
    /// <param name="monthOfYear">The month of year.</param>
    /// <param name="dayOfMonth">The day of month.</param>
    /// <param name="hourOfDay">The hour.</param>
    /// <param name="minuteOfHour">The minute.</param>
    /// <returns>An <see cref="Instant64"/> value representing the given date and time in UTC and the ISO calendar.</returns>
    public static Instant64 FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
    {
        var days = new LocalDate(year, monthOfYear, dayOfMonth).DaysSinceEpoch;
        var nanoOfDay = new LocalTime(hourOfDay, minuteOfHour).NanosecondOfDay;
        return new Instant64(days * NanosecondsPerDay + nanoOfDay);
    }

    /// <summary>
    /// Returns a new instant corresponding to the given UTC date and
    /// time in the ISO calendar.
    /// </summary>
    /// <param name="year">The year. This is the "absolute year",
    /// so a value of 0 means 1 BC, for example.</param>
    /// <param name="monthOfYear">The month of year.</param>
    /// <param name="dayOfMonth">The day of month.</param>
    /// <param name="hourOfDay">The hour.</param>
    /// <param name="minuteOfHour">The minute.</param>
    /// <param name="secondOfMinute">The second.</param>
    /// <returns>An <see cref="Instant64"/> value representing the given date and time in UTC and the ISO calendar.</returns>
    public static Instant64 FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
    {
        var days = new LocalDate(year, monthOfYear, dayOfMonth).DaysSinceEpoch;
        var nanoOfDay = new LocalTime(hourOfDay, minuteOfHour, secondOfMinute).NanosecondOfDay;

        // For extreme negative values, the naive expression may overflow before we get it back into range.
        return days >= 0
            ? new Instant64(days * NanosecondsPerDay + nanoOfDay)
            : new Instant64((days + 1) * NanosecondsPerDay + nanoOfDay - NanosecondsPerDay);
    }

    /// <summary>
    /// Returns the later instant of the given two.
    /// </summary>
    /// <param name="x">The first instant to compare.</param>
    /// <param name="y">The second instant to compare.</param>
    /// <returns>The later instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
    public static Instant64 Max(Instant64 x, Instant64 y) => x > y ? x : y;

    /// <summary>
    /// Returns the earlier instant of the given two.
    /// </summary>
    /// <param name="x">The first instant to compare.</param>
    /// <param name="y">The second instant to compare.</param>
    /// <returns>The earlier instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
    public static Instant64 Min(Instant64 x, Instant64 y) => x < y ? x : y;

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// The value of the current instance, converted to an <see cref="Instant"/>, in the default format pattern ("g"), using the current thread's
    /// culture to obtain a format provider.
    /// </returns>
    public override string ToString() => ToInstant().ToString();

    /// <summary>
    /// Formats the value of the current instance, converted to an <see cref="Instant"/>, using the specified pattern.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> containing the value of the current instance in the specified format.
    /// </returns>
    /// <param name="patternText">The <see cref="System.String" /> specifying the pattern to use,
    /// or null to use the default format pattern ("g").
    /// </param>
    /// <param name="formatProvider">The <see cref="System.IFormatProvider" /> to use when formatting the value,
    /// or null to use the current thread's culture to obtain a format provider.
    /// </param>
    /// <filterpriority>2</filterpriority>
    public string ToString(string? patternText, IFormatProvider? formatProvider) =>
        ToInstant().ToString(patternText, formatProvider);

    /// <summary>
    /// Indicates whether the value of this instant is equal to the value of the specified instant.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="other">The value to compare with this instance.</param>
    /// <returns>
    /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
    /// otherwise, false.
    /// </returns>
    public bool Equals(Instant64 other) => this == other;

    /// <summary>
    /// Initializes a new instance of the <see cref="Instant64" /> struct based
    /// on a number of seconds since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
    /// </summary>
    /// <param name="seconds">Number of seconds since the Unix epoch. May be negative (for instants before the epoch).</param>
    /// <returns>An <see cref="Instant64"/> at exactly the given number of seconds since the Unix epoch.</returns>
    /// <exception cref="OverflowException">The constructed instant would be out of the representable range.</exception>
    public static Instant64 FromUnixTimeSeconds(long seconds) => new(seconds * NanosecondsPerSecond);

    /// <summary>
    /// Initializes a new instance of the <see cref="Instant64" /> struct based
    /// on a number of milliseconds since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
    /// </summary>
    /// <param name="milliseconds">Number of milliseconds since the Unix epoch. May be negative (for instants before the epoch).</param>
    /// <returns>An <see cref="Instant64"/> at exactly the given number of milliseconds since the Unix epoch.</returns>
    /// <exception cref="OverflowException">The constructed instant would be out of the representable range.</exception>
    public static Instant64 FromUnixTimeMilliseconds(long milliseconds) => new(milliseconds * NanosecondsPerMillisecond);

    /// <summary>
    /// Initializes a new instance of the <see cref="Instant64" /> struct based
    /// on a number of ticks since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
    /// </summary>
    /// <returns>An <see cref="Instant64"/> at exactly the given number of ticks since the Unix epoch.</returns>
    /// <param name="ticks">Number of ticks since the Unix epoch. May be negative (for instants before the epoch).</param>
    /// /// <exception cref="OverflowException">The constructed instant would be out of the representable range.</exception>
    public static Instant64 FromUnixTimeTicks(long ticks) => new(ticks * NanosecondsPerTick);

    /// <summary>
    /// Initializes a new instance of the <see cref="Instant64" /> struct based
    /// on a number of nanoseconds since the Unix epoch of (ISO) January 1st 1970, midnight, UTC.
    /// </summary>
    /// <param name="nanoseconds">Number of nanoseconds since the Unix epoch. May be negative (for instants before the epoch).</param>
    /// <returns>An <see cref="Instant64"/> at exactly the given number of seconds since the Unix epoch.</returns>
    public static Instant64 FromUnixTimeNanoseconds(long nanoseconds) => new(nanoseconds);

    /// <summary>
    /// Gets the number of seconds since the Unix epoch. Negative values represent instants before the Unix epoch.
    /// </summary>
    /// <remarks>
    /// If the number of nanoseconds in this instant is not an exact number of seconds, the value is truncated towards the start of time.
    /// </remarks>
    /// <value>The number of seconds since the Unix epoch.</value>
    [Pure]
    [TestExemption(TestExemptionCategory.ConversionName)]
    public long ToUnixTimeSeconds() => DivideAndFloor(NanosecondsPerSecond);

    /// <summary>
    /// Gets the number of seconds since the Unix epoch, along with remaining nanoseconds.
    /// Negative values for seconds represent instants before the Unix epoch.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the number of nanoseconds in this instant is not an exact number of seconds
    /// the seconds part of the returned value is truncated towards the start of time, ensuring that
    /// the nanoseconds part is always non-negative. The seconds part of the returned value is always
    /// the same as would be returned by <see cref="ToUnixTimeSeconds"/>.
    /// </para>
    /// <para>
    /// The inverse of this operation is to first call <see cref="FromUnixTimeSeconds(long)"/> and then
    /// call <see cref="PlusNanoseconds(long)"/> on the returned <see cref="Instant"/>.
    /// </para>
    /// </remarks>
    /// <value>The number of seconds and remaining nanoseconds since the Unix epoch.</value>
    [Pure]
    [TestExemption(TestExemptionCategory.ConversionName)]
    public (long seconds, int nanoseconds) ToUnixTimeSecondsAndNanoseconds()
    {
        var seconds = Math.DivRem(nanoseconds, NanosecondsPerSecond, out var remainder);
        return remainder >= 0 ? (seconds, (int) remainder) : (seconds - 1, (int) (remainder + NanosecondsPerSecond));
    }

    /// <summary>
    /// Gets the number of milliseconds since the Unix epoch. Negative values represent instants before the Unix epoch.
    /// </summary>
    /// <remarks>
    /// If the number of nanoseconds in this instant is not an exact number of milliseconds, the value is truncated towards the start of time.
    /// </remarks>
    /// <value>The number of milliseconds since the Unix epoch.</value>
    [Pure]
    [TestExemption(TestExemptionCategory.ConversionName)]
    public long ToUnixTimeMilliseconds() => DivideAndFloor(NanosecondsPerMillisecond);

    /// <summary>
    /// Gets the number of ticks since the Unix epoch. Negative values represent instants before the Unix epoch.
    /// </summary>
    /// <remarks>
    /// A tick is equal to 100 nanoseconds. There are 10,000 ticks in a millisecond. If the number of nanoseconds
    /// in this instant is not an exact number of ticks, the value is truncated towards the start of time.
    /// </remarks>
    /// <returns>The number of ticks since the Unix epoch.</returns>
    [Pure]
    [TestExemption(TestExemptionCategory.ConversionName)]
    public long ToUnixTimeTicks() => DivideAndFloor(NanosecondsPerTick);

    /// <summary>
    /// Gets the number of nanoseconds since the Unix epoch. Negative values represent instants before the Unix epoch.
    /// </summary>
    /// <returns>The number of ticks since the Unix epoch.</returns>
    [Pure]
    [TestExemption(TestExemptionCategory.ConversionName)]
    public long ToUnixTimeNanoseconds() => nanoseconds;

    /// <summary>
    /// Converts this value to an <see cref="Instant"/> representing the same instant in time.
    /// This operation always succeeds and loses no information.
    /// </summary>
    /// <returns>An <see cref="Instant64"/> representing the same instant in time as this one.</returns>
    [Pure]
    public Instant ToInstant() => Instant.FromTrustedDuration(Duration.FromNanoseconds(nanoseconds));

    /// <summary>
    /// Creates an <see cref="Instant64"/> value representing the same instant in time as the specified
    /// <see cref="Instant"/>. When this succeeds, this conversion loses no information.
    /// If the specified value is outside the range of <see cref="Instant64"/>, this method will fail with an
    /// <see cref="OverflowException"/>.
    /// </summary>
    /// <param name="instant">The value to convert to a <see cref="Instant64"/>.</param>
    /// <returns>An <see cref="Instant64"/> value equivalent to <paramref name="instant"/>.</returns>
    /// <exception cref="OverflowException"><paramref name="instant"/> has a value outside
    /// the range of <see cref="Instant64"/>.</exception>
    public static Instant64 FromInstant(Instant instant) => new(instant.DaysSinceEpoch * NanosecondsPerDay + instant.NanosecondOfDay);

    private long DivideAndFloor(long nanosecondsPerUnit)
    {
        var result = Math.DivRem(nanoseconds, nanosecondsPerUnit, out var remainder);
        return remainder >= 0 ? result : result - 1;
    }
}
