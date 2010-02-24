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
using System.Reflection;

namespace NodaTime.Benchmarks.Timing
{
    /// <summary>
    /// The results of running a single test.
    /// </summary>
    internal class BenchmarkResult
    {
        private const string LongFormatString = "{0}: {1:N0} cps; ({2:N0} iterations in {3:N0} ticks)";
        private const string ShortFormatString = "{0}: {1:N0} cps";
        private readonly MethodInfo method;
        private readonly int iterations;
        private readonly Duration duration;

        internal BenchmarkResult(MethodInfo method,
            int iterations, Duration duration)
        {
            this.method = method;
            this.iterations = iterations;
            this.duration = duration;
        }

        internal MethodInfo Method { get { return method; } }
        internal long Iterations { get { return iterations; } }
        internal Duration Duration { get { return duration; } }
        internal long CallsPerSecond { get { return iterations * NodaConstants.TicksPerSecond / duration.Ticks; } }

        public string ToString(BenchmarkOptions options)
        {
            string formatString = options.DisplayRawData ? LongFormatString : ShortFormatString;
            return string.Format(formatString,
                Method.Name, CallsPerSecond, Iterations, Duration.Ticks);
        }
    }
}
