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

#if !PCL

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using NodaTime.TimeZones;

namespace NodaTime.Test.TimeZones
{
    [TestFixture]
    public class BclDateTimeZoneTest
    {
#pragma warning disable 0414 // Used by tests via reflection - do not remove!
        // This test is effectively disabled on Mono as its time zone support is broken in the current
        // stable release - see http://code.google.com/p/noda-time/issues/detail?id=97
        private static readonly ReadOnlyCollection<TimeZoneInfo> BclZonesOrEmptyOnMono = TestHelper.IsRunningOnMono
            ? new List<TimeZoneInfo>().AsReadOnly() : TimeZoneInfo.GetSystemTimeZones();
#pragma warning restore 0414

        [Test]
        [TestCaseSource("BclZonesOrEmptyOnMono")]
        public void AllZoneTransitions(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1800, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(2050, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant - Duration.Epsilon, nodaZone, windowsZone);
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                instant = nodaZone.GetZoneInterval(instant).End;
            }
        }

        /// <summary>
        /// This test catches situations where the Noda Time representation doesn't have all the
        /// transitions it should; AllZoneTransitions may pass not spot times when we *should* have
        /// a transition, because it only uses the transitions it knows about. Instead, here we
        /// check each day between 1st January 1950 and 1st January 2050. We use midnight UTC, but
        /// this is arbitrary. The choice of checking once a week is just practical - it's a relatively
        /// slow test, mostly because TimeZoneInfo is slow.
        /// </summary>
        [Test]
        [TestCaseSource("BclZonesOrEmptyOnMono")]
        public void AllZonesEveryWeek(TimeZoneInfo windowsZone)
        {
            ValidateZoneEveryWeek(windowsZone);
        }

        // This demonstrates bug 115.
        [Test]
        public void Namibia()
        {
            String bclId = "Namibia Standard Time";
            try
            {
                ValidateZoneEveryWeek(TimeZoneInfo.FindSystemTimeZoneById(bclId));
            }
            catch (TimeZoneNotFoundException)
            {
                // This may occur on Mono, for example.
                Assert.Ignore("Test assumes existence of BCL zone with ID: " + bclId);
            }
        }

        private void ValidateZoneEveryWeek(TimeZoneInfo windowsZone)
        {
            var nodaZone = BclDateTimeZone.FromTimeZoneInfo(windowsZone);

            Instant instant = Instant.FromUtc(1950, 1, 1, 0, 0);
            Instant end = Instant.FromUtc(2050, 1, 1, 0, 0);

            while (instant < end)
            {
                ValidateZoneEquality(instant, nodaZone, windowsZone);
                instant += Duration.OneStandardWeek;
            }
        }

        [Test]
        public void ForSystemDefault()
        {
            // Assume that the local time zone doesn't change between two calls...
            TimeZoneInfo local = TimeZoneInfo.Local;
            BclDateTimeZone nodaLocal1 = BclDateTimeZone.ForSystemDefault();
            BclDateTimeZone nodaLocal2 = BclDateTimeZone.ForSystemDefault();
            // Check it's actually the right zone
            Assert.AreSame(local, nodaLocal1.OriginalZone);
            // Check it's cached
            Assert.AreSame(nodaLocal1, nodaLocal2);
        }

        private void ValidateZoneEquality(Instant instant, DateTimeZone nodaZone, TimeZoneInfo windowsZone)
        {
            var interval = nodaZone.GetZoneInterval(instant);

            // Check that the zone interval really represents a transition.
            if (interval.Start != Instant.MinValue)
            {
                Assert.AreNotEqual(interval.WallOffset, nodaZone.GetUtcOffset(interval.Start - Duration.Epsilon));
            }
            var nodaOffset = interval.WallOffset;
            var windowsOffset = windowsZone.GetUtcOffset(instant.ToDateTimeUtc());
            Assert.AreEqual(windowsOffset, nodaOffset.ToTimeSpan(), "Incorrect offset at " + instant + " in interval " + interval);
        }
    }
}

#endif
