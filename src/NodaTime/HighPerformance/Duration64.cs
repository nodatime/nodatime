// Copyright 2025 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using JetBrains.Annotations;
using NodaTime.Utility;
using System;
using System.Numerics;
using static NodaTime.NodaConstants;

namespace NodaTime.HighPerformance;

/// <summary>
/// Represents a fixed (and calendar-independent) length of time.
/// This type is a equivalent to <see cref="Duration"/>, but with a more limited range (a few hundred years)
/// and more compact, high performance representation. It is expected to be used in conjunction with <see cref="Instant64"/>,
/// typically in scenarios where performance and/or memory usage are important. Note that in most cases,
/// <see cref="Duration"/> is more appropriate and convenient (with more supported methods etc). This should effectively
/// be regarded as a specialist type for unusually performance-sensitive scenarios.
/// </summary>
/// <remarks>
/// <para>
/// A duration is a length of time defined by an integral number of nanoseconds.
/// Although durations are usually used with a positive number of nanoseconds, negative durations are valid, and may occur
/// naturally when e.g. subtracting a later <see cref="Instant64"/> from an earlier one.
/// </para>
/// <para>Equality and ordering are defined in the natural way, simply comparing the number of nanoseconds contained.</para>
/// <para>The default value of this type is <see cref="Zero"/>.</para>
/// </remarks>
/// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
public readonly struct Duration64 : IEquatable<Duration64>, IComparable<Duration64>, IComparable, IFormattable
#if NET8_0_OR_GREATER
    , IAdditionOperators<Duration64, Duration64, Duration64>
    , ISubtractionOperators<Duration64, Duration64, Duration64>
    , IUnaryNegationOperators<Duration64, Duration64>
    , IUnaryPlusOperators<Duration64, Duration64>
    , IComparisonOperators<Duration64, Duration64, bool>
    , IMinMaxValue<Duration64>
    , IAdditiveIdentity<Duration64, Duration64>
