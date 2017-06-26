// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.using System;
using NodaTime.Utility;
using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Extension methods for <see cref="IDateTimeZoneSource"/> to enable migration to Noda Time 2.0.
    /// </summary>
    public static class DateTimeZoneSourceExtensions
    {
        /// <summary>
        /// Returns this source's ID for the system default time zone.
        /// </summary>
        /// <param name="source">The source to consult its ID for the system default time zone</param>
        /// <returns>The ID for the system default time zone for this source, or null if the system default time zone has no mapping in this source.</returns>
        public static string GetSystemDefaultId(this IDateTimeZoneSource source) =>
            Preconditions.CheckNotNull(source, nameof(source)).MapTimeZoneId(TimeZoneInfo.Local);
    }
}
