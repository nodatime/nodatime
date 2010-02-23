#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2010 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
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
            // TODO: Use command line:)
            return new BenchmarkOptions
            {
                TypeFilter = args.FirstOrDefault(),
                MethodFilter = args.Skip(1).FirstOrDefault(),
                WarmUpTime = Duration.StandardSeconds(1),
                TestTime = Duration.StandardSeconds(3),
                Timer = new WallTimer(),
                DisplayRawData = args.Contains("-rawData")
            };
        }
    }
}
