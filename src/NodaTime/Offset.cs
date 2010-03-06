#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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
    /// An offset from UTC in milliseconds. A positive value means that the local time is
    /// ahead of UTC (e.g. for Europe); a negative value means that the local time is behind
    /// UTC (e.g. for America).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Offsets are constrained to the range (-24 hours, 24 hours). If the millisecond value given
    /// is outside this range then the value is forced into the range by considering that time wraps
    /// as it goes around the world multiple times.
    /// </para>
    /// <para>
    /// Internally, offsets are stored as an <see cref="int"/> number of milliseconds instead of
    /// as ticks. This is because as a description of the offset of a time zone from UTC, there is
    /// no offset of less than one second. Using milliseconds gives more than enough resolution and
    /// allows us to save 4 bytes per Offset.
    /// </para>
    /// <para>
    /// This type is immutable and thread-safe.
    /// </para>
    /// </remarks>
    public struct Offset
        : IEquatable<Offset>, IComparable<Offset>
    {
        private const string MinimalFormat = "M";
        private const string ShortFormat = "S";
        private const string LongFormat = "L";

        public static readonly Offset Zero = new Offset(0);
        public static readonly Offset MinValue = new Offset(-NodaConstants.MillisecondsPerDay + 1);
        public static readonly Offset MaxValue = new Offset(NodaConstants.MillisecondsPerDay - 1);

        private readonly int milliseconds;

        /// <summary>
        /// Gets the number of milliseconds in the offset.
        /// </summary>
        public int Milliseconds
        {
            get { return this.milliseconds; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Offset"/> struct.
        /// </summary>
        /// <remarks>
        /// Offsets are constrained to the range (-24 hours, 24 hours). If the millisecond value
        /// given is outside this range then the value is forced into the range by considering that
        /// time wraps as it goes around the world multiple times
        /// </remarks>
        /// <param name="milliseconds">The number of milliseconds.</param>
        public Offset(int milliseconds)
        {
            this.milliseconds = milliseconds % NodaConstants.MillisecondsPerDay;
        }

        /// <summary>
        /// Froms the ticks.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static Offset FromTicks(long ticks)
        {
            return new Offset((int) (ticks / NodaConstants.TicksPerMillisecond));
        }

        /// <summary>
        /// Creates an offset with the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>
        /// A new <see cref="Offset"/> representing the given value.
        /// </returns>
        public static Offset ForHours(int hours)
        {
            return Create(hours, 0, 0, 0);
        }

        /// <summary>
        /// Creates an offset with the specified number of hours and minutes.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <param name="minutes">The number of minutes.</param>
        /// <returns>
        /// A new <see cref="Offset"/> representing the given values.
        /// </returns>
        /// <remarks>
        /// TODO: not sure about the name. Anyone got a better one?
        /// </remarks>
        public static Offset Create(int hours, int minutes)
        {
            return Create(hours, minutes, 0, 0);
        }

        /// <summary>
        /// Creates an offset with the specified number of hours, minutes, and seconds.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <param name="minutes">The number of minutes.</param>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>
        /// A new <see cref="Offset"/> representing the given values.
        /// </returns>
        /// <remarks>
        /// TODO: not sure about the name. Anyone got a better one?
        /// </remarks>
        public static Offset Create(int hours, int minutes, int seconds)
        {
            return Create(hours, minutes, seconds, 0);
        }

        /// <summary>
        /// Creates an offset with the specified number of hours, minutes, seconds, and
        /// milliseconds.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <param name="minutes">The number of minutes.</param>
        /// <param name="seconds">The number of seconds.</param>
        /// <param name="milliseconds">The number of milliseconds.</param>
        /// <returns>
        /// A new <see cref="Offset"/> representing the given values.
        /// </returns>
        /// <remarks>
        /// TODO: not sure about the name. Anyone got a better one?
        /// </remarks>
        public static Offset Create(int hours, int minutes, int seconds, int milliseconds)
        {
            return new Offset((hours * NodaConstants.MillisecondsPerHour) +
                              (minutes * NodaConstants.MillisecondsPerMinute) +
                              (seconds * NodaConstants.MillisecondsPerSecond) +
                              milliseconds);
        }

        /// <summary>
        /// Returns the number of ticks represented by this offset.
        /// </summary>
        /// <returns>The number of ticks.</returns>
        public long AsTicks()
        {
            return this.Milliseconds * NodaConstants.TicksPerMillisecond;
        }

        #region Operators

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset"/> representing the sum of the given values.</returns>
        public static Offset operator +(Offset left, Offset right)
        {
            return new Offset(left.Milliseconds + right.Milliseconds);
        }

        /// <summary>
        /// Adds one Offset to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset"/> representing the sum of the given values.</returns>
        public static Offset Add(Offset left, Offset right)
        {
            return left + right;
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset"/> representing the difference of the given values.</returns>
        public static Offset operator -(Offset left, Offset right)
        {
            return new Offset(left.Milliseconds - right.Milliseconds);
        }

        /// <summary>
        /// Subtracts one Offset from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset"/> representing the difference of the given values.</returns>
        public static Offset Subtract(Offset left, Offset right)
        {
            return left - right;
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Offset left, Offset right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Offset left, Offset right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Offset left, Offset right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Offset left, Offset right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Offset left, Offset right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Offset left, Offset right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion // Operators

        #region IEquatable<Offset> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Offset other)
        {
            return this.Milliseconds == other.Milliseconds;
        }

        #endregion

        #region IComparable<Offset> Members

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
        public int CompareTo(Offset other)
        {
            return this.Milliseconds.CompareTo(other.Milliseconds);
        }

        #endregion

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
            if (obj is Offset)
            {
                return this.Equals((Offset) obj);
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
            return this.Milliseconds.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ToString(MinimalFormat);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format to use.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == ShortFormat)
            {
                return this.Format(FormatType.FormatLong);
            }
            if (format == LongFormat)
            {
                return this.Format(FormatType.FormatShort);
            }
            if (format == MinimalFormat)
            {
                return this.Format(FormatType.FormatMinimal);
            }
            throw new ArgumentOutOfRangeException("format", format, @"The format parameter is not valid: " + format);
        }
        #endregion  // Object overrides

        #region Format utilities

        private enum FormatType
        {
            FormatLong,
            FormatShort,
            FormatMinimal
        }

        /// <summary>
        /// Returns a string formatted version of this offset.
        /// </summary>
        /// <param name="formatType">Determines how much of the offset is forced to display.</param>
        /// <returns></returns>
        private string Format(FormatType formatType)
        {
            bool negative = this.Milliseconds < 0;
            int millisecondsValue = negative ? -this.Milliseconds : this.Milliseconds;
            int hours = millisecondsValue / NodaConstants.MillisecondsPerHour;
            int minutes = (millisecondsValue % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
            int seconds = (millisecondsValue % NodaConstants.MillisecondsPerMinute) /
                          NodaConstants.MillisecondsPerSecond;
            millisecondsValue = millisecondsValue % NodaConstants.MillisecondsPerSecond;
            string sign = negative ? "-" : "+";
            string pattern;
            if (formatType == FormatType.FormatLong || millisecondsValue != 0)
            {
                pattern = @"{0}{1:D}:{2:D2}:{3:D2}.{4:D3}";
            }
            else if (seconds != 0)
            {
                pattern = @"{0}{1:D}:{2:D2}:{3:D2}";
            }
            else if (formatType == FormatType.FormatShort || minutes != 0)
            {
                pattern = @"{0}{1:D}:{2:D2}";
            }
            else
            {
                pattern = @"{0}{1:D}";
            }
            return string.Format(CultureInfo.InvariantCulture, pattern, sign, hours, minutes, seconds, millisecondsValue);
        }

        #endregion // Format utilities
    }
}