#endif
{
    /// <summary>
    /// Gets a zero <see cref="Duration64"/> of 0 nanoseconds.
    /// </summary>
    /// <value>The zero <see cref="Duration64"/> value.</value>
    public static Duration64 Zero => default;

    /// <summary>
    /// Gets the additive identity.
    /// </summary>
    public static Duration64 AdditiveIdentity => Zero;

    /// <summary>
    /// Gets a <see cref="Duration64"/> value equal to 1 nanosecond; the smallest amount by which an instant can vary.
    /// </summary>
    /// <value>A duration representing 1 nanosecond.</value>
    public static Duration64 Epsilon => new(1L);

    /// <summary>
    /// Gets the maximum value supported by <see cref="Duration64"/>.
    /// This is around 292 years.
    /// </summary>
    public static Duration64 MaxValue => new(long.MaxValue);

    /// <summary>
    /// Gets the minimum (largest negative) value supported by <see cref="Duration64"/>.
    /// This is around -292 years.
    /// </summary>
    public static Duration64 MinValue => new(long.MinValue);
    
    internal long Nanoseconds { get; }

    internal Duration64(long nanoseconds)
    {
        Nanoseconds = nanoseconds;
    }

    /// <summary>
    /// Gets the total number of nanoseconds in this duration, as an <see cref="Int64"/>.
    /// </summary>
    public long TotalNanoseconds => Nanoseconds;

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
    /// otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj) => obj is Duration64 other && Equals(other);

    /// <summary>
    /// Returns a hash code for this instance.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data
    /// structures like a hash table.
    /// </returns>
    public override int GetHashCode() => Nanoseconds.GetHashCode();

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// The value of the current instance, converted to a <see cref="Duration"/>, in the default format pattern ("o"), using the current thread's
    /// culture to obtain a format provider.
    /// </returns>
    public override string ToString() => ToDuration().ToString();

    /// <summary>
    /// Formats the value of the current instance using the specified pattern.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> containing the value of the current instance, converted to a <see cref="Duration"/> in the specified format.
    /// </returns>
    /// <param name="patternText">The <see cref="System.String" /> specifying the pattern to use,
    /// or null to use the default format pattern ("o").
    /// </param>
    /// <param name="formatProvider">The <see cref="System.IFormatProvider" /> to use when formatting the value,
    /// or null to use the current thread's culture to obtain a format provider.
    /// </param>
    /// <filterpriority>2</filterpriority>
    public string ToString(string? patternText, IFormatProvider? formatProvider) => ToDuration().ToString(patternText, formatProvider);

    #region Operators
    /// <summary>
    /// Implements the operator + (addition).
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64"/> representing the sum of the given values.</returns>
    public static Duration64 operator +(Duration64 left, Duration64 right) => new(left.Nanoseconds + right.Nanoseconds);

    /// <summary>
    /// Implements the operator + (unary).
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <returns>The same duration <see cref="Duration64"/> as provided.</returns>
    public static Duration64 operator +(Duration64 duration) => duration;

    /// <summary>
    /// Adds one duration to another. Friendly alternative to <c>operator+()</c>.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64"/> representing the sum of the given values.</returns>
    public static Duration64 Add(Duration64 left, Duration64 right) => left + right;

    /// <summary>
    /// Returns the result of adding another duration to this one, for a fluent alternative to <c>operator+()</c>.
    /// </summary>
    /// <param name="other">The duration to add</param>
    /// <returns>A new <see cref="Duration64" /> representing the result of the addition.</returns>
    [Pure]
    public Duration64 Plus(Duration64 other) => this + other;

    /// <summary>
    /// Implements the operator - (subtraction).
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64"/> representing the difference of the given values.</returns>
    public static Duration64 operator -(Duration64 left, Duration64 right) => new(left.Nanoseconds - right.Nanoseconds);

    /// <summary>
    /// Subtracts one duration from another. Friendly alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns>A new <see cref="Duration64"/> representing the difference of the given values.</returns>
    public static Duration64 Subtract(Duration64 left, Duration64 right) => left - right;

    /// <summary>
    /// Returns the result of subtracting another duration from this one, for a fluent alternative to <c>operator-()</c>.
    /// </summary>
    /// <param name="other">The duration to subtract</param>
    /// <returns>A new <see cref="Duration64" /> representing the result of the subtraction.</returns>
    [Pure]
    public Duration64 Minus(Duration64 other) => this - other;

    /// <summary>
    /// Implements the operator == (equality).
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
    public static bool operator ==(Duration64 left, Duration64 right) => left.Nanoseconds == right.Nanoseconds;

    /// <summary>
    /// Implements the operator != (inequality).
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
    public static bool operator !=(Duration64 left, Duration64 right) => !(left == right);

    /// <summary>
    /// Implements the operator &lt; (less than).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
    public static bool operator <(Duration64 left, Duration64 right) => left.Nanoseconds < right.Nanoseconds;

    /// <summary>
    /// Implements the operator &lt;= (less than or equal).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
    public static bool operator <=(Duration64 left, Duration64 right) => left.Nanoseconds <= right.Nanoseconds;

    /// <summary>
    /// Implements the operator &gt; (greater than).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
    public static bool operator >(Duration64 left, Duration64 right) => left.Nanoseconds > right.Nanoseconds;

    /// <summary>
    /// Implements the operator &gt;= (greater than or equal).
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <param name="left">The left hand side of the operator.</param>
    /// <param name="right">The right hand side of the operator.</param>
    /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
    public static bool operator >=(Duration64 left, Duration64 right) => left.Nanoseconds >= right.Nanoseconds;

    /// <summary>
    /// Implements the unary negation operator.
    /// </summary>
    /// <param name="duration">Duration64 to negate</param>
    /// <returns>The negative value of this duration</returns>
    /// <exception cref="OverflowException"><paramref name="duration"/> is equal to <see cref="MinValue"/>.</exception>
    public static Duration64 operator -(Duration64 duration) => new(-duration.Nanoseconds);

    /// <summary>
    /// Implements a friendly alternative to the unary negation operator.
    /// </summary>
    /// <param name="duration">Duration64 to negate</param>
    /// <returns>The negative value of this duration</returns>
    /// <exception cref="OverflowException"><paramref name="duration"/> is equal to <see cref="MinValue"/>.</exception>
    public static Duration64 Negate(Duration64 duration) => -duration;
    #endregion // Operators

    #region IComparable<Duration64> Members
    /// <summary>
    /// Implementation of <see cref="IComparable{Duration64}.CompareTo"/> to compare two durations.
    /// See the type documentation for a description of ordering semantics.
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
    public int CompareTo(Duration64 other) => Nanoseconds.CompareTo(other.Nanoseconds);

    /// <summary>
    /// Implementation of <see cref="IComparable.CompareTo"/> to compare two durations.
    /// See the type documentation for a description of ordering semantics.
    /// </summary>
    /// <remarks>
    /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Duration64"/>.</exception>
    /// <param name="obj">The object to compare this value with.</param>
    /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.HighPerformance.Duration64)"/> for general details.
    /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
    /// </returns>
    int IComparable.CompareTo(object? obj)
    {
        if (obj is null)
        {
            return 1;
        }
        Preconditions.CheckArgument(obj is Duration64, nameof(obj), "Object must be of type NodaTime.HighPerformance.Duration64.");
        return CompareTo((Duration64) obj);
    }
    #endregion

    #region IEquatable<Duration64> Members
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// See the type documentation for a description of equality semantics.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other"/> parameter;
    /// otherwise, false.
    /// </returns>
    public bool Equals(Duration64 other) => this == other;
    #endregion

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the same number of ticks as the
    /// given <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan value to convert</param>
    /// <returns>A new Duration64 with the same number of ticks as the given TimeSpan.</returns>
    public static Duration64 FromTimeSpan(TimeSpan timeSpan) => FromTicks(timeSpan.Ticks);

    /// <summary>
    /// Returns a <see cref="TimeSpan"/> that represents the same number of ticks as this
    /// <see cref="Duration64"/>.
    /// </summary>
    /// <remarks>
    /// If the number of nanoseconds in a duration is not a whole number of ticks, it is truncated towards zero.
    /// For example, durations in the range [-99ns, 99ns] would all count as 0 ticks.
    /// </remarks>
    /// <returns>A new TimeSpan with the same number of ticks as this Duration.</returns>
    [Pure]
    public TimeSpan ToTimeSpan() => new TimeSpan(Nanoseconds / NanosecondsPerTick);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of days, assuming a 'standard' 24-hour
    /// day.
    /// </summary>
    /// <param name="days">The number of days.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of days.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromDays(int days) => new(days * NanosecondsPerDay);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of hours.
    /// </summary>
    /// <param name="hours">The number of hours.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of hours.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromHours(int hours) => new(hours * NanosecondsPerHour);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of minutes.
    /// </summary>
    /// <param name="minutes">The number of minutes.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of minutes.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromMinutes(long minutes) => new(minutes * NanosecondsPerMinute);


    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of seconds.
    /// </summary>
    /// <param name="seconds">The number of seconds.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of seconds.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromSeconds(long seconds) => new(seconds * NanosecondsPerSecond);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of milliseconds.
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of milliseconds.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromMilliseconds(long milliseconds) => new(milliseconds * NanosecondsPerMillisecond);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of ticks.
    /// </summary>
    /// <param name="ticks">The number of ticks.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of ticks.</returns>
    /// <exception cref="OverflowException">The specified value cannot be represented as a <see cref="Duration64"/>.</exception>
    public static Duration64 FromTicks(long ticks) => new(ticks * NanosecondsPerTick);

    /// <summary>
    /// Returns a <see cref="Duration64"/> that represents the given number of nanoseconds.
    /// </summary>
    /// <param name="nanoseconds">The number of nanoseconds.</param>
    /// <returns>A <see cref="Duration64"/> representing the given number of nanoseconds.</returns>
    public static Duration64 FromNanoseconds(long nanoseconds) => new(nanoseconds);

    /// <summary>
    /// Returns the larger duration of the given two.
    /// </summary>
    /// <remarks>
    /// A "larger" duration is one that advances time by more than a "smaller" one. This means
    /// that a positive duration is always larger than a negative one, for example. (This is the same
    /// comparison used by the binary comparison operators.)
    /// </remarks>
    /// <param name="x">The first duration to compare.</param>
    /// <param name="y">The second duration to compare.</param>
    /// <returns>The larger duration of <paramref name="x"/> or <paramref name="y"/>.</returns>
    public static Duration64 Max(Duration64 x, Duration64 y) => x > y ? x : y;

    /// <summary>
    /// Returns the smaller duration of the given two.
    /// </summary>
    /// <remarks>
    /// A "larger" duration is one that advances time by more than a "smaller" one. This means
    /// that a positive duration is always larger than a negative one, for example. (This is the same
    /// comparison used by the binary comparison operators.)
    /// </remarks>
    /// <param name="x">The first duration to compare.</param>
    /// <param name="y">The second duration to compare.</param>
    /// <returns>The smaller duration of <paramref name="x"/> or <paramref name="y"/>.</returns>
    public static Duration64 Min(Duration64 x, Duration64 y) => x < y ? x : y;

    /// <summary>
    /// Converts this value to a <see cref="Duration"/> representing the same number of nanoseconds.
    /// This operation always succeeds and loses no information.
    /// </summary>
    /// <returns>A <see cref="Duration"/> representing the same number of nanoseconds as this one.</returns>
    [Pure]
    public Duration ToDuration() => Duration.FromNanoseconds(Nanoseconds);

    /// <summary>
    /// Creates a <see cref="Duration64"/> value representing the same number of nanoseconds as the specified
    /// <see cref="Duration"/>. When this succeeds, this conversion loses no information.
    /// If the specified value is outside the range of <see cref="Duration64"/>, this method will fail with an
    /// <see cref="OverflowException"/>.
    /// </summary>
    /// <param name="duration">The value to convert to a <see cref="Duration64"/>.</param>
    /// <returns>A <see cref="Duration64"/> value equivalent to <paramref name="duration"/>.</returns>
    /// <exception cref="OverflowException"><paramref name="duration"/> has a value outside
    /// the range of <see cref="Duration64"/>.</exception>
    public static Duration64 FromDuration(Duration duration) => new(duration.ToInt64Nanoseconds());
}
