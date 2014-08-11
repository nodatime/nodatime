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
    /// calendrical terms (hours, days, and so on) that may vary in elapsed time when applied.
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
        internal const int MaxDays = (1 << 24) - 1;
        internal const int MinDays = ~MaxDays;

        // The -1 here is to allow for the addition of nearly a whole day in the nanoOfDay field.
        private const long MaxDaysForLongNanos = (int) (long.MaxValue / NodaConstants.NanosecondsPerDay) - 1;
        private const long MinDaysForLongNanos = (int) (long.MinValue / NodaConstants.NanosecondsPerDay);

        #region Readonly static fields
        /// <summary>
        /// Represents the zero <see cref="Duration"/> value. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration Zero = new Duration(0, 0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to 1 nanosecond; the smallest amount by which an instant can vary.
        /// This field is read-only.
        /// </summary>
        public static readonly Duration Epsilon = new Duration(0, 1L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 standard week (7 days).
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 604,800,000,000,000 nanoseconds.
        /// </remarks>
        internal static readonly Duration OneWeek = new Duration(7, 0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 day.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 86.4 trillion nanoseconds; that is, 86,400,000,000,000 nanoseconds.
        /// </remarks>
        internal static readonly Duration OneDay = new Duration(1, 0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 hour.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 3.6 trillion nanoseconds; that is, 3,600,000,000,000 nanoseconds.
        /// </remarks>
        private static readonly Duration OneHour = new Duration(0, NodaConstants.NanosecondsPerHour);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 minute.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 60 billion nanoseconds; that is, 60,000,000,000 nanoseconds.
        /// </remarks>
        private static readonly Duration OneMinute = new Duration(0, NodaConstants.NanosecondsPerMinute);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equal to the number of nanoseconds in 1 second.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 1 billion nanoseconds; that is, 1,000,000,000 nanoseconds.
        /// </remarks>
        private static readonly Duration OneSecond = new Duration(0, NodaConstants.NanosecondsPerSecond);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 millisecond.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 1,000,000 nanoseconds.
        /// </remarks>
        private static readonly Duration OneMillisecond = new Duration(0, NodaConstants.NanosecondsPerMillisecond);
        #endregion

        // This is effectively a 25 bit value. (It can't be 24 bits, or we can't represent every TimeSpan value.)
        // 
        private readonly int days;
        private readonly long nanoOfDay;

        internal Duration(int days, [Trusted] long nanoOfDay)
        {
            if (days < MinDays || days > MaxDays)
            {
                Preconditions.CheckArgumentRange("days", days, MinDays, MaxDays);
            }
            Preconditions.DebugCheckArgumentRange("nanoOfDay", nanoOfDay, 0, NodaConstants.NanosecondsPerDay - 1);
            this.days = days;
            this.nanoOfDay = nanoOfDay;
        }

        /// <summary>
        /// Like the other constructor, but the nano-of-day is validated as well. Used for deserialization.
        /// </summary>
        private Duration(int days, long nanoOfDay, bool ignored) : this(days, nanoOfDay)
        {
            Preconditions.CheckArgumentRange("nanoOfDay", nanoOfDay, 0, NodaConstants.NanosecondsPerDay - 1);
        }

        /// <summary>
        /// Days portion of this duration. The <see cref="NanosecondOfFloorDay" /> is added to this
        /// value, so this effectively Math.Floor(TotalDays).
        /// </summary>
        internal int FloorDays { get { return days; } }

        /// <summary>
        /// Nanosecond within the "floor day". This is *always* non-negative, even for
        /// negative durations.
        /// </summary>
        internal long NanosecondOfFloorDay { get { return nanoOfDay; } }

        /// <summary>
        /// The total number of ticks in the duration.
        /// </summary>
        /// <remarks>
        /// If the number of nanoseconds in a duration is not a whole number of ticks, it is truncated towards zero.
        /// For example, durations in the range [-99ns, 99ns] would all count as 0 ticks.
        /// </remarks>
        /// <exception cref="OverflowException">The number of ticks cannot be represented a signed 64-bit integer.</exception>
        public long Ticks
        {
            get
            {
                long ticks = TickArithmetic.DaysAndTickOfDayToTicks(days, nanoOfDay / NodaConstants.NanosecondsPerTick);
                if (days < 0 && nanoOfDay % NodaConstants.NanosecondsPerTick != 0)
                {
                    ticks++;
                }
                return ticks;
            }
        }

        /// <summary>
        /// Adds a "small" number of nanoseconds to this duration: it is trusted to be less or equal to than 24 hours
        /// in magnitude.
        /// </summary>
        [Pure]
        internal Duration PlusSmallNanoseconds(long smallNanos)
        {
            unchecked
            {
                Preconditions.DebugCheckArgumentRange("smallNanos", smallNanos, -NodaConstants.NanosecondsPerDay, NodaConstants.NanosecondsPerDay);
                int newDays = days;
                long newNanos = nanoOfDay + smallNanos;
                if (newNanos >= NodaConstants.NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NodaConstants.NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NodaConstants.NanosecondsPerDay;
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
                Preconditions.DebugCheckArgumentRange("smallNanos", smallNanos, -NodaConstants.NanosecondsPerDay, NodaConstants.NanosecondsPerDay);
                int newDays = days;
                long newNanos = nanoOfDay - smallNanos;
                if (newNanos >= NodaConstants.NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NodaConstants.NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NodaConstants.NanosecondsPerDay;
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
        public override bool Equals(object obj)
        {
            if (obj is Duration)
            {
                return Equals((Duration)obj);
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
            return days ^ nanoOfDay.GetHashCode();
        }
        #endregion

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("o"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return DurationPattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);
        }

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
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return DurationPattern.BclSupport.Format(this, patternText, formatProvider);
        }
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
                if (newNanos >= NodaConstants.NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NodaConstants.NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NodaConstants.NanosecondsPerDay;
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
        public static Duration Add(Duration left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        /// Returns the result of adding another duration to this one, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="other">The duration to add</param>
        /// <returns>A new <see cref="Duration" /> representing the result of the addition.</returns>
        [Pure]
        public Duration Plus(Duration other)
        {
            return this + other;
        }

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
                if (newNanos >= NodaConstants.NanosecondsPerDay)
                {
                    newDays++;
                    newNanos -= NodaConstants.NanosecondsPerDay;
                }
                else if (newNanos < 0)
                {
                    newDays--;
                    newNanos += NodaConstants.NanosecondsPerDay;
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
        public static Duration Subtract(Duration left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        /// Returns the result of subtracting another duration from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The duration to subtract</param>
        /// <returns>A new <see cref="Duration" /> representing the result of the subtraction.</returns>
        [Pure]
        public Duration Minus(Duration other)
        {
            return this - other;
        }

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
        public static Duration Divide(Duration left, long right)
        {
            return left / right;
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
        public static Duration operator *(long left, Duration right)
        {
            return right * left;
        }

        /// <summary>
        /// Multiplies a duration by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the product of the given values.</returns>
        public static Duration Multiply(Duration left, long right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiplies a duration by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the product of the given values.</returns>
        public static Duration Multiply(long left, Duration right)
        {
            return left * right;
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Duration left, Duration right)
        {
            return left.days == right.days && left.nanoOfDay == right.nanoOfDay;
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Duration left, Duration right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Duration left, Duration right)
        {
            return left.days < right.days || (left.days == right.days && left.nanoOfDay < right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Duration left, Duration right)
        {
            return left.days < right.days || (left.days == right.days && left.nanoOfDay <= right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Duration left, Duration right)
        {
            return left.days > right.days || (left.days == right.days && left.nanoOfDay > right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Duration left, Duration right)
        {
            return left.days > right.days || (left.days == right.days && left.nanoOfDay >= right.nanoOfDay);
        }

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
            long newNanoOfDay = NodaConstants.NanosecondsPerDay - oldNanoOfDay;
            return new Duration(-oldDays - 1, newNanoOfDay);
        }

        /// <summary>
        /// Implements a friendly alternative to the unary negation operator.
        /// </summary>
        /// <param name="duration">Duration to negate</param>
        /// <returns>The negative value of this duration</returns>
        public static Duration Negate (Duration duration)
        {
            return -duration;
        }
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
            Preconditions.CheckArgument(obj is Duration, "obj", "Object must be of type NodaTime.Duration.");
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
        public bool Equals(Duration other)
        {
            return this == other;
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of days, assuming a 'standard' 24-hour
        /// day.
        /// </summary>
        /// <param name="days">The number of days.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of days.</returns>
        public static Duration FromDays(int days)
        {
            return new Duration(days, 0L);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of hours.</returns>
        public static Duration FromHours(long hours)
        {
            // TODO(2.0): Optimize?
            return OneHour * hours;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of minutes.</returns>
        public static Duration FromMinutes(long minutes)
        {
            // TODO(2.0): Optimize?
            return OneMinute * minutes;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of seconds.</returns>
        public static Duration FromSeconds(long seconds)
        {
            // TODO(2.0): Optimize?
            return OneSecond * seconds;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of milliseconds.</returns>
        public static Duration FromMilliseconds(long milliseconds)
        {
            // TODO(2.0): Optimize?
            return OneMillisecond * milliseconds;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of ticks.</returns>
        public static Duration FromTicks(long ticks)
        {
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);
            return new Duration(days, tickOfDay * NodaConstants.NanosecondsPerTick);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds.</param>
        /// <returns>A <see cref="Duration"/> representing the given number of nanoseconds.</returns>
        public static Duration FromNanoseconds(long nanoseconds)
        {
            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NodaConstants.NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NodaConstants.NanosecondsPerDay) - 1;

            long nanoOfDay = nanoseconds >= long.MinValue + NodaConstants.NanosecondsPerDay
                ? nanoseconds - days * NodaConstants.NanosecondsPerDay
                : nanoseconds - (days + 1) * NodaConstants.NanosecondsPerDay + NodaConstants.NanosecondsPerDay; // Avoid multiplication overflow
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
                ? (int) (nanoseconds / NodaConstants.NanosecondsPerDay)
                : (int) ((nanoseconds + 1) / NodaConstants.NanosecondsPerDay) - 1;

            long nanoOfDay = (long) (nanoseconds - ((decimal) days) * NodaConstants.NanosecondsPerDay);
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
        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(Ticks);
        }

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var pattern = DurationPattern.RoundtripPattern;
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            writer.WriteString(DurationPattern.RoundtripPattern.Format(this));
        }
        #endregion

        /// <summary>
        /// Conversion to an <see cref="Int64"/> number of nanoseconds. This will fail if the number of nanoseconds is
        /// out of the range of <c>Int64</c>, which is approximately 292 years (positive or negative).
        /// </summary>
        /// <exception cref="System.OverflowException">The number of nanoseconds is outside the representable range.</exception>
        [Pure]
        public long ToInt64Nanoseconds()
        {
            if (days < 0)
            {
                if (days >= MinDaysForLongNanos)
                {
                    return unchecked(days * NodaConstants.NanosecondsPerDay + nanoOfDay);
                }
                // Need to be careful to avoid overflow in a case where it's actually representable
                return (days + 1) * NodaConstants.NanosecondsPerDay + nanoOfDay - NodaConstants.NanosecondsPerDay;
            }
            if (days <= MaxDaysForLongNanos)
            {
                return unchecked(days * NodaConstants.NanosecondsPerDay + nanoOfDay);
            }
            // Either the multiplication or the addition could overflow, so we do it in a checked context.
            return days * NodaConstants.NanosecondsPerDay + nanoOfDay;
        }

        /// <summary>
        /// Conversion to a <see cref="Decimal"/> number of nanoseconds, as a convenient built-in numeric
        /// type which can always represent values in the range we need.
        /// </summary>
        /// <remarks>The value returned is always an integer.</remarks>
        [Pure]
        public decimal ToDecimalNanoseconds()
        {
            return ((decimal) days) * NodaConstants.NanosecondsPerDay + nanoOfDay;
        }

#if !PCL
        #region Binary serialization
        private const string DefaultDaysSerializationName = "days";
        private const string DefaultNanosecondOfDaySerializationName = "nanoOfDay";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Duration(SerializationInfo info, StreamingContext context)
            : this(info.GetInt32(DefaultDaysSerializationName), info.GetInt64(DefaultNanosecondOfDaySerializationName), true /* validate */)
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
            // FIXME(2.0): Determine serialization policy
            Serialize(info);
        }
        #endregion


        internal static Duration Deserialize(SerializationInfo info)
        {
            return Deserialize(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName);
        }

        internal static Duration Deserialize(SerializationInfo info, string daysSerializationName, string nanosecondOfDaySerializationName)
        {
            return new Duration(info.GetInt32(daysSerializationName), info.GetInt64(nanosecondOfDaySerializationName), true /* validate */);
        }

        internal void Serialize(SerializationInfo info)
        {
            Serialize(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName);
        }

        internal void Serialize(SerializationInfo info, string daysSerializationName, string nanosecondOfDaySerializationName)
        {
            info.AddValue(daysSerializationName, days);
            info.AddValue(nanosecondOfDaySerializationName, nanoOfDay);
        }
#endif
    }
}
