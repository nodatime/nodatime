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
using NodaTime.Utility;

namespace NodaTime.Periods
{
    /// <summary>
    /// SingleFieldPeriodBase is an abstract implementation of IPeriod that
    /// manages a single duration field, such as days or minutes.
    /// </summary>
    public abstract class SingleFieldPeriodBase : IPeriod, IEquatable<SingleFieldPeriodBase>
                                                    , IComparable<SingleFieldPeriodBase>, IComparable
    {
        private readonly int value;

        /// <summary>
        /// Initializes a new instance representing the specified value.
        /// </summary>
        /// <param name="value">The value to represent</param>
        protected SingleFieldPeriodBase(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the amount of this period.
        /// </summary>
        public int Value { get { return value; } }

        /// <summary>
        /// Gets the single duartion field type.
        /// </summary>
        public abstract DurationFieldType FieldType { get; }

        #region IPeriod Members

        /// <summary>
        /// Gets the period type which matches the duration field type.
        /// </summary>
        public abstract PeriodType PeriodType { get; }

        /// <summary>
        /// Gets the number of fields this period supports, which is one.
        /// </summary>
        public int Size { get { return 1; } }

        /// <summary>
        /// Gets the field type at the specified index.
        /// 
        /// The only index supported by this period is zero which returns the field type of this class.
        /// </summary>
        /// <param name="index">the index to retrieve, which must be 0</param>
        /// <returns>The field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if index != 0</exception>
        public DurationFieldType GetFieldType(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be 0 for a SingleFieldPeriod.");
            }

            return FieldType;
        }

        /// <summary>
        /// Gets the value at the specfied index.
        /// 
        /// The only index supported by this period is zero.
        /// </summary>
        /// <param name="index">the index to retrieve, which must be zero</param>
        /// <returns>the value of the field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if index != 0</exception>
        public int GetValue(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be 0 for a SingleFieldPeriod.");
            }

            return Value;
        }

        /// <summary>
        /// Gets the value of a duration field represented by this period.
        /// 
        /// If the field type specified does not match the type used by this class then zero is returned.
        /// </summary>
        /// <param name="field">the field type to query, null returns zero</param>
        /// <returns>the value of that field, zero if field not supported</returns>
        public int Get(DurationFieldType field)
        {
            if (field == FieldType)
            {
                return Value;
            }

            return 0;
        }

        /// <summary>
        /// Checks whether the duration field specified is supported by this period.
        /// </summary>
        /// <param name="field">the type to check, may be null which returns false</param>
        /// <returns>true if the field is supported</returns>
        public bool IsSupported(DurationFieldType field)
        {
            return field == FieldType;
        }

        #endregion

        #region Equality

        public override bool Equals(object obj)
        {
            return Equals(obj as SingleFieldPeriodBase);
        }

        public override int GetHashCode()
        {
            int hash = HashCodeHelper.Initialize();
            hash = HashCodeHelper.Hash(hash, FieldType);
            hash = HashCodeHelper.Hash(hash, Value);
            return hash;
        }

        public bool Equals(SingleFieldPeriodBase other)
        {
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            return FieldType == other.FieldType && Value == other.Value;
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
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
        /// <description>This object is less than the <paramref name="other"/> parameter.</description>
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
        public int CompareTo(SingleFieldPeriodBase other)
        {
            if (Object.ReferenceEquals(this, other))
            {
                return 0;
            }

            if (other == null)
            {
                return 1;
            }

            if (other.FieldType != FieldType)
            {
                throw new ArgumentException("Other object must have the same FieldType", "other");
            }
            return Value.CompareTo(other.Value);
        }

        public static int Compare(SingleFieldPeriodBase left, SingleFieldPeriodBase right)
        {
            return left == null ? -1 : left.CompareTo(right);
        }

        int IComparable.CompareTo(object obj)
        {
            return CompareTo(obj as SingleFieldPeriodBase);
        }

        #endregion

        /// <summary>
        /// Get this period as an immutable <see cref="Period"/> object.
        /// </summary>
        /// <returns>A <see cref="Period"/> representing the same field value</returns>
        /// <remarks>
        /// The period will use <see cref="NodaTime.Periods.PeriodType.Standard"/>.
        /// </remarks>
        public Period ToPeriod()
        {
            return Period.Zero.With(this);
        }
    }
}