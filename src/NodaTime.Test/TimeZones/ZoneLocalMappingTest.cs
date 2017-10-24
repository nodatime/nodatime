// Copyright 2013 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.TimeZones;
using NUnit.Framework;
using System;

namespace NodaTime.Test.TimeZones
{
    public class ZoneLocalMappingTest
    {
        private static ZoneLocalMapping Mapping
        {
            get
            {
                Offset z = Offset.Zero;
                Instant i = new Instant(0, 0);
                return new ZoneLocalMapping(
                    zone: new FixedDateTimeZone(Offset.Zero), 
                    localDateTime: new LocalDateTime(), 
                    earlyInterval: new ZoneInterval("early", i, i, z, z), 
                    lateInterval: new ZoneInterval("late", i, i, z, z), 
                    count: 100);
            }
        }

        [Test]
        public void ZonedDateTime_Single()
        {            
            Assert.Throws<InvalidOperationException>(() => Mapping.Single());
        }

        [Test]
        public void ZonedDateTime_First()
        {
            Assert.Throws<InvalidOperationException>(() => Mapping.First());
        }

        [Test]
        public void ZonedDateTime_Last()
        {
            Assert.Throws<InvalidOperationException>(() => Mapping.Last());
        }
    }
}
