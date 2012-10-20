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

using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Demo
{
    [TestFixture]
    public class TimeZoneDemo
    {
        [Test]
        public void EarlyParis()
        {
            DateTimeZone paris = DateTimeZoneProviders.Tzdb["Europe/Paris"];
            Offset offset = paris.GetUtcOffset(Instant.FromUtc(1900, 1, 1, 0, 0));
            Assert.AreEqual("+00:09:21", offset.ToString());
        }

        [Test]
        public void BritishDoubleSummerTime()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            Offset offset = london.GetUtcOffset(Instant.FromUtc(1942, 7, 1, 0, 0));
            Assert.AreEqual("+02", offset.ToString());
        }

        [Test]
        public void ZoneInterval()
        {
            DateTimeZone london = DateTimeZoneProviders.Tzdb["Europe/London"];
            ZoneInterval interval = london.GetZoneInterval(Instant.FromUtc(2010, 6, 19, 0, 0));
            Assert.AreEqual("BST", interval.Name);
            Assert.AreEqual(Instant.FromUtc(2010, 3, 28, 1, 0), interval.Start);
            Assert.AreEqual(Instant.FromUtc(2010, 10, 31, 1, 0), interval.End);
            Assert.AreEqual(Offset.FromHours(1), interval.WallOffset);
            Assert.AreEqual(Offset.FromHours(1), interval.Savings);
        }
    }
}