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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class CachedDateTimeZoneTest
    {
        // PreviousTransition is tricky, as the Info for a period may be wrong for the first
        // tick (or for all other ones, if you're not careful)
        [Test]
        public void PreviousTransition_SucceedsOnTransitionPoint()
        {
            var cached = DateTimeZones.ForId("Europe/Paris");
            var uncached = cached.Uncached();
            Instant summer = Instant.FromUtc(2010, 6, 1, 0, 0);
            Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
            Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick),
                cached.PreviousTransition(nextTransitionTick));
        }

        [Test]
        public void PreviousTransition_SucceedsOffTransitionPoint()
        {
            // This fails with naive caching
            var cached = DateTimeZones.ForId("Europe/Paris");
            var uncached = cached.Uncached();
            Instant summer = Instant.FromUtc(2010, 6, 1, 0, 0);
            Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
            Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick + Duration.One),
                cached.PreviousTransition(nextTransitionTick + Duration.One));
        }
    }
}
