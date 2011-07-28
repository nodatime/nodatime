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

using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Extension methods to help with time zone testing, and other helper methods.
    /// </summary>
    internal static class TzTestHelper
    {
        /// <summary>
        /// Returns the uncached version of the given zone. If the zone isn't
        /// an instance of CachedDateTimeZone, the same reference is returned back.
        /// </summary>
        internal static DateTimeZone Uncached(this DateTimeZone zone)
        {
            var cached = zone as CachedDateTimeZone;
            return cached == null ? zone : cached.TimeZone;
        }
    }
}