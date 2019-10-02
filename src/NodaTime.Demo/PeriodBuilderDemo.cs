// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class PeriodBuilderDemo
    {
        [Test]
        public void ConstructionFromPeriod()
        {
            Period existingPeriod = Period.FromYears(5);
            PeriodBuilder periodBuilder = Snippet.For(new PeriodBuilder(existingPeriod));
            Period period = periodBuilder.Build();
            Assert.AreEqual(5, period.Years);
        }

        [Test]
        public void Build()
        {
            PeriodBuilder periodBuilder = new PeriodBuilder();
            periodBuilder.Years = 2;
            periodBuilder.Months = 3;
            periodBuilder.Days = 4;
            Period period = Snippet.For(periodBuilder.Build());
            Assert.AreEqual(2, period.Years);
            Assert.AreEqual(3, period.Months);
            Assert.AreEqual(4, period.Days);
        }
    }
}