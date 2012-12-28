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
    /// Indicates the type of a value represented by a period field.
    /// </summary>
    /// <remarks>
    /// If another type is added after Ticks, you must edit <see cref="UnsupportedPeriodField"/> appropriately.
    /// </remarks>
    internal enum PeriodFieldType
    {
        /// <summary>
        /// PeriodFieldType for eras.
        /// </summary>
        Eras,
        /// <summary>
        /// PeriodFieldType for centuries.
        /// </summary>
        Centuries,
        /// <summary>
        /// PeriodFieldType for week-years.
        /// </summary>
        WeekYears,
        /// <summary>
        /// PeriodFieldType for years.
        /// </summary>
        Years,
        /// <summary>
        /// PeriodFieldType for months.
        /// </summary>
        Months,
        /// <summary>
        /// PeriodFieldType for weeks.
        /// </summary>
        Weeks,
        /// <summary>
        /// PeriodFieldType for days.
        /// </summary>
        Days,
        /// <summary>
        /// PeriodFieldType for half days.
        /// </summary>
        HalfDays,
        /// <summary>
        /// PeriodFieldType for hours.
        /// </summary>
        Hours,
        /// <summary>
        /// PeriodFieldType for minutes.
        /// </summary>
        Minutes,
        /// <summary>
        /// PeriodFieldType for seconds.
        /// </summary>
        Seconds,
        /// <summary>
        /// PeriodFieldType for milliseconds.
        /// </summary>
        Milliseconds,
        /// <summary>
        /// PeriodFieldType for ticks.
        /// </summary>
        Ticks,
    }
}