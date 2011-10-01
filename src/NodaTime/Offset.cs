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
using NodaTime.Utility;

#endregion

namespace NodaTime
{
    /// <summary>
    ///   An offset from UTC in milliseconds. A positive value means that the local time is
    ///   ahead of UTC (e.g. for Europe); a negative value means that the local time is behind
    ///   UTC (e.g. for America).
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Offsets are constrained to the range (-24 hours, 24 hours). If the millisecond value given
    ///     is outside this range then an exception is thrown.
    ///   </para>
    ///   <para>
    ///     Internally, offsets are stored as an <see cref="int" /> number of milliseconds instead of
    ///     as ticks. This is because as a description of the offset of a time zone from UTC, there is
    ///     no offset of less than one second. Using milliseconds gives more than enough resolution and
    ///     allows us to save 4 bytes per Offset.
    ///   </para>
    ///   <para>
    ///     This type is immutable and thread-safe.
    ///   </para>
    /// </remarks>
    public struct Offset : IEquatable<Offset>, IComparable<Offset>, IFormattable, IComparable
    {
        /// <summary>
        /// An offset of zero ticks - effectively the permanent offset for UTC.
        /// </summary>
        public static readonly Offset Zero = Offset.FromMilliseconds(0);
        /// <summary>
        /// The minimum permitted offset; one millisecond less than a standard day before UTC.
        /// </summary>
        public static readonly Offset MinValue = Offset.FromMilliseconds(-NodaConstants.MillisecondsPerStandardDay + 1);
        /// <summary>
        /// The maximum permitted offset; one millisecond less than a standard day after UTC.
        /// </summary>
        public static readonly Offset MaxValue = Offset.FromMilliseconds(NodaConstants.MillisecondsPerStandardDay - 1);

