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
    internal class OffsetBenchmarks
    {
        private static readonly NodaFormatInfo InvariantFormatInfo = NodaFormatInfo.GetFormatInfo(CultureInfo.InvariantCulture);
        private static readonly Offset SampleOffset = Offset.Create(12, 34, 0, 0);
        private static readonly OffsetParser OldParser = new OffsetParser();

        private readonly IParsedPattern<Offset> offsetPattern;
        private readonly OffsetPatternParser offsetPatternParser;

        public OffsetBenchmarks()
        {
            offsetPatternParser = new OffsetPatternParser();
            var parseResult = offsetPatternParser.ParsePattern("HH:mm", InvariantFormatInfo);
            offsetPattern = parseResult.GetResultOrThrow();
        }
        
        [Benchmark]
        public void TryParseExact_Valid_Old()
        {
            Offset result;
            OldParser.TryParseExact("12:34", "HH:mm", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidFormat_Old()
        {
            Offset result;
            OldParser.TryParseExact("12:34", "hh:mm", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_Old()
        {
            Offset result;
            OldParser.TryParseExact("123:45", "HH:mm", InvariantFormatInfo, DateTimeParseStyles.None, out result);
        }

        [Benchmark]
        public void TryParseExact_Valid_New()
        {
            Offset result;
            Offset.TryParseExact("12:34", "HH:mm", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidFormat_New()
        {
            Offset result;
            Offset.TryParseExact("12:34", "hh:mm", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void TryParseExact_InvalidValue_New()
        {
            Offset result;
            Offset.TryParseExact("123:45", "HH:mm", InvariantFormatInfo, out result);
        }

        [Benchmark]
        public void ParseExactIncludingPreparse_Valid()
        {
            var parsePatternResult = offsetPatternParser.ParsePattern("HH:mm", InvariantFormatInfo);
            var pattern = parsePatternResult.GetResultOrThrow();
            Offset result;
            NodaTime.Text.ParseResult<Offset> parseResult = pattern.Parse("12:34");
            parseResult.TryGetResult(default(Offset), out result);
        }

        [Benchmark]
        public void PreparsedParseExact_Valid()
        {
            Offset result;
            NodaTime.Text.ParseResult<Offset> parseResult = offsetPattern.Parse("12:34");
            parseResult.TryGetResult(default(Offset), out result);
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
            NodaTime.Text.ParseResult<Offset> parseResult = offsetPattern.Parse("123:45");
            parseResult.TryGetResult(default(Offset), out result);
        }

        [Benchmark]
        public void ToString_ExplicitFormat_Old()
        {
            OffsetFormat.Format(SampleOffset, "HH:mm", InvariantFormatInfo);
        }

        [Benchmark]
        public void ToString_ExplicitFormat_New()
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
