// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using NodaTime.Annotations;
using NodaTime.TimeZones;

namespace NodaTime
{
    /// <summary>
    /// Singleton implementation of <see cref="IClock"/> which reads the current system time.
    /// It is recommended that for anything other than throwaway code, this is only referenced
    /// in a single place in your code: where you provide a value to inject into the rest of
    /// your application, which should only depend on the interface.
    /// </summary>
    /// <threadsafety>This type has no state, and is thread-safe. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class SystemClock : IClock
    {
        private static readonly SystemClock instance = new SystemClock();

        /// <summary>
        /// The singleton instance of <see cref="SystemClock"/>.
        /// </summary>
        /// <value>The singleton instance of <see cref="SystemClock"/>.</value>
        public static SystemClock Instance { get { return instance; } }

        /// <summary>
        /// Constructor present to prevent external construction.
        /// </summary>
        private SystemClock()
        {
        }

        /// <summary>
        /// Returns a <see cref="ZonedClock"/> backed by the system clock, in the UTC
        /// time zone and the ISO calendar system.
        /// </summary>
        /// <returns>A <c>ZonedClock</c> in the UTC time zone and ISO calendar system,
        /// using the system clock.</returns>
        public static ZonedClock GetUtcInstance()
        {
            return new ZonedClock(instance, DateTimeZone.Utc, CalendarSystem.Iso);
        }

        /// <summary>
        /// Returns a <see cref="ZonedClock"/> backed by the system clock, in the TZDB mapping for the
        /// system default time zone time zone and the ISO calendar system.
        /// </summary>
        /// <returns>A <c>ZonedClock</c> in the system default time zone (using TZDB) and the ISO calendar system,
        /// using the system clock.</returns>
        /// <exception cref="DateTimeZoneNotFoundException">The system default time zone is not mapped by
        /// TZDB.</exception>
        /// <seealso cref="DateTimeZoneProviders.Tzdb"/>
        public static ZonedClock GetSystemDefaultTzdbInstance()
        {
            var zone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            return new ZonedClock(instance, zone, CalendarSystem.Iso);
        }

#if !PCL
        /// <summary>
        /// Returns a <see cref="ZonedClock"/> backed by the system clock, in the wrapper for the
        /// BCL system default time zone time zone and the ISO calendar system.
        /// </summary>
        /// <remarks>The <c>DateTimeZone</c> used is a wrapper for <see cref="TimeZoneInfo.Local"/>.</remarks>
        /// <returns>A <c>ZonedClock</c> in the system default time zone and the ISO calendar system,
        /// using the system clock.</returns>
        /// <seealso cref="DateTimeZoneProviders.Bcl"/>
        public static ZonedClock GetSystemDefaultBclInstance()
        {
            var zone = DateTimeZoneProviders.Bcl.GetSystemDefault();
            return new ZonedClock(instance, zone, CalendarSystem.Iso);
        }
#endif

        /// <summary>
        /// Gets the current time as an <see cref="Instant"/>.
        /// </summary>
        /// <value>The current time in ticks as an <see cref="Instant"/>.</value>
        public Instant Now { get { return NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks); } }
    }
}
