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
namespace NodaTime.TimeZones
{
    /// <summary>
    /// Provides a base class for <see cref="IDateTimeZone"/> implementations.
    /// </summary>
    /// <remarks>
    /// This base is immutable and thread safe. All sub-classes should be as well.
    /// </remarks>
    public abstract class DateTimeZoneBase
        : IDateTimeZone
    {
        private readonly string id;
        private readonly bool isFixed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeZoneBase"/> class.
        /// </summary>
        /// <param name="id">The unique id of this time zone.</param>
        /// <param name="isFixed">Set to <c>true</c> if this time zone has no transitions.</param>
        protected DateTimeZoneBase(string id, bool isFixed)
        {
            this.id = id;
            this.isFixed = isFixed;
        }

        #region IDateTimeZone Members

        /// <summary>
        /// Returns the transition occurring strictly after the specified instant,
        /// or null if there are no further transitions.
        /// </summary>
        /// <param name="instant">The instant after which to consider transitions.</param>
        /// <returns>
        /// The instant of the next transition, or null if there are no further transitions.
        /// </returns>
        public abstract Transition? NextTransition(Instant instant);

        /// <summary>
        /// Returns the transition occurring strictly before the specified instant,
        /// or null if there are no earlier transitions.
        /// </summary>
        /// <param name="instant">The instant before which to consider transitions.</param>
        /// <returns>
        /// The instant of the previous transition, or null if there are no further transitions.
        /// </returns>
        public abstract Transition? PreviousTransition(Instant instant);

        public abstract void Write(DateTimeZoneWriter writer);

        /// <summary>
        /// Returns the offset from UTC, where a positive duration indicates that local time is later
        /// than UTC. In other words, local time = UTC + offset.
        /// </summary>
        /// <param name="instant">The instant for which to calculate the offset.</param>
        /// <returns>
        /// The offset from UTC at the specified instant.
        /// </returns>
        public abstract Offset GetOffsetFromUtc(Instant instant);

        /// <summary>
        /// Returns the offset from local time to UTC, where a positive duration indicates that UTC is earlier
        /// than local time. In other words, UTC = local time - (offset from local).
        /// </summary>
        /// <param name="localInstant">The instant for which to calculate the offset.</param>
        /// <returns>The offset at the specified local time.</returns>
        public virtual Offset GetOffsetFromLocal(LocalInstant localInstant)
        {
            // TODO: Try to find offsets less frequently

            // Find an instant somewhere near the right time by assuming UTC temporarily
            var instant = new Instant(localInstant.Ticks);

            // Find the offset at that instant
            var candidateOffset1 = GetOffsetFromUtc(instant);

            // Adjust localInstant using the estimate, as a guess
            // at the real UTC instant for the local time
            var candidateInstant1 = localInstant - candidateOffset1;
            // Now find the offset at that candidate instant
            var candidateOffset2 = GetOffsetFromUtc(candidateInstant1);

            // If the offsets are the same, we need to check for ambiguous
            // local times.
            if (candidateOffset1 == candidateOffset2)
            {
                // It doesn't matter whether we use instant or candidateInstant1;
                // both are the same side of the next transition (as they have the same offset)
                var nextTransition = NextTransition(candidateInstant1);
                if (nextTransition == null)
                {
                    // No more transitions, so we must be okay
                    return candidateOffset1;
                }
                // Try to apply the offset for the later transition to
                // the local time we were originally given. If the result is
                // after the transition, then it's the correct offset - it means
                // the local time is ambiguous and we want to return the offset
                // leading to the later UTC instant.
                var candidateInstant2 = localInstant - nextTransition.Value.NewOffset;
                return (candidateInstant2 >= nextTransition.Value.Instant) ? nextTransition.Value.NewOffset : candidateOffset1;
            }
            else
            {
                // We know that candidateOffset1 doesn't work from the localInstant;
                // try candidateOffset2 instead. If that works, then all is well,
                // and we've just coped with with a DST transition between
                // instant and candidateInstant1. If it doesn't, we've been
                // given an invalid local time.
                var candidateInstant2 = localInstant - candidateOffset2;
                if (GetOffsetFromUtc(candidateInstant2) == candidateOffset2)
                {
                    return candidateOffset2;
                }
                var laterInstant = candidateInstant1 > candidateInstant2 ? candidateInstant1 : candidateInstant2;
                throw new SkippedTimeException(localInstant, this, PreviousTransition(laterInstant).Value.Instant);
            }
        }

        /// <summary>
        /// Returns the name associated with the given instant.
        /// </summary>
        /// <param name="instant">The instant to get the name for.</param>
        /// <returns>
        /// The name of this time. Never returns null.
        /// </returns>
        /// <remarks>
        /// For a fixed time zone this will always return the same value but for a time zone that
        /// honors daylight savings this will return a different name depending on the time of year
        /// it represents. For example in the Pacific Standard Time (UTC-8) it will return either
        /// PST or PDT depending on the time of year.
        /// </remarks>
        public virtual string Name(Instant instant)
        {
            return Id;
        }

        /// <summary>
        /// The database ID for the time zone.
        /// </summary>
        public string Id { get { return id; } }

        /// <summary>
        /// Indicates whether the time zone is fixed, i.e. contains no transitions.
        /// </summary>
        public bool IsFixed { get { return isFixed; } }

        #endregion

        #region Object overrides

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Id;
        }

        #endregion
    }
}
