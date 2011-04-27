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
#region usings
using System;
using NodaTime.Format;
using NodaTime.Globalization;
#endregion

namespace NodaTime
{
    /// <summary>
    ///   Represents an instant on the timeline, measured in ticks from the Unix epoch,
    ///   which is typically described as January 1st 1970, midnight, UTC (ISO calendar).
    ///   (There are 10,000 ticks in a millisecond.)
    /// </summary>
    /// <remarks>
    ///   This type is immutable and thread-safe.
    /// </remarks>
    public struct Instant : IEquatable<Instant>, IComparable<Instant>, IFormattable
    {
        public const string BeginningOfTimeLabel = "BOT";
        public const string EndOfTimeLabel = "EOT";

        public static readonly Instant UnixEpoch = new Instant(0);
        public static readonly Instant MinValue = new Instant(Int64.MinValue);
        public static readonly Instant MaxValue = new Instant(Int64.MaxValue);

        private readonly long ticks;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Instant" /> struct.
        /// </summary>
        /// <param name = "ticks">The ticks from the unix epoch.</param>
        public Instant(long ticks)
        {
            this.ticks = ticks;
        }

        /// <summary>
        ///   Ticks since the Unix epoch.
        /// </summary>
        public long Ticks { get { return ticks; } }

        #region IComparable<Instant> Members
        /// <summary>
        ///   Compares the current object with another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
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
        public int CompareTo(Instant other)
        {
            return Ticks.CompareTo(other.Ticks);
        }
        #endregion

        #region Object overrides
        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Instant)
            {
                return Equals((Instant)obj);
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
            return Ticks.GetHashCode();
        }
        #endregion  // Object overrides

        #region Operators
        /// <summary>
        ///   Implements the operator + (addition) for <see cref = "Instant" /> + <see cref = "Duration" />.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Instant" /> representing the sum of the given values.</returns>
        public static Instant operator +(Instant left, Duration right)
        {
            return new Instant(left.Ticks + right.Ticks);
        }

        /// <summary>
        ///   Adds the given offset to this instant, to return a <see cref = "LocalInstant" />.
        /// </summary>
        /// <remarks>
        ///   This was previously an operator+ implementation, but operators can't be internal.
        /// </remarks>
        /// <param name = "offset">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "LocalInstant" /> representing the sum of the given values.</returns>
        internal LocalInstant Plus(Offset offset)
        {
            return new LocalInstant(Ticks + offset.Ticks);
        }

