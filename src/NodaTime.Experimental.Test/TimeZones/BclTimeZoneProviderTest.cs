#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2012 Jon Skeet
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
using System.Collections.ObjectModel;
using NUnit.Framework;
using NodaTime.Experimental.TimeZones;

namespace NodaTime.Experimental.Test.TimeZones
{
    [TestFixture]
    public class WindowsTimeZoneProviderTest
    {
        private static readonly ReadOnlyCollection<TimeZoneInfo> WindowsSystemZones = TimeZoneInfo.GetSystemTimeZones();
        private static readonly WindowsTimeZoneProvider ZoneProvider = new WindowsTimeZoneProvider();

        [Test]
        [TestCaseSource("WindowsSystemZones")]
        public void AllZoneTransitions(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1800, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(2050, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                ValidateZoneEquality(instant - Duration.One, nodaZone, windowsZone);
                instant = nodaZone.GetZoneInterval(instant).End;
            }
        }

        private void ValidateZoneEquality(Instant instant, DateTimeZone nodaZone, TimeZoneInfo windowsZone)
        {
            var interval = nodaZone.GetZoneInterval(instant);
            var nodaOffset = nodaZone.GetOffsetFromUtc(instant);
            var windowsOffset = windowsZone.GetUtcOffset(instant.ToDateTimeUtc());
            Assert.AreEqual(windowsOffset, nodaOffset.ToTimeSpan(), "Incorrect offset at " + instant + " in interval " + interval);
        }
    }
}
