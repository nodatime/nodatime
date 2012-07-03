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
    /// Provides the interface for objects that can retrieve time zone definitions given an id.
    /// </summary>
    /// <remarks>
    /// Note that the ID "UTC" is reserved in NodaTime - even if the provider advertises that it knows about the UTC zone, 
    /// other NodaTime types (such as <see cref="DateTimeZoneFactory"/> will never ask the provider for it.
    /// </remarks>
    /// <remarks>
    /// <para>
    /// The interface presumes that the available time zones are static; there is no mechanism for 
    /// updating the list of available time zones. Any time zone ID that is returned in <see cref="Ids"/> 
    /// must be resolved by <see cref="ForId"/> for the life of the provider.
    /// </para>
    /// <para>
    /// Implementations need not cache time zones or the available time zone IDs. 
    /// Caching is provided by <see cref="DateTimeZoneFactory"/>, which most consumers should use instead of 
    /// consuming <see cref="IDateTimeZoneProvider"/> directly in order to get better performance.
    /// </para>
    /// </remarks>
    /// <threadsafety>Implementations are not required to be thread-safe.</threadsafety>
    public interface IDateTimeZoneProvider
    {
        /// <summary>
        /// Returns an enumeration of the available ids from this provider.
        /// </summary>
        /// <remarks>
        /// Every value in this enumeration must return a valid time zone from <see cref="ForId"/> for the life of the provider.
        /// </remarks>
        /// <value>The <see cref="IEnumerable{T}"/> of ids. It may be empty, but must not be <see langword="null"/>, 
        /// and must not contain any elements which are <see langword="null"/>.</value>
        IEnumerable<string> Ids { get; }

        /// <summary>
        /// Returns an appropriate version ID for diagnostic purposes, which must not be null.
        /// This doesn't have any specific format; it's solely for diagnostic purposes.
        /// For example, the default provider returns a string such as
        /// "TZDB: 2011n" indicating where the information comes from and which version of that information
        /// it's loaded.
        /// </summary>
        string VersionId { get; }

        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <remarks>
        /// If the time zone does not yet exist, its definition should be loaded from where ever this
        /// provider gets time zone definitions. The provider should not attempt to cache time zones;
        /// caching is provided by <see cref="DateTimeZoneFactory"/>.
        /// </remarks>
        /// <param name="id">The id of the time zone to return. This must be one of the IDs
        /// returned in the <see cref="Ids"/> enumeration.</param>
        /// <returns>The <see cref="DateTimeZone"/> for the given ID; must not be <see langword="null"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="id"/> is <see langword="null"/>.</exception>
        DateTimeZone ForId(string id);

        /// <summary>
        /// Returns this provider's corresponding ID for the given BCL time zone.
        /// </summary>
        /// <returns>
        /// The ID for the system default time zone for this provider, or <see langword="null"/> if the default time
        /// zone has no mapping in this provider.
        /// </returns>
        string MapTimeZoneId(TimeZoneInfo timeZone);
    }
}
