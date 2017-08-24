// Copyright 2017 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime.Extensions;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;

namespace NodaTime.Test.Extensions
{
    public class StopwatchExtensionsTest
    {
        [Test]
        public void ElapsedDuration()
        {
            var stopwatch = Stopwatch.StartNew();
            Thread.Sleep(1);
            stopwatch.Stop();
            var duration = stopwatch.ElapsedDuration();
            var timespan = stopwatch.Elapsed;
            Assert.AreEqual(timespan.Ticks, duration.BclCompatibleTicks);
        }
    }
}
