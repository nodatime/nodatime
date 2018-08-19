// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using JetBrains.Annotations;
using NodaTime.TimeZones;

namespace NodaTime.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IClock"/>.
    /// </summary>
    public static class ClockExtensions
    {
        /// <summary>
        /// Constructs a <see cref="ZonedClock"/> from a clock (the target of the method),
        /// and a time zone.
        /// </summary>
        /// <param name="clock">Clock to use in the returned object.</param>
        /// <param name="zone">Time zone to use in the returned object.</param>
        /// <returns>A <see cref="ZonedClock"/> with the given clock and time zone, in the ISO calendar system.</returns>
        [NotNull]
        public static ZonedClock InZone([NotNull] this IClock clock, [NotNull] DateTimeZone zone) =>
            InZone(clock, zone, CalendarSystem.Iso);

        /// <summary>
        /// Constructs a <see cref="ZonedClock"/> from a clock (the target of the method),
        /// a time zone, and a calendar system.
        /// </summary>
        /// <param name="clock">Clock to use in the returned object.</param>
        /// <param name="zone">Time zone to use in the returned object.</param>
        /// <param name="calendar">Calendar to use in the returned object.</param>
        /// <returns>A <see cref="ZonedClock"/> with the given clock, time zone and calendar system.</returns>
        [NotNull]
        public static ZonedClock InZone([NotNull] this IClock clock,
            [NotNull] DateTimeZone zone,
            [NotNull] CalendarSystem calendar) => new ZonedClock(clock, zone, calendar);

        /// <summary>
        /// Constructs a <see cref="ZonedClock"/> from a clock (the target of the method),
        /// using the UTC time zone and ISO calendar system.
        /// </summary>
        /// <param name="clock">Clock to use in the returned object.</param>
        /// <returns>A <see cref="ZonedClock"/> with the given clock, in the UTC time zone and ISO calendar system.</returns>
        [NotNull]
        public static ZonedClock InUtc([NotNull] this IClock clock) =>
            new ZonedClock(clock, DateTimeZone.Utc, CalendarSystem.Iso);

        /// <summary>
        /// Constructs a <see cref="ZonedClock"/> from a clock (the target of the method),
        /// in the TZDB mapping for the system default time zone time zone and the ISO calendar system.
        /// </summary>
        /// <param name="clock">Clock to use in the returned object.</param>
        /// <returns>A <c>ZonedClock</c> in the system default time zone (using TZDB) and the ISO calendar system,
        /// using the system clock.</returns>
        /// <exception cref="DateTimeZoneNotFoundException">The system default time zone is not mapped by
        /// TZDB.</exception>
        /// <seealso cref="DateTimeZoneProviders.Tzdb"/>
        [NotNull]
        public static ZonedClock InTzdbSystemDefaultZone([NotNull] this IClock clock)
        {
            var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            return new ZonedClock(clock, zone, CalendarSystem.Iso);
        }

#if !NETSTANDARD && !NETSTANDARD2_0
        /// <summary>
        /// Constructs a <see cref="ZonedClock"/> from a clock (the target of the method), in the wrapper for the
        /// BCL system default time zone time zone and the ISO calendar system.
        /// </summary>
        /// <remarks>The <c>DateTimeZone</c> used is a wrapper for <see cref="TimeZoneInfo.Local"/>.</remarks>
        /// <param name="clock">Clock to use in the returned object.</param>
        /// <returns>A <c>ZonedClock</c> in the system default time zone and the ISO calendar system,
        /// using the system clock.</returns>
        /// <seealso cref="DateTimeZoneProviders.Bcl"/>
        [NotNull]
        public static ZonedClock InBclSystemDefaultZone([NotNull] this IClock clock)
        {
            var zone = DateTimeZoneProviders.Bcl.GetSystemDefault();
            return new ZonedClock(clock, zone, CalendarSystem.Iso);
        }
#endif

    }
}
