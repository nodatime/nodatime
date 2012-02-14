#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using System;
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides the interface for objects that can retrieve time zone definitions given and id.
    /// </summary>
    /// <remarks>
    /// Note that the ID "UTC" is reserved - it's the only zone which is guaranteed to be present.
    /// Even if the provider advertises that it knows about the UTC zone, it will never be asked for it;
    /// even if it *doesn't* advertise that it knows about UTC, <see cref="DateTimeZone.Ids" /> will always
    /// include it.
    /// </remarks>
    /// <remarks>
    /// Implementations should be accessible from any thread, but will not usually be accessed from more than
    /// one thread concurrently. (Clients could call <see cref="DateTimeZone.SetProvider"/> with the same
    /// provider in multiple threads, but that would be extremely unlikely.)
    /// </remarks>
    public interface IDateTimeZoneProvider
    {
        /// <summary>
        /// Returns an enumeration of the available ids from this provider. The order in which the
        /// values are returned is irrelevant, as the time zone cache will sort them anyway. The sequence
        /// returned may be empty, but must not be null.
        /// </summary>
        /// <value>The <see cref="IEnumerable{T}"/> of ids.</value>
        IEnumerable<string> Ids { get; }

        /// <summary>
        /// Returns an appropriate version ID for diagnostic purposes. This doesn't have any specific format;
        /// it's solely for diagnostic purposes. For example, the default provider returns a string such as
        /// "TZDB: 2011n" indicating where the information comes from and which version of that information
        /// it's loaded.
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <remarks>
        /// If the time zone does not yet exist, its definition is loaded from where ever this
        /// provider gets time zone definitions. The provider should not attempt to cache time zones;
        /// the time zone cache handles that automatically.
        /// </remarks>
        /// <param name="id">The id of the time zone to return. This will be one of the IDs
        /// returned by the <see cref="Ids"/> property.</param>
        /// <returns>The <see cref="DateTimeZone"/> for the given ID; must not be null.</returns>
        DateTimeZone ForId(string id);

        /// <summary>
        /// Returns this provider's corresponding ID for the given time zone.
        /// </summary>
        /// <returns>
        /// The ID for the system default time zone for this provider, or null if the default time
        /// zone has no mapping in this provider.
        /// </returns>
        string MapTimeZoneId(TimeZoneInfo timeZone);
    }
}
