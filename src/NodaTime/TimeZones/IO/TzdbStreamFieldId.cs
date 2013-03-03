// Copyright 2012 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Enumeration of the fields which can occur in a TZDB stream file.
    /// This enables the file to be self-describing to a reasonable extent.
    /// </summary>
    internal enum TzdbStreamFieldId : byte
    {
        /// <summary>
        /// String pool. Format is: number of strings (WriteCount) followed by that many string values.
        /// The indexes into the resultant list are used for other strings in the file, in some fields.
        /// </summary>
        StringPool = 0,
        /// <summary>
        /// Repeated field of time zones. Format is: zone ID, then zone as written by DateTimeZoneWriter.
        /// </summary>
        TimeZone = 1,
        /// <summary>
        /// Single field giving the version of the TZDB source data. A string value which does *not* use the string pool.
        /// </summary>
        TzdbVersion = 2,
        /// <summary>
        /// Single field giving the mapping of ID to canonical ID, as written by DateTimeZoneWriter.WriteDictionary.
        /// </summary>
        TzdbIdMap = 3,
        /// <summary>
        /// Single field containing mapping data as written by WindowsZones.Write.
        /// </summary>
        CldrSupplementalWindowsZones = 4,
        /// <summary>
        /// Single field giving the mapping of Windows StandardName to TZDB canonical ID,
        /// for time zones where TimeZoneInfo.Id != TimeZoneInfo.StandardName,
        /// as written by DateTimeZoneWriter.WriteDictionary.
        /// </summary>
        WindowsAdditionalStandardNameToIdMapping = 5,
        /// <summary>
        /// Single field providing all geolocations. The format is simply a count, and then that many copies of
        /// TzdbGeoLocation data.
        /// </summary>
        GeoLocations = 6
    }
}
