// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.


namespace NodaTime.Testing.Extensions
{
    /// <summary>
    /// Extension methods for constructing <see cref="Duration"/> values.
    /// </summary>
    public static class DurationConstruction
    {
        /// <summary>
        /// Constructs a duration with the specified number of days.
        /// </summary>
        /// <param name="days">The desired number of days.</param>
        /// <returns>A duration with the specified number of days.</returns>
        public static Duration Days(this double days) => Duration.FromDays(days);

        /// <summary>
        /// Constructs a duration with the specified number of days.
        /// </summary>
        /// <param name="days">The desired number of days.</param>
        /// <returns>A duration with the specified number of days.</returns>
        public static Duration Days(this int days) => Duration.FromDays(days);

        /// <summary>
        /// Constructs a duration with the specified number of hours.
        /// </summary>
        /// <param name="hours">The desired number of hours.</param>
        /// <returns>A duration with the specified number of hours.</returns>
        public static Duration Hours(this double hours) => Duration.FromHours(hours);
        /// <summary>
        /// Constructs a duration with the specified number of hours.
        /// </summary>
        /// <param name="hours">The desired number of hours.</param>
        /// <returns>A duration with the specified number of hours.</returns>
        public static Duration Hours(this int hours) => Duration.FromHours(hours);

        /// <summary>
        /// Constructs a duration with the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The desired number of minutes.</param>
        /// <returns>A duration with the specified number of minutes.</returns>
        public static Duration Minutes(this double minutes) => Duration.FromMinutes(minutes);

        /// <summary>
        /// Constructs a duration with the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The desired number of minutes.</param>
        /// <returns>A duration with the specified number of minutes.</returns>
        public static Duration Minutes(this int minutes) => Duration.FromMinutes(minutes);

        /// <summary>
        /// Constructs a duration with the specified number of minutes.
        /// </summary>
        /// <param name="minutes">The desired number of minutes.</param>
        /// <returns>A duration with the specified number of minutes.</returns>
        public static Duration Minutes(this long minutes) => Duration.FromMinutes(minutes);

        /// <summary>
        /// Constructs a duration with the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The desired number of seconds.</param>
        /// <returns>A duration with the specified number of seconds.</returns>
        public static Duration Seconds(this double seconds) => Duration.FromSeconds(seconds);

        /// <summary>
        /// Constructs a duration with the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The desired number of seconds.</param>
        /// <returns>A duration with the specified number of seconds.</returns>
        public static Duration Seconds(this int seconds) => Duration.FromSeconds(seconds);

        /// <summary>
        /// Constructs a duration with the specified number of seconds.
        /// </summary>
        /// <param name="seconds">The desired number of seconds.</param>
        /// <returns>A duration with the specified number of seconds.</returns>
        public static Duration Seconds(this long seconds) => Duration.FromSeconds(seconds);

        /// <summary>
        /// Constructs a duration with the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The desired number of milliseconds.</param>
        /// <returns>A duration with the specified number of milliseconds.</returns>
        public static Duration Milliseconds(this double milliseconds) => Duration.FromMilliseconds(milliseconds);

        /// <summary>
        /// Constructs a duration with the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The desired number of milliseconds.</param>
        /// <returns>A duration with the specified number of milliseconds.</returns>
        public static Duration Milliseconds(this int milliseconds) => Duration.FromMilliseconds(milliseconds);

        /// <summary>
        /// Constructs a duration with the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The desired number of milliseconds.</param>
        /// <returns>A duration with the specified number of milliseconds.</returns>
        public static Duration Milliseconds(this long milliseconds) => Duration.FromMilliseconds(milliseconds);

        /// <summary>
        /// Constructs a duration with the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The desired number of ticks.</param>
        /// <returns>A duration with the specified number of ticks.</returns>
        public static Duration Ticks(this double ticks) => Duration.FromTicks(ticks);

        /// <summary>
        /// Constructs a duration with the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The desired number of ticks.</param>
        /// <returns>A duration with the specified number of ticks.</returns>
        public static Duration Ticks(this int ticks) => Duration.FromTicks(ticks);

        /// <summary>
        /// Constructs a duration with the specified number of ticks.
        /// </summary>
        /// <param name="ticks">The desired number of ticks.</param>
        /// <returns>A duration with the specified number of ticks.</returns>
        public static Duration Ticks(this long ticks) => Duration.FromTicks(ticks);

        /// <summary>
        /// Constructs a duration with the specified number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The desired number of nanoseconds.</param>
        /// <returns>A duration with the specified number of nanoseconds.</returns>
        public static Duration Nanoseconds(this double nanoseconds) => Duration.FromNanoseconds(nanoseconds);

        /// <summary>
        /// Constructs a duration with the specified number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The desired number of nanoseconds.</param>
        /// <returns>A duration with the specified number of nanoseconds.</returns>
        public static Duration Nanoseconds(this int nanoseconds) => Duration.FromNanoseconds(nanoseconds);

        /// <summary>
        /// Constructs a duration with the specified number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The desired number of nanoseconds.</param>
        /// <returns>A duration with the specified number of nanoseconds.</returns>
        public static Duration Nanoseconds(this long nanoseconds) => Duration.FromNanoseconds(nanoseconds);
    }
}
