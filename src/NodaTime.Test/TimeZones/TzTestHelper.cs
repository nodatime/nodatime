// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Extension methods to help with time zone testing, and other helper methods.
    /// </summary>
    internal static class TzTestHelper
    {
        /// <summary>
        /// Returns the uncached version of the given zone. If the zone isn't
        /// an instance of CachedDateTimeZone, the same reference is returned back.
        /// </summary>
        internal static DateTimeZone Uncached(this DateTimeZone zone)
        {
            var cached = zone as CachedDateTimeZone;
            return cached == null ? zone : cached.TimeZone;
        }
    }
}