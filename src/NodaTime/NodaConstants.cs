// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime
{
    /// <summary>
    /// Useful constants, mostly along the lines of "number of milliseconds in an hour".
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
        /// A constant for the number of ticks in an hour. The value of this constant is 36,000,000,000.
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
        public const int SecondsPerStandardDay = SecondsPerHour * HoursPerStandardDay;
        /// <summary>
        /// A constant for the number of seconds per standard week of seven 24-hour days.
        /// The value of this constant is 604,800.
        /// </summary>
        public const int SecondsPerWeek = SecondsPerStandardDay * DaysPerStandardWeek;

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
        /// The instant at the Unix epoch of midnight 1st January 1970 UTC.
        /// </summary>
        /// <remarks>
        /// This value is not only the Unix epoch, but the Noda Time epoch, as it represents the value
        /// with a <see cref="Instant.Ticks"/> property of 0.
        /// </remarks>
        public static readonly Instant UnixEpoch = Instant.FromTicksSinceUnixEpoch(0);

        /// <summary>
        /// The instant at the BCL epoch of midnight 1st January 0001 UTC.
        /// </summary>
        public static readonly Instant BclEpoch = Instant.FromUtc(1, 1, 1, 0, 0);
    }
}
