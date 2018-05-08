// Copyright 2015 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Test.Text;
using NUnit.Framework;
using System;

namespace NodaTime.Test
{
    public class AnnualDateTest
    {
        [Test]
        public void Feb29()
        {
            var date = new AnnualDate(2, 29);
            Assert.AreEqual(29, date.Day);
            Assert.AreEqual(2, date.Month);
            Assert.AreEqual(new LocalDate(2016, 2, 29), date.InYear(2016));
            Assert.IsTrue(date.IsValidYear(2016));
            Assert.AreEqual(new LocalDate(2015, 2, 28), date.InYear(2015));
            Assert.IsFalse(date.IsValidYear(2015));
        }

        [Test]
        public void June19()
        {
            var date = new AnnualDate(6, 19);
            Assert.AreEqual(19, date.Day);
            Assert.AreEqual(6, date.Month);
            Assert.AreEqual(new LocalDate(2016, 6, 19), date.InYear(2016));
            Assert.IsTrue(date.IsValidYear(2016));
            Assert.AreEqual(new LocalDate(2015, 6, 19), date.InYear(2015));
            Assert.IsTrue(date.IsValidYear(2015));
        }

        [Test]
        public void Validation()
        {
            // Feb 30th is invalid, but January 30th is fine
            Assert.Throws<ArgumentOutOfRangeException>(() => new AnnualDate(2, 30));
            new AnnualDate(1, 30).Consume();

            // 13th month is invalid
            Assert.Throws<ArgumentOutOfRangeException>(() => new AnnualDate(13, 1));
        }

        [Test]
        public void Equality()
        {
            TestHelper.TestEqualsStruct(new AnnualDate(3, 15), new AnnualDate(3, 15), new AnnualDate(4, 15), new AnnualDate(3, 16));
        }

        [Test]
        public void DefaultValueIsJanuary1st()
        {
            Assert.AreEqual(new AnnualDate(1, 1), new AnnualDate());
        }

        [Test]
        public void Comparision()
        {
            TestHelper.TestCompareToStruct(new AnnualDate(6, 19), new AnnualDate(6, 19), new AnnualDate(6, 20), new AnnualDate(7, 1));
        }

        [Test]
        public void Operators()
        {
            TestHelper.TestOperatorComparisonEquality(new AnnualDate(6, 19), new AnnualDate(6, 19), new AnnualDate(6, 20), new AnnualDate(7, 1));
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("02-01", new AnnualDate(2, 1).ToString());
            Assert.AreEqual("02-10", new AnnualDate(2, 10).ToString());
            Assert.AreEqual("12-01", new AnnualDate(12, 1).ToString());
            Assert.AreEqual("12-20", new AnnualDate(12, 20).ToString());
        }

        [Test]
        public void ToString_WithFormat()
        {
            Assert.AreEqual("02-01", new AnnualDate(2, 1).ToString("G", Cultures.FrFr));
            Assert.AreEqual("02-01", new AnnualDate(2, 1).ToString(null, Cultures.FrFr));
            Assert.AreEqual("02/01", new AnnualDate(2, 1).ToString("MM/dd", Cultures.FrFr));
            Assert.AreEqual("02-01", new AnnualDate(2, 1).ToString("MM/dd", Cultures.FrCa));
        }
    }
}
