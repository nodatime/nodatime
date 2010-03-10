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
    /// An immutable time period representing a number of months.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <code>Days</code> is an immutable period that can only store days.
    /// </para>
    /// <para>
    /// It does not store years, months or hours for example. As such it is a
    /// type-safe way of representing a number of days in an application.
    /// </para>
    /// <para>
    /// The number of days is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// </para>
    /// <para>
    /// <code>Days</code> is thread-safe and immutable.
    /// </para>    
    /// </remarks>
    public sealed class Days : SingleFieldPeriodBase, IEquatable<Days>, IComparable<Days>
    {
        #region Static Properties

        private static readonly Days zero = new Days(0);
        private static readonly Days one = new Days(1);
        private static readonly Days two = new Days(2);
        private static readonly Days three = new Days(3);
        private static readonly Days four = new Days(4);
        private static readonly Days five = new Days(5);
        private static readonly Days six = new Days(6);
        private static readonly Days seven = new Days(7);
        
        private static readonly Days maxValue = new Days(int.MaxValue);
        private static readonly Days minValue = new Days(int.MinValue);

        /// <summary>
        /// Gets a period representing zero days
        /// </summary>
        public static Days Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one day
        /// </summary>
        public static Days One { get { return one; } }

        /// <summary>
        /// Gets a period representing two days
        /// </summary>
        public static Days Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three days
        /// </summary>
        public static Days Three { get { return three; } }

        /// <summary>
        /// Gets a period representing four days
        /// </summary>
        public static Days Four { get { return four; } }

        /// <summary>
        /// Gets a period representing five days
        /// </summary>
        public static Days Five { get { return five; } }

        /// <summary>
        /// Gets a period representing six days
        /// </summary>
        public static Days Six { get { return six; } }

        /// <summary>
        /// Gets a period representing seven days
        /// </summary>
        public static Days Seven { get { return seven; } }

        /// <summary>
        /// Gets a period representing the maximum number of days that can be stored in this object.
        /// </summary>
        public static Days MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of days that can be stored in this object.
        /// </summary>
        public static Days MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Days</code> that may be cached.
        /// </summary>
        /// <param name="days">The number of days to obtain an instance for</param>
        /// <returns>The instance of Days</returns>
        /// <remarks>
        /// <code>Days</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Days From(int days)
        {
            switch (days)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case 4: return four;
                case 5: return five;
                case 6: return six;
                case 7: return seven;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default:
                    return new Days(days);
            }
        }

        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Days);

        /// <summary>
        /// Creates a new <code>Days</code> by parsing a string in the ISO8601 format 'PnD'.
        /// </summary>
        /// <param name="days">The period string, null returns zero</param>
        /// <returns>The period in days</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// days component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Days Parse(string days)
        {
            if (String.IsNullOrEmpty(days))
            {
                return Days.Zero;
            }

            Period p = parser.Parse(days);
            return Days.From(p.Days);
        }

        #endregion

        private Days(int days) : base(days) {}

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Days</code>.
        /// </summary>
        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Days; }
        }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Days</code>.
        /// </summary>
        public override PeriodType PeriodType
        {
            get { return PeriodType.Days; }
        }

        #region ToStandart

        /// <summary>
        /// Converts this period in days to a period in weeks assuming a
        /// 7 day week.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days
        /// long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of whole weeks for this number of days</returns>
        public Weeks ToStandardWeeks()
        {
            return Weeks.From(Value / NodaConstants.DaysPerWeek);
        }

        /// <summary>
        /// Converts this period in days to a period in hours assuming a
        /// 24 hour day.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all days are 24 hours long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of hours for this number of days</returns>
        public Hours ToStandardHours()
        {
            return Hours.From(Value * NodaConstants.HoursPerDay);
        }

        /// <summary>
        /// Converts this period in days to a period in minutes assuming a
        /// 24 hour day and 60 minute hour.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all days are 24 hours long 
        /// and all hours are 60 minutes long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of minutes for this number of hours</returns>
        public Minutes ToStandardMinutes()
        {
            return Minutes.From(Value * NodaConstants.MinutesPerDay);
        }

        /// <summary>
        /// Converts this period in days to a period in seconds assuming a
        /// 24 hour day and 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all days are 24 hours long
        /// and all hours are 60 minutes long and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of seconds for this number of hours</returns>
        public Seconds ToStandardSeconds()
        {
            return Seconds.From(Value * NodaConstants.SecondsPerDay);
        }

        /// <summary>
        /// Converts this period in days to a duration in milliseconds assuming a
        /// 24 hour day and 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all days are 24 hours long
        /// and all hours are 60 minutes long and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A duration equivalent to this number of days</returns>
        public Duration ToStandardDuration()
        {
            return new Duration(Value * NodaConstants.MillisecondsPerDay);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Days"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Days"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Days period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Days"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Days"/> period instance</param>
        /// <returns>New <see cref="Days"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Days(int value)
        {
            return Days.From(value);
        }

        #endregion

        #region Negation

        /// <summary>
        /// Returns a new instance with the days value negated.
        /// </summary>
        /// <returns>The new days period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Days Negated()
        {
            return Days.From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Days"/> instance with a negated value.</returns>
        public static Days operator -(Days period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given days period. Friendly alternative to unary <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Days"/> instance with a negated value.</returns>
        public static Days Negate(Days period)
        {
            return -period;
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Days"/> instance</returns>
        public static Days operator +(Days period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Days"/> instance with incremented value.</returns>
        public static Days operator ++(Days period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Days"/> instance with decremented value.</returns>
        public static Days operator --(Days period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Subtract(1);
        }

        #endregion

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of days added.
        /// </summary>
        /// <param name="days">The amount of days to add, may be negative</param>
        /// <returns>The new period plus the specified number of days</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Days Add(int days)
        {
            return days == 0 ? this : Days.From(Value + days);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the sum of the given values.</returns>
        public static Days operator +(Days left, Days right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one Days instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the sum of the given values.</returns>
        public static Days Add(Days left, Days right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of days taken away.
        /// </summary>
        /// <param name="days">The amount of days to take away, may be negative</param>
        /// <returns>The new period minus the specified number of days</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Days Subtract(int days)
        {
            return Add(-days);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the difference of the given values.</returns>
        public static Days operator -(Days left, Days right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one days period from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the difference of the given values.</returns>
        public static Days Subtract(Days left, Days right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the days multiplied by the specified scalar.
        /// </summary>
        /// <param name="days">The amount to multiply by, may be negative</param>
        /// <returns>The new days period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Days Multiply(int days)
        {
            return days == 1 ? this : Days.From(Value * days);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the multiplication of the given values.</returns>
        public static Days operator *(Days left, Days right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one days period by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> instance representing the multiplication of the given values.</returns>
        public static Days Multiply(Days left, Days right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the days divided by the specified divisor.
        /// </summary>
        /// <param name="days">The amount to divide by, may be negative</param>
        /// <returns>The new days period divided by the specified divisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Days Divide(int days)
        {
            return days == 1 ? this : Days.From(Value / days);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> representing the divison of the given values.</returns>
        public static Days operator /(Days left, Days right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one days period by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Days"/> instance representing the division of the given values.</returns>
        public static Days Divide(Days left, Days right)
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
        public bool Equals(Days other)
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
        public int CompareTo(Days other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Days left, Days right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Days left, Days right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Days left, Days right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Days left, Days right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Days left, Days right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns><c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Days left, Days right)
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
            return "P" + Value + "D";
        }

        #endregion

    }
}