// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Calendars;
using NodaTime.TimeZones;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A number of nanoseconds. This type is used where <see cref="Int64"/> would normally
    /// be used, but the range of date/time values in Noda Time means that occasionally we wish
    /// to represent a number of nanoseconds outside the range of <c>Int64</c>.
    /// </summary>
    public struct Nanoseconds : IEquatable<Nanoseconds>, IComparable<Nanoseconds>, IComparable
    {
        /// <summary>
        /// The number of days within this number of nanoseconds. When this is negative, it is
        /// offset by the positive <see cref="nanoOfDay"/> value. For example, -1ns is represented
        /// by -1 days + 86,399,999,999,999ns.
        /// </summary>
        private readonly int days;
        /// <summary>
        /// The number of nanoseconds into the day.
        /// </summary>
        private readonly long nanoOfDay;

        internal Nanoseconds(int days, long nanoOfDay)
        {
            // TODO(2.0): Validation? At least work out what range we expect...
            this.days = days;
            this.nanoOfDay = nanoOfDay;
        }

        /// <summary>
        /// Returns the "whole day" part of the nanosecond value.
        /// </summary>
        internal int Days { get { return days; } }

        /// <summary>
        /// Returns the "nanosecond of day" part of the nanosecond value.
        /// </summary>
        internal long NanosecondOfDay { get { return nanoOfDay; } }

        /// <summary>
        /// Creates a <see cref="Nanoseconds"/> value from a number of ticks.
        /// </summary>
        /// <param name="ticks">Number of ticks to represent.</param>
        /// <returns>A <see cref="Nanoseconds"/> value for the number of ticks.</returns>
        internal static Nanoseconds FromTicks(long ticks)
        {
            long tickOfDay;
            int days = TickArithmetic.TicksToDaysAndTickOfDay(ticks, out tickOfDay);
            return new Nanoseconds(days, tickOfDay * NodaConstants.NanosecondsPerTick);
        }

        /// <summary>
        /// The number of ticks represented by this number of nanoseconds.
        /// TODO(2.0): Determine rounding policy for negative values... truncate towards zero? Currently truncates down, always.
        /// </summary>
        /// <returns></returns>
        public long Ticks { get { return TickArithmetic.DaysAndTickOfDayToTicks(days, nanoOfDay / NodaConstants.NanosecondsPerTick); } }

        /// <summary>
        /// Explicit conversion to <see cref="Int64"/>. This conversion will fail if the number of nanoseconds is
        /// out of the range of <c>Int64</c>, which is approximately 292 years (positive or negative).
        /// </summary>
        /// <param name="nanoseconds">The <see cref="Nanoseconds"/> representation of a number of nanoseconds.</param>
        /// <exception cref="System.OverflowException">The number of nanoseconds is outside the representable range.</exception>
        public static explicit operator long(Nanoseconds nanoseconds)
        {
            if (nanoseconds.days < 0)
            {
                if (nanoseconds.days > long.MinValue / NodaConstants.NanosecondsPerStandardDay)
                {
                    return unchecked(nanoseconds.days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay);
                }
                // Need to be careful to avoid overflow in a case where it's actually representable
                return (nanoseconds.days + 1) * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay - NodaConstants.NanosecondsPerStandardDay;
            }
            if (nanoseconds.days < long.MaxValue / NodaConstants.NanosecondsPerStandardDay)
            {
                return unchecked(nanoseconds.days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay);
            }
            // Need to be careful to avoid overflow in a case where it's actually representable
            return (nanoseconds.days - 1) * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay + NodaConstants.NanosecondsPerStandardDay;
        }

        /// <summary>
        /// Explicit conversion to <see cref="Decimal"/>, as a convenient built-in numeric type which can always represent values in the range we need.
        /// </summary>
        /// <remarks>The value returned is always an integer.</remarks>
        /// <param name="nanoseconds">The <see cref="Nanoseconds"/> representation of a number of nanoseconds.</param>
        public static explicit operator decimal(Nanoseconds nanoseconds)
        {
            decimal days = nanoseconds.days;
            return days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay;
        }

        /// <summary>
        /// Explicit conversion from <see cref="Int64"/>.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to represent.</param>
        public static explicit operator Nanoseconds(long nanoseconds)
        {
            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NodaConstants.NanosecondsPerStandardDay)
                : (int) ((nanoseconds + 1) / NodaConstants.NanosecondsPerStandardDay) - 1;

            long nanoOfDay = nanoseconds >= long.MinValue + NodaConstants.NanosecondsPerStandardDay
                ? nanoseconds - days * NodaConstants.NanosecondsPerStandardDay
                : nanoseconds - (days + 1) * NodaConstants.NanosecondsPerStandardDay + NodaConstants.NanosecondsPerStandardDay; // Avoid multiplication overflow
            return new Nanoseconds(days, nanoOfDay);
        }

        /// <summary>
        /// Explicit conversion from <see cref="Decimal"/>.
        /// </summary>
        /// <remarks>Any fractional part of <paramref name="nanoseconds"/> is truncated.</remarks>
        /// <param name="nanoseconds">The number of nanoseconds to represent.</param>
        /// <exception cref="OverflowException">The specified number is outside the range of <see cref="Nanoseconds"/>.</exception>
        public static explicit operator Nanoseconds(decimal nanoseconds)
        {
            int days = nanoseconds >= 0
                ? (int) (nanoseconds / NodaConstants.NanosecondsPerStandardDay)
                : (int) ((nanoseconds + 1) / NodaConstants.NanosecondsPerStandardDay) - 1;

            long nanoOfDay = (long) (nanoseconds - ((decimal) days) * NodaConstants.NanosecondsPerStandardDay);
            return new Nanoseconds(days, nanoOfDay);
        }


        /// <summary>
        /// Indicates whether the value of this instant is equal to the value of the specified instant.
        /// </summary>
        /// <param name="other">The value to compare with this instance.</param>
        /// <returns>
        /// true if the value of this instant is equal to the value of the <paramref name="other" /> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Nanoseconds other)
        {
            return this == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Nanoseconds)
            {
                return Equals((Nanoseconds) obj);
            }
            return false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data
        ///   structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return days ^ nanoOfDay.GetHashCode();
        }

        /// <summary>
        /// Implements the operator + (addition) for <see cref="Nanoseconds" /> + <see cref="Nanoseconds" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the sum of the given values.</returns>
        public static Nanoseconds operator +(Nanoseconds left, Nanoseconds right)
        {
            int newDays = left.days + right.days;
            long newNanos = left.nanoOfDay + right.nanoOfDay;
            if (newNanos >= NodaConstants.NanosecondsPerStandardDay)
            {
                newDays++;
                newNanos -= NodaConstants.NanosecondsPerStandardDay;
            }
            else if (newNanos < 0)
            {
                newDays--;
                newNanos += NodaConstants.NanosecondsPerStandardDay;
            }
            return new Nanoseconds(newDays, newNanos);
        }

        /// <summary>
        /// Implements addition for situations where the number of nanoseconds to add
        /// is already known as an <c>Int64</c> nanos-of-day value.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to add.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the sum of the given values.</returns>
        [Pure]
        internal Nanoseconds Plus(long nanoseconds)
        {
            int newDays = days;
            long newNanos = nanoOfDay + nanoseconds;
            if (newNanos >= NodaConstants.NanosecondsPerStandardDay)
            {
                newDays++;
                newNanos -= NodaConstants.NanosecondsPerStandardDay;
            }
            else if (newNanos < 0)
            {
                newDays--;
                newNanos += NodaConstants.NanosecondsPerStandardDay;
            }
            return new Nanoseconds(newDays, newNanos);
        }

        /// <summary>
        /// Implements subtraction for situations where the number of nanoseconds to add
        /// is already known as an <c>Int64</c> nanos-of-day value.
        /// </summary>
        /// <param name="nanoseconds">The nanoseconds to subtract.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the difference between the given values.</returns>
        [Pure]
        internal Nanoseconds Minus(long nanoseconds)
        {
            int newDays = days;
            long newNanos = nanoOfDay - nanoseconds;
            if (newNanos >= NodaConstants.NanosecondsPerStandardDay)
            {
                newDays++;
                newNanos -= NodaConstants.NanosecondsPerStandardDay;
            }
            else if (newNanos < 0)
            {
                newDays--;
                newNanos += NodaConstants.NanosecondsPerStandardDay;
            }
            return new Nanoseconds(newDays, newNanos);
        }

        /// <summary>
        /// Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the sum of the given values.</returns>
        public static Nanoseconds Add(Nanoseconds left, Nanoseconds right)
        {
            return left + right;
        }

        /// <summary>
        /// Returns the result of adding a duration to this instant, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="nanoseconds">The nanoseconds to add</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the result of the addition.</returns>
        [Pure]
        public Nanoseconds Plus(Nanoseconds nanoseconds)
        {
            return this + nanoseconds;
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref="Nanoseconds" /> - <see cref="Nanoseconds" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the difference of the given values.</returns>
        public static Nanoseconds operator -(Nanoseconds left, Nanoseconds right)
        {
            int newDays = left.days - right.days;
            long newNanos = left.nanoOfDay - right.nanoOfDay;
            if (newNanos >= NodaConstants.NanosecondsPerStandardDay)
            {
                newDays++;
                newNanos -= NodaConstants.NanosecondsPerStandardDay;
            }
            else if (newNanos < 0)
            {
                newDays--;
                newNanos += NodaConstants.NanosecondsPerStandardDay;
            }
            return new Nanoseconds(newDays, newNanos);
        }

        /// <summary>
        ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the difference of the given values.</returns>
        public static Nanoseconds Subtract(Nanoseconds left, Nanoseconds right)
        {
            return left - right;
        }

        /// <summary>
        /// Returns the result of subtracting another instant from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The other instant to subtract</param>
        /// <returns>A new <see cref="Nanoseconds" /> representing the result of the subtraction.</returns>
        [Pure]
        public Nanoseconds Minus(Nanoseconds other)
        {
            return this - other;
        }

        public static Nanoseconds operator /(Nanoseconds dividend, long divisor)
        {
            // FIXME:PERF
            decimal x = (decimal) dividend;
            return (Nanoseconds) x / divisor;
        }

        public static Nanoseconds operator *(Nanoseconds nanoseconds, long scalar)
        {
            // FIXME:PERF
            decimal x = (decimal) nanoseconds;
            return (Nanoseconds) (x * scalar);
        }

        public static Nanoseconds operator *(long scalar, Nanoseconds nanoseconds)
        {
            return nanoseconds * scalar;
        }

        public static Nanoseconds operator -(Nanoseconds nanoseconds)
        {
            int oldDays = nanoseconds.days;
            long oldNanoOfDay = nanoseconds.nanoOfDay;
            if (oldNanoOfDay == 0)
            {
                return new Nanoseconds(-oldDays, 0);
            }
            long newNanoOfDay = NodaConstants.NanosecondsPerStandardDay - oldNanoOfDay;
            return new Nanoseconds(-oldDays - 1, newNanoOfDay);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Nanoseconds left, Nanoseconds right)
        {
            return left.days == right.days && left.nanoOfDay == right.nanoOfDay;
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Nanoseconds left, Nanoseconds right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Nanoseconds left, Nanoseconds right)
        {
            return left.days < right.days || (left.days == right.days && left.nanoOfDay < right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Nanoseconds left, Nanoseconds right)
        {
            return left.days < right.days || (left.days == right.days && left.nanoOfDay <= right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Nanoseconds left, Nanoseconds right)
        {
            return left.days > right.days || (left.days == right.days && left.nanoOfDay > right.nanoOfDay);
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Nanoseconds left, Nanoseconds right)
        {
            return left.days > right.days || (left.days == right.days && left.nanoOfDay >= right.nanoOfDay);
        }

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
        public int CompareTo(Nanoseconds other)
        {
            int dayComparison = days.CompareTo(other.days);
            return dayComparison != 0 ? dayComparison : nanoOfDay.CompareTo(other.nanoOfDay);
        }

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
            Preconditions.CheckArgument(obj is Nanoseconds, "obj", "Object must be of type NodaTime.Nanoseconds.");
            return CompareTo((Nanoseconds) obj);
        }
    }
}
