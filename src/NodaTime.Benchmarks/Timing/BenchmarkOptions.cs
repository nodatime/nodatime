// Copyright 2009 The Noda Time Authors. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System.Linq;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// Encapsulates all the options for benchmarking, such as
    /// the approximate length of each test, the timer to use
    /// and so on.
    /// </summary>
    internal class BenchmarkOptions
    {
        private BenchmarkOptions()
        {
        }

        internal Duration WarmUpTime { get; private set; }
        internal Duration TestTime { get; private set; }
        internal IBenchTimer Timer { get; private set; }
        internal string TypeFilter { get; private set; }
        internal string MethodFilter { get; private set; }
        internal bool DisplayRawData { get; private set; }

        internal static BenchmarkOptions FromCommandLine(string[] args)
        {
            // TODO(Post-V1): Use command line:)
            return new BenchmarkOptions
                   {
                       TypeFilter = args.FirstOrDefault(),
                       MethodFilter = args.Skip(1).FirstOrDefault(),
                       WarmUpTime = Duration.FromSeconds(1),
                       TestTime = Duration.FromSeconds(10),
                       Timer = new WallTimer(),
                       DisplayRawData = args.Contains("-rawData")
                   };
        }
    }
}