        private readonly int milliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Offset" /> struct.
        /// </summary>
        /// <remarks>
        /// Offsets are constrained to the range (-24 hours, 24 hours).
        /// </remarks>
        /// <param name="milliseconds">The number of milliseconds in the offset.</param>
        private Offset(int milliseconds)
        {
            // TODO: Possibly move this to all the public factory methods instead, and trust internal callers.
            Preconditions.CheckArgumentRange("milliseconds", milliseconds,
                -NodaConstants.MillisecondsPerStandardDay + 1,
                NodaConstants.MillisecondsPerStandardDay - 1);
            this.milliseconds = milliseconds;
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is negative.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is negative; otherwise, <c>false</c>.
        /// </value>
        public bool IsNegative { get { return milliseconds < 0; } }

        /// <summary>
        ///   Gets the hours of the offset. This is always a positive value.
        /// </summary>
        public int Hours { get { return Math.Abs(milliseconds) / NodaConstants.MillisecondsPerHour; } }

        /// <summary>
        ///   Gets the minutes of the offset. This is always a positive value.
        /// </summary>
        public int Minutes { get { return (Math.Abs(milliseconds) % NodaConstants.MillisecondsPerHour) / NodaConstants.MillisecondsPerMinute; } }

        /// <summary>
        /// Gets the seconds of the offset. This is always a positive value.
        /// </summary>
        public int Seconds { get { return (Math.Abs(milliseconds) % NodaConstants.MillisecondsPerMinute) / NodaConstants.MillisecondsPerSecond; } }

        /// <summary>
        /// Gets the fractional seconds of the offset i.e. the milliseconds of the second. This is always a positive value.
        /// </summary>
        public int FractionalSeconds { get { return Math.Abs(milliseconds) % NodaConstants.MillisecondsPerSecond; } }

        /// <summary>
        /// Gets the total number of milliseconds in the offset.
        /// </summary>
        public int TotalMilliseconds { get { return milliseconds; } }

        /// <summary>
        /// Returns the number of ticks represented by this offset.
        /// </summary>
        /// <value>The number of ticks.</value>
        public long TotalTicks { get { return TotalMilliseconds * NodaConstants.TicksPerMillisecond; } }

        /// <summary>
        /// Returns the greater offset of the given two, i.e. the one which will give a later local
        /// time when added to an instant.
        /// </summary>
        public static Offset Max(Offset x, Offset y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the lower offset of the given two, i.e. the one which will give an earlier local
        /// time when added to an instant.
        /// </summary>
        public static Offset Min(Offset x, Offset y)
        {
            return x < y ? x : y;
        }

        #region Operators
        /// <summary>
        ///   Implements the unary operator - (negation).
        /// </summary>
        /// <param name="offset">The offset to negate.</param>
        /// <returns>A new <see cref="Offset" /> instance with a negated value.</returns>
        public static Offset operator -(Offset offset)
        {
            return Offset.FromMilliseconds(-offset.TotalMilliseconds);
        }

        /// <summary>
        /// Returns the negation of the specified offset. This is the method form of the unary minus operator.
        /// </summary>
        public static Offset Negate(Offset offset)
        {
            return -offset;
        }

        /// <summary>
        ///   Implements the unary operator + .
        /// </summary>
        /// <param name="offset">The operand.</param>
        /// <returns>The same <see cref="Offset" /> instance</returns>
        public static Offset operator +(Offset offset)
        {
            return offset;
        }

        /// <summary>
        /// Returns the specified offset. This is the method form of the unary plus operator.
        /// </summary>
        public static Offset Plus(Offset offset)
        {
            return offset;
        }

        /// <summary>
        ///   Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset" /> representing the sum of the given values.</returns>
        public static Offset operator +(Offset left, Offset right)
        {
            return Offset.FromMilliseconds(left.TotalMilliseconds + right.TotalMilliseconds);
        }

        /// <summary>
        ///   Adds one Offset to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset" /> representing the sum of the given values.</returns>
        public static Offset Add(Offset left, Offset right)
        {
            return left + right;
        }

        /// <summary>
        ///   Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset" /> representing the difference of the given values.</returns>
        public static Offset operator -(Offset left, Offset right)
        {
            return Offset.FromMilliseconds(left.TotalMilliseconds - right.TotalMilliseconds);
        }

        /// <summary>
        ///   Subtracts one Offset from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Offset" /> representing the difference of the given values.</returns>
        public static Offset Subtract(Offset left, Offset right)
        {
            return left - right;
        }

        /// <summary>
        ///   Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Offset left, Offset right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Offset left, Offset right)
        {
            return !(left == right);
        }

        /// <summary>
        ///   Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Offset left, Offset right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        ///   Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Offset left, Offset right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        ///   Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Offset left, Offset right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        ///   Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Offset left, Offset right)
        {
            return left.CompareTo(right) >= 0;
        }
        #endregion // Operators

        #region IComparable<Offset> Members
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
        public int CompareTo(Offset other)
        {
            return TotalMilliseconds.CompareTo(other.TotalMilliseconds);
        }

        /// <summary>
        /// Implementation of <see cref="IComparable.CompareTo"/> to compare two offsets.
        /// </summary>
        /// <remarks>
        /// This uses explicit interface implementation to avoid it being called accidentally. The generic implementation should usually be preferred.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="obj"/> is non-null but does not refer to an instance of <see cref="Offset"/>.</exception>
        /// <param name="obj">The object to compare this value with.</param>
        /// <returns>The result of comparing this instant with another one; see <see cref="CompareTo(NodaTime.Offset)"/> for general details.
        /// If <paramref name="obj"/> is null, this method returns a value greater than 0.
        /// </returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is Offset))
            {
                throw new ArgumentException("Argument did not refer to an instance of NodaTime.Offset.", "obj");
            }
            return CompareTo((Offset)obj);
        }
        #endregion

        #region IEquatable<Offset> Members
        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   true if the current object is equal to the <paramref name = "other" /> parameter;
        ///   otherwise, false.
        /// </returns>
        public bool Equals(Offset other)
        {
            return TotalMilliseconds == other.TotalMilliseconds;
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
            if (obj is Offset)
            {
                return Equals((Offset)obj);
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
            return TotalMilliseconds.GetHashCode();
        }
        #endregion  // Object overrides

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
            return OffsetPattern.Format(this, patternText, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        ///   Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///   A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return OffsetPattern.Format(this, null, NodaFormatInfo.CurrentInfo);
        }

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
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText)
        {
            return OffsetPattern.Format(this, patternText, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        ///   Formats the value of the current instance using the specified format.
        /// </summary>
        /// <returns>
        ///   A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use to format the value.
        ///   -or- 
        ///   null to obtain the format information from the current locale setting of the operating system. 
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(IFormatProvider formatProvider)
        {
            return OffsetPattern.Format(this, null, NodaFormatInfo.GetInstance(formatProvider));
        }
        #endregion Formatting

        #region Parsing
        private static readonly string[] AllPatterns = { "g", "n", "d" };
        private const string DefaultFormatPattern = "g";

        private static readonly PatternBclSupport<Offset> OffsetPattern = new PatternBclSupport<Offset>(AllPatterns, DefaultFormatPattern, Offset.Zero, fi => fi.OffsetPatternParser);
        /// <summary>
        /// Parses the given string using the current culture's default format provider.
        /// </summary>
        public static Offset Parse(string value)
        {
            return OffsetPattern.Parse(value, NodaFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Parses the given string using the specified format provider.
        /// </summary>
        public static Offset Parse(string value, IFormatProvider formatProvider)
        {
            return OffsetPattern.Parse(value, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified format pattern and format provider.
        /// </summary>
        public static Offset ParseExact(string value, string format, IFormatProvider formatProvider)
        {
            return OffsetPattern.ParseExact(value, format, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Parses the given string using the specified patterns and format provider.
        /// </summary>
        public static Offset ParseExact(string value, string[] patterns, IFormatProvider formatProvider)
        {
            return OffsetPattern.ParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider));
        }

        /// <summary>
        /// Attempts to parse the given string using the current culture's default format provider. If the parse is successful,
        /// the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Offset.Zero"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, out Offset result)
        {
            return OffsetPattern.TryParse(value, NodaFormatInfo.CurrentInfo, out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Offset.Zero"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParse(string value, IFormatProvider formatProvider, out Offset result)
        {
            return OffsetPattern.TryParse(value, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified pattern, format provider and style.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Offset.Zero"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string patternText, IFormatProvider formatProvider, out Offset result)
        {
            return OffsetPattern.TryParseExact(value, patternText, NodaFormatInfo.GetInstance(formatProvider), out result);
        }

        /// <summary>
        /// Attempts to parse the given string using the specified patterns and format provider.
        /// If the parse is successful, the result is stored in the <paramref name="result"/> parameter and the return value is true;
        /// otherwise <see cref="Offset.Zero"/> is stored in the parameter and the return value is false.
        /// </summary>
        /// <returns>true if the value was parsed successfully; false otherwise.</returns>
        public static bool TryParseExact(string value, string[] patterns, IFormatProvider formatProvider, out Offset result)
        {
            return OffsetPattern.TryParseExact(value, patterns, NodaFormatInfo.GetInstance(formatProvider), out result);
        }
        #endregion Parsing

        #region Construction
        /// <summary>
        ///   Returns the offset for the given milliseconds value.
        /// </summary>
        /// <param name="milliseconds">The int milliseconds value.</param>
        /// <returns>The <see cref="Offset" /> for the given milliseconds value</returns>
        public static Offset FromMilliseconds(int milliseconds)
        {
             return new Offset(milliseconds);
        }

        /// <summary>
        ///   Froms the ticks.
        /// </summary>
        /// <param name="ticks">The ticks.</param>
        /// <returns></returns>
        public static Offset FromTicks(long ticks)
        {
            return new Offset((int)(ticks / NodaConstants.TicksPerMillisecond));
        }

        /// <summary>
        ///   Creates an offset with the specified number of hours.
        /// </summary>
        /// <param name="hours">The number of hours.</param>
        /// <returns>
        ///   A new <see cref="Offset" /> representing the given value.
        /// </returns>
        public static Offset FromHours(int hours)
        {
            return new Offset(hours * NodaConstants.MillisecondsPerHour);
        }

        /// <summary>
        /// Creates an offset with the specified number of hours, minutes, seconds, and
        /// milliseconds. This offset is always non-negative.
        /// </summary>
        /// <param name="hours">The number of hours, in the range [0, 24).</param>
        /// <param name="minutes">The number of minutes, in the range [0, 60).</param>
        /// <param name="seconds">The number of second, in the range [0, 60).</param>
        /// <param name="fractionalSeconds">The number of milliseconds within the second,
        /// in the range [0, 1000).</param>
        /// <returns>
        ///   A new <see cref="Offset" /> representing the given values.
        /// </returns>
        /// <remarks>
        ///   TODO: not sure about the name. Anyone got a better one?
        ///   TODO: The behaviour around negative values needs documenting too! (Make this internal for now?)
        /// </remarks>
        public static Offset Create(int hours, int minutes, int seconds, int fractionalSeconds)
        {
            return Create(hours, minutes, seconds, fractionalSeconds, false);
        }

        /// <summary>
        /// Creates an offset from the given values, including a sign to indicate whether or not the returned
        /// offset should be negative.
        /// </summary>
        /// <param name="hours">The number of hours, in the range [0, 24).</param>
        /// <param name="minutes">The number of minutes, in the range [0, 60).</param>
        /// <param name="seconds">The number of second, in the range [0, 60).</param>
        /// <param name="fractionalSeconds">The number of milliseconds within the second,
        /// in the range [0, 1000).</param>
        /// <param name="negative">True if a negative offset should be created, false for a positive one.</param>
        public static Offset Create(int hours, int minutes, int seconds, int fractionalSeconds, bool negative)
        {
            Preconditions.CheckArgumentRange("hours", hours, 0, 23);
            Preconditions.CheckArgumentRange("minutes", minutes, 0, 59);
            Preconditions.CheckArgumentRange("seconds", seconds, 0, 59);
            Preconditions.CheckArgumentRange("fractionalSeconds", fractionalSeconds, 0, 999);
            int sign = negative ? -1 : 1;
            int milliseconds = 0;
            milliseconds += hours * NodaConstants.MillisecondsPerHour;
            milliseconds += minutes * NodaConstants.MillisecondsPerMinute;
            milliseconds += seconds * NodaConstants.MillisecondsPerSecond;
            milliseconds += fractionalSeconds;
            return Offset.FromMilliseconds(sign * milliseconds);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Converts this offset to a .NET standard <see cref="TimeSpan" /> value.
        /// </summary>
        public TimeSpan ToTimeSpan()
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }
        #endregion
    }
}
