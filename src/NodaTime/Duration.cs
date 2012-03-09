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

namespace NodaTime
{
    /// <summary>
    /// A length of time in ticks. (There are 10,000 ticks in a millisecond.) A duration represents
    /// a fixed length of time, with no concept of calendars.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Properties for this type only go as far as StandardDays, as there's no way of considering a
    /// "month" without reference to a calendar. For human calculations, use a <see cref="Period"/> instead,
    /// computing values against local dates/times.
    /// </para>
    /// </remarks>
    public struct Duration : IEquatable<Duration>, IComparable<Duration>, IComparable
    {
        #region Public readonly fields
        /// <summary>
        /// Represents <see cref="Duration"/> value equal to negative 1 tick. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration NegativeOneTick = new Duration(-1L);

        /// <summary>
        /// Represents the zero <see cref="Duration"/> value. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration Zero = new Duration(0L);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to 1 tick. 
        /// This field is read-only.
        /// </summary>
        public static readonly Duration OneTick = new Duration(1L);

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
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 standard week (7 days).
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 6,048,000,000,000 ticks.
        /// </remarks>
        // TODO: Consider exposing this publicly. We use it internally, but I'm not sure about making it public...
        internal static readonly Duration OneStandardWeek = new Duration(NodaConstants.TicksPerStandardWeek);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 day.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 864 billion ticks; that is, 864,000,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneStandardDay = new Duration(NodaConstants.TicksPerStandardDay);

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Duration"/> struct.
        /// </summary>
        /// <param name="ticks">The number of ticks.</param>
        internal Duration(long ticks)
        {
            this.ticks = ticks;
        }

        private readonly long ticks;

        /// <summary>
        /// The total number of ticks in the duration.
        /// </summary>
        /// <remarks>
        /// This property effectively represents all of the information within a Duration value; a duration
        /// is simply a number of ticks.
        /// </remarks>
        public long TotalTicks { get { return ticks; } }

        /// <summary>
        /// The total number of milliseconds in this duration (so a second and a half would return 1500).
        /// This value is always truncated towards zero.
        /// </summary>
        public long TotalMilliseconds { get { return ticks / NodaConstants.TicksPerMillisecond; } }

        /// <summary>
        /// The total number of seconds in this duration (so a minute and a half would return 90).
        /// This value is always truncated towards zero.
        /// </summary>
        public long TotalSeconds { get { return ticks / NodaConstants.TicksPerSecond; } }

        /// <summary>
        /// The total number of minutes in this duration (so an hour and a half would return 90).
        /// This value is always truncated towards zero.
        /// </summary>
        public long TotalMinutes { get { return ticks / NodaConstants.TicksPerMinute; } }

        /// <summary>
        /// The total number of hours in this duration (so a day and a half would return 36).
        /// This value is always truncated towards zero.
        /// </summary>
        public long TotalHours { get { return ticks / NodaConstants.TicksPerHour; } }

        /// <summary>
        /// The total number of standard (24 hour) days in this duration (so a day and a half would return 1).
        /// This value is always truncated towards zero.
        /// </summary>
        public long StandardDays { get { return ticks / NodaConstants.TicksPerStandardDay; } }

        /// <summary>
        /// The number of ticks within a millisecond in this duration, in the range [-9999, 9999].
        /// </summary>
        /// <remarks>
        /// A negative duration will always return a negative value, rounded towards zero. So
        /// a value of -1.5 milliseconds would give -5000 for TicksRemainder, and -1 for <see cref="MillisecondsRemainder" />
        /// </remarks>
        public long TicksRemainder { get { return ticks % NodaConstants.TicksPerMillisecond; } }

        /// <summary>
        /// The number of milliseconds within a second in this duration, in the range [-999, 999].
        /// </summary>
        /// <remarks>
        /// A negative duration will always return a negative value, rounded towards zero. So
        /// a value of -1.5 seconds would give -500 for MillisecondsRemainder, and -1 for <see cref="SecondsRemainder" />
        /// </remarks>
        public long MillisecondsRemainder { get { return (ticks % NodaConstants.TicksPerSecond) / NodaConstants.TicksPerMillisecond; } }

