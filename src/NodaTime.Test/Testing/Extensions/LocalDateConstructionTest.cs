// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using static NodaTime.Testing.Extensions.LocalDateConstruction;

namespace NodaTime.Test.Testing.Extensions
{
    public class LocalDateConstructionTest
    {
        [Test]
        public void Construction()
        {
            Assert.AreEqual(new LocalDate(2017, 1, 18), 18.January(2017));
            Assert.AreEqual(new LocalDate(2017, 2, 18), 18.February(2017));
            Assert.AreEqual(new LocalDate(2017, 3, 18), 18.March(2017));
            Assert.AreEqual(new LocalDate(2017, 4, 18), 18.April(2017));
            Assert.AreEqual(new LocalDate(2017, 5, 18), 18.May(2017));
            Assert.AreEqual(new LocalDate(2017, 6, 18), 18.June(2017));
            Assert.AreEqual(new LocalDate(2017, 7, 18), 18.July(2017));
            Assert.AreEqual(new LocalDate(2017, 8, 18), 18.August(2017));
            Assert.AreEqual(new LocalDate(2017, 9, 18), 18.September(2017));
            Assert.AreEqual(new LocalDate(2017, 10, 18), 18.October(2017));
            Assert.AreEqual(new LocalDate(2017, 11, 18), 18.November(2017));
            Assert.AreEqual(new LocalDate(2017, 12, 18), 18.December(2017));
        }
    }
}
