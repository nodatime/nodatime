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


namespace NodaTime
{
    /// <summary>
    /// Original name: DateTimeConstants.
    /// 
    /// I'm not sure that everything in DateTimeConstants
    /// really belongs in one file, and it should perhaps be named to reflect its ISO background
    /// as well. We should consider an Iso8601Month enum etc. Possibly start
    /// with everything in here, then refactor it out when it's all working.
    /// </summary>
    public static class DateTimeConstants
    {
        // Note: I (Martinho) renamed Millis to Milliseconds to keep consistency with existing Framework members

        /// <summary>
        /// As per <see cref="TimeSpan.TicksPerMillisecond" />, included here for consistency.
        /// </summary>
        public const long TicksPerMillisecond = 10000;

        /// <summary>
        /// Original name: MILLIS_PER_MINUTE
        /// </summary>
        public const int MillisecondsPerSecond = 1000;

        public const long TicksPerSecond = TicksPerMillisecond * MillisecondsPerSecond;
        
        /// <summary>
        /// Original name: SECONDS_PER_MINUTE
        /// </summary>
        public const int SecondsPerMinute = 60;
        /// <summary>
        /// Original name: MILLIS_PER_MINUTE
        /// </summary>
        public const int MillisecondsPerMinute = MillisecondsPerSecond * SecondsPerMinute;

        public const long TicksPerMinute = TicksPerMillisecond * MillisecondsPerMinute;

        /// <summary>
        /// Original name: MINUTES_PER_HOUR
        /// </summary>
        public const int MinutesPerHour = 60;
        /// <summary>
        /// Original name: SECONDS_PER_HOUR
        /// </summary>
        public const int SecondsPerHour = SecondsPerMinute * MinutesPerHour;
        /// <summary>
        /// Original name: MILLIS_PER_HOUR
        /// </summary>
        public const int MillisecondsPerHour = MillisecondsPerMinute * MinutesPerHour;

        public const long TicksPerHour = TicksPerMillisecond * MillisecondsPerHour;

        /// <summary>
        /// Original name: HOURS_PER_DAY
        /// </summary>
        public const int HoursPerDay = 24;
        /// <summary>
        /// Original name: MINUTES_PER_DAY
        /// </summary>
        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        /// <summary>
        /// Original name: MINUTES_PER_DAY
        /// </summary>
        public const int SecondsPerDay = SecondsPerHour * HoursPerDay;
        /// <summary>
        /// Original name: MILLIS_PER_DAY
        /// </summary>
        public const int MillisecondsPerDay = MillisecondsPerHour * HoursPerDay;

        public const long TicksPerDay = TicksPerMillisecond * MillisecondsPerDay;

        /// <summary>
        /// Original name: DAYS_PER_WEEK
        /// </summary>
        public const int DaysPerWeek = 7;
        /// <summary>
        /// Original name: HOURS_PER_WEEK
        /// </summary>
        public const int HoursPerWeek = HoursPerDay * DaysPerWeek;
        /// <summary>
        /// Original name: MINUTES_PER_WEEK
        /// </summary>
        public const int MinutesPerWeek = MinutesPerDay * DaysPerWeek;
        /// <summary>
        /// Original name: MINUTES_PER_WEEK
        /// </summary>
        public const int SecondsPerWeek = SecondsPerDay * DaysPerWeek;
        /// <summary>
        /// Original name: MILLIS_PER_WEEK
        /// </summary>
        public const int MillisecondsPerWeek = MillisecondsPerDay * DaysPerWeek;

        public const long TicksPerWeek = TicksPerMillisecond * MillisecondsPerWeek;

    }
}