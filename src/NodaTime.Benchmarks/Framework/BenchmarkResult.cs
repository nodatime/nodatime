// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Reflection;

namespace NodaTime.Benchmarks.Framework
{
    /// <summary>
    /// The results of running a single test.
    /// </summary>
    internal class BenchmarkResult
    {
        private const long TicksPerPicosecond = 100 * 1000L;
        private const long TicksPerNanosecond = 100;

        private readonly MethodInfo method;
        private readonly int iterations;
        private readonly Duration duration;

        internal BenchmarkResult(MethodInfo method, int iterations, Duration duration)
        {
            this.method = method;
            this.iterations = iterations;
            this.duration = duration;
        }

        internal MethodInfo Method { get { return method; } }
        internal long Iterations { get { return iterations; } }
        internal Duration Duration { get { return duration; } }
        internal long CallsPerSecond { get { return iterations * NodaConstants.TicksPerSecond / duration.Ticks; } }
        internal long TicksPerCall { get { return Duration.Ticks / iterations; } }
        internal long PicosecondsPerCall { get { return (Duration.Ticks * TicksPerPicosecond) / iterations; } }
        internal long NanosecondsPerCall { get { return (Duration.Ticks * TicksPerNanosecond) / iterations; } }
    }
}