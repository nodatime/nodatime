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

namespace NodaTime.Base
{
    /// <summary>
    /// Original name: BaseSingleFieldPeriod.
    /// I'm not entirely sure about it implementing IComparable[itself] as it can only do so
    /// in certain cases.
    /// </summary>
    public abstract class SingleFieldPeriodBase : IPeriod, IComparable<SingleFieldPeriodBase>
    {
        /// <summary>
        /// The period in the units of this period.
        /// </summary>
        private int period;

        protected SingleFieldPeriodBase(int period)
        {
            this.period = period;
        }

        public int CompareTo(SingleFieldPeriodBase other)
        {
            throw new NotImplementedException();
        }

        protected int Value
        {
            get { return period; }
            set { period = value; }
        }

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

        int IPeriod.GetValue(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index", "Index must be 0 for a SingleFieldPeriod.");
            }

            return Value;
        }

        int IPeriod.Get(DurationFieldType field)
        {
            if (field == FieldType)
            {
                return Value;
            }

            return 0;
        }

        bool IPeriod.IsSupported(DurationFieldType field)
        {
            return field == FieldType;
        }

        public Period ToPeriod()
        {
            throw new NotImplementedException();
        }

        public MutablePeriod ToMutablePeriod()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
