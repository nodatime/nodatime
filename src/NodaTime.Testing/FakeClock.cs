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
namespace NodaTime.Testing
{
    /// <summary>
    /// Clock which can be constructed with an initial instant, and then advanced programmatically.
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
        /// Creates a stub clock initially set to the given instant, with no auto-advance.
        /// </summary>
        public FakeClock(Instant initial) : this(initial, Duration.Zero)
        {            
        }

        /// <summary>
        /// Creates a stub clock initially set to the given instant, with a given level of auto-advance.
        /// </summary>
        public FakeClock(Instant initial, Duration autoAdvance)
        {
            now = initial;
            this.autoAdvance = autoAdvance;
        }

        /// <summary>
        /// Returns a fake clock initially set to midnight of the given year/month/day in UTC in the ISO calendar.
        /// </summary>
        public static FakeClock FromUtc(int year, int month, int dayOfMonth)
        {
            return new FakeClock(Instant.FromUtc(year, month, dayOfMonth, 0, 0));
        }

        /// <summary>
        /// Returns a fake clock initially set to the given year/month/day/time in UTC in the ISO calendar.
        /// </summary>
        public static FakeClock FromUtc(int year, int month, int dayOfMonth, int hour, int minute, int second)
        {
            return new FakeClock(Instant.FromUtc(year, month, dayOfMonth, hour, minute, second));
        }

        /// <summary>
        /// Advances the clock by the given duration.
        /// </summary>
        public void Advance(Duration duration)
        {
            lock (mutex)
            {
                now += duration;
            }
        }

        /// <summary>
        /// Advances the clock by the given number of ticks.
        /// </summary>
        public void AdvanceTicks(long ticks)
        {
            Advance(new Duration(ticks));
        }

        /// <summary>
        /// Advances the clock by the given number of milliseconds.
        /// </summary>
        public void AdvanceMilliseconds(long milliseconds)
        {
            Advance(Duration.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Advances the clock by the given number of seconds.
        /// </summary>
        public void AdvanceSeconds(long seconds)
        {
            Advance(Duration.FromSeconds(seconds));
        }

        /// <summary>
        /// Advances the clock by the given number of minutes.
        /// </summary>
        public void AdvanceMinutes(long minutes)
        {
            Advance(Duration.FromMinutes(minutes));
        }

        /// <summary>
        /// Advances the clock by the given number of hours.
        /// </summary>
        public void AdvanceHours(long hours)
        {
            Advance(Duration.FromHours(hours));
        }

        /// <summary>
        /// Advances the clock by the given number of standard (24-hour) days.
        /// </summary>
        public void AdvanceDays(long days)
        {
            Advance(Duration.FromStandardDays(days));
        }

        /// <summary>
        /// Resets the clock to the given instant.
        /// </summary>
        public void Reset(Instant instant)
        {
            lock (mutex)
            {
                now = instant;
            }
        }

        /// <summary>
        /// Returns the "current time" for this clock. Unlike a normal clock, this
        /// property will return the same value from repeated calls until one of the methods
        /// to change the time is called.
        /// </summary>
        public Instant Now
        {
            get
            {
                lock (mutex)
                {
                    Instant then = now;
                    now += autoAdvance;
                    return then;
                }
            }
        }

        /// <summary>
        /// Amount of time to advance the clock by each time <see cref="Now"/> is fetched.
        /// </summary>
        /// <remarks>
        /// This defaults to zero, in which case the clock doesn't change other than by calls
        /// to <see cref="Reset"/>. The value may be negative, to simulate particularly odd
        /// system clock effects.
        /// </remarks>
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
