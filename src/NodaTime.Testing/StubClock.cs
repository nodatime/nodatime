#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
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
    public sealed class StubClock : IClock
    {
        private Instant now;

        public StubClock(Instant initial)
        {
            now = initial;
        }

        /// <summary>
        /// Returns a fake clock initially set to midnight of the given year/month/day in UTC in the ISO calendar.
        /// </summary>
        public static StubClock FromUtc(int year, int month, int dayOfMonth)
        {
            return new StubClock(Instant.FromUtc(year, month, dayOfMonth, 0, 0));
        }

        /// <summary>
        /// Returns a fake clock initially set to the given year/month/day/time in UTC in the ISO calendar.
        /// </summary>
        public static StubClock FromUtc(int year, int month, int dayOfMonth, int hour, int minute, int second)
        {
            return new StubClock(Instant.FromUtc(year, month, dayOfMonth, hour, minute, second));
        }

        /// <summary>
        /// Advances the clock by the given duration.
        /// </summary>
        public void Advance(Duration duration)
        {
            now += duration;
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
            now += Duration.FromHours(hours);
        }

        /// <summary>
        /// Advances the clock by the given number of standard (24-hour) days.
        /// </summary>
        public void AdvanceDays(long days)
        {
            now += Duration.FromStandardDays(days);
        }

        /// <summary>
        /// Resets the clock to the given instant.
        /// </summary>
        public void Reset(Instant instant)
        {
            now = instant;
        }

        /// <summary>
        /// Returns the "current time" for this clock. Unlike a normal clock, this
        /// property will return the same value from repeated calls until one of the methods
        /// to change the time is called.
        /// </summary>
        public Instant Now
        {
            get { return now; }
        }
    }
}
