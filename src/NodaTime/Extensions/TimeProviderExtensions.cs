// Copyright 2024 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

// Note: currently this is only supported in the .NET 8.0 NodaTime build.
// We could potentially add a dependency on Microsoft.Bcl.TimeProvider and support
// it in all builds. We'll wait until that's requested though, to avoid another
// dependency to manage.

#if NET8_0_OR_GREATER
using NodaTime.TimeZones;
using NodaTime.Utility;
using System;

namespace NodaTime.Extensions;

/// <summary>
/// Extension methods for <see cref="TimeProvider"/>.
/// </summary>
public static class TimeProviderExtensions
{
    /// <summary>
    /// Returns an <see cref="IClock"/> implementation which delegates to <paramref name="timeProvider"/>
    /// to obtain the current instant in time.
    /// </summary>
    /// <param name="timeProvider">The time provider to obtain the current instant in time from. Must not be null.</param>
    /// <returns>An <see cref="IClock"/> implementation based on the given time provider.</returns>
    public static IClock ToClock(this TimeProvider timeProvider) => new TimeProviderClock(timeProvider);

    /// <summary>
    /// Returns the current instant in time provided by <paramref name="timeProvider"/>,
    /// as an <see cref="Instant"/>.
    /// </summary>
    /// <remarks>
    /// This is equivalent to calling <see cref="TimeProvider.GetUtcNow"/>, and then calling
    /// <see cref="DateTimeOffsetExtensions.ToInstant(DateTimeOffset)"/> on the result.
    /// </remarks>
    /// <param name="timeProvider">The time provider to obtain the current instant in time from. Must not be null.</param>
    /// <returns>The current instant in time as returned by the given time provider, expressed as an <see cref="Instant"/>.</returns>
    public static Instant GetCurrentInstant(this TimeProvider timeProvider) =>
        timeProvider.GetUtcNow().ToInstant();

    /// <summary>
    /// Returns a <see cref="ZonedClock"/> which obtains the current instant in time and the local time zone
    /// from <paramref name="timeProvider"/>, and uses the ISO calendar.
    /// </summary>
    /// <remarks>
    /// The local time zone is captured from the time provider at the time of this call. If the time provider
    /// changes which time zone is reported by <see cref="TimeProvider.LocalTimeZone"/> after this call has returned,
    /// that change will not be reflected in the returned <see cref="ZonedClock"/>.
    /// </remarks>
    /// <param name="timeProvider">The time provider to delegate to. Must not be null.</param>
    /// <returns>A <see cref="ZonedClock"/> backed by the given time provider.</returns>
    public static ZonedClock ToZonedClock(this TimeProvider timeProvider) => ToZonedClock(timeProvider, CalendarSystem.Iso);

    /// <summary>
    /// Returns a <see cref="ZonedClock"/> which obtains the current instant in time and the local time zone
    /// from <paramref name="timeProvider"/>, and uses the provided calendar system.
    /// </summary>
    /// <remarks>
    /// The local time zone is captured from the time provider at the time of this call. If the time provider
    /// changes which time zone is reported by <see cref="TimeProvider.LocalTimeZone"/> after this call has returned,
    /// that change will not be reflected in the returned <see cref="ZonedClock"/>.
    /// </remarks>
    /// <param name="timeProvider">The time provider to delegate to. Must not be null.</param>
    /// <param name="calendar">The calendar system to use in the returned <see cref="ZonedClock"/>. Must not be null.</param>
    /// <returns>A <see cref="ZonedClock"/> backed by the given time provider.</returns>
    public static ZonedClock ToZonedClock(this TimeProvider timeProvider, CalendarSystem calendar) =>
        new(timeProvider.ToClock(), BclDateTimeZone.FromTimeZoneInfo(timeProvider.LocalTimeZone), calendar);

    private class TimeProviderClock : IClock
    {
        private readonly TimeProvider timeProvider;

        internal TimeProviderClock(TimeProvider timeProvider)
        {
            this.timeProvider = Preconditions.CheckNotNull(timeProvider, nameof(timeProvider));
        }

        public Instant GetCurrentInstant() => timeProvider.GetCurrentInstant();
    }
}
#endif