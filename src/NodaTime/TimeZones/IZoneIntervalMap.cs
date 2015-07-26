// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.TimeZones
{
    /// <summary>
    /// The core part of a DateTimeZone: mapping an Instant to an Interval.
    /// Separating this out into an interface allows for flexible caching.
    /// </summary>
    /// <remarks>
    /// Benchmarking shows that a delegate may be slightly faster here, but the difference
    /// isn't very significant even for very fast calls (cache hits). The interface ends up
    /// feeling slightly cleaner elsewhere in the code.
    /// </remarks>
    internal interface IZoneIntervalMap
    {
        ZoneInterval GetZoneInterval(Instant instant);
    }

    // This is slightly ugly, but it allows us to use any time zone as the tail
    // zone for PrecalculatedDateTimeZone, which is handy for testing.
    internal interface IZoneIntervalMapWithMinMax : IZoneIntervalMap
    {
        Offset MinOffset { get; }
        Offset MaxOffset { get; }
    }
}
