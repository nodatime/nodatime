#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
using System;
using System.Collections.Generic;

namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a <see cref="IDatetimeZone"/> wrapper class that implements a simple cache to speed
    /// up the lookup of transitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// TODO: what is the thread safety?
    /// </para>
    /// <para>
    /// Original name: CachedDateTimeZone
    /// </para>
    /// </remarks>
    public sealed class CachedDateTimeZone
        : IDateTimeZone
    {
        private readonly IDateTimeZone timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedDateTimeZone"/> class.
        /// </summary>
        /// <remarks>
        /// Private constructor to prevent construction. Use <see cref="ForZone"/> method.
        /// </remarks>
        /// <param name="timeZone">The time zone to cache.</param>
        private CachedDateTimeZone(IDateTimeZone timeZone)
        {
            this.timeZone = timeZone;
        }

        /// <summary>
        /// Returns a cached time zone for the given time zone.
        /// </summary>
        /// <remarks>
        /// If the time zone is already cached or it is fixed then it is returned unchanged.
        /// </remarks>
        /// <param name="timeZone">The time zone to cache.</param>
        /// <returns>The cached time zone.</returns>
        public static IDateTimeZone ForZone(IDateTimeZone timeZone)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone", "timeZone cannot be null");
            }
            else if (timeZone is CachedDateTimeZone || timeZone.IsFixed)
            {
                return timeZone;
            }
            return new CachedDateTimeZone(timeZone);
        }

        /// <summary>
        /// Returns true if this time zone is worth caching. Small time zones or time zones with
        /// lots of quick changes do not work well with <see cref="CachedDateTimeZone"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is worth caching; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWorthCaching(IEnumerable<Instant> transitions)
        {
            // Add up all the distances between transitions that are less than
            // about two years.
            double distances = 0;
            int count = 0;

            Instant previous = Instant.MinValue;
            foreach (var transition in transitions)
            {
                if (previous != Instant.MinValue)
                {
                    Duration diff = transition - previous;
                    if (diff.Ticks < ((366L + 365) * 24 * 60 * 60 * 1000))
                    {
                        distances += (double)diff.Ticks;
                        count++;
                    }
                }
                previous = transition;
            }

            if (count > 0)
            {
                double avg = distances / count;
                avg /= 24 * 60 * 60 * 1000;
                if (avg >= 25)
                {
                    // Only bother caching if average distance between
                    // transitions is at least 25 days. Why 25?
                    // CachedDateTimeZone is more efficient if the distance
                    // between transitions is large. With an average of 25, it
                    // will on average perform about 2 tests per cache
                    // hit. (49.7 / 25) is approximately 2.
                    return true;
                }
            }
            return false;
        }

        #region IDateTimeZone Members

        public Instant? NextTransition(Instant instant)
        {
            throw new System.NotImplementedException();
        }

        public Instant? PreviousTransition(Instant instant)
        {
            throw new System.NotImplementedException();
        }

        public Offset GetOffsetFromUtc(Instant instant)
        {
            throw new System.NotImplementedException();
        }

        public Offset GetOffsetFromLocal(LocalInstant instant)
        {
            throw new System.NotImplementedException();
        }

        public string Name(Instant instant)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The database ID for the time zone.
        /// </summary>
        /// <value></value>
        public string Id
        {
            get { return this.timeZone.Id; }
        }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        /// <value></value>
        public bool IsFixed
        {
            get { return this.timeZone.IsFixed; }
        }

        #endregion
    }
}
