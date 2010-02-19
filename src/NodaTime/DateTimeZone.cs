#region Copyright and license information

// Copyright 2001-2010 Stephen Colebourne
// Copyright 2010 Jon Skeet
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

namespace NodaTime
{
    /// <summary>
    ///    Provides a set of
    ///    <c>static</c>
    ///    (
    ///    <c>Shared</c>
    ///    in Visual Basic) methods for querying objects that implement
    ///    <see cref="IDateTimeZone"/>
    ///    .
    /// </summary>
    public static class DateTimeZone
    {
        /// <summary>
        /// Gets the offset from to subtract from a local time to get the UTC time.
        /// </summary>
        /// <param name="timeZone">The time zone to use.</param>
        /// <param name="localInstant">The local instant to get the offset of.</param>
        /// <returns>The offset to subtract from the specified local time to obtain a UTC instant.</returns>
        /// <remarks>
        /// Around a DST transition, local times behave peculiarly. When
        /// the time springs forward, (e.g. 12:59 to 02:00) some times never
        /// occur; when the time falls back (e.g. 1:59 to 01:00) some times
        /// occur twice. This method always returns a smaller offset when
        /// there is ambiguity, i.e. it treats the local time as the later
        /// of the possibilities. Currently for an impossible local time
        /// it will return the offset corresponding to a later instant;
        /// in the (near) future it is anticipated that an exception will be
        /// thrown instead.
        /// </remarks>
        public static Offset GetOffsetFromLocal(IDateTimeZone timeZone, LocalInstant localInstant)
        {
            if (timeZone == null)
            {
                throw new ArgumentNullException("timeZone");
            }
            // Find an instant somewhere near the right time by assuming UTC temporarily
            var instant = new Instant(localInstant.Ticks);

            // Find the offset at that instant
            var candidateOffset1 = timeZone.GetOffsetFromUtc(instant);

            // Adjust localInstant using the estimate, as a guess
            // at the real UTC instant for the local time
            var candidateInstant1 = localInstant - candidateOffset1;
            // Now find the offset at that candidate instant
            var candidateOffset2 = timeZone.GetOffsetFromUtc(candidateInstant1);

            // If the offsets are the same, we need to check for ambiguous
            // local times.
            if (candidateOffset1 == candidateOffset2)
            {
                // It doesn't matter whether we use instant or candidateInstant1;
                // both are the same side of the next transition (as they have the same offset)
                var nextTransition = timeZone.NextTransition(candidateInstant1);
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
                var candidateOffset3 = timeZone.GetOffsetFromUtc(nextTransition.Value);
                var candidateInstant2 = localInstant - candidateOffset3;
                return (candidateInstant2 >= nextTransition.Value) ? candidateOffset3 : candidateOffset1;
            }
            else
            {
                // We know that candidateOffset1 doesn't work from the localInstant;
                // try candidateOffset2 instead. If that works, then all is well,
                // and we've just coped with with a DST transition between
                // instant and candidateInstant1. If it doesn't, we've been
                // given an invalid local time.
                var candidateInstant2 = localInstant - candidateOffset2;
                if (timeZone.GetOffsetFromUtc(candidateInstant2) == candidateOffset2)
                {
                    return candidateOffset2;
                }
                var laterInstant = candidateInstant1 > candidateInstant2 ? candidateInstant1 : candidateInstant2;
                throw new SkippedTimeException(localInstant, timeZone, timeZone.PreviousTransition(laterInstant).Value);
            }
        }
    }
}