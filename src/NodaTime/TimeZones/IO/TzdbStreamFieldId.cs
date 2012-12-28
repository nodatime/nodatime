#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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
        /// Single field giving the mapping of Windows system ID to TZDB canonical ID,
        /// as written by DateTimeZoneWriter.WriteDictionary.
        /// </summary>
        WindowsMappingVersion = 4,
        /// <summary>
        /// Single field giving the version of Windows Mapping source data from CLDR. A string value which does *not* use
        /// the string pool.
        /// </summary>
        WindowsMapping = 5
    }
}
