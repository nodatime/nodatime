// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Diagnostics;

namespace NodaTime.Benchmarks.Framework
{
    /// <summary>
    /// Timer using the built-in stopwatch class; measures wall-clock time.
    /// </summary>
    internal class WallTimer : IBenchTimer
    {
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        public void Reset()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public Duration ElapsedTime { get { return Duration.FromTimeSpan(stopwatch.Elapsed); } }
    }
}