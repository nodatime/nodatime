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
using NodaTime.Fields;
using NodaTime.Periods;

namespace NodaTime
{
    /// <summary>
    /// Defines a time period specified in terms of individual duration fields
    /// such as years and days.
    /// </summary>
    /// <remarks>
    /// Periods are split up into multiple fields, for example days and seconds.
    /// Implementations are not required to evenly distribute the values across the fields.
    /// The value for each field may be positive or negative.
    /// 
    /// </remarks>
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
        /// <param name="index">The zero-based index of the value to get</param>
        /// <returns>The field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the index is invalid</exception>
        DurationFieldType GetFieldType(int index);

        /// <summary>
        /// Checks whether the field type specified is supported by this period.
        /// </summary>
        /// <param name="field">The field to check, may be null which returns false</param>
        /// <returns>True if the field is supported</returns>
        bool IsSupported(DurationFieldType field);

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get </param>
        /// <returns>The value of the field at the specified index</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the index is less than 0 or equal to or greater than Size</exception>
        int this[int index] { get; }

        /// <summary>
        /// Gets the value of one of the fields.
        /// </summary>
        /// <param name="field">The field type to query, null return zero</param>
        /// <returns>The value of that field, zero if field not supported</returns>
        /// <remarks>
        /// If the field type specified is not supported by the period then zero is returned.
        /// </remarks>
        int this[DurationFieldType field] { get; }
    }
}
