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
using NodaTime.Fields;
using NodaTime.Format;
using System;

namespace NodaTime.Periods
{
    /// <summary>
    /// An immutable time period representing a number of hours.
    /// <para>
    /// <code>Hours</code> is an immutable period that can only store hours.
    /// It does not store years, months or minutes for example. As such it is a
    /// type-safe way of representing a number of hours in an application.
    /// </para>
    /// <para>
    /// The number of hours is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// <code>Add()</code>, <code>Subtract()</code>, <code>Multiply()</code> and
    /// <code>Divide()</code>.
    /// </para>
    /// <para>
    /// <code>Hours</code> is thread-safe and immutable.
    /// </para>
    /// </summary>
    public sealed class Hours : SingleFieldPeriodBase, IEquatable<Hours>, IComparable<Hours>
    {
        #region Static Properties

        private static readonly Hours zero = new Hours(0);
        private static readonly Hours one = new Hours(1);
        private static readonly Hours two = new Hours(2);
        private static readonly Hours three = new Hours(3);
        private static readonly Hours four = new Hours(4);
        private static readonly Hours five = new Hours(5);
        private static readonly Hours six = new Hours(6);
        private static readonly Hours seven = new Hours(7);
        private static readonly Hours eight = new Hours(8);

        private static readonly Hours maxValue = new Hours(int.MaxValue);
        private static readonly Hours minValue = new Hours(int.MinValue);

        /// <summary>
        /// Gets a period representing zero hours
        /// </summary>
        public static Hours Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one hour
        /// </summary>
        public static Hours One { get { return one; } }

        /// <summary>
        /// Gets a period representing two hours
        /// </summary>
        public static Hours Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three hours
        /// </summary>
        public static Hours Three { get { return three; } }

        /// <summary>
        /// Gets a period representing four hours
        /// </summary>
        public static Hours Four { get { return four; } }

        /// <summary>
        /// Gets a period representing five hours
        /// </summary>
        public static Hours Five { get { return five; } }

        /// <summary>
        /// Gets a period representing six hours
        /// </summary>
        public static Hours Six { get { return six; } }

        /// <summary>
        /// Gets a period representing seven hours
        /// </summary>
        public static Hours Seven { get { return seven; } }

        /// <summary>
        /// Gets a period representing eight hours
        /// </summary>
        public static Hours Eight { get { return eight; } }

        /// <summary>
        /// Gets a period representing the maximum number of hours that can be stored in this object.
        /// </summary>
        public static Hours MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of hours that can be stored in this object.
        /// </summary>
        public static Hours MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Hours</code> that may be cached.
        /// </summary>
        /// <param name="hours">The number of hours to obtain an instance for</param>
        /// <returns>The instance of Hours</returns>
        /// <remarks>
        /// <code>Hours</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Hours From(int hours)
        {
            switch (hours)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case 4: return four;
                case 5: return five;
                case 6: return six;
                case 7: return seven;
                case 8: return eight;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default:
                    return new Hours(hours);
            }
        }

        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Hours);

        /// <summary>
        /// Creates a new <code>Hours</code> by parsing a string in the ISO8601 format 'PTnH'.
        /// </summary>
        /// <param name="hours">The period string, null returns zero</param>
        /// <returns>The period in hours</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// hours component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Hours Parse(string hours)
        {
            if (String.IsNullOrEmpty(hours))
            {
                return Hours.Zero;
            }

            Period p = parser.Parse(hours);
            return Hours.From(p.Hours);
        }

        #endregion

        private Hours(int value) : base(value) { }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Hours</code>.
        /// </summary>
        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Hours; }
        }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Hours</code>.
        /// </summary>
        public override PeriodType PeriodType
        {
            get { return PeriodType.Hours; }
        }

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Hours"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Hours"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Hours period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Hours"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Hours"/> period instance</param>
        /// <returns>New <see cref="Hours"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Hours(int value)
        {
            return Hours.From(value);
        }

        #endregion

        #region Negation

        /// <summary>
        /// Returns a new instance with the hours value negated.
        /// </summary>
        /// <returns>The new days period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Hours Negated()
        {
            return Hours.From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Hours"/> instance with a negated value.</returns>
        public static Hours operator -(Hours period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given days period. Friendly alternative to unary <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Hours"/> instance with a negated value.</returns>
        public static Hours Negate(Hours period)
        {
            return -period;
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Hours"/> instance</returns>
        public static Hours operator +(Hours period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Hours"/> instance with incremented value.</returns>
        public static Hours operator ++(Hours period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Hours"/> instance with decremented value.</returns>
        public static Hours operator --(Hours period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Subtract(1);
        }

        #endregion

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of hours added.
        /// </summary>
        /// <param name="hours">The amount of days to add, may be negative</param>
        /// <returns>The new period plus the specified number of hours</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Hours Add(int hours)
        {
            return hours == 0 ? this : Hours.From(Value + hours);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the sum of the given values.</returns>
        public static Hours operator +(Hours left, Hours right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one Hours instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the sum of the given values.</returns>
        public static Hours Add(Hours left, Hours right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of hours taken away.
        /// </summary>
        /// <param name="hours">The amount of hours to take away, may be negative</param>
        /// <returns>The new period minus the specified number of hours</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Hours Subtract(int hours)
        {
            return Add(-hours);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the difference of the given values.</returns>
        public static Hours operator -(Hours left, Hours right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one hours period from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the difference of the given values.</returns>
        public static Hours Subtract(Hours left, Hours right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the hours multiplied by the specified scalar.
        /// </summary>
        /// <param name="hours">The amount to multiply by, may be negative</param>
        /// <returns>The new hours period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Hours Multiply(int hours)
        {
            return hours == 1 ? this : Hours.From(Value * hours);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the multiplication of the given values.</returns>
        public static Hours operator *(Hours left, Hours right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one hours period by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> instance representing the multiplication of the given values.</returns>
        public static Hours Multiply(Hours left, Hours right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the hours divided by the specified divisor.
        /// </summary>
        /// <param name="hours">The amount to divide by, may be negative</param>
        /// <returns>The new hours period divided by the specified divisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Hours Divide(int days)
        {
            return days == 1 ? this : Hours.From(Value / days);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> representing the divison of the given values.</returns>
        public static Hours operator /(Hours left, Hours right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one hours period by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Hours"/> instance representing the division of the given values.</returns>
        public static Hours Divide(Hours left, Hours right)
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
        public bool Equals(Hours other)
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
        public int CompareTo(Hours other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Hours left, Hours right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Hours left, Hours right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Hours left, Hours right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Hours left, Hours right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Hours left, Hours right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Hours left, Hours right)
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
            return "PT" + Value + "H";
        }

        #endregion

    }
}
