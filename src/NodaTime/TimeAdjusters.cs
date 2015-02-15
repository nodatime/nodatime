// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace NodaTime
{
    /// <summary>
    /// Factory class for time adjusters: functions from <see cref="LocalTime"/> to <c>LocalTime</c>,
    /// which can be applied to <see cref="LocalTime"/>, <see cref="LocalDateTime"/>, and <see cref="OffsetDateTime"/>.
    /// </summary>
    public static class TimeAdjusters
    {
        /// <summary>
        /// Gets a time adjuster to truncate the time to the second, discarding fractional seconds.
        /// </summary>
        /// <value>A time adjuster to truncate the time to the second, discarding fractional seconds.</value>
        public static Func<LocalTime, LocalTime> TruncateToSecond { get; }
            = time => new LocalTime(time.Hour, time.Minute, time.Second);

        /// <summary>
        /// Gets a time adjuster to truncate the time to the minute, discarding fractional minutes.
        /// </summary>
        /// <value>A time adjuster to truncate the time to the minute, discarding fractional minutes.</value>
        public static Func<LocalTime, LocalTime> TruncateToMinute { get; }
            = time => new LocalTime(time.Hour, time.Minute);

        /// <summary>
        /// Get a time adjuster to truncate the time to the hour, discarding fractional hours.
        /// </summary>
        /// <value>A time adjuster to truncate the time to the hour, discarding fractional hours.</value>
        public static Func<LocalTime, LocalTime> TruncateToHour { get; }
            = time => new LocalTime(time.Hour, 0);
    }
}
