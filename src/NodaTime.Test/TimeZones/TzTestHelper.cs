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

using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    /// <summary>
    /// Extension methods to help with time zone testing, and other helper methods.
    /// </summary>
    internal static class TzTestHelper
    {
        /// <summary>
        /// Returns the uncached version of the given zone. If the zone isn't
        /// an instance of CachedDateTimeZone, the same reference is returned back.
        /// </summary>
        internal static IDateTimeZone Uncached(this IDateTimeZone zone)
        {
            CachedDateTimeZone cached = zone as CachedDateTimeZone;
            return cached == null ? zone : cached.TimeZone;
        }

        /*
        internal static Transition ValidateNextTransition(this IDateTimeZone zone, Instant instant)
        {
            Transition? transition = zone.NextTransition(instant);
            return transition.Validate(zone);
        }

        internal static Transition ValidatePreviousTransition(this IDateTimeZone zone, Instant instant)
        {
            Transition? transition = zone.PreviousTransition(instant);
            return transition.Validate(zone);
        }

        /// <summary>
        /// Convenience method which puts a transition through its paces. Apply liberally
        /// to any transition you don't expect to be null.
        /// </summary>
        internal static Transition Validate(this Transition? nullableTransition, IDateTimeZone zone)
        {
            Assert.IsNotNull(nullableTransition);
            Transition transition = nullableTransition.Value;
            Assert.AreEqual(transition.NewOffset, zone.GetOffsetFromUtc(transition.Instant));
            Assert.AreEqual(transition.OldOffset, zone.GetOffsetFromUtc(transition.Instant - Duration.One));

            Instant instant = transition.Instant;

            if (instant == Instant.MinValue)
            {
                Assert.IsNull(zone.PreviousTransition(instant));
            }
            else
            {
                Assert.AreEqual(transition, zone.NextTransition(instant - Duration.One));
                Assert.AreEqual(zone.PreviousTransition(instant),
                    zone.PreviousTransition(instant - Duration.One));
            }

            if (instant == Instant.MaxValue)
            {
                Assert.IsNull(zone.NextTransition(instant));
            }
            else
            {
                Assert.AreEqual(transition, zone.PreviousTransition(instant + Duration.One));
                Assert.AreEqual(zone.NextTransition(instant),
                    zone.NextTransition(instant + Duration.One));
            }

            return transition;
        }
        */
    }
}