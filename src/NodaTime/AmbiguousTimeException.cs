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
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// Exception thrown to indicate that the specified local date/time occurs twice
    /// in a particular time zone due to daylight saving time changes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This occurs for transitions where the clock goes backward (usually by
    /// an hour). For example, suppose the time zone goes backward
    /// at 2am, so the second after 01:59:59 becomes 01:00:00. In that case,
    /// times such as 01:30:00 occur twice.
    /// </para>
    /// <para>
    /// This exception is used to indicate such problems, as they're usually
    /// not the same as other <see cref="ArgumentOutOfRangeException" /> causes,
    /// such as entering "15" for a month number.
    /// </para>
    /// <para>
    /// In theory this isn't calendar-specific; the local instant will be ambiguous in
    /// this time zone regardless of the calendar used. However, this exception is
    /// always created in conjunction with a specific calendar, which leads to a more
    /// natural way of examining its information and constructing an error message.
    /// </para>
    /// </remarks>
    [Serializable]
    public class AmbiguousTimeException : ArgumentOutOfRangeException
    {
        private readonly ZonedDateTime earlierMapping;
        private readonly ZonedDateTime laterMapping;

        /// <summary>
        /// The local date and time which is ambiguous in the time zone.
        /// </summary>
        internal LocalDateTime LocalDateTime { get { return earlierMapping.LocalDateTime; } }

        /// <summary>
        /// The time zone in which the local date and time is ambiguous.
        /// </summary>
        public DateTimeZone Zone { get { return earlierMapping.Zone; } }

        /// <summary>
        /// The earlier of the two occurrences of the local date and time within the time zone.
        /// </summary>
        public ZonedDateTime EarlierMapping { get { return earlierMapping; } }

        /// <summary>
        /// The later of the two occurrences of the local date and time within the time zone.
        /// </summary>
        public ZonedDateTime LaterMapping { get { return laterMapping; } }

        /// <summary>
        /// Constructs an instance from the given information.
        /// </summary>
        /// <remarks>
        /// <para>
        /// User code is unlikely to need to deliberately call this constructor except
        /// possibly for testing.
        /// </para>
        /// <para>
        /// The two mappings must have the same local time and time zone.
        /// </para>
        /// </remarks>
        /// <param name="earlierMapping">The earlier possible mapping</param>
        /// <param name="laterMapping">The later possible mapping</param>
        public AmbiguousTimeException(ZonedDateTime earlierMapping, ZonedDateTime laterMapping)
            : base("Local time " + earlierMapping.LocalDateTime + " is ambiguous in time zone " + earlierMapping.Zone.Id)
        {
            this.earlierMapping = earlierMapping;
            this.laterMapping = laterMapping;
            Preconditions.CheckArgument(earlierMapping.Zone == laterMapping.Zone, "laterMapping",
                                        "Ambiguous possible values must use the same time zone");
            Preconditions.CheckArgument(earlierMapping.LocalDateTime == laterMapping.LocalDateTime, "laterMapping",
                                        "Ambiguous possible values must have the same local date/time");
        }
    }
}
