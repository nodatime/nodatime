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
using NodaTime.Fields;
using NodaTime.Format;
using System;

namespace NodaTime.Periods
{
    /// <summary>
    /// An immutable time period representing a number of seconds.
    /// <para>
    /// <code>Seconds</code> is an immutable period that can only store seconds.
    /// It does not store years, months or hours for example. As such it is a
    /// type-safe way of representing a number of seconds in an application.
    /// </para>
    /// <para>
    /// The number of seconds is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// <code>Add()</code>, <code>Subtract()</code>, <code>Multiply()</code> and
    /// <code>Divide()</code>.
    /// </para>
    /// <para>
    /// <code>Seconds</code> is thread-safe and immutable.
    /// </para>
    /// </summary>
    public sealed class Seconds : SingleFieldPeriodBase, IEquatable<Seconds>, IComparable<Seconds>
    {
        #region Static Properties

        private static readonly Seconds zero = new Seconds(0);
        private static readonly Seconds one = new Seconds(1);
        private static readonly Seconds two = new Seconds(2);
        private static readonly Seconds three = new Seconds(3);

        private static readonly Seconds maxValue = new Seconds(int.MaxValue);
        private static readonly Seconds minValue = new Seconds(int.MinValue);

        /// <summary>
        /// Gets a period representing zero seconds
        /// </summary>
        public static Seconds Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one seconds
        /// </summary>
        public static Seconds One { get { return one; } }

        /// <summary>
        /// Gets a period representing two seconds
        /// </summary>
        public static Seconds Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three seconds
        /// </summary>
        public static Seconds Three { get { return three; } }

