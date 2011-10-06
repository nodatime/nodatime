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

using System;
using System.Diagnostics.CodeAnalysis;

namespace NodaTime
{
    /// <summary>
    /// Original name: NodaConstants.
    /// 
    /// I'm not sure that everything in NodaConstants
    /// really belongs in one file, and it should perhaps be named to reflect its ISO background
    /// as well. We should consider an Iso8601Month enum etc. Possibly start
    /// with everything in here, then refactor it out when it's all working.
    /// </summary>
    public static class NodaConstants
    {
        /// <summary>
        /// A constant for the number of ticks in a millisecond. The value of this constant is 10,000.
        /// </summary>
        public const long TicksPerMillisecond = TimeSpan.TicksPerMillisecond;
        /// <summary>
        /// A constant for the number of ticks in a second. The value of this constant is 10,000,000.
        /// </summary>
        public const long TicksPerSecond = TicksPerMillisecond * MillisecondsPerSecond;
        /// <summary>
        /// A constant for the number of ticks in a minute. The value of this constant is 600,000,000.
        /// </summary>
        public const long TicksPerMinute = TicksPerSecond * SecondsPerMinute;
        /// <summary>
        /// A constant for the number of ticks in a minute. The value of this constant is 36,000,000,000.
        /// </summary>
        public const long TicksPerHour = TicksPerMinute * MinutesPerHour;

        /// <summary>
        /// A constant for the number of ticks in a standard 24-hour day.
        /// The value of this constant is 864,000,000,000.
        /// </summary>
        public const long TicksPerStandardDay = TicksPerHour * HoursPerStandardDay;

        /// <summary>
        /// A constant for the number of ticks in a standard week of seven 24-hour days.
        /// The value of this constant is 6,048,000,000,000.
        /// </summary>
        public const long TicksPerStandardWeek = TicksPerStandardDay * DaysPerStandardWeek;

        /// <summary>
        /// A constant for the number of milliseconds per second.
        /// The value of this constant is 1000.
        /// </summary>
        public const int MillisecondsPerSecond = 1000;
        /// <summary>
        /// A constant for the number of milliseconds per minute.
        /// The value of this constant is 60,000.
        /// </summary>
        public const int MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;
        /// <summary>
        /// A constant for the number of milliseconds per hour.
        /// The value of this constant is 3,600,000.
        /// </summary>
        public const int MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;
        /// <summary>
        /// A constant for the number of milliseconds per standard 24-hour day.
        /// The value of this constant is 86,400,000.
        /// </summary>
        public const int MillisecondsPerStandardDay = MillisecondsPerHour * HoursPerStandardDay;
        /// <summary>
        /// A constant for the number of milliseconds per standard week of seven 24-hour days.
        /// The value of this constant is 604,800,000.
        /// </summary>
        public const int MillisecondsPerStandardWeek = MillisecondsPerStandardDay * DaysPerStandardWeek;

        /// <summary>
        /// A constant for the number of seconds per minute.
        /// The value of this constant is 60.
        /// </summary>
        public const int SecondsPerMinute = 60;
        /// <summary>
        /// A constant for the number of seconds per hour.
        /// The value of this constant is 3,600.
        /// </summary>
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        /// <summary>
        /// A constant for the number of seconds per standard 24-hour day.
        /// The value of this constant is 86,400.
        /// </summary>
        public const int SecondsPerDay = SecondsPerHour * HoursPerStandardDay;
        /// <summary>
        /// A constant for the number of seconds per standard week of seven 24-hour days.
        /// The value of this constant is 604,800.
        /// </summary>
        public const int SecondsPerWeek = SecondsPerDay * DaysPerStandardWeek;

        /// <summary>
        /// A constant for the number of minutes per hour.
        /// The value of this constant is 60.
        /// </summary>
        public const int MinutesPerHour = 60;
        /// <summary>
        /// A constant for the number of minutes per standard 24-hour day.
        /// The value of this constant is 1,440.
        /// </summary>
        public const int MinutesPerStandardDay = MinutesPerHour * HoursPerStandardDay;
        /// <summary>
        /// A constant for the number of minutes per standard week of seven 24-hour days.
        /// The value of this constant is 10,080.
        /// </summary>
        public const int MinutesPerStandardWeek = MinutesPerStandardDay * DaysPerStandardWeek;

        /// <summary>
        /// A constant for the number of hours in a standard day. Note that the number of hours
        /// in a day can vary due to daylight saving effects.
        /// The value of this constant is 24.
        /// </summary>
        public const int HoursPerStandardDay = 24;
        /// <summary>
        /// A constant for the number of hours in a standard week of seven 24-hour days.
        /// The value of this constant is 168.
        /// </summary>
        public const int HoursPerStandardWeek = HoursPerStandardDay * DaysPerStandardWeek;

        /// <summary>
        /// Number of days in a standard Gregorian week.
        /// The value of this constant is 7.
        /// </summary>
        public const int DaysPerStandardWeek = 7;

        /// <summary>
        /// The number of ticks in a <see cref="DateTime"/> at the Unix epoch.
        /// </summary>
        /// <remarks>
        /// Noda Time and the .NET framework use different "epochs" or origins for their time frames.
        /// Noda Time associates 0 ticks with midnight on January 1st 1970 UTC; .NET associates
        /// 0 ticks with midnight on January 0001 UTC. If the value of this field is added to
        /// the number of ticks in an <see cref="Instant"/>, the result is the number of ticks in
        /// the corresponding DateTime.
        /// </remarks>
        public static readonly long DateTimeEpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
    }
}
