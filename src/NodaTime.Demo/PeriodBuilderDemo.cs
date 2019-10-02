// Copyright 2019 The Noda Time Authors. All rights reserved.
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
            periodBuilder.Months = 6;
            Period period = periodBuilder.Build();
            Assert.AreEqual(5, period.Years);
            Assert.AreEqual(6, period.Months);
        }

        [Test]
        public void Build()
        {
            PeriodBuilder periodBuilder = new PeriodBuilder()
            {
                Years = 2,
                Months = 3,
                Days = 4
            };
            Period period = Snippet.For(periodBuilder.Build());
            Assert.AreEqual(2, period.Years);
            Assert.AreEqual(3, period.Months);
            Assert.AreEqual(4, period.Days);
        }
    }
}