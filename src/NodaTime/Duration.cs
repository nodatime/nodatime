#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
using System.IO;
using NodaTime.Format;

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
    public struct Duration : IEquatable<Duration>, IComparable<Duration>
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
        public static readonly Duration OneWeek = new Duration(NodaConstants.TicksPerWeek);

        /// <summary>
        /// Represents the <see cref="Duration"/> value equals to number of ticks in 1 day.
        /// This field is constant.
        /// </summary>
        /// <remarks>
        /// The value of this constant is 864 billion ticks; that is, 864,000,000,000 ticks.
        /// </remarks>
        public static readonly Duration OneDay = new Duration(NodaConstants.TicksPerDay);

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

        /// <summary>
        /// Gets the value as a <see cref="String"/> in the ISO8601 duration format including
        /// only seconds and ticks.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the instance as an ISO8601 string
        /// </returns>
        /// <example>
        /// For example, "PT72.3450000S" represents 1 minute, 12 seconds and 345 milliseconds.
        /// </example>
        public override string ToString()
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);
            writer.Write("PT");

            long seconds = Ticks / NodaConstants.TicksPerSecond;
            writer.Write(seconds);

            int ticksValue = (int)Math.Abs(Ticks % NodaConstants.TicksPerSecond);
            if (ticksValue > 0)
            {
                writer.Write(".");
                FormatUtils.WritePaddedInteger(writer, ticksValue, 7);
            }

            writer.Write("S");
            return writer.ToString();
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
        /// Converts the string representation of a duration to its <see cref="Duration"/> equivalent 
        /// and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">When this method returns, contains an object that represents the duration specified by value,
        /// or Duration.Zero if the conversion failed. This parameter is passed uninitialized.</param>
        /// <returns>True if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, out Duration result)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            result = Zero;

            int len = value.Length;
            if (len >= 4 && (value[0] == 'P' || value[0] == 'p') && (value[1] == 'T' || value[1] == 't') && (value[len - 1] == 'S' || value[len - 1] == 's'))
            {
                // ok
            }
            else
            {
                return false;
            }

            var body = value.Substring(2, len - 3);
            int dot = -1;
            for (int i = 0; i < body.Length; i++)
            {
                if ((body[i] >= '0' && body[i] <= '9') || (i == 0 && body[0] == '-'))
                {
                    // ok
                }
                else if (i > 0 && body[i] == '.' && dot == -1)
                {
                    // ok
                    dot = i;
                }
                else
                {
                    return false;
                }
            }

            long seconds;
            int ticks = 0;
            if (dot > 0)
            {
                seconds = long.Parse(body.Substring(0, dot), CultureInfo.InvariantCulture);
                var ticksValue = body.Substring(dot + 1);
                if (ticksValue.Length != 7)
                {
                    ticksValue = ticksValue + "0000000";
                }
                ticks = FormatUtils.ParseDigits(ticksValue, 0, 7);
            }
            else
            {
                seconds = long.Parse(body, CultureInfo.InvariantCulture);
            }

            result = seconds < 0 ? FromSeconds(seconds) - new Duration(ticks) : FromSeconds(seconds) + new Duration(ticks);

            return true;
        }

        /// <summary>
        /// Parses the specified value into a <see cref="Duration"/>.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <exception cref="FormatException">If the <paramref name="value"/> is badly formatted./**/</exception>
        /// <exception cref="ArgumentNullException">If the <paramref name="value"/> is <c>null</c>.</exception>
        /// <returns>The <see cref="Duration"/>.</returns>
        public static Duration Parse(string value)
        {
            Duration result;
            if (TryParse(value, out result))
            {
                return result;
            }
            throw new FormatException("Invalid format: \"" + value + '"');
        }
    }
}