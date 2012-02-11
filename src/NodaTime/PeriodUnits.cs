#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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

namespace NodaTime
{
    /// <summary>
    /// The units within a <see cref="Period"/>. When a period is created to find the difference between two local values,
    /// the caller may specify which units are required - for example, you can ask for the difference between two dates
    /// in "years and weeks". Units are always applied largest-first in arithmetic.
    /// </summary>
    /// <remarks>
    /// For Noda Time developers: This enum should not be confused with NodaTime.Calendars.Fields.PeriodFieldType, although it's
    /// clearly related. This enum is more restricted (there are no eras, centuries, week-years or half days) and it's deliberately
    /// a "flags" enum as the values are combined to form the set of units for a <see cref="Period"/>. This type is roughly equivalent
    /// to org.joda.time.PeriodType in Joda Time.
    /// </remarks>
    [Flags]
    public enum PeriodUnits
    {
        /// <summary>
        /// Year element within a <see cref="Period" />
        /// </summary>
        Year = 1,

        /// <summary>
        /// Month element within a <see cref="Period" />
        /// </summary>
        Month = 2,

        /// <summary>
        /// Week element within a <see cref="Period" />
        /// </summary>
        Week = 4,

        /// <summary>
        /// Day element within a <see cref="Period" />
        /// </summary>
        Day = 8,

        /// <summary>
        /// Compound value representing the combination of <see cref="Year"/>, <see cref="Month"/>, <see cref="Week"/> and <see cref="Day"/>.
        /// </summary>
        AllDateUnits = Year | Month | Week | Day,

        /// <summary>
        /// Compound value representing the combination of <see cref="Year"/>, <see cref="Month"/> and <see cref="Day"/>.
        /// </summary>
        YearMonthDay = Year | Month | Day,

        /// <summary>
        /// Hour element within a <see cref="Period" />
        /// </summary>
        Hour = 16,

        /// <summary>
        /// Minute element within a <see cref="Period" />
        /// </summary>
        Minute = 32,

        /// <summary>
        /// Second element within a <see cref="Period" />
        /// </summary>
        Second = 64,

        /// <summary>
        /// Millisecond element within a <see cref="Period" />
        /// </summary>
        Millisecond = 128,

        /// <summary>
        /// Tick element within a <see cref="Period" />
        /// </summary>
        Tick = 256,

        /// <summary>
        /// Compound value representing the combination of <see cref="Hour"/>, <see cref="Minute"/> and <see cref="Second"/>.
        /// </summary>
        HourMinuteSecond = Hour | Minute | Second,

        /// <summary>
        /// Compound value representing the combination of all time elements.
        /// </summary>
        AllTimeUnits = Hour | Minute | Second | Millisecond | Tick,

        /// <summary>
        /// Compound value representing the combination of all possible elements except weeks.
        /// </summary>
        DateAndTime = Year | Month | Day | Hour | Minute | Second | Millisecond | Tick,

        /// <summary>
        /// Compound value representing the combination of all possible elements.
        /// </summary>
        AllUnits = Year | Month | Week | Day | Hour | Minute | Second | Millisecond | Tick
    }
}