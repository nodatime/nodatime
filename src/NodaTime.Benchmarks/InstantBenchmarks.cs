#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
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

using NodaTime.Benchmarks.Timing;
using NodaTime.Globalization;
using NodaTime.Text;

namespace NodaTime.Benchmarks
{
    internal class InstantBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.InvariantInfo;
        private static readonly Instant Sample = Instant.FromUtc(2011, 8, 24, 12, 29, 30);
        private static readonly IPattern<Instant> GeneralPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("g").GetResultOrThrow();
        private static readonly IPattern<Instant> NumberPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("n").GetResultOrThrow();

        [Benchmark]
        public void FormatN()
        {
            Sample.ToString("n", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatN_WithParsedPattern()
        {
            NumberPattern.Format(Sample);
        }

        [Benchmark]
        public void FormatG()
        {
            Sample.ToString("g", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatD()
        {
            Sample.ToString("d", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatG_WithParsedPattern()
        {
            GeneralPattern.Format(Sample);
        }
    }
}
