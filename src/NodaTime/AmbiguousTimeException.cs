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

namespace NodaTime
{
    /// <summary>
    /// Exception thrown to indicate that the specified local time occurs twice
    /// in a particular time zone due to daylight saving time changes.    
    /// </summary>
    /// <remarks>
    /// <para>
    /// This occurs for fall transitions, where the clock goes backward
    /// (usually by an hour). For example, suppose the time zone goes backward
    /// at 2am, so the second after 01:59:59 becomes 01:00:00. In that case,
    /// times such as 01:30:00 occur twice.
    /// </para>
    /// <para>
    /// This exception is used to indicate such problems, as they're usually
    /// not the same as other <see cref="ArgumentOutOfRangeException" /> causes,
    /// such as entering "15" for a month number.
    /// </para>
    /// </remarks>
    [Serializable]
    public class AmbiguousTimeException : ArgumentOutOfRangeException
    {
        private readonly LocalInstant localInstant;

        /// <summary>
        /// The local instant which is invalid in the time zone
        /// </summary>
        internal LocalInstant LocalInstant { get { return localInstant; } }

        private readonly DateTimeZone zone;

        /// <summary>
        /// The time zone in which the local instant is invalid
        /// </summary>
        public DateTimeZone Zone { get { return zone; } }

        internal AmbiguousTimeException(LocalInstant localInstant, DateTimeZone zone)
            : base("Local time " + localInstant + " is ambiguous in time zone " + zone.Id)
        {
            this.localInstant = localInstant;
            this.zone = zone;
        }

        public LocalDateTime GetInvalidLocalDateTime(CalendarSystem calendar)
        {
            return new LocalDateTime(LocalInstant, calendar);
        }

        // TODO: IsoLocalDateTime as a convenience property?
        // TODO: Does this only ever happen with reference to a user-specified calendar? If so, we can provide the LocalDateTime instead.
        // TODO: Can we provide the earlier and later possibilities?
    }
}
