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
using NodaTime.Text;

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
        /// <summary>
        /// String used to represent "the beginning of time" (as far as Noda Time is concerned).
        /// </summary>
        public const string BeginningOfTimeLabel = "BOT";
        /// <summary>
        /// String used to represent "the end of time" (as far as Noda Time is concerned).
        /// </summary>
        public const string EndOfTimeLabel = "EOT";

        /// <summary>
        /// The instant at the Unix epoch of midnight 1st January 1970 UTC.
        /// </summary>
        public static readonly Instant UnixEpoch = new Instant(0);

        /// <summary>
        /// The minimum instant value, which is also used to represent the beginning of time.
        /// </summary>
        public static readonly Instant MinValue = new Instant(Int64.MinValue);
        /// <summary>
        /// The maximum instant value, which is also used to represent the end of time.
        /// </summary>
        public static readonly Instant MaxValue = new Instant(Int64.MaxValue);

        private readonly long ticks;

        /// <summary>
        /// Initializes a new instance of the <see cref="Instant" /> struct.
        /// </summary>
        /// <param name="ticks">The ticks from the unix epoch.</param>
        public Instant(long ticks)
        {
            this.ticks = ticks;
        }

        /// <summary>
        /// Ticks since the Unix epoch.
        /// </summary>
        public long Ticks { get { return ticks; } }

        #region IComparable<Instant> Members
        /// <summary>
        ///   Compares the current object with another object of the same type.
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
        public int CompareTo(Instant other)
        {
            return Ticks.CompareTo(other.Ticks);
        }
        #endregion

        #region Object overrides
        /// <summary>
        ///   Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance;
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
        ///   Implements the operator + (addition) for <see cref="Instant" /> + <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant operator +(Instant left, Duration right)
        {
            return new Instant(left.Ticks + right.Ticks);
        }

        /// <summary>
        ///   Adds the given offset to this instant, to return a <see cref="LocalInstant" />.
        /// </summary>
        /// <remarks>
        ///   This was previously an operator+ implementation, but operators can't be internal.
        /// </remarks>
        /// <param name="offset">The right hand side of the operator.</param>
        /// <returns>A new <see cref="LocalInstant" /> representing the sum of the given values.</returns>
        internal LocalInstant Plus(Offset offset)
        {
            return new LocalInstant(Ticks + offset.Ticks);
        }

        /// <summary>
        ///   Adds a duration to an instant. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant Add(Instant left, Duration right)
        {
            return left + right;
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Instant" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Duration operator -(Instant left, Instant right)
        {
            return new Duration(left.Ticks - right.Ticks);
        }

        /// <summary>
        ///   Implements the operator - (subtraction) for <see cref="Instant" /> - <see cref="Duration" />.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the sum of the given values.</returns>
        public static Instant operator -(Instant left, Duration right)
        {
            return new Instant(left.Ticks - right.Ticks);
        }

        /// <summary>
        ///   Subtracts one instant from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Duration" /> representing the difference of the given values.</returns>
        public static Duration Subtract(Instant left, Instant right)
        {
            return left - right;
        }

        /// <summary>
        ///   Subtracts a duration from an instant. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Instant" /> representing the difference of the given values.</returns>
        public static Instant Subtract(Instant left, Duration right)
        {
            return left - right;
        }

        /// <summary>
        ///   Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Instant left, Instant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Instant left, Instant right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Instant left, Instant right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///   Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Instant left, Instant right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///   Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Instant left, Instant right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///   Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Instant left, Instant right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion // Operators

        #region Convenience methods
        /// <summary>
        ///   Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
        ///   In most cases applications should use <see cref="ZonedDateTime" /> to represent a date
        ///   and time, but this method is useful in some situations where an <see cref="Instant" /> is
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
        ///   use <see cref="ZonedDateTime" />
        ///   to represent a date and time, but this method is useful in some 
        ///   situations where an Instant is required, such as time zone testing.
        /// </summary>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            return new Instant(local.Ticks);
        }

        /// <summary>
        /// Returns the later instant of the given two.
        /// </summary>
        public static Instant Max(Instant x, Instant y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the earlier instant of the given two.
        /// </summary>
        public static Instant Min(Instant x, Instant y)
        {
            return x < y ? x : y;
        }
        #endregion

        #region Formatting
        /// <summary>
        /// Compiles the given format pattern string and returns a formatter object that formats
        /// <see cref="Instant"/> objects using the given format and the thread's current culture.
        /// </summary>
        /// <param name="format">The format pattern string.</param>
        /// <param name="formatProvider"></param>
        /// <returns>An <see cref="INodaFormatter{T}"/> formatter object.</returns>
        public static INodaFormatter<Instant> GetFormatter(string format, IFormatProvider formatProvider)
        {
            return InstantFormat.MakeFormatter(format, formatProvider);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="format">The <see cref="T:System.String" /> specifying the format to use.
        ///   -or- 
        ///   null to use the default format defined for the type of the <see cref="T:System.IFormattable" /> implementation. 
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the numeric format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return InstantParser.Format(this, format, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance. Equivilent to
        ///   calling <c>ToString(null)</c>.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InstantParser.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="format">The <see cref="T:System.String" /> specifying the format to use.
        ///   -or- 
        ///   null to use the default format defined for the type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string format)
        {
            return InstantParser.Format(this, format, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified <see cref="IFormatProvider" />.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance.
        /// </returns>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the format information from the current locale setting of the current thread. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider formatProvider)
        {
            return InstantParser.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        private static readonly string[] AllFormats = { "g", "n" };
        private const string DefaultFormatPattern = "g";

        private static readonly PatternSupport<Instant> InstantParser = new PatternSupport<Instant>(AllFormats, DefaultFormatPattern, Instant.MinValue, fi => fi.InstantPatternParser);
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        public static Instant Parse(string value)
        {
            return InstantParser.Parse(value, NodaFormatInfo.CurrentInfo, ParseStyles.None);
        }

        /// <summary>
        /// Parses the given string using the specified format provider.
        /// </summary>
        public static Instant Parse(string value, IFormatProvider formatProvider)
        {
            return InstantParser.Parse(value, NodaFormatInfo.GetInstance(formatProvider), ParseStyles.None);
        }

        /// <summary>
        /// Parses the given string using the specified format provider and style.
        /// </summary>
        public static Instant Parse(string value, IFormatProvider formatProvider, ParseStyles styles)
        {
            return InstantParser.Parse(value, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        /// <summary>
        /// Parses the given string using the specified format pattern and format provider.
        /// </summary>
        public static Instant ParseExact(string value, string format, IFormatProvider formatProvider)
        {
            return InstantParser.ParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), ParseStyles.None);
        }

        /// <summary>
        /// Parses the given string using the specified format pattern, format provider and style.
        /// </summary>
        public static Instant ParseExact(string value, string format, IFormatProvider formatProvider, ParseStyles styles)
        {
            return InstantParser.ParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        /// <summary>
        /// Parses the given string using the specified format patterns, format provider and style.
        /// </summary>
        public static Instant ParseExact(string value, string[] formats, IFormatProvider formatProvider, ParseStyles styles)
        {
            return InstantParser.ParseExact(value, formats, NodaFormatInfo.GetInstance(formatProvider), styles);
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out Instant result)
        {
            return InstantParser.TryParse(value, NodaFormatInfo.CurrentInfo, ParseStyles.None, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format provider and style.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, ParseStyles styles, out Instant result)
        {
            return InstantParser.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format pattern, format provider and style.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string format, IFormatProvider formatProvider, ParseStyles styles, out Instant result)
        {
            return InstantParser.TryParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format patterns, format provider and style.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string[] formats, IFormatProvider formatProvider, ParseStyles styles, out Instant result)
        {
            return InstantParser.TryParseExact(value, formats, NodaFormatInfo.GetInstance(formatProvider), styles, out result);
        }
        #endregion Parsing

        #region IEquatable<Instant> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(Instant other)
        {
            return Ticks == other.Ticks;
        }
        #endregion

        /// <summary>
        /// Constructs a <see cref="DateTime"/> from this Instant which has a <see cref="DateTime.Kind" />
        /// of <see cref="DateTimeKind.Utc"/> and represents the same instant of time as this value.
        /// </summary>
        public DateTime ToDateTimeUtc()
        {
            return new DateTime(ticks + NodaConstants.DateTimeEpochTicks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> from this Instant which has an offset of zero.
        /// </summary>
        public DateTimeOffset ToDateTimeOffset()
        {
            return new DateTimeOffset(ticks + NodaConstants.DateTimeEpochTicks, TimeSpan.Zero);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> into a new Instant representing the same instant in time.
        /// </summary>
        /// <param name="dateTime">Date and time value which must have a <see cref="DateTime.Kind"/> of <see cref="DateTimeKind.Utc"/></param>
        /// <exception cref="ArgumentException"><paramref name="dateTime"/> has the wrong <see cref="DateTime.Kind"/>.</exception>
        public static Instant FromDateTimeUtc(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Invalid DateTime.Kind for Instant.FromDateTimeUtc", "dateTime");
            }
            return new Instant(dateTime.Ticks - NodaConstants.DateTimeEpochTicks);
        }
    }
}