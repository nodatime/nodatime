// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;
using NodaTime.Text;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// An offset from UTC in seconds. A positive value means that the local time is
    /// ahead of UTC (e.g. for Europe); a negative value means that the local time is behind
    /// UTC (e.g. for America).
    /// </summary>
    /// <remarks>
    /// Offsets are always strictly less than 24 hours (as either a positive or negative offset).
    /// </remarks>
    /// <threadsafety>This type is an immutable value type. See the thread safety section of the user guide for more information.</threadsafety>
#if !PCL
    [Serializable]
#endif
    public struct Offset : IEquatable<Offset>, IComparable<Offset>, IFormattable, IComparable, IXmlSerializable
#if !PCL
        , ISerializable
#endif
    {
        /// <summary>
        /// An offset of zero seconds - effectively the permanent offset for UTC.
        /// </summary>
        public static readonly Offset Zero = FromSeconds(0);
        /// <summary>
        /// The minimum permitted offset; one second less than a standard day before UTC.
        /// </summary>
        public static readonly Offset MinValue = FromSeconds(-NodaConstants.SecondsPerStandardDay + 1);
        /// <summary>
        /// The maximum permitted offset; one second less than a standard day after UTC.
        /// </summary>
        public static readonly Offset MaxValue = FromSeconds(NodaConstants.SecondsPerStandardDay - 1);

        private readonly int seconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="Offset" /> struct.
        /// </summary>
        /// <remarks>
        /// Offsets are constrained to the range (-24 hours, 24 hours).
        /// </remarks>
        /// <param name="seconds">The number of seconds in the offset.</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        private Offset(int seconds)
        {
            Preconditions.CheckArgumentRange("seconds", seconds,
                -NodaConstants.SecondsPerStandardDay + 1, NodaConstants.SecondsPerStandardDay - 1);
            this.seconds = seconds;
        }

        /// <summary>
        /// Gets the number of seconds represented by this offset, which may be negative.
        /// </summary>
        public int Seconds { get { return seconds; } }

        /// <summary>
        /// Gets the number of milliseconds represented by this offset, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the number of seconds is simply multiplied
        /// by 1,000 to give the number of milliseconds.
        /// </remarks>
        public int Milliseconds { get { return unchecked(seconds * NodaConstants.MillisecondsPerSecond); } }

        /// <summary>
        /// Gets the number of ticks represented by this offset, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the number of seconds is simply multiplied
        /// by 10,000,000 to give the number of ticks.
        /// </remarks>
        /// <value>The number of ticks.</value>
        public long Ticks { get { return unchecked(seconds * NodaConstants.TicksPerSecond); } }

        /// <summary>
        /// Gets the number of nanoseconds represented by this offset, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the number of seconds is simply multiplied
        /// by 1,000,000,000 to give the number of nanoseconds.
        /// </remarks>
        /// <value>The number of nanoseconds.</value>
        public long Nanoseconds { get { return unchecked(seconds * NodaConstants.NanosecondsPerSecond); } }

        /// <summary>
        /// Returns the greater offset of the given two, i.e. the one which will give a later local
        /// time when added to an instant.
        /// </summary>
        /// <param name="x">The first offset</param>
        /// <param name="y">The second offset</param>
        /// <returns>The greater offset of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static Offset Max(Offset x, Offset y)
        {
            return x > y ? x : y;
        }

        /// <summary>
        /// Returns the lower offset of the given two, i.e. the one which will give an earlier local
        /// time when added to an instant.
        /// </summary>
        /// <param name="x">The first offset</param>
        /// <param name="y">The second offset</param>
        /// <returns>The lower offset of <paramref name="x"/> and <paramref name="y"/>.</returns>
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
            return FromSeconds(-offset.Seconds);
        }

        /// <summary>
        /// Returns the negation of the specified offset. This is the method form of the unary minus operator.
        /// </summary>
        /// <param name="offset">The offset to negate.</param>
        /// <returns>The negation of the specified offset.</returns>
        public static Offset Negate(Offset offset)
        {
            return -offset;
        }

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="offset">The operand.</param>
        /// <remarks>There is no method form of this operator; the <see cref="Plus"/> method is an instance
        /// method for addition, and is more useful than a method form of this would be.</remarks>
        /// <returns>The same <see cref="Offset" /> instance</returns>
        public static Offset operator +(Offset offset)
        {
            return offset;
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>A new <see cref="Offset" /> representing the sum of the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset operator +(Offset left, Offset right)
        {
            return FromSeconds(left.Seconds + right.Seconds);
        }

        /// <summary>
        /// Adds one Offset to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>A new <see cref="Offset" /> representing the sum of the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset Add(Offset left, Offset right)
        {
            return left + right;
        }

        /// <summary>
        /// Returns the result of adding another Offset to this one, for a fluent alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="other">The offset to add</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>The result of adding the other offset to this one.</returns>
        [Pure]
        public Offset Plus(Offset other)
        {
            return this + other;
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="minuend">The left hand side of the operator.</param>
        /// <param name="subtrahend">The right hand side of the operator.</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>A new <see cref="Offset" /> representing the difference of the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset operator -(Offset minuend, Offset subtrahend)
        {
            return FromSeconds(minuend.Seconds - subtrahend.Seconds);
        }

        /// <summary>
        /// Subtracts one Offset from another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="minuend">The left hand side of the operator.</param>
        /// <param name="subtrahend">The right hand side of the operator.</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>A new <see cref="Offset" /> representing the difference of the given values.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset Subtract(Offset minuend, Offset subtrahend)
        {
            return minuend - subtrahend;
        }

        /// <summary>
        /// Returns the result of subtracting another Offset from this one, for a fluent alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="other">The offset to subtract</param>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        /// <returns>The result of subtracting the other offset from this one.</returns>
        [Pure]
        public Offset Minus(Offset other)
        {
            return this - other;
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
        public int CompareTo(Offset other)
        {
            return Seconds.CompareTo(other.Seconds);
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
            Preconditions.CheckArgument(obj is Offset, "obj", "Object must be of type NodaTime.Offset.");
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
            return Seconds == other.Seconds;
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
            return Seconds.GetHashCode();
        }
        #endregion  // Object overrides

        #region Formatting
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// The value of the current instance in the default format pattern ("g"), using the current thread's
        /// culture to obtain a format provider.
        /// </returns>
        public override string ToString()
        {
            return OffsetPattern.BclSupport.Format(this, null, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Formats the value of the current instance using the specified pattern.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> containing the value of the current instance in the specified format.
        /// </returns>
        /// <param name="patternText">The <see cref="T:System.String" /> specifying the pattern to use,
        /// or null to use the default format pattern ("g").
        /// </param>
        /// <param name="formatProvider">The <see cref="T:System.IFormatProvider" /> to use when formatting the value,
        /// or null to use the current thread's culture to obtain a format provider.
        /// </param>
        /// <filterpriority>2</filterpriority>
        public string ToString(string patternText, IFormatProvider formatProvider)
        {
            return OffsetPattern.BclSupport.Format(this, patternText, formatProvider);
        }
        #endregion Formatting

        #region Construction
        /// <summary>
        /// Returns an offset for the given seconds value, which may be negative.
        /// </summary>
        /// <param name="seconds">The int seconds value.</param>
        /// <returns>An offset representing the given number of seconds.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromSeconds(int seconds)
        {
            return new Offset(seconds);
        }

        /// <summary>
        /// Returns an offset for the given milliseconds value, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the given number of milliseconds is simply divided
        /// by 1,000 to give the number of seconds - any remainder is truncated.
        /// </remarks>
        /// <param name="milliseconds">The int milliseconds value.</param>
        /// <returns>An offset representing the given number of milliseconds, to the (truncated) second.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromMilliseconds(int milliseconds)
        {
             return new Offset((int) milliseconds / NodaConstants.MillisecondsPerSecond);
        }

        /// <summary>
        /// Returns an offset for the given number of ticks, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the given number of ticks is simply divided
        /// by 10,000,000 to give the number of seconds - any remainder is truncated.
        /// </remarks>
        /// <param name="ticks">The number of ticks specifying the length of the new offset.</param>
        /// <returns>An offset representing the given number of ticks, to the (truncated) second.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromTicks(long ticks)
        {
            return new Offset((int)(ticks / NodaConstants.TicksPerSecond));
        }

        /// <summary>
        /// Returns an offset for the given number of nanoseconds, which may be negative.
        /// </summary>
        /// <remarks>
        /// Offsets are only accurate to second precision; the given number of nanoseconds is simply divided
        /// by 1,000,000,000 to give the number of seconds - any remainder is truncated towards zero.
        /// </remarks>
        /// <param name="nanoseconds">The number of nanoseconds specifying the length of the new offset.</param>
        /// <returns>An offset representing the given number of nanoseconds, to the (truncated) second.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromNanoseconds(long nanoseconds)
        {
            return new Offset((int) (nanoseconds / NodaConstants.NanosecondsPerSecond));
        }

        /// <summary>
        /// Returns an offset for the specified number of hours, which may be negative.
        /// </summary>
        /// <param name="hours">The number of hours to represent in the new offset.</param>
        /// <returns>An offset representing the given value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromHours(int hours)
        {
            return new Offset(hours * NodaConstants.SecondsPerHour);
        }

        /// <summary>
        /// Returns an offset for the specified number of hours and minutes.
        /// </summary>
        /// <remarks>
        /// The result simply takes the hours and minutes and converts each component into milliseconds
        /// separately. As a result, a negative offset should usually be obtained by making both arguments
        /// negative. For example, to obtain "three hours and ten minutes behind UTC" you might call
        /// <c>Offset.FromHoursAndMinutes(-3, -10)</c>.
        /// </remarks>
        /// <param name="hours">The number of hours to represent in the new offset.</param>
        /// <param name="minutes">The number of minutes to represent in the new offset.</param>
        /// <returns>An offset representing the given value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        public static Offset FromHoursAndMinutes(int hours, int minutes)
        {
            return new Offset(hours * NodaConstants.SecondsPerHour + minutes * NodaConstants.SecondsPerMinute);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Converts this offset to a .NET standard <see cref="TimeSpan" /> value.
        /// </summary>
        /// <returns>An equivalent <see cref="TimeSpan"/> to this value.</returns>
        [Pure]
        public TimeSpan ToTimeSpan()
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Converts the given <see cref="TimeSpan"/> to an offset, with fractional seconds truncated.
        /// </summary>
        /// <param name="timeSpan">The timespan to convert</param>
        /// <exception cref="ArgumentOutOfRangeException">The given time span falls outside the range of +/- 24 hours.</exception>
        /// <returns>An offset for the same time as the given time span.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The result of the operation is outside the range of Offset.</exception>
        internal static Offset FromTimeSpan(TimeSpan timeSpan)
        {
            long seconds = (long) timeSpan.TotalSeconds;
            Preconditions.CheckArgumentRange("timeSpan", seconds, MinValue.Seconds, MaxValue.Seconds);
            return new Offset((int) seconds);
        }
        #endregion

        #region XML serialization
        /// <inheritdoc />
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <inheritdoc />
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Preconditions.CheckNotNull(reader, "reader");
            var pattern = OffsetPattern.GeneralInvariantPattern;
            string text = reader.ReadElementContentAsString();
            this = pattern.Parse(text).Value;
        }

        /// <inheritdoc />
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Preconditions.CheckNotNull(writer, "writer");
            writer.WriteString(OffsetPattern.GeneralInvariantPattern.Format(this));
        }
        #endregion

#if !PCL
        #region Binary serialization
        // Note that serialization is done via milliseconds, for compatibility with Noda Time 1.x.

        private const string MillisecondsSerializationName = "milliseconds";

        /// <summary>
        /// Private constructor only present for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to fetch data from.</param>
        /// <param name="context">The source for this deserialization.</param>
        private Offset(SerializationInfo info, StreamingContext context)
            : this(info.GetInt32(MillisecondsSerializationName) / NodaConstants.MillisecondsPerSecond)
        {
        }

        /// <summary>
        /// Implementation of <see cref="ISerializable.GetObjectData"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination for this serialization.</param>
        [System.Security.SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(MillisecondsSerializationName, Milliseconds);
        }
        #endregion
#endif
    }
}
