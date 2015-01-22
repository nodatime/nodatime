// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using JetBrains.Annotations;
using NodaTime.Annotations;
using NodaTime.Utility;

namespace NodaTime
{
    /// <summary>
    /// A clock with an associated time zone and calendar. This is effectively a convenience
    /// class decorating an <see cref="IClock"/>.
    /// </summary>
    /// <threadsafety>This type is immutable reference type. See the thread safety section of the user guide for more information.</threadsafety>
    [Immutable]
    public sealed class ZonedClock : IClock
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
            this.clock = Preconditions.CheckNotNull(clock, nameof(clock));
            this.zone = Preconditions.CheckNotNull(zone, nameof(zone));
            this.calendar = Preconditions.CheckNotNull(calendar, nameof(calendar));
        }

        /// <summary>
        /// Returns the current instant provided by the underlying clock.
        /// </summary>
        /// <returns>The current instant provided by the underlying clock.</returns>
        public Instant GetCurrentInstant() => clock.GetCurrentInstant();

        /// <summary>
        /// Returns the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        [Pure]
        public ZonedDateTime GetCurrentZonedDateTime() => GetCurrentInstant().InZone(zone, calendar);

        /// <summary>
        /// Returns the local date/time of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The local date/time of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        [Pure]
        public LocalDateTime GetCurrentLocalDateTime() => GetCurrentZonedDateTime().LocalDateTime;

        /// <summary>
        /// Returns the offset date/time of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The offset date/time of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        [Pure]
        public OffsetDateTime GetCurrentOffsetDateTime() => GetCurrentZonedDateTime().ToOffsetDateTime();

        /// <summary>
        /// Returns the local date of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The local date of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        [Pure]
        public LocalDate GetCurrentDate() => GetCurrentZonedDateTime().Date;

        /// <summary>
        /// Returns the local time of the current instant provided by the underlying clock, adjusted
        /// to the time zone of this object.
        /// </summary>
        /// <returns>The local time of the current instant provided by the underlying clock, adjusted to the
        /// time zone of this object.</returns>
        [Pure]
        public LocalTime GetCurrentTimeOfDay() => GetCurrentZonedDateTime().TimeOfDay;
    }
}
