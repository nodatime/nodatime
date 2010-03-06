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
using System.Reflection;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using System;
using NodaTime.Utility;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Original name: ZoneInfoProvider
    /// </summary>
    public sealed class DateTimeZoneResourceProvider
        : IDateTimeZoneProvider
    {
        public const string IdMapKey = "IdMap";

        private readonly ResourceManager manager;
        private readonly IDictionary<string, string> timeZoneIdMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneResourceProvider"/> class.
        /// </summary>
        /// <param name="baseName">GetName of the base.</param>
        public DateTimeZoneResourceProvider(string baseName)
        {
            this.manager = new ResourceManager(baseName, Assembly.GetExecutingAssembly());
            this.timeZoneIdMap = ResourceHelper.LoadDictionary(this.manager, IdMapKey);
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
            string queryId = id;
            if (timeZoneIdMap.ContainsKey(queryId))
            {
                queryId = timeZoneIdMap[queryId];
            }
            return ResourceHelper.LoadTimeZone(this.manager, queryId, id);
        }

        /// <summary>
        /// Returns an enumeration of the available ids from this provider.
        /// </summary>
        /// <value>The <see cref="IEnumerable"/> of ids.</value>
        public IEnumerable<string> Ids
        {
            get
            {
                return this.timeZoneIdMap.Keys;
            }
        }

        #endregion
    }
}
