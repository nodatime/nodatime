// Copyright 2020 The Noda Time Authors. All rights reserved.
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
            var february23rd = new AnnualDate(2, 23);
            var august17th = new AnnualDate(8, 17);

            var lessThan = Snippet.For(february23rd.CompareTo(august17th));
            var equal = february23rd.CompareTo(february23rd);
            var greaterThan = august17th.CompareTo(february23rd);

            Assert.Less(lessThan, 0);
            Assert.AreEqual(0, equal);
            Assert.Greater(greaterThan, 0);
        }

        [Test]
        public void Equals()
        {
            var february23rd = new AnnualDate(2, 23);
            var august17th = new AnnualDate(8, 17);

            var unequal = Snippet.For(february23rd.Equals(august17th));
            var equal = february23rd.Equals(february23rd);

            Assert.False(unequal);
            Assert.True(equal);
        }

        [Test]
        public void InYear()
        {
            var annualDate = new AnnualDate(3, 12);
            var localDate = Snippet.For(annualDate.InYear(2013));

            Assert.AreEqual(new LocalDate(2013, 3, 12), localDate);
        }

        [Test]
        public void IsValidYear()
        {
            var leapDay = new AnnualDate(2, 29);

            var leapYear = Snippet.For(leapDay.IsValidYear(2020));
            var nonLeapYear = leapDay.IsValidYear(2018);

            Assert.True(leapYear);
            Assert.False(nonLeapYear);
        }
    }
}
