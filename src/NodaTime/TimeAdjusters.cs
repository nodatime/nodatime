// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodaTime
{
    /// <summary>
    /// Factory class for time adjusters: functions from <see cref="LocalTime"/> to <c>LocalTime</c>,
    /// which can be applied to <see cref="LocalTime"/>, <see cref="LocalDateTime"/>, and <see cref="OffsetDateTime"/>.
    /// </summary>
    public static class TimeAdjusters
    {
        private static readonly Func<LocalTime, LocalTime> truncateToSecond = time => new LocalTime(time.Hour, time.Minute, time.Second);
        private static readonly Func<LocalTime, LocalTime> truncateToMinute = time => new LocalTime(time.Hour, time.Minute);
        private static readonly Func<LocalTime, LocalTime> truncateToHour = time => new LocalTime(time.Hour, 0);

        /// <summary>
        /// A time adjuster to truncate the time to the second, discarding fractional seconds.
        /// </summary>
        public static Func<LocalTime, LocalTime> TruncateToSecond { get { return truncateToSecond; } }

        /// <summary>
        /// A time adjuster to truncate the time to the minute, discarding fractional minutes.
        /// </summary>
        public static Func<LocalTime, LocalTime> TruncateToMinute { get { return truncateToMinute; } }

        /// <summary>
        /// A time adjuster to truncate the time to the hour, discarding fractional hours.
        /// </summary>
        public static Func<LocalTime, LocalTime> TruncateToHour { get { return truncateToHour; } }
    }
}
