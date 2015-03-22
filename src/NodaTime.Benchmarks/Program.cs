// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Minibench.Framework;

namespace NodaTime.Benchmarks
{
    /// <summary>
    /// Entry point for benchmarking.
    /// </summary>
    internal class Program
    {
        private static int Main()
        {
            var run = BenchmarkRunner.RunFromCommandLine(typeof(Program).Assembly);
            return run == null ? 1 : 0;
        }
    }
}
