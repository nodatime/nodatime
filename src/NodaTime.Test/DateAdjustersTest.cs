// Copyright 2014 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using NUnit.Framework;

namespace NodaTime.Test
{
    [TestFixture]
    public class DateAdjustersTest
    {
        [Test]
        public void StartOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 1);
            Assert.AreEqual(end, DateAdjusters.StartOfMonth(start));
        }

        [Test]
        public void EndOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 30);
            Assert.AreEqual(end, DateAdjusters.EndOfMonth(start));
        }

        [Test]
        public void DayOfMonth()
        {
            var start = new LocalDate(2014, 6, 27);
            var end = new LocalDate(2014, 6, 19);
            var adjuster = DateAdjusters.DayOfMonth(19);
            Assert.AreEqual(end, adjuster(start));
        }
    }
}
