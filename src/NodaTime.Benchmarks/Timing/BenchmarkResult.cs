// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Reflection;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// The results of running a single test.
    /// </summary>
    internal class BenchmarkResult
    {
        private const string LongFormatString = "{0}: {1:N0} cps; ({2:N0} iterations in {3:N0} ticks; {4:N0} ticks per iteration)";
        private const string ShortFormatString = "{0}: {1:N0} cps ({4:N0} tpc)";
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

        public string ToString(BenchmarkOptions options)
        {
            if (Duration == Duration.Zero)
            {
                return string.Format("Invalid result: duration was 0 ({0} iterations)", Iterations);
            }
            string formatString = options.DisplayRawData ? LongFormatString : ShortFormatString;
            return string.Format(formatString, Method.Name, CallsPerSecond, Iterations, Duration.Ticks, TicksPerCall);
        }
    }
}