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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides an implementation of a <see cref="IDateTimeZoneSource" /> that looks
    /// for its time zone definitions from a named resource in an assembly.
    /// </summary>
    /// <remarks>
    /// All calls to <see cref="ForId"/> for fixed-offset IDs advertsed by the source (i.e. "UTC" and "UTC+/-Offset")
    /// will return zones equal to those returned by <see cref="DateTimeZone.ForOffset"/>.
    /// </remarks>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    public sealed class TzdbDateTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// The key used to find the Windows to TZDB ID mappings.
        /// </summary>
        public const string WindowsToPosixMapKey = "Windows_To_Posix";

        /// <summary>
        /// The key used to find the Windows to TZDB ID version.
        /// </summary>
        public const string WindowsToPosixMapVersionKey = "Windows_To_Posix_Version";

        /// <summary>
        /// The key used to find ID mappings within the resource. Deliberately
        /// uses an unscore which would be normalized away if this were a time zone name.
        /// </summary>
        public const string IdMapKey = "Id_Map";

        /// <summary>
        /// The key used to find the TZDB version ID within the resource. Deliberately
        /// uses an unscore which would be normalized away if this were a time zone name.
        /// </summary>
        public const string VersionKey = "Version_Id";

        private readonly ResourceSet source;
        private readonly IDictionary<string, string> timeZoneIdMap;
        private readonly IDictionary<string, string> windowsIdMap;
        private readonly string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="baseName">GetName of the base.</param>
        public TzdbDateTimeZoneSource(string baseName) : this(baseName, Assembly.GetExecutingAssembly())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="baseName">GetName of the base.</param>
        /// <param name="assembly">The assembly to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(string baseName, Assembly assembly)
            : this(new ResourceManager(baseName, assembly))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="source">The <see cref="ResourceManager"/> to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(ResourceManager source)
            : this(ResourceHelper.GetDefaultResourceSet(source))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TzdbDateTimeZoneSource" /> class.
        /// </summary>
        /// <param name="source">The <see cref="ResourceSet"/> to search for the time zone resources.</param>
        public TzdbDateTimeZoneSource(ResourceSet source)
        {
            this.source = source;
            timeZoneIdMap = ResourceHelper.LoadDictionary(source, IdMapKey);
            if (timeZoneIdMap == null)
            {
                throw new InvalidDataException("No map with key " + IdMapKey + " in resource");
            }
            windowsIdMap = ResourceHelper.LoadDictionary(source, WindowsToPosixMapKey);
            // TODO(Post-V1): Consider forming inverse map too.
            if (windowsIdMap == null)
            {
                throw new InvalidDataException("No map with key " + WindowsToPosixMapKey + " in resource");
            }
            this.version = source.GetString(VersionKey) + " (mapping: " + source.GetString(WindowsToPosixMapVersionKey) + ")";
        }

        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <param name="id">The id of the time zone to return.</param>
        /// <returns>
        /// The <see cref="DateTimeZone"/> or null if there is no time zone with the given id.
        /// </returns>
        public DateTimeZone ForId(string id)
        {
            var queryId = timeZoneIdMap.ContainsKey(id) ? timeZoneIdMap[id] : id;
            return ResourceHelper.LoadTimeZone(source, queryId, id);
        }

        /// <summary>
        /// Returns a sequence of the available IDs from this source.
        /// </summary>
        [DebuggerStepThrough]
        public IEnumerable<string> GetIds()
        {
            return timeZoneIdMap.Keys;
        }

        /// <summary>
        /// Returns a version identifier for this source.
        /// </summary>
        public string VersionId { get { return "TZDB: " + version; } }

        /// <summary>
        /// Attempts to map the system time zone to a zoneinfo ID, and return that ID.
        /// </summary>
        public string MapTimeZoneId(TimeZoneInfo zone)
        {
            string result;
            windowsIdMap.TryGetValue(zone.Id, out result);
            return result;
        }
    }
}
