#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides the interface for objects that can retrieve time zone definitions given and id.
    /// </summary>
    /// <remarks>
    /// Original name: Provider. (I felt this was too general, and likely to clash with IOC
    /// containers etc.)
    /// </remarks>
    public interface IDateTimeZoneProvider
    {
        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <remarks>
        /// If the time zone does not yet exist, its definition is loaded from where ever this
        /// provider gets time zone definitions. Time zones should not be cached in the provider as
        /// they will be cached in <see cref="DateTimeZones"/>.
        /// </remarks>
        /// <param name="id">The id of the time zone to return.</param>
        /// <returns>The <see cref="IDateTimeZone"/> or <c>null</c> if there is no time zone with the given id.</returns>
        IDateTimeZone ForId(string id);

        /// <summary>
        /// Returns an enumeration of the available ids from this provider.
        /// </summary>
        /// <value>The <see cref="IEnumerable"/> of ids.</value>
        IEnumerable<string> Ids { get; }
    }
}
