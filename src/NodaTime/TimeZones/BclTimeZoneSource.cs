#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using System.Linq;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// An <see cref="IDateTimeZoneSource" /> implementation which uses <see cref="TimeZoneInfo"/> from
    /// .NET 3.5 and later.
    /// </summary>
    /// <threadsafety>This type maintains no state, and all members are thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    public class BclTimeZoneSource : IDateTimeZoneSource
    {
        /// <summary>
        /// Returns the IDs of all system time zones.
        /// </summary>
        public IEnumerable<string> GetIds()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(zone => zone.Id);
        }

        /// <summary>
        /// Returns version information corresponding to the version of the assembly
        /// containing <see cref="TimeZoneInfo"/>.
        /// </summary>
        public string VersionId
        {
            get { return "TimeZoneInfo: " + typeof(TimeZoneInfo).Assembly.GetName().Version; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="BclTimeZone" /> from the <see cref="TimeZoneInfo"/> with the given
        /// ID. The ID must be a known system time zone ID.
        /// </summary>
        /// <param name="id">The ID of the system time zone to convert</param>
        /// <exception cref="ArgumentException">The given zone doesn't exist</exception>
        /// <returns>The Noda Time representation of the given Windows system time zone</returns>
        public DateTimeZone ForId(string id)
        {
            try
            {
                TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(id);
                return BclTimeZone.FromTimeZoneInfo(zone);
            }
            catch (TimeZoneNotFoundException)
            {                
                throw new ArgumentException(id + " is not a system time zone ID", "id");
            }
        }

        /// <summary>
        /// Maps the BCL ID to "our" ID as an identity projection.
        /// </summary>
        public string MapTimeZoneId(TimeZoneInfo timeZone)
        {
            return timeZone.Id;
        }
    }
}