        /// <summary>
        /// Gets a period representing the maximum number of seconds that can be stored in this object.
        /// </summary>
        public static Seconds MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of seconds that can be stored in this object.
        /// </summary>
        public static Seconds MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Seconds</code> that may be cached.
        /// </summary>
        /// <param name="seconds">The number of seconds to obtain an instance for</param>
        /// <returns>The instance of Seconds</returns>
        /// <remarks>
        /// <code>Seconds</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Seconds From(int seconds)
        {
            switch (seconds)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default:
                    return new Seconds(seconds);
            }
        }

        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Seconds);

        /// <summary>
        /// Creates a new <code>Minutes</code> by parsing a string in the ISO8601 format 'PTnS'.
        /// </summary>
        /// <param name="seconds">The period string, null returns zero</param>
        /// <returns>The period in seconds</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// seconds component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Seconds Parse(string seconds)
        {
            if (String.IsNullOrEmpty(seconds))
            {
                return Seconds.Zero;
            }

            Period p = parser.Parse(seconds);
            return Seconds.From(p.Seconds);
        }

        #endregion

        private Seconds(int value) : base(value) { }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Seconds</code>.
        /// </summary>
        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Seconds; }
        }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Seconds</code>.
        /// </summary>
        public override PeriodType PeriodType
        {
            get { return PeriodType.Seconds; }
        }

        #region ToStandart

        /// <summary>
        /// Converts this period in seconds to a period in weeks assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days
        /// long, all days are 24 hours long, all hours are 60 minutes long and
        /// all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of whole weeks for this number of seconds</returns>
        public Weeks ToStandardWeeks()
        {
            return Weeks.From(Value / NodaConstants.SecondsPerWeek);
        }

        /// <summary>
        /// Converts this period in seconds to a period in days assuming a
        /// 24 hour day, 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all days are 24 hours long, 
        /// all hours are 60 minutes long and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of whole days for this number of seconds</returns>
        public Days ToStandardDays()
        {
            return Days.From(Value / NodaConstants.SecondsPerDay);
        }

        /// <summary>
        /// Converts this period in seconds to a period in hours assuming a
        /// 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all hours are 60 minutes long 
        /// and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of whole hours for this number of seconds</returns>
        public Hours ToStandardHours()
        {
            return Hours.From(Value / NodaConstants.SecondsPerHour);
        }

        /// <summary>
        /// Converts this period in seconds to a period in minutes assuming a
        /// 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of whole minutes for this number of seconds</returns>
        public Minutes ToStandardMinutes()
        {
            return Minutes.From(Value / NodaConstants.SecondsPerMinute);
        }

        /// <summary>
        /// Converts this period in seconds to a duration in milliseconds assuming a
        /// 7 day week, 24 hour day, 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days
        /// long, all days are 24 hours long, all hours are 60 minutes long and
        /// all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A duration equivalent to this number of seconds</returns>
        public Duration ToStandardDuration()
        {
            return new Duration(Value*NodaConstants.MillisecondsPerSecond);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Seconds"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Seconds"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Seconds period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Seconds"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Seconds"/> period instance</param>
        /// <returns>New <see cref="Seconds"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Seconds(int value)
        {
            return Seconds.From(value);
        }

        #endregion

        #region Negation

        /// <summary>
        /// Returns a new instance with the seconds value negated.
        /// </summary>
        /// <returns>The new seconds period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Seconds Negated()
        {
            return Seconds.From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Seconds"/> instance with a negated value.</returns>
        public static Seconds operator -(Seconds period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given days period. Friendly alternative to unary <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Seconds"/> instance with a negated value.</returns>
        public static Seconds Negate(Seconds period)
        {
            return -period;
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Seconds"/> instance</returns>
        public static Seconds operator +(Seconds period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Seconds"/> instance with incremented value.</returns>
        public static Seconds operator ++(Seconds period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Seconds"/> instance with decremented value.</returns>
        public static Seconds operator --(Seconds period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Subtract(1);
        }

        #endregion

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of seconds added.
        /// </summary>
        /// <param name="seconds">The amount of seconds to add, may be negative</param>
        /// <returns>The new period plus the specified number of seconds</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Seconds Add(int seconds)
        {
            return seconds == 0 ? this : Seconds.From(Value + seconds);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the sum of the given values.</returns>
        public static Seconds operator +(Seconds left, Seconds right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one Hours instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the sum of the given values.</returns>
        public static Seconds Add(Seconds left, Seconds right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of seconds taken away.
        /// </summary>
        /// <param name="seconds">The amount of seconds to take away, may be negative</param>
        /// <returns>The new period minus the specified number of seconds</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Seconds Subtract(int seconds)
        {
            return Add(-seconds);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the difference of the given values.</returns>
        public static Seconds operator -(Seconds left, Seconds right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one hours period from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the difference of the given values.</returns>
        public static Seconds Subtract(Seconds left, Seconds right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the minutes multiplied by the specified scalar.
        /// </summary>
        /// <param name="seconds">The amount to multiply by, may be negative</param>
        /// <returns>The new seconds period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Seconds Multiply(int minutes)
        {
            return minutes == 1 ? this : Seconds.From(Value * minutes);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the multiplication of the given values.</returns>
        public static Seconds operator *(Seconds left, Seconds right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one minutes period by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> instance representing the multiplication of the given values.</returns>
        public static Seconds Multiply(Seconds left, Seconds right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the seconds divided by the specified divisor.
        /// </summary>
        /// <param name="seconds">The amount to divide by, may be negative</param>
        /// <returns>The new seconds period divided by the specified divisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Seconds Divide(int minutes)
        {
            return minutes == 1 ? this : Seconds.From(Value / minutes);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> representing the divison of the given values.</returns>
        public static Seconds operator /(Seconds left, Seconds right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one minutes period by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Seconds"/> instance representing the division of the given values.</returns>
        public static Seconds Divide(Seconds left, Seconds right)
        {
            return left / right;
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Indicates whether the current period is equal to another period.
        /// </summary>
        /// <param name="other">Another period to compare with this period.</param>
        /// <returns>
        /// True if the current period is equal to the <paramref name="other"/> parameter;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Seconds other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// Compares the current period with another period.
        /// </summary>
        /// <param name="other">Another period to compare with this object.</param>
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
        /// <description>This period is less than the <paramref name="other"/> period.</description>
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
        public int CompareTo(Seconds other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Seconds left, Seconds right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Seconds left, Seconds right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Seconds left, Seconds right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Seconds left, Seconds right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Seconds left, Seconds right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Seconds left, Seconds right)
        {
            return SingleFieldPeriodBase.Compare(left, right) >= 0;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "PT" + Value + "S";
        }

        #endregion
    }
}
