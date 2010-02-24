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
            this.summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZones.Utc).ToInstant();
        }

        #endregion

        private CachedDateTimeZone timeZone;
        private Instant summer;

        [Test]
        public void GetPeriodInstant_NotNull()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            Assert.IsNotNull(period);
        }

        [Test]
        public void GetPeriodInstant_RepeatedCallsReturnDifferentObject()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            for (int i = 0; i < CachedDateTimeZone.DefaultCacheSize + 1; i++)
            {
                var instant = Instant.UnixEpoch + Duration.StandardWeeks(52 * i);
                Assert.IsNotNull(this.timeZone.GetPeriod(instant));
            }
            var newPeriod = this.timeZone.GetPeriod(this.summer);
            Assert.AreNotSame(period, newPeriod);
        }

        [Test]
        public void GetPeriodInstant_RepeatedCallsReturnSameObject()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            for (int i = 0; i < 100; i++)
            {
                var newPeriod = this.timeZone.GetPeriod(this.summer);
                Assert.AreSame(period, newPeriod);
            }
        }

        [Test]
        public void GetPeriodInstant_RepeatedCallsReturnSameObjectWithOthersInterspersed()
        {
            var period = this.timeZone.GetPeriod(this.summer);
            Assert.IsNotNull(this.timeZone.GetPeriod(Instant.UnixEpoch));
            Assert.IsNotNull(this.timeZone.GetPeriod(Instant.UnixEpoch + Duration.StandardWeeks(2000)));
            Assert.IsNotNull(this.timeZone.GetPeriod(Instant.UnixEpoch + Duration.StandardWeeks(3000)));
            Assert.IsNotNull(this.timeZone.GetPeriod(Instant.UnixEpoch + Duration.StandardWeeks(4000)));
            var newPeriod = this.timeZone.GetPeriod(this.summer);
            Assert.AreSame(period, newPeriod);
        }
    }
}