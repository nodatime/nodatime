#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009 Jon Skeet
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
        private Duration warmUpTime;
        private Duration testTime;
        private IBenchTimer timer;
        private string typeFilter;
        private string methodFilter;

        private BenchmarkOptions()
        {
        }

        internal Duration WarmUpTime { get { return warmUpTime; } }
        internal Duration TestTime { get { return testTime; } }
        internal IBenchTimer Timer { get { return timer; } }
        internal string TypeFilter { get { return typeFilter; } }
        internal string MethodFilter { get { return methodFilter; } }

        internal static BenchmarkOptions FromCommandLine(string[] args)
        {
            // TODO: Use command line:)
            return new BenchmarkOptions
            {
                typeFilter = args.FirstOrDefault(),
                methodFilter = args.Skip(1).FirstOrDefault(),
                warmUpTime = Duration.StandardSeconds(1),
                testTime = Duration.StandardSeconds(3),
                timer = new WallTimer()
            };
        }
    }
}
