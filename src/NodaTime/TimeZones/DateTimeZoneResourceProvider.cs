#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System.Reflection;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using System;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Original name: ZoneInfoProvider
    /// </summary>
    public sealed class DateTimeZoneResourceProvider
        : IDateTimeZoneProvider
    {
        public const string IdMapKey = "IdMap";

        private static readonly Regex invalidResourceNameCharacters = new Regex("[^A-Za-z0-9_/]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string baseName;
        private readonly ResourceManager manager;
        private readonly IDictionary<string, string> aliasMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneResourceProvider"/> class.
        /// </summary>
        /// <param name="baseName">Name of the base.</param>
        public DateTimeZoneResourceProvider(string baseName)
        {
            this.baseName = baseName;
            this.manager = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
            this.aliasMap = LoadTimeZoneAliasMap();
        }

        /// <summary>
        /// Loads the time zone id map.
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, string> LoadTimeZoneAliasMap()
        {
            byte[] bytes = this.manager.GetObject(NormalizeAsResourceName(IdMapKey)) as byte[];
            if (bytes != null)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    DateTimeZoneReader reader = new DateTimeZoneReader(stream);
                    return reader.ReadTimeZoneAliasMap();
                }
            }
            throw new ArgumentException("The resource file does not contain an Alias Map");
        }

        /// <summary>
        /// Normalizes the given time zone id into a valid resource name.
        /// </summary>
        /// <param name="id">The id to normalize.</param>
        /// <returns>The normalized name.</returns>
        public static string NormalizeAsResourceName(string id)
        {
            id = id.Replace("-", "_minus_");
            id = id.Replace("+", "_plus_");
            return invalidResourceNameCharacters.Replace(id, "_");
        }

        #region IDateTimeZoneProvider Members

        /// <summary>
        /// Returns the time zone definition associated with the given id.
        /// </summary>
        /// <param name="id">The id of the time zone to return.</param>
        /// <returns>
        /// The <see cref="IDateTimeZone"/> or <c>null</c> if there is no time zone with the given id.
        /// </returns>
        /// <remarks>
        /// If the time zone does not yet exist, its definition is loaded from where ever this
        /// provider gets time zone definitions. Time zones should not be cached in the provider as
        /// they will be cached in <see cref="DateTimeZones"/>.
        /// </remarks>
        public IDateTimeZone ForId(string id)
        {
            IDateTimeZone timeZone = null;
            string queryId = id;
            if (aliasMap.ContainsKey(queryId))
            {
                queryId = aliasMap[queryId];
            }
            byte[] bytes = this.manager.GetObject(NormalizeAsResourceName(queryId)) as byte[];
            if (bytes != null)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    DateTimeZoneReader reader = new DateTimeZoneReader(stream);
                    timeZone = reader.ReadTimeZone(id);
                }
            }
            return timeZone;
        }

        /// <summary>
        /// Returns an enumeration of the available ids from this provider.
        /// </summary>
        /// <value>The <see cref="IEnumerable"/> of ids.</value>
        public IEnumerable<string> Ids
        {
            get
            {
                IEnumerable<string> result = this.manager.GetObject(IdMapKey) as string[];
                if (result == null)
                {
                    result = new string[0];
                }
                return result;
            }
        }

        #endregion
    }
}
