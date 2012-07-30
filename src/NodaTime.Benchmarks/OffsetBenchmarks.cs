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

using System.Globalization;
using NodaTime.Benchmarks.Timing;
using NodaTime.Globalization;
using NodaTime.Text;

namespace NodaTime.Benchmarks
{
    internal class OffsetBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.GetFormatInfo(CultureInfo.InvariantCulture);
        private static readonly Offset SampleOffset = Offset.FromHoursAndMinutes(12, 34);

        private readonly IPattern<Offset> offsetPattern;
        private readonly OffsetPatternParser offsetPatternParser;

        public OffsetBenchmarks()
        {
            offsetPatternParser = new OffsetPatternParser();
            var parseResult = offsetPatternParser.ParsePattern("HH:mm", InvariantFormatInfo);
            offsetPattern = parseResult.GetResultOrThrow();
        }

        [Benchmark]
        public void ParseExactIncludingPreparse_Valid()
        {
            var parsePatternResult = offsetPatternParser.ParsePattern("HH:mm", InvariantFormatInfo);
            var pattern = parsePatternResult.GetResultOrThrow();
            Offset result;
            ParseResult<Offset> parseResult = pattern.Parse("12:34");
            parseResult.TryGetValue(default(Offset), out result);
        }

        [Benchmark]
        public void PreparsedParseExact_Valid()
        {
            Offset result;
            ParseResult<Offset> parseResult = offsetPattern.Parse("12:34");
            parseResult.TryGetValue(default(Offset), out result);
        }

        [Benchmark]
        public void ParsePattern_Invalid()
        {
            offsetPatternParser.ParsePattern("hh:mm", InvariantFormatInfo);
        }

        [Benchmark]
        public void ParsePattern_Valid()
        {
            offsetPatternParser.ParsePattern("HH:mm", InvariantFormatInfo);
        }

        [Benchmark]
        public void PreparedParseExact_InvalidValue()
        {
            Offset result;
            ParseResult<Offset> parseResult = offsetPattern.Parse("123:45");
            parseResult.TryGetValue(default(Offset), out result);
        }

        [Benchmark]
        public void ToString_ExplicitFormat()
        {
            SampleOffset.ToString("HH:mm", InvariantFormatInfo);
        }

        [Benchmark]
        public void ParsedPattern_Format()
        {
            offsetPattern.Format(SampleOffset);
        }
    }
}