        /// <summary>
        ///   Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Instant" /> representing the sum of the given values.</returns>
        public static Instant Add(Instant left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref = "Instant" /> - <see cref = "Instant" />.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Instant" /> representing the sum of the given values.</returns>
        public static Duration operator -(Instant left, Instant right)
        {
            return new Duration(left.Ticks - right.Ticks);
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref = "Instant" /> - <see cref = "Duration" />.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Instant" /> representing the sum of the given values.</returns>
        public static Instant operator -(Instant left, Duration right)
        {
            return new Instant(left.Ticks - right.Ticks);
        }

        /// <summary>
        ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Duration" /> representing the difference of the given values.</returns>
        public static Duration Subtract(Instant left, Instant right)
        {
            return left - right;
        }

        /// <summary>
        ///   Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns>A new <see cref = "Instant" /> representing the difference of the given values.</returns>
        public static Instant Subtract(Instant left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        ///   Implements the operator == (equality).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Instant left, Instant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   Implements the operator != (inequality).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Instant left, Instant right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Implements the operator &lt; (less than).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Instant left, Instant right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///   Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Instant left, Instant right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///   Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Instant left, Instant right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///   Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name = "left">The left hand side of the operator.</param>
        /// <param name = "right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Instant left, Instant right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion // Operators

        #region Convenience methods
        /// <summary>
        ///   Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
        ///   In most cases applications should use <see cref = "ZonedDateTime" /> to represent a date
        ///   and time, but this method is useful in some situations where an <see cref = "Instant" /> is
        ///   required, such as time zone testing.
        /// </summary>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            return new Instant(local.Ticks);
        }

        /// <summary>
        ///   Returns a new instant corresponding to the given UTC date and
        ///   time in the ISO calendar. In most cases applications should 
        ///   use <see cref = "ZonedDateTime" />
        ///   to represent a date and time, but this method is useful in some 
        ///   situations where an Instant is required, such as time zone testing.
        /// </summary>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            return new Instant(local.Ticks);
        }
        #endregion

        #region Formatting

        /// <summary>
        /// Compiles the given format pattern string and returns a formatter object that formats
        /// <see cref="Instant"/> objects using the given format and the thread's current culture.
        /// </summary>
        /// <param name="format">The format pattern string.</param>
        /// <returns>An <see cref="INodaFormatter{T}"/> formatter object.</returns>
        public static INodaFormatter<Instant> GetFormatter(string format)
        {
            return InstantFormat.MakeFormatter(format);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name = "format">The <see cref = "T:System.String" /> specifying the format to use.
        ///   -or- 
        ///   null to use the default format defined for the type of the <see cref = "T:System.IFormattable" /> implementation. 
        /// </param>
        /// <param name = "formatProvider">The <see cref = "T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the numeric format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return InstantFormat.Format(this, format, formatProvider);
        }

        /// <summary>
        ///   Returns a <see cref = "System.String" /> that represents this instance. Equivilent to
        ///   calling <c>ToString(null)</c>.
        /// </summary>
        /// <returns>
        ///   A <see cref = "System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InstantFormat.Format(this, null, null);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name = "format">The <see cref = "T:System.String" /> specifying the format to use.
        ///   -or- 
        ///   null to use the default format defined for the type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format)
        {
            return InstantFormat.Format(this, format, null);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified <see cref = "IFormatProvider" />.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.String" /> containing the value of the current instance.
        /// </returns>
        /// <param name = "formatProvider">The <see cref = "T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the format information from the current locale setting of the current thread. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider formatProvider)
        {
            return InstantFormat.Format(this, null, formatProvider);
        }
        #endregion Formatting

        #region Parsing
        public static Instant Parse(string value)
        {
            return InstantParse.Parse(value, NodaFormatInfo.CurrentInfo, DateTimeParseStyles.None);
        }

        public static Instant Parse(string value, IFormatProvider formatProvider)
        {
            return InstantParse.Parse(value, NodaFormatInfo.GetInstance(formatProvider), DateTimeParseStyles.None);
        }

        public static Instant Parse(string value, IFormatProvider formatProvider, DateTimeParseStyles styles)
        {
            return InstantParse.Parse(value, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        public static Instant ParseExact(string value, string format, IFormatProvider formatProvider)
        {
            return InstantParse.ParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), DateTimeParseStyles.None);
        }

        public static Instant ParseExact(string value, string format, IFormatProvider formatProvider, DateTimeParseStyles styles)
        {
            return InstantParse.ParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        public static Instant ParseExact(string value, string[] formats, IFormatProvider formatProvider, DateTimeParseStyles styles)
        {
            return InstantParse.ParseExact(value, formats, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        public static bool TryParse(string value, out Instant result)
        {
            return InstantParse.TryParse(value, NodaFormatInfo.CurrentInfo, DateTimeParseStyles.None, out result);
        }

        public static bool TryParse(string value, IFormatProvider formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            return InstantParse.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }

        public static bool TryParseExact(string value, string format, IFormatProvider formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            return InstantParse.TryParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }

        public static bool TryParseExact(string value, string[] formats, IFormatProvider formatProvider, DateTimeParseStyles styles, out Instant result)
        {
            return InstantParse.TryParseExactMultiple(value, formats, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }
        #endregion Parsing

        #region IEquatable<Instant> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name = "other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(Instant other)
        {
            return Ticks == other.Ticks;
        }
        #endregion
    }
}