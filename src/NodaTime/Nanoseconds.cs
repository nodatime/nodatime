// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using NodaTime.Calendars;
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
        /// The zero value for the type.
        /// </summary>
        public static readonly Nanoseconds Zero = new Nanoseconds();

        // The -1 here is to allow for the addition of nearly a whole day in the nanoOfDay field.
        private const long MaxDaysForLongNanos = (int) (long.MaxValue / NodaConstants.NanosecondsPerStandardDay) - 1;
        private const long MinDaysForLongNanos = (int) (long.MinValue / NodaConstants.NanosecondsPerStandardDay);

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
        /// </summary>
        /// <remarks>
        /// If the value does not consist of an exact number of ticks, it is truncated to towards 0.
        /// For example, the entire range of [-99ns, 99ns] returns 0 ticks. This is consistent with
        /// integer division.
        /// </remarks>
        /// <returns>The number of ticks in this value, truncating towards zero.</returns>
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
        /// Similar to Ticks, but truncates negatively, so a value of -1ns will return -1 tick.
        /// This is used by Instant.Ticks to always truncate towards the start of time.
        /// </summary>
        internal long FloorTicks
        {
            get { return TickArithmetic.DaysAndTickOfDayToTicks(days, nanoOfDay / NodaConstants.NanosecondsPerTick); }
        }

        /// <summary>
        /// Explicit conversion to <see cref="Int64"/>. This conversion will fail if the number of nanoseconds is
        /// out of the range of <c>Int64</c>, which is approximately 292 years (positive or negative).
        /// </summary>
        /// <param name="nanoseconds">The <see cref="Nanoseconds"/> representation of a number of nanoseconds.</param>
        /// <exception cref="System.OverflowException">The number of nanoseconds is outside the representable range.</exception>
        public static explicit operator long(Nanoseconds nanoseconds)
        {
            int days = nanoseconds.days;
            if (days < 0)
            {
                if (days >= MinDaysForLongNanos)
                {
                    return unchecked(days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay);
                }
                // Need to be careful to avoid overflow in a case where it's actually representable
                return (days + 1) * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay - NodaConstants.NanosecondsPerStandardDay;
            }
            if (days <= MaxDaysForLongNanos)
            {
                return unchecked(days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay);
            }
            // Either the multiplication or the addition could overflow, so we do it in a checked context.
            return days * NodaConstants.NanosecondsPerStandardDay + nanoseconds.nanoOfDay;
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
            // Avoid worrying about what happens in later arithmetic.
            nanoseconds = decimal.Truncate(nanoseconds);

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

        /// <summary>
        /// Implements the operator / (division). The result is truncated towards zero.
        /// </summary>
        /// <param name="dividend">The left hand side of the operator.</param>
        /// <param name="divisor">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds"/> representing the result of dividing <paramref name="dividend"/> by
        /// <paramref name="divisor"/>.</returns>
        public static Nanoseconds operator /(Nanoseconds dividend, long divisor)
        {
            int days = dividend.days;
            // Simplest scenario to handle
            if (days == 0 && divisor > 0)
            {
                return new Nanoseconds(0, dividend.nanoOfDay / divisor);
            }
            // Now for the ~[-250, +250] year range, where we can do it all as a long.
            if (days >= MinDaysForLongNanos && days <= MaxDaysForLongNanos)
            {
                long nanos = ((long) dividend) / divisor;
                return (Nanoseconds) nanos;
            }
            // Fall back to decimal arithmetic.
            decimal x = (decimal) dividend;
            return (Nanoseconds) (x / divisor);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="nanoseconds">The left hand side of the operator.</param>
        /// <param name="scalar">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds"/> representing the result of multiplying <paramref name="nanoseconds"/> by
        /// <paramref name="scalar"/>.</returns>
        public static Nanoseconds operator *(Nanoseconds nanoseconds, long scalar)
        {
            if (scalar == 0 || nanoseconds == Zero)
            {
                return Zero;
            }
            int days = nanoseconds.days;
            // Speculatively try integer arithmetic... currently just for positive nanos because
            // it's more common and easier to think about.
            if (days >= 0 && days <= MaxDaysForLongNanos)
            {
                long originalNanos = (long) nanoseconds;
                if (scalar > 0)
                {
                    if (scalar < long.MaxValue / originalNanos)
                    {
                        return (Nanoseconds) (originalNanos * scalar);
                    }
                }
                else
                {
                    if (scalar > long.MinValue / originalNanos)
                    {
                        return (Nanoseconds) (originalNanos * scalar);
                    }
                }
            }
            // Fall back to decimal arithmetic
            decimal x = (decimal) nanoseconds;
            return (Nanoseconds) (x * scalar);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="scalar">The left hand side of the operator.</param>
        /// <param name="nanoseconds">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Nanoseconds"/> representing the result of multiplying <paramref name="nanoseconds"/> by
        /// <paramref name="scalar"/>.</returns>
        public static Nanoseconds operator *(long scalar, Nanoseconds nanoseconds)
        {
            return nanoseconds * scalar;
        }

        /// <summary>
        /// Implements the unary negation operator.
        /// </summary>
        /// <param name="nanoseconds">Number of nanoseconds to negate</param>
        /// <returns>The negative value of this number of nanoseconds</returns>
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
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(Nanoseconds)"/> for general details.
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

        /// <summary>
        /// Returns the string representation of this value, as an integer followed by "ns".
        /// </summary>
        /// <returns>The string representation of this value.</returns>
        public override string ToString()
        {
            return string.Format("{0}ns", (decimal) this);
        }

#if !BCL
        private const string DefaultDaysSerializationName = "days";
        private const string DefaultNanosecondOfDaySerializationName = "nanoOfDay";

        internal static Nanoseconds Deserialize(SerializationInfo info)
        {
            return Deserialize(info, DefaultDaysSerializationName, DefaultNanosecondOfDaySerializationName);
        }

        internal static Nanoseconds Deserialize(SerializationInfo info, string daysSerializationName, string nanosecondOfDaySerializationName)
            
        {
            return new Nanoseconds(info.GetInt32(daysSerializationName), info.GetInt64(nanosecondOfDaySerializationName));
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
