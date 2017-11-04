// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NUnit.Framework;
using System;

namespace NodaTime.Demo
{
    public class DurationDemo
    {
        [Test]
        public void ConstructionFromDays()
        {
            Duration duration = Snippet.For(Duration.FromDays(5));
            Assert.AreEqual(5, duration.Days);
            Assert.AreEqual("5:00:00:00", duration.ToString());
        }

        [Test]
        public void ConstructionFromHours()
        {
            Duration duration = Snippet.For(Duration.FromHours(1.5));
            Assert.AreEqual(1, duration.Hours);
            Assert.AreEqual(1.5, duration.TotalHours);
            Assert.AreEqual(90, duration.TotalMinutes);
            Assert.AreEqual("0:01:30:00", duration.ToString());
        }

        [Test]
        public void ConstructionFromMinutes()
        {
            Duration duration = Snippet.For(Duration.FromMinutes(50));
            Assert.AreEqual(50, duration.Minutes);
            Assert.AreEqual("0:00:50:00", duration.ToString());
        }

        [Test]
        public void ConstructionFromSeconds()
        {
            Duration duration = Snippet.For(Duration.FromSeconds(42));
            Assert.AreEqual(42, duration.Seconds);
            Assert.AreEqual("0:00:00:42", duration.ToString());
        }

        [Test]
        public void ConstructionFromMilliseconds()
        {
            Duration duration = Snippet.For(Duration.FromMilliseconds(600));
            Assert.AreEqual(600, duration.Milliseconds);
            Assert.AreEqual(0.6, duration.TotalSeconds);
            Assert.AreEqual("0:00:00:00.6", duration.ToString());
        }

        [Test]
        public void ConstructionFromTicks()
        {
            Duration duration = Snippet.For(Duration.FromTicks(10_000_000));
            Assert.AreEqual(10_000_000, duration.TotalTicks);
            Assert.AreEqual(1, duration.TotalSeconds);
            Assert.AreEqual("0:00:00:01", duration.ToString());
        }

        [Test]
        public void ConstructionFromNanoseconds()
        {
            Duration duration = Snippet.For(Duration.FromNanoseconds(1_000_000_000));
            Assert.AreEqual(1_000_000_000, duration.TotalNanoseconds);
            Assert.AreEqual(1, duration.TotalSeconds);
            Assert.AreEqual("0:00:00:01", duration.ToString());
        }

        [Test]
        public void ConstructionFromTimeSpan()
        {
            Duration duration = Snippet.For(Duration.FromTimeSpan(TimeSpan.FromHours(3)));
            Assert.AreEqual(3, duration.Hours);
            Assert.AreEqual("0:03:00:00", duration.ToString());
        }        
    }
}
