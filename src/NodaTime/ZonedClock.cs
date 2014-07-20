// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A clock with an associated time zone and calendar. This is effectively a convenience
    /// class decorating an <see cref="IClock"/>.
    /// </summary>
    public sealed class ZonedClock
    {
        private readonly IClock clock;
        private readonly DateTimeZone zone;
        private readonly CalendarSystem calendar;

        /// <summary>
        /// Creates a new <see cref="ZonedClock"/> with the given clock, time zone and calendar system.
        /// </summary>
        /// <param name="clock">Clock to use to obtain instants.</param>
        /// <param name="zone">Time zone to adjust instants into.</param>
        /// <param name="calendar">Calendar system to use.</param>
        public ZonedClock([NotNull] IClock clock, [NotNull] DateTimeZone zone, [NotNull] CalendarSystem calendar)
        {
            this.clock = Preconditions.CheckNotNull(clock, "clock");
            this.zone = Preconditions.CheckNotNull(zone, "zone");
            this.calendar = Preconditions.CheckNotNull(calendar, "calendar");
        }

        /// <summary>
        /// Returns the current instant provided by the underlying clock.
        /// </summary>
        /// <returns>The current instant provided by the underlying clock.</returns>
        public Instant GetCurrentInstant()
        {
            return clock.Now;
        }

        /// <summary>
        /// Returns the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        public ZonedDateTime GetCurrentZonedDateTime()
        {
            return GetCurrentInstant().InZone(zone, calendar);
        }

        /// <summary>
        /// Returns the local date/time of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The local date/time of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        public LocalDateTime GetCurrentLocalDateTime()
        {
            return GetCurrentZonedDateTime().LocalDateTime;
        }

        /// <summary>
        /// Returns the offset date/time of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The offset date/time of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        public OffsetDateTime GetCurrentOffsetDateTime()
        {
            return GetCurrentZonedDateTime().ToOffsetDateTime();
        }

        /// <summary>
        /// Returns a new <see cref="ZonedClock"/> with the same underlying clock
        /// and calendar system as this one, but using the specified time zone.
        /// This does not change the current object.
        /// </summary>
        /// <param name="zone">The time zone for the new <c>ZonedClock</c>.</param>
        /// <returns>A new <c>ZonedClock</c> using the specified time zone.</returns>
        public ZonedClock WithZone([NotNull] DateTimeZone zone)
        {
            return new ZonedClock(clock, zone, calendar);
        }

        /// <summary>
        /// Returns a new <see cref="ZonedClock"/> with the same underlying clock
        /// and time zone as this one, but using the specified calendar system.
        /// This does not change the current object.
        /// </summary>
        /// <param name="calendar">The calendar system for the new <c>ZonedClock</c>.</param>
        /// <returns>A new <c>ZonedClock</c> using the specified calendar system.</returns>
        public ZonedClock WithCalendar([NotNull] CalendarSystem calendar)
        {
            return new ZonedClock(clock, zone, calendar);
        }
    }
}
