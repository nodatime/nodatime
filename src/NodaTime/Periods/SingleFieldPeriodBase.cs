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

namespace NodaTime.Periods
{
    /// <summary>
    /// Original name: BaseSingleFieldPeriod.
    /// I'm not entirely sure about it implementing IComparable[itself] as it can only do so
    /// in certain cases.
    /// </summary>
    public abstract class SingleFieldPeriodBase : IPeriod, IComparable<SingleFieldPeriodBase>
    {
        #region Static Methods

        /// <summary>
        /// Calculates the number of whole units between the two specified datetimes.
        /// </summary>
        /// <param name="start">the start instant, validated to not be null</param>
        /// <param name="end">the end instant, validated to not be null</param>
        /// <param name="field">the field type to use, must not be null</param>
        /// <returns>the period</returns>
        /// <exception cref="System.ArgumentException">if the instants are null or invalid</exception>
        protected static int Between(ZonedDateTime start, ZonedDateTime end, DurationFieldType field)
        {
            throw new NotImplementedException();
        }

        protected static int Between(IPartial start, IPartial end, IPeriod field)
        {
            throw new NotImplementedException();
        }

        protected static int StandardPeriodIn(IPeriod period, long millisPerUnit)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected SingleFieldPeriodBase(int period)
        {
            this.Value = period;
        }

        public int CompareTo(SingleFieldPeriodBase other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The amount of this period.
        /// </summary>
        protected int Value { get; set; }

        /// <summary>
        /// Gets the single duartion field type.
        /// </summary>
        public abstract DurationFieldType FieldType { get; }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        #region IPeriod Members

        /// <summary>
        /// Gets the period type which matches the duration field type.
        /// </summary>
        public abstract PeriodType PeriodType { get; }

        /// <summary>
        /// Gets the number of fields this period supports, which is one.
        /// </summary>
        int IPeriod.Size { get { return 1; } }

        /// <summary>
        /// Gets the field type at the specified index.
        /// 
        /// The only index supported by this period is zero which returns the field type of this class.
        /// </summary>
        /// <param name="index">the index to retrieve, which must be 0</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if index != 0</exception>
        DurationFieldType IPeriod.GetFieldType(int index)
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
        int IPeriod.GetValue(int index)
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
        int IPeriod.Get(DurationFieldType field)
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
        bool IPeriod.IsSupported(DurationFieldType field)
        {
            return field == FieldType;
        }

        #endregion
    }
}