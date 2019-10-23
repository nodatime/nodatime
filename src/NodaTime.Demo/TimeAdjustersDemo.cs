// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class TimeAdjustersDemo
    {
        [Test]
        public void TruncateToHour()
        {
            var time = LocalTime.FromMinutesSinceMidnight(63);
            var truncated = Snippet.For(TimeAdjusters.TruncateToHour(time));
            Assert.AreEqual(LocalTime.FromHoursSinceMidnight(1), truncated);
        }

        [Test]
        public void TruncateToMinute()
        {
            var time = LocalTime.FromSecondsSinceMidnight(127);
            var truncated = Snippet.For(TimeAdjusters.TruncateToMinute(time));
            Assert.AreEqual(LocalTime.FromMinutesSinceMidnight(2), truncated);
        }

        [Test]
        public void TruncateToSecond()
        {
            var time = LocalTime.FromMillisecondsSinceMidnight(3042);
            var truncated = Snippet.For(TimeAdjusters.TruncateToSecond(time));
            Assert.AreEqual(LocalTime.FromSecondsSinceMidnight(3), truncated);
        }
    }
}
