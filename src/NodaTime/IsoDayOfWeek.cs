// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime
{
    /// <summary>
    /// Equates the days of the week with their numerical value according to
    /// ISO-8601. This corresponds with System.DayOfWeek except for Sunday, which
    /// is 7 in the ISO numbering and 0 in System.DayOfWeek.
    /// </summary>
    public enum IsoDayOfWeek
    {
        /// <summary>
        /// Value indicating no day of the week; this will never be returned
        /// by any IsoDayOfWeek property, and is not valid as an argument to
        /// any method.
        /// </summary>
        None = 0,
        /// <summary>
        /// Value representing Monday (1).
        /// </summary>
        Monday = 1,
        /// <summary>
        /// Value representing Tuesday (2).
        /// </summary>
        Tuesday = 2,
        /// <summary>
        /// Value representing Wednesday (3).
        /// </summary>
        Wednesday = 3,
        /// <summary>
        /// Value representing Thursday (4).
        /// </summary>
        Thursday = 4,
        /// <summary>
        /// Value representing Friday (5).
        /// </summary>
        Friday = 5,
        /// <summary>
        /// Value representing Saturday (6).
        /// </summary>
        Saturday = 6,
        /// <summary>
        /// Value representing Sunday (7).
        /// </summary>
        Sunday = 7
    }
}
