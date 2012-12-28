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

using System.Collections.Generic;

namespace NodaTime.TimeZones.IO
{
    /// <summary>
    /// Provides the raw data exposed by <see cref="TzdbDateTimeZoneSource"/>. It would
    /// be simpler to use inheritance in this case with static factory methods, but
    /// there are already public constructors exposed - so to delegate to different
    /// implementations, we need a separate hierarchy :(
    /// </summary>
    internal interface ITzdbDataSource
    {
        /// <summary>
        /// Returns the TZDB version string.
        /// </summary>
        string TzdbVersion { get; }

        /// <summary>
        /// Returns the Windows mapping version string.
        /// </summary>
        string WindowsMappingVersion { get; }

        /// <summary>
        /// Returns the TZDB ID dictionary. This needn't be read-only; it won't be
        /// exposed directly.
        /// </summary>
        IDictionary<string, string> TzdbIdMap { get; }

        /// <summary>
        /// Returns the Windows mapping dictionary. This needn't be read-only; it won't
        /// be exposed directly.
        /// </summary>
        IDictionary<string, string> WindowsMapping { get; }

        /// <summary>
        /// Creates the <see cref="DateTimeZone"/> for the given canonical ID, which will definitely
        /// be one of the values of the TzdbAliases dictionary.
        /// </summary>
        /// <param name="id">ID for the returned zone, which may be an alias.</param>
        /// <param name="canonicalId">Canonical ID for zone data</param>
        DateTimeZone CreateZone(string id, string canonicalId);
    }
}
