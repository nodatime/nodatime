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

using NodaTime.TimeZones;
using NUnit.Framework;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class PrecalculatedDateTimeZoneTest
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            var cached = DateTimeZones.ForId("Europe/Paris");
            this.timeZone = cached.Uncached() as PrecalculatedDateTimeZone;
            if (this.timeZone == null)
            {
                Assert.Fail(@"The Europe/Paris time zone does not contain a PrecalculatedDateTimeZone.");
            }
            this.summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
        }

        #endregion

        private PrecalculatedDateTimeZone timeZone;
        private Instant summer;

        [Test]
        public void GetPeriodInstant_Start()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            var start = this.timeZone.GetPeriod(period.Start);
            Assert.AreEqual(period, start);
        }

        [Test]
        public void GetPeriodInstant_End()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            var end = this.timeZone.GetPeriod(period.End);
            Assert.AreEqual(period, end);
        }

        /*
                       // PreviousTransition is tricky, as the Info for a period may be wrong for the first
                       // tick (or for all other ones, if you're not careful)

                       [Test]
                       public void PreviousTransition_SucceedsOffTransitionPoint()
                       {
                           // This fails with naive caching
                           var cached = DateTimeZones.ForId("Europe/Paris");
                           var uncached = cached.Uncached();
                           Instant summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
                           Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
                           Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick + Duration.One),
                                           cached.PreviousTransition(nextTransitionTick + Duration.One));
                       }

                       [Test]
                       public void PreviousTransition_SucceedsOnTransitionPoint()
                       {
                           var cached = DateTimeZones.ForId("Europe/Paris");
                           var uncached = cached.Uncached();
                           Instant summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
                           Instant nextTransitionTick = uncached.NextTransition(summer).Value.Instant;
                           Assert.AreEqual(uncached.PreviousTransition(nextTransitionTick),
                                           cached.PreviousTransition(nextTransitionTick));
                       }
               */
    }
}