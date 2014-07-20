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
        // Use ticks here rather than nanoseconds, as otherwise the multiplication could easily overflow. As an alternative,
        // we could use decimal arithmetic or BigInteger...
        internal long CallsPerSecond { get { return iterations * NodaConstants.TicksPerSecond / duration.Ticks; } }
        internal long NanosecondsPerCall { get { return Duration.ToInt64Nanoseconds() / iterations; } }
    }
}