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
using NodaTime.Fields;
using NodaTime.Format;

namespace NodaTime.Periods
{
    /// <summary>
    /// An immutable time period representing a number of minutes.
    /// <para>
    /// <code>Minutes</code> is an immutable period that can only store minutes.
    /// It does not store years, months or hours for example. As such it is a
    /// type-safe way of representing a number of minutes in an application.
    /// </para>
    /// <para>
    /// The number of minutes is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// <code>Add()</code>, <code>Subtract()</code>, <code>Multiply()</code> and
    /// <code>Divide()</code>.
    /// </para>
    /// <para>
    /// <code>Minutes</code> is thread-safe and immutable.
    /// </para>
    /// </summary>
    public sealed class Minutes : SingleFieldPeriodBase, IEquatable<Minutes>, IComparable<Minutes>
    {
        #region Static Properties

        private static readonly Minutes zero = new Minutes(0);
        private static readonly Minutes one = new Minutes(1);
        private static readonly Minutes two = new Minutes(2);
        private static readonly Minutes three = new Minutes(3);

        private static readonly Minutes maxValue = new Minutes(int.MaxValue);
        private static readonly Minutes minValue = new Minutes(int.MinValue);

        /// <summary>
        /// Gets a period representing zero minutes
        /// </summary>
        public static Minutes Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one minute
        /// </summary>
        public static Minutes One { get { return one; } }

        /// <summary>
        /// Gets a period representing two minutes
        /// </summary>
        public static Minutes Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three minutes
        /// </summary>
        public static Minutes Three { get { return three; } }

        /// <summary>
        /// Gets a period representing the maximum number of minutes that can be stored in this object.
        /// </summary>
        public static Minutes MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of minutes that can be stored in this object.
        /// </summary>
        public static Minutes MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Minutes</code> that may be cached.
        /// </summary>
        /// <param name="minutes">The number of minutes to obtain an instance for</param>
        /// <returns>The instance of Minutes</returns>
        /// <remarks>
        /// <code>Minutes</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Minutes From(int minutes)
        {
            switch (minutes)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default:
                    return new Minutes(minutes);
            }
        }

        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Minutes);

        /// <summary>
        /// Creates a new <code>Minutes</code> by parsing a string in the ISO8601 format 'PTnM'.
        /// </summary>
        /// <param name="minutes">The period string, null returns zero</param>
        /// <returns>The period in minutes</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// minutes component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Minutes Parse(string minutes)
        {
            if (String.IsNullOrEmpty(minutes))
            {
                return Minutes.Zero;
            }

            Period p = parser.Parse(minutes);
            return Minutes.From(p.Minutes);
        }

        #endregion

        private Minutes(int value) : base(value) { }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Minutes</code>.
        /// </summary>
        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Minutes; }
        }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Minutes</code>.
        /// </summary>
        public override PeriodType PeriodType
        {
            get { return PeriodType.Minutes; }
        }

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Minutes"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Minutes"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Minutes period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Minutes"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Minutes"/> period instance</param>
        /// <returns>New <see cref="Minutes"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Minutes(int value)
        {
            return Minutes.From(value);
        }

        #endregion

        #region Negation

        /// <summary>
        /// Returns a new instance with the minutes value negated.
        /// </summary>
        /// <returns>The new minutes period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Minutes Negated()
        {
            return Minutes.From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Minutes"/> instance with a negated value.</returns>
        public static Minutes operator -(Minutes period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given days period. Friendly alternative to unary <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Minutes"/> instance with a negated value.</returns>
        public static Minutes Negate(Minutes period)
        {
            return -period;
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Minutes"/> instance</returns>
        public static Minutes operator +(Minutes period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Minutes"/> instance with incremented value.</returns>
        public static Minutes operator ++(Minutes period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Minutes"/> instance with decremented value.</returns>
        public static Minutes operator --(Minutes period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Subtract(1);
        }

        #endregion

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of minutes added.
        /// </summary>
        /// <param name="minutes">The amount of minutes to add, may be negative</param>
        /// <returns>The new period plus the specified number of minutes</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Minutes Add(int minutes)
        {
            return minutes == 0 ? this : Minutes.From(Value + minutes);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the sum of the given values.</returns>
        public static Minutes operator +(Minutes left, Minutes right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one Hours instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the sum of the given values.</returns>
        public static Minutes Add(Minutes left, Minutes right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of minutes taken away.
        /// </summary>
        /// <param name="minutes">The amount of minutes to take away, may be negative</param>
        /// <returns>The new period minus the specified number of minutes</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Minutes Subtract(int hours)
        {
            return Add(-hours);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the difference of the given values.</returns>
        public static Minutes operator -(Minutes left, Minutes right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one hours period from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the difference of the given values.</returns>
        public static Minutes Subtract(Minutes left, Minutes right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the minutes multiplied by the specified scalar.
        /// </summary>
        /// <param name="minutes">The amount to multiply by, may be negative</param>
        /// <returns>The new minutes period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Minutes Multiply(int minutes)
        {
            return minutes == 1 ? this : Minutes.From(Value * minutes);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the multiplication of the given values.</returns>
        public static Minutes operator *(Minutes left, Minutes right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one minutes period by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> instance representing the multiplication of the given values.</returns>
        public static Minutes Multiply(Minutes left, Minutes right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the minutes divided by the specified divisor.
        /// </summary>
        /// <param name="minutes">The amount to divide by, may be negative</param>
        /// <returns>The new minutes period divided by the specified divisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Minutes Divide(int minutes)
        {
            return minutes == 1 ? this : Minutes.From(Value / minutes);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> representing the divison of the given values.</returns>
        public static Minutes operator /(Minutes left, Minutes right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one minutes period by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Minutes"/> instance representing the division of the given values.</returns>
        public static Minutes Divide(Minutes left, Minutes right)
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
        public bool Equals(Minutes other)
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
        public int CompareTo(Minutes other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Minutes left, Minutes right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Minutes left, Minutes right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Minutes left, Minutes right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Minutes left, Minutes right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Minutes left, Minutes right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Minutes left, Minutes right)
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
            return "PT" + Value + "M";
        }

        #endregion

    }
}
