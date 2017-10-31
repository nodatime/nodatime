// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class PeriodDemo
    {
        [Test]
        public void ConstructionFromYears()
        {
            Period period = Snippet.For(Period.FromYears(27));
            Assert.AreEqual(27, period.Years);
            Assert.AreEqual("P27Y", period.ToString());
        }

        [Test]
        public void ConstructionFromMonths()
        {
            Period period = Snippet.For(Period.FromMonths(10));
            Assert.AreEqual(10, period.Months);
            Assert.AreEqual("P10M", period.ToString());
        }

        [Test]
        public void ConstructionFromWeeks()
        {
            Period period = Snippet.For(Period.FromWeeks(1));
            Assert.AreEqual(1, period.Weeks);
            Assert.AreEqual("P1W", period.ToString());
        }

        [Test]
        public void ConstructionFromDays()
        {
            Period period = Snippet.For(Period.FromDays(3));
            Assert.AreEqual(3, period.Days);
            Assert.AreEqual("P3D", period.ToString());
        }

        [Test]
        public void ConstructionFromHours()
        {
            Period period = Snippet.For(Period.FromHours(5));
            Assert.AreEqual(5, period.Hours);
            Assert.AreEqual("PT5H", period.ToString());
        }

        [Test]
        public void ConstructionFromMinutes()
        {
            Period period = Snippet.For(Period.FromMinutes(15));
            Assert.AreEqual(15, period.Minutes);
            Assert.AreEqual("PT15M", period.ToString());
        }

        [Test]
        public void ConstructionFromSeconds()
        {
            Period period = Snippet.For(Period.FromSeconds(70));
            Assert.AreEqual(70, period.Seconds);
            Assert.AreEqual("PT70S", period.ToString());
        }

        [Test]
        public void ConstructionFromMilliseconds()
        {
            Period period = Snippet.For(Period.FromMilliseconds(1500));
            Assert.AreEqual(1500, period.Milliseconds);
            Assert.AreEqual("PT1500s", period.ToString());
        }

        [Test]
        public void ConstructionFromTicks()
        {
            Period period = Snippet.For(Period.FromTicks(42));
            Assert.AreEqual(42, period.Ticks);
            Assert.AreEqual("PT42t", period.ToString());
        }

        [Test]
        public void ConstructionFromNanoseconds()
        {
            Period period = Snippet.For(Period.FromNanoseconds(42));
            Assert.AreEqual(42, period.Nanoseconds);
            Assert.AreEqual("PT42n", period.ToString());
        }
    }
}