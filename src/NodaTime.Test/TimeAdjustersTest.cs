// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class TimeAdjustersTest
    {
        [Test]
        public void TruncateToSecond()
        {
            var start = new LocalTime(7, 4, 30, 123, 4567);
            var end = new LocalTime(7, 4, 30);
            Assert.AreEqual(end, TimeAdjusters.TruncateToSecond(start));
        }

        [Test]
        public void TruncateToMinute()
        {
            var start = new LocalTime(7, 4, 30, 123, 4567);
            var end = new LocalTime(7, 4, 0);
            Assert.AreEqual(end, TimeAdjusters.TruncateToMinute(start));
        }

        [Test]
        public void TruncateToHour()
        {
            var start = new LocalTime(7, 4, 30, 123, 4567);
            var end = new LocalTime(7, 0, 0);
            Assert.AreEqual(end, TimeAdjusters.TruncateToHour(start));
        }
    }
}
