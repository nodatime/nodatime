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
    // For Noda Time developers:
    // This enum should not be confused with NodaTime.Calendars.Fields.PeriodFieldType, although it's
    // clearly related. This enum is more restricted (there are no eras, centuries, week-years or half days) and it's deliberately
    // a "flags" enum as the values are combined to form the set of units for a Period. This type is roughly equivalent
    // to org.joda.time.PeriodType in Joda Time.
    //
    // Note that the values of the single (non-compound) values must match up with the internal indexes used for
    // Period's values array.
    [Flags]
    public enum PeriodUnits
    {
        /// <summary>
        /// Value indicating no units - an empty period.
        /// </summary>        
        None = 0,

        /// <summary>
        /// Years element within a <see cref="Period" />
        /// </summary>
        Years = 1,

        /// <summary>
        /// Months element within a <see cref="Period" />
        /// </summary>
        Months = 2,

        /// <summary>
        /// Weeks element within a <see cref="Period" />
        /// </summary>
        Weeks = 4,

        /// <summary>
        /// Days element within a <see cref="Period" />
        /// </summary>
        Days = 8,

        /// <summary>
        /// Compound value representing the combination of <see cref="Years"/>, <see cref="Months"/>, <see cref="Weeks"/> and <see cref="Days"/>.
        /// </summary>
        AllDateUnits = Years | Months | Weeks | Days,

        /// <summary>
        /// Compound value representing the combination of <see cref="Years"/>, <see cref="Months"/> and <see cref="Days"/>.
        /// </summary>
        YearMonthDay = Years | Months | Days,

        /// <summary>
        /// Hours element within a <see cref="Period" />
        /// </summary>
        Hours = 16,

        /// <summary>
        /// Minutes element within a <see cref="Period" />
        /// </summary>
        Minutes = 32,

        /// <summary>
        /// Seconds element within a <see cref="Period" />
        /// </summary>
        Seconds = 64,

        /// <summary>
        /// Milliseconds element within a <see cref="Period" />
        /// </summary>
        Milliseconds = 128,

        /// <summary>
        /// Tick element within a <see cref="Period" />
        /// </summary>
        Ticks = 256,

        /// <summary>
        /// Compound value representing the combination of <see cref="Hours"/>, <see cref="Minutes"/> and <see cref="Seconds"/>.
        /// </summary>
        HourMinuteSecond = Hours | Minutes | Seconds,

        /// <summary>
        /// Compound value representing the combination of all time elements.
        /// </summary>
        AllTimeUnits = Hours | Minutes | Seconds | Milliseconds | Ticks,

        /// <summary>
        /// Compound value representing the combination of all possible elements except weeks.
        /// </summary>
        DateAndTime = Years | Months | Days | Hours | Minutes | Seconds | Milliseconds | Ticks,

        /// <summary>
        /// Compound value representing the combination of all possible elements.
        /// </summary>
        AllUnits = Years | Months | Weeks | Days | Hours | Minutes | Seconds | Milliseconds | Ticks
    }
}
