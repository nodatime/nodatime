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
    /// example, subtracting one <see cref="Instant"/> from another will always give a valid <c>Duration</c>. See the user guide
    /// for more details of the exact range, but it is not expected that this will ever be exceeded in normal code.
    /// </para>
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct Duration : IEquatable<Duration>, IComparable<Duration>, IComparable, IXmlSerializable, IFormattable
#if !PCL
        , ISerializable
#endif
    {
        // This is one more bit than we really need, but it allows Instant.BeforeMinValue and Instant.AfterMaxValue
        // to be easily 
        internal const int MaxDays = (1 << 24) - 1;
        internal const int MinDays = ~MaxDays;

        // The -1 here is to allow for the addition of nearly a whole day in the nanoOfDay field.
        private const long MaxDaysForLongNanos = (int) (long.MaxValue / NanosecondsPerDay) - 1;
        private const long MinDaysForLongNanos = (int) (long.MinValue / NanosecondsPerDay);

        #region Readonly static properties
        /// <summary>
        /// Gets a zero <see cref="Duration"/> of 0 nanoseconds.
        /// </summary>
        /// <value>The zero <see cref="Duration"/> value.</value>
        public static Duration Zero => new Duration(0, 0L);

        /// <summary>
        /// Get a <see cref="Duration"/> value equal to 1 nanosecond; the smallest amount by which an instant can vary.
        /// </summary>
        /// <value>A duration representing 1 nanosecond.</value>
        public static Duration Epsilon => new Duration(0, 1L);

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

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 hour.
        /// </summary>
        /// <remarks>
        /// The value of this property is 3.6 trillion nanoseconds; that is, 3,600,000,000,000 nanoseconds.
        /// </remarks>
        private static Duration OneHour => new Duration(0, NanosecondsPerHour);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 minute.
        /// </summary>
        /// <remarks>
        /// The value of this property is 60 billion nanoseconds; that is, 60,000,000,000 nanoseconds.
        /// </remarks>
        private static Duration OneMinute => new Duration(0, NanosecondsPerMinute);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 second.
        /// </summary>
        /// <remarks>
        /// The value of this property is 1 billion nanoseconds; that is, 1,000,000,000 nanoseconds.
        /// </remarks>
        private static Duration OneSecond => new Duration(0, NanosecondsPerSecond);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 millisecond.
        /// </summary>
        /// <remarks>
        /// The value of this property is 1,000,000 nanoseconds.
        /// </remarks>
        private static Duration OneMillisecond => new Duration(0, NanosecondsPerMillisecond);
        #endregion

        // This is effectively a 25 bit value. (It can't be 24 bits, or we can't represent every TimeSpan value.)
        private readonly int days;
        // Always non-negative; a negative duration will have a negative number of days, but may have a
        // positive nanoOfDay. (A duration of -1ns will have a days value of -1 and a nanoOfDay of
        // NanosecondsPerDay - 1, for example.)
        private readonly long nanoOfDay;

        internal Duration(int days, [Trusted] long nanoOfDay)
        {
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
        public int Minutes =>
            unchecked((int) ((NanosecondOfDay / NanosecondsPerMinute) % MinutesPerHour));

        /// <summary>
        /// Gets the second component of this duration, in the range [-59, 59], truncated towards zero.
        /// </summary>
        /// <value>The second component of the duration, within the minute.</value>
        public int Seconds =>
            unchecked((int) ((NanosecondOfDay / NanosecondsPerSecond) % SecondsPerMinute));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in milliseconds, in the range [-999, 999] and truncated towards zero.
        /// </summary>
        /// <value>The subsecond component of the duration, in milliseconds.</value>
        public int Milliseconds =>
            unchecked((int) ((NanosecondOfDay / NanosecondsPerMillisecond) % MillisecondsPerSecond));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in ticks, in the range [-9999999, 9999999] and truncated towards zero.
        /// </summary>
        /// <value>The subsecond component of the duration, in ticks.</value>
        public int SubsecondTicks =>
            unchecked((int) ((NanosecondOfDay / NanosecondsPerTick) % TicksPerSecond));

        /// <summary>
        /// Gets the subsecond component of this duration, expressed in nanoseconds, in the range [-999999999, 999999999].
        /// </summary>
        /// <value>The subsecond component of the duration, in nanoseconds.</value>
        public int SubsecondNanoseconds =>
            unchecked((int) (NanosecondOfDay % NanosecondsPerSecond));

        /// <summary>
        /// Gets the total number of ticks in the duration.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the number of nanoseconds in a duration is not a whole number of ticks, it is truncated towards zero.
        /// For example, durations in the range [-99ns, 99ns] would all count as 0 ticks.
        /// </para>
        /// <para>Although this method can overflow, it will only do so in very exceptional cases, with durations
        /// with a magnitude of more than 29000 Gregorian years or so.</para>
        /// </remarks>
        /// <exception cref="OverflowException">The number of ticks cannot be represented a signed 64-bit integer.</exception>
        /// <value>The total number of ticks in the duration.</value>
        public long Ticks
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

        /// <summary>
        /// Gets the total number of days in this duration, as a <see cref="double"/>.
        /// </summary>
        /// <remarks>This method is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalDays"/>.
        /// It represents the complete duration in days, rather than only the whole number of
        /// days. For example, for a duration of 36 hours, this property would return 1.5.
        /// </remarks>
        /// <value>The total number of days in this duration.</value>
        public double TotalDays => days + nanoOfDay / (double) NanosecondsPerDay;

        /// <summary>
        /// Gets the total number of hours in this duration, as a <see cref="double"/>.
        /// </summary>
        /// <remarks>
        /// This method is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalHours"/>.
        /// Unlike <see cref="Hours"/>, it represents the complete duration in hours rather than the
        /// whole number of hours as part of the day. So for a duration
        /// of 1 day, 2 hours and 30 minutes, the <c>Hours</c> property will return 2, but <c>TotalHours</c>
        /// will return 26.5.
        /// </remarks>
        /// <value>The total number of hours in this duration.</value>
        public double TotalHours =>days * 24.0 + nanoOfDay / (double) NanosecondsPerHour;

        /// <summary>
        /// Gets the total number of minutes in this duration, as a <see cref="double"/>.
        /// </summary>
        /// <remarks>This method is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalMinutes"/>.</remarks>
        /// Unlike <see cref="Minutes"/>, it represents the complete duration in minutes rather than
        /// the whole number of minutes within the hour. So for a duration
        /// of 2 hours, 30 minutes and 45 seconds, the <c>Minutes</c> property will return 30, but <c>TotalMinutes</c>
        /// will return 150.75.
        /// <value>The total number of minutes in this duration.</value>
        public double TotalMinutes =>
            days * (double) MinutesPerDay + nanoOfDay / (double) NanosecondsPerMinute;

        /// <summary>
        /// Gets the total number of seconds in this duration, as a <see cref="double"/>.
        /// </summary>
        /// <remarks>This method is the <c>Duration</c> equivalent of <see cref="TimeSpan.TotalSeconds"/>.</remarks>
        /// Unlike <see cref="Seconds"/>, it represents the complete duration in seconds rather than
        /// the whole number of seconds within the minute. So for a duration
        /// of 10 minutes, 20 seconds and 250 milliseconds, the <c>Seconds</c> property will return 20, but <c>TotalSeconds</c>
        /// will return 620.25.
        /// <value>The total number of minutes in this duration.</value>
        public double TotalSeconds =>
            days * (double) SecondsPerDay + nanoOfDay / (double) NanosecondsPerSecond;

        /// <summary>
        /// Adds a "small" number of nanoseconds to this duration: it is trusted to be less or equal to than 24 hours
        /// in magnitude.
        /// </summary>
        [Pure]
        internal Duration PlusSmallNanoseconds(long smallNanos)
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
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NanosecondsPerDay;
                }
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
        /// Implements the operator / (division).
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
                return Duration.FromNanoseconds(nanos);
            }
            // Fall back to decimal arithmetic.
            decimal x = left.ToDecimalNanoseconds();
            return FromNanoseconds(x / right);
        }

        /// <summary>
        /// Divides a duration by a number. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of dividing <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration Divide(Duration left, long right) => left / right;

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the result of multiplying <paramref name="left"/> by
        /// <paramref name="right"/>.</returns>
        public static Duration operator *(Duration left, long right)
        {
            if (right == 0 || left == Zero)
            {
                return Zero;
            }
            int days = left.days;
            // Speculatively try integer arithmetic... currently just for positive nanos because
            // it's more common and easier to think about.
            if (days >= 0 && days <= MaxDaysForLongNanos)
            {
                long originalNanos = left.ToInt64Nanoseconds();
                if (right > 0)
                {
                    if (right < long.MaxValue / originalNanos)
                    {
                        return FromNanoseconds(originalNanos * right);
                    }
                }
                else
                {
                    if (right > long.MinValue / originalNanos)
                    {
                        return FromNanoseconds(originalNanos * right);
                    }
                }
            }
            // Fall back to decimal arithmetic
            decimal x = left.ToDecimalNanoseconds();
            return FromNanoseconds(x * right);
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

        // TODO(2.0): Optimize?
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of hours.</returns>
        public static Duration FromHours(long hours) => OneHour * hours;

        // TODO(2.0): Optimize?
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of minutes.</returns>
        public static Duration FromMinutes(long minutes) => OneMinute * minutes;

        // TODO(2.0): Optimize?
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of seconds.</returns>
        public static Duration FromSeconds(long seconds) => OneSecond * seconds;

        // TODO(2.0): Optimize?
        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of milliseconds.</returns>
        public static Duration FromMilliseconds(long milliseconds) => OneMillisecond * milliseconds;

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of ticks.</returns>
        public static Duration FromTicks(long ticks)
        {
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);
            return new Duration(days, tickOfDay * NanosecondsPerTick);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of nanoseconds.</returns>
        public static Duration FromNanoseconds(long nanoseconds)
        {
            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NanosecondsPerDay) - 1;

            long nanoOfDay = nanoseconds >= long.MinValue + NanosecondsPerDay
                ? nanoseconds - days * NanosecondsPerDay
                : nanoseconds - (days + 1) * NanosecondsPerDay + NanosecondsPerDay; // Avoid multiplication overflow
            return new Duration(days, nanoOfDay);
        }

        /// <summary>
        /// Converts a number of nanoseconds expressed as a <see cref="Decimal"/> into a duration.
        /// </summary>
        /// <remarks>Any fractional part of <paramref name="nanoseconds"/> is truncated.</remarks>
        /// <param name="nanoseconds">The number of nanoseconds to represent.</param>
        /// <returns>A duration with the given number of nanoseconds.</returns>
        /// <exception cref="OverflowException">The specified number is outside the range of <see cref="Duration"/>.</exception>
        public static Duration FromNanoseconds(decimal nanoseconds)
        {
            // Avoid worrying about what happens in later arithmetic.
            nanoseconds = decimal.Truncate(nanoseconds);

            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NanosecondsPerDay) - 1;

            long nanoOfDay = (long) (nanoseconds - ((decimal) days) * NanosecondsPerDay);
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
        public TimeSpan ToTimeSpan() => new TimeSpan(Ticks);

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema() => null;

        /// <inheritdoc />
        void IXmlSerializable.ReadXml([NotNull] XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, nameof(reader));
            var pattern = DurationPattern.RoundtripPattern;
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml([NotNull] XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, nameof(writer));
            writer.WriteString(DurationPattern.RoundtripPattern.Format(this));
        }
        #endregion

        /// <summary>
        /// Conversion to an <see cref="Int64"/> number of nanoseconds. This will fail if the number of nanoseconds is
        /// out of the range of <c>Int64</c>, which is approximately 292 years (positive or negative).
        /// </summary>
        /// <exception cref="System.OverflowException">The number of nanoseconds is outside the representable range.</exception>
        /// <returns>This duration as a number of nanoseconds, represented as an <c>Int64</c>.</returns>
        [Pure]
        public long ToInt64Nanoseconds()
        {
            if (days < 0)
            {
                if (days >= MinDaysForLongNanos)
                {
                    return unchecked(days * NanosecondsPerDay + nanoOfDay);
                }
                // Need to be careful to avoid overflow in a case where it's actually representable
                return (days + 1) * NanosecondsPerDay + nanoOfDay - NanosecondsPerDay;
            }
            if (days <= MaxDaysForLongNanos)
            {
                return unchecked(days * NanosecondsPerDay + nanoOfDay);
            }
            // Either the multiplication or the addition could overflow, so we do it in a checked context.
            return days * NanosecondsPerDay + nanoOfDay;
        }

        /// <summary>
        /// Conversion to a <see cref="Decimal"/> number of nanoseconds, as a convenient built-in numeric
        /// type which can always represent values in the range we need.
        /// </summary>
        /// <remarks>The value returned is always an integer.</remarks>
        /// <returns>This duration as a number of nanoseconds, represented as a <c>Decimal</c>.</returns>
        [Pure]
        public decimal ToDecimalNanoseconds() => ((decimal)days) * NanosecondsPerDay + nanoOfDay;

#if !PCL
        #region Binary serialization
        private const string DefaultDaysSerializationName = "days";
        private const string DefaultNanosecondOfDaySerializationName = "nanoOfDay";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Duration([NotNull] SerializationInfo info, StreamingContext context)
            : this(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName)
        {
        }

        /// <summary>
        /// Internal constructor used for deserialization, for cases where this is part of
        /// a larger value, but the duration part has been serialized with the default keys.
        /// </summary>
        internal Duration([NotNull] SerializationInfo info)
            : this(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName)
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
            // FIXME(2.0): Determine serialization policy
            Serialize(info);
        }
        #endregion

        internal void Serialize([NotNull] SerializationInfo info)
        {
            Serialize(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName);
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
