#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
    /// An offset from UTC in ticks. (There are 10,000 ticks in a millisecond.)
    /// </summary>
    /// <remarks>
    /// <para>
    /// Offsets are constrained to the range (-24 hours, 24 hours). If the ticks value given is
    /// outside this range then the value is forced into the range by considering that time wraps as
    /// it goes around the world multiple times. 
    /// </para>
    /// <para>
    /// There is no concept of fields, such as days or seconds, as these fields can vary in length.
    /// A duration may be converted to an <see cref="IPeriod" /> to obtain field values. This
    /// conversion will typically cause a loss of precision.
    /// </para>
    /// <para>
    /// Internally, offsets are stored as an <see cref="Int32"/> number of milliseconds instead of
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
        public static readonly Offset Zero = new Offset(0L);
        public static readonly Offset MinValue = new Offset(-NodaConstants.MillisecondsPerDay + 1);
        public static readonly Offset MaxValue = new Offset(NodaConstants.MillisecondsPerDay - 1);

        private readonly int milliseconds;

        /// <summary>
        /// Gets the number of ticks in the offset.
        /// </summary>
        public long Ticks { get { return Milliseconds * NodaConstants.TicksPerMillisecond; } }

        /// <summary>
        /// Gets the number of milliseconds in the offset.
        /// </summary>
        public int Milliseconds { get { return milliseconds; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Offset"/> struct.
        /// </summary>
        /// <remarks>
        /// Offsets are constrained to the range (-24 hours, 24 hours). If the ticks value given is
        /// outside this range then the value is forced into the range by considering that time
        /// wraps as it goes around the world multiple times. Also the resolution of an offset is
        /// milliseconds so any values below that resolution are ignored.
        /// </remarks>
        /// <param name="ticks">The number of ticks.</param>
        public Offset(long ticks)
            : this((int)(ticks / NodaConstants.TicksPerMillisecond))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Offset"/> struct.
        /// </summary>
        /// <remarks>
        /// We keep this constructor private so there is no confusion as to whether we are
        /// initializing from ticks or milliseconds. As far as the outside world is concerned, this
        /// offset uses ticks.
        /// </remarks>
        /// <param name="milliseconds">The milliseconds.</param>
        private Offset(int milliseconds)
        {
            this.milliseconds = milliseconds % NodaConstants.MillisecondsPerDay;
        }

        /// <summary>
        /// Creates an offset with the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>
        /// A new <see cref="Offset"/> representing the given value.
        /// </returns>
        /// <remarks>
        /// TODO: not sure about the name. Anyone got a better one?
        /// </remarks>
        public static Offset Create(int hours)
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
            return new Offset((int)(
                (hours * NodaConstants.MillisecondsPerHour) +
                (minutes * NodaConstants.MillisecondsPerMinute) +
                (seconds * NodaConstants.MillisecondsPerSecond) +
                milliseconds
                ));
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
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Offset left, Offset right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Offset left, Offset right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Offset left, Offset right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Offset left, Offset right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Offset left, Offset right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
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
            return Milliseconds.CompareTo(other.Milliseconds);
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
                return Equals((Offset)obj);
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
            return Milliseconds.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            bool negative = Milliseconds < 0;
            int milliseconds = negative ? -Milliseconds : Milliseconds;
            int hours = milliseconds / NodaConstants.MillisecondsPerHour;
            int minutes = (milliseconds % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute;
            int seconds = (milliseconds % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond;
            milliseconds = milliseconds % NodaConstants.MillisecondsPerSecond;
            string sign = negative ? "-" : "+";
            return string.Format(CultureInfo.InvariantCulture, "{0}{1:D2}:{2:D2}:{3:D2}.{4:D3}", sign, hours, minutes, seconds, milliseconds);
        }

        #endregion  // Object overrides
    }
}
