// Copyright 2019 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;
namespace NodaTime.Demo
{
    class AnnualDateDemo
    {
        [Test]
        public void CompareTo()
        {
            var earlierDate = new AnnualDate(2, 23);
            var laterDate = new AnnualDate(8, 17);

            var lessThan = Snippet.For(earlierDate.CompareTo(laterDate));
            var equal = earlierDate.CompareTo(earlierDate);
            var greaterThan = laterDate.CompareTo(earlierDate);

            Assert.Less(lessThan, 0);
            Assert.AreEqual(0, equal);
            Assert.Greater(greaterThan, 0);
        }

        [Test]
        public void Equals()
        {
            var date1 = new AnnualDate(2, 23);
            var date2 = new AnnualDate(8, 17);

            var unequal = Snippet.For(date1.Equals(date2));
            var equal = date1.Equals(date1);

            Assert.False(unequal);
            Assert.True(equal);
        }

        [Test]
        public void InYear()
        {
            var annualDate = Snippet.For(new AnnualDate(3, 12));
            var localDate = annualDate.InYear(2013);

            Assert.AreEqual(new LocalDate(2013, 3, 12), localDate);
        }

        [Test]
        public void IsValidYear()
        {
            var date = new AnnualDate(2, 29);

            var leapYear = Snippet.For(date.IsValidYear(2020));
            var nonLeapYear = date.IsValidYear(2018);

            Assert.True(leapYear);
            Assert.False(nonLeapYear);
        }
    }
}
