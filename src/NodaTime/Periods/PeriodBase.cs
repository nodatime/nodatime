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
using NodaTime.Calendars;
using NodaTime.Fields;

namespace NodaTime.Periods
{
    /// <summary>
    /// Original name: BasePeriod.
    /// </summary>
    public abstract class PeriodBase : IPeriod
    {
        private readonly PeriodType periodType;
        private int[] fieldValues;


        protected PeriodBase( int[] fieldValues, PeriodType periodType)
        {
            this.periodType = periodType;

            this.fieldValues = fieldValues;
        }

        protected PeriodBase(int years, int months, int weeks, int days,
                         int hours, int minutes, int seconds, int millis,
                         PeriodType periodType)
        {
            this.periodType = NodaDefaults.CheckPeriodType(periodType);

            SetPeriodInternal(years, months, weeks, days, hours, minutes, seconds, millis);
        }

        protected PeriodBase(Duration duration, ICalendarSystem calendar, PeriodType periodType)
        {
            this.periodType = NodaDefaults.CheckPeriodType(periodType);

            calendar = NodaDefaults.CheckCalendarSystem(calendar);
            this.fieldValues = calendar.GetPeriodValues(this, duration);
        }

        private void SetPeriodInternal(int years, int months, int weeks, int days,
                                       int hours, int minutes, int seconds, int millis)
        {
            int[] newValues = new int[Size];
            CheckAndUpdate(DurationFieldType.Years, newValues, years);
            CheckAndUpdate(DurationFieldType.Months, newValues, months);
            CheckAndUpdate(DurationFieldType.Weeks, newValues, weeks);
            CheckAndUpdate(DurationFieldType.Days, newValues, days);
            CheckAndUpdate(DurationFieldType.Hours, newValues, hours);
            CheckAndUpdate(DurationFieldType.Minutes, newValues, minutes);
            CheckAndUpdate(DurationFieldType.Seconds, newValues, seconds);
            CheckAndUpdate(DurationFieldType.Milliseconds, newValues, millis);
            fieldValues = newValues;
        }

        private void CheckAndUpdate(DurationFieldType type, int[] values, int newValue)
        {
            int index = PeriodType.IndexOf(type);
            if (index == -1)
            {
                if (newValue != 0)
                {
                    throw new  ArgumentException(
                        "Period does not support field '" + type + "'");
                }
            }
            else
            {
                values[index] = newValue;
            }
        }

        protected int[] MergePeriodInto(int[] values, IPeriod period)
        {
            for (int i = 0, isize = period.Size; i < isize; i++)
            {
                DurationFieldType type = period.GetFieldType(i);
                int value = period.GetValue(i);
                CheckAndUpdate(type, values, value);
            }
            return values;
        }

        /// <summary>
        /// Gets an array of the field types that this period supports.
        /// </summary>
        /// <remarks>
        /// The fields are returned largest to smallest, for example Hours, Minutes, Seconds.
        /// </remarks>
        /// <returns>The fields supported in an array that may be altered, largest to smallest</returns>
        public DurationFieldType[] GetFieldTypes()
        {
            DurationFieldType[] result = new DurationFieldType[Size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetFieldType(i);
            }
            return result;
        }

        /// <summary>
        /// Gets an array of the value of each of the fields that this period supports.
        /// </summary>
        /// <remarks>
        /// The fields are returned largest to smallest, for example Hours, Minutes, Seconds.
        /// Each value corresponds to the same array index as <code>GetFieldTypes()</code>
        /// </remarks>
        /// <returns>The current values of each field in an array that may be altered, largest to smallest</returns>
        public int[] GetValues()
        {
            int[] result = new int[Size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = GetValue(i);
            }
            return result;
        }

        #region IPeriod Members

        public PeriodType PeriodType
        {
            get { return periodType; }
        }

        public int Size
        {
            get { return periodType.Size; }
        }

        public DurationFieldType GetFieldType(int index)
        {
            return periodType.GetFieldType(index);
        }

        public bool IsSupported(DurationFieldType field)
        {
            return PeriodType.IsSupported(field);
        }

        public int GetValue(int index)
        {
            return fieldValues[index];
        }

        public int Get(DurationFieldType field)
        {
            int index = PeriodType.IndexOf(field);
            if (index == -1)
            {
                return 0;
            }
            return GetValue(index);
        }

        public Period ToPeriod()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}