        /// <summary>
        /// The number of seconds within a minute in this duration, in the range [-59, 59].
        /// </summary>
        /// <remarks>
        /// A negative duration will always return a negative value, rounded towards zero. So
        /// a value of -90 seconds would give -30 for SecondsRemainder, and -1 for <see cref="MinutesRemainder" />.
        /// </remarks>
        public long SecondsRemainder { get { return (ticks % NodaConstants.TicksPerMinute) / NodaConstants.TicksPerSecond; } }

        /// <summary>
        /// The number of minutes within an hour in this duration, in the range [-59, 59].
        /// </summary>
        /// <remarks>
        /// A negative duration will always return a negative value, rounded towards zero. So
        /// a value of -90 minutes would give -30 for MinutesRemainder, and -1 for <see cref="HoursRemainder" />.
        /// </remarks>
        public long MinutesRemainder { get { return (ticks % NodaConstants.TicksPerHour) / NodaConstants.TicksPerMinute; } }

        /// <summary>
        /// The number of hours within an day in this duration, in the range [-23, 23].
        /// </summary>
        /// <remarks>
        /// A negative duration will always return a negative value, rounded towards zero. So
        /// a value of -36 hours would give -12 for HoursRemainder, and -1 for <see cref="StandardDays" />.
        /// </remarks>
        public long HoursRemainder { get { return (ticks % NodaConstants.TicksPerStandardDay) / NodaConstants.TicksPerHour; } }

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
            return TotalTicks.GetHashCode();
        }

        // TODO(Post-V1): We should *consider* representing this as in the same way as a period.
        /// <summary>
        /// Gets the value as a <see cref="String"/> showing the number of ticks represented by this duration.
        /// </summary>
        /// <returns>A string representation of this duration.</returns>
        public override string ToString()
        {
            return TotalTicks.ToString();
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
            return new Duration(left.TotalTicks + right.TotalTicks);
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
            return new Duration(left.TotalTicks - right.TotalTicks);
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
        public Duration Minus(Duration other)
        {
            return this - other;
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the duration divided by the scale.</returns>
        public static Duration operator /(Duration left, long right)
        {
            return new Duration(left.TotalTicks / right);
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
            return new Duration(left.TotalTicks * right);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration"/> representing the duration multiplied by the scale.</returns>
        public static Duration operator *(long left, Duration right)
        {
            return new Duration(left * right.TotalTicks);
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

        /// <summary>
        /// Implements the unary negation operator.
        /// </summary>
        /// <param name="duration">Duration to negate</param>
        /// <returns>The negative value of this duration</returns>
        public static Duration operator -(Duration duration)
        {
            return new Duration(-duration.TotalTicks);
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
            return TotalTicks.CompareTo(other.TotalTicks);
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
            return TotalTicks == other.TotalTicks;
        }
        #endregion

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of standard weeks made
        /// up from 7 24-hour days.
        /// </summary>
        /// <param name="weeks">The number of weeks.</param>
        /// <returns>A <see cref="Duration"/> number of weeks.</returns>
        public static Duration FromStandardWeeks(long weeks)
        {
            return OneStandardWeek * weeks;
        }

        /// <summary>
        /// Returns a <see cref="Duration"/> that represents the given number of standard days made
        /// up from 24 hours.
        /// </summary>
        /// <param name="days">The number of days.</param>
        /// <returns>A <see cref="Duration"/> number of days.</returns>
        public static Duration FromStandardDays(long days)
        {
            return OneStandardDay * days;
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
        /// Returns a <see cref="TimeSpan"/> that represents the same number of ticks as thi
        /// <see cref="Duration"/>.
        /// </summary>
        /// <returns>A new TimeSpan with the same number of ticks as this Duration.</returns>
        public TimeSpan ToTimeSpan()
        {
            return new TimeSpan(ticks);
        }
    }
}