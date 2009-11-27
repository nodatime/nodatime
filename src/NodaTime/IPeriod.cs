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
using NodaTime.Periods;

namespace NodaTime
{
    /// <summary>
    /// Original name: ReadablePeriod
    /// </summary>
    public interface IPeriod
    {
        /// <summary>
        /// Gets the period type that defines which fields are included in the period.
        /// </summary>
        PeriodType PeriodType { get; }

        /// <summary>
        /// Gets the number of fields this period supports.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Gets the field type at the specified index.
        /// </summary>
        /// <param name="index">the index the retrieve</param>
        /// <returns>the field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the index is invalid</exception>
        DurationFieldType GetFieldType(int index);

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">the index to retrieve</param>
        /// <returns>the value of the field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the index is invalid</exception>
        int GetValue(int index);

        /// <summary>
        /// Gets the value of one of the fields.
        /// 
        /// If the field type specified is not supported by the period then zero is returned.
        /// </summary>
        /// <param name="field">the field type to query, null return zero</param>
        /// <returns>the value of that field, zero if field not supported</returns>
        int Get(DurationFieldType field);

        /// <summary>
        /// Checks whether the field type specified is supported by this period.
        /// </summary>
        /// <param name="field">the field to check, may be null which returns false</param>
        /// <returns>true if the field is supported</returns>
        bool IsSupported(DurationFieldType field);

        /// <summary>
        /// Get this period as an immutable <see cref="Period"/> object.
        /// 
        /// This will either typecase this instance, or create a new <see cref="Period"/>Period</see>.
        /// </summary>
        /// <returns>a Duration using the same field set and values</returns>
        Period ToPeriod();
    }
}
