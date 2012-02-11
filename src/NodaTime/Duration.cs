#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Globalization;

namespace NodaTime
{
    /// <summary>
    /// A length of time in ticks. (There are 10,000 ticks in a millisecond.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// There is no concept of fields, such as days or seconds, as these fields can vary in length.
    /// A duration may be converted to a <see cref="Period" /> to obtain field values. This
    /// conversion will typically cause a loss of precision.
    /// </para>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    /// </remarks>
    public struct Duration : IEquatable<Duration>, IComparable<Duration>, IComparable
    {
        #region Public readonly fields
        /// <summary>
        /// Represents <see cref="Duration"/> value equal to negative 1 tick. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration NegativeOne = new Duration(-1L);

        /// <summary>
        /// Represents the zero <see cref="Duration"/> value. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration Zero = new Duration(0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to 1 tick. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration One = new Duration(1L);

        /// <summary>
        /// Represents the mimimum <see cref="Duration"/> value. 
        /// This field is read-only.
        /// </summary>
        /// <remarks>
        /// The value of this field is equivalent to <see cref="Int64.MinValue"/> ticks. 
        /// The string representation of this value is PT-922337203685.4775808S.
        /// </remarks>
        public static readonly Duration MinValue = new Duration(Int64.MinValue);

        /// <summary>
        /// Represents the maximum <see cref="Duration"/> value. 
        /// This field is read-only.
        /// </summary>
        /// <remarks>
        /// The value of this field is equivalent to <see cref="Int64.MaxValue"/> ticks. 
        /// The string representation of this value is PT922337203685.4775807S.
        /// </remarks>
        public static readonly Duration MaxValue = new Duration(Int64.MaxValue);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 week.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 6,048.000,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneWeek = new Duration(NodaConstants.TicksPerStandardWeek);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 day.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 864 billion ticks; that is, 864,000,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneDay = new Duration(NodaConstants.TicksPerStandardDay);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 hour.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 36 billion ticks; that is, 36,000,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneHour = new Duration(NodaConstants.TicksPerHour);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 minute.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 600 million ticks; that is, 600,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneMinute = new Duration(NodaConstants.TicksPerMinute);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 second.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 10 million ticks; that is, 10,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneSecond = new Duration(NodaConstants.TicksPerSecond);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 millisecond.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// TThe value of this constant is 10 thousand; that is, 10,000.
        /// </remarks>
        public static readonly Duration OneMillisecond = new Duration(NodaConstants.TicksPerMillisecond);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> struct.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        public Duration(long ticks)
        {
            this.ticks = ticks;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> struct as the difference
        /// betweeen the given ticks. This is effectively <c>new Duration(end - start)</c>.
        /// </summary>
        /// <param name="startTicks">The start ticks.</param>
        /// <param name="endTicks">The end ticks.</param>
        public Duration(long startTicks, long endTicks)
            : this(endTicks - startTicks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> struct as the difference
        /// betweeen the given <see cref="Instant"/> values. This is effectively <c>new
        /// Duration(end.Ticks - start.Ticks)</c>.
        /// </summary>
        /// <param name="start">The start <see cref="Instant"/> value.</param>
        /// <param name="end">The end <see cref="Instant"/> value.</param>
        public Duration(Instant start, Instant end)
            : this(end.Ticks - start.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> struct as the duration of the
        /// given <see cref="Interval"/> object.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public Duration(Interval interval)
            : this(interval.Duration.Ticks)
        {
        }
        #endregion

        private readonly long ticks;

        /// <summary>
        /// The number of ticks in the duration.
        /// </summary>
        public long Ticks { get { return ticks; } }

        // TODO: Add milliseconds, seconds, minutes, hours, standard days?

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
            return Ticks.GetHashCode();
        }

        // TODO: We should *consider* representing this as in the same way as a period.
        /// <summary>
        /// Gets the value as a <see cref="String"/> showing the number of ticks represented by this duration.
        /// </summary>
        /// <returns>A string representation of this duration.</returns>
        public override string ToString()
        {
            return Ticks.ToString();
        }
        #endregion  // Object overrides

        #region Operators
        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the sum of the given values.</returns>
        public static Duration operator +(Duration left, Duration right)
        {
            return new Duration(left.Ticks + right.Ticks);
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
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the difference of the given values.</returns>
        public static Duration operator -(Duration left, Duration right)
        {
            return new Duration(left.Ticks - right.Ticks);
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
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the duration divided by the scale.</returns>
        public static Duration operator /(Duration left, long right)
        {
            return new Duration(left.Ticks / right);
        }

        /// <summary>
        /// Divides a duration by a number. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the quotient of the given values.</returns>
        public static Duration Divide(Duration left, long right)
        {
            return left / right;
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the duration multiplied by the scale.</returns>
        public static Duration operator *(Duration left, long right)
        {
            return new Duration(left.Ticks * right);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the duration multiplied by the scale.</returns>
        public static Duration operator *(long left, Duration right)
        {
            return new Duration(left * right.Ticks);
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
            return left.Equals(right);
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
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Duration left, Duration right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Duration left, Duration right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Duration left, Duration right)
        {
            return left.CompareTo(right) >= 0;
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
            return Ticks.CompareTo(other.Ticks);
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
            if (!(obj is Duration))
            {
                throw new ArgumentException("Argument did not refer to an instance of NodaTime.Duration.", "obj");
            }
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
            return Ticks == other.Ticks;
        }
        #endregion

        /// <summary>
        /// Converts this duration to an <see cref="Interval"/> starting at the specified instant.
        /// </summary>
        /// <param name="start">The instant to start the interval at</param>
        /// <returns>An <see cref="Interval"/> starting at the specified instant</returns>
        public Interval ToIntervalFrom(Instant start)
        {
            return new Interval(start, start + this);
        }

        /// <summary>
        /// Converts this duration to an <see cref="Interval"/> ending at the specified instant.
        /// </summary>
        /// <param name="end">The instant to end the interval at</param>
        /// <returns>An <see cref="Interval"/> ending at the specified instant</returns>
        public Interval ToIntervalTo(Instant end)
        {
            return new Interval(end - this, end);
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of standard weeks made
        /// up from 7 24-hour days.
        /// </summary>
        /// <param name="weeks">The number of weeks.</param>
        /// <returns>A <see cref="Duration"/> number of weeks.</returns>
        public static Duration FromStandardWeeks(long weeks)
        {
            return OneWeek * weeks;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of standard days made
        /// up from 24 hours.
        /// </summary>
        /// <param name="days">The number of days.</param>
        /// <returns>A <see cref="Duration"/> number of days.</returns>
        public static Duration FromStandardDays(long days)
        {
            return OneDay * days;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>A <see cref="Duration"/> number of hours.</returns>
        public static Duration FromHours(long hours)
        {
            return OneHour * hours;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>A <see cref="Duration"/> number of minutes.</returns>
        public static Duration FromMinutes(long minutes)
        {
            return OneMinute * minutes;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>A <see cref="Duration"/> number of seconds.</returns>
        public static Duration FromSeconds(long seconds)
        {
            return OneSecond * seconds;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>A <see cref="Duration"/> number of milliseconds.</returns>
        public static Duration FromMilliseconds(long milliseconds)
        {
            return OneMillisecond * milliseconds;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of ticks.
        /// </summary>
        /// <remarks>
        /// This is simply an alternative to calling the constructor, allowing for consistent
        /// code when there are multiple <c>FromXyz</c> calls.
        /// </remarks>
        /// <param name="ticks">The number of ticks.</param>
        /// <returns>A <see cref="Duration"/> number of ticks.</returns>
        public static Duration FromTicks(long ticks)
        {
            return new Duration(ticks);
        }
    }
}