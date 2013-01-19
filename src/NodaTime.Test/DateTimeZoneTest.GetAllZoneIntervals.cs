#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2013 Jon Skeet
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

using System.Linq;
using NodaTime.Testing.TimeZones;
using NUnit.Framework;
using System;


namespace NodaTime.Test
{
    // Tests for DateTimeZoneTest.GetAllZoneIntervals. The calls to ToList()
    // in the assertions are to make the actual values get dumped on failure, instead of
    // just <NodaTime.DateTimeZone+<GetAllZoneIntervalsImpl>d__0>
    public partial class DateTimeZoneTest
    {
        private static readonly SingleTransitionDateTimeZone TestZone = new SingleTransitionDateTimeZone
            (Instant.FromUtc(2000, 1, 1, 0, 0), -3, +4);

        [Test]
        public void GetAllZoneIntervals_EndBeforeStart()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DateTimeZone.Utc.GetAllZoneIntervals(new Instant(100L), new Instant(99L)));
        }

        [Test]
        public void GetAllZoneIntervals_EndEqualToStart()
        {
            CollectionAssert.IsEmpty(DateTimeZone.Utc.GetAllZoneIntervals(new Instant(100L), new Instant(100L)));
        }

        [Test]
        public void GetAllZoneIntervals_FixedZone()
        {
            var zone = DateTimeZone.ForOffset(Offset.FromHours(3));
            var expected = new[] { zone.GetZoneInterval(Instant.MinValue) };
            // Give a reasonably wide interval...
            var actual = zone.GetAllZoneIntervals(Instant.FromUtc(1900, 1, 1, 0, 0), Instant.FromUtc(2100, 1, 1, 0, 0));
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetAllZoneIntervals_SingleTransitionZone_IntervalCoversTransition()
        {
            Instant start = TestZone.Transition - Duration.FromStandardDays(5);
            Instant end = TestZone.Transition + Duration.FromStandardDays(5);
            var expected = new[] { TestZone.EarlyInterval, TestZone.LateInterval };
            var actual = TestZone.GetAllZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetAllZoneIntervals_SingleTransitionZone_IntervalDoesNotCoverTransition()
        {
            Instant start = TestZone.Transition - Duration.FromStandardDays(10);
            Instant end = TestZone.Transition - Duration.FromStandardDays(5);
            var expected = new[] { TestZone.EarlyInterval };
            var actual = TestZone.GetAllZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetAllZoneIntervals_IncludesStart()
        {
            Instant start = TestZone.Transition - Duration.Epsilon;
            Instant end = TestZone.Transition + Duration.FromStandardDays(5);
            var expected = new[] { TestZone.EarlyInterval, TestZone.LateInterval };
            var actual = TestZone.GetAllZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetAllZoneIntervals_ExcludesEnd()
        {
            Instant start = TestZone.Transition - Duration.FromStandardDays(10);
            Instant end = TestZone.Transition;
            var expected = new[] { TestZone.EarlyInterval };
            var actual = TestZone.GetAllZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [Test]
        public void GetAllZoneIntervals_Complex()
        {
            var london = DateTimeZoneProviders.Tzdb["Europe/London"];
            // Transitions are always Spring/Autumn, so June and January should be clear.
            var expected = new[] { 
                london.GetZoneInterval(Instant.FromUtc(1999, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2000, 1, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2000, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2001, 1, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2001, 6, 1, 0, 0)),
                london.GetZoneInterval(Instant.FromUtc(2002, 1, 1, 0, 0)),
            };
            // After the instant we used to fetch the expected zone interval, but that's fine:
            // it'll be the same one, as there's no transition within June.
            var start = Instant.FromUtc(1999, 6, 19, 0, 0);
            var end = Instant.FromUtc(2002, 2, 4, 0, 0);
            var actual = london.GetAllZoneIntervals(start, end);
            CollectionAssert.AreEqual(expected, actual.ToList());
        }
    }
}
