// Copyright 2010 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;

namespace NodaTime.Demo
{
    public class PeriodDemo
    {
        [Test]
        public void ConstructionFromDays()
        {
            Period period = Snippet.For(Period.FromDays(3));
            Assert.AreEqual(3, period.Days);
        }
    }
}