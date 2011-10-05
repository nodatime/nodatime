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
    /// Exception thrown to indicate that the specified local time doesn't
    /// exist in a particular time zone due to daylight saving time changes.    
    /// </summary>
    /// <remarks>
    /// <para>
    /// This occurs for spring transitions, where the clock goes forward
    /// (usually by an hour). For example, suppose the time zone goes forward
    /// at 2am, so the second after 01:59:59 becomes 03:00:00. In that case,
    /// times such as 02:30:00 never occur.
    /// </para>
    /// <para>
    /// This exception is used to indicate such problems, as they're usually
    /// not the same as other <see cref="ArgumentOutOfRangeException" /> causes,
    /// such as entering "15" for a month number.
    /// </para>
    /// <para>
    /// In theory this isn't calendar-specific; the local instant won't exist in
    /// this time zone regardless of the calendar used. However, this exception is
    /// always created in conjunction with a specific calendar, which leads to a more
    /// natural way of examining its information and constructing an error message.
    /// </para>
    /// </remarks>
    [Serializable]
    public class SkippedTimeException : ArgumentOutOfRangeException
    {
        private readonly LocalDateTime localDateTime;
        private readonly DateTimeZone zone;

        /// <summary>
        /// The local instant which is invalid in the time zone
        /// </summary>
        public LocalDateTime LocalDateTime { get { return localDateTime; } }

        /// <summary>
        /// The time zone in which the local instant is invalid
        /// </summary>
        public DateTimeZone Zone { get { return zone; } }

        /// <summary>
        /// Creates a new instance for the given local date/time and time zone.
        /// </summary>
        /// <remarks>
        /// User code is unlikely to need to deliberately call this constructor except
        /// possibly for testing.
        /// </remarks>
        /// <param name="localDateTime">The local date time which is skipped in the specified time zone.</param>
        /// <param name="zone">The time zone in which the local date time does not exist.</param>
        public SkippedTimeException(LocalDateTime localDateTime, DateTimeZone zone) : base("Local time " + localDateTime + " is invalid in time zone " + zone.Id)
        {
            this.localDateTime = localDateTime;
            this.zone = zone;
        }
    }
}