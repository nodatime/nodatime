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
using NodaTime.Fields;
using NodaTime.Format;

namespace NodaTime.Periods
{
    /// <summary>
    /// An immutable time period representing a number of years.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <code>Years</code> is an immutable period that can only store years.
    /// It does not store years, days or hours for example. As such it is a
    /// type-safe way of representing a number of years in an application.
    /// </para>
    /// <para>
    /// The number of years is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// <code>Add()</code>, <code>Subtract()</code>, <code>multipliedBy()</code> and
    /// <code>dividedBy()</code>.
    /// </para>
    /// <para>
    /// <code>Years</code> is thread-safe and immutable.
    /// </para>
    /// </remarks>
    public sealed class Years : SingleFieldPeriodBase, IEquatable<Years>, IComparable<Years>
    {
        #region Static Properties and Methods
        private static readonly Years zero = new Years(0);
        private static readonly Years one = new Years(1);
        private static readonly Years two = new Years(2);
        private static readonly Years three = new Years(3);

        private static readonly Years maxValue = new Years(int.MaxValue);
        private static readonly Years minValue = new Years(int.MinValue);

        /// <summary>
        /// Gets a period representing zero years
        /// </summary>
        public static Years Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one year
        /// </summary>
        public static Years One { get { return one; } }

        /// <summary>
        /// Gets a period representing two years
        /// </summary>
        public static Years Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three years
        /// </summary>
        public static Years Three { get { return three; } }

        /// <summary>
        /// Gets a period representing the maximum number of years that can be stored in this object.
        /// </summary>
        public static Years MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of years that can be stored in this object.
        /// </summary>
        public static Years MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Years</code> that may be cached.
        /// </summary>
        /// <param name="years">The number of years to obtain an instance for</param>
        /// <returns>The instance of Years</returns>
        /// <remarks>
        /// <code>Years</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Years From(int years)
        {
            switch (years)
            {
                case 0:
                    return zero;
                case 1:
                    return one;
                case 2:
                    return two;
                case 3:
                    return three;
                case int.MaxValue:
                    return maxValue;
                case int.MinValue:
                    return minValue;
                default:
                    return new Years(years);
            }
        }

        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Years);

        /// <summary>
        /// Creates a new <code>Years</code> by parsing a string in the ISO8601 format 'PnY'.
        /// </summary>
        /// <param name="years">The period string, null returns zero</param>
        /// <returns>The period in years</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// years component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Years Parse(string years)
        {
            if (String.IsNullOrEmpty(years))
            {
                return Zero;
            }

            Period p = parser.Parse(years);
            return From(p.Years);
        }
        #endregion

        private Years(int years) : base(years)
        {
        }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Years</code>.
        /// </summary>
        public override DurationFieldType FieldType { get { return DurationFieldType.Years; } }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Years</code>.
        /// </summary>
        public override PeriodType PeriodType { get { return PeriodType.Years; } }

        #region Conversion
        /// <summary>
        /// Creates a new int from the specified <see cref="Years"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Years"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Years period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Years"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Years"/> period instance</param>
        /// <returns>New <see cref="Years"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Years(int value)
        {
            return From(value);
        }
        #endregion

        #region Negation
        /// <summary>
        /// Returns a new instance with the years value negated.
        /// </summary>
        /// <returns>The new years period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Years Negated()
        {
            return From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Years"/> instance with a negated value.</returns>
        public static Years operator -(Years period)
        {
            return ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given years period. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Years"/> instance with a negated value.</returns>
        public static Years Negate(Years period)
        {
            return -period;
        }
        #endregion

        #region Unary operators
        /// <summary>
        /// Returns the same instance. Friendly alternative for <c>Years.operator +(Years)</c> operator.
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Years"/> instance</returns>
        public static Years Plus(Years period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Years"/> instance</returns>
        public static Years operator +(Years period)
        {
            return period;
        }

        /// <summary>
        /// Increments the given period by 1. Friendly alternative for <c>Years.operator ++(Years)</c> operator.
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Years"/> instance with incremented value.</returns>
        public static Years Increment(Years period)
        {
            return ++period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Years"/> instance with incremented value.</returns>
        public static Years operator ++(Years period)
        {
            return ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Decrements the given period by 1. Friendly alternative for <c>Years.operator --(Years)</c> operator.
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Years"/> instance with decremented value.</returns>
        public static Years Decrement(Years period)
        {
            return --period;
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Years"/> instance with decremented value.</returns>
        public static Years operator --(Years period)
        {
            return ReferenceEquals(period, null) ? null : period.Subtract(1);
        }
        #endregion

        #region Add
        /// <summary>
        /// Returns a new instance with the specified number of years added.
        /// </summary>
        /// <param name="years">The amount of years to add, may be negative</param>
        /// <returns>The new period plus the specified number of years</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Years Add(int years)
        {
            return years == 0 ? this : From(Value + years);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the sum of the given values.</returns>
        public static Years operator +(Years left, Years right)
        {
            return ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one years instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the sum of the given values.</returns>
        public static Years Add(Years left, Years right)
        {
            return left + right;
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Returns a new instance with the specified number of years taken away.
        /// </summary>
        /// <param name="years">The amount of years to take away, may be negative</param>
        /// <returns>The new period minus the specified number of years</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Years Subtract(int years)
        {
            return Add(-years);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the difference of the given values.</returns>
        public static Years operator -(Years left, Years right)
        {
            return ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one years from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the difference of the given values.</returns>
        public static Years Subtract(Years left, Years right)
        {
            return left - right;
        }
        #endregion

        #region Multiplication
        /// <summary>
        /// Returns a new instance with the years multiplied by the specified scalar.
        /// </summary>
        /// <param name="scalar">The amount to multiply by, may be negative</param>
        /// <returns>The new period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Years Multiply(int scalar)
        {
            return scalar == 1 ? this : From(Value * scalar);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the years period multiplied by the scale.</returns>
        public static Years operator *(Years left, int right)
        {
            return ReferenceEquals(left, null) ? Zero : left.Multiply(right);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the years period multiplied by the scale.</returns>
        public static Years operator *(int left, Years right)
        {
            return ReferenceEquals(right, null) ? Zero : right.Multiply(left);
        }

        /// <summary>
        /// Multiply one year by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the years period multiplied by the scale.</returns>
        public static Years Multiply(Years left, int right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiply one year by a number. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the years period multiplied by the scale.</returns>
        public static Years Multiply(int left, Years right)
        {
            return left * right;
        }
        #endregion

        #region Division
        /// <summary>
        /// Returns a new instance with the years divided by the specified divisor.
        /// </summary>
        /// <param name="divisor">The amount to divide by, may be negative</param>
        /// <returns>The new period divided by the specified dvisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Years Divide(int divisor)
        {
            return divisor == 1 ? this : From(Value / divisor);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> Representing the years period divided by the scale.</returns>
        public static Years operator /(Years left, int right)
        {
            return ReferenceEquals(left, null) ? Zero : left.Divide(right);
        }

        /// <summary>
        /// Divide one year by a number. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the years period divided by the scale.</returns>
        public static Years Divide(Years left, int right)
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
        public bool Equals(Years other)
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
        public int CompareTo(Years other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Years left, Years right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Years left, Years right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Years left, Years right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Years left, Years right)
        {
            return Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Years left, Years right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Years left, Years right)
        {
            return Compare(left, right) >= 0;
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
            return "P" + Value + "Y";
        }
        #endregion
    }
}