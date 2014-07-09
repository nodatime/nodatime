// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Collections.Generic;
using NodaTime.TimeZones.Cldr;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Provides the raw data exposed by <see cref="TzdbDateTimeZoneSource"/>. It would
    /// be simpler to use inheritance in this case with static factory methods, but
    /// there are already public constructors exposed - so to delegate to different
    /// implementations, we need a separate hierarchy :(
    /// FIXME(2.0): So let's remove the constructors...
    /// </summary>
    internal interface ITzdbDataSource
    {
        /// <summary>
        /// Returns the TZDB version string.
        /// </summary>
        string TzdbVersion { get; }
        
        /// <summary>
        /// Returns the TZDB ID dictionary (alias to canonical ID). This needn't be read-only; it won't be
        /// exposed directly.
        /// </summary>
        IDictionary<string, string> TzdbIdMap { get; }

        /// <summary>
        /// Returns the Windows mapping dictionary. (As the type is immutable, it can be exposed directly
        /// to callers.)
        /// </summary>
        WindowsZones WindowsMapping { get; }

        /// <summary>
        /// Returns the zone locations for the source, or null if no location data is available.
        /// This needn't be read-only; it won't be exposed directly.
        /// </summary>
        IList<TzdbZoneLocation> ZoneLocations { get; }

        /// <summary>
        /// Creates the <see cref="DateTimeZone"/> for the given canonical ID, which will definitely
        /// be one of the values of the TzdbAliases dictionary.
        /// </summary>
        /// <param name="id">ID for the returned zone, which may be an alias.</param>
        /// <param name="canonicalId">Canonical ID for zone data</param>
        DateTimeZone CreateZone(string id, string canonicalId);

        /// <summary>
        /// Additional mappings from Windows standard name to TZDB ID. Primarily used in
        /// the PCL build, where we can't get at the system ID. This never returns null.
        /// </summary>
        IDictionary<string, string> WindowsAdditionalStandardNameToIdMapping { get; }
    }
}
