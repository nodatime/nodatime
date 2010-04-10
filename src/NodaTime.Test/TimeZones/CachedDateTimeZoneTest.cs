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
    public class CachedDateTimeZoneTest
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            this.timeZone = DateTimeZones.ForId("America/Los_Angeles") as CachedDateTimeZone;
            if (this.timeZone == null)
            {
                Assert.Fail(@"The America/Los_Angeles time zone does not contain a CachedDateTimeZone.");
            }
            this.summer = Instant.FromUtc(2010, 6, 1, 0, 0);
        }

        #endregion

        private CachedDateTimeZone timeZone;
        private Instant summer;

        [Test]
        public void GetZoneIntervalInstant_NotNull()
        {
            var actual = this.timeZone.GetZoneInterval(this.summer);
            Assert.IsNotNull(actual);
        }

        [Test]
        public void GetZoneIntervalInstant_RepeatedCallsReturnDifferentObject()
        {
            var actual = this.timeZone.GetZoneInterval(Instant.UnixEpoch);
            for (int i = 0; i < this.timeZone.CacheSize + 1; i++)
            {
                var instant = Instant.UnixEpoch + Duration.FromStandardWeeks(52 * (i + 1));
                Assert.IsNotNull(this.timeZone.GetZoneInterval(instant));
            }
            var newPeriod = this.timeZone.GetZoneInterval(this.summer);
            Assert.AreNotSame(actual, newPeriod);
        }

        [Test]
        public void GetZoneIntervalInstant_RepeatedCallsReturnSameObject()
        {
            var actual = this.timeZone.GetZoneInterval(this.summer);
            for (int i = 0; i < 100; i++)
            {
                var newPeriod = this.timeZone.GetZoneInterval(this.summer);
                Assert.AreSame(actual, newPeriod);
            }
        }

        [Test]
        public void GetZoneIntervalInstant_RepeatedCallsReturnSameObjectWithOthersInterspersed()
        {
            var actual = this.timeZone.GetZoneInterval(this.summer);
            Assert.IsNotNull(this.timeZone.GetZoneInterval(Instant.UnixEpoch));
            Assert.IsNotNull(this.timeZone.GetZoneInterval(Instant.UnixEpoch + Duration.FromStandardWeeks(2000)));
            Assert.IsNotNull(this.timeZone.GetZoneInterval(Instant.UnixEpoch + Duration.FromStandardWeeks(3000)));
            Assert.IsNotNull(this.timeZone.GetZoneInterval(Instant.UnixEpoch + Duration.FromStandardWeeks(4000)));
            var newPeriod = this.timeZone.GetZoneInterval(this.summer);
            Assert.AreSame(actual, newPeriod);
        }
    }
}