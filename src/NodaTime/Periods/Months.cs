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
    /// An immutable time period representing a number of months.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <code>Months</code> is an immutable period that can only store months.
    /// It does not store years, days or hours for example. As such it is a
    /// type-safe way of representing a number of months in an application.
    /// </para>
    /// <para>
    /// The number of months is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// </para>
    /// <para>
    /// <code>Months</code> is thread-safe and immutable.
    /// </para>
    /// </remarks>
    public sealed class Months : SingleFieldPeriodBase, IEquatable<Months>, IComparable<Months>
    {
        #region Static Properties

        private static readonly Months zero = new Months(0);
        private static readonly Months one = new Months(1);
        private static readonly Months two = new Months(2);
        private static readonly Months three = new Months(3);
        private static readonly Months maxValue = new Months(int.MaxValue);
        private static readonly Months minValue = new Months(int.MinValue);

        /// <summary>
        /// Gets a period representing zero months
        /// </summary>
        public static Months Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one month
        /// </summary>
        public static Months One { get { return one; } }

        /// <summary>
        /// Gets a period representing two months
        /// </summary>
        public static Months Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three months
        /// </summary>
        public static Months Three { get { return three; } }

        /// <summary>
        /// Gets a period representing the maximum number of months that can be stored in this object.
        /// </summary>
        public static Months MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of months that can be stored in this object.
        /// </summary>
        public static Months MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Months</code> that may be cached.
        /// </summary>
        /// <param name="months">The number of months to obtain an instance for</param>
        /// <returns>The instance of Months</returns>
        /// <remarks>
        /// <code>Months</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Months From (int months)
        {
            switch (months)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default: 
                    return new Months(months);
            }
        }

        
        
        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Months);

        /// <summary>
        /// Creates a new <code>Months</code> by parsing a string in the ISO8601 format 'PnM'.
        /// </summary>
        /// <param name="months">The period string, null returns zero</param>
        /// <returns>The period in months</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// months component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Months Parse(string months)
        {
            if (String.IsNullOrEmpty(months))
            {
                return Months.Zero;
            }

            Period p = parser.Parse(months);
            return Months.From(p.Months);
        }

        #endregion

        private Months(int months) : base(months) { }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Months</code>.
        /// </summary>
        public override DurationFieldType FieldType { get { return DurationFieldType.Months; } }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Months</code>.
        /// </summary>
        public override PeriodType PeriodType { get { return PeriodType.Months; } }

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of months added.
        /// </summary>
        /// <param name="months">The amount of months to add, may be negative</param>
        /// <returns>The new period plus the specified number of months</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Months Add(int months)
        {
            return months == 0 ? this : Months.From(Value + months);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the sum of the given values.</returns>
        public static Months operator +(Months left, Months right)
        {
            return object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one months instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the sum of the given values.</returns>
        public static Months Add(Months left, Months right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of months taken away.
        /// </summary>
        /// <param name="months">The amount of months to take away, may be negative</param>
        /// <returns>The new period minus the specified number of months</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Months Subtract(int months)
        {
            return Add(-months);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the difference of the given values.</returns>
        public static Months operator -(Months left, Months right)
        {
            return object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one months from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the difference of the given values.</returns>
        public static Months Subtract(Months left, Months right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the months multiplied by the specified scalar.
        /// </summary>
        /// <param name="months">The amount to multiply by, may be negative</param>
        /// <returns>The new period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Months Multiply(int months)
        {
            return Months.From(Value * months);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the multiplication of the given values.</returns>
        public static Months operator *(Months left, Months right)
        {
            return object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one months by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Months"/> representing the multiplication of the given values.</returns>
        public static Months Multiply(Months left, Months right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the months divided by the specified divisor.
        /// </summary>
        /// <param name="years">The amount to divide by, may be negative</param>
        /// <returns>The new period divided by the specified dvisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Months Divide(int years)
        {
            return Months.From(Value / years);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> Representing the divison of the given values.</returns>
        public static Months operator /(Months left, Months right)
        {
            return object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one year by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> representing the division of the given values.</returns>
        public static Months Divide(Months left, Months right)
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
        public bool Equals(Months other)
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
        public int CompareTo(Months other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Months left, Months right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Months left, Months right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Months left, Months right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Months left, Months right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Months left, Months right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Months left, Months right)
        {
            return SingleFieldPeriodBase.Compare(left, right) >= 0;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Months"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Months"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Months period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Months"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Months"/> period instance</param>
        /// <returns>New <see cref="Months"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Months(int value)
        {
            return Months.From(value);
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
            return "P" + Value + "M";
        }

        #endregion
    }
}
