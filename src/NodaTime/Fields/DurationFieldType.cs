#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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
namespace NodaTime.Fields
{
    /// <summary>
    /// Indicates the type of a value represented by a duration field.
    /// </summary>
    public enum DurationFieldType
    {
        /// <summary>
        /// DurationFieldType for eras.
        /// </summary>
        Eras,
        /// <summary>
        /// DurationFieldType for centuries.
        /// </summary>
        Centuries,
        /// <summary>
        /// DurationFieldType for week-years.
        /// </summary>
        WeekYears,
        /// <summary>
        /// DurationFieldType for years.
        /// </summary>
        Years,
        /// <summary>
        /// DurationFieldType for months.
        /// </summary>
        Months,
        /// <summary>
        /// DurationFieldType for weeks.
        /// </summary>
        Weeks,
        /// <summary>
        /// DurationFieldType for days.
        /// </summary>
        Days,
        /// <summary>
        /// DurationFieldType for half days.
        /// </summary>
        HalfDays,
        /// <summary>
        /// DurationFieldType for hours.
        /// </summary>
        Hours,
        /// <summary>
        /// DurationFieldType for minutes.
        /// </summary>
        Minutes,
        /// <summary>
        /// DurationFieldType for seconds.
        /// </summary>
        Seconds,
        /// <summary>
        /// DurationFieldType for milliseconds.
        /// </summary>
        Milliseconds,
        /// <summary>
        /// DurationFieldType for ticks.
        /// </summary>
        Ticks,
    }
}