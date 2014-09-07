// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.Annotations;
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
    /// In theory this isn't calendar-specific; the local value will be ambiguous in
    /// this time zone regardless of the calendar used. However, this exception is
    /// always created in conjunction with a specific calendar, which leads to a more
    /// natural way of examining its information and constructing an error message.
    /// </para>
    /// </remarks>
    /// <threadsafety>Any public static members of this type are thread safe. Any instance members are not guaranteed to be thread safe.
    /// See the thread safety section of the user guide for more information.
    /// </threadsafety>
#if !PCL
    [Serializable]
#endif
    [Mutable] // Exception itself is mutable
    public sealed class AmbiguousTimeException : ArgumentOutOfRangeException
    {
        private readonly ZonedDateTime earlierMapping;
        private readonly ZonedDateTime laterMapping;

        /// <summary>
        /// Get the local date and time which is ambiguous in the time zone.
        /// </summary>
        /// <value>The local date and time which is ambiguous in the time zone.</value>
        internal LocalDateTime LocalDateTime { get { return earlierMapping.LocalDateTime; } }

        /// <summary>
        /// The time zone in which the local date and time is ambiguous.
        /// </summary>
        /// <value>The time zone in which the local date and time is ambiguous.</value>
        [NotNull] public DateTimeZone Zone { get { return earlierMapping.Zone; } }

        /// <summary>
        /// Gets the earlier of the two occurrences of the local date and time within the time zone.
        /// </summary>
        /// <value>The earlier of the two occurrences of the local date and time within the time zone.</value>
        public ZonedDateTime EarlierMapping { get { return earlierMapping; } }

        /// <summary>
        /// Gets the later of the two occurrences of the local date and time within the time zone.
        /// </summary>
        /// <value>The later of the two occurrences of the local date and time within the time zone.</value>
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
