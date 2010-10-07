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

namespace NodaTime.Partials
{
    public static class PartialUtils
    {
        /// <summary>
        /// Checks whether the partial is contiguous.
        /// </summary>
        /// <param name="partial">The partial to check</param>
        /// <returns>True if the partial is contiguous, false otherwise</returns>
        /// <remarks>
        /// A partial is contiguous if one field starts where another ends.
        /// <para>
        /// For example <code>LocalDate</code> is contiguous because DayOfMonth has
        /// the same range (Month) as the unit of the next field (MonthOfYear), and
        /// MonthOfYear has the same range (Year) as the unit of the next field (Year).
        /// </para>
        /// <para>
        /// Similarly, <code>LocalTime</code> is contiguous, as it consists of
        /// MillisOfSecond, SecondOfMinute, MinuteOfHour and HourOfDay (note how
        /// the names of each field 'join up').
        /// </para>
        /// <para>
        /// However, a Year/HourOfDay partial is not contiguous because the range
        /// field Day is not equal to the next field Year.
        /// Similarly, a DayOfWeek/DayOfMonth partial is not contiguous because
        /// the range Month is not equal to the next field Day.
        /// </para>
        /// </remarks>
        public static bool IsContiguous(IPartial partial)
        {
            if (partial == null)
            {
                throw new ArgumentNullException("partial");
            }

            DurationFieldType lastType = default(DurationFieldType);
            for (int i = 0; i < partial.Size; i++)
            {
                DateTimeFieldBase loopField = partial.GetField(i);
                if (i > 0)
                {
                    if (loopField.RangeDurationField.FieldType != lastType)
                    {
                        return false;
                    }
                }
                lastType = loopField.DurationField.FieldType;
            }
            return true;
        }
    }
}