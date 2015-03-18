// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime
{
    /// <summary>
    /// The units within a <see cref="Period"/>. When a period is created to find the difference between two local values,
    /// the caller may specify which units are required - for example, you can ask for the difference between two dates
    /// in "years and weeks". Units are always applied largest-first in arithmetic.
    /// </summary>
    // Note to Noda Time developers: that the values of the single (non-compound) values must match up with the internal indexes used for
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
        /// Nanoseconds element within a <see cref="Period" />.
        /// </summary>
        Nanoseconds = 512,

        /// <summary>
        /// Compound value representing the combination of <see cref="Hours"/>, <see cref="Minutes"/> and <see cref="Seconds"/>.
        /// </summary>
        HourMinuteSecond = Hours | Minutes | Seconds,

        /// <summary>
        /// Compound value representing the combination of all time elements.
        /// </summary>
        AllTimeUnits = Hours | Minutes | Seconds | Milliseconds | Ticks | Nanoseconds,

        /// <summary>
        /// Compound value representing the combination of all possible elements except weeks.
        /// </summary>
        DateAndTime = Years | Months | Days | Hours | Minutes | Seconds | Milliseconds | Ticks | Nanoseconds,

        /// <summary>
        /// Compound value representing the combination of all possible elements.
        /// </summary>
        AllUnits = Years | Months | Weeks | Days | Hours | Minutes | Seconds | Milliseconds | Ticks | Nanoseconds,
    }
}
