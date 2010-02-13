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
    /// An immutable time period representing a number of weeks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <code>Weeks</code> is an immutable period that can only store weeks.
    /// It does not store years, months or hours for example. As such it is a
    /// type-safe way of representing a number of weeks in an application.
    /// </para>
    /// <para>
    /// The number of weeks is set in the constructor, and may be queried using
    /// <code>Value</code>. Basic mathematical operations are provided -
    /// </para>
    /// <para>
    /// <code>Weeks</code> is thread-safe and immutable.
    /// </para>
    /// </remarks>
    public sealed class Weeks : SingleFieldPeriodBase, IEquatable<Weeks>, IComparable<Weeks>
    {
        #region Static Properties

        private static readonly Weeks zero = new Weeks(0);
        private static readonly Weeks one = new Weeks(1);
        private static readonly Weeks two = new Weeks(2);
        private static readonly Weeks three = new Weeks(3);
        private static readonly Weeks maxValue = new Weeks(int.MaxValue);
        private static readonly Weeks minValue = new Weeks(int.MinValue);

        /// <summary>
        /// Gets a period representing zero weeks
        /// </summary>
        public static Weeks Zero { get { return zero; } }

        /// <summary>
        /// Gets a period representing one week
        /// </summary>
        public static Weeks One { get { return one; } }

        /// <summary>
        /// Gets a period representing two weeks
        /// </summary>
        public static Weeks Two { get { return two; } }

        /// <summary>
        /// Gets a period representing three weeks
        /// </summary>
        public static Weeks Three { get { return three; } }

        /// <summary>
        /// Gets a period representing the maximum number of weeks that can be stored in this object.
        /// </summary>
        public static Weeks MaxValue { get { return maxValue; } }

        /// <summary>
        /// Gets a period representing the minimum number of weeks that can be stored in this object.
        /// </summary>
        public static Weeks MinValue { get { return minValue; } }

        /// <summary>
        /// Obtains an instance of <code>Weeks</code> that may be cached.
        /// </summary>
        /// <param name="weeks">The number of weeks to obtain an instance for</param>
        /// <returns>The instance of Weeks</returns>
        /// <remarks>
        /// <code>Weeks</code> is immutable, so instances can be cached and shared.
        /// This factory method provides access to shared instances.
        /// </remarks>
        public static Weeks From(int weeks)
        {
            switch (weeks)
            {
                case 0: return zero;
                case 1: return one;
                case 2: return two;
                case 3: return three;
                case int.MaxValue: return maxValue;
                case int.MinValue: return minValue;
                default:
                    return new Weeks(weeks);
            }
        }



        private static readonly PeriodFormatter parser = IsoPeriodFormats.Standard.WithParseType(PeriodType.Weeks);

        /// <summary>
        /// Creates a new <code>Weeks</code> by parsing a string in the ISO8601 format 'PnW'.
        /// </summary>
        /// <param name="months">The period string, null returns zero</param>
        /// <returns>The period in months</returns>
        /// <remarks>
        /// <para>
        /// The parse will accept the full ISO syntax of PnYnMnWnDTnHnMnS however only the
        /// weeks component may be non-zero. If any other component is non-zero, an exception
        /// will be thrown
        /// </para>
        /// </remarks>
        public static Weeks Parse(string weeks)
        {
            if (String.IsNullOrEmpty(weeks))
            {
                return Weeks.Zero;
            }

            Period p = parser.Parse(weeks);
            return Weeks.From(p.Weeks);
        }

        #endregion

        private Weeks(int weeks) : base(weeks) { }

        /// <summary>
        /// Gets the duration field type, which is <code>DurationFieldType.Weeks</code>.
        /// </summary>
        public override DurationFieldType FieldType
        {
            get { return DurationFieldType.Weeks; }
        }

        /// <summary>
        /// Gets the period type, which is <code>PeriodType.Weeks</code>.
        /// </summary>
        public override PeriodType PeriodType
        {
            get { return PeriodType.Weeks; }
        }

        #region ToStandart

        /// <summary>
        /// Converts this period in weeks to a period in weeks assuming a
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
        /// <returns>A period representing the number of days for this number of weeks</returns>
        public Days ToStandardDays()
        {
            return Days.From(Value * NodaConstants.DaysPerWeek);
        }

        /// <summary>
        /// Converts this period in weeks to a period in hours assuming a
        /// 7 day week and 24 hour day.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days long
        /// all days are 24 hours long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of hours for this number of weeks</returns>
        public Hours ToStandardHours()
        {
            return Hours.From(Value * NodaConstants.HoursPerWeek);
        }

        /// <summary>
        /// Converts this period in weeks to a period in minutes assuming a
        /// 7 day week, 24 hour day and 60 minute hour.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days long
        /// ,all days are 24 hours long and all hours are 60 minutes long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of minutes for this number of weeks</returns>
        public Minutes ToStandardMinutes()
        {
            return Minutes.From(Value * NodaConstants.MinutesPerWeek);
        }

        /// <summary>
        /// Converts this period in weeks to a period in seconds assuming a
        /// 7 day week, 24 hour day and 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days long,
        /// all days are 24 hours long, all hours are 60 minutes long and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A period representing the number of seconds for this number of weeks</returns>
        public Seconds ToStandardSeconds()
        {
            return Seconds.From(Value * NodaConstants.SecondsPerWeek);
        }

        /// <summary>
        /// Converts this period in weeks to a duration in milliseconds assuming a
        /// 7 day week, 24 hour day and 60 minute hour and 60 second minute.
        /// <para>
        /// This method allows you to convert between different types of period.
        /// However to achieve this it makes the assumption that all weeks are 7 days long,
        /// all days are 24 hours long, all hours are 60 minutes long and all minutes are 60 seconds long.
        /// This is not true when daylight savings time is considered, and may also
        /// not be true for some unusual chronologies. However, it is included as it
        /// is a useful operation for many applications and business rules.
        /// </para>
        /// </summary>        
        /// <returns>A duration equivalent to this number of weeks</returns>
        public Duration ToStandardDuration()
        {
            return new Duration(Value * NodaConstants.MillisecondsPerWeek);
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Creates a new int from the specified <see cref="Weeks"/> instance
        /// </summary>
        /// <param name="period">An instance of <see cref="Weeks"/> period to get value from</param>
        /// <returns>A new int which represents a value of given period</returns>
        public static implicit operator int(Weeks period)
        {
            return period == null ? 0 : period.Value;
        }

        /// <summary>
        /// Creates a new <see cref="Weeks"/> instance from the specified integer value
        /// </summary>
        /// <param name="value">A value to use for initialization of new <see cref="Weeks"/> period instance</param>
        /// <returns>New <see cref="Weeks"/> instance whose Value property is initialized to the given value</returns>
        public static explicit operator Weeks(int value)
        {
            return Weeks.From(value);
        }

        #endregion

        #region Negation

        /// <summary>
        /// Returns a new instance with the weeks value negated.
        /// </summary>
        /// <returns>The new weeks period with a negated value</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Weeks Negated()
        {
            return Weeks.From(-Value);
        }

        /// <summary>
        /// Implements the unary operator - (negation).
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Weeks"/> instance with a negated value.</returns>
        public static Weeks operator -(Weeks period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Negated();
        }

        /// <summary>
        /// Negate a given weeks period. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="period">The period to negate.</param>
        /// <returns>A new <see cref="Weeks"/> instance with a negated value.</returns>
        public static Weeks Negate(Weeks period)
        {
            return -period;
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Implements the unary operator + .
        /// </summary>
        /// <param name="period">The operand.</param>
        /// <returns>The same <see cref="Weeks"/> instance</returns>
        public static Weeks operator +(Weeks period)
        {
            return period;
        }

        /// <summary>
        /// Implements the unary operator ++ (increment).
        /// </summary>
        /// <param name="period">The period to increment.</param>
        /// <returns>A new <see cref="Weeks"/> instance with incremented value.</returns>
        public static Weeks operator ++(Weeks period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Add(1);
        }

        /// <summary>
        /// Implements the unary operator ++ (decrement).
        /// </summary>
        /// <param name="period">The period to decrement.</param>
        /// <returns>A new <see cref="Weeks"/> instance with decremented value.</returns>
        public static Weeks operator --(Weeks period)
        {
            return Object.ReferenceEquals(period, null) ? null : period.Subtract(1);
        }

        #endregion

        #region Add

        /// <summary>
        /// Returns a new instance with the specified number of weeks added.
        /// </summary>
        /// <param name="weeks">The amount of weeks to add, may be negative</param>
        /// <returns>The new period plus the specified number of weeks</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Weeks Add(int weeks)
        {
            return weeks == 0 ? this : Weeks.From(Value + weeks);
        }

        /// <summary>
        /// Implements the operator + (addition).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> representing the sum of the given values.</returns>
        public static Weeks operator +(Weeks left, Weeks right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Add(right);
        }

        /// <summary>
        /// Adds one Weeks instance to another. Friendly alternative to <c>operator+()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> representing the sum of the given values.</returns>
        public static Weeks Add(Weeks left, Weeks right)
        {
            return left + right;
        }

        #endregion

        #region Subtract

        /// <summary>
        /// Returns a new instance with the specified number of weeks taken away.
        /// </summary>
        /// <param name="weeks">The amount of weeks to take away, may be negative</param>
        /// <returns>The new period minus the specified number of weeks</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Weeks Subtract(int weeks)
        {
            return Add(-weeks);
        }

        /// <summary>
        /// Implements the operator - (subtraction).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> representing the difference of the given values.</returns>
        public static Weeks operator -(Weeks left, Weeks right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Subtract(right);
        }

        /// <summary>
        /// Subtracts one weeks period from an another. Friendly alternative to <c>operator-()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> representing the difference of the given values.</returns>
        public static Weeks Subtract(Weeks left, Weeks right)
        {
            return left - right;
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Returns a new instance with the weeks multiplied by the specified scalar.
        /// </summary>
        /// <param name="weeks">The amount to multiply by, may be negative</param>
        /// <returns>The new weeks period multiplied by the specified scalar</returns>
        /// <remarks>
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Weeks Multiply(int weeks)
        {
            return weeks == 1 ? this : Weeks.From(Value * weeks);
        }

        /// <summary>
        /// Implements the operator * (multiplication).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> representing the multiplication of the given values.</returns>
        public static Weeks operator *(Weeks left, Weeks right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Multiply(right);
        }

        /// <summary>
        /// Multiply one weeks period by an another. Friendly alternative to <c>operator*()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> instance representing the multiplication of the given values.</returns>
        public static Weeks Multiply(Weeks left, Weeks right)
        {
            return left * right;
        }

        #endregion

        #region Division

        /// <summary>
        /// Returns a new instance with the weeks divided by the specified divisor.
        /// </summary>
        /// <param name="weeks">The amount to divide by, may be negative</param>
        /// <returns>The new weeks period divided by the specified dvisor</returns>
        /// <remarks>
        /// The calculation uses integer division, thus 3 divided by 2 is 1.
        /// This instance is immutable and unaffected by this method call.
        /// </remarks>
        public Weeks Divide(int weeks)
        {
            return weeks == 1 ? this : Weeks.From(Value / weeks);
        }

        /// <summary>
        /// Implements the operator / (division).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Years"/> Representing the divison of the given values.</returns>
        public static Weeks operator /(Weeks left, Weeks right)
        {
            return Object.ReferenceEquals(left, null) ? right : left.Divide(right);
        }

        /// <summary>
        /// Divide one Weeks period by an another. Friendly alternative to <c>operator/()</c>.
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>A new <see cref="Weeks"/> instance representing the division of the given values.</returns>
        public static Weeks Divide(Weeks left, Weeks right)
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
        public bool Equals(Weeks other)
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
        public int CompareTo(Weeks other)
        {
            return base.CompareTo(other);
        }

        /// <summary>
        /// Implements the operator == (equality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator ==(Weeks left, Weeks right)
        {
            return Object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator != (inequality).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if values are not equal to each other, otherwise <c>false</c>.</returns>
        public static bool operator !=(Weeks left, Weeks right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implements the operator &lt; (less than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than the right value, otherwise <c>false</c>.</returns>
        public static bool operator <(Weeks left, Weeks right)
        {
            return SingleFieldPeriodBase.Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;= (less than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is less than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator <=(Weeks left, Weeks right)
        {
            return SingleFieldPeriodBase.Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt; (greater than).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than the right value, otherwise <c>false</c>.</returns>
        public static bool operator >(Weeks left, Weeks right)
        {
            return SingleFieldPeriodBase.Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;= (greater than or equal).
        /// </summary>
        /// <param name="left">The left hand side of the operator.</param>
        /// <param name="right">The right hand side of the operator.</param>
        /// <returns>c>true</c> if the left value is greater than or equal to the right value, otherwise <c>false</c>.</returns>
        public static bool operator >=(Weeks left, Weeks right)
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
            return "P" + Value + "W";
        }

        #endregion

    }
}
