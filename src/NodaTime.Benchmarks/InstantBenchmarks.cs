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
using NodaTime.Format;
using NodaTime.Globalization;
using System.Globalization;
using NodaTime.Text;
using NodaTime.Text.Patterns;

namespace NodaTime.Benchmarks
{
    internal class InstantBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.InvariantInfo;
        private static readonly Instant Sample = Instant.FromUtc(2011, 8, 24, 12, 29, 30);
        private static readonly InstantParser InstantParser = new InstantParser();
        private static readonly IParsedPattern<Instant> GeneralParsedPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("g").GetResultOrThrow();
        private static readonly IParsedPattern<Instant> NumberParsedPattern =
            NodaFormatInfo.InvariantInfo.InstantPatternParser.ParsePattern("n").GetResultOrThrow();

        [Benchmark]
        public void TryParseExact_Valid_General_WithNewCode()
        {
            Instant result;
            Instant.TryParseExact("2011-08-21T08:11:30Z", "g", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_General_ParsedPattern()
        {
            Instant result;
            GeneralParsedPattern.Parse("2011-08-21T08:11:30Z").TryGetResult(Instant.MinValue, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_General_WithInstantParser()
        {
            Instant result;
            InstantParser.TryParseExact("2011-08-21T08:11:30Z", "g", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_Numeric_WithNewCode()
        {
            Instant result;
            Instant.TryParseExact("123456789", "n", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_Numeric_ParsedPattern()
        {
            Instant result;
            NumberParsedPattern.Parse("123456789").TryGetResult(Instant.MinValue, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_Numeric_WithInstantParser()
        {
            Instant result;
            InstantParser.TryParseExact("123456789", "n", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidFormat_WithInstantParser()
        {
            Instant result;
            InstantParser.TryParseExact("0", "x", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_General()
        {
            Instant result;
            Instant.TryParseExact("2011-13-21T08:11:30Z", "g", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_Numeric()
        {
            Instant result;
            Instant.TryParseExact("12345xyz", "n", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void FormatN_WithInstantFormat()
        {
            InstantFormat.Format(Sample, "n", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatG_WithInstantFormat()
        {
            InstantFormat.Format(Sample, "g", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatD_WithInstantFormat()
        {
            InstantFormat.Format(Sample, "d", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatN_WithNewCode()
        {
            Sample.ToString("n", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatN_WithParsedPattern()
        {
            NumberParsedPattern.Format(Sample);
        }

        [Benchmark]
        public void FormatG_WithNewCode()
        {
            Sample.ToString("g", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatD_WithNewCode()
        {
            Sample.ToString("d", InvariantFormatInfo);
        }

        [Benchmark]
        public void FormatG_WithParsedPattern()
        {
            GeneralParsedPattern.Format(Sample);
        }
    }
}
