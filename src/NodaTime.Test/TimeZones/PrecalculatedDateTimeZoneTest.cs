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
            var cached = DateTimeZone.ForId("Europe/Paris");
            timeZone = cached.Uncached() as PrecalculatedDateTimeZone;
            if (timeZone == null)
            {
                Assert.Fail("The Europe/Paris time zone does not contain a PrecalculatedDateTimeZone.");
            }
            summer = new ZonedDateTime(2010, 6, 1, 0, 0, 0, DateTimeZone.Utc).ToInstant();
        }
        #endregion

        private PrecalculatedDateTimeZone timeZone;
        private Instant summer;

        [Test]
        public void GetZoneIntervalInstant_End()
        {
            var expected = timeZone.GetZoneInterval(summer);
            var actual = timeZone.GetZoneInterval(expected.End - Duration.One);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetZoneIntervalInstant_Start()
        {
            var expected = timeZone.GetZoneInterval(summer);
            var actual = timeZone.GetZoneInterval(expected.Start);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Validation_EmptyPeriodArray()
        {
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(new ZoneInterval[0]));
        }

        [Test]
        public void Validation_BadFirstStartingPoint()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", new Instant(10), new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), new Instant(30), Offset.Zero, Offset.Zero),                                       
            };
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(intervals));
        }

        [Test]
        public void Validation_NonAdjoiningIntervals()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(25), new Instant(30), Offset.Zero, Offset.Zero),                                       
            };
            Assert.Throws<ArgumentException>(() => PrecalculatedDateTimeZone.ValidatePeriods(intervals));
        }

        [Test]
        public void Validation_Success()
        {
            ZoneInterval[] intervals =
            {
                new ZoneInterval("foo", Instant.MinValue, new Instant(20), Offset.Zero, Offset.Zero),
                new ZoneInterval("foo", new Instant(20), new Instant(30), Offset.Zero, Offset.Zero),                                       
                new ZoneInterval("foo", new Instant(30), new Instant(100), Offset.Zero, Offset.Zero),                                       
                new ZoneInterval("foo", new Instant(100), new Instant(200), Offset.Zero, Offset.Zero),                                       
            };
            PrecalculatedDateTimeZone.ValidatePeriods(intervals);
        }
    }
}