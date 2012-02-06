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
using NodaTime.Globalization;
using NodaTime.Text.Patterns;

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
    public struct Instant : IEquatable<Instant>, IComparable<Instant>, IFormattable, IComparable
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

        #region IComparable<Instant> and IComparable Members
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
        public int CompareTo(Instant other)
        {
            return Ticks.CompareTo(other.Ticks);
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
            if (!(obj is Instant))
            {
                throw new ArgumentException("Argument did not refer to an instance of NodaTime.Instant.", "obj");
            }
            return CompareTo((Instant)obj);
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
            return new LocalInstant(Ticks + offset.TotalTicks);
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
        /// Returns a new instant corresponding to the given UTC date and time in the ISO calendar.
        /// In most cases applications should use <see cref="ZonedDateTime" /> to represent a date
        /// and time, but this method is useful in some situations where an <see cref="Instant" /> is
        /// required, such as time zone testing.
        /// </summary>
        /// <param name="year">Year of the instant to return.</param>
        /// <param name="monthOfYear">Month of year of the instant to return.</param>
        /// <param name="dayOfMonth">Day of month of the instant to return.</param>
        /// <param name="hourOfDay">Hour of day of the instant to return.</param>
        /// <param name="minuteOfHour">Minute of hour of the instant to return.</param>
        /// <returns>The instant representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour);
            return new Instant(local.Ticks);
        }

        /// <summary>
        /// Returns a new instant corresponding to the given UTC date and
        /// time in the ISO calendar. In most cases applications should 
        /// use <see cref="ZonedDateTime" />
        /// to represent a date and time, but this method is useful in some 
        /// situations where an Instant is required, such as time zone testing.
        /// </summary>
        /// <param name="year">Year of the instant to return.</param>
        /// <param name="monthOfYear">Month of year of the instant to return.</param>
        /// <param name="dayOfMonth">Day of month of the instant to return.</param>
        /// <param name="hourOfDay">Hour of day of the instant to return.</param>
        /// <param name="minuteOfHour">Minute of hour of the instant to return.</param>
        /// <param name="secondOfMinute">Second of minute of the instant to return.</param>
        /// <returns>The instant representing the given date and time in UTC and the ISO calendar.</returns>
        public static Instant FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            var local = CalendarSystem.Iso.GetLocalInstant(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute);
            return new Instant(local.Ticks);
        }

        /// <summary>
        /// Returns the later instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The later instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Max(Instant x, Instant y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the earlier instant of the given two.
        /// </summary>
        /// <param name="x">The first instant to compare.</param>
        /// <param name="y">The second instant to compare.</param>
        /// <returns>The earlier instant of <paramref name="x"/> or <paramref name="y"/>.</returns>
        public static Instant Min(Instant x, Instant y)
        {
            return x < y ? x : y;
        }
        #endregion

        #region Formatting
        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use.
        ///   -or- 
        ///   null to use the default pattern defined for the type of the <see cref="T:System.IFormattable" /> implementation. 
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the numeric format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return InstantPattern.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance. Equivalent to
        ///   calling <c>ToString(null)</c>.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InstantPattern.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use.
        ///   -or- 
        ///   null to use the default pattern defined for the type. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText)
        {
            return InstantPattern.Format(this, patternText, NodaFormatInfo.CurrentInfo);
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
            return InstantPattern.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        private static readonly string[] AllPatterns = { "g", "n" };
        private const string DefaultFormatPattern = "g";

        private static readonly PatternBclSupport<Instant> InstantPattern = new PatternBclSupport<Instant>(AllPatterns, DefaultFormatPattern, Instant.MinValue, fi => fi.InstantPatternParser);
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <returns>The parsed instant.</returns>
        public static Instant Parse(string value)
        {
            return InstantPattern.Parse(value, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parses the given string using the specified format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed instant.</returns>
        public static Instant Parse(string value, IFormatProvider formatProvider)
        {
            return InstantPattern.Parse(value, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified pattern and format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed instant.</returns>
        public static Instant ParseExact(string value, string patternText, IFormatProvider formatProvider)
        {
            return InstantPattern.ParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified patterns and format provider.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <returns>The parsed instant.</returns>
        public static Instant ParseExact(string value, string[] patterns, IFormatProvider formatProvider)
        {
            return InstantPattern.ParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="result">The parsed instant, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out Instant result)
        {
            return InstantPattern.TryParse(value, NodaFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed instant, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out Instant result)
        {
            return InstantPattern.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified pattern and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patternText">The text of the pattern to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed instant, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string patternText, IFormatProvider formatProvider, out Instant result)
        {
            return InstantPattern.TryParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified patterns and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Instant.MinValue"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="patterns">The patterns to use for parsing.</param>
        /// <param name="formatProvider">The format provider to use for culture-specific settings.</param>
        /// <param name="result">The parsed instant, when successful.</param>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string[] patterns, IFormatProvider formatProvider, out Instant result)
        {
            return InstantPattern.TryParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider), out result);
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
        /// <returns>A <see cref="DateTime"/> representing the same instant in time as this value, with a kind of "universal".</returns>
        public DateTime ToDateTimeUtc()
        {
            return new DateTime(ticks + NodaConstants.DateTimeEpochTicks, DateTimeKind.Utc);
        }

        /// <summary>
        /// Constructs a <see cref="DateTimeOffset"/> from this Instant which has an offset of zero.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> representing the same instant in time as this value.</returns>
        public DateTimeOffset ToDateTimeOffset()
        {
            return new DateTimeOffset(ticks + NodaConstants.DateTimeEpochTicks, TimeSpan.Zero);
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> into a new Instant representing the same instant in time.
        /// </summary>
        /// <returns>An <see cref="Instant"/> value representing the same instant in time as the given universal <see cref="DateTime"/>.</returns>
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