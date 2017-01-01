// Copyright 2016 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using static NodaTime.Testing.Extensions.DurationConstruction;

namespace NodaTime.Test.Testing.Extensions
{
    public class DurationConstructionTest
    {
        [Test]
        public void Days()
        {
            Assert.AreEqual(Duration.FromDays(2), 2.Days());
            Assert.AreEqual(Duration.FromDays(2.5), 2.5.Days());
        }

        [Test]
        public void Hours()
        {
            Assert.AreEqual(Duration.FromHours(2), 2.Hours());
            Assert.AreEqual(Duration.FromHours(2.5), 2.5.Hours());
        }

        [Test]
        public void Minutes()
        {
            Assert.AreEqual(Duration.FromMinutes(2), 2.Minutes());
            Assert.AreEqual(Duration.FromMinutes(2), 2L.Minutes());
            Assert.AreEqual(Duration.FromMinutes(2.5), 2.5.Minutes());
        }

        [Test]
        public void Seconds()
        {
            Assert.AreEqual(Duration.FromSeconds(2), 2.Seconds());
            Assert.AreEqual(Duration.FromSeconds(2), 2L.Seconds());
            Assert.AreEqual(Duration.FromSeconds(2.5), 2.5.Seconds());
        }

        [Test]
        public void Milliseconds()
        {
            Assert.AreEqual(Duration.FromMilliseconds(2), 2.Milliseconds());
            Assert.AreEqual(Duration.FromMilliseconds(2), 2L.Milliseconds());
            Assert.AreEqual(Duration.FromMilliseconds(2.5), 2.5.Milliseconds());
        }

        [Test]
        public void Ticks()
        {
            Assert.AreEqual(Duration.FromTicks(2), 2.Ticks());
            Assert.AreEqual(Duration.FromTicks(2), 2L.Ticks());
            Assert.AreEqual(Duration.FromTicks(2.5), 2.5.Ticks());
        }

        [Test]
        public void Nanoseconds()
        {
            Assert.AreEqual(Duration.FromNanoseconds(2), 2.Nanoseconds());
            Assert.AreEqual(Duration.FromNanoseconds(2), 2L.Nanoseconds());
            Assert.AreEqual(Duration.FromNanoseconds(2.5), 2.5.Nanoseconds());
        }
    }
}
