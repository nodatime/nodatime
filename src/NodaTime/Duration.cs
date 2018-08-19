// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using static NodaTime.NodaConstants;

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Text;
using NodaTime.Utility;
using System.Numerics;

namespace NodaTime
{
    // Implementation note:
    // Although we use BigInteger in the public API as it's a better fit for "this could be a large integer"
    // we use decimal for computations where we have to. It's more efficient in CPU (and space) than BigInteger,
    // and still has plenty of range/precision. The FromNanoseconds(BigInteger) and ToBigIntegerNanoseconds() methods
    // should not be called within Noda Time, except for in FromNanoseconds(double), as the double to decimal conversion
    // loses information whereas double to BigInteger doesn't.

    /// <summary>
    /// Represents a fixed (and calendar-independent) length of time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A duration is a length of time defined by an integral number of nanoseconds.
    /// Although durations are usually used with a positive number of nanoseconds, negative durations are valid, and may occur
    /// naturally when e.g. subtracting an earlier <see cref="Instant"/> from a later one.
    /// </para>
    /// <para>
    /// A duration represents a fixed length of elapsed time along the time line that occupies the same amount of
    /// time regardless of when it is applied. In contrast, <see cref="Period"/> represents a period of time in
    /// calendrical terms (years, months, days, and so on) that may vary in elapsed time when applied.
    /// </para>
    /// <para>
    /// In general, use <see cref="Duration"/> to represent durations applied to global types like <see cref="Instant"/>
    /// and <see cref="ZonedDateTime"/>; use <c>Period</c> to represent a period applied to local types like
    /// <see cref="LocalDateTime"/>.
    /// </para>
    /// <para>
    /// The range of valid values of a <c>Duration</c> is greater than the span of time supported by Noda Time - so for
    /// example, subtracting one <see cref="Instant"/> from another will always give a valid <c>Duration</c>. The range
    /// is also greater than that of <see cref="TimeSpan"/> (<see cref="long.MinValue" /> ticks to <see cref="long.MaxValue"/> ticks).
    /// See the user guide for more details of the exact range, but it is not expected that this will ever be exceeded in normal code.
    /// </para>
    /// <para>
    /// Various operations accept or return a <see cref="Double"/>, in-keeping with durations often being natural lengths
    /// of time which are imprecisely measured anyway. The implementation of these operations should never result in a not-a-number
    /// or infinite value, nor do any operations accept not-a-number or infinite values. Additionally, operations involving
    /// <c>Double</c> have initially been implemented fairly naïvely; it's possible that future releases will improve the accuracy
    /// or performance (or both) of various operations.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !NETSTANDARD
    [Serializable]
#endif
    public struct Duration : IEquatable<Duration>, IComparable<Duration>, IComparable, IXmlSerializable, IFormattable
#if !NETSTANDARD
        , ISerializable
#endif
    {
        // This is one more bit than we really need, but it allows Instant.BeforeMinValue and Instant.AfterMaxValue
        // to be easily 
        internal const int MaxDays = (1 << 24) - 1;
        internal const int MinDays = ~MaxDays;
        internal static readonly BigInteger MinNanoseconds = (BigInteger)MinDays * NanosecondsPerDay;
        internal static readonly BigInteger MaxNanoseconds = (MaxDays + BigInteger.One) * NanosecondsPerDay - BigInteger.One;
        internal static readonly decimal MinDecimalNanoseconds = (decimal) MinNanoseconds;
        internal static readonly decimal MaxDecimalNanoseconds = (decimal) MaxNanoseconds;
        private static readonly double MinDoubleNanoseconds = (double) MinNanoseconds;
        private static readonly double MaxDoubleNanoseconds = (double) MaxNanoseconds;

        // The -1 here is to allow for the addition of nearly a whole day in the nanoOfDay field.
        private const long MaxDaysForLongNanos = (int) (long.MaxValue / NanosecondsPerDay) - 1;
        private const long MinDaysForLongNanos = (int) (long.MinValue / NanosecondsPerDay);

        #region Readonly static properties

        // TODO(optimization): Evaluate performance of this implementation vs readonly automatically implemented properties.
        // Consider adding a private constructor which performs no validation at all.

        /// <summary>
        /// Gets a zero <see cref="Duration"/> of 0 nanoseconds.
        /// </summary>
        /// <value>The zero <see cref="Duration"/> value.</value>
        public static Duration Zero => new Duration(0, 0L);

        /// <summary>
        /// Gets a <see cref="Duration"/> value equal to 1 nanosecond; the smallest amount by which an instant can vary.
        /// </summary>
        /// <value>A duration representing 1 nanosecond.</value>
        public static Duration Epsilon => new Duration(0, 1L);

        /// <summary>
        /// Gets the maximum value supported by <see cref="Duration"/>.
        /// </summary>
        public static Duration MaxValue => new Duration(MaxDays, NanosecondsPerDay - 1);

        /// <summary>
        /// Gets the minimum (largest negative) value supported by <see cref="Duration"/>.
        /// </summary>
        public static Duration MinValue => new Duration(MinDays, 0);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 standard week (7 days).
        /// </summary>
        /// <remarks>
        /// The value of this property is 604,800,000,000,000 nanoseconds.
        /// </remarks>
        internal static Duration OneWeek => new Duration(7, 0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 day.
        /// </summary>
        /// <remarks>
        /// The value of this property is 86.4 trillion nanoseconds; that is, 86,400,000,000,000 nanoseconds.
        /// </remarks>
        internal static Duration OneDay => new Duration(1, 0L);
        #endregion

        // This is effectively a 25 bit value. (It can't be 24 bits, or we can't represent every TimeSpan value.)
        private readonly int days;
        // Always non-negative; a negative duration will have a negative number of days, but may have a
        // positive nanoOfDay. (A duration of -1ns will have a days value of -1 and a nanoOfDay of
        // NanosecondsPerDay - 1, for example.)
        private readonly long nanoOfDay;

        // Implementation note: I've tried making this internal and calling it directly from Instant.FromUnixTimeSeconds etc?
        // That reduces the number of range checks, but doesn't seem to affect the performance in a significant way.
        // Leaving it as it is leaves a cleaner API - this constructor doesn't feel like something that *should*
        // be exposed to other classes.

        /// <summary>
        /// Constructs an instance from a given number of units. This was previously a method (FromUnits) but making it a
        /// constructor avoids calling the other constructor which validates its "days" parameter.
        /// Note that we could compute various parameters from nanosPerUnit, but we know them as compile-time constants, so
        /// there's no point in recomputing them on each call.
        /// </summary>
        /// <param name="units">Number of units</param>
        /// <param name="paramName">Name of the parameter, e.g. "hours"</param>
        /// <param name="minValue">Minimum number of units for a valid duration.</param>
        /// <param name="maxValue">Maximum number of units for a valid duration.</param>
        /// <param name="unitsPerDay">Number of units in a day.</param>
        /// <param name="nanosPerUnit">Number of nanoseconds per unit.</param>
        private Duration (long units, string paramName, long minValue, long maxValue, long unitsPerDay, long nanosPerUnit)
        {
            Preconditions.CheckArgumentRange(paramName, units, minValue, maxValue);
            unchecked
            {
                // Note: DivRem appears to make this (significantly) slower, despite helping in Period.
                days = (int) (units / unitsPerDay);
                long unitOfDay = units - (unitsPerDay * days);
                if (unitOfDay < 0)
                {
                    days--;
                    unitOfDay += unitsPerDay;
                }
                nanoOfDay = unitOfDay * nanosPerUnit;
            }
        }

        internal Duration(int days, [Trusted] long nanoOfDay)
        {
            // Heavily used: avoid the method call unless it's going to throw.
            // (Benchmarking Duration.FromDays, unconditionally calling CheckArgumentRange
            // raises the call time from ~2ns to ~15ns.)
            if (days < MinDays || days > MaxDays)
            {
                Preconditions.CheckArgumentRange(nameof(days), days, MinDays, MaxDays);
            }
            Preconditions.DebugCheckArgumentRange(nameof(nanoOfDay), nanoOfDay, 0, NanosecondsPerDay - 1);
            this.days = days;
            this.nanoOfDay = nanoOfDay;
        }

        /// <summary>
        /// Days portion of this duration. The <see cref="NanosecondOfFloorDay" /> is added to this
        /// value, so this effectively Math.Floor(TotalDays).
        /// </summary>
        internal int FloorDays => days;

        /// <summary>
        /// Nanosecond within the "floor day". This is *always* non-negative, even for
        /// negative durations.
        /// </summary>
        internal long NanosecondOfFloorDay => nanoOfDay;

        /// <summary>
        /// Gets the whole number of standard (24 hour) days within this duration. This is truncated towards zero;
        /// for example, "-1.75 days" and "1.75 days" would have results of -1 and 1 respectively.
        /// </summary>
        /// <value>The whole number of days in the duration</value>
        public int Days => days >= 0 || nanoOfDay == 0 ? days : days + 1;

        /// <summary>
        /// Gets the number of nanoseconds within the day of this duration. For negative durations, this
        /// will be negative (or zero).
        /// </summary>
        /// <value>The number of nanoseconds within the day of this duration.</value>
        public long NanosecondOfDay => days >= 0 ? nanoOfDay
                    : nanoOfDay == 0 ? 0L
                    : nanoOfDay - NanosecondsPerDay;

        /// <summary>
        /// The hour component of this duration, in the range [-23, 23], truncated towards zero.
        /// </summary>
        /// <value>The hour component of the duration, within the day.</value>
        public int Hours => unchecked((int) (NanosecondOfDay / NanosecondsPerHour));

        /// <summary>
        /// The minute component of this duration, in the range [-59, 59], truncated towards zero.
        /// </summary>
        /// <value>The minute component of the duration, within the hour.</value>
        public int Minutes => unchecked((int) ((NanosecondOfDay / NanosecondsPerMinute) % MinutesPerHour));

        /// <summary>
        /// Gets the second component of this duration, in the range [-59, 59], truncated towards zero.
        /// </summary>
        /// <value>The second component of the duration, within the minute.</value>
        public int Seconds => unchecked((int) ((NanosecondOfDay / NanosecondsPerSecond) % SecondsPerMinute));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in milliseconds, in the range [-999, 999] and truncated towards zero.
        /// </summary>
        /// <value>The subsecond component of the duration, in milliseconds.</value>
        public int Milliseconds => unchecked((int) ((NanosecondOfDay / NanosecondsPerMillisecond) % MillisecondsPerSecond));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in ticks, in the range [-9999999, 9999999] and truncated towards zero.
        /// </summary>
        /// <value>The subsecond component of the duration, in ticks.</value>
        public int SubsecondTicks => unchecked((int) ((NanosecondOfDay / NanosecondsPerTick) % TicksPerSecond));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in nanoseconds, in the range [-999999999, 999999999].
        /// </summary>
        /// <value>The subsecond component of the duration, in nanoseconds.</value>
        public int SubsecondNanoseconds => unchecked((int) (NanosecondOfDay % NanosecondsPerSecond));

        /// <summary>
        /// Gets the total number of ticks in the duration as a 64-bit integer, truncating towards zero where necessary.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Within the constraints specified below, this property is intended to be equivalent to
        /// <see cref="TimeSpan.Ticks"/>.
        /// </para>
        /// <para>
        /// If the number of nanoseconds in a duration is not a whole number of ticks, it is truncated towards zero.
        /// For example, durations in the range [-99ns, 99ns] would all count as 0 ticks.
        /// </para>
        /// <para>Although this method can overflow, it will only do so in very exceptional cases, with durations
        /// with a magnitude of more than 29000 Gregorian years or so.</para>
        /// </remarks>
        /// <exception cref="OverflowException">The number of ticks cannot be represented a signed 64-bit integer.</exception>
        /// <value>The total number of ticks in the duration.</value>
        /// <seealso cref="TotalTicks"/>
        public long BclCompatibleTicks
        {
            get
            {
                long ticks = TickArithmetic.DaysAndTickOfDayToTicks(days, nanoOfDay / NanosecondsPerTick);
                if (days < 0 && nanoOfDay % NanosecondsPerTick != 0)
                {
                    ticks++;
                }
                return ticks;
            }
        }

        // TODO(optimization): Reimplement all of these using TotalNanoseconds?

        /// <summary>
        /// Gets the total number of days in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalDays"/>.
        /// It represents the complete duration in days, rather than only the whole number of
        /// days. For example, for a duration of 36 hours, this property would return 1.5.
        /// </remarks>
        /// <value>The total number of days in this duration.</value>
        public double TotalDays => days + nanoOfDay / (double) NanosecondsPerDay;

        /// <summary>
        /// Gets the total number of hours in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>
        /// This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalHours"/>.
        /// Unlike <see cref="Hours"/>, it represents the complete duration in hours rather than the
        /// whole number of hours as part of the day. So for a duration
        /// of 1 day, 2 hours and 30 minutes, the <c>Hours</c> property will return 2, but <c>TotalHours</c>
        /// will return 26.5.
        /// </remarks>
        /// <value>The total number of hours in this duration.</value>
        public double TotalHours => days * 24.0 + nanoOfDay / (double) NanosecondsPerHour;

        /// <summary>
        /// Gets the total number of minutes in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalMinutes"/>.
        /// Unlike <see cref="Minutes"/>, it represents the complete duration in minutes rather than
        /// the whole number of minutes within the hour. So for a duration
        /// of 2 hours, 30 minutes and 45 seconds, the <c>Minutes</c> property will return 30, but <c>TotalMinutes</c>
        /// will return 150.75.
        /// </remarks>
        /// <value>The total number of minutes in this duration.</value>
        public double TotalMinutes => days * (double) MinutesPerDay + nanoOfDay / (double) NanosecondsPerMinute;

        /// <summary>
        /// Gets the total number of seconds in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>
        /// This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalSeconds"/>.
        /// Unlike <see cref="Seconds"/>, it represents the complete duration in seconds rather than
        /// the whole number of seconds within the minute. So for a duration
        /// of 10 minutes, 20 seconds and 250 milliseconds, the <c>Seconds</c> property will return 20, but <c>TotalSeconds</c>
        /// will return 620.25.
        /// </remarks>
        /// <value>The total number of seconds in this duration.</value>
        public double TotalSeconds => days * (double) SecondsPerDay + nanoOfDay / (double) NanosecondsPerSecond;

        /// <summary>
        /// Gets the total number of milliseconds in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalMilliseconds"/>.
        /// Unlike <see cref="Milliseconds"/>, it represents the complete duration in seconds rather than
        /// the whole number of seconds within the minute. So for a duration
        /// of 10 minutes, 20 seconds and 250 milliseconds, the <c>Seconds</c> property will return 20, but <c>TotalSeconds</c>
        /// will return 620.25.
        /// </remarks>
        /// <value>The total number of milliseconds in this duration.</value>
        public double TotalMilliseconds => days * (double) MillisecondsPerDay + nanoOfDay / (double) NanosecondsPerMillisecond;

        /// <summary>
        /// Gets the total number of ticks in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>This property is the <c>Duration</c> equivalent of <see cref="TimeSpan.Ticks"/>,
        /// except represented as double-precision floating point number instead of a 64-bit integer. This
        /// is because <see cref="Duration"/> has a precision of nanoseconds, and also because the range
        /// of 64-bit integers doesn't cover the number of possible ticks in a <see cref="Duration"/>. (The
        /// latter is only an issue in durations outside the range of <see cref="TimeSpan"/> - in other words,
        /// with magnitudes of over 29,000 years.)
        /// </remarks>
        /// <value>The total number of ticks in this duration.</value>
        /// <seealso cref="BclCompatibleTicks"/>
        public double TotalTicks => days * (double) TicksPerDay + nanoOfDay / (double) NanosecondsPerTick;

        /// <summary>
        /// Gets the total number of nanoseconds in this duration, as a <see cref="Double"/>.
        /// </summary>
        /// <remarks>The result is always an integer, but may not be precise due to the limitations
        /// of the <c>Double</c> type. In other works, <c>Duration.FromNanoseconds(duration.TotalNanoseconds)</c>
        /// is not guaranteed to round-trip. To guarantee precision and round-tripping,
        /// use <see cref="ToBigIntegerNanoseconds" /> and <see cref="FromNanoseconds(BigInteger)"/>.
        /// </remarks>
        /// <returns>This duration as a number of nanoseconds, represented as a <c>Double</c>.</returns>
        public double TotalNanoseconds => ((double) days) * NanosecondsPerDay + nanoOfDay;

        /// <summary>
        /// Adds a "small" number of nanoseconds to this duration: it is trusted to be less or equal to than 24 hours
        /// in magnitude.
        /// </summary>
        [Pure]
        internal Duration PlusSmallNanoseconds([Trusted] long smallNanos)
        {
            unchecked
            {
                Preconditions.DebugCheckArgumentRange(nameof(smallNanos), smallNanos, -NanosecondsPerDay, NanosecondsPerDay);
                int newDays = days;
                long newNanos = nanoOfDay + smallNanos;
                if (newNanos >= NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NanosecondsPerDay;
                }
                return new Duration(newDays, newNanos);
            }
        }

        /// <summary>
        /// Subtracts a "small" number of nanoseconds from this duration: it is trusted to be less than 24 hours
        /// in magnitude.
        /// </summary>
        [Pure]
        internal Duration MinusSmallNanoseconds(long smallNanos)
        {
            unchecked
            {
                Preconditions.DebugCheckArgumentRange(nameof(smallNanos), smallNanos, -NanosecondsPerDay, NanosecondsPerDay);
                int newDays = days;
                long newNanos = nanoOfDay - smallNanos;
                if (newNanos >= NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NanosecondsPerDay;
                }
                return new Duration(newDays, newNanos);
            }
        }

        #region Object overrides
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => obj is Duration && Equals((Duration)obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => days ^ nanoOfDay.GetHashCode();
        #endregion

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("o"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString() => DurationPattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("o").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider) =>
            DurationPattern.BclSupport.Format(this, patternText, formatProvider);
        #endregion Formatting

        #region Operators
        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the sum of the given values.</returns>
        public static Duration operator +(Duration left, Duration right)
        {
            unchecked
            {
                int newDays = left.days + right.days;
                long newNanos = left.nanoOfDay + right.nanoOfDay;
                if (newNanos >= NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NanosecondsPerDay;
                }
                // nanoOfDay is always non-negative (and much less than half of long.MaxValue), so adding two 
                // of them together will never produce a negative result.
                return new Duration(newDays, newNanos);
            }
        }

        /// <summary>
        /// Adds one duration to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the sum of the given values.</returns>
        public static Duration Add(Duration left, Duration right) => left + right;

        /// <summary>
        /// Returns the result of adding another duration to this one, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="other">The duration to add</param>
        /// <returns>A new <see cref="Duration" /> representing the result of the addition.</returns>
        [Pure]
        public Duration Plus(Duration other) => this + other;

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the difference of the given values.</returns>
        public static Duration operator -(Duration left, Duration right)
        {
            unchecked
            {
                int newDays = left.days - right.days;
                long newNanos = left.nanoOfDay - right.nanoOfDay;
                if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NanosecondsPerDay;
                }
                return new Duration(newDays, newNanos);
            }
        }

        /// <summary>
        /// Subtracts one duration from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the difference of the given values.</returns>
        public static Duration Subtract(Duration left, Duration right) => left - right;

        /// <summary>
        /// Returns the result of subtracting another duration from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The duration to subtract</param>
        /// <returns>A new <see cref="Duration" /> representing the result of the subtraction.</returns>
        [Pure]
        public Duration Minus(Duration other) => this - other;

        /// <summary>
        /// Implements the operator / (division) to divide a duration by an <see cref="Int64"/>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator /(Duration left, long right)
        {
            int days = left.days;
            // Simplest scenario to handle
            if (days == 0 && right > 0)
            {
                return new Duration(0, left.nanoOfDay / right);
            }
            // Now for the ~[-250, +250] year range, where we can do it all as a long.
            if (days >= MinDaysForLongNanos && days <= MaxDaysForLongNanos)
            {
                long nanos = left.ToInt64Nanoseconds() / right;
                return FromNanoseconds(nanos);
            }
            // Fall back to decimal arithmetic.
            decimal x = left.ToDecimalNanoseconds();
            return FromNanoseconds(x / right);
        }

        /// <summary>
        /// Implements the operator / (division) to divide a duration by a <see cref="Double"/>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator /(Duration left, double right)
        {
            // Exclude infinity and NaN
            Preconditions.CheckArgumentRange(nameof(right), right, double.MinValue, double.MaxValue);
            if (right == 0d)
            {
                throw new DivideByZeroException("Attempt to divide a duration by zero.");
            }
            return FromNanoseconds(left.TotalNanoseconds / right);
        }

        /// <summary>
        /// Implements the operator / (division) to divide one duration by another.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>The <see cref="Double"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static double operator /(Duration left, Duration right)
        {
            double rightNanos = right.TotalNanoseconds;
            if (rightNanos == 0d)
            {
                throw new DivideByZeroException("Attempt to divide by a zero duration.");
            }
            return left.TotalNanoseconds / rightNanos;
        }

        /// <summary>
        /// Divides a duration by an <see cref="Int64"/>. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration Divide(Duration left, long right) => left / right;

        /// <summary>
        /// Divides a duration by a <see cref="Double"/>. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration Divide(Duration left, double right) => left / right;

        /// <summary>
        /// Divides one duration by another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>The <see cref="Double"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static double Divide(Duration left, Duration right) => left / right;

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of multiplying <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator *(Duration left, double right)
        {
            // Exclude infinity and NaN
            Preconditions.CheckArgumentRange(nameof(right), right, double.MinValue, double.MaxValue);
            return FromNanoseconds(left.TotalNanoseconds * right);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of multiplying <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator *(Duration left, long right)
        {
            unchecked
            {
                // Optimize very simple cases that require no arithmetic.
                if (right == 0 || left == Zero)
                {
                    return Zero;
                }
                if (right == 1)
                {
                    return left;
                }
                // Attempt to find a sweet spot... rather than detecting whether arithmetic
                // will overflow accurately (which is painful), we just check whether both
                // values are within a common range - durations of more than +/-100 days
                // are rare, and so is multiplication by huge numbers. (The range we get is
                // roughly +/- 1000.) If this changes, tests should change too.
                // Note that this *isn't* a good sweet spot for Integer.FromUnixTime*... so
                // FromSeconds etc are optimized separately.
                const int DaysToOptimize = 100;
                // Almost every real use case will fit in here, I suspect...
                if (left.days >= -DaysToOptimize &&
                    left.days < DaysToOptimize && 
                    right > long.MinValue / (NanosecondsPerDay * DaysToOptimize) &&
                    right < long.MaxValue / (NanosecondsPerDay * DaysToOptimize))
                {
                    long nanos = left.ToInt64NanosecondsUnchecked();
                    return FromNanoseconds(nanos * right);
                }
            }
            // Fall back to decimal arithmetic
            return FromNanoseconds(left.ToDecimalNanoseconds() * right);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of multiplying <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator *(long left, Duration right) => right * left;

        /// <summary>
        /// Multiplies a duration by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the product of the given values.</returns>
        public static Duration Multiply(Duration left, long right) => left * right;

        /// <summary>
        /// Multiplies a duration by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the product of the given values.</returns>
        public static Duration Multiply(Duration left, double right) => left * right;

        /// <summary>
        /// Multiplies a duration by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the product of the given values.</returns>
        public static Duration Multiply(long left, Duration right) => left * right;

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Duration left, Duration right) =>
            left.days == right.days && left.nanoOfDay == right.nanoOfDay;

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Duration left, Duration right) => !(left == right);

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Duration left, Duration right) =>
            left.days < right.days || (left.days == right.days && left.nanoOfDay < right.nanoOfDay);

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Duration left, Duration right) =>
            left.days < right.days || (left.days == right.days && left.nanoOfDay <= right.nanoOfDay);

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Duration left, Duration right) =>
            left.days > right.days || (left.days == right.days && left.nanoOfDay > right.nanoOfDay);

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Duration left, Duration right) =>
            left.days > right.days || (left.days == right.days && left.nanoOfDay >= right.nanoOfDay);

        /// <summary>
        /// Implements the unary negation operator.
        /// </summary>
        /// <param name="duration">Duration to negate</param>
        /// <returns>The negative value of this duration</returns>
        public static Duration operator -(Duration duration)
        {
            int oldDays = duration.days;
            long oldNanoOfDay = duration.nanoOfDay;
            if (oldNanoOfDay == 0)
            {
                return new Duration(-oldDays, 0);
            }
            long newNanoOfDay = NanosecondsPerDay - oldNanoOfDay;
            return new Duration(-oldDays - 1, newNanoOfDay);
        }

        /// <summary>
        /// Implements a friendly alternative to the unary negation operator.
        /// </summary>
        /// <param name="duration">Duration to negate</param>
        /// <returns>The negative value of this duration</returns>
        public static Duration Negate(Duration duration) => -duration;
        #endregion // Operators

        #region IComparable<Duration> Members
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
        public int CompareTo(Duration other)
        {
            int dayComparison = days.CompareTo(other.days);
            return dayComparison != 0 ? dayComparison : nanoOfDay.CompareTo(other.nanoOfDay);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two offsets.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Duration"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.Duration)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Preconditions.CheckArgument(obj is Duration, nameof(obj), "Object must be of type NodaTime.Duration.");
            return CompareTo((Duration)obj);
        }
        #endregion

        #region IEquatable<Duration> Members
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Duration other) => this == other;
        #endregion

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of days, assuming a 'standard' 24-hour
        /// day.
        /// </summary>
        /// <param name="days">The number of days.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of days.</returns>
        public static Duration FromDays(int days) => new Duration(days, 0L);

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of days, assuming a 'standard' 24-hour
        /// day.
        /// </summary>
        /// <param name="days">The number of days.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of days.</returns>
        public static Duration FromDays(double days)
        {
            Preconditions.CheckArgumentRange(
                nameof(days),
                days,
                MinDays,
                MaxDays);
            return FromNanoseconds(days * NanosecondsPerDay);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of hours.</returns>
        public static Duration FromHours(int hours) =>
            new Duration(hours, nameof(hours), (long) MinDays * HoursPerDay, ((MaxDays + 1L) * HoursPerDay) - 1, HoursPerDay, NanosecondsPerHour);

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of hours.</returns>
        public static Duration FromHours(double hours)
        {
            Preconditions.CheckArgumentRange(
                nameof(hours),
                hours,
                (long) MinDays * HoursPerDay,
                (MaxDays + 1L) * HoursPerDay - 1);
            return FromNanoseconds(hours * NanosecondsPerHour);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of minutes.</returns>
        public static Duration FromMinutes(long minutes) =>
            new Duration(minutes, nameof(minutes), (long)MinDays * MinutesPerDay, ((MaxDays + 1L) * MinutesPerDay) - 1, MinutesPerDay, NanosecondsPerMinute);

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of minutes.</returns>
        public static Duration FromMinutes(double minutes)
        {
            Preconditions.CheckArgumentRange(
                nameof(minutes),
                minutes,
                (long) MinDays * MinutesPerDay,
                (MaxDays + 1L) * MinutesPerDay - 1);
            return FromNanoseconds(minutes * NanosecondsPerMinute);
        }
        
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of seconds.</returns>
        public static Duration FromSeconds(long seconds) =>
            new Duration(seconds, nameof(seconds), (long)MinDays * SecondsPerDay, ((MaxDays + 1L) * SecondsPerDay) - 1, SecondsPerDay, NanosecondsPerSecond);

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of seconds.</returns>
        public static Duration FromSeconds(double seconds)
        {
            Preconditions.CheckArgumentRange(
                nameof(seconds),
                seconds,
                (long) MinDays * SecondsPerDay,
                (MaxDays + 1L) * SecondsPerDay - 1);
            return FromNanoseconds(seconds * NanosecondsPerSecond);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of milliseconds.</returns>
        public static Duration FromMilliseconds(long milliseconds) =>
            new Duration(milliseconds, nameof(milliseconds), (long) MinDays * MillisecondsPerDay, ((MaxDays + 1L) * MillisecondsPerDay) - 1, MillisecondsPerDay, NanosecondsPerMillisecond);

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of milliseconds.</returns>
        public static Duration FromMilliseconds(double milliseconds)
        {
            Preconditions.CheckArgumentRange(
                nameof(milliseconds),
                milliseconds,
                (long) MinDays * MillisecondsPerDay,
                (MaxDays + 1L) * MillisecondsPerDay - 1);
            return FromNanoseconds(milliseconds * NanosecondsPerMillisecond);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of ticks.</returns>
        public static Duration FromTicks(long ticks)
        {
            // No precondition here, as we cover a wider range than Int64 ticks can handle...
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out long tickOfDay);
            return new Duration(days, tickOfDay * NanosecondsPerTick);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of ticks.</returns>
        public static Duration FromTicks(double ticks)
        {
            Preconditions.CheckArgumentRange(
                nameof(ticks),
                ticks,
                MinDays * (double) TicksPerDay,
                (MaxDays + 1d) * TicksPerDay - 1);
            return FromNanoseconds(ticks * NanosecondsPerTick);
        }
        
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of nanoseconds.</returns>
        public static Duration FromNanoseconds(long nanoseconds)
        {
            // TODO(optimization): Try DivRem
            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NanosecondsPerDay) - 1;

            long nanoOfDay = nanoseconds >= long.MinValue + NanosecondsPerDay
                ? nanoseconds - days * NanosecondsPerDay
                : nanoseconds - (days + 1) * NanosecondsPerDay + NanosecondsPerDay; // Avoid multiplication overflow
            return new Duration(days, nanoOfDay);
        }

        /// <summary>
        /// Converts a number of nanoseconds expressed as a <see cref="Double"/> into a duration. Any fractional
        /// parts of the value are truncated towards zero.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to represent.</param>
        /// <returns>A duration with the given number of nanoseconds.</returns>
        public static Duration FromNanoseconds(double nanoseconds)
        {
            Preconditions.CheckArgumentRange(nameof(nanoseconds), nanoseconds, MinDoubleNanoseconds, MaxDoubleNanoseconds);
            return nanoseconds >= long.MinValue && nanoseconds <= long.MaxValue
                ? FromNanoseconds((long) nanoseconds) : FromNanoseconds((BigInteger) nanoseconds);
        }

        // NOTE: Do not use from internal code - use decimal instead - *unless* you're converting from double.
        /// <summary>
        /// Converts a number of nanoseconds expressed as a <see cref="BigInteger"/> into a duration.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to represent.</param>
        /// <returns>A duration with the given number of nanoseconds.</returns>
        public static Duration FromNanoseconds(BigInteger nanoseconds)
        {
            if (nanoseconds < MinNanoseconds || nanoseconds > MaxNanoseconds)
            {
                throw new ArgumentOutOfRangeException(nameof(nanoseconds), $"Value should be in range [{MinNanoseconds}-{MaxNanoseconds}]");
            }

            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NanosecondsPerDay) - 1;

            long nanoOfDay = (long) (nanoseconds - ((BigInteger) days) * NanosecondsPerDay);
            return new Duration(days, nanoOfDay);
        }

        internal static Duration FromNanoseconds(decimal nanoseconds)
        {
            if (nanoseconds < MinDecimalNanoseconds || nanoseconds > MaxDecimalNanoseconds)
            {
                // Note: use the BigInteger value rather than decimal to avoid decimal points in the message. They're the same values.
                throw new ArgumentOutOfRangeException(nameof(nanoseconds), $"Value should be in range [{MinNanoseconds}-{MaxNanoseconds}]");
            }

            int days = nanoseconds >= 0
                ? (int)(nanoseconds / NanosecondsPerDay)
                : (int)((nanoseconds + 1) / NanosecondsPerDay) - 1;

            long nanoOfDay = (long)(nanoseconds - ((decimal)days) * NanosecondsPerDay);
            return new Duration(days, nanoOfDay);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the same number of ticks as the
        /// given <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="timeSpan">The TimeSpan value to convert</param>
        /// <returns>A new Duration with the same number of ticks as the given TimeSpan.</returns>
        public static Duration FromTimeSpan(TimeSpan timeSpan)
        {
            return FromTicks(timeSpan.Ticks);
        }

        /// <summary>
        /// Returns a <see cref="TimeSpan"/> that represents the same number of ticks as this
        /// <see cref="Duration"/>.
        /// </summary>
        /// <remarks>
        /// If the number of nanoseconds in a duration is not a whole number of ticks, it is truncated towards zero.
        /// For example, durations in the range [-99ns, 99ns] would all count as 0 ticks.
        /// </remarks>
        /// <exception cref="OverflowException">The number of ticks cannot be represented a signed 64-bit integer.</exception>
        /// <returns>A new TimeSpan with the same number of ticks as this Duration.</returns>
        [Pure]
        public TimeSpan ToTimeSpan() => new TimeSpan(BclCompatibleTicks);

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = DurationPattern.Roundtrip;
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteString(DurationPattern.Roundtrip.Format(this));
        }
        #endregion

        /// <summary>
        /// Conversion to an <see cref="Int64"/> number of nanoseconds. This will fail if the number of nanoseconds is
        /// out of the range of <c>Int64</c>, which is approximately 292 years (positive or negative).
        /// </summary>
        /// <exception cref="System.OverflowException">The number of nanoseconds is outside the representable range.</exception>
        /// <returns>This duration as a number of nanoseconds, represented as an <c>Int64</c>.</returns>
        [Pure]
        public long ToInt64Nanoseconds() =>
            // Fast path (common case)
            IsInt64Representable ? ToInt64NanosecondsUnchecked()
            // Need to be careful to avoid overflow in a case where it's actually representable
            : days < 0 ? (days + 1) * NanosecondsPerDay + nanoOfDay - NanosecondsPerDay
            // Either the multiplication or the addition could overflow, so we do it in a checked context.
            : days * NanosecondsPerDay + nanoOfDay;
        
        /// <summary>
        /// A quick check to tell whether this duration is within the range of an Int64 value of nanoseconds
        /// (roughly +/- 292 years). This is conservative as it only checks the days field -
        /// there may be values just within Int64 range for which this property returns true, but they're
        /// unlikely to come up... this property should *only* be used for optimization purposes.
        /// </summary>
        internal bool IsInt64Representable => days >= MinDaysForLongNanos && days <= MaxDaysForLongNanos;

        /// <summary>
        /// Performs an unchecked conversion from this duration to an Int64 value of nanoseconds.
        /// This should only be called if IsInt64Representable returns true.
        /// </summary>
        [Pure]
        private long ToInt64NanosecondsUnchecked() => unchecked(days * NanosecondsPerDay + nanoOfDay);

        // NOTE: Do not use from internal code. Use decimal instead. TODO: Add an attribute, and use a Roslyn analyzer to validate.

        /// <summary>
        /// Conversion to a <see cref="BigInteger"/> number of nanoseconds, as a convenient built-in numeric
        /// type which can always represent values in the range we need.
        /// </summary>
        /// <returns>This duration as a number of nanoseconds, represented as a <c>BigInteger</c>.</returns>
        [Pure]
        public BigInteger ToBigIntegerNanoseconds() => IsInt64Representable ? ToInt64NanosecondsUnchecked() : ((BigInteger) days) * NanosecondsPerDay + nanoOfDay;

        [Pure]
        internal decimal ToDecimalNanoseconds() => IsInt64Representable ? ToInt64NanosecondsUnchecked() : ((decimal)days) * NanosecondsPerDay + nanoOfDay;

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
        public static Duration Max(Duration x, Duration y)
        {
            return x > y ? x : y;
        }

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
        public static Duration Min(Duration x, Duration y) => x < y ? x : y;

#if !NETSTANDARD
        #region Binary serialization
        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Duration([NotNull] SerializationInfo info, StreamingContext context)
            : this(info)
        {
        }

        /// <summary>
        /// Internal constructor used for deserialization, for cases where this is part of
        /// a larger value, but the duration part has been serialized with the default keys.
        /// </summary>
        internal Duration([NotNull] SerializationInfo info)
            : this(info,
                  BinaryFormattingConstants.DurationDefaultDaysSerializationName,
                  BinaryFormattingConstants.DurationDefaultNanosecondOfDaySerializationName)
        {
        }

        /// <summary>
        /// Internal constructor used for deserialization, for cases where the
        /// names in the serialization info aren't the default ones.
        /// </summary>
        internal Duration([NotNull] SerializationInfo info,
            [Trusted] [NotNull] string daysSerializationName, [Trusted] [NotNull] string nanosecondOfDaySerializationName)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            Preconditions.DebugCheckNotNull(daysSerializationName, nameof(daysSerializationName));
            Preconditions.DebugCheckNotNull(nanosecondOfDaySerializationName, nameof(nanosecondOfDaySerializationName));
            days = info.GetInt32(daysSerializationName);
            nanoOfDay = info.GetInt64(nanosecondOfDaySerializationName);
            if (days < MinDays || days > MaxDays)
            {
                throw new ArgumentException("Serialized value contains a 'days' value which is out of range");
            }
            if (nanoOfDay < 0 || nanoOfDay >= NanosecondsPerDay)
            {
                throw new ArgumentException("Serialized value contains a 'nanosecond-of-day' value which is out of range");
            }
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData([NotNull] SerializationInfo info, StreamingContext context)
        {
            Serialize(info);
        }
        #endregion

        internal void Serialize([NotNull] SerializationInfo info)
        {
            Serialize(info,
                BinaryFormattingConstants.DurationDefaultDaysSerializationName, 
                BinaryFormattingConstants.DurationDefaultNanosecondOfDaySerializationName);
        }

        internal void Serialize([NotNull] SerializationInfo info,
            [Trusted] [NotNull] string daysSerializationName, [Trusted] [NotNull] string nanosecondOfDaySerializationName)
        {
            Preconditions.CheckNotNull(info, nameof(info));
            Preconditions.DebugCheckNotNull(daysSerializationName, nameof(daysSerializationName));
            Preconditions.DebugCheckNotNull(nanosecondOfDaySerializationName, nameof(nanosecondOfDaySerializationName));
            info.AddValue(daysSerializationName, days);
            info.AddValue(nanosecondOfDaySerializationName, nanoOfDay);
        }
#endif
    }
}
