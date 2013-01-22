// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Timer used to measure performance. Implementations
    /// may use wall time, CPU timing etc. Implementations
    /// don't need to be thread-safe.
    /// </summary>
    internal interface IBenchTimer
    {
        void Reset();
        Duration ElapsedTime { get; }
    }
}