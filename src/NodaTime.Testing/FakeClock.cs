// Copyright 2011 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Testing
{
    /// <summary>
    /// Clock which can be constructed with an initial instant, and then advanced programmatically (and optionally,
    /// automatically advanced on each read).
    /// This class is designed to be used when testing classes which take an <see cref="IClock"/> as a dependency.
    /// </summary>
    /// <remarks>
    /// This class is somewhere between a fake and a stub, depending on how it's used - if it's set to
    /// <see cref="AutoAdvance"/> then time will pass, but in a pretty odd way (i.e. dependent on how
    /// often it's consulted).
    /// </remarks>
    /// <threadsafety>
    /// This type is thread-safe, primarily in order to allow <see cref="IClock"/> to be documented as
    /// "thread safe in all built-in implementations".
    /// </threadsafety>
    public sealed class FakeClock : IClock
    {
        private readonly object mutex = new object();
        private Instant now;
        private Duration autoAdvance = Duration.Zero;

        /// <summary>
        /// Creates a fake clock initially set to the given instant, with no auto-advance.
        /// </summary>
        /// <param name="initial">The initial instant.</param>
        public FakeClock(Instant initial) : this(initial, Duration.Zero)
        {            
        }

        /// <summary>
        /// Creates a fake clock initially set to the given instant. The clock will advance by the given duration on
        /// each read.
        /// </summary>
        /// <param name="initial">The initial instant.</param>
        /// <param name="autoAdvance">The duration to advance the clock on each read.</param>
        /// <seealso cref="AutoAdvance"/>
        public FakeClock(Instant initial, Duration autoAdvance)
        {
            now = initial;
            this.autoAdvance = autoAdvance;
        }

        /// <summary>
        /// Returns a fake clock initially set to midnight of the given year/month/day in UTC in the ISO calendar.
        /// The value of the <see cref="AutoAdvance"/> property will be initialised to zero.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <returns>A <see cref="FakeClock"/> initialised to the given instant, with no auto-advance.</returns>
        public static FakeClock FromUtc(int year, int monthOfYear, int dayOfMonth)
        {
            return new FakeClock(Instant.FromUtc(year, monthOfYear, dayOfMonth, 0, 0));
        }

        /// <summary>
        /// Returns a fake clock initially set to the given year/month/day/time in UTC in the ISO calendar.
        /// The value of the <see cref="AutoAdvance"/> property will be initialised to zero.
        /// </summary>
        /// <param name="year">The year. This is the "absolute year",
        /// so a value of 0 means 1 BC, for example.</param>
        /// <param name="monthOfYear">The month of year.</param>
        /// <param name="dayOfMonth">The day of month.</param>
        /// <param name="hourOfDay">The hour.</param>
        /// <param name="minuteOfHour">The minute.</param>
        /// <param name="secondOfMinute">The second.</param>
        /// <returns>A <see cref="FakeClock"/> initialised to the given instant, with no auto-advance.</returns>
        public static FakeClock FromUtc(int year, int monthOfYear, int dayOfMonth, int hourOfDay, int minuteOfHour, int secondOfMinute)
        {
            return new FakeClock(Instant.FromUtc(year, monthOfYear, dayOfMonth, hourOfDay, minuteOfHour, secondOfMinute));
        }

        /// <summary>
        /// Advances the clock by the given duration.
        /// </summary>
        /// <param name="duration">The duration to advance the clock by (or if negative, the duration to move it back
        /// by).</param>
        public void Advance(Duration duration)
        {
            lock (mutex)
            {
                now += duration;
            }
        }

        /// <summary>
        /// Advances the clock by the given number of nanoseconds.
        /// </summary>
        /// <param name="nanoseconds">The number of nanoseconds to advance the clock by (or if negative, the number to move it back
        /// by).</param>
        public void AdvanceNanoseconds(long nanoseconds) => Advance(Duration.FromNanoseconds(nanoseconds));        

        /// <summary>
        /// Advances the clock by the given number of ticks.
        /// </summary>
        /// <param name="ticks">The number of ticks to advance the clock by (or if negative, the number to move it back
        /// by).</param>
        public void AdvanceTicks(long ticks) => Advance(Duration.FromTicks(ticks));

        /// <summary>
        /// Advances the clock by the given number of milliseconds.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to advance the clock by (or if negative, the number
        /// to move it back by).</param>
        public void AdvanceMilliseconds(long milliseconds) => Advance(Duration.FromMilliseconds(milliseconds));

        /// <summary>
        /// Advances the clock by the given number of seconds.
        /// </summary>
        /// <param name="seconds">The number of seconds to advance the clock by (or if negative, the number to move it
        /// back by).</param>
        public void AdvanceSeconds(long seconds) => Advance(Duration.FromSeconds(seconds));

        /// <summary>
        /// Advances the clock by the given number of minutes.
        /// </summary>
        /// <param name="minutes">The number of minutes to advance the clock by (or if negative, the number to move it
        /// back by).</param>
        public void AdvanceMinutes(long minutes) => Advance(Duration.FromMinutes(minutes));

        /// <summary>
        /// Advances the clock by the given number of hours.
        /// </summary>
        /// <param name="hours">The number of hours to advance the clock by (or if negative, the number to move it
        /// back by).</param>
        public void AdvanceHours(int hours) => Advance(Duration.FromHours(hours));

        /// <summary>
        /// Advances the clock by the given number of standard (24-hour) days.
        /// </summary>
        /// <param name="days">The number of days to advance the clock by (or if negative, the number to move it
        /// back by).</param>
        public void AdvanceDays(int days) => Advance(Duration.FromDays(days));

        /// <summary>
        /// Resets the clock to the given instant.
        /// The value of the <see cref="AutoAdvance"/> property will be unchanged.
        /// </summary>
        /// <param name="instant">The instant to set the clock to.</param>
        public void Reset(Instant instant)
        {
            lock (mutex)
            {
                now = instant;
            }
        }

        /// <summary>
        /// Returns the "current time" for this clock. Unlike a normal clock, this
        /// property may return the same value from repeated calls until one of the methods
        /// to change the time is called.
        /// </summary>
        /// <remarks>
        /// If the value of the <see cref="AutoAdvance"/> property is non-zero, then every
        /// call to this method will advance the current time by that value.
        /// </remarks>
        /// <returns>The "current time" from this (fake) clock.</returns>
        public Instant GetCurrentInstant()
        {
            lock (mutex)
            {
                Instant then = now;
                now += autoAdvance;
                return then;
            }
        }

        /// <summary>
        /// Gets the amount of time to advance the clock by on each call to read the current time.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This defaults to zero, with the exception of the <see cref="FakeClock(Instant, Duration)"/> constructor,
        /// which takes the initial value directly.  If this is zero, the current time as reported by this clock will
        /// not change other than by calls to <see cref="Reset"/> or to one of the <see cref="Advance"/> methods.
        /// </para>
        /// <para>
        /// The value could even be negative, to simulate particularly odd system clock effects.
        /// </para>
        /// </remarks>
        /// <seealso cref="GetCurrentInstant"/>
        /// <value>The amount of time to advance the clock by on each call to read the current time.</value>
        public Duration AutoAdvance
        {
            get
            {
                lock (mutex)
                {
                    return autoAdvance;
                }
            }
            set
            {
                lock (mutex)
                {
                    autoAdvance = value;
                }
            }
        }
    }
}
