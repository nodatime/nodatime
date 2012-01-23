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
    /// Time zone provider which never provides any time zones. Setting this as the provider
    /// for the system will effectively mean that only UTC is recognized.
    /// </summary>
    public sealed class EmptyDateTimeZoneProvider : IDateTimeZoneProvider
    {
        /// <summary>
        /// Always returns an empty array.
        /// </summary>
        public IEnumerable<string> Ids
        {
            get { return new string[0];  }
        }

        /// <summary>
        /// Always throws <see cref="InvalidOperationException"/> as no time zones are supported.
        /// (A provider should not be asked for time zones it doesn't support.)
        /// </summary>
        public DateTimeZone ForId(string id)
        {
            throw new InvalidOperationException("EmptyDateTimeZoneProvider should never be asked for a zone");
        }

        /// <summary>
        /// Returns a version identifier for this provider.
        /// </summary>
        public string VersionId { get { return "Empty (UTC-only)"; } }
    }
}
